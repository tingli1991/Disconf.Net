using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Disconf.Net.Core.Utils
{
    /// <summary>
    /// 字典类型数据扩展
    /// </summary>
    public static class ConfigManagerExtend
    {
        /// <summary>
        /// 获取自定义配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static T GetConfigurationSection<T>(string sectionName)
        {
            var section = ConfigurationManager.GetSection(sectionName);
            return (T)section;
        }

        /// <summary>
        /// 获取Value值
        /// </summary>
        /// <param name="nameValue"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetNameValue(this NameValueCollection nameValue, string key)
        {
            return nameValue[key];
        }

        /// <summary>
        /// 根据键值对的键获取对应的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameValue"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetNameValue<T>(this NameValueCollection nameValue, string key)
        {
            var value = nameValue.GetNameValue(key);
            return ChangeTo<T>(value);
        }

        /// <summary>
        /// 获取value值
        /// </summary>
        /// <param name="elementCollection"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ConfigurationElement GetNameValue(this ConnectionStringSettingsCollection elementCollection, string key)
        {
            return elementCollection[key];
        }

        /// <summary>
        /// 将字符串转化为指定类型
        /// </summary>
        /// <typeparam name="T">指定的类型</typeparam>
        /// <param name="str">需要转换的字符串</param>
        /// <returns></returns>
        public static T ChangeTo<T>(string str)
        {
            T result = default(T);
            result = (T)Convert.ChangeType(str, typeof(T));
            return result;
        }
    }
}