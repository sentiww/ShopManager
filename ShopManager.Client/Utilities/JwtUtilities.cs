using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShopManager.Client.Utilities;

public static class JwtUtilities
{
    public static ClaimsPrincipal GetClaimsPrincipalFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.ReadToken(jwt) as JwtSecurityToken;

        if (securityToken is null)
        {
            throw new Exception("Invalid JWT");
        }

        var claims = securityToken.Claims.ToList();
        
        return new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value),
            new Claim(ClaimTypes.Email, claims.First(claim => claim.Type == ClaimTypes.Email).Value),
            new Claim(ClaimTypes.Name, claims.First(claim => claim.Type == ClaimTypes.Name).Value)
        }));
    }
}