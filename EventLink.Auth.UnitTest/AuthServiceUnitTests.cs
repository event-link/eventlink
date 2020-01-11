using EventLink.Auth.Model;
using EventLink.Auth.Services;
using EventLink.DataAccess;
using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EventLink.Auth.UnitTest
{
    [TestClass]
    public class AuthServiceUnitTests
    {
        private static readonly IAuthService _authService = AuthService.Instance;
        private static readonly IUserService _userService = UserService.Instance;

        const string testUserEmail = "testauthservice@eventlink.ml";
        const string testUserPassword = "internalpassword";

        [TestInitialize()]
        public void Initialize()
        {

        }

        [TestCleanup()]
        public void Cleanup()
        {

        }

        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            var testUser = new User(AccountType.Admin, LoginMethod.Eventlink, "picUrl", "Test", "Testey", "Testo", "Test Testey Testo", testUserEmail,
            "address", DateTime.Now, testUserPassword, "12345678", "Denmark",
            new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            try
            {
                _userService.CreateUser(testUser);
            }
            catch (DADocAlreadyExistsException e)
            {
                Console.WriteLine("User already exists!");
            }
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {

        }

        [TestMethod]
        public void TestAuthenticate_ExistingUser()
        {
            var authRequestModel = new AuthRequestModel
            {
                Email = testUserEmail,
                Password = testUserPassword
            };

            AuthResponseModel authResponse = null;

            try
            {
                authResponse = _authService.Authenticate(authRequestModel);
            }
            catch (AuthException e)
            {
                Assert.Fail("Authentication test threw an exception when it should have not. Exception: " + e.Message);
            }

            if (authResponse == null)
            {
                Assert.Fail("authResponse is null!");
            }

            if (string.IsNullOrEmpty(authResponse.Token))
            {
                Assert.Fail("authResponse.Token is null or empty!");
            }

            if (string.IsNullOrEmpty(authResponse.RefreshToken))
            {
                Assert.Fail("authResponse.RefreshToken is null or empty!");
            }

            if (string.IsNullOrEmpty(authResponse.Message))
            {
                Assert.Fail("authResponse.Message is null or empty!");
            }
        }

        [TestMethod]
        public void TestAuthenticate_NonExistingUser()
        {
            var authRequestModel = new AuthRequestModel
            {
                Email = "test@email.test",
                Password = "testpassword"
            };

            try
            {
                var authResponse = _authService.Authenticate(authRequestModel);
            }
            catch (AuthException e)
            {
                Assert.IsTrue(e.Message.Contains("not found"));
                return;
            }

            Assert.Fail("The test method should have thrown an exception!");
        }

        [TestMethod]
        public void TestAuthenticate_ExistingUserWrongPasswordRightEmail()
        {
            var authRequestModel = new AuthRequestModel
            {
                Email = testUserEmail,
                Password = "wrongPassword"
            };

            try
            {
                var authResponse = _authService.Authenticate(authRequestModel);
            }
            catch (AuthBadCredentialsException e)
            {
                Assert.IsTrue(true);
                return;
            }
            catch (AuthException e)
            {
                Assert.Fail("Authentication test threw an exception when it should have not. Exception: " + e.Message);
            }

            Assert.Fail("The test method should have thrown an exception!");
        }

        [TestMethod]
        public void TestAuthenticate_ExistingUserWrongEmailRightPassword()
        {
            var authRequestModel = new AuthRequestModel
            {
                Email = "bademail",
                Password = testUserPassword,
            };

            try
            {
                var authResponse = _authService.Authenticate(authRequestModel);
            }
            catch (AuthUserNotFoundException e)
            {
                Assert.IsTrue(true);
                return;
            }
            catch (AuthException e)
            {
                Assert.Fail("Authentication test threw an exception when it should have not. Exception: " + e.Message);
            }

            Assert.Fail("The test method should have thrown an exception!");
        }

        [TestMethod]
        public void TestForgotPassword_ExistingUser()
        {
            var emailModel = new EmailModel
            {
                Email = testUserEmail
            };

            try
            {
                _authService.ForgotPassword(emailModel);
            }
            catch (AuthException e)
            {
                Assert.Fail("Authentication test threw an exception when it should have not. Exception: " + e.Message);
            }
        }

        public void TestForgotPassword_NonExistingUser()
        {
            var emailModel = new EmailModel
            {
                Email = "thisuserdoesnotexist@eventlink.cool"
            };

            try
            {
                _authService.ForgotPassword(emailModel);
            }
            catch (AuthException e)
            {
                Assert.Fail("Authentication test threw an exception when it should have not. Exception: " + e.Message);
            }

        }

    }

}
