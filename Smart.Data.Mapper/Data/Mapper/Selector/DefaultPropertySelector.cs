namespace Smart.Data.Mapper.Selector;

using System.Reflection;

using Smart.Data.Mapper.Attributes;
using Smart.Text;

public sealed class DefaultPropertySelector : IPropertySelector
{
    public static DefaultPropertySelector Instance { get; } = new();

    private DefaultPropertySelector()
    {
    }

    public PropertyInfo? SelectProperty(PropertyInfo[] properties, string name)
    {
        var pi = FindProperty(properties, name, false) ?? FindProperty(properties, name, true);
        if (pi is not null)
        {
            return pi;
        }

        var pascalName = Inflector.Pascalize(name);
        if (pascalName != name)
        {
            pi = FindProperty(properties, pascalName, false) ?? FindProperty(properties, pascalName, true);
            if (pi is not null)
            {
                return pi;
            }
        }

        return null;
    }

    private static PropertyInfo? FindProperty(PropertyInfo[] properties, string name, bool ignoreCase)
    {
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < properties.Length; i++)
        {
            var pi = properties[i];
            if (IsMatchName(pi, name, ignoreCase))
            {
                return pi;
            }
        }

        return null;
    }

    private static bool IsMatchName(PropertyInfo pi, string name, bool ignoreCase)
    {
        var propertyName = pi.GetCustomAttribute<NameAttribute>()?.Name ?? pi.Name;
        return String.Equals(propertyName, name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }
}
