namespace Microsoft.Extensions.DependencyInjection
{
	public interface IMittoBuilder
	{
		IServiceCollection Services { get; }

		void AddTopicTypeMapping<TMapping>(string topic) where TMapping : class;

		IServiceCollection Build();
	}
}