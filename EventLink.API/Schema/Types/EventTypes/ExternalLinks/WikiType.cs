using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes.ExternalLinks
{
    public class WikiType : ObjectGraphType<Wiki>
    {
        public WikiType()
        {
            Field(x => x.Url).Description("The URL to Wiki");
        }
    }
}