namespace Smart.Data.Mapper.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public sealed class NameAttribute : Attribute
{
    public string Name { get; }

    public NameAttribute(string name)
    {
        Name = name;
    }
}
