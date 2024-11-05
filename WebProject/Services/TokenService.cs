using ServerProject.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebProject.Services
{
	public class TokenService
	{
		public TokenService(ILocalStorageService localStorageService, IHttpContextAccessor httpContextAccessor)
		{
			LocalStorageService = localStorageService;
			HttpContext = httpContextAccessor.HttpContext;
		}

		public async Task<UserToken?> GetTokenAsync()
		{
			if (HttpContext.Request.Cookies.TryGetValue("uft-cookies", out var jwtToken))
				return new UserToken() { Id = DecodeToken(jwtToken).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value };
			return null;
		}
		public JwtSecurityToken DecodeToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();

			var resultJwtToken = handler.ReadJwtToken(token);
			return resultJwtToken;
		}

		private ILocalStorageService LocalStorageService { get; set; }
		private HttpContext HttpContext { get; set; }
	}

	public class UserToken
	{
		public string Id { get; set; }
	}
}
