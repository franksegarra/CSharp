using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace NAPowerWebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
            logger.Info("NAPower Web Services Site Started", null);
        }

        protected void Application_Error()
        {
            logger.Fatal("Unhandled exception occurred", this.Server.GetLastError());
        }
    }
}
