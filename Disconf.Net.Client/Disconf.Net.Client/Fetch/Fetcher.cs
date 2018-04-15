using Disconf.Net.Core.Model;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using RestSharp;
using System;
using System.Net;

namespace Disconf.Net.Client.Fetch
{
    /// <summary>
    /// 
    /// </summary>
    public class Fetcher : IFetcher
    {
        private string _apiHost;
        private RetryPolicy _policy;
        const string GetConfigResource = "/api/ZooKeeper/GetConfig";
        const string GetAllConfigsResource = "/api/ZooKeeper/GetConfigs";
        const string GetZkHostsResource = "/api/ZooKeeper/GetZookeeperHost";
        const string GetLastChangedTimeResource = "/api/ZooKeeper/GetConfigLastTime";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiHost"></param>
        /// <param name="policy"></param>
        public Fetcher(string apiHost, RetryPolicy policy)
        {
            _apiHost = apiHost;
            _policy = policy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public string GetConfig(ConfigFetchFilter filter)
        {
            return CallApi(GetConfigResource, request => request.AddJsonBody(filter));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public string GetAllConfigs(FetchFilter filter)
        {
            return CallApi(GetAllConfigsResource, request =>
            {
                request.AddJsonBody(filter);
                request.Timeout = 300000;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetZkHosts()
        {
            return CallApi(GetZkHostsResource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private IRestResponse CallApi(IRestRequest request)
        {
            Func<IRestResponse> func = () =>
            {
                RestClient client = new RestClient(_apiHost);
                var res = client.Execute(request);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Fetch Fail");
                }
                return res;
            };
            IRestResponse response = null;
            //如果CallApi失败，抛出的异常由外部调用方，即ConfigManager来处理，该部分不与异常处理产生交集
            if (_policy != null)
            {
                response = _policy.ExecuteAction(func);
            }
            else
            {
                response = func();
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="act"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private string CallApi(string resource, Action<IRestRequest> act = null, Method method = Method.POST)
        {
            IRestRequest request = new RestRequest(resource, method);
            act?.Invoke(request);
            var response = CallApi(request);
            return response.Content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public string GetLastChangedTime(FetchFilter filter)
        {
            return CallApi(GetLastChangedTimeResource, request =>
            {
                request.AddJsonBody(filter);
            });
        }
    }
}