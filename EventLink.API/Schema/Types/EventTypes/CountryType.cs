using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class CountryType : ObjectGraphType<Country>
    {
        public CountryType()
        {
            Field(x => x.Name).Description("The name of the country");
            Field(x => x.Code).Description("The country code of the country");
        }
    }
}
