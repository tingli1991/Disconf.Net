using Disconf.Net.Client.Fetch;
using Disconf.Net.Client.Rules;
using Disconf.Net.Core.Zookeeper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Disconf.Net.Client
{
    /// <summary>
    /// 配置管理类
    /// </summary>
    public class ConfigManager
    {
        private NodeWatcher _fileWatcher;
        private NodeWatcher _itemWatcher;
        private ExceptionHandler _handler;
        public event Action<Exception> Exception;//更新异常时调用事件进行通知
        public event Action<string, string> NodeChanged;//节点更新完成事件通知
        private ClientConfigSection config = ClientConfigSection.Current;
        public static readonly ConfigManager Instance = new ConfigManager();
        public readonly RuleCollection<FileRule> FileRules = new RuleCollection<FileRule>();
        public readonly RuleCollection<ItemRule> ItemRules = new RuleCollection<ItemRule>();

        /// <summary>
        /// 构造函数
        /// </summary>
        private ConfigManager()
        {
            _handler = new ExceptionHandler();
            _handler.Faulted += Handler_Faulted;
        }

        /// <summary>
        /// 处理失败事件执行方法
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="ex"></param>
        private void Handler_Faulted(string configName, Exception ex)
        {
            if (Exception != null)
            {
                if (!string.IsNullOrWhiteSpace(configName))
                {
                    ex = new Exception(configName, ex);
                }
                Exception(ex);
            }
        }

        /// <summary>
        /// Config初始化，包括zookeeper、scan等
        /// 该方法理应在所有代码执行之前就被调用，否则可能会出现配置调用顺序错误
        /// </summary>
        /// <param name="config"></param>
        public void Init()
        {
            if (!config.EnableRemote)
                return;

            var task = Task.Run(() => _handler.Execute(string.Empty, () => GetAllConfigs()));
            if (!config.UpdateStrategy.StartedSync)
                return;

            //同步等待
            task.Wait();
        }

        /// <summary>
        /// 获取所有配置信息
        /// </summary>
        private void GetAllConfigs()
        {
            string zkHosts = null;
            bool downLoad = false;
            IEnumerable<string> files = null;
            IDictionary<string, string> items = null;
            var fetchManager = FetchManager.Instance;
            zkHosts = fetchManager.GetZookeeperHosts();
            var ltimeFromLocal = GetLastChangedTimeFromLocalIfExist();
            var ltimeFromServer = fetchManager.GetLastChangedTime();
            if (ltimeFromLocal > DateTime.Now || ltimeFromLocal < ltimeFromServer)
            {
                fetchManager.GetAllConfigs(out files, out items);
                fetchManager.SaveLastChangedTime(ltimeFromServer);
                downLoad = true;
            }

            if (!downLoad)
            {
                //如果更新异常、或者不需要从服务端获取，则从本地恢复item
                items = GetItemsFromLocalIfExist();

                //file方式虽然本身就是替换了实际文件的，但发布时配置文件存在覆盖问题，所以也需要恢复
                files = GetFilesFromLocalIfExist();
                fetchManager.CopyFiles(files);
            }

            //刷新配置
            RefreshAndInitItems(zkHosts, items);
            RefreshAndInitFiles(zkHosts, files);
        }

        /// <summary>
        /// 刷新文件配置
        /// </summary>
        /// <param name="zkHosts"></param>
        /// <param name="files"></param>
        private void RefreshAndInitFiles(string zkHosts, IEnumerable<string> files)
        {
            if (files != null)
            {
                var fileBuilder = new ZkFileTreeBuilder(config.ClientInfo.AppName, config.ClientInfo.Version, config.ClientInfo.Environment);
                foreach (var configName in files)
                {
                    fileBuilder.GetOrAddZnodeName(configName);
                    FileRules.For(configName).MapTo(configName);
                    FileWatcher_NodeChanged(configName);
                }

                if (!string.IsNullOrWhiteSpace(zkHosts))
                {
                    _fileWatcher = new NodeWatcher(zkHosts, 30000, fileBuilder, config.ClientInfo.ClientName);
                    _fileWatcher.NodeChanged += FileWatcher_NodeChanged;
                }
            }
        }

        /// <summary>
        /// 刷新键值对
        /// </summary>
        /// <param name="zkHosts"></param>
        /// <param name="items"></param>
        private void RefreshAndInitItems(string zkHosts, IDictionary<string, string> items)
        {
            if (items != null)
            {
                var itemBuilder = new ZkItemTreeBuilder(config.ClientInfo.AppName, config.ClientInfo.Version, config.ClientInfo.Environment);
                foreach (var itemName in items.Keys)
                {
                    itemBuilder.GetOrAddZnodeName(itemName);
                    ItemWatcher_NodeChanged(itemName);
                }

                if (!string.IsNullOrWhiteSpace(zkHosts))
                {
                    _itemWatcher = new NodeWatcher(zkHosts, 30000, itemBuilder, config.ClientInfo.ClientName);
                    _itemWatcher.NodeChanged += ItemWatcher_NodeChanged;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetFilesFromLocalIfExist()
        {
            IEnumerable<string> files = null;
            var fileName = config.Preservation.TmpFilesLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                if (File.Exists(path))
                {
                    files = File.ReadAllText(path, Encoding.UTF8).Split(',');
                }
            }
            return files;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, string> GetItemsFromLocalIfExist()
        {
            IDictionary<string, string> dic = null;
            var fileName = config.Preservation.TmpItemsLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                try
                {
                    XElement root = XElement.Load(path);
                    var eles = root.Elements("item");
                    if (eles.Any())
                    {
                        dic = eles.ToDictionary(e => e.Attribute("key").Value, e => e.Attribute("value").Value);
                    }
                }
                catch { }
            }
            return dic;
        }

        /// <summary>
        /// 如果本地文件存则从本地获取最后一次更新时间
        /// </summary>
        /// <returns></returns>
        private DateTime GetLastChangedTimeFromLocalIfExist()
        {
            var fileName = config.Preservation.TmpTimeLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                try
                {
                    if (DateTime.TryParse(File.ReadAllText(path), out DateTime dt))
                        return dt;
                }
                catch { }
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// 节点变更事件执行方法
        /// </summary>
        /// <param name="itemName"></param>
        private void ItemWatcher_NodeChanged(string itemName)
        {
            _handler.Execute(itemName, () =>
             {
                 if (!config.UpdateStrategy.ItemIgnoreList.Contains(itemName))
                 {
                     //获取键值对的值
                     string value = FetchManager.Instance.GetItem(itemName);

                     //节点更新完成事件
                     NodeChanged(itemName, value);

                     //推送给指定的回调函数
                     ItemRules.ConfigChanged(itemName, value);
                 }
             });
        }

        /// <summary>
        /// 监听别点变更事件执行方法
        /// </summary>
        /// <param name="obj"></param>
        private void FileWatcher_NodeChanged(string configName)
        {
            _handler.Execute(configName, () =>
             {
                 //当前文件是否是过滤文件，如果是不更新
                 if (config.UpdateStrategy.FileIgnoreList.Contains(configName))
                     return;

                 //下载并更新配置文件
                 var configValue = FetchManager.Instance.GetConfig(configName);
                 var isSaveSuccess = FetchManager.Instance.SaveAndCopyFile(configName, configValue);
                 if (string.IsNullOrEmpty(configValue) || !isSaveSuccess)
                     return;

                 //节点更新完成事件
                 NodeChanged(configName, configValue);

                 //推送给指定的回调函数
                 FileRules.ConfigChanged(configName, configValue);
             });
        }
    }
}