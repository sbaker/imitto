namespace IMitto.Net.Models;

public class ClientNotificationBody : TransmittoMessageBody
{
	public ClientNotificationModel Notification { get; set; }
}

public class ClientNotificationModel
{
	// TODO: Send something to the server to let then know which client sent it.

	public string? Topic { get; set; }

	public string? Message { get; set; }
}