using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ServerProject.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using WebProject.Services;

namespace WebProject
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");
			builder.RootComponents.Add<HeadOutlet>("head::after");
			builder.Logging.SetMinimumLevel(LogLevel.Debug);

			var env = builder.HostEnvironment;

			builder.Services.AddTransient<ILocalStorageService, LocalStorageService>();
			builder.Services.AddTransient<CookieProviderService>();
			builder.Services.AddSingleton<TokenService>();

			builder.Services.AddHttpClient("MainHttpClient", (sp,client) =>
			{
				var baseAddress = env.IsDevelopment() ? "https://localhost:7049/" : "https://DropFile.com/";
				client.BaseAddress = new Uri(baseAddress);
			});

			builder.Services.AddScoped<HttpRequestService>();

			await builder.Build().RunAsync();
		}
	}
}
