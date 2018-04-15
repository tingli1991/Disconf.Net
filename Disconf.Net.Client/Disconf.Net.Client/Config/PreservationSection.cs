using Disconf.Net.Client.Utils;
using System;
using System.Configuration;
using System.IO;

namespace Disconf.Net.Client
{
    /// <summary>
    /// 本地持久化设置相关
    /// </summary>
    public class PreservationSection : ConfigurationElement
    {
        /// <summary>
        /// 是否绝对路径，默认false
        /// 当false时，表示默认以AppDomain.CurrentDomain.BaseDirectory为比较点
        /// 注意：该配置同时适用于TmpRootDirectory、FactRootDirectory，即要么都只能绝对路径，要么都只能相对路径
        /// </summary>
        [ConfigurationProperty("absolutePath", DefaultValue = false)]
        public bool AbsolutePath => Convert.ToBoolean(this["absolutePath"]);

        /// <summary>
        /// 配置文件实际所在的根目录，默认值为Configs
        /// </summary>
        [ConfigurationProperty("factRootDirectory", DefaultValue = "Configs")]
        public string FactRootDirectory => $"{this["factRootDirectory"]}";

        /// <summary>
        /// 下载下来的配置临时保存文件夹根目录，默认为Tmp/Download/Configs
        /// </summary>
        [ConfigurationProperty("tmpRootDirectory", DefaultValue = "Tmp/Download/Configs")]
        public string TmpRootDirectory => $"{this["tmpRootDirectory"]}";

        /// <summary>
        /// 在临时目录下用于保存所有键值对的文件名，设置为空表示不保存
        /// 为方便服务器配置发生变更时进行对应值的修改，这里存储格式为xml
        /// 文件保存在TmpRootDirectory目录下，所以注意不要与实际配置文件名字冲突
        /// </summary>
        [ConfigurationProperty("tmpItemsLocalName", DefaultValue = "~items.xml")]
        public string TmpItemsLocalName => $"{this["tmpItemsLocalName"]}";

        /// <summary>
        /// 在临时目录下用于保存所有文件配置名的文件名，设置为空表示不保存
        /// 因为运行中不存在修改的可能性，所以此部分直接简单的存储为文本格式，多个文件名之间以,分隔
        /// 文件保存在TmpRootDirectory目录下，所以注意不要与实际配置文件名字冲突
        /// </summary>
        [ConfigurationProperty("tmpFilesLocalName", DefaultValue = "~files.txt")]
        public string TmpFilesLocalName => $"{this["tmpFilesLocalName"]}";

        /// <summary>
        /// 在临时目录下用于保存上次取到的最后一项配置修改时间，设置为空会导致每次都从服务器拉取全部配置
        /// </summary>
        [ConfigurationProperty("tmpTimeLocalName", DefaultValue = "~time.txt")]
        public string TmpTimeLocalName => $"{this["tmpTimeLocalName"]}";

        /// <summary>
        /// 
        /// </summary>
        public string TmpRootPhysicalPath
        {
            get { return GetPhysicalPath(TmpRootDirectory); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FactRootPhysicalPath
        {
            get { return GetPhysicalPath(FactRootDirectory); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetPhysicalPath(string path)
        {
            var physicalPath = path;
            if (!AbsolutePath)
            {
                physicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }
            return physicalPath;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PostDeserialize()
        {
            //节点读取成功后，初始化创建相应的文件夹，同时承担路径设置不对的校验
            if (string.Equals(TmpRootDirectory, FactRootDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("tmpRootDirectory and factRootDirectory can't be same.");
            }
            if (string.IsNullOrWhiteSpace(TmpRootDirectory))
            {
                throw new ArgumentException("tmpRootDirectory can't be empty.");
            }
            var tmpRoot = TmpRootPhysicalPath;
            var factRoot = FactRootPhysicalPath;
            DirectoryHelper.CreateDirectories(tmpRoot, factRoot);
            base.PostDeserialize();
        }
    }
}