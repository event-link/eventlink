using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class GenreType : ObjectGraphType<Genre>
    {
        public GenreType()
        {
            Field(x => x.Id).Description("The ID of the genre");
            Field(x => x.Name).Description("The name of the genre");
        }
    }
}