using EventLink.DataAccess.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace EventLink.DataAccess.Services
{

    public enum LogDb
    {
        System = 1,    /* Logs everything that is related to the system */
        Event = 2,     /* Logs everything related to events (Crawler, fetching of events, etc.) */
        Payment = 3,   /* Logs everything related to payments (Orders, etc.) */
        User = 4,      /* Logs everything related to users (User updates, authentication, etc) */
        Statistics = 5 /* Logs everything related to statistics (Crawler statistics, user auth tries, user activity, etc.) */
    }

    public interface ILogService
    {
        Log GetLog(LogDb logDb, string id);
        IEnumerable<Log> GetLogs(LogDb logDb);
        void CreateLog(LogDb logDb, Log logObj);
        void UpdateLog(LogDb logDb, Log logObj);
        void DeleteLog(LogDb logDb, Log logObj);
        void DeleteLog(LogDb logDb, string id);
        /* DestroyLog methods will DELETE data permanently. */
        /* These methods should ONLY be used for unit testing. Not in production. */
        void DestroyLog(LogDb logDb, Log logObj);
        void DestroyLog(LogDb logDb, string id);
        void CheckNullLog(Log logObj);
        void CheckEssentialLogData(Log logObj);
    }

    public class LogService : ILogService
    {
        private static readonly DbContext DbContext = DbContext.Instance;

        private static readonly Dictionary<LogDb, IMongoCollection<Log>> logColls
            = new Dictionary<LogDb, IMongoCollection<Log>>()
        {
            { LogDb.System, DbContext.GetLogCollection(LogDb.System) },
            { LogDb.Event, DbContext.GetLogCollection(LogDb.Event) },
            { LogDb.Payment, DbContext.GetLogCollection(LogDb.Payment) },
            { LogDb.User, DbContext.GetLogCollection(LogDb.User) },
            { LogDb.Statistics, DbContext.GetLogCollection(LogDb.Statistics) },
        };

        private static readonly Lazy<LogService> instance =
            new Lazy<LogService>(() => new LogService());

        public static LogService Instance => instance.Value;

        private LogService()
        {

        }

        public Log GetLog(LogDb logDb, string id)
        {
            Log logDoc;

            /* If the Log Id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException("Log Id is null or empty!");
            }

            /* Tries to find the first Log object with the given Id */
            try
            {
                logDoc = logColls[logDb].Find(l => l.Id == id).FirstOrDefault();
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception comes from DB, throw exception */
                throw new DAException(e.Message, e);
            }

            /*
             * If the Log document is null, then we assume that a Log with the given
             * id does not exist in the database.
             */
            if (logDoc == null)
            {
                throw new DADocNotFoundException("Log Id (" + id + ") was not found!");
            }

            return logDoc;
        }

        public IEnumerable<Log> GetLogs(LogDb logDb)
        {
            IEnumerable<Log> logDocList;

            /* Tries to find all Log documents */
            try
            {
                logDocList = logColls[logDb].Find("{}").ToList(); // {} has same effect as * in SQL (wildcard)
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message);
            }

            /* If the Log list is still null, then return exception */
            if (logDocList == null)
            {
                throw new DADocNotFoundException("List of Log objects is null!");
            }

            return logDocList;
        }

        public void CreateLog(LogDb logDb, Log logObj)
        {
            /* Validate object */
            CheckNullLog(logObj);
            CheckEssentialLogData(logObj);

            /* Creates a new Log document */
            try
            {
                /* If the log already is created throw exception. */
                if (logObj.Id != null && logColls[logDb].Find(l => l.Id == logObj.Id).FirstOrDefault() != null)
                {
                    throw new DADocAlreadyExistsException("Log document already exists!");
                }

                logObj.DbCreatedDate = DateTime.Now;
                logObj.DbModifiedDate = DateTime.Now;
                logColls[logDb].InsertOne(logObj);
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + logObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void UpdateLog(LogDb logDb, Log logObj)
        {
            /* Validate the Log object */
            CheckNullLog(logObj);

            /* If the Log Id is null or empty */
            if (string.IsNullOrEmpty(logObj.Id))
            {
                throw new DANullOrEmptyIdException("Log Id is null or empty!");
            }

            /* Checks whether the Log document to update exists in the DB or not. */
            try
            {
                if (logColls[logDb].Find(l => l.Id == logObj.Id).FirstOrDefault() == null)
                {
                    throw new DADocNotFoundException("Log with Id (" + logObj.Id + ") was not found!");
                }
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + logObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            CheckEssentialLogData(logObj);

            /* Tries to update a Log document */
            try
            {
                logObj.DbModifiedDate = DateTime.Now;
                logColls[logDb].ReplaceOne(l => l.Id == logObj.Id, logObj);
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + logObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception comes from DB, throw exception */
                throw new DAException(e.Message);
            }
        }

        public void DeleteLog(LogDb logDb, Log logObj)
        {
            /* Validate Log object */
            CheckNullLog(logObj);

            /* Checks whether the Log document exists */
            try
            {
                if (logColls[logDb].Find(e => e.Id == logObj.Id).FirstOrDefault() == null)
                {
                    throw new DADocNotFoundException("Log Id (" + logObj.Id + ") was not found!");
                }
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + logObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            DeleteLog(logDb, logObj.Id);
        }

        public void DeleteLog(LogDb logDb, string id)
        {
            /* If Log Id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Find the document in the database */
            var logObj = GetLog(logDb, id);

            /* Validate the object */
            CheckNullLog(logObj);

            /* Tries to set Log document inactive */
            try
            {
                logObj.IsDeleted = true;
                logObj.DbDeletedDate = DateTime.Now;
                logObj.DbModifiedDate = DateTime.Now;
                UpdateLog(logDb, logObj);
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void DestroyLog(LogDb logDb, Log logObj)
        {
            if (!SharedConstants.UnitTestMode)
            {
#pragma warning disable CS0162 // Unreachable code detected
                return;
#pragma warning restore CS0162 // Unreachable code detected
            }

            /* Validate the Log object */
            CheckNullLog(logObj);

            /* If Log Id is null or empty */
            if (string.IsNullOrEmpty(logObj.Id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Deletes the document from the DB */
            DestroyLog(logDb, logObj.Id);
        }

        public void DestroyLog(LogDb logDb, string id)
        {
            if (!SharedConstants.UnitTestMode)
            {
#pragma warning disable CS0162 // Unreachable code detected
                return;
#pragma warning restore CS0162 // Unreachable code detected
            }

            /* If Log Id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Deletes the document from the DB */
            try
            {
                logColls[logDb].DeleteOne(e => e.Id == id);
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void CheckNullLog(Log logObj)
        {
            /* If Log object is null, throw exception */
            if (logObj == null)
            {
                throw new DAException("Log object is null!");
            }
        }

        public void CheckEssentialLogData(Log logObj)
        {
            /* Checks whether essential data for the Log object is intact */
            if (string.IsNullOrEmpty(logObj.ParentName) || string.IsNullOrEmpty(logObj.FunctionName))
            {
                throw new DAException("All or some essential Log data has not been found!");
            }
        }

    }
}