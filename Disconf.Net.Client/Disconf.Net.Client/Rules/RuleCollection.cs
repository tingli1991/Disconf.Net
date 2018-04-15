using System;
using System.Collections.Concurrent;

namespace Disconf.Net.Client.Rules
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RuleCollection<T> where T : IRule, new()
    {
        private ConcurrentDictionary<string, T> _rules = new ConcurrentDictionary<string, T>();

        /// <summary>
        /// 根据configName获取对应的Rule对象
        /// </summary>
        /// <param name="configName">注意configName区分大小写</param>
        /// <returns></returns>
        public T For(string configName)
        {
            T rule = _rules.GetOrAdd(configName, _ =>
            {
                var t = new T();
                t.MapTo(_);
                return t;
            });
            return rule;
        }

        /// <summary>
        /// 配置发生改变
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="changedValue"></param>
        public void ConfigChanged(string configName, string changedValue)
        {
            if (_rules.TryGetValue(configName, out T rule))
            {
                rule.ConfigChanged(configName, changedValue);
            }
        }
    }
}