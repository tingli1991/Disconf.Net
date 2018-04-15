using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disconf.Net.Core.Model;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// 
    /// </summary>
    public class ZkItemTreeBuilder : ZkConfigTreeBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="version"></param>
        /// <param name="environment"></param>
        public ZkItemTreeBuilder(string appName, string version, string environment)
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
                return ConfigType.Item;
            }
        }
    }
}