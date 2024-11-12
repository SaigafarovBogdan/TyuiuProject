using ServerProject.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace WebProject.Services
{
	public class TokenService
	{
		public TokenService(ILocalStorageService localStorageService,CookieProviderService cookieService)
		{
			_localStorageService = localStorageService;
			_cookieProviderService = cookieService;
		}

		public async Task<UserToken?> GetTokenAsync()
		{
			//var jwtToken = await _cookieProviderService.GetCookieAsync("uft-cookies");
			var jwtToken = await _localStorageService.GetStringAsync("uft-cookies");
			if (jwtToken != null)
			{
				JwtToken = jwtToken;
				return new UserToken() { Id = DecodeToken(jwtToken).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value };
			}
			return null;
		}
		public async Task SetTokenAsync(string token)
		{
			JwtToken = token;
			await _localStorageService.SetStringAsync("uft-cookies", token);
			//await _cookieProviderService.SetCookieAsync("uft-cookies", token, 1);
		}
		public JwtSecurityToken DecodeToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();

			var resultJwtToken = handler.ReadJwtToken(token);
			return resultJwtToken;
		}

		public UserToken Token {  get; set; } = new UserToken();
		public string JwtToken { get; set; } = string.Empty;
		readonly CookieProviderService _cookieProviderService;
		readonly ILocalStorageService _localStorageService;
	}

	public class UserToken
	{
		public string Id { get; set; } = string.Empty;
	}
}
