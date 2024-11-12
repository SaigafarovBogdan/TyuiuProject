using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerProject.Services;
using System.Security.Claims;

namespace ServerProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
		private readonly ILogger<UploadController> _logger;
        private readonly FileStorageService _fileStorageService;
        private readonly IJwtProvider _jwtProvider;

        public UploadController(ILogger<UploadController> logger, FileStorageService storage, IJwtProvider jwtProvider)
        {
            _logger = logger;
            _fileStorageService = storage;
            _jwtProvider = jwtProvider;
        }

        [HttpPost("uploadfiles")]
        [Authorize]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Upload([FromForm] IEnumerable<IFormFile> files)
        {
            if (files == null)
                return BadRequest("Нет файла для загрузки.");

            var token = _jwtProvider.DecodeToken(HttpContext.Request.Headers.Authorization.ToString().Substring("Bearer ".Length).Trim());
            var tokenId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (tokenId == null) return Unauthorized();

			var filesId = _fileStorageService.CreateFilesDirectory(tokenId);
			foreach (var file in files)
            {
                var resultPath = _fileStorageService.AddFileDTO(tokenId, filesId, file.FileName, file.Length);

                if(resultPath == null) return StatusCode(StatusCodes.Status500InternalServerError);

				var path = Path.Combine(resultPath, file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return Ok(tokenId + filesId);
        }
		[Authorize]
		[HttpGet("getfiles/{userId:length(5)}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult GetUserFiles(string userId)
		{
            if(!_jwtProvider.VerifyId(userId, HttpContext.Request.Headers.Authorization.ToString().Substring("Bearer ".Length).Trim())) return Unauthorized();
            if (!_fileStorageService.Files.TryGetValue(userId, out List<Models.FilesGroupDTO>? value)) return NotFound();
            return Ok(value);
		}
	}
}
