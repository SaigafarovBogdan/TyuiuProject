using ServerProject.Models;
using ServerProject.Utilites;

namespace ServerProject.Services
{
	public class FileStorageService
	{
		private readonly IConfiguration configuration;
		public Dictionary<string, List<FilesGroupDTO>> Files { get; private set; }
		public FileStorageService(IConfiguration config)
		{
			configuration = config;
			if(Files == null)
			{
				FilesInitialization();
			}
		}

		private void FilesInitialization()
		{
			Files = new Dictionary<string, List<FilesGroupDTO>>();
			var userDirectories = Directory.GetDirectories(configuration["UploadPath"]);

			foreach (var userDirectory in userDirectories)
			{
				var userFolderName = Path.GetFileName(userDirectory);
				var filesGroups = new List<FilesGroupDTO>();

				var fileGroups = Directory.GetDirectories(userDirectory);

				foreach (var fileGroup in fileGroups)
				{
					var filesGroupDTO = new FilesGroupDTO
					{
						Id = Path.GetFileName(fileGroup),
						DateUpload = Directory.GetLastWriteTime(fileGroup)
					};

					var files = Directory.GetFiles(fileGroup);

					foreach (var file in files)
					{
						var info = new FileInfo(file);
						var fileDTO = new FileDTO
						{
							FileName = info.Name,
							FileSize = info.Length,
							FilePath = file
						};

						filesGroupDTO.Files.Add(fileDTO);
					}

					filesGroups.Add(filesGroupDTO);
				}

				Files[userFolderName] = filesGroups;
			}
		}
		public string CreateFilesDirectory(string tokenId)
		{
			var path = Path.Combine(configuration["UploadPath"], tokenId);
			if (!Files.ContainsKey(tokenId))
			{
				Directory.CreateDirectory(path);
				Files[tokenId] = new List<FilesGroupDTO>();
			}

			Files[tokenId].Add(new FilesGroupDTO());
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
		public string? AddFileDTO(string tokenId, string fileId, string fileName, long fileSize)
		{
			var pathRoot = Path.Combine(configuration["UploadPath"],tokenId);
			var filePath = Path.Combine(pathRoot, tokenId + fileId);

			var fileGroup = Files[tokenId].FirstOrDefault(group => (group.Id == tokenId + fileId) || string.IsNullOrEmpty(group.Id));
			if (fileGroup == null) return null;
			if (string.IsNullOrEmpty(fileGroup.Id))
			{
				fileGroup.Id = tokenId + fileId;
				fileGroup.DateUpload = DateTime.UtcNow;
			}
			fileGroup.Files.Add(new FileDTO { FileName = fileName, FileSize = fileSize, FilePath = filePath });

			return filePath;
		}
	}

}
