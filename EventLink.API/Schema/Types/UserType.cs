using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Field(x => x.Id).Description("Unique id of this object");
            Field<EnumerationGraphType<AccountType>>("AccountType");
            Field<EnumerationGraphType<LoginMethod>>("LoginMethod");
            Field(x => x.PicUrl).Description("Profile picture url");
            Field(x => x.FirstName).Description("First name of the user");
            Field(x => x.MiddleName).Description("Middle name of the user");
            Field(x => x.LastName).Description("Last name of the user");
            Field(x => x.FullName).Description("Full name of the user");
            Field(x => x.Email).Description("Email of the user");
            Field(x => x.Address).Description("User address");
            Field<DateGraphType>("Birthdate");
            Field(x => x.HashedPassword).Description("Hashed password of the user");
            Field(x => x.PhoneNumber).Description("Phone number of the user");
            Field(x => x.Country).Description("The country the user lives in");
            Field<ListGraphType<StringGraphType>>("ParticipatingEvents");
            Field<ListGraphType<StringGraphType>>("FavoriteEvents");
            Field<ListGraphType<StringGraphType>>("PastEvents");
            Field<ListGraphType<StringGraphType>>("Buddies");
            Field<ListGraphType<StringGraphType>>("Payments");
            Field<DateGraphType>("LastActivityDate");
            Field(x => x.IsActive).Description("States whether the user is active or not");
            Field<DateGraphType>("DbCreatedDate");
            Field<DateGraphType>("DbModifiedDate");
            Field<DateGraphType>("DbDeletedDate");
            Field<DateGraphType>("DbReactivatedDate");
            Field(x => x.IsDeleted).Description("Status whether the object is considered deleted in the database");
        }
    }
}