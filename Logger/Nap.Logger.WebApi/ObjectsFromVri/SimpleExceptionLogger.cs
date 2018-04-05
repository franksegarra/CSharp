using System.Web.Http.ExceptionHandling;

namespace Nap.Logger.WebApi.ObjectsFromVri
{
    public class SimpleExceptionLogger : ExceptionLogger
    {
        private readonly INapLogger _napLogger;

        public SimpleExceptionLogger(INapLogger napLogger)
        {
            _napLogger = napLogger;
        }

        public override void Log(ExceptionLoggerContext context)
        {
            if (context == null) return;

            _napLogger.Error("Unhandled exception", context.Exception);
        }
    }
}