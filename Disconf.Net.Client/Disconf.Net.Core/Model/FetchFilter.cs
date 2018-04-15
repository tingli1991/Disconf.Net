namespace Disconf.Net.Core.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class FetchFilter
    {
        /// <summary>
        /// 系统名
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 系统环境
        /// </summary>
        public string Environment { get; set; }
        /// <summary>
        /// 系统版本
        /// </summary>
        public string Version { get; set; }
    }
}