using System.ComponentModel;
using System.Reflection;

namespace ActionCalculator.Utilities
{
    public static class EnumExtensions
    {
        public static T GetValueFromDescription<T>(this string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        return (T)field.GetValue(null)!;
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T)field.GetValue(null)!;
                    }
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }

        public static IEnumerable<T> ToEnumerable<T>(this T value, T exclude) where T : Enum =>
            value.ToEnumerable().Where(x => !exclude.Contains(x));

        private static IEnumerable<T> ToEnumerable<T>(this T value) where T : Enum =>
            value.GetType().ToEnumerable<T>().Where(x => value.Contains(x));

        public static IEnumerable<T> ToEnumerable<T>(this Type type) where T : Enum =>
            Enum.GetValues(type).Cast<T>();

        public static string GetDescriptionFromValue(this Enum value) =>
            value.GetType()
                .GetMember(value.ToString())[0]
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description ?? string.Empty;

        public static bool Contains<T>(this T flags, T value) where T : Enum => flags.HasFlag(value);
    }
}
