using AdopPixAPI.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdopPixAPI.Services
{
    public class TokenService : ITokenService
    {
        private SymmetricSecurityKey key;

        public TokenService()
        {
        }
        public string CreateToken(string tokenKey, List<Claim> claims, DateTime expireDate)
        {
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                SigningCredentials = creds,
                Subject = new ClaimsIdentity(claims),
                Expires = expireDate
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public JwtSecurityToken ReadToken(string tokenKey)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(tokenKey);
        }

        public string EncodeBase64(string text)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string DecodeBase64(string tokenBase64)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(tokenBase64);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
