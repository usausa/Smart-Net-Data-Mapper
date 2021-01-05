namespace Smart.Data.Mapper.Selector
{
    using System.Reflection;

    public interface IPropertySelector
    {
        PropertyInfo? SelectProperty(PropertyInfo[] properties, string name);
    }
}
