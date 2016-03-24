using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using Autofac;
using Framework.DAL.SqlServer;
using FrameWork.Caching;
using FrameWork.DAL;
using FrameWork.Redis;
using FrameWork.Utils;

namespace FrameWork.AutoFac
{
    public class AutoFacHelper
    {
        public static ContainerBuilder GetContainner(bool isRegisterIService = false,
            bool isRegisterProcess= false)
        {
            var builder = new ContainerBuilder();
            //注册
            foreach (var con in ConfigHelper.GetConnStrings())
            {
                builder.Register(c => new SqlConnection(con.Value))
                    .Named<IDbConnection>(con.Key)
                    .InstancePerLifetimeScope();
            }
            //注册工作单元
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            //注册IDbConnection
            builder.Register<Func<string, IDbConnection>>(c => named => c.ResolveNamed<IDbConnection>(named)).
                InstancePerLifetimeScope();
            //注册Repository
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            if (isRegisterIService)
            {
                //依赖程序集注入IService接口
                //var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>()
                //.Where(a => a.GetTypes().FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IService)))!= null).ToArray();
                //builder.RegisterAssemblyTypes(assemblies)
                //    .Where(t => t.GetInterfaces().Contains(typeof(IService)))
                //    .AsSelf()
                //    .InstancePerRequest();
            }
            //注册RedisProxy
            //注册Process（WebApi专用）
            //RedisProxy.Initialize();
            //var redis = new RedisProxy();
            //builder.Register(c => redis).As<ICache>().SingleInstance();
            //builder.Register(c => redis).As<IRedisProxy>().SingleInstance();

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
            return builder;
        }
    }
}
