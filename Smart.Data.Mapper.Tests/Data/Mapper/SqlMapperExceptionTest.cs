namespace Smart.Data.Mapper
{
    using System;

    using Xunit;

    public class SqlMapperExceptionTest
    {
        [Fact]
        public void CoverageFix()
        {
            Assert.Throws<SqlMapperException>((Action)(() => throw new SqlMapperException()));
            Assert.Throws<SqlMapperException>((Action)(() => throw new SqlMapperException("test")));
            Assert.Throws<SqlMapperException>((Action)(() => throw new SqlMapperException("test", new ArgumentException())));
        }
    }
}
