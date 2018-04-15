using System;
using System.Configuration;

namespace Disconf.Net.Client
{
    /// <summary>
    /// 更新策略
    /// </summary>
    public class UpdateStrategySection : ConfigurationElement
    {
        /// <summary>
        /// 要忽略更新的文件配置，以,分割，注意大小写要与服务端一致
        /// </summary>
        [ConfigurationProperty("fileIgnores", DefaultValue = "")]
        public string FileIgnores => $"{this["fileIgnores"]}";

        /// <summary>
        /// 要忽略更新的键值对配置，以,分割，注意大小写要与服务端一致
        /// </summary>
        [ConfigurationProperty("itemIgnores", DefaultValue = "")]
        public string ItemIgnores => $"{this["itemIgnores"]}";

        /// <summary>
        /// 启动时是否同步加载，默认同步
        /// </summary>
        [ConfigurationProperty("startedSync", DefaultValue = true)]
        public bool StartedSync => Convert.ToBoolean(this["startedSync"]);

        /// <summary>
        /// 当获取失败时的重试次数，默认为3
        /// </summary>
        [ConfigurationProperty("retryTimes", DefaultValue = 3)]
        public int RetryTimes => Convert.ToInt32(this["retryTimes"]);

        /// <summary>
        /// 每次重试时间间隔，单位秒，默认每10秒重试一次
        /// </summary>
        [ConfigurationProperty("retryIntervalSeconds", DefaultValue = 10)]
        public int RetryIntervalSeconds => Convert.ToInt32(this["retryIntervalSeconds"]);

        /// <summary>
        /// 要忽略更新的文件配置列表
        /// </summary>
        public string[] FileIgnoreList { get; private set; }

        /// <summary>
        /// 要忽略更新的键值对配置列表
        /// </summary>
        public string[] ItemIgnoreList { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected override void PostDeserialize()
        {
            FileIgnoreList = FileIgnores.Split(',');
            ItemIgnoreList = ItemIgnores.Split(',');
            base.PostDeserialize();
        }
    }
}