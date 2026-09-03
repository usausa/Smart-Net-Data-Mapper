namespace Smart.Data.Mapper.Builders;

public sealed class SqlInsertTests
{
    [Fact]
    public void Values()
    {
        Assert.Equal(
            "INSERT INTO Table (Key1, SubKey, Name, Flag) VALUES (@Key1, @Key2, @Name, @IsEnable)",
            SqlInsert<Entity>.Values());
    }
}
