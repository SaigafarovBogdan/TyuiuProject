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
    public record class FileDTO (string FileName, long FileSize, string FilePath);

}
