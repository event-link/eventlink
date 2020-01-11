using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using EventLink.DataAccess.Util;
using MongoDB.Driver;
using System;

namespace EventLink.DataAccess
{
    public sealed class DbContext
    {
        private readonly string _dbConn = ConfigurationManager.AppSetting["ConnectionStrings:DbServerConnection"];
        private readonly string _dbName = ConfigurationManager.AppSetting["DbNames:EventLink"];

        private readonly string _eventColl = ConfigurationManager.AppSetting["CollectionNames:Event"];
        private readonly string _paymentColl = ConfigurationManager.AppSetting["CollectionNames:Payment"];
        private readonly string _userColl = ConfigurationManager.AppSetting["CollectionNames:User"];

        private readonly string _logSystemColl = ConfigurationManager.AppSetting["CollectionNames:Log:System"];
        private readonly string _logEventColl = ConfigurationManager.AppSetting["CollectionNames:Log:Event"];
        private readonly string _logPaymentColl = ConfigurationManager.AppSetting["CollectionNames:Log:Payment"];
        private readonly string _logUserColl = ConfigurationManager.AppSetting["CollectionNames:Log:User"];
        private readonly string _logStatisticsColl = ConfigurationManager.AppSetting["CollectionNames:Log:Statistics"];

        private static readonly Lazy<DbContext> Lazy = new Lazy<DbContext>(() => new DbContext());

        public static DbContext Instance => Lazy.Value;

        private static IMongoDatabase _db;

        private DbContext()
        {
            IMongoClient dbClient = new MongoClient(_dbConn);
            _db = dbClient.GetDatabase(_dbName);
        }

        public IMongoCollection<Event> GetEventCollection()
        {
            return _db.GetCollection<Event>(_eventColl);
        }

        public IMongoCollection<Log> GetLogCollection(LogDb logDb)
        {
            switch (logDb)
            {
                case LogDb.System:
                    return _db.GetCollection<Log>(_logSystemColl);
                case LogDb.Event:
                    return _db.GetCollection<Log>(_logEventColl);
                case LogDb.Payment:
                    return _db.GetCollection<Log>(_logPaymentColl);
                case LogDb.User:
                    return _db.GetCollection<Log>(_logUserColl);
                case LogDb.Statistics:
                    return _db.GetCollection<Log>(_logStatisticsColl);
                default:
                    return null;
            }
        }

        public IMongoCollection<Payment> GetPaymentCollection()
        {
            return _db.GetCollection<Payment>(_paymentColl);
        }

        public IMongoCollection<User> GetUserCollection()
        {
            return _db.GetCollection<User>(_userColl);
        }
    }
}