namespace ServerProject.Models
{
	public class FileDTO
	{
		public required string Id {  get; set; }

		public DateTime DateUpload { get; set; }

		public required string FilePath { get; set; }
	}
}
