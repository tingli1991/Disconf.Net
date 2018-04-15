using System;
using System.Threading;

namespace Disconf.Net.Core.Utils
{
    /// <summary>
    /// 仿Java的CountDownLatch
    /// </summary>
    public class CountDownLatch
    {
        int _count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        public CountDownLatch(int count)
        {
            _count = count;
        }

        /// <summary>
        /// 
        /// </summary>
        public void CountDown()
        {
            if (_count > 0)
            {
                Interlocked.Decrement(ref _count);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecond"></param>
        public void Await(long millisecond = 0)
        {
            var intervalMs = 10;
            if (millisecond <= 0)
            {
                millisecond = long.MaxValue;
            }
            var count = Math.Ceiling(millisecond * 1.0 / intervalMs);
            for (var i = 0; i < count; i++)
            {
                if (_count <= 0)
                {
                    return;
                }
                Thread.Sleep(intervalMs);
            }
            throw new TimeoutException("CountDownLatch is timeout");
        }
    }
}