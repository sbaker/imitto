using System.Text.Json;
using Transmitto.Net;
using Transmitto.Net.Models;
using Transmitto.Net.Requests;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Transmitto;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TransmittoMessageReadAsExtensions
{
	public static TBody ReadBodyAs<TBody>(this ITransmittoMessage message, JsonSerializerOptions options)
		where TBody : notnull, TransmittoMessageBody, new()
	{
		ArgumentNullException.ThrowIfNull(message, nameof(message));
		ArgumentNullException.ThrowIfNull(options, nameof(options));

		var body = new TBody();

		if (!message.HasBody())
		{
			return body;
		}

		var messageBody = message.GetBody();

		if (!string.IsNullOrEmpty(messageBody.RawBody))
		{
			body = JsonSerializer.Deserialize<TBody>(messageBody.RawBody!, options) ?? new TBody();
		}

		body.RawBody = messageBody.RawBody;

		return body;
	}
}
