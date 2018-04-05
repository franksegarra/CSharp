using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using Nap.Logger.WebApi.Controllers;
using NUnit.Framework;

namespace Nap.Logger.UnitTests
{
    [TestFixture]
    public class WebApiControllerTests
    {
        [Test]
        public void TestCallStatusMethodPositive()
        {
            // Arrange
            var host = ConfigurationManager.AppSettings["Host"];
            var env = (LogEnvironmentType)Enum.Parse(typeof(LogEnvironmentType), ConfigurationManager.AppSettings["LogEnvironment"]);
            var level = ConfigurationManager.AppSettings["LogLevel"];
            INapLogger napLogger = new NapLogger(host, env, level);

            var controller = new TestLoggingController(napLogger)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            // Act
            for (var i = 0; i < 20000; i++)
            {
                controller.PostDebug("Debug Message " + i + " from WebApi");
                controller.PostInformation("Info Message " + i + " from WebApi");
                controller.PostWarning("Warning Message " + i + " from WebApi");
                controller.PostError("Error Message " + i + " from WebApi");
                controller.PostFatal("Fatal Message " + i + " from WebApi");
            }

            //Assert
            Assert.IsTrue(true);
        }
    }
}
