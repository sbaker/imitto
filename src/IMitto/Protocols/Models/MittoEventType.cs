namespace IMitto.Protocols.Models;

public enum MittoEventType
{
	None = 0,
	Consume = 1,
	Produce = 2,
	Authentication = 3,
	Completed = 5,
	Error = 6,
	Unauthorized = 7
}