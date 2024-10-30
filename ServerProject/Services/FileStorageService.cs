using ServerProject.Models;
using ServerProject.Utilites;
namespace ServerProject.Services
{
	public class FileStorageService
	{
		private readonly IConfiguration configuration;
		public Dictionary<string, List<FileDTO>> Files { get; private set; }
		public FileStorageService(IConfiguration config)
		{
			Files = new Dictionary<string, List<FileDTO>>();
			configuration = config;
		}
		public (string, string) AddFileDTO(string tokenId)
		{
			var path = Path.Combine(configuration["UploadPath"], tokenId);
			if (!Files.ContainsKey(path))
			{
				Directory.CreateDirectory(path);
				Files[path] = new List<FileDTO>();
			}
			string filePath;
			string fileId;
			do
			{
				fileId = FuncUtilites.GenerateId();
				filePath = Path.Combine(path, tokenId + fileId);
			}
			while (Directory.Exists(filePath));
			Directory.CreateDirectory(filePath);
			Files[path].Add(new FileDTO { Id = path, DateUpload = DateTime.UtcNow, FilePath = filePath });

			return (filePath, fileId);
		}
	}

}
