using Api.Data;
using Api.Model;
using Api.Service.Payment;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller
{
    public class PaymentController : StoreController
    {
        private readonly IPaymentService paymentService;
        public PaymentController(AppDbContext dbContext,
            IPaymentService paymentService) 
            : base(dbContext)
        {
            this.paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseServer>> MakePayment(
            string userId, int orderHeaderId, string cardNumber)
        {
            return await paymentService.HandlePaymentAsync(
                userId, orderHeaderId, cardNumber);
        }
    }
}