using Microsoft.AspNetCore.Mvc;
using ServerProject.Services;
using ServerProject.Utilites;

namespace ServerProject.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class TokenController : ControllerBase
	{
		private readonly ILogger<TokenController> _logger;
		private readonly IJwtProvider _jwtProvider;

		public TokenController(ILogger<TokenController> logger, IJwtProvider provider)
		{
			_logger = logger;
			_jwtProvider = provider;
		}

		[HttpGet("gettoken")]
		public IActionResult GetNewToken()
		{
			var token = _jwtProvider.GenerateToken(FuncUtilites.GenerateId());
			HttpContext.Response.Cookies.Append("uft-cookies", token);

			return Ok(token);
		}
	}
}
