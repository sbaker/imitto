namespace IMitto.Protocols;

public interface IMittoPackageBuilder
{
	IMittoPackageBuilder AddHeader(string key, string value);

	IMittoPackageBuilder AddHeaders(IEnumerable<KeyValuePair<string, string>> headers);

	IMittoPackageBuilder AddHeaders(params IReadOnlyList<KeyValuePair<string, string>> headers);

	IMittoPackage Build();

	IMittoPackageBuilder WithAction(MittoAction action);

	IMittoPackageBuilder WithModifier(MittoModifier modifier);

	IMittoPackageBuilder WithPackage(string package);
}
