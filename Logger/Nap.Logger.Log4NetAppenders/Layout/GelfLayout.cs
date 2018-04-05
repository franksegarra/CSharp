using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;
using Newtonsoft.Json;

namespace Nap.Logger.Log4NetAppenders.Layout
{
    public sealed class GelfLayout : LayoutSkeleton
    {
        private const string GelfVersion = "1.0";
        private const int ShortMessageLength = 250;

        private readonly PatternLayout _patternLayout;

        public GelfLayout()
        {
            Facility = Facility ?? "Gelf";
            IncludeLocationInformation = false;
            LogStackTraceFromMessage = true;
            IgnoresException = false;
            _patternLayout = new PatternLayout();
        }

        /// <summary>
        /// The content type output by this layout.
        /// </summary>
        public override string ContentType => "application/json";

        /// <summary>
        /// Gets or sets Facility.
        /// </summary>
        public string Facility { get; set; }

        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        public string HostName { get; set; }

        public string FieldSeparator { get; set; }

        public string KeyValueSeparator { get; set; }

        /// <summary>
        /// The additional fields configured in log4net config.
        /// </summary>
        public string AdditionalFields { get; set; }

        /// <summary>
        /// The conversion pattern to use for the message body
        /// </summary>
        public string ConversionPattern { get; set; }

        /// <summary>
        /// If should append the stack trace to the message if the message is an exception
        /// </summary>
        public bool LogStackTraceFromMessage { get; set; }

        /// <summary>
        /// Specifies wehter location inormation should be included in the message
        /// </summary>
        public bool IncludeLocationInformation { get; set; }

        public override void ActivateOptions()
        {
        }


        private Dictionary<string, object> ParseField(string value)
        {
            var innerAdditionalFields = new Dictionary<string, object>();
            if (value == null) return innerAdditionalFields;

            var fields = !string.IsNullOrEmpty(FieldSeparator) ? value.Split(new[] { FieldSeparator }, StringSplitOptions.RemoveEmptyEntries) : value.Split(',');

            if (!string.IsNullOrEmpty(KeyValueSeparator))
            {
                innerAdditionalFields = fields
                    .Select(it => it.Split(new[] { KeyValueSeparator }, StringSplitOptions.RemoveEmptyEntries))
                    .ToDictionary(it => it[0], it => (object)it[1]);
            }
            else
            {
                innerAdditionalFields = fields
                    .Select(it => it.Split(':'))
                    .ToDictionary(it => it[0], it => (object)it[1]);
            }
            return innerAdditionalFields;
        }

        public override void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            var gelfMessage = GetBaseGelfMessage(loggingEvent);
            AddLoggingEventToMessage(loggingEvent, gelfMessage);
            AddAdditionalFields(loggingEvent, gelfMessage);
            writer.Write(JsonConvert.SerializeObject(gelfMessage, Formatting.Indented));
        }

        private void AddLoggingEventToMessage(LoggingEvent loggingEvent, GelfMessage gelfMessage)
        {
            //If conversion pattern is specified then defer to PatterLayout for building the message body
            if (!string.IsNullOrWhiteSpace(ConversionPattern))
            {
                var message = GetValueFromPattern(loggingEvent, ConversionPattern);
                gelfMessage.FullMessage = message;
                gelfMessage.ShortMessage = message.TruncateMessage(ShortMessageLength);
            }
            else //Otherwise do our custom message builder stuff
            {
                var messageObject = loggingEvent.MessageObject;
                if (messageObject == null)
                {
                    if (!string.IsNullOrEmpty(loggingEvent.RenderedMessage))
                    {
                        if (loggingEvent.RenderedMessage.ValidateJson())
                        {
                            AddToMessage(gelfMessage, loggingEvent.RenderedMessage.ToJson().ToDictionary());
                        }

                        gelfMessage.FullMessage = !string.IsNullOrEmpty(gelfMessage.FullMessage) ? gelfMessage.FullMessage : loggingEvent.RenderedMessage;
                        gelfMessage.ShortMessage = !string.IsNullOrEmpty(gelfMessage.ShortMessage) ? gelfMessage.ShortMessage : gelfMessage.FullMessage.TruncateMessage(ShortMessageLength);

                        return;
                    }
                    gelfMessage.FullMessage = SystemInfo.NullText;
                    gelfMessage.ShortMessage = SystemInfo.NullText;
                }

                if (messageObject is string || messageObject is SystemStringFormat)
                {
                    var fullMessage = messageObject.ToString();
                    gelfMessage.FullMessage = fullMessage;
                    gelfMessage.ShortMessage = fullMessage.TruncateMessage(ShortMessageLength);
                }
                else if (messageObject is IDictionary)
                {
                    AddToMessage(gelfMessage, messageObject as IDictionary);
                }
                else
                {
                    AddToMessage(gelfMessage, messageObject.ToDictionary());
                }

                if (string.IsNullOrEmpty(gelfMessage.FullMessage) && messageObject != null)
                    gelfMessage.FullMessage = messageObject.ToString();

                gelfMessage.ShortMessage = !string.IsNullOrEmpty(gelfMessage.ShortMessage) ? gelfMessage.ShortMessage : gelfMessage.FullMessage.TruncateMessage(ShortMessageLength);
            }

            if (LogStackTraceFromMessage && loggingEvent.ExceptionObject != null)
            {
                gelfMessage.FullMessage = $"{gelfMessage.FullMessage} - {loggingEvent.GetExceptionString()}.";
            }
        }

