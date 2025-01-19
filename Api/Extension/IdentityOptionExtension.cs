using Microsoft.AspNetCore.Identity;

namespace Api.Extension
{
    public static class IdentityOptionExtension
    {
        public static IServiceCollection AddConfigureIdentityOptions(
            this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });

            return services;
        }
    }
}
