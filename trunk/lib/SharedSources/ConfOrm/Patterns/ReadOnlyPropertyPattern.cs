using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class ReadOnlyPropertyPattern: IPattern<MemberInfo>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			var property = subject as PropertyInfo;
			if(property == null)
			{
				return false;
			}
			if (CanReadCantWriteInsideType(property) || CanReadCantWriteInBaseType(property))
			{
				return !PropertyToFieldPatterns.Defaults.Any(p=> p.Match(property)) || IsAutoproperty(property);
			}
			return false;
		}

		private bool IsAutoproperty(PropertyInfo property)
		{
			return property.ReflectedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
																					 | BindingFlags.DeclaredOnly).Any(pi => pi.Name == string.Concat("<", property.Name, ">k__BackingField"));
		}

		#endregion

		private bool CanReadCantWriteInsideType(PropertyInfo property)
		{
			return !property.CanWrite && property.CanRead && property.DeclaringType == property.ReflectedType;
		}

		private bool CanReadCantWriteInBaseType(PropertyInfo property)
		{
			if(property.DeclaringType == property.ReflectedType)
			{
				return false;
			}
			var rfprop = property.DeclaringType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
			                                     | BindingFlags.DeclaredOnly).SingleOrDefault(pi => pi.Name == property.Name);
			return rfprop != null && !rfprop.CanWrite && rfprop.CanRead;
		}
	}
}