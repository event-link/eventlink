using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types
{
    public class PaymentType : ObjectGraphType<Payment>
    {
        public PaymentType()
        {
            Field(x => x.Id).Description("Unique id of this object");
            Field(x => x.UserId).Description("The unique user id who this payment belongs to");
            Field(x => x.EventId).Description("The unique id of the event that is involved with this payment");
            Field(x => x.PaymentDate).Description("Timestamp of the day of payment");
            Field(x => x.Amount).Description("The amount of money that was involved with this payment");
            Field(x => x.Currency).Description("The currency that was used in this payment");
            Field(x => x.IsCharged).Description("Status whether the payment has gone through or not");
            Field<DateGraphType>("DbCreatedDate");
            Field<DateGraphType>("DbModifiedDate");
            Field<DateGraphType>("DbDeletedDate");
            Field<DateGraphType>("DbReactivatedDate");
            Field(x => x.IsDeleted).Description("Status whether the object is considered deleted in the database");
        }
    }
}