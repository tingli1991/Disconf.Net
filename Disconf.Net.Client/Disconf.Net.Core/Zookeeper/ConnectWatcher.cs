using Disconf.Net.Core.Utils;
using System;
using System.Threading;
using ZooKeeper.Net;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// 这个类主要负责监控zookeeper的连接
    /// </summary>
    public class ConnectWatcher : IWatcher, IDisposable
    {
        CountDownLatch latch;
        private int _sessionTimeOut;
        private string _connectionString;
        static object lockObj = new object();

        /// <summary>
        /// 因为程序不可能无限等待，所以需要设置等待zookeeper连接超时的时间，单位毫秒
        /// 至于zookeeper客户端自身实际似乎是会进行无限制连接尝试的
        /// </summary>
        private int _connectTimeOut = 3000;

        /// <summary>
        /// 重连时的时间间隔，单位毫秒
        /// </summary>
        private int _retryIntervalMillisecond = 3000;

        /// <summary>
        /// 该Watcher监控的zookeeper客户端
        /// </summary>
        public ZooKeeper.Net.ZooKeeper ZooKeeper { get; private set; }

        /// <summary>
        /// 用于监控连接的Zookeeper Watcher
        /// </summary>
        /// <param name="connectionString">zookeeper连接字符串</param>
        /// <param name="timeOut">zookeeper session timout,单位毫秒(ms)</param>
        public ConnectWatcher(string connectionString, int timeOut)
        {
            _connectionString = connectionString;
            _sessionTimeOut = timeOut;
            Connect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        public virtual void Process(WatchedEvent @event)
        {
            switch (@event.State)
            {
                case KeeperState.SyncConnected:
                    //测试下来连接和断开连接无论你是否重新注册，都一定会进行通知，而且不论对多少个节点注册了watch，都只会触发一次
                    latch.CountDown();
                    break;
                case KeeperState.Expired:
                    ReConnect();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Connect()
        {
            latch = new CountDownLatch(1);
            ZooKeeper = new ZooKeeper.Net.ZooKeeper(_connectionString, TimeSpan.FromMilliseconds(_sessionTimeOut), this);
            try
            {
                latch.Await(_connectTimeOut);
            }
            catch (Exception)
            {
                //Connect TimeOut
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ReConnect()
        {
            lock (lockObj)
            {
                while (true)
                {
                    try
                    {
                        if (ZooKeeper != null && !States.CLOSED.Equals(ZooKeeper.State))
                        {
                            break;
                        }
                        Dispose();
                        Connect();
                        ReConnectCallBack();
                    }
                    catch
                    {
                        Thread.Sleep(_retryIntervalMillisecond);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void ReConnectCallBack()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (ZooKeeper != null)
            {
                ZooKeeper.Dispose();
            }
        }
    }
}