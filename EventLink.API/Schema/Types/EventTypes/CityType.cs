using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class CityType : ObjectGraphType<City>
    {
        public CityType()
        {
            Field(x => x.Name).Description("The name of the city");
        }
    }
}