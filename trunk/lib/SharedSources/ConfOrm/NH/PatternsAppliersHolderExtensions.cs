using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Patterns;

namespace ConfOrm.NH
{
	public static class PatternsAppliersHolderExtensions
	{
		private const string NotSupportedApplierExceptionMessageTemplate = "The IPatternsAppliersHolder does not support appliers of {0}, {1}";

		/// <summary>
		/// Add the <paramref name="applier"/> to the correspondig collection inside the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="TSubject">The subject of the pattern.</typeparam>
		/// <typeparam name="TApplyTo">The target of the applier.</typeparam>
		/// <param name="source">An instance of <see cref="IPatternsAppliersHolder"/>>.</param>
		/// <param name="applier">The instance of the applier to add.</param>
		/// <returns>The <paramref name="source"/> instance (to chain 'merge')</returns>
		/// <remarks>
		/// The new applier is added only where does not exists an applier of the same <see cref="Type"/>.
		/// </remarks>
		public static IPatternsAppliersHolder Merge<TSubject, TApplyTo>(this IPatternsAppliersHolder source, IPatternApplier<TSubject, TApplyTo> applier)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (applier == null)
			{
				return source;
			}
			var patternsAppliersCollection = GetCollectionPropertyOf<TSubject, TApplyTo>(source);
			if (patternsAppliersCollection != null)
			{
				PerformMerge(patternsAppliersCollection, applier);
			}
			else
			{
				throw new ArgumentOutOfRangeException("applier",
				                                      string.Format(NotSupportedApplierExceptionMessageTemplate,
				                                                    typeof (TSubject).FullName, typeof (TApplyTo).FullName));
			}
			return source;
		}

		private static void PerformMerge<TSubject, TApplyTo>(ICollection<IPatternApplier<TSubject, TApplyTo>> destination, IPatternApplier<TSubject, TApplyTo> applier)
		{
			var applierType = applier.GetType();
			if (IsDelegatedApplier(applierType) || !destination.Any(a => a.GetType() == applierType))
			{
				destination.Add(applier);
			}
		}

		private static bool IsDelegatedApplier(Type applierType)
		{
			return applierType.IsGenericType
			       &&
			       (applierType.GetGenericTypeDefinition() == typeof (DelegatedApplier<,>)
			        || applierType.GetGenericTypeDefinition() == typeof (DelegatedAdvancedApplier<,>));
		}

		private static ICollection<IPatternApplier<TSubject, TApplyTo>> GetCollectionPropertyOf<TSubject, TApplyTo>(IPatternsAppliersHolder source)
		{
			var property = typeof(IPatternsAppliersHolder).GetFirstPropertyOfType(typeof(ICollection<IPatternApplier<TSubject, TApplyTo>>));
			if (property != null)
			{
				return (ICollection<IPatternApplier<TSubject, TApplyTo>>)(((PropertyInfo)property).GetValue(source, null));
			}
			return null;
		}

