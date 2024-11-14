namespace ServerProject.Models
{
	public class ChunkedFileUploadModel
	{
		public required IFormFile File { get; set; }
		public string FileName { get; set; } = string.Empty;
		public bool IsLastChunk { get; set; }
	}
}
