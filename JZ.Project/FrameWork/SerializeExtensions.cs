namespace FrameWork
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class SerializeExtensions
    {
        public static T Deserialize<T>(this byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream2 = new MemoryStream(stream))
            {
                return (T) formatter.Deserialize(stream2);
            }
        }

        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static byte[] Serialize(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        public static string ToJson(this object obj, bool ignoreNull = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include
            };
            return JsonConvert.SerializeObject(obj, Formatting.None, settings);
        }
    }
}

