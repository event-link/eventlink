using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace EventLink.DataAccess.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("EventId")]
        public string EventId { get; set; }

        [BsonElement("PaymentDate")]
        public DateTime PaymentDate { get; set; }

        [BsonElement("Amount")]
        public double Amount { get; set; }

        [BsonElement("Currency")]
        public string Currency { get; set; }

        [BsonElement("IsCharged")]
        public bool IsCharged { get; set; }

        [BsonElement("DbCreatedDate")]
        public DateTime DbCreatedDate { get; set; }

        [BsonElement("DbModifiedDate")]
        public DateTime DbModifiedDate { get; set; }

        [BsonElement("DbDeletedDate")]
        public DateTime DbDeletedDate { get; set; }

        [BsonElement("DbReactivatedDate")]
        public DateTime DbReactivatedDate { get; set; }

        [BsonElement("IsDeleted")]
        public bool IsDeleted { get; set; }

        [JsonConstructor]
        public Payment(string userId, string eventId,
            DateTime paymentDate, double amount, bool isCharged)
        {
            UserId = userId;
            EventId = eventId;
            PaymentDate = paymentDate;
            Amount = amount;
            IsCharged = isCharged;
        }

        public Payment(string id, string userId, string eventId,
            DateTime paymentDate, double amount, bool isCharged)
        {
            Id = id;
            UserId = userId;
            EventId = eventId;
            PaymentDate = paymentDate;
            Amount = amount;
            IsCharged = isCharged;
        }

        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}, {nameof(EventId)}: {EventId}, " +
                   $"{nameof(PaymentDate)}: {PaymentDate}, {nameof(Amount)}: {Amount}, " +
                   $"{nameof(IsCharged)}: {IsCharged}, {nameof(IsDeleted)}: {IsDeleted}, " +
                   $"{nameof(DbCreatedDate)}: {DbCreatedDate}, {nameof(DbModifiedDate)}: {DbModifiedDate}, " +
                   $"{nameof(DbDeletedDate)}: {DbDeletedDate}, {nameof(DbReactivatedDate)}: {DbReactivatedDate}";
        }
    }
}