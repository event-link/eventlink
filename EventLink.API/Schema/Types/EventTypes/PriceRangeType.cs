using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class PriceRangeType : ObjectGraphType<PriceRange>
    {
        public PriceRangeType()
        {
            Field(x => x.Type).Description("The type of ticket you're paying for");
            Field(x => x.Currency).Description("The currency of the payment");
            Field(x => x.Min, nullable: true).Description("The minimum price value");
            Field(x => x.Max, nullable: true).Description("The Maximum price value");

        }
    }
}