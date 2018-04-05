using System;
using System.Web.Http;
using Nap.Logger.WebApi.ObjectsFromVri;

namespace Nap.Logger.WebApi.Controllers
{
    [ApiVersion1RoutePrefix("testlogging")]
    public sealed class TestLoggingController : ApiController
    {
        private readonly INapLogger _napLogger;

        public TestLoggingController(INapLogger napLogger)
        {
            _napLogger = napLogger;
        }

        [HttpPost]
        [Route("debug")]
        public IHttpActionResult PostDebug(string message)
        {
            _napLogger.Debug(message);
            return Ok();
        }

        [HttpPost]
        [Route("information")]
        public IHttpActionResult PostInformation(string message)
        {
            _napLogger.Information(message);
            return Ok();
        }

        [HttpPost]
        [Route("warning")]
        public IHttpActionResult PostWarning(string message)
        {
            _napLogger.Warning(message);
            return Ok();
        }

        [HttpPost]
        [Route("error")]
        public IHttpActionResult PostError(string message)
        {
            _napLogger.Error(message);
            return Ok();
        }

        [HttpPost]
        [Route("fatal")]
        public IHttpActionResult PostFatal(string message)
        {
            _napLogger.Fatal(message);
            return Ok();
        }
    }
}
