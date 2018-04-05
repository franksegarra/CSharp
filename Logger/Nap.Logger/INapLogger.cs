using System;

namespace Nap.Logger
{
    public interface INapLogger
    {
        void Debug(string message);
        void Debug(string message, Exception exception);
        void DebugFormat(IFormatProvider provider, string format, params object[] args);
        void Error(string message);
        void Error(string message, Exception exception);
        void Error(IFormatProvider provider, string format, params object[] args);
        void Fatal(string message);
        void Fatal(string message, Exception exception);
        void Fatal(IFormatProvider provider, string format, params object[] args);
        void Information(string message);
        void Information(string message, Exception exception);
        void Information(IFormatProvider provider, string format, params object[] args);
        void Warning(string message);
        void Warning(string message, Exception exception);
        void Warning(IFormatProvider provider, string format, params object[] args);
    }
}