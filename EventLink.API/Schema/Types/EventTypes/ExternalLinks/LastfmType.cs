using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes.ExternalLinks
{
    public class LastfmType : ObjectGraphType<Lastfm>
    {
        public LastfmType()
        {
            Field(x => x.Url).Description("The URL to Lastfm");
        }
    }
}