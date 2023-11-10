using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AdopPixAPI.Services.IServices
{
    public interface ITokenService
    {
        string CreateToken(string tokenKey, List<Claim> claims, DateTime expireDate);
        JwtSecurityToken ReadToken(string tokenKey);
        string EncodeBase64(string text);
        string DecodeBase64(string tokenBase64);
    }
}
 