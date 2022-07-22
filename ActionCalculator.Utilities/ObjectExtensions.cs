using Newtonsoft.Json;

namespace ActionCalculator.Utilities
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T source)
        {
            if (ReferenceEquals(source, null))
            {
                return default!;
            }
            
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings)!;
        }
    }
}
