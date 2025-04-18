namespace IMitto.Protocols;

public abstract class MittoPackageBuilderBase : IPackageBuilder
{
	protected MittoPackageBuilderBase()
	{
		Headers.AddRange(
			CreateHeaderFromKvp("encoding", "utf-8"),
			CreateHeaderFromKvp("timestamp", TimeProvider.System.GetUtcNow().ToString("u"))
		);
	}

	protected abstract MittoProtocolVersion Version { get; }

	protected MittoAction Action { get; set; } = MittoAction.None;

	protected List<IMittoHeader> Headers { get; } = [];

	protected MittoModifier Modifier { get; set; } = MittoModifier.None;
	
	protected string? Package { get; set; }

	public virtual IPackageBuilder WithAction(MittoAction action)
	{
		if (action == MittoAction.None)
		{
			throw new ArgumentException("Action cannot be None.", nameof(action));
		}

		Action = action;
		return this;
	}

	public virtual IPackageBuilder WithModifier(MittoModifier modifier)
	{
		Modifier = modifier;
		return this;
	}

	public virtual IPackageBuilder AddHeader(string key, string value)
	{
		ArgumentNullException.ThrowIfNull(key, nameof(key));
		ArgumentNullException.ThrowIfNull(value, nameof(value));

		Headers.Add(CreateHeaderFromKvp(key, value));

		return this;
	}

	public virtual IPackageBuilder AddHeaders(IEnumerable<KeyValuePair<string, string>> headers)
	{
		ArgumentNullException.ThrowIfNull(headers, nameof(headers));

		Headers.AddRange(CreateKvpToHeader(headers));

		return this;
	}

	public virtual IPackageBuilder AddHeaders(params IReadOnlyList<KeyValuePair<string, string>> headers)
	{
		ArgumentNullException.ThrowIfNull(headers, nameof(headers));
		
		Headers.AddRange(CreateKvpToHeader(headers));

		return this;
	}

	public virtual IPackageBuilder WithPackage(string package)
	{
		ArgumentNullException.ThrowIfNull(package, nameof(package));

		Package = package;

		return this;
	}

	public abstract IMittoPackage Build();

	protected abstract IMittoHeader CreateHeaderFromKvp(string key, string value);

	protected ReadOnlySpan<IMittoHeader> CreateKvpToHeader(IEnumerable<KeyValuePair<string, string>> headers)
	{
		return headers.Select(
			kvp => CreateHeaderFromKvp(kvp.Key, kvp.Value)
		).ToArray();
	}
}
