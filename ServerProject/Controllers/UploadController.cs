using Microsoft.AspNetCore.Mvc;
using ServerProject.Services;

namespace ServerProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
		private readonly ILogger<UploadController> _logger;
        private readonly FileStorageService fileStorageService;

        public UploadController(ILogger<UploadController> logger, FileStorageService storage)
        {
            _logger = logger;
            fileStorageService = storage;
        }

        [HttpPost("uploadfiles")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Upload([FromForm] IEnumerable<IFormFile> files, [FromForm] string tokenId, [FromForm] DateTime tokenDate)
        {
            if (files == null)
                return BadRequest("Нет файла для загрузки.");
            if (tokenDate < DateTime.UtcNow) return BadRequest("Время токена исстекло.");

            var filesId = fileStorageService.CreateFilesDirectory(tokenId);
			foreach (var file in files)
            {
                var resultPath = fileStorageService.AddFileDTO(tokenId, filesId, file.FileName, file.Length);

                if(resultPath == null) return StatusCode(StatusCodes.Status500InternalServerError);

				var path = Path.Combine(resultPath, file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return Ok(tokenId + filesId);
        }
		[HttpGet("getfiles/{userId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult GetUserFiles(string userId, [FromQuery] string tokenId, [FromQuery] DateTime tokenDate)
		{
			if (tokenDate < DateTime.UtcNow) return BadRequest("Время токена исстекло.");

            if (!fileStorageService.Files.ContainsKey(userId)) return NotFound();
            return Ok(fileStorageService.Files[userId]);
		}
	}
}
