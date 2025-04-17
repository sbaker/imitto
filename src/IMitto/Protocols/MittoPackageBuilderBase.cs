namespace IMitto.Protocols;

public abstract class MittoPackageBuilderBase : IPackageBuilder
{
	protected abstract MittoProtocolVersion Version { get; }

	protected MittoAction Action { get; set; } = MittoAction.None;

	protected List<KeyValuePair<string, string>> Headers { get; } = [
		new KeyValuePair<string, string>("encoding", "utf-8"),
		new KeyValuePair<string, string>("timestamp", TimeProvider.System.GetUtcNow().ToString("u")),
	];

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

		Headers.Add(new KeyValuePair<string, string>(key, value));

		return this;
	}

	public virtual IPackageBuilder AddHeaders(IEnumerable<KeyValuePair<string, string>> headers)
	{
		ArgumentNullException.ThrowIfNull(headers, nameof(headers));

		Headers.AddRange(headers);

		return this;
	}

	public virtual IPackageBuilder AddHeaders(params IReadOnlyList<KeyValuePair<string, string>> headers)
	{
		ArgumentNullException.ThrowIfNull(headers, nameof(headers));
		
		Headers.AddRange(headers);

		return this;
	}

	public virtual IPackageBuilder WithPackage(string package)
	{
		ArgumentNullException.ThrowIfNull(package, nameof(package));

		Package = package;

		return this;
	}

	public abstract IMittoPackage Build();
}
