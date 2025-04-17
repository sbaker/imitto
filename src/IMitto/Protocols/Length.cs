using System.Numerics;

namespace IMitto.Protocols;

public readonly struct Length<T>(Func<T> provider) where T : struct, INumber<T>
{
	private readonly Func<T> _provider = provider;

	public Length(T value) : this(() => value)
	{
	}

	public Length() : this(Zero)
	{
	}

	public static Length<T> Zero { get; } = new Length<T>(T.Zero);

	public T Value => _provider();

	public static implicit operator Length<T>(T value)
	{
		return new Length<T>(T.One * value);
	}

	public static implicit operator T(Length<T> item)
	{
		return item.Value;
	}

	public static implicit operator Length<T>(int value)
	{
		return new Length<T>(T.CreateChecked(value));
	}

	public static implicit operator int(Length<T> item)
	{
		return int.CreateChecked(item.Value);
	}

	public static implicit operator long(Length<T> item)
	{
		return long.CreateChecked(item.Value);
	}

	public static implicit operator Length<T>(long value)
	{
		return new Length<T>(T.CreateChecked(value));
	}

	public static implicit operator Length<T>(byte value)
	{
		return new Length<T>(T.CreateChecked(value));
	}

	public static implicit operator byte(Length<T> item)
	{
		return byte.CreateChecked(item.Value);
	}

	public static implicit operator Length<T>(short value)
	{
		return new Length<T>(T.CreateChecked(value));
	}

	public static implicit operator short(Length<T> item)
	{
		return short.CreateChecked(item.Value);
	}
}