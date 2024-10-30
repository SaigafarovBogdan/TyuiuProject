using Microsoft.AspNetCore.Mvc;
using ServerProject.Utilites;

namespace ServerProject.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class TokenController : ControllerBase
	{
		private readonly IConfiguration configuration;
		private readonly ILogger<TokenController> _logger;

		public TokenController(ILogger<TokenController> logger, IConfiguration config)
		{
			_logger = logger;
			configuration = config;
		}

		[HttpGet("gettoken")]
		public IActionResult GetNewToken()
		{
			return Ok(FuncUtilites.GenerateId());
		}
	}
}
