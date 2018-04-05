using System;
using System.Net;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace Nap.Logger.Log4NetAppenders.Appenders
{
    public class NapGelfAppender : AppenderSkeleton
    {
        #region privates

        private Uri _host;

        #endregion privates

        #region public properties

        public string Host { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        #endregion public properties

        #region ctor

        public NapGelfAppender(string host)
        {
            Host = host;
        }

        #endregion ctor

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            _host = new Uri(Host);
            ServicePointManager.FindServicePoint(_host).Expect100Continue = false;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var payload = RenderLoggingEvent(loggingEvent);
            var byteArray = Encoding.UTF8.GetBytes(payload);

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(_host);
                req.ServicePoint.Expect100Continue = false;
                req.Method = "POST";
                req.ContentType = "application/json; charset=UTF-8";
                req.ContentLength = byteArray.Length;
                req.Expect = "";
                using (var reqs = req.GetRequestStream())
                {
                    reqs.Write(byteArray, 0, byteArray.Length);
                }

                var response = (HttpWebResponse)req.GetResponse();

                if (response.StatusCode != HttpStatusCode.Accepted)
                {
                    //Log to eventlog?
                    ErrorHandler.Error("Unable to send logging event to remote host " + Host);
                }

                #region comments
                //using (var client = new HttpClient())
                //{
                //    var postdataJson = JsonConvert.SerializeObject(loggingEvent);
                //    var postdataString = new StringContent(postdataJson, new UTF8Encoding(), "application/json");
                //    var responseMessage = client.PostAsync(_host, postdataString).Result;
                //    var responseString = responseMessage.Content.ReadAsStringAsync().Result;
                //}

                //using (var webClient = new WebClient())
                //{
                //    if (!string.IsNullOrEmpty(_credentials))
                //    {
                //        webClient.Headers[HttpRequestHeader.Authorization] = $"Basic {_credentials}";
                //    }

                //    webClient.UploadString(_host, payload);
                //    //webClient.UploadStringAsync(_host, payload);
                //}
                #endregion comments
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Unable to send logging event to remote host " + Host, ex);
            }
        }
    }
}
