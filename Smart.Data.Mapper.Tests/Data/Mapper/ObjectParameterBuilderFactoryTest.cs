namespace Smart.Data.Mapper
{
    using System;
    using System.Data;

    using Smart.Data.Mapper.Attributes;
    using Smart.Mock.Data;

    using Xunit;

    public class ObjectParameterBuilderFactoryTest
    {
        [Fact]
        public void BuildParameter()
        {
            using (var con = new MockDbConnection())
            {
                var cmd = new MockDbCommand();
                cmd.SetupResult(0);
                cmd.Executing = ec =>
                {
                    Assert.Equal(13, ec.Parameters.Count);
                    Assert.Equal(1, ec.Parameters[nameof(Parameter.Value1)].Value);
                    Assert.Equal(1, ec.Parameters[nameof(Parameter.Value2)].Value);
                    Assert.Equal(DBNull.Value, ec.Parameters[nameof(Parameter.Value3)].Value);
                    Assert.Equal(Value.One, ec.Parameters[nameof(Parameter.Value4)].Value);
                    Assert.Equal(Value.One, ec.Parameters[nameof(Parameter.Value5)].Value);
                    Assert.Equal(DBNull.Value, ec.Parameters[nameof(Parameter.Value6)].Value);

                    ec.Parameters[nameof(Parameter.Output1)].Value = 1;
                    ec.Parameters[nameof(Parameter.Output2)].Value = DBNull.Value;
                    ec.Parameters[nameof(Parameter.Output3)].Value = 1;
                    ec.Parameters[nameof(Parameter.Output4)].Value = DBNull.Value;
                    ec.Parameters[nameof(Parameter.Output5)].Value = 1;
                    ec.Parameters[nameof(Parameter.Output6)].Value = DBNull.Value;
                    ec.Parameters[nameof(Parameter.Output7)].Value = 1;
                };
                con.SetupCommand(cmd);

                var parameter = new Parameter
                {
                    Value1 = 1,
                    Value2 = 1,
                    Value3 = null,
                    Value4 = Value.One,
                    Value5 = Value.One,
                    Value6 = null
                };
                con.Execute("TEST", parameter, commandType: CommandType.StoredProcedure);

                Assert.Equal(1, parameter.Output1);
                Assert.Equal(0, parameter.Output2);
                Assert.Equal(1, parameter.Output3);
                Assert.Null(parameter.Output4);
                Assert.Equal(Value.One, parameter.Output5);
                Assert.Null(parameter.Output6);
                Assert.Equal("1", parameter.Output7);
            }
        }

        protected class Parameter
        {
            public int Value1 { get; set; }

            public int? Value2 { get; set; }

            public int? Value3 { get; set; }

            public Value Value4 { get; set; }

            public Value? Value5 { get; set; }

            public Value? Value6 { get; set; }

            public int Value7 { set => Value7 = value; }

            [Ignore]
            public int Value8 { get; set; }

            [Direction(ParameterDirection.Output)]
            public int Output1 { get; set; }

            [Direction(ParameterDirection.Output)]
            public int Output2 { get; set; }

            [Direction(ParameterDirection.Output)]
            public int? Output3 { get; set; }

            [Direction(ParameterDirection.Output)]
            public int? Output4 { get; set; }

            [Direction(ParameterDirection.Output)]
            public Value Output5 { get; set; }

            [Direction(ParameterDirection.Output)]
            public Value? Output6 { get; set; }

            [Direction(ParameterDirection.Output)]
            public string Output7 { get; set; }
        }

        protected enum Value
        {
            Zero = 0,
            One = 1
        }
    }
}
