using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Nap.Logger.WebApi.ObjectsFromVri
{
    public sealed class ApiVersionConstraint : IHttpRouteConstraint
    {
        public ApiVersionConstraint(string allowedVersion)
        {
            AllowedVersion = !string.IsNullOrEmpty(allowedVersion) ? allowedVersion.ToLowerInvariant() : null;
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            object value;
            if (values != null && values.TryGetValue(parameterName, out value) && value != null)
            {
                if (!AllowedVersion.Equals(value.ToString().ToLowerInvariant()))
                {
                    const HttpStatusCode code = (HttpStatusCode)(int)ResponseCode.NotFound; //we only use a subset of codes
                    throw new HttpResponseException(request.CreateErrorResponse(code, string.Format("The version {0} is not supported.It must be v1.", value, CultureInfo.InvariantCulture)));
                }
            }

            return true;
        }

        public string AllowedVersion { get; private set; }
    }
}