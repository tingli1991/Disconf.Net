namespace Disconf.Net.Application
{
    public interface IRequest<out T> where T : AbstractResponse
    {
    }
}