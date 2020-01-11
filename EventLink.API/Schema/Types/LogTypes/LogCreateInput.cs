using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;

namespace EventLink.API.Schema.Types.LogTypes
{
    public class LogCreateInput
    {
        public LogDb LogDb { get; set; }

        public string ParentName { get; set; }

        public string FunctionName { get; set; }

        public string Message { get; set; }

        public LogLevel LogLevel { get; set; }
    }
}