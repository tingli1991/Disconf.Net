using Disconf.Net.Domain.Models;
using System;
using X.PagedList;

namespace Disconf.Net.Model.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class LogListVM
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 操作时间 End
        /// </summary>
        public DateTime? EndTime { get; set; }
        
        /// <summary>
        /// 关键词（也就是内容模糊查询）
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IPagedList<OperationLog> Logs { get; set; }
    }
}