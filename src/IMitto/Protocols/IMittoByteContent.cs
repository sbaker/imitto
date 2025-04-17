using System.Buffers;
using System.Numerics;
using System.Text;

namespace IMitto.Protocols;

public interface IMittoByteContent<TLength> : IMittoMessageItem where TLength : struct, INumber<TLength>
{
	Length<TLength> ContentLength { get; }

	ReadOnlySequence<byte> Content { get; set; }
}


public class MittoByteContent<TLength>(ReadOnlySequence<byte> content) : IMittoByteContent<TLength> where TLength : struct, INumber<TLength>
{
	public MittoByteContent() : this(ReadOnlySequence<byte>.Empty)
	{
	}

	public Length<TLength> ContentLength { get; } = new Length<TLength>(() => TLength.CreateChecked(content.Length));

	public ReadOnlySequence<byte> Content { get; set; } = content;

	public bool IsSerialized { get; protected set; } = !content.IsEmpty;

	public string ReadContentAsString()
	{
		return GetContent(Content);
	}

	protected static string GetContent(ReadOnlySequence<byte> content)
	{
		if (content.IsEmpty)
		{
			return string.Empty;
		}

		if (content.IsSingleSegment)
		{
			return Encoding.UTF8.GetString(content.FirstSpan);
		}

		return string.Create((int)content.Length, content, (result, buffer) =>
		{
			foreach (var segment in buffer)
			{
				Encoding.UTF8.GetChars(segment.Span, result);
			}
		});
	}

	protected string GetContent()
	{
		return GetContent(Content);
	}
}