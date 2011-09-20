using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ComponentMapper : AbstractPropertyContainerMapper, IComponentMapper
	{
		private ComponentParentMapper parentMapper;
		private readonly HbmComponent component;
		private readonly IAccessorPropertyMapper accessorPropertyMapper;

		public ComponentMapper(HbmComponent component, Type componentType, MemberInfo declaringTypeMember, HbmMapping mapDoc)
			: base(componentType, mapDoc)
		{
			this.component = component;
			component.@class = componentType.GetShortClassName(mapDoc);
			accessorPropertyMapper = new AccessorPropertyMapper(declaringTypeMember.DeclaringType, declaringTypeMember.Name, x => component.access = x);
		}

		#region Overrides of AbstractPropertyContainerMapper

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] { property };
			component.Items = component.Items == null ? toAdd : component.Items.Concat(toAdd).ToArray();
		}

		#endregion

		#region Implementation of IComponentMapper

		public void Parent(MemberInfo parent)
		{
			Parent(parent, x=> { });
		}

		public void Parent(MemberInfo parent, Action<IComponentParentMapper> parentMapping)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			var mapper = GetParentMapper(parent);
			parentMapping(mapper);
		}

		public void Update(bool consideredInUpdateQuery)
		{
			component.update = consideredInUpdateQuery;
		}

		public void Insert(bool consideredInInsertQuery)
		{
			component.insert = consideredInInsertQuery;
		}

		public void Lazy(bool isLazy)
		{
			component.lazy = isLazy;
		}

		public void Class(Type componentType)
		{
			component.@class = componentType.GetShortClassName(mapDoc);
		}

		#endregion

		private IComponentParentMapper GetParentMapper(MemberInfo parent)
		{
			if (parentMapper != null)
			{
				return parentMapper;
			}
			component.parent = new HbmParent();
			return parentMapper = new ComponentParentMapper(component.parent, parent);
		}

		public void Access(Accessor accessor)
		{
			accessorPropertyMapper.Access(accessor);
		}

		public void Access(Type accessorType)
		{
			accessorPropertyMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			component.optimisticlock = takeInConsiderationForOptimisticLock;
		}
	}
}