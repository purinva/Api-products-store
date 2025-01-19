using Api.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Service
{
    public class JwtTokenGenerator
    {
        private readonly string secretKey;
        public JwtTokenGenerator(IConfiguration configuration)
        {
            this.secretKey = configuration["AuthSettings:SecretKey"];
        }

        public string GenerateJwtToken(AppUser appUser, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim("FirstName", appUser.FirstName),
                    new Claim("id", appUser.Id),
                    new Claim(ClaimTypes.Email, appUser.Email),
                    new Claim(ClaimTypes.Email, String.Join(",", roles))
                }),

                Expires = DateTime.UtcNow.AddDays(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
