namespace Smart.Data.Mapper.Parameters;

using System;
using System.Data.Common;

public sealed class ParameterBuilder
{
    public Action<DbCommand, object>? Build { get; }

    public Action<DbCommand, object>? PostProcess { get; }

    public ParameterBuilder(Action<DbCommand, object>? build, Action<DbCommand, object>? postProcess)
    {
        Build = build;
        PostProcess = postProcess;
    }
}
