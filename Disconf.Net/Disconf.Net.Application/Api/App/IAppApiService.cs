using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.App
{
    public interface IAppApiService
    {
        Task<ResponseWrapper<AppResponse>> GetAppIdByName(AppRequest request);
    }
}
