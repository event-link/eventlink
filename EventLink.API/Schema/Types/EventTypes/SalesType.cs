using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class SalesType : ObjectGraphType<Sales>
    {
        public SalesType()
        {
            Field(x => x.StartDateTime, nullable: true).Description("The start date of the sale");
            Field(x => x.StartTBD, nullable: true).Description("If the date is not determend yet");
            Field(x => x.EndDateTime, nullable: true).Description("The date of the events end");
        }
    }
}