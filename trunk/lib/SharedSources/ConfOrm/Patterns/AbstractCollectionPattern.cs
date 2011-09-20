using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public abstract class AbstractCollectionPattern : IPattern<MemberInfo>
	{
		public bool Match(MemberInfo subject)
		{
			if (MemberMatch(subject)) return true;
			var pi = subject as PropertyInfo;
			if (pi != null)
			{
				var fieldPattern = PropertyToFieldPatterns.Defaults.FirstOrDefault(pp => pp.Match(pi));
				if (fieldPattern != null)
				{
					var fieldInfo = fieldPattern.GetBackFieldInfo(pi);
					return MemberMatch(fieldInfo);
				}
			}
			return false;
		}

		protected abstract bool MemberMatch(MemberInfo subject);
	}

}