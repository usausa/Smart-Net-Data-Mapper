namespace Smart.Data.Mapper.Builders
{
    using Xunit;

    public class SelectTest
    {
        [Fact]
        public void ByKey()
        {
            Assert.Equal(
                "SELECT * FROM Table WHERE Key1 = @Key1 AND SubKey = @Key2",
                Select<Entity>.ByKey());
        }

        [Fact]
        public void By()
        {
            Assert.Equal(
                "SELECT * FROM Table WHERE Key1 = @Key1",
                Select<Entity>.By("Key1 = @Key1"));
        }
    }
}
