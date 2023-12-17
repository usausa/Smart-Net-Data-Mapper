namespace Smart.Data.Mapper.Builders;

using Smart.Data.Mapper.Attributes;

public sealed class KeyOnlyEntity
{
    [PrimaryKey(1)]
    public int Key1 { get; set; }

    [PrimaryKey(2)]
    public int Key2 { get; set; }
}
