using System.IO.Compression;

namespace ServerProject.Utilites
{
	public static class FuncUtilites
	{
		public static string GenerateId()
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			Random random = new Random();
			char[] id = new char[5];

			for (int i = 0; i < 5; i++)
			{
				id[i] = chars[random.Next(chars.Length)];
			}

			return new string(id);
		}
		public static string GetContentType(string extension)
		{
			return extension.ToLower() switch
			{
				".txt" => "text/plain",
				".pdf" => "application/pdf",
				".jpg" => "image/jpeg",
				".png" => "image/png",
				".gif" => "image/gif",
				".zip" => "application/zip",
				".csv" => "text/csv",
				".json" => "application/json",
				".dll" => "application/octet-stream",
				_ => throw new NotImplementedException("Такой тип файла не доступен."),
			};
		}
	}
}
