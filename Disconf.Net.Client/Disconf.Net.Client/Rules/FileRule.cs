using System;
using System.Configuration;
using System.IO;

namespace Disconf.Net.Client.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public class FileRule : IFileRule
    {
        /// <summary>
        /// 
        /// </summary>
        public FileRule()
        {
            AutoRefresh = true;
        }

        /// <summary>
        /// 要刷新的节点名称
        /// </summary>
        internal string SectionName { get; private set; }

        /// <summary>
        /// 是否自动刷新，默认自动刷新
        /// </summary>
        internal bool AutoRefresh { get; private set; }

        /// <summary>
        /// 远程配置值变更时要执行的委托
        /// </summary>
        internal Action Action { get; private set; }

        /// <summary>
        /// 配置改变成功后的回调方法
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IFileRule CallBack(Action action)
        {
            if (action != null)
            {
                Action += action;
            }
            return this;
        }

        /// <summary>
        /// 配置改变
        /// </summary>
        /// <param name="configName">配置文件名称</param>
        /// <param name="changedValue">配置文件值</param>
        public void ConfigChanged(string configName, string changedValue)
        {
            if (AutoRefresh)
            {
                ConfigurationManager.RefreshSection(SectionName);
            }
            Action?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshSectionName"></param>
        /// <returns></returns>
        public IFileRule MapTo(string refreshSectionName)
        {
            if (!string.IsNullOrWhiteSpace(refreshSectionName))
            {
                SectionName = Path.GetFileNameWithoutExtension(refreshSectionName);
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IFileRule RefreshIgnores()
        {
            AutoRefresh = false;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        IRule IRule.MapTo(string configName)
        {
            return MapTo(configName);
        }
    }
}