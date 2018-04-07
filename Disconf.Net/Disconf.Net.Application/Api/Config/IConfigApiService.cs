﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.Config
{
    public interface IConfigApiService
    {
        Task<ResponseWrapper<ConfigResponse>> GetConfigLastTime(ConfigRequest request);
        Task<ResponseWrapper<Dictionary<int, Dictionary<string, string>>>> GetConfigs(ConfigRequest request);
        Task<ResponseWrapper<string>> GetConfig(ConfigRequest request);
    }
}