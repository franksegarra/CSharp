using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using System.Web.Http.Tracing;
using Microsoft.Practices.Unity;
using Nap.Logger.WebApi.Dependency;
using Nap.Logger.WebApi.ObjectsFromVri;
using Nap.Logger.WebApi.Resolver;

namespace Nap.Logger.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //******Dependency
            Registries.Register(config);
            //******

            // Web API configuration and services
            config.Services.Add(typeof(IExceptionLogger), new SimpleExceptionLogger(WebContainerManager.Get<INapLogger>()));
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            config.EnableSystemDiagnosticsTracing();
            config.Services.Replace(typeof(ITraceWriter), new TraceWriter(WebContainerManager.Get<INapLogger>()));

            var traceWriter = config.Services.GetTraceWriter();
            traceWriter.Trace(null, "Application Startup: Nap.Logger.WebApi", TraceLevel.Info, "{0}", "Application Startup: Nap.Logger.WebApi.");

            var constraintsResolver = new DefaultInlineConstraintResolver();
            constraintsResolver.ConstraintMap.Add("apiVersionConstraint", typeof(ApiVersionConstraint));
            constraintsResolver.ConstraintMap.Add("valuerangeWithStatus", typeof(ValueRangeWithStatusRouteConstraint));
            config.MapHttpAttributeRoutes(constraintsResolver);

            var naplogger = (INapLogger)config.DependencyResolver.GetService(typeof(INapLogger));
            naplogger.Fatal("Application Startup: " + ConfigurationManager.AppSettings["LogName"]);
        }
    }
}
