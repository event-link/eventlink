using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class AttractionType : ObjectGraphType<Attraction>
    {
        public AttractionType()
        {
            Field(x => x.Name).Description("The name of the attraction");
            Field(x => x.Type).Description("The type of the attraction");
            Field(x => x.Id).Description("The ID of the attraction");
            Field(x => x.Locale).Description("The locale of the attraction");
            Field<ExternallinkType>("Externallinks");
        }
    }
}