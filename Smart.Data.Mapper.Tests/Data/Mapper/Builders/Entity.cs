namespace Smart.Data.Mapper.Builders;

using Smart.Data.Mapper.Attributes;

[Name("Table")]
public class Entity
{
    [PrimaryKey(1)]
    public int Key1 { get; set; }

    [PrimaryKey(2)]
    [Name("SubKey")]
    public int Key2 { get; set; }

    public string? Name { get; set; }

    [Name("Flag")]
    public bool IsEnable { get; set; }

    [Ignore]
    public int IgnoreValue { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Ignore")]
    public int WriteOnlyValue
    {
        set => IgnoreValue = value;
    }
}
