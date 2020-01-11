using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes.ExternalLinks
{
    public class ItunesType : ObjectGraphType<Itune>
    {
        public ItunesType()
        {
            Field(x => x.Url).Description("The URL to itunes");
        }
    }
}