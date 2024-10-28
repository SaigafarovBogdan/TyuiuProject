using Microsoft.AspNetCore.Mvc;
using ServerProject.Utilites;

namespace ServerProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
		private readonly IConfiguration configuration;
		private readonly ILogger<UploadController> _logger;

        public UploadController(ILogger<UploadController> logger, IConfiguration config)
        {
            _logger = logger;
            configuration = config;
        }

        [HttpPost]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload([FromForm] IEnumerable<IFormFile> files, [FromForm] string tokenId, [FromForm] string tokenDate)
        {
            if (files == null)
                return BadRequest("Нет файла для загрузки.");
            if (DateTime.Parse(tokenDate) < DateTime.UtcNow) return BadRequest("Время токена исстекло.");

			var directoryPath = Path.Combine(configuration["UploadPath"], tokenId);
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

            string filePath;
            string fileId;
			do
            {
				fileId = FuncUtilites.GenerateId();
				filePath = Path.Combine(configuration["UploadPath"], tokenId, tokenId+fileId);
			}
            while(Directory.Exists(filePath));
			Directory.CreateDirectory(filePath);

			foreach (var file in files)
            {
				var path = Path.Combine(filePath, file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return Ok();
        }
    }
}
