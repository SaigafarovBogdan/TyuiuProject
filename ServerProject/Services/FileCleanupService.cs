using ServerProject.Controllers;
using ServerProject.Models;

namespace ServerProject.Services
{
	public class FileCleanupService : BackgroundService
	{
		private readonly Dictionary<string, List<FilesGroupDTO>> _files;
		private readonly ILogger _logger;
		private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10);
		private readonly TimeSpan _fileLifeSpan = TimeSpan.FromMinutes(10);

		public FileCleanupService(FileStorageService storage, ILogger<UploadController> logger)
		{
			_files = storage.Files;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(_checkInterval, stoppingToken);
				CleanupExpiredFiles();
			}
		}

		private void CleanupExpiredFiles()
		{
			var expiredFiles = _files
				.SelectMany(utd => utd.Value
					.Where(group => DateTime.UtcNow - group.DateUpload > _fileLifeSpan)
				.SelectMany(group => group.Files)
			).ToList();

			foreach (var file in expiredFiles)
			{
				if (Directory.Exists(file.FilePath))
				{
					try
					{
						Directory.Delete(file.FilePath, true);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			var expiredFileGroupsIds = _files
				.Where(utd => utd.Value.Any(group => group.Files.Any(file => DateTime.UtcNow - group.DateUpload > _fileLifeSpan)))
				.Select(utd => utd.Key)
				.ToHashSet();

			foreach (var key in expiredFileGroupsIds)
			{
				_files.Remove(key);
			}
			_logger.LogDebug("Очистка истёкших файлов произведена. Файлов удалено: {expiredFiles}", expiredFiles.Count);
		}
	}
}
