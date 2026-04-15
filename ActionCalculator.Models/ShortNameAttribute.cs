namespace ActionCalculator.Models
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ShortNameAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }
}
