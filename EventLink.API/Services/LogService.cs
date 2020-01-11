using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using System;
using System.Collections.Generic;

namespace EventLink.API.Services
{
    public interface ILogService
    {
        Log GetLog(LogDb logDb, string id);
        IEnumerable<Log> GetLogs(LogDb logDb);
        void CreateLog(LogDb logDb, Log logObj);
    }

    public class LogService : ILogService
    {
        private static readonly Lazy<ILogService> instance =
            new Lazy<ILogService>(() => new LogService());

        public static ILogService Instance => instance.Value;

        private readonly DataAccess.Services.LogService _logService = DataAccess.Services.LogService.Instance;

        private LogService()
        {

        }

        public Log GetLog(LogDb logDb, string id)
        {
            try
            {
                return _logService.GetLog(logDb, id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public IEnumerable<Log> GetLogs(LogDb logDb)
        {
            try
            {
                return _logService.GetLogs(logDb);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public void CreateLog(LogDb logDb, Log logObj)
        {
            try
            {
                _logService.CreateLog(logDb, logObj);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

    }
}