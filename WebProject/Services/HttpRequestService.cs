using BlazorInputFile;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
namespace WebProject.Services
{
	public class HttpRequestService
	{
		readonly HttpClient _httpClient;
		readonly IHttpClientFactory _httpClientFactory;

		readonly TokenService _tokenService;
		public HttpRequestService(IHttpClientFactory httpClientFactory,TokenService tokenService)
		{
			_tokenService = tokenService;
			_httpClientFactory = httpClientFactory;
			_httpClient = httpClientFactory.CreateClient("MainHttpClient");
		}

		public async Task<UserToken?> GetOrCreateTokenAsync()
		{
			var token = await _tokenService.GetTokenAsync();
			if (token != null) return token;
			try
			{
				var response = await _httpClient.GetAsync("Token/gettoken");
				if (response.IsSuccessStatusCode)
				{
					var jwtToken = await response.Content.ReadAsStringAsync();
					jwtToken = jwtToken.Replace("\"", "");
					await _tokenService.SetTokenAsync(jwtToken);

					var idClaim = _tokenService.DecodeToken(jwtToken).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
					var uToken = new UserToken() { Id = idClaim };

					_tokenService.Token = uToken;
					return uToken;
				}
				return null;
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
				return null;
			}

		}

		public async Task<HttpResponseMessage> UploadFileAsync(IFileListEntry[] files)
		{
			using (var content = new MultipartFormDataContent())
			{
				foreach (var file in files)
				{
					var fileContent = new StreamContent(file.Data);

					string type = string.IsNullOrEmpty(file.Type) ? "application/octet-stream" : file.Type;
					fileContent.Headers.ContentType = new MediaTypeHeaderValue(type);

					content.Add(
						content: fileContent,
						name: "files",
						fileName: file.Name
					);
				}
				var request = new HttpRequestMessage(HttpMethod.Post, "upload/uploadfiles")
				{
					Content = content
				};

				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", $"{_tokenService.JwtToken}");
				return await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
			}
		}
		public async Task<HttpResponseMessage> GetUserFilesAsync(string userId)
		{
			try
			{
                var request = new HttpRequestMessage(HttpMethod.Get, $"upload/getfiles/{userId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", $"{_tokenService.JwtToken}");

                return await _httpClient.SendAsync(request);
            }
			catch (HttpRequestException ex)
			{
                var errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Произошла ошибка: {ex.Message}")
                };
                return errorResponse;
            }
		}
		public async Task<HttpResponseMessage> DownloadFileAsync(string fileId)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, $"downloadfile/{fileId}");
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.JwtToken);
			return await _httpClient.SendAsync(request);
		}
	}
}
