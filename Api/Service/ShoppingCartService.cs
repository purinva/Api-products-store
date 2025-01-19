using Api.Data;
using Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Service
{
    public class ShoppingCartService
    {
        private readonly AppDbContext dbContext;

        public ShoppingCartService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateNewCartAsync(
            string userId, int productId, int quantity)
        {
            ShoppingCart newCart = new ShoppingCart
            {
                UserId = userId
            };

            await dbContext.ShoppingCarts.AddAsync(newCart);
            await dbContext.SaveChangesAsync();

            CartItem newCartItem = new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                ShoppingCartID = newCart.Id,
                Product = null
            };

            await dbContext.CartItems.AddAsync(newCartItem);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateExistingCartAsync(
            ShoppingCart shoppingCart, int productId, int newQuantity)
        {
            CartItem cartItemCart = shoppingCart
                .CartItems
                .FirstOrDefault(x => x.ProductId == productId);

            if (cartItemCart == null && newQuantity > 0)
            {
                CartItem cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = newQuantity,
                    ShoppingCartID = shoppingCart.Id,
                    Product = null
                };

                await dbContext.CartItems.AddAsync(cartItem);
            }
            else if (cartItemCart != null)
            {
                int updateQuantity = cartItemCart.Quantity + newQuantity;

                if(newQuantity == 0 || updateQuantity <= 0)
                {
                    dbContext.CartItems.Remove(cartItemCart);

                    if(shoppingCart.CartItems.Count == 1)
                    {
                        dbContext.ShoppingCarts.Remove(shoppingCart);
                    }
                }
                else
                {
                    cartItemCart.Quantity = newQuantity;
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<ShoppingCart> GetShoppingCartAsync(
            string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new ShoppingCart();
            }

            ShoppingCart shoppingCart = await dbContext
                .ShoppingCarts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (shoppingCart == null
                && shoppingCart.CartItems != null)
            {
                shoppingCart.TotalAmount = shoppingCart
                    .CartItems
                    .Sum(x => x.Quantity * x.Product.Price);
            }

            return shoppingCart;
        }
    }
}
