using DapperExtensions;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using Disconf.Net.Infrastructure.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Implementation
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;
        public UserServiceImpl(IUserRepository userRepository, IRoleService roleService)
        {
            _userRepository = userRepository;
            _roleService = roleService;
        }
        public async Task<bool> Delete(long id)
        {
            return await _userRepository.Delete<User>(id);
        }

        public async Task<User> Get(long id)
        {
            return await _userRepository.GetById<User>(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetList()
        {
            return await _userRepository.GetList<User>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> Insert(User model)
        {
            return await _userRepository.Insert<User>(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> Update(User model)
        {
            return await _userRepository.Update<User>(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<User> Login(User model)
        {
            return await GetUserByUserName(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<User> GetUserByUserName(User model)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            pg.Predicates.Add(Predicates.Field<User>(u => u.UserName, Operator.Eq, model.UserName));
            pg.Predicates.Add(Predicates.Field<User>(u => u.PassWord, Operator.Eq, UtilHelper.Md5(model.PassWord)));
            var list = await _userRepository.GetListWithCondition<User>(pg);
            var user = list.Any() ? list.Where(s => s.IsDelete == false).FirstOrDefault() : null;
            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetUserList(long userId)
        {
            var condition = new RoleCondition
            {
                CreateId = userId
            };
            var roleList = await _roleService.GetList(condition);
            var roleidList = string.Join(",", roleList.Select(s => s.Id.ToString()));
            return await _userRepository.GetUserList(roleidList);
        }
    }
}