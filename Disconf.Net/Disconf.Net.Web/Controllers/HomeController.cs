using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Models;
using Disconf.Net.Model.Result;
using log4net;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Disconf.Net.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private static readonly ILog _log = LogManager.GetLogger(typeof(HomeController));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 用户登陆接口
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<JsonResult> UserLogin(User user)
        {
            var result = new BaseResult();
            try
            {
                var model = await _userService.Login(user);
                result.IsSuccess = model == null ? false : true;
                if (result.IsSuccess)
                {
                    Session["User"] = model;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"【用户登陆】用户：{user.UserName}登陆发生异常：", ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}