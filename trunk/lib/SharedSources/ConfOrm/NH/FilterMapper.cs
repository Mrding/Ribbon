using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class FilterMapper : IFilterMapper
	{
		private readonly HbmFilter filter;

		public FilterMapper(string filterName, HbmFilter filter)
		{
			if (filterName == null)
			{
				throw new ArgumentNullException("filterName");
			}
			if (string.Empty.Equals(filterName.Trim()))
			{
				throw new ArgumentOutOfRangeException("filterName","Invalid filter-name: the name should contain no blank characters.");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			this.filter = filter;
			this.filter.name = filterName;
		}

		#region Implementation of IFilterMapper

		public void Condition(string sqlCondition)
		{
			if (sqlCondition == null || string.Empty.Equals(sqlCondition) || string.Empty.Equals(sqlCondition.Trim()))
			{
				filter.condition = null;
				filter.Text = null;
				return;
			}
			var conditionLines = sqlCondition.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			if(conditionLines.Length > 1)
			{
				filter.Text = conditionLines;
				filter.condition = null;
			}
			else
			{
				filter.condition = sqlCondition;
				filter.Text = null;				
			}
		}

		#endregion
	}
}