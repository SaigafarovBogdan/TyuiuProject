namespace WebProject.Utilities
{
	public class FilesGroupDTO
	{
		public FileDTO[] Files;
		public string Id { get; private set; }
		public DateTime DateUpload { get; private set; }

		public FilesGroupDTO(FileDTO[] files, string id, DateTime date)
		{
			Files = files;
			Id = id;
			DateUpload = date;
		}
		public FilesGroupDTO(ServerFilesGroupDTO group)
		{
			Files = group.Files.ToArray();
			Id = group.Id;
			DateUpload = group.DateUpload;
		}

	}
	public record class FileDTO(string FileName, long FileSize);
}
