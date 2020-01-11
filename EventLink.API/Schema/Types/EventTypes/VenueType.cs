using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class VenueType : ObjectGraphType<Venue>
    {
        public VenueType()
        {
            Field(x => x.Name).Description("The name of the venue");
            Field(x => x.Type).Description("The type of venue");
            Field(x => x.Id).Description("The ID of the venue");
            Field(x => x.Url).Description("The URL to the venue");
            Field(x => x.Locale).Description("The location of the venue");
            Field(x => x.Timezone).Description("The timezone the venue is in.");
            Field<CityType>("City");
            Field<CountryType>("Country");
            Field<AddressType>("Address");
        }
    }
}