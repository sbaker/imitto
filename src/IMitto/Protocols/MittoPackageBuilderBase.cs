namespace IMitto.Protocols;

public abstract class MittoPackageBuilderBase : IMittoPackageBuilder
{
	protected MittoPackageBuilderBase()
	{
		Headers.AddRange(
			CreateHeader("encoding", "utf-8"),
			CreateHeader("timestamp", TimeProvider.System.GetUtcNow().ToString("u"))
		);
	}

	protected abstract MittoProtocolVersion Version { get; }

	protected MittoAction Action { get; set; } = MittoAction.None;

	protected List<IMittoHeader> Headers { get; } = [];

	protected MittoModifier Modifier { get; set; } = MittoModifier.None;
	
	protected string? Package { get; set; }

	public IMittoPackageBuilder AddHeader(MittoHeaderKey key, string value)
	{
		Headers.Add(CreateHeader(key, value));

		return this;
	}

	public IMittoPackageBuilder AddHeaders(IEnumerable<KeyValuePair<MittoHeaderKey, string>> headers)
	{
		throw new NotImplementedException();
	}

	public IMittoPackageBuilder AddHeaders(params IReadOnlyList<KeyValuePair<MittoHeaderKey, string>> headers)
	{
		throw new NotImplementedException();
	}

	public virtual IMittoPackageBuilder WithAction(MittoAction action)
	{
		if (action == MittoAction.None)
		{
			throw new ArgumentException("Action cannot be None.", nameof(action));
		}

		Action = action;
		return this;
	}

	public virtual IMittoPackageBuilder WithModifier(MittoModifier modifier)
	{
		Modifier = modifier;
		return this;
	}

	public virtual IMittoPackageBuilder AddHeader(string key, string value)
	{
		ArgumentNullException.ThrowIfNull(key, nameof(key));
		ArgumentNullException.ThrowIfNull(value, nameof(value));

		Headers.Add(CreateHeader(key, value));

		return this;
	}

	public virtual IMittoPackageBuilder AddHeaders(IEnumerable<KeyValuePair<string, string>> headers)
	{
		ArgumentNullException.ThrowIfNull(headers, nameof(headers));

		Headers.AddRange(CreateHeaders(headers));

		return this;
	}

	public virtual IMittoPackageBuilder AddHeaders(params IReadOnlyList<KeyValuePair<string, string>> headers)
	{
		ArgumentNullException.ThrowIfNull(headers, nameof(headers));
		
		Headers.AddRange(CreateHeaders(headers));

		return this;
	}

	public virtual IMittoPackageBuilder WithPackage(string package)
	{
		ArgumentNullException.ThrowIfNull(package, nameof(package));

		Package = package;

		return this;
	}

	public abstract IMittoPackage Build();

	protected abstract IMittoHeader CreateHeader(string key, string value);

	protected ReadOnlySpan<IMittoHeader> CreateHeaders(IEnumerable<KeyValuePair<string, string>> headers)
	{
		return headers.Select(
			kvp => CreateHeader(kvp.Key, kvp.Value)
		).ToArray();
	}

	protected abstract IMittoHeader CreateHeader(MittoHeaderKey key, string value);

	protected ReadOnlySpan<IMittoHeader> CreateHeaders(IEnumerable<KeyValuePair<MittoHeaderKey, string>> headers)
	{
		return headers.Select(
			kvp => CreateHeader(kvp.Key, kvp.Value)
		).ToArray();
	}
}
