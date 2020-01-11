using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes.ExternalLinks
{
    public class FacebookType : ObjectGraphType<Facebook>
    {
        public FacebookType()
        {
            Field(x => x.Url).Description("The URL to Facebook");
        }
    }
}