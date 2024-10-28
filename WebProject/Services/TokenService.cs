using ServerProject.Services;

namespace WebProject.Services
{
	public class TokenService
	{
		public TokenService(ILocalStorageService localStorageService)
		{
			LocalStorageService = localStorageService;
		}

		public async Task<UserToken?> GetTokenAsync()
		{
			var token = await LocalStorageService.GetAsync<UserToken>(nameof(UserToken));
			if (token == null) return null;
			if (string.IsNullOrWhiteSpace(token.Id) || token.Date < DateTime.UtcNow) return null;
			return token;
		}

		public async Task<UserToken> CreateTokenAsync(string id)
		{
			var token = new UserToken
			{
				Id = id,
				Date = DateTime.UtcNow.AddDays(1),
			};
			await LocalStorageService.SetAsync(nameof(UserToken), token);
			return token;
		}
		public ILocalStorageService LocalStorageService { get; set; }
	}

	public class UserToken
	{
		public string Id { get; set; }

		public DateTime Date { get; set; }
	}
}
