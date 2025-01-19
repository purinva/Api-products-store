using Api.Service;

namespace Api.Extension
{
    public static class ShoppingCartServiceExtension
    {
        public static IServiceCollection AddShoppingCartService(
            this IServiceCollection services)
        {
            return services.AddScoped<ShoppingCartService>();
        }
    }
}
