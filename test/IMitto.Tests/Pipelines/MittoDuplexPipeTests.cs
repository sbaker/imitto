using IMitto.Pipelines;
using IMitto.Net.Settings;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using IMitto.Net.Models;
using IMitto.Converters;

namespace IMitto.Tests.Pipelines;

public class MittoDuplexPipeTests
{
	[Fact]
	public void Constructor_ShouldInitializeReaderAndWriter()
	{
		// Arrange
		var stream = new MemoryStream();
		var options = new MittoPipeOptions();

		// Act
		var pipe = new MittoDuplexPipe(stream, options);

		// Assert
		Assert.NotNull(pipe.Input);
		Assert.NotNull(pipe.Output);
	}

	[Fact]
	public async Task ReadAsync_ShouldReturnReadResult()
	{
		// Arrange
		var json = new MittoJsonOptions();
		json.Serializer.Converters.Add(new MittoStringBodyConverter());
		var options = new MittoPipeOptions(json, Encoding.UTF8);

		var stream = new MemoryStream(new byte[1000]);
		var pipe = new MittoDuplexPipe<MittoStringMessageBody>(stream, options);
		var cancellationToken = new CancellationToken();
		
		var message = new MittoStringMessageBody()
		{
			Content = "Test data Test dataTest dataTest dataTest dataTest dataTest data Test data Test data Test data Test data Test data Test data Test dataTest data"
		};
		
		await pipe.Writer.WriteAsync(message, cancellationToken);
		stream.Write([options.CharTerminator]);
		stream.Position = 0;

		var result = await pipe.Reader.ReadValueAsync(cancellationToken);
		
		message = result;

		// Assert
		Assert.NotNull(result);
	}
}
