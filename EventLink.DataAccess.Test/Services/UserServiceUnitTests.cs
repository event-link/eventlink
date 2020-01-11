using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventLink.DataAccess.UnitTest.Services
{
    [TestClass]
    public class UserServiceTests
    {

        private static readonly UserService userService = UserService.Instance;

        [TestInitialize()]
        public void Initialize()
        {
            var testUsers = GetTestUsers();

            foreach (var u in testUsers)
            {
                userService.CreateUser(u);
            }
        }

        [TestCleanup()]
        public void Cleanup()
        {
            var testUsers = GetTestUsers();


            /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
            if (SharedConstants.UnitTestMode)
            {
                foreach (var u in testUsers)
                {
                    userService.DestroyUser(u.Id);
                }
            }
        }

        /********************************************************
         * GetDocument METHOD TESTS
         ********************************************************/
        [TestMethod]
        public void GetDocument_FoundDoc()
        {
            User u = null;
            User u1 = null;

            try
            {
                u = userService.GetUser("507f191e810c19729de860aa");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                u1 = userService.GetUser("507f191e810c19729de860aa");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsNotNull(u);
            Assert.IsNotNull(u1);
        }

        [TestMethod]
        public void GetDocument_ProviderEventIdNull()
        {
            try
            {
                userService.GetUser(null);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /*
         * We thought that MongoDB would throw a MongoDBException, but instead
         * if MongoDB does not recognize the given Id, it just returns null.
         */
        [TestMethod]
        public void GetDocument_ProviderEventIdNotFound_DAIdNotFoundException()
        {
            try
            {
                userService.GetUser("507f191e810c19729de860ff");
            }
            catch (DADocNotFoundException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
         * GetDocuments METHOD TESTS
         ********************************************************/
        [TestMethod]
        public void GetDocuments_FoundDocList()
        {
            IEnumerable<User> userDocList = null;

            try
            {
                userDocList = userService.GetUsers();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            Assert.IsNotNull(userDocList);
            Assert.IsTrue(userDocList.Any());
        }

        /*
         * If the database is empty, an empty list is returned.
         * No exceptions are thrown.
         */
        [TestMethod]
        public void GetDocuments_DocListNull()
        {
            /* Cleanup the test events such that the database is empty */
            Cleanup();

            IEnumerable<User> userDocList = null;

            try
            {
                userDocList = userService.GetUsers();
            }
            catch (DAException ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            Assert.IsNotNull(userDocList);
            Assert.IsTrue(!userDocList.Any());
        }

        /********************************************************
        * CreateDocument METHOD TESTS
        ********************************************************/
        [TestMethod]
        public void CreateDocument_DocCreated()
        {
            var u = new User("507f191e810c19729de860ae", AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Brian", "Nielsen",
                "Dostrovyeski", "Mahmoud Nielsen Dmitri", "mndy@gmail.com", "address", DateTime.Now,
                "asd", "12345678", "SE", null,
                null, null, null, null, DateTime.Today, true);

            try
            {
                userService.CreateUser(u);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            User e1 = null;

            try
            {
                e1 = userService.GetUser("507f191e810c19729de860ae");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsNotNull(e1);

            /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
            if (SharedConstants.UnitTestMode)
            {
                userService.DestroyUser(e1);
            }
        }

        [TestMethod]
        public void CreateDocument_DocAlreadyExists()
        {
            var u = new User("507f191e810c19729de860aa", AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Brian", "Nielsen",
                "Dostrovyeski", "Mahmoud Nielsen Dmitri", "mndx@gmail.com", "address", DateTime.Now,
                "asd", "12345678", "SE", null,
                null, null, null, null, DateTime.Today, true);

            try
            {
                userService.CreateUser(u);
            }
            catch (DADocAlreadyExistsException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
        * UpdateDocument METHOD TESTS
        ********************************************************/
        [TestMethod]
        public void UpdateDocument_DocWasUpdated()
        {
            var u = userService.GetUser("507f191e810c19729de860aa");

            var emailExpected = "Dank@Password.com";
            var firstNameExpected = "ThisNameIsDank";
            var modifiedBefore = u.DbModifiedDate;

            u.Email = emailExpected;
            u.FirstName = firstNameExpected;

            try
            {
                userService.UpdateUser(u);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var eNew = userService.GetUser(u.Id);

            Assert.IsNotNull(eNew);
            Assert.AreEqual(emailExpected, eNew.Email);
            Assert.AreEqual(firstNameExpected, eNew.FirstName);
            Assert.AreNotEqual(modifiedBefore, eNew.DbModifiedDate);
            Assert.AreEqual(u.LastName, eNew.LastName);
            Assert.AreEqual(u.Id, eNew.Id);
            Assert.AreEqual(u.Country, eNew.Country);
        }

        [TestMethod]
        public void UpdateDocument_DocWasNotFound()
        {
            var u = new User("507f191e810c19729de860ee", AccountType.Regular, LoginMethod.Eventlink, "", "Brian", "Nielsen",
                "Dostrovyeski", "Mahmoud Nielsen Dmitri", "mndz@gmail.com", "address", DateTime.Now,
                "asd", "12345678", "SE", null,
                null, null, null, null, DateTime.Today, true);

            try
            {
                userService.UpdateUser(u);
            }
            catch (DADocNotFoundException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
         * DELETE METHOD TESTS
         ********************************************************/
        /*
         * DeleteDocument does not delete the actual file in the database.
         * It sets the "isDeleted" value to true on the object itself.
         */
        [TestMethod]
        public void DeleteDocument_DocWasDeleted()
        {
            // Get user
            var u = userService.GetUser("507f191e810c19729de860aa");

            // delete user
            try
            {
                userService.DeleteUser(u.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            var uNew = userService.GetUser("507f191e810c19729de860aa");

            Assert.IsNotNull(uNew);
            Assert.AreNotEqual(u.DbDeletedDate, uNew.DbDeletedDate);
            Assert.AreNotEqual(u.IsDeleted, uNew.IsDeleted);
        }

        [TestMethod]
        public void DeleteDocument_DocIsNull()
        {
            try
            {
                userService.DeleteUser((User)null);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void DeleteDocument_DocIdIsNull()
        {
            try
            {
                userService.DeleteUser((string)null);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void DeleteDocument_DocNotFoundWrongId()
        {
            var u = new User("507f191e810c19729de860ee", AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Brian", "Nielsen",
            "Dostrovyeski", "Mahmoud Nielsen Dmitri", "mnd@gmail.com", "address", DateTime.Now, "asd",
            "12345678", "SE", null, null, null, null, null, DateTime.Today, true);

            try
            {
                userService.DeleteUser(u);
            }
            catch (DADocNotFoundException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
         * DestroyLog METHOD TESTS
         *********************************************************/
        [TestMethod]
        public void DestroyDocument_DestroySingleDoc()
        {
            try
            {
                /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
                if (SharedConstants.UnitTestMode)
                {
                    userService.DestroyUser("507f191e810c19729de860aa");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            try
            {
                userService.GetUser("507f191e810c19729de860aa");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void DestroyDocument_DestroyNullDoc()
        {
            try
            {
                /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
                if (SharedConstants.UnitTestMode)
                {
                    userService.DestroyUser((User)null);
                }
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }


        [TestMethod]
        public void DestroyDocument_DestroyNullStringId()
        {
            try
            {
                /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
                if (SharedConstants.UnitTestMode)
                {
                    userService.DestroyUser((string)null);
                }
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
         * UTIL METHOD TESTS
         ********************************************************/
        [TestMethod]
        public void CheckUser_NullDAObject()
        {
            try
            {
                userService.CreateUser(null);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void CheckUser_EssentialData_Fullname()
        {
            var u = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", null, null, null, null,
                "Hans@gert.dk", "address", DateTime.Now, "asd", "12345678", "DK",
                null, null, null, null, null, DateTime.Today, true);

            try
            {
                userService.CheckEssentialUserData(u);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void CheckUser_EssentialData_Name()
        {
            var u = new User(AccountType.Regular, LoginMethod.Eventlink, "", null, null, null, "Hans Mogens",
                null, "address", DateTime.Now, "asd", "12345678", "DK", null,
                null, null, null, null, DateTime.Today, true);

            try
            {
                userService.CheckEssentialUserData(u);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
         * ASSISTANT METHODS
         ********************************************************/
        private static IEnumerable<User> GetTestUsers()
        {
            return new List<User> { new User("507f191e810c19729de860aa", AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Hans", "Anders", "Abdullah",
                    "Hans Anders Abdullah", "Haa@gmail.com", "address", DateTime.Now, "asd", "12345678", "DK",
                    null, null, null, null, null,DateTime.Today, true),
                new User("507f191e810c19729de860af", AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Mahmoud", "Nielsen", "Dmitri",
                    "Mahmoud Nielsen Dmitri", "mnd@gmail.com", "address", DateTime.Now, "asd", "12345678",
                    "SE", null, null, null, null, null, DateTime.Today, true),
            };
        }

    }
}