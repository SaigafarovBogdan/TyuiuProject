using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ServerProject.Extensions
{
	public static class ApiExtensions
	{
		public static void AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
				AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
				{
					options.TokenValidationParameters = new()
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JwtOptions").GetSection("SecretKey").Value))
					};
					options.Events = new JwtBearerEvents()
					{
						OnMessageReceived = context =>
						{
							context.Token = context.Request.Cookies["uft-cookies"];
							return Task.CompletedTask;
						}
					};	
				});
			services.AddAuthorization();
		}
	}
}
