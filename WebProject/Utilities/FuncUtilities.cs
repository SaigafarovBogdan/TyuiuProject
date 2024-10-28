namespace WebProject.Utilities
{
	public class FuncUtilities
	{
		public static string GetSizeString(long? bytes)
		{
			if(bytes == null) return string.Empty;
			return bytes switch
			{
				>= 1024 * 1024 => $"{bytes / (1024 * 1024.0):F1} Мб",
				>= 1024 => $"{bytes / 1024.0:F1} Кб",
				_ => $"{bytes} Байт"
			};
		}
	}
}
