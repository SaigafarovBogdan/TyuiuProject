using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerProject.Models;
using ServerProject.Services;
using System.IdentityModel.Tokens.Jwt;
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
		public async Task<IActionResult> Upload([FromForm] ChunkedFileUploadModel model, [FromForm] string? FileGroupId, [FromForm] long FileSize)
		{
			if (model.File == null)
				return BadRequest("Нет файла для загрузки.");
			string? tokenId;
			string filesId;
			string? resultPath = string.Empty;

			if (string.IsNullOrEmpty(FileGroupId))
			{
				tokenId = _jwtProvider.DecodeToken(HttpContext.Request.Headers.Authorization.ToString().Substring("Bearer ".Length).Trim())
					.Claims.First(c => c.Type == ClaimTypes.Name).Value;

				filesId = _fileStorageService.CreateFilesDirectory(tokenId);

				resultPath = _fileStorageService.AddFileDTO(tokenId, filesId, model.FileName, FileSize);
				if (resultPath == null) return StatusCode(StatusCodes.Status500InternalServerError);
			}
			else
			{
				tokenId = FileGroupId.Substring(0, 5);
				filesId = FileGroupId;
			}
			var filePath = string.IsNullOrEmpty(resultPath) ? Path.Combine(_fileStorageService.GetGroupPath(filesId),model.FileName):Path.Combine(resultPath, model.FileName);

			using (var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
			{
				await model.File.CopyToAsync(stream);
			}

			if (model.IsLastChunk)
			{

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
