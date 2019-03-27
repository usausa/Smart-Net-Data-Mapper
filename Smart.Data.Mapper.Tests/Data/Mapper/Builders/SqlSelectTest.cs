namespace Smart.Data.Mapper.Builders
{
    using Xunit;

    public class SqlSelectTest
    {
        //--------------------------------------------------------------------------------
        // Entity
        //--------------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------------
        // KeyOnly
        //--------------------------------------------------------------------------------

        [Fact]
        public void KeyOnlyByKey()
        {
            Assert.Equal(
                "SELECT * FROM KeyOnly WHERE Key1 = @Key1 AND Key2 = @Key2",
                SqlSelect<KeyOnlyEntity>.ByKey());
        }

        [Fact]
        public void KeyOnlyBy()
        {
            Assert.Equal(
                "SELECT * FROM KeyOnly WHERE Key1 = @Key1",
                SqlSelect<KeyOnlyEntity>.By("Key1 = @Key1"));
        }

        //--------------------------------------------------------------------------------
        // NonKey
        //--------------------------------------------------------------------------------

        [Fact]
        public void NonKeyByKey()
        {
            Assert.Null(
                SqlSelect<NonKeyEntity>.ByKey());
        }

        [Fact]
        public void NonKeyBy()
        {
            Assert.Equal(
                "SELECT * FROM NonKey WHERE Key1 = @Key1",
                SqlSelect<NonKeyEntity>.By("Key1 = @Key1"));
        }
    }
}
