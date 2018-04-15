using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZooKeeper.Net;
using Disconf.Net.Core.Utils;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// 这个类只负责监控znode的变化
    /// </summary>
    public class NodeWatcher : ConnectWatcher
    {
        /// <summary>
        /// 最后一次Node更新的时间戳，用于Expired重连之后，判断哪些znode在Expired这段时间内有过变化
        /// </summary>
        private long _mtime;

        /// <summary>
        /// 
        /// </summary>
        private byte[] _clientName;

        /// <summary>
        /// 
        /// </summary>
        private IZkTreeBuilder _builder;

        /// <summary>
        /// znode发生变化时的回调事件，arg1对应configName
        /// </summary>
        public event Action<string> NodeChanged;

        /// <summary>
        /// znode监控类
        /// </summary>
        /// <param name="connectionString">zookeeper连接字符串</param>
        /// <param name="timeOut">zookeeper session timout,单位毫秒(ms)</param>
        /// <param name="builder"></param>
        /// <param name="clientName">客户端名称，用于标识更新状况，传入空或null则使用Environment.MachineName</param>
        public NodeWatcher(string connectionString, int timeOut, IZkTreeBuilder builder, string clientName = null)
            : base(connectionString, timeOut)
        {
            _builder = builder;
            if (string.IsNullOrWhiteSpace(clientName))
            {
                clientName = Environment.MachineName;
            }
            _clientName = Encoding.UTF8.GetBytes(clientName);
            RegisterWatcher();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void ReConnectCallBack()
        {
            var configs = RegisterWatcher();
            if (NodeChanged != null)
            {
                //Expired之后变更的节点需要补调通知
                foreach (var config in configs)
                {
                    if (!string.IsNullOrWhiteSpace(config))
                    {
                        NodeChanged(config);
                    }
                }
            }
        }

        /// <summary>
        /// 注册监控关系，并返回在注册监控之前变更过的znode完整路径对应的configName
        /// </summary>
        public IEnumerable<string> RegisterWatcher()
        {
            var configs = new HashSet<string>();
            if (_builder != null)
            {
                long mtime = _mtime;
                var znodes = _builder.GetAllZnodes();
                foreach (var node in znodes)
                {
                    var path = _builder.GetZkPath(node);
                    try
                    {
                        var stat = ZooKeeper.Exists(path, true);
                        if (stat != null)
                        {
                            if (_mtime > 0 && stat.Mtime > _mtime)
                            {
                                //通过_mtime是否大于0进行判断是第一次还是Expired后重连，只有重连时才需要返回变更过的节点
                                configs.Add(_builder.GetConfigName(node));
                            }
                            ZooKeeper.AddTmpChildNode(path, _clientName);
                            mtime = Math.Max(mtime, stat.Mtime);
                        }
                    }
                    catch (Exception)
                    {
                        //TODO:可能需要判断Expired
                    }
                }
                _mtime = mtime;
            }
            return configs;
        }

        /// <summary>
        /// 监控进程
        /// </summary>
        /// <param name="event"></param>
        public override void Process(WatchedEvent @event)
        {
            base.Process(@event);
            switch (@event.Type)
            {
                case EventType.NodeDataChanged:
                    var path = @event.Path;
                    if (NodeChanged != null && !string.IsNullOrWhiteSpace(path))
                    {
                        NodeChanged(_builder.GetConfigName(Path.GetFileName(path)));
                        try
                        {
                            //重新注册监控
                            //这里可能会存在Expired问题
                            var stat = ZooKeeper.Exists(path, true);
                            ZooKeeper.AddTmpChildNode(path, _clientName);
                            //按正常逻辑，最后更新的节点，mtime肯定比目前记录的mtime大，所以这里不进行Math.Max处理
                            _mtime = stat.Mtime;
                        }
                        catch (Exception)
                        {
                            //TODO:可能需要判断Expired
                        }
                    }
                    break;
            }
        }
    }
}