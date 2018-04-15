using Disconf.Net.Core.Model;
using System.IO;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ZkConfigTreeBuilder : ZkTreeBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="version"></param>
        /// <param name="environment"></param>
        public ZkConfigTreeBuilder(string appName, string version, string environment)
            : base(appName, version, environment)
        {
        }

        /// <summary>
        /// 当前对应配置类型
        /// </summary>
        public abstract ConfigType ConfigType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetZkRootPath()
        {
            return Path.Combine(base.GetZkRootPath(), ConfigType.ToString());
        }
    }
}