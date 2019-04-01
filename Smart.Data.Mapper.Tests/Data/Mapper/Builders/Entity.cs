namespace Smart.Data.Mapper.Builders
{
    using Smart.Data.Mapper.Attributes;

    [Name("Table")]
    public class Entity
    {
        [PrimaryKey(1)]
        public int Key1 { get; set; }

        [PrimaryKey(2)]
        [Name("SubKey")]
        public int Key2 { get; set; }

        public string Name { get; set; }

        [Name("Flag")]
        public bool IsEnable { get; set; }

        [Ignore]
        public int IgnoreValue { get; set; }

#pragma warning disable CA1044 // Properties should not be write only
        public int WriteOnlyValue
        {
            set => IgnoreValue = value;
        }
#pragma warning restore CA1044 // Properties should not be write only
    }
}
