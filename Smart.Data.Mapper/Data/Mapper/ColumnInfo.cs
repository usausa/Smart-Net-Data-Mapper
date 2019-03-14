namespace Smart.Data.Mapper
{
    using System;

    public struct ColumnInfo
    {
        public string Name { get; }

        public Type Type { get; }

        public ColumnInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
