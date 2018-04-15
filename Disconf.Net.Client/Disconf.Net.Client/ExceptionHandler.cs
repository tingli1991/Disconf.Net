using System;

namespace Disconf.Net.Client
{
    /// <summary>
    /// 执行处理类
    /// </summary>
    internal class ExceptionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public event Action<string, Exception> Faulted;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="action"></param>
        public void Execute(string configName, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (Faulted != null)
                {
                    try
                    {
                        //防止事件方法内部错误导致程序报错
                        Faulted(configName, ex);
                    }
                    catch { }
                }
            }
        }
    }
}