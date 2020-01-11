using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using System;

namespace EventLink.Logging
{
    public abstract class LogBase
    {
        public abstract void Log(LogDb logDb, string parentName, string functionName, string message, LogLevel logLevel, bool isPrinting);
    }

    public class Logger : LogBase
    {
        private static readonly Lazy<Logger> instance =
            new Lazy<Logger>(() => new Logger());

        private static readonly LogService LogService = LogService.Instance;

        public static Logger Instance => instance.Value;

        private Logger()
        {

        }

        public override void Log(LogDb logDb, string parentName, string functionName, string message, LogLevel logLevel, bool isPrinting)
        {
            var logMessage = "[" + DateTime.Now + " | Database: " + logDb + " | Parent: " + parentName +
                             " | Function: " + functionName + " | " + logLevel + "]: " + message;
            var logObj = new Log(parentName, functionName, message, logLevel);

            /* Always log to the system log */
            LogService.CreateLog(LogDb.System, logObj);

            /* If the log is pointing to anywhere other than the system, log there as well. */
            if (!logDb.Equals(LogDb.System))
            {
                LogService.CreateLog(logDb, logObj);
            }

            if (isPrinting)
            {
                Console.WriteLine(logMessage);
            }
        }

    }
}