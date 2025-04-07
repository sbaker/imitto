using Microsoft.Extensions.Options;
using System.Threading.Channels;
using Xunit;

namespace IMitto.Channels.Tests;

public class MittoBoundedChannelProviderTests
{
	private readonly MittoBoundedChannelProvider<int> _provider;
	private readonly MittoBoundedChannelOptions _options;

	public MittoBoundedChannelProviderTests()
	{
		_options = new MittoBoundedChannelOptions { Capacity = 10, ChannelFullMode = BoundedChannelFullMode.Wait };
		var optionsMock = Options.Create(_options);
		_provider = new MittoBoundedChannelProvider<int>(optionsMock);
	}

	[Fact]
	public void GetReader_ShouldReturnChannelReader()
	{
		var reader = _provider.GetReader();
		Assert.NotNull(reader);
	}

	[Fact]
	public void GetWriter_ShouldReturnChannelWriter()
	{
		var writer = _provider.GetWriter();
		Assert.NotNull(writer);
	}

	[Fact]
	public void Complete_ShouldReturnTrue()
	{
		var result = _provider.Complete();
		Assert.True(result);
	}

	[Fact]
	public void Complete_ShouldReturnFalse_WhenAlreadyCompleted()
	{
		_provider.Complete();
		var result = _provider.Complete();
		Assert.False(result);
	}
}
