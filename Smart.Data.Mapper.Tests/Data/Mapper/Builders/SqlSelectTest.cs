namespace Smart.Data.Mapper.Builders
{
    using Xunit;

    public class SqlSelectTest
    {
        [Fact]
        public void ByKey()
        {
            Assert.Equal(
                "SELECT * FROM Table WHERE Key1 = @Key1 AND SubKey = @Key2",
                SqlSelect<Entity>.ByKey());
        }

        [Fact]
        public void By()
        {
            Assert.Equal(
                "SELECT * FROM Table WHERE Key1 = @Key1",
                SqlSelect<Entity>.By("Key1 = @Key1"));
        }
    }
}
