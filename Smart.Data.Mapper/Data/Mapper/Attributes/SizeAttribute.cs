namespace Smart.Data.Mapper.Attributes;

using System;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SizeAttribute : Attribute
{
    public int Size { get; }

    public SizeAttribute(int size)
    {
        Size = size;
    }
}
