namespace Smart.Data.Mapper.Builders
{
    using Xunit;

    public class SqlInsertTest
    {
        [Fact]
        public void Values()
        {
            Assert.Equal(
                "INSERT INTO Table (Key1, SubKey, Name, Flag) VALUES (@Key1, @Key2, @Name, @IsEnable)",
                SqlInsert<Entity>.Values());
        }
    }
}
