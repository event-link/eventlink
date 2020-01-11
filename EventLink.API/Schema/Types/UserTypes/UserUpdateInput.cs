using EventLink.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace EventLink.API.Schema.Types.UserTypes
{
    public class UserUpdateInput
    {
        public string Id { get; set; }

        public AccountType AccountType { get; set; }

        public LoginMethod LoginMethod { get; set; }

        public string PicUrl { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DateTime Birthdate { get; set; }

        public string HashedPassword { get; set; }

        public string PhoneNumber { get; set; }

        public string Country { get; set; }

        public IEnumerable<string> ParticipatingEvents { get; set; }

        public IEnumerable<string> FavoriteEvents { get; set; }

        public IEnumerable<string> PastEvents { get; set; }

        public IEnumerable<string> Buddies { get; set; }

        public IEnumerable<string> Payments { get; set; }

        public DateTime LastActivityDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime DbCreatedDate { get; set; }

        public DateTime DbModifiedDate { get; set; }

        public DateTime DbDeletedDate { get; set; }

        public DateTime DbReactivatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
