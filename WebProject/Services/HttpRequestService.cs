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

		const int CHUNK_PROPORTION = 4;
		public HttpRequestService(IHttpClientFactory httpClientFactory, TokenService tokenService)
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
			string fileGroupId = string.Empty;
			var tasks = new List<Task<HttpResponseMessage>>();

			foreach (var file in files)
			{
				long totalBytes = file.Data.Length;
				int chunkSize = Math.Max((int)(totalBytes / CHUNK_PROPORTION), 512 * 1024);

				int chunkCount = (int)Math.Ceiling((double)totalBytes / chunkSize);
				long bytesRead = 0;

				for (int i = 0; i < chunkCount; i++)
				{
					int currentChunkSize = (int)Math.Min(chunkSize, totalBytes - bytesRead);
					byte[] chunk = new byte[currentChunkSize];

					int bytesReaded = 0;
					while (bytesReaded < currentChunkSize)
					{
						int remaining = currentChunkSize - bytesReaded;
						int readBytes = await file.Data.ReadAsync(chunk.AsMemory(bytesReaded, remaining));
						if (readBytes == 0)
						{
							break;
						}
						bytesReaded += readBytes;
					}
					bytesRead += bytesReaded;

					var uploadTask = CreateUploadTask(file, chunk, bytesReaded, i, chunkCount, fileGroupId);
					tasks.Add(uploadTask);
					if(string.IsNullOrEmpty(fileGroupId))
					{
						var firstResponse = await tasks[0];
						if (firstResponse.IsSuccessStatusCode)
						{
							fileGroupId = await firstResponse.Content.ReadAsStringAsync();
						}
						else
						{
							return firstResponse;
						}
					}
				}
			}

			var responses = await Task.WhenAll(tasks.Skip(1));
			foreach (var response in responses)
			{
				if (!response.IsSuccessStatusCode)
				{
					return response;
				}
			}

			return new HttpResponseMessage(HttpStatusCode.OK);
		}

		private Task<HttpResponseMessage> CreateUploadTask(IFileListEntry file, byte[] chunk, int bytesReaded, int chunkIndex, int chunkCount, string fileGroupId)
		{
			return Task.Run(async () =>
			{
				using (var content = new MultipartFormDataContent())
				{
					var fileContent = new StreamContent(new MemoryStream(chunk, 0, bytesReaded));
					string type = string.IsNullOrEmpty(file.Type) ? "application/octet-stream" : file.Type;
					fileContent.Headers.ContentType = new MediaTypeHeaderValue(type);

					content.Add(fileContent, "File", $"{file.Name}.part{chunkIndex + 1}");
					content.Add(new StringContent(file.Name), "FileName");
					content.Add(new StringContent(chunkIndex == chunkCount - 1 ? "true" : "false"), "IsLastChunk");
					content.Add(new StringContent(fileGroupId), "FileGroupId");
					content.Add(new StringContent(file.Size.ToString()), "FileSize");

					var request = new HttpRequestMessage(HttpMethod.Post, "upload/uploadfiles")
					{
						Content = content
					};
					request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", $"{_tokenService.JwtToken}");

					var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
					return response;
				}
			});
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
