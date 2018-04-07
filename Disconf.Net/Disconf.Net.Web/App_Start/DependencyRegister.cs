using Autofac;
using Autofac.Extras.DynamicProxy2;
using Autofac.Integration.WebApi;
using Disconf.Net.Application;
using Disconf.Net.Application.Api;
using Disconf.Net.Repositories;
using System.Linq;
using System.Web.Configuration;
using System.Web.Http;

namespace Disconf.Net.Web.App_Start
{
    /// <summary>
    /// 
    /// </summary>
    public class DependencyRegister
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            // Call RegisterHttpRequestMessage to add the feature.
            builder.RegisterHttpRequestMessage(GlobalConfiguration.Configuration);

            // Register your Web API controllers.
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly);

            var connectionString = WebConfigurationManager.ConnectionStrings["DisconfConnectionString"].ConnectionString;
            builder.RegisterAssemblyTypes(typeof(BaseRepository).Assembly)
                .Where(t => t.IsSubclassOf(typeof(BaseRepository)))
                .AsImplementedInterfaces()
                .WithParameter((new TypedParameter(typeof(string), connectionString)))
                .EnableInterfaceInterceptors();

            builder.RegisterAssemblyTypes(typeof(IRequest<>).Assembly)
                .Where(t => t.Name.EndsWith("Request"))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly)
                .Where(t => t.Name.EndsWith("Impl"))
                .AsImplementedInterfaces();
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}