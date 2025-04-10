namespace Smart.Data.Mapper;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    private Node[] nodes;

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
        uint size = 0;

        for (var i = 1L; i < requestSize; i *= 2)
        {
            size = (size << 1) + 1;
        }

        return (int)(size + 1);
    }

    private static Node[] CreateInitialTable()
    {
        var newNodes = new Node[InitialSize];

        for (var i = 0; i < newNodes.Length; i++)
        {
            newNodes[i] = EmptyNode;
        }

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

    private static void RelocateNodes(Node[] nodes, Node[] oldNodes)
    {
        for (var i = 0; i < oldNodes.Length; i++)
        {
            var node = oldNodes[i];
            if (node == EmptyNode)
            {
                continue;
            }

            do
            {
                var next = node.Next;
                node.Next = null;

                UpdateLink(ref nodes[node.Hash & (nodes.Length - 1)], node);

                node = next;
            }
            while (node is not null);
        }
    }

    private void AddNode(Node node)
    {
        var requestSize = Math.Max(InitialSize, (count + 1) * Factor);
        var size = CalculateSize(requestSize);
        if (size > nodes.Length)
        {
            var newNodes = new Node[size];
            for (var i = 0; i < newNodes.Length; i++)
            {
                newNodes[i] = EmptyNode;
            }

            RelocateNodes(newNodes, nodes);

            UpdateLink(ref newNodes[node.Hash & (newNodes.Length - 1)], node);

            Interlocked.MemoryBarrier();

            nodes = newNodes;
            depth = CalculateDepth(newNodes);
            count++;
        }
        else
        {
            Interlocked.MemoryBarrier();

            var hash = node.Hash;

            UpdateLink(ref nodes[hash & (nodes.Length - 1)], node);

            depth = Math.Max(CalculateDepth(nodes[hash & (nodes.Length - 1)]), depth);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsMatchColumn(ref ColumnInfo[] columns1, ref ColumnInfo[] columns2, int length)
    {
        if (length != columns1.Length)
        {
            return false;
        }

        for (var i = 0; i < length; i++)
        {
            ref var column1 = ref columns1[i];
            ref var column2 = ref columns2[i];

            if ((column1.Type != column2.Type) || !String.Equals(column1.Name, column2.Name, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(Type targetType, ref ColumnInfo[] columns, int length, int hash, [NotNullWhen(true)] out object? value)
    {
        var temp = nodes;
        var node = temp[hash & (temp.Length - 1)];
        do
        {
            if (node.TargetType == targetType && IsMatchColumn(ref node.Columns, ref columns, length))
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

    public object AddIfNotExist(Type targetType, ref ColumnInfo[] columns, int length, int hash, Func<Type, ColumnInfo[], object> valueFactory)
    {
        lock (sync)
        {
            // Double-checked locking
            if (TryGetValue(targetType, ref columns, length, hash, out var currentValue))
            {
                return currentValue;
            }

            var copyColumns = new ColumnInfo[length];
            columns.AsSpan(0, length).CopyTo(new Span<ColumnInfo>(copyColumns));

            var value = valueFactory(targetType, copyColumns);

            // Check if added by recursive
            if (TryGetValue(targetType, ref columns, length, hash, out currentValue))
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
