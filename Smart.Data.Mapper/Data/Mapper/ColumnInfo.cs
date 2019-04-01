namespace Smart.Data.Mapper
{
    using System;

    public struct ColumnInfo : IEquatable<ColumnInfo>
    {
        public string Name { get; }

        public Type Type { get; }

        public ColumnInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }

#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
        public override int GetHashCode() => (Name, Type).GetHashCode();
#pragma warning restore SA1008 // Opening parenthesis must be spaced correctly

        public override bool Equals(object obj) => obj is ColumnInfo other && Equals(other);

        public bool Equals(ColumnInfo other) => Name == other.Name && Type == other.Type;

        public static bool operator ==(ColumnInfo x, ColumnInfo y) => x.Equals(y);

        public static bool operator !=(ColumnInfo x, ColumnInfo y) => !x.Equals(y);
    }
}
