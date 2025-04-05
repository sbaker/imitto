﻿using System.Net;
using System.Text;

namespace IMitto.Net.Settings;

public abstract class MittoBaseOptions
{
	public MittoConnectionOptions Connection { get; } = new MittoConnectionOptions()
	{
		Host = new MittoHost(IPAddress.Loopback)
	};

	public Encoding Encoding { get; set; } = Encoding.UTF8;

	public MittoEventsOptions Events { get; set; } = new MittoEventsOptions();

	public MittoJsonOptions Json { get; set; } = new MittoJsonOptions();

	public int MaxConnectionRetries { get; set; } = 5;
}
