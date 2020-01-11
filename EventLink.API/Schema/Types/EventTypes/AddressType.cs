using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class AddressType : ObjectGraphType<Address>
    {
        public AddressType()
        {
            Field(x => x.Line).Description("The address line");
        }
    }
}