# Disconf.Net
Disconf的.net版，含配置管理平台及客户端

部分无法运行的代码已经修复，可以直接上项目使用
     
  为了更好的解决分布式环境下多台服务实例的配置统一管理问题，本文提出了一套完整的分布式配置管理解决方案。结合.net项目具体情况，
实现了配置发布的统一化，对配置进行持久化管理并对外提供restful接口，在此基础上，基于ZooKeeper实现对配置更改的实时推送。
系统参考了百度的Disconf，实现和改进了部分功能，是Disconf的.Net精简版，功能有待进一步完善。
 
详细介绍请点击链接（本项目的原作者）
http://www.cnblogs.com/qkbao/p/6638721.html

# Web.config 或者App.config新增配置节点
``` csharp    
<configSections>
  <section name="disconfSections" type="Disconf.Net.Client.ClientConfigSection, Disconf.Net.Client" />
</configSections>
<disconfSections configSource="Configs\disconfSections.config" />
```

# disconfSections.config
``` csharp
<?xml version="1.0" encoding="utf-8" ?>
<disconfSections host="http://192.168.3.18:81/" enableRemote="true">
  <client appName="Quartz.Scheduler.Web" environment="TEST" version="1.0.0" clientName="Web.Test" />
  <updateStrategy fileIgnores="" itemIgnores="" startedSync="true" retryTimes="3" retryIntervalSeconds="10" />
  <preservation absolutePath="false" tmpRootDirectory="Tmp\Download\Configs" factRootDirectory="Configs" tmpItemsLocalName="~items.xml" tmpFilesLocalName="~files.txt"/>
</disconfSections>
```

# 具体参数说明请看源码 ClientConfigSection 的具体属性
``` csharp    
/// <summary>
/// Disconf.Net 的客户端配置参数
/// </summary>
public class ClientConfigSection : ConfigurationSection
{
    /// <summary>
    /// 自定义配置节点
    /// </summary>
    public static ClientConfigSection Current = (ClientConfigSection)ConfigurationManager.GetSection("disconfSections");

    /// <summary>
    /// Rest服务器域名地址
    /// </summary>
    [ConfigurationProperty("host", IsRequired = true)]
    public string WebApiHost => $"{this["host"]}";

    /// <summary>
    /// 是否启用远程配置，默认true，设为false的话表示不从远程服务器下载配置
    /// </summary>
    [ConfigurationProperty("enableRemote", DefaultValue = true)]
    public bool EnableRemote => Convert.ToBoolean(this["enableRemote"]);

    /// <summary>
    /// 业务系统信息设置
    /// </summary>
    [ConfigurationProperty("client", IsRequired = true)]
    public ClientInfoSection ClientInfo => (ClientInfoSection)this["client"];

    /// <summary>
    /// 更新策略设置
    /// </summary>
    [ConfigurationProperty("updateStrategy")]
    public UpdateStrategySection UpdateStrategy => (UpdateStrategySection)this["updateStrategy"];

    /// <summary>
    /// 本地持久化设置
    /// </summary>
    [ConfigurationProperty("preservation")]
    public PreservationSection Preservation => (PreservationSection)this["preservation"];

    /// <summary>
    /// 
    /// </summary>
    protected override void PostDeserialize()
    {
            Uri uri = new Uri(WebApiHost);//用来验证uri是否正确
            base.PostDeserialize();
    }
}
```
