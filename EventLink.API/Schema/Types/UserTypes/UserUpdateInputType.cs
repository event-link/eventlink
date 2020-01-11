using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.UserTypes
{
    public class UserUpdateInputType : InputObjectGraphType
    {
        public UserUpdateInputType()
        {
            Name = "UserUpdateInput";
            Field<NonNullGraphType<StringGraphType>>("Id");
            Field<NonNullGraphType<EnumerationGraphType<AccountType>>>("AccountType");
            Field<EnumerationGraphType<LoginMethod>>("LoginMethod");
            Field<NonNullGraphType<StringGraphType>>("PicUrl");
            Field<NonNullGraphType<StringGraphType>>("FirstName");
            Field<NonNullGraphType<StringGraphType>>("MiddleName");
            Field<NonNullGraphType<StringGraphType>>("LastName");
            Field<NonNullGraphType<StringGraphType>>("FullName");
            Field<NonNullGraphType<StringGraphType>>("Email");
            Field<NonNullGraphType<StringGraphType>>("Address");
            Field<NonNullGraphType<DateGraphType>>("Birthdate");
            Field<NonNullGraphType<StringGraphType>>("HashedPassword");
            Field<NonNullGraphType<StringGraphType>>("PhoneNumber");
            Field<NonNullGraphType<StringGraphType>>("Country");
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("ParticipatingEvents");
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("FavoriteEvents");
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("PastEvents");
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("Buddies");
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("Payments");
            Field<NonNullGraphType<DateGraphType>>("LastActivityDate");
            Field<NonNullGraphType<BooleanGraphType>>("IsActive");
            Field<NonNullGraphType<DateGraphType>>("DbCreatedDate");
            Field<NonNullGraphType<DateGraphType>>("DbModifiedDate");
            Field<NonNullGraphType<DateGraphType>>("DbDeletedDate");
            Field<NonNullGraphType<DateGraphType>>("DbReactivatedDate");
            Field<NonNullGraphType<BooleanGraphType>>("IsDeleted");
        }
    }
}
