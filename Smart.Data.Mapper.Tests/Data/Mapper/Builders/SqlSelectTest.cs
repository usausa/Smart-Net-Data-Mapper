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
        public void Where()
        {
            Assert.Equal(
                "SELECT * FROM Table WHERE Key1 = @Key1",
                SqlSelect<Entity>.Where("Key1 = @Key1"));
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
        public void KeyOnlyWhere()
        {
            Assert.Equal(
                "SELECT * FROM KeyOnly WHERE Key1 = @Key1",
                SqlSelect<KeyOnlyEntity>.Where("Key1 = @Key1"));
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
        public void NonKeyWhere()
        {
            Assert.Equal(
                "SELECT * FROM NonKey WHERE Key1 = @Key1",
                SqlSelect<NonKeyEntity>.Where("Key1 = @Key1"));
        }

        //--------------------------------------------------------------------------------
        // Build
        //--------------------------------------------------------------------------------

        [Fact]
        public void BuildDefault()
        {
            Assert.Equal(
                "SELECT * FROM Table ORDER BY Key1, SubKey",
                SqlSelect<Entity>.Build());
        }

        // TODO
    }
}
