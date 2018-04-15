using Disconf.Net.Client;
using System;
using System.Collections.Specialized;
using System.Configuration;
using Disconf.Net.Core.Utils;
using System.Diagnostics;

namespace Disconf.Net.ConsoleTest
{
    /// <summary>
    /// 
    /// </summary>
    class DisConfigRules
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Register()
        {
            Register(ConfigManager.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        public static void Register(ConfigManager manager)
        {
            //要更新的文件
            manager.FileRules.For("appSettings.config").CallBack(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start(); // 开始监视代码运行时间
                for (int i = 0; i < 10000000; i++)
                {
                    var appSettings = ConfigManagerExtend.GetConfigurationSection<NameValueCollection>("appSettings");
                    var str = appSettings.GetNameValue<string>("aaa");
                }
                stopwatch.Stop(); // 停止监视
                TimeSpan timespan = stopwatch.Elapsed; // 获取当前实例测量得出的总时间
                string hours = timespan.TotalHours.ToString("#0.00000000 "); // 总小时
                string minutes = timespan.TotalMinutes.ToString("#0.00000000 "); // 总分钟
                string seconds = timespan.TotalSeconds.ToString("#0.00000000 "); // 总秒数
                string milliseconds = timespan.TotalMilliseconds.ToString("#0.00000000 "); // 总毫秒数
                Console.WriteLine("File changed at:{0:yy-MM-dd HH:mm:ss fff}", DateTime.Now);
            });

            manager.FileRules.For("connectionStrings.config").CallBack(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start(); // 开始监视代码运行时间
                for (int i = 0; i < 10000000; i++)
                {
                    var appSettings = ConfigurationManager.ConnectionStrings;
                    var str = appSettings.GetNameValue("DisconfConnectionString");
                }
                stopwatch.Stop(); // 停止监视
                TimeSpan timespan = stopwatch.Elapsed; // 获取当前实例测量得出的总时间
                string hours = timespan.TotalHours.ToString("#0.00000000 "); // 总小时
                string minutes = timespan.TotalMinutes.ToString("#0.00000000 "); // 总分钟
                string seconds = timespan.TotalSeconds.ToString("#0.00000000 "); // 总秒数
                string milliseconds = timespan.TotalMilliseconds.ToString("#0.00000000 "); // 总毫秒数
                Console.WriteLine("File changed at:{0:yy-MM-dd HH:mm:ss fff}", DateTime.Now);
            });

            //要更新的键值对
            manager.ItemRules.For("PropMap").MapTo("Person").SetStaticProperty<Program>().CallBack(v =>
            {
                Console.WriteLine("Now item value:{0}", v);
                Console.WriteLine("Program.Person is {0} now", Program.Person);
            });

            //忽略更新到本地的键值对
            manager.ItemRules.For("Peng").CallBack(v =>
            {
                Console.WriteLine("Peng 's value is:{0}", v);
            });

            manager.NodeChanged += Manager_NodeChanged;
            manager.Exception += Manager_Faulted;
            manager.Init();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="configValue"></param>
        private static void Manager_NodeChanged(string configName, string configValue)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        private static void Manager_Faulted(Exception ex)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(ex);
            Console.ForegroundColor = color;
        }
    }
}