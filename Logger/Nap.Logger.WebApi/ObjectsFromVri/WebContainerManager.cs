using System;
using System.Globalization;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace Nap.Logger.WebApi.ObjectsFromVri
{
    public static class WebContainerManager
    {
        /// <summary>
        ///     Provides access to the dependency resolver.
        /// </summary>
        public static IDependencyResolver DependencyResolver
        {
            get
            {
                var dependencyResolver = GlobalConfiguration.Configuration.DependencyResolver;
                if (dependencyResolver != null)
                {
                    return dependencyResolver;
                }

                throw new InvalidOperationException("The dependency resolver has not been set.");
            }
        }

        /// <summary>
        ///  Provides access to a specific type of dependency managed by the IDependencyResolver. Use only
        ///  where access to the resolver is not convenient/possible.
        /// </summary>
        public static T Get<T>()
        {
            var service = DependencyResolver.GetService(typeof(T));

            if (service == null)
                throw new VriException(($"Requested service of type {typeof(T).FullName}, but null was found.").ToString(CultureInfo.InvariantCulture));

            return (T)service;
        }
    }
}