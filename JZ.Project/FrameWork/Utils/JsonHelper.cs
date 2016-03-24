using Newtonsoft.Json;

namespace FrameWork.Utils
{
    public static class JsonHelper
    {
        private static JsonSerializerSettings defaultSettings = new JsonSerializerSettings();
        private static JsonSerializerSettings IgnoreNullValueSettings;

        static JsonHelper()
        {
            defaultSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            IgnoreNullValueSettings = new JsonSerializerSettings();
            IgnoreNullValueSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            IgnoreNullValueSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public static T Json2Obj<T>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static string Obj2Json(object obj, bool ignoreNullValue = false)
        {
            if (ignoreNullValue)
            {
                return JsonConvert.SerializeObject(obj, Formatting.None, IgnoreNullValueSettings);
            }
            return JsonConvert.SerializeObject(obj, defaultSettings);
        }
    }
}
