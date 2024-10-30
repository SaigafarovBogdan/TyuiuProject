using ServerProject.Controllers;
using ServerProject.Models;

namespace ServerProject.Services
{
	public class FileCleanupService: BackgroundService
	{
		private readonly Dictionary<string, List<FileDTO>> _files;
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
			var expiredFiles = _files.SelectMany(utd => utd.Value
				.Where(file => DateTime.UtcNow - file.DateUpload > _fileLifeSpan))
				.ToList(); // utd - UserTokenDirectory

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
			foreach (var utd in _files.ToList())
			{
				utd.Value.RemoveAll(file => expiredFiles.Contains(file));
			}
			_logger.LogDebug("Очистка истёкших файлов произведена. Файлов удалено: {expiredFiles}", expiredFiles.Count);
		}
	}
}
