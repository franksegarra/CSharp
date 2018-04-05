using System.Net;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace Nap.Logger.WebApi.ObjectsFromVri
{
    /// <summary>
    /// place in WebApiConfig
    /// config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
    /// will send back the appropriate message to the client
    /// </summary>
    public sealed class GlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            if (context != null)
            {
                var exception = context.Exception;

                var httpException = exception as HttpException;
                if (httpException != null)
                {
                    context.Result = new ErrorResult(context.Request,
                        (HttpStatusCode)httpException.GetHttpCode(), httpException.Message);
                    return;

                }

                context.Result = new ErrorResult(context.Request, HttpStatusCode.InternalServerError, exception.Message);
            }
        }
    }
}