namespace WebProject.Utilities
{
	public class ServerFilesGroupDTO
	{
		public List<FileDTO> Files { get; set; }
		public string Id { get; set; }
		public DateTime DateUpload { get; set; }
		public ServerFilesGroupDTO()
		{
			Files = new List<FileDTO>();
			Id = "";
		}
	}

	public class ServerFileDTO
	{
		public required string FileName { get; set; }
		public required long FileSize { get; set; }
		public required string FilePath { get; set; }
	}
}
