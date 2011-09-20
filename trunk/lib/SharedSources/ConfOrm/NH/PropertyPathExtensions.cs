using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping;

namespace ConfOrm.NH
{
	public static class PropertyPathExtensions
	{
		public static Type GetContainerEntity(this PropertyPath propertyPath, IDomainInspector domainInspector)
		{
			PropertyPath analizing = propertyPath;
			while (analizing.PreviousPath != null && !domainInspector.IsEntity(analizing.LocalMember.ReflectedType))
			{
				analizing = analizing.PreviousPath;
			}
			return analizing.LocalMember.ReflectedType;
		}

		public static string ToColumnName(this PropertyPath propertyPath, string pathSeparator)
		{
			return propertyPath.ToString().Replace(".", pathSeparator);
		}

		/// <summary>
		/// Provide the list of progressive-paths
		/// </summary>
		/// <param name="source"></param>
		/// <returns>
		/// Given a path as : Pl1.Pl2.Pl3.Pl4.Pl5 returns paths-sequence as:
		/// Pl5
		/// Pl4.Pl5
		/// Pl3.Pl4.Pl5
		/// Pl2.Pl3.Pl4.Pl5
		/// Pl1.Pl2.Pl3.Pl4.Pl5
		/// </returns>
		public static IEnumerable<PropertyPath> InverseProgressivePath(this PropertyPath source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			var analizing = source;
			var returnLocalMembers = new List<MemberInfo>(10);
			do
			{
				returnLocalMembers.Add(analizing.LocalMember);
				PropertyPath progressivePath = null;
				for (int i = returnLocalMembers.Count -1; i >= 0; i--)
				{
					progressivePath = new PropertyPath(progressivePath, returnLocalMembers[i]);
				}
				yield return progressivePath;
				analizing = analizing.PreviousPath;
			}
			while (analizing != null);
		}
	}
}