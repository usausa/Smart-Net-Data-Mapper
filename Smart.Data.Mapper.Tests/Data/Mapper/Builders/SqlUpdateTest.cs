namespace Smart.Data.Mapper.Builders
{
    using Xunit;

    public class SqlUpdateTest
    {
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
    }
}
