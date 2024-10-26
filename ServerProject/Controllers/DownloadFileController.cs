using Microsoft.AspNetCore.Mvc;
using ServerProject.Utilites;

namespace ServerProject.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DownloadFileController : ControllerBase
	{
		private readonly IWebHostEnvironment environment;

		private readonly ILogger<DownloadFileController> _logger;

		public DownloadFileController(ILogger<DownloadFileController> logger, IWebHostEnvironment _environment)
		{
			_logger = logger;
			environment = _environment;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public ActionResult DownloadFile()
		{
			var path = Directory.GetFiles(Path.Combine(environment.WebRootPath,"uploads")).FirstOrDefault();

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
				return BadRequest("Файл не найден");
			}
		}
	}
}
