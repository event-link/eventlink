using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class ClassificationType : ObjectGraphType<Classification>
    {
        public ClassificationType()
        {
            Field(x => x.Primary, true).Description("If this is checked, then this is the primary classification used to display data");
            Field(x => x.Family, true).Description("True if this is a family classification");
            Field<SegmentType>("Segment");
            Field<GenreType>("Genre");
            Field<SubGenreType>("SubGenre");
        }
    }
}