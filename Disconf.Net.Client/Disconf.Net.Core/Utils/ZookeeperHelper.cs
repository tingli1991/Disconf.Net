using Org.Apache.Zookeeper.Data;
using System;
using System.Collections.Generic;
using ZooKeeper.Net;

namespace Disconf.Net.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class ZookeeperHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="acl"></param>
        /// <param name="createMode"></param>
        /// <returns></returns>
        public static string CreateWithPath(this ZooKeeper.Net.ZooKeeper zk, string path, byte[] data, IEnumerable<ACL> acl, CreateMode createMode)
        {
            var tmp = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (path.Length > 1)
            {
                string tmpPath = "";
                for (var i = 0; i < tmp.Length - 1; i++)
                {
                    tmpPath += "/" + tmp[i];
                    if (zk.Exists(tmpPath, false) == null)
                    {
                        zk.Create(tmpPath, null, acl, createMode);//临时节点目前不允许存在子节点，所以这里可能会有问题
                    }
                }
            }
            return zk.Create(path, data, acl, createMode);
        }
    }
}