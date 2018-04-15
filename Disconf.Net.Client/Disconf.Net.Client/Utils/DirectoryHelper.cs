﻿using System.IO;

namespace Disconf.Net.Client.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// 批量创建文件夹
        /// </summary>
        /// <param name="paths"></param>
        public static void CreateDirectories(params string[] paths)
        {
            if (paths != null && paths.Length > 0)
            {
                foreach (var path in paths)
                {
                    DirectoryInfo d = new DirectoryInfo(path);
                    if (!d.Exists)
                    {
                        d.Create();
                    }
                }
            }
        }
    }
}