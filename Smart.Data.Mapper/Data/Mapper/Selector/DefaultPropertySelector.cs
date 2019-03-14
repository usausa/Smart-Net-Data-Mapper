namespace Smart.Data.Mapper.Selector
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Smart.Text;

    public sealed class DefaultPropertySelector : IPropertySelector
    {
        public static DefaultPropertySelector Instance { get; } = new DefaultPropertySelector();

        private DefaultPropertySelector()
        {
        }

        public PropertyInfo Select(PropertyInfo[] properties, string name)
        {
            var pi = properties.FirstOrDefault(x => String.Equals(x.Name, name, StringComparison.Ordinal)) ??
                     properties.FirstOrDefault(x => String.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            if (pi != null)
            {
                return pi;
            }

            var pascalName = Inflector.Pascalize(name);
            return properties.FirstOrDefault(x => String.Equals(x.Name, pascalName, StringComparison.Ordinal)) ??
                   properties.FirstOrDefault(x => String.Equals(x.Name, pascalName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
