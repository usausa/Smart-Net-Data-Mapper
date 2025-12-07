namespace Smart.Data.Mapper.Mappers;

using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

using Smart.Data.Mapper.Attributes;
using Smart.Reflection.Emit;

public sealed class EmitObjectResultMapperFactory : IResultMapperFactory
{
    public static EmitObjectResultMapperFactory Instance { get; } = new();

#if NET9_0_OR_GREATER
    private readonly Lock sync = new();
#else
    private readonly object sync = new();
#endif

    private readonly MethodInfo getValueMethod;

    private readonly MethodInfo getValueWithConvertMethod;

    private readonly HashSet<string> targetAssemblies = [];

    private int typeNo;

    private AssemblyBuilder? assemblyBuilder;

    private ModuleBuilder moduleBuilder = default!;

    private EmitObjectResultMapperFactory()
    {
        getValueMethod = GetType().GetMethod(nameof(GetValue), BindingFlags.Static | BindingFlags.NonPublic)!;
        getValueWithConvertMethod = GetType().GetMethod(nameof(GetValueWithConvert), BindingFlags.Static | BindingFlags.NonPublic)!;
    }

    public bool IsMatch(Type type) => true;

    private TypeBuilder DefineType(Type type)
    {
        lock (sync)
        {
            if (assemblyBuilder is null)
            {
                assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName("EmitObjectResultMapperFactoryAssembly"),
                    AssemblyBuilderAccess.Run);
                moduleBuilder = assemblyBuilder.DefineDynamicModule(
                    "EmitObjectResultMapperFactoryModule");

                assemblyBuilder!.SetCustomAttribute(new CustomAttributeBuilder(
                    typeof(IgnoresAccessChecksToAttribute).GetConstructor([typeof(string)])!,
                    [typeof(EmitObjectResultMapperFactory).Assembly.GetName().Name!]));
            }

            var assemblyName = type.Assembly.GetName().Name;
            if ((assemblyName is not null) && !targetAssemblies.Contains(assemblyName))
            {
                assemblyBuilder!.SetCustomAttribute(new CustomAttributeBuilder(
                    typeof(IgnoresAccessChecksToAttribute).GetConstructor([typeof(string)])!,
                    [assemblyName]));

                targetAssemblies.Add(assemblyName);
            }

            var typeBuilder = moduleBuilder.DefineType(
                $"Holder_{typeNo}",
                TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);
            typeNo++;

            return typeBuilder;
        }
    }

#pragma warning disable CA1062
    public ResultMapper<T> CreateMapper<T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns)
    {
        var ci = type.GetConstructor(Type.EmptyTypes);
        if (ci is null)
        {
            throw new ArgumentException($"Default constructor not found. type=[{type.FullName}]", nameof(type));
        }

        var entries = CreateMapEntries(config, type, columns);

        // Define type
        var typeBuilder = DefineType(type);

        // Set base type
        var baseType = typeof(ResultMapper<>).MakeGenericType(type);
        typeBuilder.SetParent(baseType);

        // Define method
        var methodBuilder = typeBuilder.DefineMethod(
            nameof(ResultMapper<>.Map),
            MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig,
            type,
            [typeof(IDataRecord)]);

        var ilGenerator = methodBuilder.GetILGenerator();

        ilGenerator.Emit(OpCodes.Newobj, ci);

        foreach (var entry in entries)
        {
            ilGenerator.Emit(OpCodes.Dup);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.EmitLdcI4(entry.Index);
            if (entry.Parser is null)
            {
                var method = getValueMethod.MakeGenericMethod(entry.Property.PropertyType);
                ilGenerator.Emit(OpCodes.Call, method);
            }
            else
            {
                var field = typeBuilder.DefineField(
                    $"parser{entry.Index}",
                    typeof(Func<object, object>),
                    FieldAttributes.Public);

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, field);
                var method = getValueWithConvertMethod.MakeGenericMethod(entry.Property.PropertyType);
                ilGenerator.Emit(OpCodes.Call, method);
            }
            ilGenerator.EmitCallMethod(entry.Property.SetMethod!);
        }

        ilGenerator.Emit(OpCodes.Ret);

        // Create instance
        var typeInfo = typeBuilder.CreateTypeInfo();
        var holderType = typeInfo.AsType();
        var holder = (ResultMapper<T>)Activator.CreateInstance(holderType)!;

        foreach (var entry in entries)
        {
            if (entry.Parser is not null)
            {
                var field = holderType.GetField($"parser{entry.Index}");
                field!.SetValue(holder, entry.Parser);
            }
        }

        return holder;
    }
#pragma warning restore CA1062

    private static MapEntry[] CreateMapEntries(ISqlMapperConfig config, Type type, ColumnInfo[] columns)
    {
        var selector = config.GetPropertySelector();
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(IsTargetProperty)
            .ToArray();

        var list = new List<MapEntry>();
        for (var i = 0; i < columns.Length; i++)
        {
            var column = columns[i];
            var pi = selector(properties, column.Name);
            if (pi is null)
            {
                continue;
            }

            list.Add(new MapEntry(i, pi, config.CreateParser(column.Type, pi.PropertyType)));
        }

        return [.. list];
    }

    private static bool IsTargetProperty(PropertyInfo pi)
    {
        return pi.CanWrite && (pi.GetCustomAttribute<IgnoreAttribute>() is null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T GetValue<T>(IDataRecord reader, int index)
    {
        var value = reader.GetValue(index);
        return value is DBNull ? default! : (T)value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T GetValueWithConvert<T>(IDataRecord reader, int index, Func<object, object> parser)
    {
        var value = reader.GetValue(index);
        return value is DBNull ? default! : (T)parser(value);
    }

#pragma warning disable SA1401
    private sealed class MapEntry
    {
        public readonly int Index;

        public readonly PropertyInfo Property;

        public readonly Func<object, object>? Parser;

        public MapEntry(int index, PropertyInfo property, Func<object, object>? parser)
        {
            Index = index;
            Property = property;
            Parser = parser;
        }
    }
#pragma warning restore SA1401
}
