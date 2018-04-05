using System;
using System.Configuration;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Nap.Logger.WebApi.Resolver;

namespace Nap.Logger.WebApi.Dependency
{
    internal static class Registries
    {
        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();

            //Why use HierarchicalLifetimeManager?  Wouldn't we want one logger logging everything for the Api?
            container.RegisterType<INapLogger, NapLogger>(new ContainerControlledLifetimeManager(), 
                                                            new InjectionConstructor( ConfigurationManager.AppSettings["Host"], 
                                                                                      (LogEnvironmentType)Enum.Parse(typeof(LogEnvironmentType), ConfigurationManager.AppSettings["LogEnvironment"]), 
                                                                                      ConfigurationManager.AppSettings["LogLevel"]
                                                                                    ));

            config.DependencyResolver = new UnityResolver(container);
        }
    }
}