using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JZ.App.WebHost.Common
{
    public class AutofacConfig
    {
        public static void Register()
        {
            //var configuration = GlobalConfiguration.Configuration;
            //var builder = new ContainerBuilder();
            //builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            //builder.RegisterWebApiFilterProvider(configuration);



            ////1.0连接字符串
            //foreach (var con in ConfigHelper.GetConnStrings())
            //    builder.Register(c => new SqlConnection(con.Value))
            //        .Named<IDbConnection>(con.Key)
            //        .InstancePerRequest();
            //builder.Register<Func<string, IDbConnection>>(c =>
            //{
            //    var ic = c.Resolve<IComponentContext>();
            //    return named => ic.ResolveNamed<IDbConnection>(named);
            //});
            ////2.0工作单元
            //builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            ////3.0仓储
            //builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerRequest();
            ////4.0业务逻辑
            //var assemblies = BuildManager.GetReferencedAssemblies()
            //   .Cast<Assembly>()
            //   .Where(a => a.GetTypes().FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IService))) != null)
            //   .ToArray();
            //builder.RegisterAssemblyTypes(assemblies)
            //    .Where(t => t.GetInterfaces().Contains(typeof(IService)))
            //    .AsSelf()
            //    .InstancePerRequest();



            //RedisProxy服务,MemcacheProxy服务,ServiceBus

            //RedisProxy.Initialize();
            //var redis = new RedisProxy();
            //builder.Register(c => redis).As<ICache>().SingleInstance();
            //builder.Register(c => redis).As<IRedisProxy>().SingleInstance();
            //builder.RegisterType<MemcacheProxy>().As<IMemcacheProxy>().SingleInstance();
            //builder.RegisterType<ServiceBus>().As<IServiceBus>().SingleInstance();

            #region API Processor Register

            //builder.RegisterType<ProcessorFactory>().As<IProcessorFactory>().InstancePerRequest();

            //foreach (var type in ProcessorUtil.GetProcessors())
            //{
            //    builder.RegisterType(type)
            //        .Named<IProcessor>(ProcessorUtil.GetBizCode(type))
            //        .InstancePerRequest();
            //}

            //builder.Register<Func<string, IProcessor>>(c =>
            //{
            //    var ic = c.Resolve<IComponentContext>();
            //    return name => ic.ResolveNamed<IProcessor>(name);
            //});

            #endregion

            //var container = builder.Build();
            //configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}