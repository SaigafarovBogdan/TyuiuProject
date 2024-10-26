using ServerProject.Services;

namespace ServerProject.Utilites
{
	public class FuncUtilites
	{
		public static string GenerateId()
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			Random random = new Random();
			char[] id = new char[5];

			for (int i = 0; i < 5; i++)
			{
				id[i] = chars[random.Next(chars.Length)];
			}

			return new string(id);
		}
		public static string GetContentType(string extension)
		{
			return extension.ToLower() switch
			{
				".txt" => "text/plain",
				".pdf" => "application/pdf",
				".jpg" => "image/jpeg",
				".png" => "image/png",
				".gif" => "image/gif",
				".zip" => "application/zip",
				".csv" => "text/csv",
				".json" => "application/json",
				_ => throw new NotImplementedException(),
			};
		}
		//public class LoginModel
		//{
		//	public LoginModel(ILocalStorageService localStorageService)
		//	{
		//		LoginData = new LoginViewModel();
		//		LocalStorageService = localStorageService;
		//	}

		//	public async Task CreateTokenAsync()
		//	{
		//		var token = new SecurityToken
		//		{
		//			UserName = LoginData.Id,
		//			AccessToken = LoginData.Password
		//		};
		//		await LocalStorageService.SetAsync(nameof(SecurityToken), token);
		//	}
		//	public LoginViewModel LoginData { get; set; }
		//	public ILocalStorageService LocalStorageService { get; set; }
		//}

		//var token = await _localStorageService.GetAsync<SecurityToken>(nameof(SecurityToken));

		//if (token == null) return CreateAnonymous();
		//if (string.IsNullOrWhiteSpace(token.AccessToken)) return CreateAnonymous();
	}
}
