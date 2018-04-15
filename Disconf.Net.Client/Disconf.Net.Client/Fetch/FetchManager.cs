using Disconf.Net.Core.Model;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Disconf.Net.Client.Fetch
{
    /// <summary>
    /// 
    /// </summary>
    public class FetchManager
    {
        private readonly RetryPolicy _policy;
        private readonly IFetcher _fetcher;
        private ClientConfigSection config = ClientConfigSection.Current;
        public static readonly FetchManager Instance = new FetchManager();

        /// <summary>
        /// 
        /// </summary>
        private FetchManager()
        {
            _policy = GetFixedRetryPolicy(config.UpdateStrategy.RetryTimes, config.UpdateStrategy.RetryIntervalSeconds);
            _fetcher = new Fetcher(config.WebApiHost, _policy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetZookeeperHosts()
        {
            return _fetcher.GetZkHosts();
        }

        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <param name="files">获取到的所有file配置</param>
        /// <param name="items">所有到的所有item配置</param>
        public void GetAllConfigs(out IEnumerable<string> files, out IDictionary<string, string> items)
        {
            files = new HashSet<string>();
            items = new Dictionary<string, string>();
            var content = _fetcher.GetAllConfigs(new FetchFilter
            {
                AppName = config.ClientInfo.AppName,
                Version = config.ClientInfo.Version,
                Environment = config.ClientInfo.Environment
            });

            var dic = JsonConvert.DeserializeObject<Dictionary<ConfigType, Dictionary<string, string>>>(content);
            if (dic != null)
            {
                if (dic.ContainsKey(ConfigType.File) && dic[ConfigType.File] != null)
                {
                    var fileDic = dic[ConfigType.File];
                    files = fileDic.Keys;
                    foreach (var key in fileDic.Keys)
                    {
                        SaveAndCopyFile(key, fileDic[key]);
                    }
                    SaveFileList(files);
                }
                if (dic.ContainsKey(ConfigType.Item) && dic[ConfigType.Item] != null)
                {
                    items = dic[ConfigType.Item];
                    SaveItems(items);
                }
            }
        }

        /// <summary>
        /// 获取配置文件值
        /// </summary>
        /// <param name="configName">配置文件名称</param>
        /// <returns></returns>
        public string GetConfig(string configName)
        {
            var filter = GetFilter(configName, ConfigType.File);
            return _fetcher.GetConfig(filter);
        }

        /// <summary>
        /// 根据配置名获取对应的值
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public string GetItem(string configName)
        {
            var filter = GetFilter(configName, ConfigType.Item);
            var value = _fetcher.GetConfig(filter);
            AddOrSetItem(configName, value);
            return value;
        }

        /// <summary>
        /// 保存并且更新文件
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="content"></param>
        public bool SaveAndCopyFile(string configName, string content)
        {
            var tempDir = config.Preservation.TmpRootPhysicalPath;
            string tmpFileName = Path.Combine(tempDir, configName);
            if (!Directory.Exists(tempDir))
            {
                //创建临时目录
                Directory.CreateDirectory(tempDir);
            }

            File.WriteAllText(tmpFileName, content, Encoding.UTF8);
            CopyFile(configName);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retryCount"></param>
        /// <param name="retryIntervalSeconds"></param>
        /// <returns></returns>
        private RetryPolicy GetFixedRetryPolicy(int retryCount, int retryIntervalSeconds)
        {
            FixedInterval interval = new FixedInterval(retryCount, TimeSpan.FromSeconds(retryIntervalSeconds));
            return new RetryPolicy(RetryPolicy.DefaultFixed.ErrorDetectionStrategy, interval);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private ConfigFetchFilter GetFilter(string configName, ConfigType type)
        {
            return new ConfigFetchFilter
            {
                AppName = config.ClientInfo.AppName,
                Version = config.ClientInfo.Version,
                Environment = config.ClientInfo.Environment,
                ConfigType = type,
                ConfigName = configName
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        private void CopyFile(string configName)
        {
            string tmpFileName = Path.Combine(config.Preservation.TmpRootPhysicalPath, configName);
            string factFileName = Path.Combine(config.Preservation.FactRootPhysicalPath, configName);
            if (!Directory.Exists(config.Preservation.FactRootPhysicalPath))
            {
                Directory.CreateDirectory(config.Preservation.FactRootPhysicalPath);
            }
            File.Copy(tmpFileName, factFileName, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        public void CopyFiles(IEnumerable<string> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        CopyFile(file);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ltime"></param>
        public void SaveLastChangedTime(DateTime ltime)
        {
            var fileName = config.Preservation.TmpTimeLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                File.WriteAllText(path, ltime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        /// <summary>
        /// 批量保存配置文件名称到指定的文件中
        /// </summary>
        /// <param name="files"></param>
        private void SaveFileList(IEnumerable<string> files)
        {
            StringBuilder tmp = new StringBuilder();
            foreach (var file in files)
            {
                tmp.Append(',');
                tmp.Append(file);
            }
            if (tmp.Length > 0)
            {
                tmp.Remove(0, 1);
                string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, config.Preservation.TmpFilesLocalName);
                File.WriteAllText(path, tmp.ToString(), Encoding.UTF8);
            }
        }

        /// <summary>
        /// 批量保存键值对配置文件到指定的文件中
        /// </summary>
        /// <param name="items"></param>
        private void SaveItems(IDictionary<string, string> items)
        {
            var fileName = config.Preservation.TmpItemsLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                Task.Run(() =>
                {
                    string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                    XElement root = new XElement("items");
                    foreach (var kv in items)
                    {
                        AddElementItem(root, kv.Key, kv.Value);
                    }
                    root.Save(path);
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void AddElementItem(XElement root, string key, string value)
        {
            root.Add(new XElement("item", new XAttribute("key", key), new XAttribute("value", value)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void AddOrSetItem(string key, string value)
        {
            var fileName = config.Preservation.TmpItemsLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                Task.Run(() =>
                {
                    string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                    try
                    {
                        XElement root = XElement.Load(path);
                        var ele = root.Elements("item").FirstOrDefault(e => e.Attribute("key").Value == key);
                        if (ele == null)
                        {
                            AddElementItem(root, key, value);
                        }
                        else
                        {
                            ele.Attribute("value").SetValue(value);
                        }
                        root.Save(path);
                    }
                    catch { }
                });
            }
        }

        /// <summary>
        /// 获取最后一次变更时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastChangedTime()
        {
            var content = _fetcher.GetLastChangedTime(new FetchFilter
            {
                AppName = config.ClientInfo.AppName,
                Version = config.ClientInfo.Version,
                Environment = config.ClientInfo.Environment
            });
            return DateTime.Parse(content);
        }
    }
}