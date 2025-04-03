namespace Transmitto.Net;

public enum TransmittoEventType
{
	None = 0,
	ConsumeTopic = 1,
	PublishMessage = 2,
	Authentication = 3,
	Completed = 5,
	Error = 6,
	Unauthorized = 7
}