namespace Smart.Data.Mapper.Builders
{
    using Xunit;

    public class SqlUpdateTest
    {
        //--------------------------------------------------------------------------------
        // Entity
        //--------------------------------------------------------------------------------

        [Fact]
        public void SetByKey()
        {
            Assert.Equal(
                "UPDATE Table SET Name = @Name, Flag = @IsEnable WHERE Key1 = @Key1 AND SubKey = @Key2",
                SqlUpdate<Entity>.SetByKey());
        }

        [Fact]
        public void SetByKeyWithCondition()
        {
            Assert.Equal(
                "UPDATE Table SET Name = @Name WHERE Key1 = @Key1 AND SubKey = @Key2",
                SqlUpdate<Entity>.SetByKey("Name = @Name"));
        }

        [Fact]
        public void SetBy()
        {
            Assert.Equal(
                "UPDATE Table SET Name = @Name WHERE Key1 = @Key1",
                SqlUpdate<Entity>.SetBy("Name = @Name", "Key1 = @Key1"));
        }

        //--------------------------------------------------------------------------------
        // KeyOnly
        //--------------------------------------------------------------------------------

        [Fact]
        public void KeyOnlySetByKey()
        {
            Assert.Null(
                SqlUpdate<KeyOnlyEntity>.SetByKey());
        }

        [Fact]
        public void KeyOnlySetByKeyWithCondition()
        {
            Assert.Null(
                SqlUpdate<KeyOnlyEntity>.SetByKey("Key2 = @Key2"));
        }

        [Fact]
        public void KeyOnlySetBy()
        {
            Assert.Equal(
                "UPDATE KeyOnly SET Key2 = @Key2 WHERE Key1 = @Key1",
                SqlUpdate<KeyOnlyEntity>.SetBy("Key2 = @Key2", "Key1 = @Key1"));
        }

        //--------------------------------------------------------------------------------
        // KeyOnly
        //--------------------------------------------------------------------------------

        [Fact]
        public void NonKeySetByKey()
        {
            Assert.Null(
                SqlUpdate<NonKeyEntity>.SetByKey());
        }

        [Fact]
        public void NonKeySetByKeyWithCondition()
        {
            Assert.Null(
                SqlUpdate<NonKeyEntity>.SetByKey("Key2 = @Key2"));
        }

        [Fact]
        public void NonKeySetBy()
        {
            Assert.Equal(
                "UPDATE NonKey SET Key2 = @Key2 WHERE Key1 = @Key1",
                SqlUpdate<NonKeyEntity>.SetBy("Key2 = @Key2", "Key1 = @Key1"));
        }
    }
}
