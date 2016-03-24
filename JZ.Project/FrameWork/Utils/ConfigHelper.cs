using System.Collections.Generic;
using System.Configuration;

namespace FrameWork.Utils
{
    public static class ConfigHelper
    {
        /// <summary>
        /// 配置信息帮助类
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetConnStrings()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            int num = 0;
            int count = ConfigurationManager.ConnectionStrings.Count;
            while (num < count)
            {
                dictionary.Add(ConfigurationManager.ConnectionStrings[num].Name,
                    ConfigurationManager.ConnectionStrings[num].ConnectionString);
                num++;
            }
            return dictionary;
        }

    }
}
