using System;
using System.ComponentModel;
using System.Reflection;

namespace ActionCalculator
{
    public static class EnumExtensions
    {
        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }

        public static string GetDescriptionFromValue(this Enum value) =>
            value.GetType()
                .GetMember(value.ToString())[0]
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description ?? string.Empty;
    }
}