        private GelfMessage GetBaseGelfMessage(LoggingEvent loggingEvent)
        {
            var message = new GelfMessage
            {
                Facility = Facility ?? "GELF",
                File = string.Empty,
                Host = HostName ?? Environment.MachineName,
                Level = GetSyslogSeverity(loggingEvent.Level),
                Line = string.Empty,
                TimeStamp = loggingEvent.TimeStamp,
                Version = GelfVersion
            };

            message.Add("LoggerName", loggingEvent.LoggerName);

            if (IncludeLocationInformation)
            {
                message.File = loggingEvent.LocationInformation.FileName;
                message.Line = loggingEvent.LocationInformation.LineNumber;
            }

            return message;
        }

        private static void AddToMessage(GelfMessage gelfMessage, IDictionary messageObject)
        {
            foreach (DictionaryEntry entry in messageObject)
            {
                var key = (entry.Key ?? string.Empty).ToString();
                var value = (entry.Value ?? string.Empty).ToString();
                if (FullMessageKeyValues.Contains(key, StringComparer.OrdinalIgnoreCase))
                    gelfMessage.FullMessage = value;
                else if (ShortMessageKeyValues.Contains(key, StringComparer.OrdinalIgnoreCase))
                    gelfMessage.ShortMessage = value.TruncateMessage(ShortMessageLength);
                else
                {
                    key = key.StartsWith("_") ? key : "_" + key;
                    gelfMessage[key] = FormatAdditionalField(entry.Value);
                }
            }
        }

        private void AddAdditionalFields(LoggingEvent loggingEvent, GelfMessage message)
        {
            var additionalFields = ParseField(AdditionalFields) ?? new Dictionary<string, object>();
            foreach (DictionaryEntry item in loggingEvent.GetProperties())
            {
                var key = item.Key as string;
                if (key != null && !key.StartsWith("log4net:") /*exclude log4net built-in properties */ )
                {
                    additionalFields.Add(key, FormatAdditionalField(item.Value));
                }
            }

            foreach (var kvp in additionalFields)
            {
                var key = kvp.Key.StartsWith("_") ? kvp.Key : "_" + kvp.Key;

                //If the value starts with a '%' then defer to the pattern layout
                var patternValue = kvp.Value as string;
                var value = patternValue != null && patternValue.StartsWith("%") ? GetValueFromPattern(loggingEvent, patternValue) : kvp.Value;
                message[key] = value;
            }
        }

        private static object FormatAdditionalField(object value)
        {
            return value == null || value.GetType().IsNumeric() ? value : value.ToString();
        }

        private string GetValueFromPattern(LoggingEvent loggingEvent, string pattern)
        {
            //Reset the pattern layout
            _patternLayout.ConversionPattern = pattern;
            _patternLayout.ActivateOptions();

            //Write the results
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                _patternLayout.Format(writer, loggingEvent);
                writer.Flush();
                return sb.ToString();
            }
        }

        private static long GetSyslogSeverity(Level level)
        {
            if (level == Level.Alert)
                return (long)LocalSyslogAppender.SyslogSeverity.Alert;

            if (level == Level.Critical || level == Level.Fatal)
                return (long)LocalSyslogAppender.SyslogSeverity.Critical;

            if (level == Level.Debug)
                return (long)LocalSyslogAppender.SyslogSeverity.Debug;

            if (level == Level.Emergency)
                return (long)LocalSyslogAppender.SyslogSeverity.Emergency;

            if (level == Level.Error)
                return (long)LocalSyslogAppender.SyslogSeverity.Error;

            if (level == Level.Fine
                || level == Level.Finer
                || level == Level.Finest
                || level == Level.Info
                || level == Level.Off)
                return (long)LocalSyslogAppender.SyslogSeverity.Informational;

            if (level == Level.Notice
                || level == Level.Verbose
                || level == Level.Trace)
                return (long)LocalSyslogAppender.SyslogSeverity.Notice;

            if (level == Level.Severe)
                return (long)LocalSyslogAppender.SyslogSeverity.Emergency;

            if (level == Level.Warn)
                return (long)LocalSyslogAppender.SyslogSeverity.Warning;

            return (long)LocalSyslogAppender.SyslogSeverity.Debug;
        }

        private static readonly IEnumerable<string> FullMessageKeyValues = new[] { "FULLMESSAGE", "FULL_MESSAGE", "MESSAGE" };
        private static readonly IEnumerable<string> ShortMessageKeyValues = new[] { "SHORTMESSAGE", "SHORT_MESSAGE", "MESSAGE" };
    }
}
