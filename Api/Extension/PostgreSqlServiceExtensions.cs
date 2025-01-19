using Api.Data;
using Api.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Extension
{
    public static class PostgreSqlServiceExtensions
    {
        public static void AddPostgreSqlDbContext(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("PostgreSQLConnection"));
            });
        }

        public static void AddPostgreSqlIdentityContext(
            this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
