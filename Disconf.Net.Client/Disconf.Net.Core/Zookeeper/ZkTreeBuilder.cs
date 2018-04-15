using Disconf.Net.Core.Utils;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// zk tree构造类，该类构造的树除去共有的主节点部分外，所以节点均为同级节点
    /// </summary>
    public class ZkTreeBuilder : IZkTreeBuilder
    {
        /// <summary>
        /// 应用名
        /// </summary>
        public string AppName { get; private set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// 环境
        /// </summary>
        public string Environment { get; private set; }

        /// <summary>
        /// 用于存储znodeName与configName对应关系的字典,Key为znodeName,Value为configName
        /// </summary>
        protected ConcurrentDictionary<string, string> _dic = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="version"></param>
        /// <param name="environment"></param>
        public ZkTreeBuilder(string appName, string version, string environment)
        {
            AppName = appName;
            Version = version;
            Environment = environment;
        }

        /// <summary>
        /// 获取或者添加znode名称
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public virtual string GetOrAddZnodeName(string configName)
        {
            //这里忽略SHA1理论上也存在重复的可能性
            string znodeName = HashAlgorithmHelper<SHA1CryptoServiceProvider>.ComputeHash(configName);
            _dic[znodeName] = configName;
            return znodeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="znodeName"></param>
        /// <returns></returns>
        public string GetConfigName(string znodeName)
        {
            string configName;
            _dic.TryGetValue(znodeName, out configName);
            return configName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string GetZkRootPath()
        {
            return Path.Combine("\\", AppName, HashAlgorithmHelper<MD5CryptoServiceProvider>.ComputeHash(Version), Environment).Replace("\\", "/");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="znodeName"></param>
        /// <returns></returns>
        public virtual string GetZkPath(string znodeName)
        {
            return Path.Combine(GetZkRootPath(), znodeName).Replace("\\", "/");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllZnodes()
        {
            return _dic.Keys;
        }
    }
}