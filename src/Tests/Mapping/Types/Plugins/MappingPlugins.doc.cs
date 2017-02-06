using System;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Newtonsoft.Json;
using Tests.Framework;
using Xunit.Sdk;
using static Tests.Framework.RoundTripper;

namespace Tests.Mapping.Types.Plugins
{
	public class MappingPlugins : DocumentationTestBase
	{

		private interface IMyPluginProperty : IPluginProperty
		{
			[JsonProperty("some_value")]
			int SomeValue { get; set; }
		}

		public class MyPluginProperty : PluginPropertyBase, IMyPluginProperty
		{
			public MyPluginProperty() : base("my_plugin_type") { }

			public int SomeValue { get; set; }
		}

		private PutMappingRequest _putMappingRequest;
		public MappingPlugins()
		{
			this._putMappingRequest = new PutMappingRequest("index", "type")
			{
				Properties = new Properties
				{
					{"my_property", new MyPluginProperty {SomeValue = 12}}
				}
			};
		}

		[U] public void CanSendCustomPluginType() => Expect(new
			{
				properties = new {
					my_property = new {
						some_value = 12,
						type = "my_plugin_type"
					}
				}
			})
			.WhenSerializingOnce(this._putMappingRequest);

		//TODO we have no mechanism to hook in deserializing, something for 6.0
		[U] public void DeserializingThrows() => Expect(new
			{
				properties = new {
					my_property = new {
						some_value = 12,
						type = "my_plugin_type"
					}
				}
			})
			.ThrowsOnRoundtrip<PutMappingRequest, ArgumentException>(this._putMappingRequest, e =>
			{
				e.Message.Should().Contain("not supported");
				e.Message.Should().Contain("not a built in property");
			});
	}
}
