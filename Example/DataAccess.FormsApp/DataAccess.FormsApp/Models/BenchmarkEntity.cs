namespace DataAccess.FormsApp.Models
{
    using Smart.Data.Mapper.Attributes;

    public class BenchmarkEntity
    {
        [PrimaryKey]
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
