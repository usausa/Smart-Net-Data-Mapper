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
        var pi = properties.FirstOrDefault(x => IsMatchName(x, name, false)) ??
                 properties.FirstOrDefault(x => IsMatchName(x, name, true));
        if (pi is not null)
        {
            return pi;
        }

        var pascalName = Inflector.Pascalize(name);
        if (pascalName != name)
        {
            pi = properties.FirstOrDefault(x => IsMatchName(x, pascalName, false)) ??
                 properties.FirstOrDefault(x => IsMatchName(x, pascalName, true));
            if (pi is not null)
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
