using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class PromoterType : ObjectGraphType<Promoter>
    {
        public PromoterType()
        {
            Field(x => x.Id).Description("The id of the promoter");
            Field(x => x.Name).Description("The name of the promoter");
        }
    }
}