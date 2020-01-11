using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class DateType : ObjectGraphType<Dates>
    {
        public DateType()
        {
            Field(x => x.LocalStartDate).Description("The local starting time and date");
            Field(x => x.Timezone).Description("The timezone");
            Field(x => x.StatusCode).Description("Is the event on sale or not?");
            Field(x => x.SpanMultipleDays, nullable: true).Description("Does it span multiple days");
        }
    }
}