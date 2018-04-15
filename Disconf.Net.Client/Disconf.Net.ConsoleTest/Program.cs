using System;

namespace Disconf.Net.ConsoleTest
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        public static string Person { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DisConfigRules.Register();
            Console.WriteLine("press any key to exist...");
            Console.ReadKey();
        }
    }
}