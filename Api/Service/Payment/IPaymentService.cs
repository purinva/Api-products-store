using Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Service.Payment
{
    public interface IPaymentService
    {
        Task<ActionResult<ResponseServer>> HandlePaymentAsync(
            string userId, int orderHeaderId, string cardNumber);
    }
}
