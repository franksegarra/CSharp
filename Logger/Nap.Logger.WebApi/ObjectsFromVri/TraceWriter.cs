using System;
using System.Globalization;
using System.Net.Http;
using System.Web.Http.Tracing;

namespace Nap.Logger.WebApi.ObjectsFromVri
{
    public sealed class TraceWriter : ITraceWriter
    {
        private readonly INapLogger _napLogger;

        public TraceWriter(INapLogger napLogger)
        {
            _napLogger = napLogger;
        }

        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if (traceAction == null)
            {
                return;
            }

            var rec = new TraceRecord(request, category, level);
            traceAction(rec);

            WriteTrace(rec);
        }

        public void WriteTrace(TraceRecord rec)
        {
            if (rec == null)
            {
                return;
            }
            var format = ($"RequestId : {rec.RequestId} Kind: {rec.Kind} Status: {rec.Status} Operation: {rec.Operation} Operator: {rec.Operator} Category: {rec.Category} Request: {rec.Request} Message: {rec.Message}").ToString(CultureInfo.InvariantCulture);

            switch (rec.Level)
            {
                case TraceLevel.Off:
                    break;
                case TraceLevel.Debug:
                    _napLogger.Debug(format);
                    break;
                case TraceLevel.Info:
                    _napLogger.Information(format);
                    break;
                case TraceLevel.Warn:
                    _napLogger.Warning(format);
                    break;
                case TraceLevel.Error:
                    _napLogger.Error(format);
                    break;
                case TraceLevel.Fatal:
                    _napLogger.Fatal(format);
                    break;
                default:
                    return;
            }
        }
    }
}