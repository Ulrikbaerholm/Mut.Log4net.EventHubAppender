using System;
using System.Diagnostics;
using System.Text;
using fastJSON;
using log4net.Appender;
using log4net.Core;
using Microsoft.ServiceBus.Messaging;

namespace Mut.Log4net.EventHubAppender
{
    public class EventHubAppender : AppenderSkeleton
    {
        public string EventHubName { get; set; }
        public string EventHubConnectionString { get; set; }
        public string Environment { get; set; }
        public string Application { get; set; }
        public string Subsystem { get; set; }

        private Process _currentProcess;
        private EventHubClient _eventHubClient;

        public override void ActivateOptions()
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(EventHubConnectionString, EventHubName);
            _currentProcess = Process.GetCurrentProcess();
            base.ActivateOptions();
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            try
            {
                foreach (var loggingEvent in loggingEvents)
                {
                    var o = ConvertToAnonymous(loggingEvent);
                    var message = JSON.ToJSON(o, new JSONParameters() { DateTimeMilliseconds = true, EnableAnonymousTypes = true, SerializeNullValues = false, SerializeToLowerCaseNames = true, UseValuesOfEnums = true });
                    _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
            }
            catch
            {
                //NOOP
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                var eventData = ConvertToAnonymous(loggingEvent);
                var message = JSON.ToJSON(eventData, new JSONParameters() { DateTimeMilliseconds = true, EnableAnonymousTypes = true, SerializeNullValues = false, SerializeToLowerCaseNames = true, UseValuesOfEnums = true });
                _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
            }
            catch (Exception e)
            {
                // TODO
                throw;
            }

        }

        private object ConvertToAnonymous(LoggingEvent loggingEvent)
        {
            var exceptionString = loggingEvent.GetExceptionString();
            if (string.IsNullOrWhiteSpace(exceptionString))
            {
                exceptionString = null;
            }
            return new
            {
                level = loggingEvent.Level.DisplayName,
                time = loggingEvent.TimeStamp.ToString("yyyyMMdd HHmmss.fff zzz"),
                machine = System.Environment.MachineName,
                process = _currentProcess.ProcessName,
                thread = loggingEvent.ThreadName,
                message = loggingEvent.MessageObject,
                ex = exceptionString,
                application = this.Application,
                system = this.Subsystem,
                environment = this.Environment
            };
        }
    }
}
