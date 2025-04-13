namespace IMitto;

internal static class MittoConstants
{
	public static readonly string Version = typeof(MittoConstants).Assembly.GetName().Version?.ToString() ?? "0.0.0.1";
}