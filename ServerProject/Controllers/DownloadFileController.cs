using Microsoft.AspNetCore.Mvc;
using ServerProject.Utilites;
using System.IO.Compression;

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
		public ActionResult DownloadFile(string fileId, [FromQuery] string tokenId)
		{
			var paths = Directory.GetFiles(Path.Combine(configuration["UploadPath"], tokenId, fileId));

			if (paths != null)
			{
				if (paths.Length > 1)
				{
					using (var memoryStream = new MemoryStream())
					{
						using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
						{
							foreach (var filePath in paths)
							{
								var fileName = Path.GetFileName(filePath);
								var zipEntry = zipArchive.CreateEntryFromFile(filePath, fileName);
							}
						}
						memoryStream.Position = 0;
						return File(memoryStream.ToArray(), "application/zip", $"{fileId}.zip");
					}
				}
				else
				{
					try
					{
						byte[] buffer = System.IO.File.ReadAllBytes(paths.First());
						using (var memoryStream = new MemoryStream(buffer))
							return File(memoryStream.ToArray(), FuncUtilites.GetContentType(Path.GetExtension(paths.First())), Path.GetFileName(paths.First()));
					}
					catch (Exception e)
					{
						return Conflict(e.Message);
					}
				}
			}
			else
			{
				return NotFound("Файл не найден");
			}
		}
	}
}
