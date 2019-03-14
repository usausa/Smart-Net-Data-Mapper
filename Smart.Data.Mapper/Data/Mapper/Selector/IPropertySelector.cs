namespace Smart.Data.Mapper.Selector
{
    using System.Reflection;

    public interface IPropertySelector
    {
        PropertyInfo Select(PropertyInfo[] properties, string name);
    }
}
