namespace Smart.Data.Mapper;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

[DebuggerDisplay("{" + nameof(Diagnostics) + "}")]
internal sealed class ResultMapperCache
{
    private static readonly Node EmptyNode = new(typeof(EmptyKey), [], default!, 0);

    private const int InitialSize = 64;

    private const int Factor = 3;

#if NET9_0_OR_GREATER
    private readonly Lock sync = new();
#else
    private readonly object sync = new();
#endif

    private volatile Node[] nodes;

    private int depth;

    private int count;

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public ResultMapperCache()
    {
        nodes = CreateInitialTable();
    }

    //--------------------------------------------------------------------------------
    // Private
    //--------------------------------------------------------------------------------

    private static int CalculateDepth(Node node)
    {
        var length = 1;
        var next = node.Next;
        while (next is not null)
        {
            length++;
            next = next.Next;
        }

        return length;
    }

    private static int CalculateDepth(Node[] targetNodes)
    {
        var depth = 0;

        for (var i = 0; i < targetNodes.Length; i++)
        {
            var node = targetNodes[i];
            if (node != EmptyNode)
            {
                depth = Math.Max(CalculateDepth(node), depth);
            }
        }

        return depth;
    }

    private static int CalculateSize(int requestSize)
    {
        return (int)BitOperations.RoundUpToPowerOf2((uint)requestSize);
    }

    private static int CalculateCount(Node[] targetNodes)
    {
        var count = 0;
        for (var i = 0; i < targetNodes.Length; i++)
        {
            var node = targetNodes[i];
            if (node != EmptyNode)
            {
                do
                {
                    count++;
                    node = node.Next;
                }
                while (node is not null);
            }
        }

        return count;
    }

    private static Node[] CreateInitialTable()
    {
        var newNodes = new Node[InitialSize];
        newNodes.AsSpan().Fill(EmptyNode);
        return newNodes;
    }

    private static Node FindLastNode(Node node)
    {
        while (node.Next is not null)
        {
            node = node.Next;
        }

        return node;
    }

    private static void UpdateLink(ref Node node, Node addNode)
    {
        if (node == EmptyNode)
        {
            node = addNode;
        }
        else
        {
            var last = FindLastNode(node);
            last.Next = addNode;
        }
    }

    private static void RelocateNodes(Node[] newNodes, Node[] oldNodes, int count)
    {
        var remaining = count;
        for (var i = 0; (i < oldNodes.Length) && (remaining > 0); i++)
        {
            var current = oldNodes[i];
            if (current == EmptyNode)
            {
                continue;
            }

            do
            {
                UpdateLink(ref newNodes[current.Hash & (newNodes.Length - 1)], new Node(current.TargetType, current.Columns, current.Hash, current.Value));

                current = current.Next;
                remaining--;
            }
            while (current is not null);
        }
    }

    private void AddNode(Node node)
    {
        var currentNodes = nodes;

        var requestSize = Math.Max(InitialSize, (count + 1) * Factor);
        var size = CalculateSize(requestSize);
        if (size > currentNodes.Length)
        {
            var newNodes = new Node[size];
            newNodes.AsSpan().Fill(EmptyNode);

            RelocateNodes(newNodes, currentNodes, count);

            UpdateLink(ref newNodes[node.Hash & (newNodes.Length - 1)], node);

            nodes = newNodes;
            depth = CalculateDepth(newNodes);
            count = CalculateCount(newNodes);
        }
        else
        {
            Interlocked.MemoryBarrier();

            UpdateLink(ref currentNodes[node.Hash & (currentNodes.Length - 1)], node);

            depth = Math.Max(CalculateDepth(currentNodes[node.Hash & (currentNodes.Length - 1)]), depth);
            count++;
        }
    }

    //--------------------------------------------------------------------------------
    // Public
    //--------------------------------------------------------------------------------

    public DiagnosticsInfo Diagnostics
    {
        get
        {
            lock (sync)
            {
                return new DiagnosticsInfo(nodes.Length, depth, count);
            }
        }
    }

    public void Clear()
    {
        lock (sync)
        {
            var newNodes = CreateInitialTable();

            Interlocked.MemoryBarrier();

            nodes = newNodes;
            depth = 0;
            count = 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool IsMatchColumn(ReadOnlySpan<ColumnInfo> cached, ReadOnlySpan<ColumnInfo> current)
    {
        if (cached.Length != current.Length)
        {
            return false;
        }

        for (var i = 0; i < cached.Length; i++)
        {
            ref readonly var column1 = ref cached[i];
            ref readonly var column2 = ref current[i];

            if ((column1.Type != column2.Type) || !String.Equals(column1.Name, column2.Name, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGetValue(Type targetType, ReadOnlySpan<ColumnInfo> columns, int hash, [NotNullWhen(true)] out object? value)
    {
        var temp = nodes;
        var node = temp[hash & (temp.Length - 1)];
        do
        {
            if (node.TargetType == targetType && IsMatchColumn(node.Columns, columns))
            {
                value = node.Value;
                return true;
            }
            node = node.Next;
        }
        while (node is not null);

        value = default;
        return false;
    }

    public object AddIfNotExist(Type targetType, ReadOnlySpan<ColumnInfo> columns, int hash, Func<Type, ColumnInfo[], object> valueFactory)
    {
        lock (sync)
        {
            // Double-checked locking
            if (TryGetValue(targetType, columns, hash, out var currentValue))
            {
                return currentValue;
            }

            var copyColumns = columns.ToArray();

            var value = valueFactory(targetType, copyColumns);

            // Check if added by recursive
            if (TryGetValue(targetType, columns, hash, out currentValue))
            {
                return currentValue;
            }

            AddNode(new Node(targetType, copyColumns, hash, value));

            return value;
        }
    }

    //--------------------------------------------------------------------------------
    // Inner
    //--------------------------------------------------------------------------------

#pragma warning disable CA1812
    private sealed class EmptyKey
    {
    }
#pragma warning restore CA1812

#pragma warning disable SA1214
#pragma warning disable SA1401
    private sealed class Node
    {
        public readonly Type TargetType;

        public ColumnInfo[] Columns;

        public readonly int Hash;

        public readonly object Value;

        public Node? Next;

        public Node(Type targetType, ColumnInfo[] columns, int hash, object value)
        {
            TargetType = targetType;
            Columns = columns;
            Hash = hash;
            Value = value;
        }
    }
#pragma warning restore SA1401
#pragma warning restore SA1214

    //--------------------------------------------------------------------------------
    // Diagnostics
    //--------------------------------------------------------------------------------

    public sealed class DiagnosticsInfo
    {
        public int Width { get; }

        public int Depth { get; }

        public int Count { get; }

        public DiagnosticsInfo(int width, int depth, int count)
        {
            Width = width;
            Depth = depth;
            Count = count;
        }

        public override string ToString() => $"Count={Count}, Width={Width}, Depth={Depth}";
    }
}
