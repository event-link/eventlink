using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventLink.Auth.UnitTest
{
    [TestClass]
    public class AuthAPIUnitTests
    {

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

        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {

        }
    }
}
