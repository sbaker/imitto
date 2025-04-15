using System.Text.Json;
using IMitto.Protocols;
using IMitto.Protocols.Models;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace IMitto;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MittoMessageReadAsExtensions
{
	public static TBody ReadBodyAs<TBody>(this IMittoMessage message, JsonSerializerOptions options)
		where TBody : notnull, MittoMessageBody, new()
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
