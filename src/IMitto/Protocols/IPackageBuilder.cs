namespace IMitto.Protocols;

public interface IPackageBuilder
{
	IPackageBuilder WithAction(MittoAction action);

	IPackageBuilder WithModifier(MittoModifier modifier);

	IPackageBuilder AddHeader(string key, string value);

	IPackageBuilder AddHeaders(IEnumerable<KeyValuePair<string, string>> headers);

	IPackageBuilder AddHeaders(params IReadOnlyList<KeyValuePair<string, string>> headers);

	IPackageBuilder WithPackage(string package);

	IMittoPackage Build();
}
