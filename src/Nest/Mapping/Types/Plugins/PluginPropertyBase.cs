using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject(MemberSerialization.OptIn)]
	[ContractJsonConverter(typeof(PropertyJsonConverter))]
	public interface IPluginProperty : IProperty
	{
		string PluginType { get; }
	}

	[DebuggerDisplay("{DebugDisplay}")]
	public abstract class PluginPropertyBase : PropertyBase,  IPluginProperty
	{
		private readonly string _pluginType;
		string IPluginProperty.PluginType => this._pluginType;

		public override TypeName Type => this._pluginType;
		protected PluginPropertyBase(string type) : base(FieldType.None)
		{
			this._pluginType = type;
		}
		protected string DebugDisplay => $"Type: {this._pluginType}, Name: {Name.DebugDisplay} ";

	}
	public abstract class PluginPropertyDescriptorBase<TDescriptor, TInterface, T>
		: PropertyDescriptorBase<TDescriptor, TInterface, T>, IPluginProperty
		where TDescriptor : PluginPropertyDescriptorBase<TDescriptor, TInterface, T>, TInterface
		where TInterface : class, IPluginProperty
		where T : class
	{
		private readonly string _pluginType;
		string IPluginProperty.PluginType => this._pluginType;

		TypeName IProperty.Type
		{
			get { return this._pluginType; }
			set { }
		}

		protected string DebugDisplay => $"Type: {this._pluginType}, Name: {Self.Name.DebugDisplay} ";

		protected PluginPropertyDescriptorBase(string type) : base(FieldType.None)
		{
			this._pluginType = type;
		}
	}
}
