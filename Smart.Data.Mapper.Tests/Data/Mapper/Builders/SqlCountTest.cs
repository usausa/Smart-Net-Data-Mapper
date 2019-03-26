namespace Smart.Data.Mapper.Builders
{
    using Xunit;

    public class SqlCountTest
    {
        [Fact]
        public void All()
        {
            Assert.Equal(
                "SELECT COUNT(*) FROM Table",
                SqlCount<Entity>.All());
        }

        [Fact]
        public void By()
        {
            Assert.Equal(
                "SELECT COUNT(*) FROM Table WHERE Key1 = @Key1",
                SqlCount<Entity>.By("Key1 = @Key1"));
        }
    }
}
