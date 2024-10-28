using Microsoft.AspNetCore.Mvc;
using ServerProject.Utilites;

namespace ServerProject.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DownloadFileController : ControllerBase
	{
		private readonly IConfiguration configuration;
		private readonly ILogger<DownloadFileController> _logger;

		public DownloadFileController(ILogger<DownloadFileController> logger, IConfiguration config)
		{
			_logger = logger;
			configuration = config;
		}

		[HttpGet("{fileId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public ActionResult DownloadFile(string fileId, [FromQuery] string tokenId, [FromQuery] string tokenDate)
		{
			//Проверка токена
			var path = Directory.GetFiles(Path.Combine(configuration["UploadPath"], tokenId, fileId)).FirstOrDefault();

			if (path != null)
			{
				try
				{
					byte[] buffer = System.IO.File.ReadAllBytes(path);
					var memoryStream = new MemoryStream(buffer);
					return File(memoryStream, FuncUtilites.GetContentType(Path.GetExtension(path)), Path.GetFileName(path));
				}
				catch(Exception e)
				{
					return Conflict(e.Message);
				}
			}
			else
			{
				return NotFound("Файл не найден");
			}
		}
	}
}
