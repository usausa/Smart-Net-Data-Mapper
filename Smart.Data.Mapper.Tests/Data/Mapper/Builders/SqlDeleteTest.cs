namespace Smart.Data.Mapper.Builders
{
    using Xunit;

    public class SqlDeleteTest
    {
        [Fact]
        public void ByKey()
        {
            Assert.Equal(
                "DELETE FROM Table WHERE Key1 = @Key1 AND SubKey = @Key2",
                SqlDelete<Entity>.ByKey());
        }

        [Fact]
        public void By()
        {
            Assert.Equal(
                "DELETE FROM Table WHERE Key1 = @Key1",
                SqlDelete<Entity>.By("Key1 = @Key1"));
        }
    }
}
