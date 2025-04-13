using IMitto.Settings;
using System.Buffers;
using System.IO.Pipelines;

namespace IMitto.Pipelines;

public class SerializingPipeReader : PipeReader
{
	private readonly PipeReader _innerReader;
	private readonly MittoPipeOptions _options;

	public SerializingPipeReader(Stream stream, MittoPipeOptions options)
		: this(Create(stream, options.CreateReaderOptions()), options)
	{

	}

	public SerializingPipeReader(PipeReader innerReader, MittoPipeOptions options)
	{
		_innerReader = innerReader;
		_options = options;
	}

	public override void AdvanceTo(SequencePosition consumed)
	{
		_innerReader.AdvanceTo(consumed);
	}

	public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
	{
		_innerReader.AdvanceTo(consumed, examined);
	}

	public override void CancelPendingRead()
	{
		_innerReader.CancelPendingRead();
	}

	public override void Complete(Exception? exception = null)
	{
		_innerReader.Complete(exception);
	}

	public override ValueTask<ReadResult> ReadAsync(CancellationToken token = default)
	{
		return _innerReader.ReadAsync(token);
	}

	public async ValueTask<T> ReadValueAsync<T>(CancellationToken token = default)
	{
		T value = default!;

		while (true)
		{
			ReadResult result = await _innerReader.ReadAsync(token).Await();

			SequencePosition? position = null;
			var buffer = result.Buffer;

			try
			{
				do
				{
					position = buffer.PositionOf(_options.CharTerminator);

					if (position != null)
					{
						value = Serialize(buffer.Slice(0, position.Value))!;
						buffer = buffer.Slice(buffer.GetPosition(1, position.Value));

						return value;
					}
				}
				while (position != null);

				_innerReader.AdvanceTo(buffer.Start, buffer.End);

				if (result.IsCompleted)
				{
					if (buffer.Length > 0)
					{
						throw new InvalidDataException("Reader completed with an unread buffer.");
					}

					break;
				}
			}
			catch (Exception e)
			{

			}
			finally
			{
				_innerReader.AdvanceTo(buffer.Start, buffer.End);
			}
		}

		return value;

		T Serialize(ReadOnlySequence<byte> buffer)
		{
			var serializer = _options.CreateDefaultReaderSerializer<T>();

			if (buffer.IsSingleSegment)
			{
				// Convert the buffer to a string
				var value = _options.Encoding.GetString(buffer.FirstSpan);

				// Deserialize the string to the object
				return serializer(value);
			}

			// Convert the buffer to a string
			var result = string.Create((int)buffer.Length, buffer, (span, sequence) =>
			{
				foreach (var segment in sequence)
				{
					_options.Encoding.GetChars(segment.Span, span);

					span = span.Slice(segment.Length);
				}
			});
				
			// Deserialize the string to the object
			return serializer(result);
		}
	}

	public override bool TryRead(out ReadResult result)
	{
		if (_innerReader.TryRead(out var innerResult))
		{
			result = innerResult;
			return true;
		}

		result = default;
		return false;
	}
}
