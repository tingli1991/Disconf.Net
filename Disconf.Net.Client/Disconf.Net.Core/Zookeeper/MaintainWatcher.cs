using Disconf.Net.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeper.Net;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// 负责zk节点设置的监控
    /// </summary>
    public class MaintainWatcher : ConnectWatcher
    {
        /// <summary>
        /// 重试时间间隔
        /// </summary>
        const int RetryIntervalMillisecond = 1000;

        /// <summary>
        /// 
        /// </summary>
        readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

        /// <summary>
        /// znode设置类
        /// </summary>
        /// <param name="connectionString">zookeeper连接字符串</param>
        /// <param name="timeOut">zookeeper session timout,单位毫秒(ms)</param>
        public MaintainWatcher(string connectionString, int timeOut)
            : base(connectionString, timeOut)
        {
            Task.Run(() => Execute());
        }

        /// <summary>
        /// 添加或者设置data
        /// </summary>
        /// <param name="zkPath"></param>
        /// <param name="data"></param>
        public void AddOrSetData(string zkPath, byte[] data)
        {
            _queue.Enqueue(() =>
            {
                var stat = ZooKeeper.Exists(zkPath, false);
                if (stat != null)
                {
                    //先删除子节点，再更新值保证不会出现客户端已经更新完并新增了节点，而服务端还没删完的情况
                    ZooKeeper.RemoveTmpChildNode(zkPath);
                    ZooKeeper.SetData(zkPath, data, -1);
                }
                else
                {
                    ZooKeeper.CreateWithPath(zkPath, data, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zkPath"></param>
        public void Remove(string zkPath)
        {
            _queue.Enqueue(() =>
            {
                //zookeeper在存在子节点时，不允许直接删除父节点，所以需要先删除子节点
                ZooKeeper.RemoveTmpChildNode(zkPath);
                ZooKeeper.Delete(zkPath, -1);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void Execute()
        {
            while (true)
            {
                if (_queue.TryPeek(out Action act) && Execute(act))
                {
                    while (!_queue.TryDequeue(out act))
                    {
                        //do nothing
                    }
                    continue;
                }
                Thread.Sleep(RetryIntervalMillisecond);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool Execute(Action action)
        {
            try
            {
                action();
            }
            catch (KeeperException)
            {
                return false;
            }
            catch (Exception)
            {
                //TODO: 非zk错误，记录日志
            }
            return true;
        }
    }
}