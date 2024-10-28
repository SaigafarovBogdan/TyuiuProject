using ServerProject.Services;
using WebProject.Components;
using WebProject.Services;

namespace WebProject;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddHttpClient("MainHttpClient", (sp, client) =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();
            var baseAddress = env.IsDevelopment() ? "https://localhost:7049/" : "https://DropFile.com/";
            client.BaseAddress = new Uri(baseAddress);
        });
		builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
        builder.Services.AddScoped<TokenService>();
        builder.Services.AddScoped<HttpRequestService>();

		builder.Services.AddServerSideBlazor()
            .AddCircuitOptions(options =>
            {
                options.DetailedErrors = true; // Убрать при открытии сайта
            });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
