using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace EventLink.DataAccess.Models
{
    public enum LogLevel
    {
        Debug,  // DEBUG: Additional information about application behavior for cases when that information is necessary to diagnose problems.
        Info,   // INFO:  Application events for general purposes.
        Warn,   // WARN:  Application events that may be an indication of a problem.
        Error,  // ERROR: Typically logged in the catch block a try/catch block, includes the exception and contextual data.
        Fatal,  // FATAL: A critical error that results in the termination of an application.
        Trace   // TRACE: Used to mark the entry and exit of functions, for purposes of performance profiling.
    }

    public class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ParentName")]
        public string ParentName { get; set; }

        [BsonElement("FunctionName")]
        public string FunctionName { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }

        [BsonElement("LogLevel")]
        public LogLevel LogLevel { get; set; }

        [BsonElement("DbCreatedDate")]
        public DateTime DbCreatedDate { get; set; }

        [BsonElement("DbModifiedDate")]
        public DateTime DbModifiedDate { get; set; }

        [BsonElement("DbDeletedDate")]
        public DateTime DbDeletedDate { get; set; }

        [BsonElement("DbReactivatedDate")]
        public DateTime DbReactivatedDate { get; set; }

        [BsonElement("IsDeleted")]
        public bool IsDeleted { get; set; }

        [JsonConstructor]
        public Log(string parentName, string functionName,
            string message, LogLevel logLevel)
        {
            ParentName = parentName;
            FunctionName = functionName;
            Message = message;
            LogLevel = logLevel;
        }

        public Log(string id, string parentName, string functionName,
            string message, LogLevel logLevel)
        {
            Id = id;
            ParentName = parentName;
            FunctionName = functionName;
            Message = message;
            LogLevel = logLevel;
        }

        public override string ToString()
        {
            return $"{nameof(ParentName)}: {ParentName}, {nameof(FunctionName)}: {FunctionName}," +
                   $" {nameof(Message)}: {Message}, {nameof(LogLevel)}: {LogLevel}, {nameof(IsDeleted)}: {IsDeleted}," +
                   $" {nameof(DbCreatedDate)}: {DbCreatedDate}, {nameof(DbModifiedDate)}: {DbModifiedDate}," +
                   $" {nameof(DbDeletedDate)}: {DbDeletedDate}, {nameof(DbReactivatedDate)}: {DbReactivatedDate}";
        }
    }
}