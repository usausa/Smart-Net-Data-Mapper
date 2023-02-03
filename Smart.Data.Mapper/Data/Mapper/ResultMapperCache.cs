namespace Smart.Data.Mapper;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[DebuggerDisplay("{" + nameof(Diagnostics) + "}")]
internal sealed class ResultMapperCache
{
    private static readonly Node EmptyNode = new(typeof(EmptyKey), Array.Empty<ColumnInfo>(), default!, 0);

    private const int InitialSize = 64;

    private const int Factor = 3;

    private readonly object sync = new();

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

#pragma warning disable CA1307
#pragma warning disable CA1309
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsMatchColumn(Span<ColumnInfo> columns1, Span<ColumnInfo> columns2)
    {
        var length = columns1.Length;
        if (length != columns2.Length)
        {
            return false;
        }

        ref var column1 = ref MemoryMarshal.GetReference(columns1);
        ref var end = ref Unsafe.Add(ref column1, length);
        ref var column2 = ref MemoryMarshal.GetReference(columns2);

    Compare:
        if ((column1.Type != column2.Type) || !String.Equals(column1.Name, column2.Name))
        {
            return false;
        }

        column1 = ref Unsafe.Add(ref column1, 1);
        if (Unsafe.IsAddressLessThan(ref column1, ref end))
        {
            column2 = ref Unsafe.Add(ref column2, 1);
            goto Compare;
        }

        return true;
    }
#pragma warning restore CA1309
#pragma warning restore CA1307

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(Type targetType, Span<ColumnInfo> columns, int hash, [NotNullWhen(true)] out object? value)
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

    public object AddIfNotExist(Type targetType, Span<ColumnInfo> columns, int hash, Func<Type, ColumnInfo[], object> valueFactory)
    {
        lock (sync)
        {
            // Double checked locking
            if (TryGetValue(targetType, columns, hash, out var currentValue))
            {
                return currentValue;
            }

            var copyColumns = new ColumnInfo[columns.Length];
            columns.CopyTo(new Span<ColumnInfo>(copyColumns));

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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Framework only")]
    private sealed class EmptyKey
    {
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Performance")]
    private sealed class Node
    {
        public readonly Type TargetType;

        public readonly ColumnInfo[] Columns;

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
