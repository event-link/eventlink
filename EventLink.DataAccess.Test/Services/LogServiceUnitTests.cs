using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventLink.DataAccess.UnitTest.Services
{
    [TestClass]
    public class LogServiceUnitTests
    {
        private static readonly LogService logService = LogService.Instance;

        [TestInitialize()]
        public void Initialize()
        {
            var testLogs = GetTestLogs();

            foreach (var l in testLogs)
            {
                logService.CreateLog(LogDb.System, l);
            }
        }

        [TestCleanup()]
        public void Cleanup()
        {
            var testLogs = GetTestLogs();

            /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
            if (SharedConstants.UnitTestMode)
            {
                foreach (var l in testLogs)
                {
                    logService.DestroyLog(LogDb.System, l.Id);
                }
            }
        }

        /********************************************************
         * GetDocument METHOD TESTS
         ********************************************************/
        [TestMethod]
        public void GetDocument_FoundDoc()
        {
            Log l = null;
            Log l1 = null;

            try
            {
                l = logService.GetLog(LogDb.System, "507f191e810c19729de860aa");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                l1 = logService.GetLog(LogDb.System, "507f191e810c19729de860bb");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsNotNull(l);
            Assert.IsNotNull(l1);
        }

        [TestMethod]
        public void GetDocument_IdNull()
        {
            try
            {
                logService.GetLog(LogDb.System, null);
            }
            catch (DANullOrEmptyIdException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void GetDocument_IdNotFound()
        {
            try
            {
                logService.GetLog(LogDb.System, "507f191e810c19729de860ff");
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
         *********************************************************/
        [TestMethod]
        public void GetDocuments_FoundDocList()
        {
            IEnumerable<Log> logDocList = null;

            try
            {
                logDocList = logService.GetLogs(LogDb.System);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            Assert.IsNotNull(logDocList);
            Assert.IsTrue(logDocList.Any());
        }

        /*
         * If the database is empty, an empty list is returned.
         * No exceptions are thrown.
         */
        [TestMethod]
        public void GetDocuments_DocListNull()
        {
            /* Cleanup the test logs such that the database is empty */
            Cleanup();

            IEnumerable<Log> logDocList = null;

            try
            {
                logDocList = logService.GetLogs(LogDb.System);
            }
            catch (DAException ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            Assert.IsNotNull(logDocList);
            Assert.IsTrue(!logDocList.Any());
        }

        /********************************************************
         * CreateDocument METHOD TESTS
         ********************************************************/
        [TestMethod]
        public void CreateDocument_DocCreated()
        {
            var l = new Log("507f191e810c19729de860ab", "parentNameTest", "functionNameTest", "messageTest", LogLevel.Trace);

            try
            {
                logService.CreateLog(LogDb.System, l);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Log l1 = null;

            try
            {
                l1 = logService.GetLog(LogDb.System, "507f191e810c19729de860ab");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsNotNull(l1);

            /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
            if (SharedConstants.UnitTestMode)
            {
                logService.DestroyLog(LogDb.System, l1);
            }
        }

        [TestMethod]
        public void CreateDocument_DocAlreadyExists()
        {
            var l = new Log("507f191e810c19729de860aa", "parentNameTest", "functionNameTest", "messageTest", LogLevel.Trace);

            try
            {
                logService.CreateLog(LogDb.System, l);
            }
            catch (DADocAlreadyExistsException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void CreateDocument_CheckInvalidIdFormat()
        {
            var l = new Log("asdfcvdsf", "parentNameTest", "functionNameTest", "Message", LogLevel.Fatal);

            try
            {
                logService.CreateLog(LogDb.System, l);
            }
            catch (DAInvalidIdException)
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
            var l = logService.GetLog(LogDb.System, "507f191e810c19729de860aa");

            var parentNameExpected = "This is a test update. (parentName)";
            var funtionNameExpected = "This is a test update. (functionName)";
            var modifiedBefore = l.DbModifiedDate;

            l.ParentName = parentNameExpected;
            l.FunctionName = funtionNameExpected;

            try
            {
                logService.UpdateLog(LogDb.System, l);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var eNew = logService.GetLog(LogDb.System, l.Id);

            Assert.IsNotNull(eNew);
            Assert.AreEqual(parentNameExpected, eNew.ParentName);
            Assert.AreEqual(funtionNameExpected, eNew.FunctionName);
            Assert.AreNotEqual(modifiedBefore, eNew.DbModifiedDate);
            Assert.AreEqual(l.Id, eNew.Id);
            Assert.AreEqual(l.LogLevel, eNew.LogLevel);
        }

        [TestMethod]
        public void UpdateDocument_DocWasNotFound()
        {
            var l = new Log("507f191e810c19729de860ff", "parentNameTest", "functionNameTest", "Message", LogLevel.Fatal);

            try
            {
                logService.UpdateLog(LogDb.System, l);
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
            // Get log
            var l = logService.GetLog(LogDb.System, "507f191e810c19729de860aa");

            // delete log
            try
            {
                logService.DeleteLog(LogDb.System, l.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            var lNew = logService.GetLog(LogDb.System, l.Id);

            Assert.IsNotNull(lNew);
            Assert.AreNotEqual(l.DbDeletedDate, lNew.DbDeletedDate);
            Assert.AreEqual(true, lNew.IsDeleted);
            Assert.AreNotEqual(l.IsDeleted, lNew.IsDeleted);
        }

        [TestMethod]
        public void DeleteDocument_DocIsNull()
        {
            try
            {
                logService.DeleteLog(LogDb.System, (Log)null);
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
                logService.DeleteLog(LogDb.System, (string)null);
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
            var l = new Log("507f191e810c19729de860ff", "parentNameTest", "functionNameTest", "Message", LogLevel.Fatal);

            try
            {
                logService.DeleteLog(LogDb.System, l);
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
                    logService.DestroyLog(LogDb.System, "507f191e810c19729de860aa");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            try
            {
                logService.GetLog(LogDb.System, "507f191e810c19729de860aa");
            }
            catch (DADocNotFoundException)
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
                    logService.DestroyLog(LogDb.System, (Log)null);
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
                    logService.DestroyLog(LogDb.System, (string)null);
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
        public void CheckLog_NullDAObject()
        {
            try
            {
                logService.CreateLog(LogDb.System, null);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void CheckLog_EssentialData_ParentName()
        {
            var l = new Log("507f191e810c19729de860ff", null, "functionNameTest", "Message", LogLevel.Fatal);

            try
            {
                logService.CheckEssentialLogData(l);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void CheckLog_EssentialData_FunctionName()
        {
            var l = new Log("507f191e810c19729de860ff", "parentNameTest", null, "Message", LogLevel.Fatal);

            try
            {
                logService.CheckEssentialLogData(l);
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
        private static IEnumerable<Log> GetTestLogs()
        {
            return new List<Log>
            {
                new Log("507f191e810c19729de860aa", "parentName1", "functionName1", "message1", LogLevel.Info),
                new Log("507f191e810c19729de860bb", "parentName2", "functionName2", "message2", LogLevel.Debug)
            };
        }
    }
}