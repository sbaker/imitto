namespace IMitto.Protocols;

public enum MittoAction : short
{
	None = 0,
	Auth = 1,
	Produce = 2,
	Connect = 4,
	Disconnect = 8,
	Consume = 16,
	Stream = 32,
	Session = 64,
	//Reserved1 = 128,
	//Reserved2 = 256,
	//Reserved3 = 512,
	//Reserved4 = 1024,
	//Reserved5 = 2048,
	//Reserved6 = 4096,
	//Reserved7 = 8192,
	//Reserved8 = 16384
}
