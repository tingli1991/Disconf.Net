using Disconf.Net.Core.Model;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// 
    /// </summary>
    public class ZkFileTreeBuilder : ZkConfigTreeBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="version"></param>
        /// <param name="environment"></param>
        public ZkFileTreeBuilder(string appName, string version, string environment)
            : base(appName, version, environment)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override ConfigType ConfigType
        {
            get
            {
                return ConfigType.File;
            }
        }
    }
}