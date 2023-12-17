namespace Smart.Data.Mapper.Builders;

public sealed class SqlUpdateTest
{
    //--------------------------------------------------------------------------------
    // Entity
    //--------------------------------------------------------------------------------

    [Fact]
    public void ByKey()
    {
        Assert.Equal(
            "UPDATE Table SET Name = @Name, Flag = @IsEnable WHERE Key1 = @Key1 AND SubKey = @Key2",
            SqlUpdate<Entity>.ByKey());
    }

    [Fact]
    public void ByKeyWithCondition()
    {
        Assert.Equal(
            "UPDATE Table SET Name = @Name WHERE Key1 = @Key1 AND SubKey = @Key2",
            SqlUpdate<Entity>.ByKey("Name = @Name"));
    }

    [Fact]
    public void Set()
    {
        Assert.Equal(
            "UPDATE Table SET Name = @Name WHERE Key1 = @Key1",
            SqlUpdate<Entity>.Set("Name = @Name", "Key1 = @Key1"));
    }

    //--------------------------------------------------------------------------------
    // KeyOnly
    //--------------------------------------------------------------------------------

    [Fact]
    public void KeyOnlyByKey()
    {
        Assert.Empty(
            SqlUpdate<KeyOnlyEntity>.ByKey());
    }

    [Fact]
    public void KeyOnlyByKeyWithCondition()
    {
        Assert.Empty(
            SqlUpdate<KeyOnlyEntity>.ByKey("Key2 = @Key2"));
    }

    [Fact]
    public void KeyOnlySet()
    {
        Assert.Equal(
            "UPDATE KeyOnly SET Key2 = @Key2 WHERE Key1 = @Key1",
            SqlUpdate<KeyOnlyEntity>.Set("Key2 = @Key2", "Key1 = @Key1"));
    }

    //--------------------------------------------------------------------------------
    // KeyOnly
    //--------------------------------------------------------------------------------

    [Fact]
    public void NonKeyByKey()
    {
        Assert.Empty(
            SqlUpdate<NonKeyEntity>.ByKey());
    }

    [Fact]
    public void NonKeyByKeyWithCondition()
    {
        Assert.Empty(
            SqlUpdate<NonKeyEntity>.ByKey("Key2 = @Key2"));
    }

    [Fact]
    public void NonKeySet()
    {
        Assert.Equal(
            "UPDATE NonKey SET Key2 = @Key2 WHERE Key1 = @Key1",
            SqlUpdate<NonKeyEntity>.Set("Key2 = @Key2", "Key1 = @Key1"));
    }
}
