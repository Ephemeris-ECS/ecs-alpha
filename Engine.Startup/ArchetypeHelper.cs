using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Engine.Archetypes;

namespace Engine.Startup
{
	public class ArchetypeHelper
	{
		public static List<Archetype> GetPublicStaticArchetypes<TSource>()
		{
			return GetPublicStaticArchetypes(typeof(TSource));
		}

		public static List<Archetype> GetPublicStaticArchetypes(Type sourceType)
		{
			var archetypes = new List<Archetype>();
			foreach(var fieldInfo in sourceType.GetFields(BindingFlags.Static | BindingFlags.Public).Where(fi => typeof(Archetype).IsAssignableFrom(fi.FieldType)))
			{
				archetypes.Add((Archetype) fieldInfo.GetValue(null));
			}
			return archetypes;
		}

		public static List<Archetype> GetPublicInstanceArchetypes(object source)
		{
			var archetypes = new List<Archetype>();
			foreach (var fieldInfo in source.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(fi => typeof(Archetype).IsAssignableFrom(fi.FieldType)))
			{
				archetypes.Add((Archetype)fieldInfo.GetValue(source));
			}
			return archetypes;
		}
	}
}
