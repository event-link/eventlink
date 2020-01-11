using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class SegmentType : ObjectGraphType<Segment>
    {
        public SegmentType()
        {
            Field(x => x.Id).Description("The ID of the segment");
            Field(x => x.Name).Description("The name of the segment");
        }
    }
}