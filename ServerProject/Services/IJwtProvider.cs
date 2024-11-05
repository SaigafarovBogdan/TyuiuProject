namespace ServerProject.Services
{
	public interface IJwtProvider
	{
		string GenerateToken(string tokenId);
	}
}