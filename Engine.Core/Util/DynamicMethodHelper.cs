using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json.Utilities;
namespace Engine.Util
{
	internal class DynamicMethodHelper
	{

		#region Get

		//public static Func<T, object> CreateGet<T>(MemberInfo memberInfo)
		//{
		//	var propertyInfo = memberInfo as PropertyInfo;
		//	if (propertyInfo != null)
		//	{
		//		return CreateGet<T>(propertyInfo);
		//	}

		//	var fieldInfo = memberInfo as FieldInfo;
		//	if (fieldInfo != null)
		//	{
		//		return CreateGet<T>(fieldInfo);
		//	}

		//	throw new Exception(string.Format(CultureInfo.InvariantCulture, "Could not create getter for {0}.", memberInfo));
		//}

		public static Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
		{
			var dynamicMethod = CreateDynamicMethod("Get" + propertyInfo.Name, typeof(object), new[] { typeof(T) }, propertyInfo.DeclaringType);
			var generator = dynamicMethod.GetILGenerator();

			GenerateCreateGetPropertyIL(propertyInfo, generator);

			return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
		}

		private static void GenerateCreateGetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
		{
			var getMethod = propertyInfo.GetGetMethod(true);
			if (getMethod == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Property '{0}' does not have a getter.", propertyInfo.Name));
			}

			if (!getMethod.IsStatic)
			{
				generator.PushInstance(propertyInfo.DeclaringType);
			}

			generator.CallMethod(getMethod);
			generator.BoxIfNeeded(propertyInfo.PropertyType);
			generator.Return();
		}

		public static Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
		{
			if (fieldInfo.IsLiteral)
			{
				var constantValue = fieldInfo.GetValue(null);
				Func<T, object> getter = o => constantValue;
				return getter;
			}

			var dynamicMethod = CreateDynamicMethod("Get" + fieldInfo.Name, typeof(T), new[] { typeof(object) }, fieldInfo.DeclaringType);
			var generator = dynamicMethod.GetILGenerator();

			GenerateCreateGetFieldIL(fieldInfo, generator);

			return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
		}

		private static void GenerateCreateGetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
		{
			if (!fieldInfo.IsStatic)
			{
				generator.PushInstance(fieldInfo.DeclaringType);
				generator.Emit(OpCodes.Ldfld, fieldInfo);
			}
			else
			{
				generator.Emit(OpCodes.Ldsfld, fieldInfo);
			}

			generator.BoxIfNeeded(fieldInfo.FieldType);
			generator.Return();
		}

		#endregion

		#region set

		//public static Action<T, object> CreateSet<T>(MemberInfo memberInfo)
		//{
		//	var propertyInfo = memberInfo as PropertyInfo;
		//	if (propertyInfo != null)
		//	{
		//		return CreateSet<T>(propertyInfo);
		//	}

		//	var fieldInfo = memberInfo as FieldInfo;
		//	if (fieldInfo != null)
		//	{
		//		return CreateSet<T>(fieldInfo);
		//	}

		//	throw new Exception(string.Format(CultureInfo.InvariantCulture, "Could not create setter for {0}.", memberInfo));
		//}

		public static Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
		{
			var dynamicMethod = CreateDynamicMethod("Set" + fieldInfo.Name, null, new[] { typeof(T), typeof(object) }, fieldInfo.DeclaringType);
			var generator = dynamicMethod.GetILGenerator();

			GenerateCreateSetFieldIL(fieldInfo, generator);

			return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
		}

		private static void GenerateCreateSetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
		{
			if (!fieldInfo.IsStatic)
			{
				generator.PushInstance(fieldInfo.DeclaringType);
			}

			generator.Emit(OpCodes.Ldarg_1);
			generator.UnboxIfNeeded(fieldInfo.FieldType);

			if (!fieldInfo.IsStatic)
			{
				generator.Emit(OpCodes.Stfld, fieldInfo);
			}
			else
			{
				generator.Emit(OpCodes.Stsfld, fieldInfo);
			}

			generator.Return();
		}

		public static Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
		{
			var dynamicMethod = CreateDynamicMethod("Set" + propertyInfo.Name, null, new[] { typeof(T), typeof(object) }, propertyInfo.DeclaringType);
			var generator = dynamicMethod.GetILGenerator();

			GenerateCreateSetPropertyIL(propertyInfo, generator);

			return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
		}

		private static void GenerateCreateSetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
		{
			var setMethod = propertyInfo.GetSetMethod(true);
			if (!setMethod.IsStatic)
			{
				generator.PushInstance(propertyInfo.DeclaringType);
			}

			generator.Emit(OpCodes.Ldarg_1);
			generator.UnboxIfNeeded(propertyInfo.PropertyType);
			generator.CallMethod(setMethod);


		}

		#endregion

		private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
		{
			var dynamicMethod = !owner.IsInterface()
				? new DynamicMethod(name, returnType, parameterTypes, owner, true)
				: new DynamicMethod(name, returnType, parameterTypes, owner.Module, true);

			return dynamicMethod;
		}
	}
}
