using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServerProject.Services
{
	public class JwtProvider : IJwtProvider
	{
		private readonly IConfiguration _configuration;

		public JwtProvider(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public bool VerifyId(string id, string jwtToken)
		{
			var token = DecodeToken(jwtToken);
			return id == token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
		}

		public string GenerateToken(string tokenId)
		{
			var section = _configuration.GetSection("JwtOptions");
			Claim[] claims = { new Claim(ClaimTypes.Name, tokenId) };

			var signingCredentials = new SigningCredentials(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section.GetSection("SecretKey").Value)),
				SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				claims: claims,
				signingCredentials: signingCredentials,
				expires: DateTime.UtcNow.AddHours(double.Parse(section.GetSection("ExpiresHours").Value)));

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
		public JwtSecurityToken DecodeToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();

			var resultJwtToken = handler.ReadJwtToken(token);
			return resultJwtToken;
		}
	}
}
