using Org.Apache.Zookeeper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooKeeper.Net;

namespace Disconf.Net.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class ZookeeperHelper
    {
        /// <summary>
        /// 添加临时直接点
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void AddTmpChildNode(this ZooKeeper.Net.ZooKeeper zk, string path, byte[] data)
        {
            //添加监控时同时增加临时节点，表明客户端已经下载过节点数据，删除节点部分工作由服务端进行
            string nodePath = string.Format("{0}/{1}", path, Guid.NewGuid());
            zk.Create(nodePath, data, Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);//注意使用的是临时节点
        }

        /// <summary>
        /// 移除临时子节点，表明该节点目前已更新，客户端需要重新下载
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="path"></param>
        public static void RemoveTmpChildNode(this ZooKeeper.Net.ZooKeeper zk, string path)
        {
            var childs = zk.GetChildren(path, false);
            if (childs != null && childs.Any())
            {
                foreach (var child in childs)
                {
                    zk.Delete(string.Format("{0}/{1}", path, child), -1);
                }
            }
        }

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