namespace Smart.Data.Mapper.Builders;

using Xunit;

public class SqlDeleteTest
{
    //--------------------------------------------------------------------------------
    // Entity
    //--------------------------------------------------------------------------------

    [Fact]
    public void ByKey()
    {
        Assert.Equal(
            "DELETE FROM Table WHERE Key1 = @Key1 AND SubKey = @Key2",
            SqlDelete<Entity>.ByKey());
    }

    [Fact]
    public void Where()
    {
        Assert.Equal(
            "DELETE FROM Table WHERE Key1 = @Key1",
            SqlDelete<Entity>.Where("Key1 = @Key1"));
    }

    //--------------------------------------------------------------------------------
    // KeyOnly
    //--------------------------------------------------------------------------------

    [Fact]
    public void KeyOnlyByKey()
    {
        Assert.Equal(
            "DELETE FROM KeyOnly WHERE Key1 = @Key1 AND Key2 = @Key2",
            SqlDelete<KeyOnlyEntity>.ByKey());
    }

    [Fact]
    public void KeyOnlyWhere()
    {
        Assert.Equal(
            "DELETE FROM KeyOnly WHERE Key1 = @Key1",
            SqlDelete<KeyOnlyEntity>.Where("Key1 = @Key1"));
    }

    //--------------------------------------------------------------------------------
    // NonKey
    //--------------------------------------------------------------------------------

    [Fact]
    public void NonKeyByKey()
    {
        Assert.Empty(
            SqlDelete<NonKeyEntity>.ByKey());
    }

    [Fact]
    public void NonKeyWhere()
    {
        Assert.Equal(
            "DELETE FROM NonKey WHERE Key1 = @Key1",
            SqlDelete<NonKeyEntity>.Where("Key1 = @Key1"));
    }
}
