namespace ServerProject.Models
{
	public class FilesGroupDTO
	{
        public List<FileDTO> Files { get; set; }
        public string Id {  get; set; }
		public DateTime DateUpload { get; set; }
        public FilesGroupDTO()
        {
            Files = new List<FileDTO>();
            Id = "";
        }
	}
    public class FileDTO
    {
        public required string FileName { get; set; }
        public required long FileSize { get; set; }
        public required string FilePath { get; set; }
    }
}
