using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Enum;
using Disconf.Net.Domain.Models;
using Disconf.Net.Model.Result;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Disconf.Net.Web.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleService"></param>
        /// <param name="permissionService"></param>
        public RoleController(IRoleService roleService, IPermissionService permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> Get(long id)
        {
            var model = await _roleService.Get(id);
            var obj = new
            {
                Name = model.Name,
                PermissionIds = model.PermissionIds
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> Insert(Role model)
        {
            var result = new BaseResult();
            var list = await _roleService.GetList();
            var nameList = list.Where(s => s.Name == model.Name);
            if (nameList != null && nameList.Count() > 0)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该角色名已经存在";
            }
            else
            {
                var user = (User)Session["User"];
                model.CreateId = user.Id;
                result.IsSuccess = await _roleService.Insert(model);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetRoleList()
        {
            var user = (User)Session["User"];
            var condition = new RoleCondition
            {
                CreateId = user.Id,
                RoleId = user.RoleId
            };
            var list = await _roleService.GetList(condition);
            return Json(list.Where(s => s.Id > 1), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetList()
        {
            var user = (User)Session["User"];
            var condition = new RoleCondition
            {
                CreateId = user.Id,
                RoleId = user.RoleId
            };
            var list = await _roleService.GetList(condition);
            return Json(user.Id == 1 ? list : list.Where(s => s.Id > 1), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> Update(Role model)
        {
            var result = new BaseResult();
            var list = await _roleService.GetList();
            var nameList = list.Where(s => s.Name == model.Name);
            if (nameList != null && nameList.Count() > 0 && nameList.FirstOrDefault().Id != model.Id)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该角色名已经存在";
            }
            else
            {
                if (model.PermissionIds == null)
                    model.PermissionIds = string.Empty;
                model.CreateId = nameList.FirstOrDefault().CreateId;
                result.IsSuccess = await _roleService.Update(model);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(long id)
        {
            await _roleService.Delete(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetPerList()
        {
            var user = (User)Session["User"];
            var list = await _permissionService.GetList();
            var role = await _roleService.Get(user.RoleId);

            var permissionType = (int)PermissionType.App;
            var permissionIds = role.PermissionIds.Split('|').Select(e => long.Parse(e)).ToArray();//权限Id列表
            var permissionList = list.Where(s => s.PermissionType == permissionType && (permissionIds.Contains(s.Id) || role.CreateId == 0)).ToList();//权限列表
            var vmList = permissionList.Select(s => new
            {
                Id = s.Id,
                Name = s.Name,
                Children = list.Where(t => t.ParentId == s.Id).Select(f => new
                {
                    Id = f.Id,
                    Name = f.Name,
                    ParentId = f.ParentId
                }).ToList()
            });
            return Json(vmList, JsonRequestBehavior.AllowGet);
        }
    }
}