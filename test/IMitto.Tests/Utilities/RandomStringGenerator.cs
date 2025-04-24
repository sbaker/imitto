namespace IMitto.Tests.Utilities;

public static class RandomStringGenerator
{
	private static readonly Random Random = new();

	public static string GenerateRandomString(int length)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		return new string([.. Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)])]);
	}
}