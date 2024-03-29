using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ManyToManyMapper : IManyToManyMapper
	{
		private readonly Type elementType;
		private readonly HbmManyToMany manyToMany;
		private readonly HbmMapping mapDoc;

		public ManyToManyMapper(Type elementType, HbmManyToMany manyToMany, HbmMapping mapDoc)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (manyToMany == null)
			{
				throw new ArgumentNullException("manyToMany");
			}
			this.elementType = elementType;
			this.manyToMany = manyToMany;
			this.mapDoc = mapDoc;
		}

		#region Implementation of IColumnsMapper

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (manyToMany.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through single-column API.");
			}
			manyToMany.formula = null;
			HbmColumn hbm = manyToMany.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      	{
			      		name = manyToMany.column,
			      		unique = manyToMany.unique,
			      		uniqueSpecified = manyToMany.unique,
			      	};
			string defaultColumnName = elementType.Name;
			columnMapper(new ColumnMapper(hbm, defaultColumnName));
			if (ColumnTagIsRequired(hbm))
			{
				manyToMany.Items = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				manyToMany.column = defaultColumnName == null || !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
				manyToMany.unique = hbm.unique;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetColumnPlainValues();
			int i = 1;
			var columns = new List<HbmColumn>(columnMapper.Length);
			foreach (var action in columnMapper)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = elementType.Name + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			manyToMany.Items = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.length != null || hbm.precision != null || hbm.scale != null || hbm.notnull || hbm.uniquekey != null
			       || hbm.sqltype != null || hbm.index != null || hbm.@default != null || hbm.check != null;
		}

		private void ResetColumnPlainValues()
		{
			manyToMany.column = null;
			manyToMany.unique = false;
			manyToMany.formula = null;
		}

		#endregion

		#region IManyToManyMapper Members

		public void Class(Type entityType)
		{
			if (!elementType.IsAssignableFrom(entityType))
			{
				throw new ArgumentOutOfRangeException("entityType",
				                                      string.Format("The type is incompatible; expected assignable to {0}",
				                                                    elementType));
			}
			manyToMany.@class = entityType.GetShortClassName(mapDoc);
		}

		public void EntityName(string entityName)
		{
			manyToMany.entityname = entityName;
		}

		public void NotFound(NotFoundMode mode)
		{
			if (mode == null)
			{
				return;
			}
			manyToMany.notfound = mode.ToHbm();
		}

		public void Formula(string formula)
		{
			if (formula == null)
			{
				return;
			}

			ResetColumnPlainValues();
			manyToMany.Items = null;
			var formulaLines = formula.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				manyToMany.Items = new[] { new HbmFormula { Text = formulaLines } };
			}
			else
			{
				manyToMany.formula = formula;
			}
		}

		public void Lazy(LazyRelation lazyRelation)
		{
			switch (lazyRelation.ToHbm())
			{
				case HbmLaziness.False:
					manyToMany.lazy = HbmRestrictedLaziness.False;
					manyToMany.lazySpecified = true;
					break;
				case HbmLaziness.Proxy:
					manyToMany.lazy = HbmRestrictedLaziness.Proxy;
					manyToMany.lazySpecified = true;
					break;
				case HbmLaziness.NoProxy:
					manyToMany.lazy = HbmRestrictedLaziness.False;
					manyToMany.lazySpecified = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion
	}
}