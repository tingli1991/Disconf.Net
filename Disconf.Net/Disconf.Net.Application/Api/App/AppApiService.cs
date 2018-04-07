using Disconf.Net.Domain.Repositories;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.App
{
    /// <summary>
    /// 
    /// </summary>
    public class AppApiService : IAppApiService
    {
        private readonly IAppRepository _appRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appRepository"></param>
        public AppApiService(IAppRepository appRepository)
        {
            this._appRepository = appRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseWrapper<AppResponse>> GetAppIdByName(AppRequest request)
        {

            var response = new ResponseWrapper<AppResponse>();
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                response.Error = new ErrorResponse();
            }
            else
            {
                response.Result = new AppResponse
                {
                    AppId = await _appRepository.GetAppIdByName(request.Name)
                };
            }
            return response;
        }
    }
}