using System.Configuration;

namespace Disconf.Net.Client
{
    /// <summary>
    /// 客户端程序信息
    /// </summary>
    public class ClientInfoSection : ConfigurationElement
    {
        /// <summary>
        /// 客户端程序名称，注意大小写要与服务端一致
        /// </summary>
        [ConfigurationProperty("appName", IsRequired = true)]
        public string AppName => $"{this["appName"]}";

        /// <summary>
        /// 客户端标识，用于服务端查看已更新客户端，如果不设置则默认获取客户端电脑名称
        /// </summary>
        [ConfigurationProperty("clientName", DefaultValue = null)]
        public string ClientName => $"{this["clientName"]}";

        /// <summary>
        /// 当前客户端程序所处环境，注意大小写要与服务端一致
        /// </summary>
        [ConfigurationProperty("environment", IsRequired = true)]
        public string Environment => $"{this["environment"]}";

        /// <summary>
        /// 当前客户端程序版本，注意大小写要与服务端一致
        /// </summary>
        [ConfigurationProperty("version", IsRequired = true)]
        public string Version => $"{this["version"]}";
    }
}