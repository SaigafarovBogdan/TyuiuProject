namespace ServerProject.Models
{
	public class FilesGroupDTO
	{
        public List<FileDTO> Files { get; set; }
        public string Id {  get; set; }
        public string UserId { get; set; }
		public DateTime DateUpload { get; set; }
        public FilesGroupDTO()
        {
            Files = new List<FileDTO>();
            Id = "";
            UserId = "";
        }
        public FilesGroupDTO(string id, string userId)
        {
            Files = new List<FileDTO>();
            Id = id;
            UserId = userId;
        }
	}
    public record class FileDTO (string FileName, long FileSize, string FilePath);

}
