namespace DataAccess.FormsApp.Models
{
    using System;

    using Smart.Data.Mapper.Attributes;

    public class TestEntity
    {
        [PrimaryKey]
        public long Id { get; set; }

        public string StringValue { get; set; }

        public int IntValue { get; set; }

        public long LongValue { get; set; }

        public double DoubleValue { get; set; }

        public decimal DecimalValue { get; set; }

        public bool BoolValue { get; set; }

        public DateTimeOffset DateTimeOffsetValue { get; set; }
    }
}
