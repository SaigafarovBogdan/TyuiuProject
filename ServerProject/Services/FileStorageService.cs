using ServerProject.Models;
using ServerProject.Utilites;
using System.Globalization;
using System.IO;
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
		public string CreateFilesDirectory(string tokenId)
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
				filePath = Path.Combine(Path.Combine(configuration["UploadPath"], tokenId), tokenId + fileId);
			}
			while (Directory.Exists(filePath));
			Directory.CreateDirectory(filePath);
			return fileId;
		}
		public string AddFileDTO(string tokenId, string fileId, string fileName)
		{
			var pathRoot = Path.Combine(configuration["UploadPath"],tokenId);
			var filePath = Path.Combine(pathRoot, tokenId + fileId);
			Files[pathRoot].Add(new FileDTO { Id = pathRoot, FileName = fileName, DateUpload = DateTime.UtcNow, FilePath =  filePath});

			return filePath;
		}
	}

}
