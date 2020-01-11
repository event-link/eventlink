using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types
{
    public class LogType : ObjectGraphType<Log>
    {
        public LogType()
        {
            Field(x => x.Id).Description("Unique id of this object");
            Field(x => x.ParentName).Description("Class name of where the log occurred");
            Field(x => x.FunctionName).Description("Function name of where the log occurred");
            Field(x => x.Message).Description("Informative message about the logged incident");
            Field<EnumerationGraphType<LogLevel>>("LogLevel");
            Field<DateGraphType>("DbCreatedDate");
            Field<DateGraphType>("DbModifiedDate");
            Field<DateGraphType>("DbDeletedDate");
            Field<DateGraphType>("DbReactivatedDate");
            Field(x => x.IsDeleted).Description("Status whether the object is considered deleted in the database");
        }
    }
}