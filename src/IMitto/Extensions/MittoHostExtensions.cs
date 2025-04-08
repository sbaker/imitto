using IMitto.Settings;
using Microsoft.Extensions.Options;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace IMitto;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MittoOptionsExtensions
{
	public static IOptions<MittoOptions> AsOptions(this MittoOptions options)
	{
		return Options.Create(options);
	}

	public static IOptions<MittoJsonOptions> AsOptions(this MittoJsonOptions options)
	{
		return Options.Create(options);
	}
}