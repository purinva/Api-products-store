using Api.Data;
using Api.Model;
using Api.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Controller
{
    public class ShoppingCartController : StoreController
    {
        private readonly ShoppingCartService shoppingCartService;
        public ShoppingCartController(AppDbContext dbContext, 
            ShoppingCartService shoppingCartService) 
            : base(dbContext)
        {
            this.shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseServer>> AppendOrUpdateItemInCart(
            string userId, int productId, int updateQuantity)
        {
            Product product = await dbContext
                .Products
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = {"Такого товара не найдено" }
                });
            }

            ShoppingCart shoppingCart = await dbContext
                .ShoppingCarts
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (shoppingCart == null
                && updateQuantity > 0)
            {
                await shoppingCartService
                    .CreateNewCartAsync(userId, productId, updateQuantity);
            }
            else if (shoppingCart != null)
            {
                await shoppingCartService
                    .UpdateExistingCartAsync(shoppingCart, productId, updateQuantity);
            }

            return Ok(new ResponseServer
            {
                StatusCode = HttpStatusCode.OK
            });
        }

        [HttpGet]
        public async Task<ActionResult<ResponseServer>> GetShoppingCart(string userId)
        {
            try
            {
                ShoppingCart shoppingCart = await shoppingCartService
                    .GetShoppingCartAsync(userId);

                return Ok(new ResponseServer
                {
                    StatusCode = HttpStatusCode.OK,
                    Result = shoppingCart
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Ошибка получения корзины", ex.Message }
                });
            }
        }
    }
}
