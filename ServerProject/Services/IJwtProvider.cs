using System.IdentityModel.Tokens.Jwt;

namespace ServerProject.Services
{
	public interface IJwtProvider
	{
		string GenerateToken(string tokenId);
		JwtSecurityToken DecodeToken(string token);

		bool VerifyId(string id, string jwtToken);
	}
}