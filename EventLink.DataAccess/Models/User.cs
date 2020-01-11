using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EventLink.DataAccess.Models
{
    public enum AccountType
    {
        Admin,
        Regular,
    }

    public enum LoginMethod
    {
        Eventlink,
        Facebook,
        Google,
        Apple
    }

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("AccountType")]
        public AccountType AccountType { get; set; }

        [BsonElement("LoginMethod")]
        public LoginMethod LoginMethod { get; set; }

        [BsonElement("PicUrl")]
        public string PicUrl { get; set; }

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("MiddleName")]
        public string MiddleName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }

        [BsonElement("FullName")]
        public string FullName { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Address")]
        public string Address { get; set; }

        [BsonElement("Birthdate")]
        public DateTime Birthdate { get; set; }

        [BsonElement("HashedPassword")]
        public string HashedPassword { get; set; }

        [BsonElement("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("Country")]
        public string Country { get; set; }

        [BsonElement("ParticipatingEvents")]
        public IEnumerable<string> ParticipatingEvents { get; set; }

        [BsonElement("FavoriteEvents")]
        public IEnumerable<string> FavoriteEvents { get; set; }

        [BsonElement("PastEvents")]
        public IEnumerable<string> PastEvents { get; set; }

        [BsonElement("Buddies")]
        public IEnumerable<string> Buddies { get; set; }

        [BsonElement("Payments")]
        public IEnumerable<string> Payments { get; set; }

        [BsonElement("LastActivityDate")]
        public DateTime LastActivityDate { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

        /*
         * The elements below are common for every DAObject.
         * They are not defined in the abstract class because
         * we want these to be at the end of the data model.
         */
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
        public User(AccountType accountType, LoginMethod loginMethod, string picUrl, string firstName, string middleName, string lastName,
            string fullName, string email, string address, DateTime birthdate, string hashedPassword, string phoneNumber,
            string country, IEnumerable<string> participatingEvents, IEnumerable<string> favoriteEvents,
            IEnumerable<string> pastEvents, IEnumerable<string> buddies, IEnumerable<string> payments,
            DateTime lastActivityDate, bool isActive)
        {
            AccountType = accountType;
            LoginMethod = loginMethod;
            PicUrl = picUrl;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            FullName = fullName;
            Email = email;
            Address = address;
            Birthdate = birthdate;
            HashedPassword = hashedPassword;
            PhoneNumber = phoneNumber;
            Country = country;
            ParticipatingEvents = participatingEvents;
            FavoriteEvents = favoriteEvents;
            PastEvents = pastEvents;
            Buddies = buddies;
            Payments = payments;
            LastActivityDate = lastActivityDate;
            IsActive = isActive;
        }

        public User(string id, AccountType accountType, LoginMethod loginMethod, string picUrl, string firstName, string middleName, string lastName,
           string fullName, string email, string address, DateTime birthdate, string hashedPassword, string phoneNumber,
           string country, IEnumerable<string> participatingEvents, IEnumerable<string> favoriteEvents,
           IEnumerable<string> pastEvents, IEnumerable<string> buddies, IEnumerable<string> payments,
           DateTime lastActivityDate, bool isActive)
        {
            Id = id;
            AccountType = accountType;
            LoginMethod = loginMethod;
            PicUrl = picUrl;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            FullName = fullName;
            Email = email;
            Address = address;
            Birthdate = birthdate;
            HashedPassword = hashedPassword;
            PhoneNumber = phoneNumber;
            Country = country;
            ParticipatingEvents = participatingEvents;
            FavoriteEvents = favoriteEvents;
            PastEvents = pastEvents;
            Buddies = buddies;
            Payments = payments;
            LastActivityDate = lastActivityDate;
            IsActive = isActive;
        }
        public override string ToString()
        {
            return $"{nameof(AccountType)}: {AccountType}, {nameof(FirstName)}: {FirstName}, {nameof(MiddleName)}: {MiddleName}," +
                   $" {nameof(LastName)}: {LastName}, {nameof(FullName)}: {FullName}, {nameof(Email)}: {Email}," +
                   $" {nameof(HashedPassword)}: {HashedPassword}, {nameof(PhoneNumber)}: {PhoneNumber}," +
                   $" {nameof(Country)}: {Country}, {nameof(ParticipatingEvents)}: {ParticipatingEvents}," +
                   $" {nameof(FavoriteEvents)}: {FavoriteEvents}, {nameof(PastEvents)}: {PastEvents}," +
                   $" {nameof(Buddies)}: {Buddies}, {nameof(Payments)}: {Payments}, {nameof(LastActivityDate)}: {LastActivityDate}" +
                   $"{nameof(IsActive)}: {IsActive}, {nameof(IsDeleted)}: {IsDeleted}, {nameof(DbCreatedDate)}: {DbCreatedDate}," +
                   $" {nameof(DbModifiedDate)}: {DbModifiedDate}, {nameof(DbDeletedDate)}: {DbDeletedDate}," +
                   $" {nameof(DbReactivatedDate)}: {DbReactivatedDate}";
        }
    }
}