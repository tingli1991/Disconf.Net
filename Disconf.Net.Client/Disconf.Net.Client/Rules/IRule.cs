namespace Disconf.Net.Client.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// 注册Rule规则，设置默认的属性映射
        /// </summary>
        /// <param name="configName">默认采用远程的configName</param>
        /// <returns></returns>
        IRule MapTo(string configName);

        /// <summary>
        /// 当远程配置的值发生变化时，通知值变更
        /// </summary>
        /// <param name="configName">配置文件名称</param>
        /// <param name="changedValue">配置文件值</param>
        void ConfigChanged(string configName, string changedValue);
    }
}