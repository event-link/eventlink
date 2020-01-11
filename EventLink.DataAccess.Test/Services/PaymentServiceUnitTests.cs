using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventLink.DataAccess.UnitTest.Services
{
    [TestClass]
    public class PaymentServiceUnitTests
    {

        private static readonly PaymentService paymentService = PaymentService.Instance;

        [TestInitialize()]
        public void Initialize()
        {
            var testPayments = GetTestPayments();

            foreach (var p in testPayments)
            {
                paymentService.CreatePayment(p);
            }
        }

        [TestCleanup()]
        public void Cleanup()
        {
            var testPayments = GetTestPayments();

            /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
            if (SharedConstants.UnitTestMode)
            {
                foreach (var p in testPayments)
                {
                    paymentService.DestroyPayment(p.Id);
                }
            }
        }

        /********************************************************
         * GetDocument METHOD TESTS
         ********************************************************/
        [TestMethod]
        public void GetDocument_FoundDoc()
        {
            Payment p1 = null;
            Payment p2 = null;

            try
            {
                p1 = paymentService.GetPayment("507f191e810c19729de860aa");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                p2 = paymentService.GetPayment("507f191e810c19729de860ab");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsNotNull(p1);
            Assert.IsNotNull(p2);
        }

        [TestMethod]
        public void GetDocument_IdIsNull()
        {
            try
            {
                paymentService.GetPayment(null);
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
        public void GetDocument_IdNotFound()
        {
            try
            {
                paymentService.GetPayment("507f191e810c19729de860ac");
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
            IEnumerable<Payment> PaymentDocList = null;

            try
            {
                PaymentDocList = paymentService.GetPayments();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            Assert.IsNotNull(PaymentDocList);
            Assert.IsTrue(PaymentDocList.Any());
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

            IEnumerable<Payment> paymentDocList = null;

            try
            {
                paymentDocList = paymentService.GetPayments();
            }
            catch (DAException ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            Assert.IsNotNull(paymentDocList);
            Assert.IsTrue(!paymentDocList.Any());
        }

        /********************************************************
        * CreateDocument METHOD TESTS
        ********************************************************/
        [TestMethod]
        public void CreateDocument_DocCreated()
        {
            var u = new Payment("507f191e810c19729de860ac", "UserId3", "EventId3", DateTime.Now, 100.2, true);

            try
            {
                paymentService.CreatePayment(u);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Payment p1 = null;

            try
            {
                p1 = paymentService.GetPayment("507f191e810c19729de860ac");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsNotNull(p1);

            /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
            if (SharedConstants.UnitTestMode)
            {
                paymentService.DestroyPayment(p1);
            }
        }

        [TestMethod]
        public void CreateDocument_DocAlreadyExists()
        {
            var u = new Payment("507f191e810c19729de860aa", "UserId1", "EventId1", DateTime.Now, 100.2, true);

            try
            {
                paymentService.CreatePayment(u);
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
            var u = paymentService.GetPayment("507f191e810c19729de860aa");

            var eventIdExpected = "EventId3";
            var dateTimeExpected = DateTime.MinValue;
            var modifiedBefore = u.DbModifiedDate;

            u.EventId = eventIdExpected;
            u.PaymentDate = dateTimeExpected;

            try
            {
                paymentService.UpdatePayment(u);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var pNew = paymentService.GetPayment(u.Id);

            Assert.IsNotNull(pNew);
            Assert.AreEqual(eventIdExpected, pNew.EventId);
            Assert.AreEqual(dateTimeExpected, pNew.PaymentDate);
            Assert.AreNotEqual(modifiedBefore, pNew.DbModifiedDate);
            Assert.AreEqual(u.Amount, pNew.Amount);
            Assert.AreEqual(u.Id, pNew.Id);
            Assert.AreEqual(u.Currency, pNew.Currency);
        }

        [TestMethod]
        public void UpdateDocument_DocWasNotFound()
        {
            var u = new Payment("507f191e810c19729de860ac", "UserId3", "EventId3", DateTime.Now, 1000.0, true);

            try
            {
                paymentService.UpdatePayment(u);
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
            // Get Payment
            var u = paymentService.GetPayment("507f191e810c19729de860aa");

            // delete Payment
            try
            {
                paymentService.DeletePayment(u.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            var pNew = paymentService.GetPayment("507f191e810c19729de860aa");

            Assert.IsNotNull(pNew);
            Assert.AreNotEqual(u.DbDeletedDate, pNew.DbDeletedDate);
            Assert.AreNotEqual(u.IsDeleted, pNew.IsDeleted);
        }

        [TestMethod]
        public void DeleteDocument_DocIsNull()
        {
            try
            {
                paymentService.DeletePayment((Payment)null);
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
                paymentService.DeletePayment((string)null);
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
            var u = new Payment("507f191e810c19729de860bc", "UserId3", "EventId3", DateTime.Now, 100.00, true);

            try
            {
                paymentService.DeletePayment(u);
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
                    paymentService.DestroyPayment("507f191e810c19729de860aa");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            try
            {
                paymentService.GetPayment("507f191e810c19729de860aa");
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
                    paymentService.DestroyPayment((Payment)null);
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
                    paymentService.DestroyPayment((string)null);
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
        public void CheckPayment_NullDAObject()
        {
            try
            {
                paymentService.CreatePayment(null);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void CheckPayment_EssentialData_EventId()
        {
            var u = new Payment("507f191e810c19729de860ac", "userId3", null, DateTime.Now, 100.00, true);

            try
            {
                paymentService.CheckEssentialPaymentData(u);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void CheckPayment_EssentialData_UserId()
        {
            var u = new Payment("507f191e810c19729de860ac", null, "eventId3", DateTime.Now, 100.00, true);

            try
            {
                paymentService.CheckEssentialPaymentData(u);
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
        private static IEnumerable<Payment> GetTestPayments()
        {
            return new List<Payment>
            {
                new Payment("507f191e810c19729de860aa", "UserId1", "EventId1", DateTime.Now, 10.0, true),
                new Payment("507f191e810c19729de860ab", "UserId2", "EventId2", DateTime.Now, 10.0, true)
            };
        }
    }
}
