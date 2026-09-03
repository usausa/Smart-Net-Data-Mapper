namespace Smart.Data.Mapper;

public sealed class SqlMapperExceptionTests
{
    [Fact]
    public void CoverageFix()
    {
        Assert.Throws<SqlMapperException>((Action)(static () => throw new SqlMapperException()));
        Assert.Throws<SqlMapperException>((Action)(static () => throw new SqlMapperException("test")));
        Assert.Throws<SqlMapperException>((Action)(static () => throw new SqlMapperException("test", new ArgumentException())));
    }
}
