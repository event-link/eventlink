using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes.ExternalLinks
{
    public class TwitterType : ObjectGraphType<Twitter>
    {
        public TwitterType()
        {
            Field(x => x.Url).Description("The url to Twitter");
        }
    }
}