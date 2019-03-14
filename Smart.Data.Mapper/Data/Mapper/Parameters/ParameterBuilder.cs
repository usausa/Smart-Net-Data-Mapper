namespace Smart.Data.Mapper.Parameters
{
    using System;
    using System.Data;

    public sealed class ParameterBuilder
    {
        public Action<IDbCommand, object> Build { get; }

        public Action<IDbCommand, object> PostProcess { get; }

        public ParameterBuilder(Action<IDbCommand, object> build, Action<IDbCommand, object> postProcess)
        {
            Build = build;
            PostProcess = postProcess;
        }
    }
}
