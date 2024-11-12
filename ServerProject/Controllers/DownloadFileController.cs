using Microsoft.AspNetCore.Mvc;
using ServerProject.Utilites;
using System.IO.Compression;
using ServerProject.Services;

namespace ServerProject.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DownloadFileController : ControllerBase
	{
		private readonly IConfiguration configuration;
		private readonly ILogger<DownloadFileController> _logger;
		private readonly IJwtProvider _jwtProvider;
		private readonly FileStorageService _fileStorageService;

		public DownloadFileController(ILogger<DownloadFileController> logger, IConfiguration config, IJwtProvider jwtProvider, FileStorageService fileStorage)
		{
			_logger = logger;
			configuration = config;
			_jwtProvider = jwtProvider;
			_fileStorageService = fileStorage;
		}

		[HttpGet("{fileId:length(10)}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public ActionResult DownloadFile(string fileId)
		{
			fileId = fileId.Trim();
			if (string.IsNullOrWhiteSpace(fileId)) return BadRequest();
			var FilesGroup = _fileStorageService.GetUserFilesGroup(fileId);
			if(FilesGroup == null) return NotFound("Файл не найден");

			var paths = Directory.GetFiles(Path.Combine(configuration["UploadPath"], FilesGroup.UserId, fileId));

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
						{
							var fileName = Path.GetFileName(paths.First());
                            return File(memoryStream.ToArray(), FuncUtilites.GetContentType(Path.GetExtension(paths.First())), fileName);
						}
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
