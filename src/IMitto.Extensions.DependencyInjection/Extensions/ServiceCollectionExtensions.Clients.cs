﻿using IMitto.Consumers;
using IMitto.Extensions.DependencyInjection;
using IMitto.Net.Clients;
using IMitto.Producers;
using IMitto.Protocols.Models;
using IMitto.Settings;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static partial class ServiceCollectionExtensions
{
	public static IServiceCollection AddIMitto(this IServiceCollection services, Action<IMittoClientBuilder> configure, bool startHostedBackgroundService = true)
	{
		var builder = new MittoClientBuilder(services, startHostedBackgroundService);

		configure(builder);

		return builder.Build()
			.AddIMittoChannels<EventNotificationsModel>(_ => { })
			.AddSingleton(typeof(IMittoProducerProvider<>), typeof(MittoProducerProvider<>))
			.AddSingleton<IMittoClientEventManager, MittoClientEventManager>()
			.AddSingleton<IMittoEventDispatcher, ChannelMittoEventDispatcher>();
	}

	internal class MittoClientBuilder(IServiceCollection services, bool startHostedBackgroundService) : IMittoClientBuilder
	{
		private readonly Dictionary<string, TopicPackageTypeMapping> _mappings = [];

		public IServiceCollection Services { get; } = services;

		private bool StartHostedBackgroundService { get; } = startHostedBackgroundService;

		public void AddProducerMapping<TPackage>(string topic) where TPackage : class
		{
			TopicPackageTypeMapping? typeMapping = null;

			if (_mappings.TryGetValue(topic, out var value))
			{
				typeMapping = value;
				typeMapping.MappingType |= TopicMappingType.Producer;
			}

			_mappings[topic] = typeMapping ?? new TopicPackageTypeMapping(typeof(TPackage), topic, TopicMappingType.Producer);
		}

		public void AddTopicTypeMapping<TConsumer, TPackage>(string topic, TopicMappingType mappingType)
			where TConsumer : class, IMittoPackageConsumer<TPackage>
			where TPackage : class
		{
			TopicPackageTypeMapping? typeMapping = null;

			if (_mappings.TryGetValue(topic, out var value))
			{
				typeMapping = value;
				typeMapping.MappingType |= mappingType;
				typeMapping.ConsumerTypes.Add(typeof(TConsumer));
			}

			_mappings[topic] = typeMapping ?? new TopicPackageTypeMapping(typeof(TPackage), topic, mappingType);
		}

		public IServiceCollection Build()
		{
			var services = Services.Configure<MittoClientOptions>(options =>
			{
				foreach (var mapping in _mappings)
				{
					var typeMapping = mapping.Value;


					// TODO: Schema for the topic. Needs work and I don't want to introduce another library.

					//var typeInfo = options.Json.Serializer.GetTypeInfo(typeMapping.PackageType);
					//typeMapping.TopicJsonSchema = typeInfo.GetJsonSchemaAsNode(new JsonSchemaExporterOptions
					//{
					//	TreatNullObliviousAsNonNullable = true,
					//	TransformSchemaNode = (schemaContext, node) => {
					//		//node.TypeInfo = typeInfo;
					//		if (schemaContext.TypeInfo.)
					//		{
					//		}

					//		return node;
					//	}
					//});

					options.TypeMappings.AddOrUpdate(mapping.Key, typeMapping, (s, t) =>
					{
						return typeMapping;
					});
				}
			})
			.AddSingleton<IMittoClient, MittoClient>();

			if (StartHostedBackgroundService)
			{
				services.AddHostedService<MittoClientHostedBackgroundService>();
			}

			return services;
		}

		private sealed class MittoJsonTypeInfoResolver : IJsonTypeInfoResolver
		{
			public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
			{
				throw new NotImplementedException();
			}
		}
	}
}