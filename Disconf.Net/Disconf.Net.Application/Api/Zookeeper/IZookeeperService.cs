namespace Disconf.Net.Application.Api.Zookeeper
{
    public interface IZookeeperService
    {
        ResponseWrapper<ZooKeeperResponse> GetZookeeperHost();
    }
}