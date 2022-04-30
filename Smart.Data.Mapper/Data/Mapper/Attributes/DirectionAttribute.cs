namespace Smart.Data.Mapper.Attributes;

using System.Data;

[AttributeUsage(AttributeTargets.Property)]
public sealed class DirectionAttribute : Attribute
{
    public ParameterDirection Direction { get; }

    public DirectionAttribute(ParameterDirection direction)
    {
        Direction = direction;
    }
}
