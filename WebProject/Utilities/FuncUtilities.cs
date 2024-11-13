namespace WebProject.Utilities
{
	public static class FuncUtilities
	{
		public static string GetSizeString(long? bytes)
		{
			if (bytes == null) return string.Empty;
			return bytes switch
			{
				>= 1024 * 1024 => $"{bytes / (1024 * 1024.0):F1} Мб",
				>= 1024 => $"{bytes / 1024.0:F1} Кб",
				_ => $"{bytes} Байт"
			};
		}

		public static FilesGroupDTO? ConvertFileGroupDTO(ServerFilesGroupDTO group)
		{
			if (group == null) return null;
			return new FilesGroupDTO(group);
		}

		public static string GetMimeType(string fileName)
		{
			var fileExtension = fileName.Split('.').Last().ToLower();

			return fileExtension switch
			{
				"zip" => "zip.svg",
				"jpg" or "png" or "gif" or "dds" => "image.svg",
				"mp4" or "mov" or "avi" => "video.svg",
				"mp3" or "ogg" => "audio.svg",
				_ => "file.svg"
			};
		}
	}
}
