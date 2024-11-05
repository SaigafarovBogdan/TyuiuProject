using BlazorInputFile;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace WebProject.Services
{
	public class HttpRequestService
	{
		IHttpClientFactory httpFactory { get; set; }
		HttpClient httpClient { get; set; }
		TokenService tokenService { get; set; }

		public HttpRequestService(IHttpClientFactory httpFactory, TokenService tokenService)
		{
			this.httpFactory = httpFactory;
			this.tokenService = tokenService;
			httpClient = httpFactory.CreateClient("MainHttpClient");
		}

		public async Task<UserToken?> GetOrCreateTokenAsync()
		{
			var token = await tokenService.GetTokenAsync();
			if (token != null) return token;
			try
			{
				var response = await httpClient.GetAsync("Token/gettoken");
				if (response.IsSuccessStatusCode)
				{
					var jwtToken = await response.Content.ReadAsStringAsync();

					var idClaim = tokenService.DecodeToken(jwtToken).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
					return new UserToken() { Id = idClaim };
				}
				return null;
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
				return null;
			}

		}

		public async Task<HttpResponseMessage> UploadFileAsync(IFileListEntry[] files, UserToken token)
		{
			using (var content = new MultipartFormDataContent())
			{
				var tokenIdContent = new StringContent(token.Id);

				content.Add(
					content: tokenIdContent,
					name: "tokenId"
				);

				foreach (var file in files)
				{
					var ms = new MemoryStream();
					await file.Data.CopyToAsync(ms);
					ms.Position = 0;

					var fileContent = new StreamContent(ms);
					fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.Type);

					content.Add(
						content: fileContent,
						name: "files",
						fileName: file.Name
					);
				}
				return await httpClient.PostAsync("upload/uploadfiles", content);
			}
		}
		public async Task<HttpResponseMessage> GetUserFilesAsync(string userId, UserToken token)
		{
			var url = $"{httpClient.BaseAddress}upload/getfiles/{userId}?tokenId={token.Id}";

			return await httpClient.GetAsync(url);
		}
		public async Task<HttpResponseMessage> DownloadFileAsync(string fileId, UserToken token)
		{
			var url = $"{httpClient.BaseAddress}downloadfile/{fileId}?tokenId={token.Id}";

			return await httpClient.GetAsync(url);
		}
	}
}
