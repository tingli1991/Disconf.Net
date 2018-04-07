using System.Runtime.Serialization;

namespace Disconf.Net.Application.Api.App
{
    [DataContract]
    public class AppRequest : IRequest<AppResponse>
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
