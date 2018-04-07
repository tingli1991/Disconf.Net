using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Models;
using Disconf.Net.Infrastructure.Helper;
using Disconf.Net.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using X.PagedList;

namespace Disconf.Net.Web.Controllers
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public class OLogController : BaseController
    {
        private readonly ILogService _logService;
        private readonly IUserService _userService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logService"></param>
        /// <param name="userService"></param>
        public OLogController(ILogService logService, IUserService userService)
        {
            this._logService = logService;
            this._userService = userService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(LogPagingFilteringVM command)
        {
            if (command == null)
            {
                command = new LogPagingFilteringVM();
            }

            command.Content = command.Content.RemoveSpace();
            var model = new LogListVM()
            {
                EndTime = command.EndTime,
                Content = command.Content,
                StartTime = command.StartTime,
                Logs = await GetLogPageList(command)
            };
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private async Task<IPagedList<OperationLog>> GetLogPageList(LogPagingFilteringVM command)
        {
            var total = await this._logService.GetLogTotal(command);
            var users = await this._userService.GetList();
            var userDic = users.ToDictionary(s => s.Id, v => v.Name);
            IEnumerable<OperationLog> logs;
            if (total > 0)
            {
                logs = await this._logService.GetLogList(PageSize, command);
            }
            else
            {
                logs = new List<OperationLog>();
            }

            logs.ToList().ForEach(s =>
            {
                s.Name = userDic[s.UId];
            });
            return new StaticPagedList<OperationLog>(logs, command.Page, PageSize, total);
        }
    }
}