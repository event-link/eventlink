using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes.ExternalLinks
{
    public class InstagramType : ObjectGraphType<Instagram>
    {
        public InstagramType()
        {
            Field(x => x.Url).Description("The URL to Instagram");
        }
    }
}