using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm
{
	internal static class EnumerableExtensions
	{
		public static bool ContainsMember(this ICollection<MemberInfo> source, MemberInfo item)
		{
			return source.Count > 0 && (source.Contains(item) || (!item.DeclaringType.Equals(item.ReflectedType) && source.Contains(item.GetMemberFromDeclaringType())) ||
			                            item.GetPropertyFromInterfaces().Any(source.Contains));
		}

		public static bool IsSingle<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicateOfTheSingleFound)
		{
			if (source == null)
			{
				return false;
			}
			var list = source as IList<TSource>;
			if (list != null)
			{
				return list.Count == 1 && predicateOfTheSingleFound(list[0]);
			}
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					return false;
				}
				var mayBeSingleElement = enumerator.Current;
				if (!enumerator.MoveNext())
				{
					return predicateOfTheSingleFound(mayBeSingleElement);
				}
			}
			return false;
		}
	}
}