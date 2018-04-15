using System;
using System.Configuration;

namespace Disconf.Net.Client
{
    /// <summary>
    /// Disconf.Net的客户端配置参数
    /// </summary>
    public class ClientConfigSection : ConfigurationSection
    {
        /// <summary>
        /// 
        /// </summary>
        public static ClientConfigSection Current = (ClientConfigSection)ConfigurationManager.GetSection("disconfSections");

        /// <summary>
        /// Rest服务器域名地址
        /// </summary>
        [ConfigurationProperty("host", IsRequired = true)]
        public string WebApiHost => $"{this["host"]}";

        /// <summary>
        /// 是否启用远程配置，默认true，设为false的话表示不从远程服务器下载配置
        /// </summary>
        [ConfigurationProperty("enableRemote", DefaultValue = true)]
        public bool EnableRemote => Convert.ToBoolean(this["enableRemote"]);

        /// <summary>
        /// 业务系统信息设置
        /// </summary>
        [ConfigurationProperty("client", IsRequired = true)]
        public ClientInfoSection ClientInfo => (ClientInfoSection)this["client"];

        /// <summary>
        /// 更新策略设置
        /// </summary>
        [ConfigurationProperty("updateStrategy")]
        public UpdateStrategySection UpdateStrategy => (UpdateStrategySection)this["updateStrategy"];

        /// <summary>
        /// 本地持久化设置
        /// </summary>
        [ConfigurationProperty("preservation")]
        public PreservationSection Preservation => (PreservationSection)this["preservation"];

        /// <summary>
        /// 
        /// </summary>
        protected override void PostDeserialize()
        {
            Uri uri = new Uri(WebApiHost);//用来验证uri是否正确
            base.PostDeserialize();
        }
    }
}