using EventLink.DataAccess.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventLink.DataAccess.Services
{
    public interface IUserService
    {
        IEnumerable<User> SearchUsers(string query);
        User GetUser(string id);
        User GetUserByEmail(string email);
        IEnumerable<User> GetUsers();
        void CreateUser(User userObj);
        void UpdateUser(User userObj);
        void DeleteUser(User userObj);
        void DeleteUser(string id);
        /* DestroyDocument methods will DELETE data permanently. */
        /* These methods should ONLY be used for unit testing. Not in production. */
        void DestroyUser(User userObj);
        void DestroyUser(string id);
        void CheckNullUser(User userObj);
        void CheckEssentialUserData(User userObj);
    }

    public class UserService : IUserService
    {
        private static readonly DbContext DbContext = DbContext.Instance;
        private readonly IMongoCollection<User> _users;

        private static readonly Lazy<UserService> instance =
            new Lazy<UserService>(() => new UserService());

        public static UserService Instance => instance.Value;

        private UserService()
        {
            _users = DbContext.GetUserCollection();
        }

        public IEnumerable<User> SearchUsers(string query)
        {
            IEnumerable<User> searchResults;

            if (query == null)
            {
                query = "";
            }
            else
            {
                query = query.Replace(" ", "*");
            }

            try
            {
                var filterQuery = Builders<User>.Filter.Regex("FullName", new BsonRegularExpression(query, "i"));

                if (!string.IsNullOrEmpty(query))
                {
                    searchResults = _users.Find(filterQuery).SortBy(e => e.FullName).ToList();
                }
                else
                {
                    searchResults = _users.Find(filterQuery).SortBy(e => e.FullName).ToList();
                }
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            if (searchResults == null)
            {
                throw new DAException("Something went wrong searching with query (" + query + ")!");
            }

            return searchResults;
        }

        public User GetUserByEmail(string email)
        {
            User userDoc;

            /* If the User Email is null or empty */
            if (string.IsNullOrEmpty(email))
            {
                throw new DAException("Email is null or empty!");
            }

            /* Tries to find the first object with the given Email */
            try
            {
                userDoc = _users.Find(u => u.Email == email).FirstOrDefault();
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            /*
             * If the User is null, then we assume that an User with the given
             * Email does not exist in the database.
             */
            if (userDoc == null)
            {
                throw new DADocNotFoundException("User Email (" + email + ") was not found!");
            }

            return userDoc;
        }

        /*****************************
         * CRUD OPERATIONS
         *****************************/
        public User GetUser(string id)
        {
            User userDoc;

            /* If the User Id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Tries to find the first object with the given Id */
            try
            {
                userDoc = _users.Find(u => u.Id == id).FirstOrDefault();
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            /*
             * If the User is null, then we assume that an User with the given
             * Id does not exist in the database.
             */
            if (userDoc == null)
            {
                throw new DADocNotFoundException("User Id (" + id + ") was not found!");
            }

            return userDoc;
        }

        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> userDocList;

            /* Tries to find all User documents */
            try
            {
                userDocList = _users.Find("{}").ToList(); // {} has same effect as * in SQL (wildcard)
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            /* If the list is null, throw an exception */
            if (userDocList == null)
            {
                throw new DADocNotFoundException("List of Event objects is null!");
            }

            return userDocList;
        }

        public void CreateUser(User userObj)
        {
            /* Validate object */
            CheckNullUser(userObj);
            CheckEssentialUserData(userObj);

            /* Creates a new document */
            try
            {
                /* If the user already is created throw exception. */
                if (userObj.Id != null && _users.Find(u => u.Id == userObj.Id).FirstOrDefault() != null)
                {
                    throw new DADocAlreadyExistsException("User Id (" + userObj.Id + ") already exists!");
                }

                /* If the user (Email) already is created throw exception. */
                if (userObj.Email != null && _users.Find(u => u.Email == userObj.Email).FirstOrDefault() != null)
                {
                    throw new DADocAlreadyExistsException("User Email (" + userObj.Email + ") already exists!");
                }

                /* Hash user password */
                var hashedPassword = HashingUtils.SHA512(userObj.HashedPassword);
                userObj.HashedPassword = hashedPassword;

                userObj.DbCreatedDate = DateTime.Now;
                userObj.DbModifiedDate = DateTime.Now;
                _users.InsertOne(userObj);
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + userObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void UpdateUser(User userObj)
        {
            /* Validate the User object */
            CheckNullUser(userObj);

            /* If the User Id is null or empty */
            if (string.IsNullOrEmpty(userObj.Id))
            {
                throw new DANullOrEmptyIdException("User Id is null or empty!");
            }

            /* Checks whether the User document to update exists in the DB or not. */
            try
            {
                if (_users.Find(u => u.Id == userObj.Id).FirstOrDefault() == null)
                {
                    throw new DADocNotFoundException("User with Id (" + userObj.Id + ") was not found!");
                }
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + userObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            CheckEssentialUserData(userObj);

            /* Tries to update the User document */
            try
            {
                userObj.DbModifiedDate = DateTime.Now;
                _users.ReplaceOne(u => u.Id == userObj.Id, userObj);
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + userObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void DeleteUser(User userObj)
        {
            /* Validate the object */
            CheckNullUser(userObj);

            /* Checks whether the User document exists */
            try
            {
                if (_users.Find(u => u.Id == userObj.Id).FirstOrDefault() == null)
                {
                    throw new DADocNotFoundException("User Id (" + userObj.Id + ") was not found!");
                }
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + userObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            DeleteUser(userObj.Id);
        }

        public void DeleteUser(string id)
        {
            /* If User Id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException("User Id is null or empty!");
            }

            /* Find the document in the database */
            var userObj = GetUser(id);

            /* Validate the object */
            CheckNullUser(userObj);

            /* Tries to set the User inactive */
            try
            {
                userObj.IsDeleted = true;
                userObj.DbDeletedDate = DateTime.Now;
                userObj.DbModifiedDate = DateTime.Now;
                UpdateUser(userObj);
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void DestroyUser(User userObj)
        {
            /* Deletes the document from the DB */
            if (!SharedConstants.UnitTestMode)
            {
#pragma warning disable CS0162 // Unreachable code detected
                return;
#pragma warning restore CS0162 // Unreachable code detected
            }

            /* Validate the User object */
            CheckNullUser(userObj);

            /* If User Id is null or empty */
            if (string.IsNullOrEmpty(userObj.Id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Deletes the document from the DB */
            DestroyUser(userObj.Id);
        }

        public void DestroyUser(string id)
        {
            if (!SharedConstants.UnitTestMode)
            {
#pragma warning disable CS0162 // Unreachable code detected
                return;
#pragma warning restore CS0162 // Unreachable code detected
            }

            /* If Event Id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Deletes the document from the DB */
            try
            {
                _users.DeleteOne(e => e.Id == id);
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void CheckNullUser(User userObj)
        {
            /* If User object is null, throw exception */
            if (userObj == null)
            {
                throw new DAException("User object is null!");
            }
        }

        public void CheckEssentialUserData(User userObj)
        {
            /* Checks whether essential data for the User object is intact */
            if (userObj.FullName == null || userObj.Email == null)
            {
                throw new DAException("All or some essential User data has not been found!");
            }
        }

    }
}