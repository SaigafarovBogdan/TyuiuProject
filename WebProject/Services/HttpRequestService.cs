using BlazorInputFile;
using System.Net.Http.Headers;

namespace WebProject.Services
{
	public class HttpRequestService
	{
		IHttpClientFactory httpFactory {  get; set; }
		HttpClient httpClient {  get; set; }
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
			if (token == null)
			{
				try
				{
					var response = await httpClient.GetAsync("Token/gettoken");
					if (response.IsSuccessStatusCode)
					{
						return await tokenService.CreateTokenAsync(response.Content.ReadAsStringAsync().Result);
					}
					return null;
				}
				catch (HttpRequestException ex)
				{
					Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
					return null;
				}
			}
			return token;
			
		}

		public async Task<HttpResponseMessage> UploadFileAsync(IFileListEntry[] files, UserToken token)
		{

			using (var content = new MultipartFormDataContent())
			{
				var tokenIdContent = new StringContent(token.Id);
				var tokenDateContent =  new StringContent(token.Date.ToString());

				content.Add(
					content: tokenIdContent,
					name: "tokenId"
				);
				content.Add(
					content: tokenDateContent,
					name: "tokenDate"
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
				return await httpClient.PostAsync("upload", content);
			}
		}
		public async Task<HttpResponseMessage> DownloadFileAsync(string fileId,UserToken token)
		{
			var url = Path.Combine(httpClient.BaseAddress.ToString(), "downloadfile", $"{fileId}?tokenId={token.Id}&tokenDate={token.Date.ToString()}");
			return await httpClient.GetAsync(url);
		}
	}
}
