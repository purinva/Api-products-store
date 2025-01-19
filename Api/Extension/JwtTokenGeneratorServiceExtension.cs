using Api.Service;

namespace Api.Extension
{
    public static class JwtTokenGeneratorServiceExtension
    {
        public static IServiceCollection AddJwtTokenGenerator(
            this IServiceCollection services)
        {
            return services.AddScoped<JwtTokenGenerator>();
        }
    }
}
