using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes.ExternalLinks
{
    public class HomepageType : ObjectGraphType<Homepage>
    {
        public HomepageType()
        {
            Field(x => x.Url).Description("The URL to the homepage");
        }
    }
}