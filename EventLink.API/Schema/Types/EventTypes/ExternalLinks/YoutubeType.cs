using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes.ExternalLinks
{
    public class YoutubeType : ObjectGraphType<Youtube>
    {
        public YoutubeType()
        {
            Field(x => x.Url).Description("The url to Youtube");
        }
    }
}