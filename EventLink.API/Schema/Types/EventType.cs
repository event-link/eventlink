using EventLink.API.Schema.Types.EventTypes;
using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types
{
    public class EventType : ObjectGraphType<Event>
    {
        public EventType()
        {
            Field(x => x.Id).Description("The events ID");
            Field(x => x.ProviderEventId).Description("The provider of the events ID");
            Field(x => x.ProviderName).Description("The name of the provider");
            Field(x => x.Name).Description("The name of the event");
            Field(x => x.Type).Description("The type of the event");
            Field(x => x.Url).Description("The URL to the event");
            Field(x => x.Locale).Description("Where the event is located");
            Field(x => x.Description).Description("Whats the description of the evnt.");
            Field<SalesType>("Sales");
            Field<DateType>("Dates");
            Field<ListGraphType<ClassificationType>>("Classifications");
            Field<PromoterType>("Promoter");
            Field<ListGraphType<PriceRangeType>>("PriceRanges");
            Field<ListGraphType<VenueType>>("Venues");
            Field<ListGraphType<AttractionType>>("Attractions");
            Field<ListGraphType<ImageType>>("Images");
            Field(x => x.IsActive, nullable: true).Description("Is the event active?");
            Field<DateGraphType>("DbCreatedDate");
            Field<DateGraphType>("DbModifiedDate");
            Field<DateGraphType>("DbDeletedDate");
            Field<DateGraphType>("DbReactivatedDate");
            Field(x => x.IsDeleted).Description("Is the event deleted?");
        }
    }
}