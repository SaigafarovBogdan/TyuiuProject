using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ServerProject.Extensions
{
	public static class ApiExtensions
	{
		public static void AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).
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
							var authHeader = context.Request.Headers.Authorization.ToString();
							if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
							{
								context.Token = authHeader.Substring("Bearer ".Length).Trim();
							}
							return Task.CompletedTask;
						}
					};
				});
			services.AddAuthorization();
		}
	}
}
