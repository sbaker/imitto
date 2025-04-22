using System.Buffers;
using System.Numerics;
using System.Text;

namespace IMitto.Protocols;

public interface IMittoByteContent<TLength> : IMittoMessageItem where TLength : struct, INumber<TLength>
{
	Length<TLength> ContentLength { get; }

	ReadOnlySequence<byte> Content { get; set; }
}


public class MittoByteContent<TLength> : IMittoByteContent<TLength> where TLength : struct, INumber<TLength>
{
	public MittoByteContent(ReadOnlySequence<byte> content)
	{
		ContentLength = new Length<TLength>(() => TLength.CreateChecked(Content.Length));

		Content = new ReadOnlySequence<byte>(content.ToArray());
		IsSerialized = !content.IsEmpty;
	}

	public MittoByteContent() : this(ReadOnlySequence<byte>.Empty)
	{
	}

	public Length<TLength> ContentLength { get; }

	public ReadOnlySequence<byte> Content { get; set; }

	public bool IsSerialized { get; protected set; }

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
			return Encoding.UTF8.GetString(content.First.Span);
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