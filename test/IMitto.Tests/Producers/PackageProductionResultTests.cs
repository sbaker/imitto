using IMitto.Producers;

namespace IMitto.Tests.Producers;

public class PackageProductionResultTests
{
	[Fact]
	public void Success_ShouldReturnSuccessfulResult()
	{
		// Arrange
		var package = "test-package";

		// Act
		var result = PackageProductionResult.Success(package);

		// Assert
		Assert.True(result.Produced);
		Assert.Equal(package, result.Package);
	}

	[Fact]
	public void Failure_ShouldReturnFailedResult()
	{
		// Arrange
		var package = "test-package";

		// Act
		var result = PackageProductionResult.Failure<string>(new Exception(package));

		// Assert
		Assert.False(result.Produced);
		Assert.Equal(default, result.Package);
		Assert.Equal(default, result.Package);
	}

	[Fact]
	public void GetPackagedGoods_ShouldReturnPackagedGoods()
	{
		// Arrange
		var package = "test-package";
		var result = new PackageProductionResult<string>(package);
		var topic = "test-topic";

		// Act
		var packagedGoods = result.GetPackagedGoods(topic);

		// Assert
		Assert.Equal(package, packagedGoods.Goods);
		Assert.Equal(topic, packagedGoods.Topic);
	}
}

