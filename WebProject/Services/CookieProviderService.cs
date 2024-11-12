using Microsoft.JSInterop;

namespace WebProject.Services
{
	public class CookieProviderService
	{
		private readonly IJSRuntime _jsRuntime;

		public CookieProviderService(IJSRuntime jsRuntime)
		{
			_jsRuntime = jsRuntime;
		}

		public async Task<string> GetCookieAsync(string name)
		{
			return await _jsRuntime.InvokeAsync<string>("getCookie", name);
		}

		public async Task SetCookieAsync(string name, string value, int days)
		{
			await _jsRuntime.InvokeVoidAsync("setCookie", name, value, days);
		}

		public async Task DeleteCookieAsync(string name)
		{
			await _jsRuntime.InvokeVoidAsync("deleteCookie", name);
		}
	}
}
