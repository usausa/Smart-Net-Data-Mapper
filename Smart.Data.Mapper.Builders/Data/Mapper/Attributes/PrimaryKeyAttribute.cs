namespace Smart.Data.Mapper.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PrimaryKeyAttribute : Attribute
{
    public int Order { get; }

    public PrimaryKeyAttribute(int order = 0)
    {
        Order = order;
    }
}
