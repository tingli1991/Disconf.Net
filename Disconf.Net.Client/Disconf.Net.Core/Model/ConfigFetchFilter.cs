namespace Disconf.Net.Core.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigFetchFilter : FetchFilter
    {
        /// <summary>
        /// 配置名
        /// </summary>
        public string ConfigName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public ConfigType ConfigType { get; set; }
    }
}