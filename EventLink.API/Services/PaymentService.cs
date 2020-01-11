using EventLink.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventLink.API.Services
{

    public interface IPaymentService
    {
        IEnumerable<Payment> GetUserPayments(string userId);
        void BuyTicket(string userId, string eventId);
    }

    public class PaymentService : IPaymentService
    {
        private static readonly Lazy<IPaymentService> instance =
            new Lazy<IPaymentService>(() => new PaymentService());

        public static IPaymentService Instance => instance.Value;

        private readonly DataAccess.Services.PaymentService _paymentService = DataAccess.Services.PaymentService.Instance;
        private readonly DataAccess.Services.UserService _userService = DataAccess.Services.UserService.Instance;

        private PaymentService()
        {

        }

        public IEnumerable<Payment> GetUserPayments(string userId)
        {
            try
            {
                var user = _userService.GetUser(userId);
                var paymentIds = user.Payments;
                return paymentIds.Select(paymentId => _paymentService.GetPayment(paymentId)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /* TODO: This also needs to open up the URL for buying ticket. */
        public void BuyTicket(string userId, string eventId)
        {
            throw new NotImplementedException();
        }

    }
}