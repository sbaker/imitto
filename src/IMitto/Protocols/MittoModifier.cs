namespace IMitto.Protocols;

public enum MittoModifier : byte
{
	None = 0,
	End = 1,
	Start = 2,
	Ack = 4,
	Nack = 8,
	Error = 16,
	//Reserved1 = 32,
	//Reserved2 = 64,
	//Reserved3 = 128,
}
