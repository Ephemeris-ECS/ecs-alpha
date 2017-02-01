using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Engine.Components;
using Engine.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Zenject;

namespace Engine.Serialization
{
	// ReSharper disable once InconsistentNaming
	public class ECSContractResolver : DefaultContractResolver
	{
		private readonly DiContainer _container;

		private readonly HashSet<int> _entitiesDeserialized;

		private bool _entityCreated;

		private readonly IComponentRegistry _componentRegistry;

		private readonly Dictionary<Type, bool> _contractsResolved;

		public bool TrackDeserializedEntities { get; set; } = true;
		
		public ECSContractResolver(DiContainer container, IComponentRegistry componentRegistry)
		{
			_container = container;
			_entitiesDeserialized = new HashSet<int>();
			_componentRegistry = componentRegistry;
			_contractsResolved = new Dictionary<Type, bool>();
		}

		public override JsonContract ResolveContract(Type type)
		{
			var contract = base.ResolveContract(type);

			bool contractResolved;
			if (_contractsResolved.TryGetValue(type, out contractResolved) == false)
			{
				_contractsResolved.Add(type, true);
			}
			
			if (type == typeof(EntityDictionary) && contractResolved == false)
			{
				contract.DefaultCreator = () => ResolveCreator(type);
				if (TrackDeserializedEntities)
				{
					contract.OnDeserializedCallbacks.Add(EntityDictionaryDeserialized);
				}
			}
			else if (type == typeof(Entity) && contractResolved == false)
			{
				contract.DefaultCreator = () => CreateEntity(type);

				if (TrackDeserializedEntities)
				{
					contract.OnDeserializedCallbacks.Add(EntityDeserialized);
				}
			}
			else if (typeof(IComponent).IsAssignableFrom(type) && contractResolved == false)
			{
				contract.DefaultCreator = () => InstantiateCreator(type);
			}

			return contract;
		}

		private void EntityDeserialized(object obj, StreamingContext streamingContext)
		{
			var entity = obj as Entity;
			if (entity != null)
			{
				_entitiesDeserialized.Add(entity.Id);
				if (_entityCreated)
				{
					_componentRegistry.UpdateMatcherGroups(entity);
				}
			}
			_entityCreated = false;
		}

		private void EntityDictionaryDeserialized(object obj, StreamingContext streamingContext)
		{
			var entityDictionary = obj as EntityDictionary;
			if (entityDictionary != null)
			{
				foreach (var i in entityDictionary.Keys.Except(_entitiesDeserialized).ToArray())
				{
					Entity entity;
					if (entityDictionary.TryGetValue(i, out entity))
					{
						// TODO: see if there is a better way to clean up these
						entity.Dispose();
						entityDictionary.Remove(i);
					}
				}
			}
			_entitiesDeserialized.Clear();
		}

		private object ResolveCreator(Type type)
		{
			return _container.Resolve(type);
		}

		private object CreateEntity(Type type)
		{
			_entityCreated = true;
			return InstantiateCreator(type);
		}

		private object InstantiateCreator(Type type)
		{
			// TODO: RESOLVE OBJECT POOL AND GET FROM THERE
			return _container.Instantiate(type);
		}

		//protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
		//{
		//	return base.CreateDictionaryContract(objectType);
		//}

		//protected override JsonArrayContract CreateArrayContract(Type objectType)
		//{
		//	var contract =  base.CreateArrayContract(objectType);

		//	//if (objectType.IsEntityCollection())
		//	//{
		//	//	contract.It
		//	//}

		//	return contract;
		//}

		//private void AttachEntityValueProviderDeserializationCallback(ECSValueProvider valueProvider)
		//{
		//	valueProvider.DeserializingEntityReference += ValueProviderOnDeserializingEntityReference;
		//}

		//private void ValueProviderOnDeserializingEntityReference(object sender, DeserializingEntityReferenceEventArgs deserializingEntityReferenceEventArgs)
		//{
		//	_entityReferenceResolverQueue.Add(deserializingEntityReferenceEventArgs.Setter);
		//}

		//public void ResolveEntityReferences()
		//{
		//	foreach (var entityReferenceResolveAction in _entityReferenceResolverQueue)
		//	{
		//		entityReferenceResolveAction();
		//	}
		//	_entityReferenceResolverQueue.Clear();
		//}


		#region property resolver

		protected override JsonProperty CreateProperty(
			MemberInfo member,
			MemberSerialization memberSerialization)
		{
			//TODO: Maybe cache
			var prop = base.CreateProperty(member, memberSerialization);

			if (!prop.Writable)
			{
				var property = member as PropertyInfo;
				if (property != null)
				{
					var hasPrivateSetter = property.GetSetMethod(true) != null;
					prop.Writable = hasPrivateSetter;
				}
			}

			return prop;
		}

		//protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		//{
		//	List<JsonProperty> properties;
		//	if (_propertyCache.TryGetValue(type, out properties) == false)
		//	{
		//		var typeIsEntity = type.IsEntity();
		//		var orderedProperties = new List<OrderedProperty>();

		//		var currentType = type;
		//		while (currentType != null && currentType != typeof(object))
		//		{
		//			var props = currentType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

		//			foreach (var propertyInfo in props)
		//			{
		//				var syncStateAttribute =
		//					propertyInfo.GetCustomAttributes(true)
		//						.OfType<SyncStateAttribute>()
		//						.SingleOrDefault(ssa => Level.IncludesFlag(ssa.Levels));
		//				if (syncStateAttribute == null)
		//				{
		//					continue;
		//				}

		//				var jsonProperty = base.CreateProperty(propertyInfo, memberSerialization);
		//				jsonProperty.Writable = true;
		//				jsonProperty.Readable = true;

		//				if ((propertyInfo.PropertyType.IsEntityCollection() || propertyInfo.PropertyType.IsEntity()) 
		//					&& type.IsECS() == false)
		//				{
		//					jsonProperty.ValueProvider = new ECSValueProvider(_ecs, propertyInfo);
		//				}

		//				orderedProperties.Add(new OrderedProperty()
		//				{
		//					Property = jsonProperty,
		//					Order = syncStateAttribute.Order
		//				});
		//			}

		//			foreach (var fieldInfo in currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
		//			{
		//				var syncStateAttribute = fieldInfo.GetCustomAttributes(true)
		//						.OfType<SyncStateAttribute>()
		//						.SingleOrDefault(ssa => Level.IncludesFlag(ssa.Levels));
		//				if (syncStateAttribute == null)
		//				{
		//					continue;
		//				}
		//				var jsonProperty = base.CreateProperty(fieldInfo, memberSerialization);
		//				jsonProperty.Writable = true;
		//				jsonProperty.Readable = true;

		//				if (fieldInfo.FieldType.IsEntity() && type.IsECS() == false)
		//				{
		//					jsonProperty.ValueProvider = new ECSValueProvider(_ecs, fieldInfo);
		//				}

		//				orderedProperties.Add(new OrderedProperty()
		//				{
		//					Property = jsonProperty,
		//					Order = syncStateAttribute.Order
		//				});
		//			}
		//			currentType = currentType.BaseType;
		//		}
		//		orderedProperties.Sort();
		//		properties = orderedProperties.Select(op => op.Property).ToList();
		//		_propertyCache.Add(type, properties);
		//	}
		//	return properties;
		//}

		#endregion
	}
}
