using Microsoft.AspNetCore.CookiePolicy;
using ServerProject.Extensions;
using ServerProject.Services;

namespace ServerProject
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowWebOrigin",
					builder =>
					{
						builder.WithOrigins("https://localhost:7101")
							   .AllowAnyHeader()
							   .WithExposedHeaders("Content-Disposition")
							   .AllowAnyMethod()
							   .AllowCredentials();
					});
			});

			builder.Services.AddSingleton<FileStorageService>();
			builder.Services.AddHostedService<FileCleanupService>();
			builder.Services.AddScoped<IJwtProvider, JwtProvider>();

			builder.Services.AddApiAuthentication(builder.Configuration);
			var app = builder.Build();
			app.UseCors("AllowWebOrigin");

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			//app.UseCookiePolicy(new CookiePolicyOptions
			//{
			//	MinimumSameSitePolicy = SameSiteMode.None,
			//	Secure = CookieSecurePolicy.Always,
			//});

			app.UseAuthentication();
			app.UseAuthorization();
			app.UseStaticFiles();

			app.MapControllers();

			app.Run();
		}
	}
}
