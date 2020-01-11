using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class SubGenreType : ObjectGraphType<SubGenre>
    {
        public SubGenreType()
        {
            Field(x => x.Id).Description("The ID of the subgenre");
            Field(x => x.Name).Description("The name of the subgenre");
        }
    }
}