		/// <summary>
		/// Add or Replace the <paramref name="applier"/> to the correspondig collection inside the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="TSubject">The subject of the pattern.</typeparam>
		/// <typeparam name="TApplyTo">The target of the applier.</typeparam>
		/// <param name="source">An instance of <see cref="IPatternsAppliersHolder"/>>.</param>
		/// <param name="applier">The instance of the applier to add.</param>
		/// <returns>The <paramref name="source"/> instance (to chain 'union')</returns>
		/// <remarks>
		/// The Replace action is performed removing all appliers with the same type-name, where exists, and then adding the new applier.
		/// This method is usefull when you want override a behaviour of an existing applier in an existing <see cref="IPatternsAppliersHolder"/>.
		/// </remarks>
		public static IPatternsAppliersHolder UnionWith<TSubject, TApplyTo>(this IPatternsAppliersHolder source, IPatternApplier<TSubject, TApplyTo> applier)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (applier == null)
			{
				return source;
			}
			var patternsAppliersCollection = GetCollectionPropertyOf<TSubject, TApplyTo>(source);
			if (patternsAppliersCollection != null)
			{
				PerformUnionWith(patternsAppliersCollection, applier);
			}
			else
			{
				throw new ArgumentOutOfRangeException("applier",
				                                      string.Format(NotSupportedApplierExceptionMessageTemplate,
				                                                    typeof (TSubject).FullName, typeof (TApplyTo).FullName));
			}
			return source;
		}

		private static void PerformUnionWith<TSubject, TApplyTo>(ICollection<IPatternApplier<TSubject, TApplyTo>> destination, IPatternApplier<TSubject, TApplyTo> applier)
		{
			var applierType = applier.GetType();
			var existingAppliers = destination.Where(a => a.GetType().Name == applierType.Name && !IsDelegatedApplier(applierType)).ToList();
			if (existingAppliers.Count > 0)
			{
				foreach (var existingApplier in existingAppliers)
				{
					destination.Remove(existingApplier);
				}
			}
			destination.Add(applier);
		}

		public static void ApplyAllMatchs<TSubject, TApplyTo>(this IEnumerable<IPatternApplier<TSubject, TApplyTo>> appliers, TSubject subject, TApplyTo applyTo)
		{
			foreach (var patternApplier in appliers.Where(pa => pa.Match(subject)))
			{
				patternApplier.Apply(subject, applyTo);
			}
		}

		/// <summary>
		/// Merge tow instances of <see cref="IPatternsAppliersHolder"/>.
		/// </summary>
		/// <param name="first">The main <see cref="IPatternsAppliersHolder"/> to merge.</param>
		/// <param name="second">The second <see cref="IPatternsAppliersHolder"/> to merge.</param>
		/// <returns>A new instance of <see cref="IPatternsAppliersHolder"/> with the result of merge.</returns>
		/// <remarks>
		/// The rules of this methods are the same of <see cref="Merge{TSubject, TApplyTo}"/>.
		/// The result does not contains duplicated appliers.
		/// When an applier of the same <see cref="Type"/> exists in both side the instance contained in <paramref name="first"/> will be returned.
		/// </remarks>
		public static IPatternsAppliersHolder Merge(this IPatternsAppliersHolder first, IPatternsAppliersHolder second)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			var result = new EmptyPatternsAppliersHolder();
			MergePatternsAppliersHolders(first, result);
			if (second != null)
			{
				MergePatternsAppliersHolders(second, result);
			}
			return result;
		}

		private static void MergePatternsAppliersHolders(IPatternsAppliersHolder source, IPatternsAppliersHolder destination)
		{
			MergeAppliersCollection(source.RootClass, destination.RootClass);
			MergeAppliersCollection(source.JoinedSubclass, destination.JoinedSubclass);
			MergeAppliersCollection(source.Subclass, destination.Subclass);
			MergeAppliersCollection(source.UnionSubclass, destination.UnionSubclass);
			MergeAppliersCollection(source.Component, destination.Component);
			MergeAppliersCollection(source.ComponentProperty, destination.ComponentProperty);
			MergeAppliersCollection(source.ComponentPropertyPath, destination.ComponentPropertyPath);

			MergeAppliersCollection(source.Poid, destination.Poid);
			MergeAppliersCollection(source.Version, destination.Version);

			MergeAppliersCollection(source.Property, destination.Property);
			MergeAppliersCollection(source.PropertyPath, destination.PropertyPath);

			MergeAppliersCollection(source.ManyToOne, destination.ManyToOne);
			MergeAppliersCollection(source.ManyToOnePath, destination.ManyToOnePath);

			MergeAppliersCollection(source.OneToOne, destination.OneToOne);
			MergeAppliersCollection(source.OneToOnePath, destination.OneToOnePath);

			MergeAppliersCollection(source.Any, destination.Any);
			MergeAppliersCollection(source.AnyPath, destination.AnyPath);

			MergeAppliersCollection(source.Collection, destination.Collection);
			MergeAppliersCollection(source.CollectionPath, destination.CollectionPath);
			MergeAppliersCollection(source.Bag, destination.Bag);
			MergeAppliersCollection(source.BagPath, destination.BagPath);
			MergeAppliersCollection(source.List, destination.List);
			MergeAppliersCollection(source.ListPath, destination.ListPath);
			MergeAppliersCollection(source.Set, destination.Set);
			MergeAppliersCollection(source.SetPath, destination.SetPath);
			MergeAppliersCollection(source.Map, destination.Map);
			MergeAppliersCollection(source.MapPath, destination.MapPath);

			MergeAppliersCollection(source.ComponentParent, destination.ComponentParent);

			MergeAppliersCollection(source.ManyToMany, destination.ManyToMany);
			MergeAppliersCollection(source.ManyToManyPath, destination.ManyToManyPath);

			MergeAppliersCollection(source.Element, destination.Element);
			MergeAppliersCollection(source.ElementPath, destination.ElementPath);

			MergeAppliersCollection(source.OneToMany, destination.OneToMany);
			MergeAppliersCollection(source.OneToManyPath, destination.OneToManyPath);

			MergeAppliersCollection(source.MapKeyManyToMany, destination.MapKeyManyToMany);
			MergeAppliersCollection(source.MapKeyManyToManyPath, destination.MapKeyManyToManyPath);

			MergeAppliersCollection(source.MapKey, destination.MapKey);
			MergeAppliersCollection(source.MapKeyPath, destination.MapKeyPath);
		}

		private static void MergeAppliersCollection<TSubject, TApplyTo>(
			IEnumerable<IPatternApplier<TSubject, TApplyTo>> source, ICollection<IPatternApplier<TSubject, TApplyTo>> destination)
		{
			foreach (var patternApplier in source)
			{
				PerformMerge(destination, patternApplier);
			}
		}

		/// <summary>
		/// Union tow instances of <see cref="IPatternsAppliersHolder"/>.
		/// </summary>
		/// <param name="first">The main <see cref="IPatternsAppliersHolder"/> to union.</param>
		/// <param name="second">The second <see cref="IPatternsAppliersHolder"/> to union.</param>
		/// <returns>A new instance of <see cref="IPatternsAppliersHolder"/> with the result of union.</returns>
		/// <remarks>
		/// The rules of this methods are the same of <see cref="UnionWith{TSubject, TApplyTo}"/>.
		/// The result does not contains duplicated appliers.
		/// When an applier with the same type-name exists in both side the instance contained in <paramref name="second"/> will be returned.
		/// </remarks>
		public static IPatternsAppliersHolder UnionWith(this IPatternsAppliersHolder first, IPatternsAppliersHolder second)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			var result = new EmptyPatternsAppliersHolder();
			UnionWithPatternsAppliersHolders(first, result);
			if (second != null)
			{
				UnionWithPatternsAppliersHolders(second, result);
			}
			return result;
		}

		private static void UnionWithPatternsAppliersHolders(IPatternsAppliersHolder source, EmptyPatternsAppliersHolder destination)
		{
			UnionWithAppliersCollection(source.RootClass, destination.RootClass);
			UnionWithAppliersCollection(source.JoinedSubclass, destination.JoinedSubclass);
			UnionWithAppliersCollection(source.Subclass, destination.Subclass);
			UnionWithAppliersCollection(source.UnionSubclass, destination.UnionSubclass);
			UnionWithAppliersCollection(source.Component, destination.Component);
			UnionWithAppliersCollection(source.ComponentProperty, destination.ComponentProperty);
			UnionWithAppliersCollection(source.ComponentPropertyPath, destination.ComponentPropertyPath);

			UnionWithAppliersCollection(source.Poid, destination.Poid);
			UnionWithAppliersCollection(source.Version, destination.Version);

			UnionWithAppliersCollection(source.Property, destination.Property);
			UnionWithAppliersCollection(source.PropertyPath, destination.PropertyPath);

			UnionWithAppliersCollection(source.ManyToOne, destination.ManyToOne);
			UnionWithAppliersCollection(source.ManyToOnePath, destination.ManyToOnePath);

			UnionWithAppliersCollection(source.OneToOne, destination.OneToOne);
			UnionWithAppliersCollection(source.OneToOnePath, destination.OneToOnePath);

			UnionWithAppliersCollection(source.Any, destination.Any);
			UnionWithAppliersCollection(source.AnyPath, destination.AnyPath);

			UnionWithAppliersCollection(source.Collection, destination.Collection);
			UnionWithAppliersCollection(source.CollectionPath, destination.CollectionPath);
			UnionWithAppliersCollection(source.Bag, destination.Bag);
			UnionWithAppliersCollection(source.BagPath, destination.BagPath);
			UnionWithAppliersCollection(source.List, destination.List);
			UnionWithAppliersCollection(source.ListPath, destination.ListPath);
			UnionWithAppliersCollection(source.Set, destination.Set);
			UnionWithAppliersCollection(source.SetPath, destination.SetPath);
			UnionWithAppliersCollection(source.Map, destination.Map);
			UnionWithAppliersCollection(source.MapPath, destination.MapPath);

			UnionWithAppliersCollection(source.ComponentParent, destination.ComponentParent);

			UnionWithAppliersCollection(source.ManyToMany, destination.ManyToMany);
			UnionWithAppliersCollection(source.ManyToManyPath, destination.ManyToManyPath);

			UnionWithAppliersCollection(source.Element, destination.Element);
			UnionWithAppliersCollection(source.ElementPath, destination.ElementPath);

			UnionWithAppliersCollection(source.OneToMany, destination.OneToMany);
			UnionWithAppliersCollection(source.OneToManyPath, destination.OneToManyPath);

			UnionWithAppliersCollection(source.MapKeyManyToMany, destination.MapKeyManyToMany);
			UnionWithAppliersCollection(source.MapKeyManyToManyPath, destination.MapKeyManyToManyPath);

			UnionWithAppliersCollection(source.MapKey, destination.MapKey);
			UnionWithAppliersCollection(source.MapKeyPath, destination.MapKeyPath);
		}

		private static void UnionWithAppliersCollection<TSubject, TApplyTo>(
			IEnumerable<IPatternApplier<TSubject, TApplyTo>> source, ICollection<IPatternApplier<TSubject, TApplyTo>> destination)
		{
			foreach (var patternApplier in source)
			{
				PerformUnionWith(destination, patternApplier);
			}
		}
	}
}