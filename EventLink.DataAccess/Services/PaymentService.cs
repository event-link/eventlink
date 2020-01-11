using EventLink.DataAccess.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace EventLink.DataAccess.Services
{
    public interface IPaymentService
    {
        Payment GetPayment(string id);
        IEnumerable<Payment> GetPayments();
        void CreatePayment(Payment paymentObj);
        void UpdatePayment(Payment paymentObj);
        void DeletePayment(Payment paymentObj);
        void DeletePayment(string id);
        /* DestroyDocument methods will DELETE data permanently. */
        /* These methods should ONLY be used for unit testing. Not in production. */
        void DestroyPayment(Payment paymentObj);
        void DestroyPayment(string id);
        void CheckNullPayment(Payment paymentObj);
        void CheckEssentialPaymentData(Payment paymentObj);
    }

    public class PaymentService : IPaymentService
    {
        private static readonly DbContext DbContext = DbContext.Instance;
        private readonly IMongoCollection<Payment> _payments;

        private static readonly Lazy<PaymentService> instance =
            new Lazy<PaymentService>(() => new PaymentService());

        public static PaymentService Instance => instance.Value;

        private PaymentService()
        {
            _payments = DbContext.GetPaymentCollection();
        }

        public Payment GetPayment(string id)
        {
            Payment paymentDoc;

            /* If Payment Id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Tries to find the first object with the given Id */
            try
            {
                paymentDoc = _payments.Find(p => p.Id == id).FirstOrDefault();
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
             * If the Payment is null, then we assume that a Payment with the given
             * Id does not exist in the database.
             */
            if (paymentDoc == null)
            {
                throw new DADocNotFoundException("Payment with Id (" + id + ") was not found!");
            }

            return paymentDoc;
        }

        public IEnumerable<Payment> GetPayments()
        {
            IEnumerable<Payment> paymentDocList;

            /* Tries to find all Payment documents */
            try
            {
                paymentDocList = _payments.Find("{}").ToList(); // {} has same effect as * in SQL (wildcard)
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            /* If the Payment list is still null, throw exception */
            if (paymentDocList == null)
            {
                throw new DADocNotFoundException("List of Payment objects is null!");
            }

            return paymentDocList;
        }

        public void CreatePayment(Payment paymentObj)
        {
            /* Validate the Payment object */
            CheckNullPayment(paymentObj);
            CheckEssentialPaymentData(paymentObj);

            /* Creates a new Payment document */
            try
            {
                /* If the Payment already is created throw exception. */
                if (paymentObj.Id != null && _payments.Find(p => p.Id == paymentObj.Id).FirstOrDefault() != null)
                {
                    throw new DADocAlreadyExistsException("Payment document already exists!");
                }

                paymentObj.DbCreatedDate = DateTime.Now;
                paymentObj.DbModifiedDate = DateTime.Now;
                _payments.InsertOne(paymentObj);
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + paymentObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void UpdatePayment(Payment paymentObj)
        {
            /* Validate the Event object */
            CheckNullPayment(paymentObj);

            /* If the Payment Id is null or empty */
            if (string.IsNullOrEmpty(paymentObj.Id))
            {
                throw new DANullOrEmptyIdException("Payment Id is null or empty!");
            }

            /* Checks whether the Payment document to update exists in the DB or not. */
            try
            {
                if (_payments.Find(p => p.Id == paymentObj.Id).FirstOrDefault() == null)
                {
                    throw new DADocNotFoundException("Payment Id (" + paymentObj.Id + ") was not found!");
                }
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + paymentObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            CheckEssentialPaymentData(paymentObj);

            /* Tries to update a document */
            try
            {
                paymentObj.DbModifiedDate = DateTime.Now;
                _payments.ReplaceOne(p => p.Id == paymentObj.Id, paymentObj);
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + paymentObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void DeletePayment(Payment paymentObj)
        {
            /* Validate the object */
            CheckNullPayment(paymentObj);

            /* Checks whether the Payment document exists */
            try
            {
                if (_payments.Find(p => p.Id == paymentObj.Id).FirstOrDefault() == null)
                {
                    throw new DADocNotFoundException("Payment with Id (" + paymentObj.Id + ") was not found!");
                }
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + paymentObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            DeletePayment(paymentObj.Id);
        }

        public void DeletePayment(string id)
        {
            /* If payment id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Find the document in the database */
            var paymentObj = GetPayment(id);

            /* Validate the object */
            CheckNullPayment(paymentObj);

            /* Tries to set the Payment inactive */
            try
            {
                paymentObj.IsDeleted = true;
                paymentObj.DbDeletedDate = DateTime.Now;
                paymentObj.DbModifiedDate = DateTime.Now;
                UpdatePayment(paymentObj);
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void DestroyPayment(Payment paymentObj)
        {
            if (!SharedConstants.UnitTestMode)
            {
#pragma warning disable CS0162 // Unreachable code detected
                return;
#pragma warning restore CS0162 // Unreachable code detected
            }

            /* Validate the Event object */
            CheckNullPayment(paymentObj);

            /* If Payment Id is null or empty */
            if (string.IsNullOrEmpty(paymentObj.Id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Deletes the document from the DB */
            DestroyPayment(paymentObj.Id);
        }

        public void DestroyPayment(string id)
        {
            if (!SharedConstants.UnitTestMode)
            {
#pragma warning disable CS0162 // Unreachable code detected
                return;
#pragma warning restore CS0162 // Unreachable code detected
            }

            /* If Payment Id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Deletes the document from the DB */
            try
            {
                _payments.DeleteOne(p => p.Id == id);
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

        public void CheckNullPayment(Payment paymentObj)
        {
            /* If Payment object is null, throw exception */
            if (paymentObj == null)
            {
                throw new DAException("Payment object is null!");
            }
        }

        public void CheckEssentialPaymentData(Payment paymentObj)
        {
            if (paymentObj.EventId == null || paymentObj.PaymentDate == null || paymentObj.UserId == null)
            {
                throw new DAException("All or some essential Payment data has not been found!");
            }
        }

    }
}