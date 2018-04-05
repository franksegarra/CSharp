using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Nap.Logger.WebApi.ObjectsFromVri
{
    public class ValueRangeWithStatusRouteConstraint : IHttpRouteConstraint
    {
        private readonly int _from;
        private readonly int _length;
        private readonly HttpStatusCode _statusCode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="length"></param>
        /// <param name="statusCode"></param>
        public ValueRangeWithStatusRouteConstraint(int from, int length, string statusCode)
        {
            _from = from;
            //_to = to;
            _length = length;
            if (!Enum.TryParse(statusCode, true, out _statusCode))
            {
                _statusCode = HttpStatusCode.NotFound;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="route"></param>
        /// <param name="parameterName"></param>
        /// <param name="values"></param>
        /// <param name="routeDirection"></param>
        /// <returns></returns>
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values,
            HttpRouteDirection routeDirection)
        {
            object value;
            if (values != null)
                if (values.TryGetValue(parameterName, out value) && value != null)
                {
                    var stringValue = value as string;
                    var intValue = 0;
                    if (stringValue != null && int.TryParse(stringValue, out intValue) && (stringValue.Length == _length))
                    {
                        if (intValue >= _from)
                        {
                            return true;
                        }
                    }
                }
            throw new HttpResponseException(request.CreateErrorResponse(_statusCode, "Invalid TPV Code."));

        }

    }
}