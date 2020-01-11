using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.LogTypes
{
    public class LogCreateInputType : InputObjectGraphType
    {
        public LogCreateInputType()
        {
            Name = "LogInput";
            Field<NonNullGraphType<EnumerationGraphType<LogDb>>>("LogDb");
            Field<NonNullGraphType<StringGraphType>>("ParentName");
            Field<NonNullGraphType<StringGraphType>>("FunctionName");
            Field<NonNullGraphType<StringGraphType>>("Message");
            Field<NonNullGraphType<EnumerationGraphType<LogLevel>>>("LogLevel");
        }
    }
}