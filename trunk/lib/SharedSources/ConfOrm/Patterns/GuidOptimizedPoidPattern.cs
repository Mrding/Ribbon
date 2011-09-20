using System.Reflection;

namespace ConfOrm.Patterns
{
	public class GuidOptimizedPoidPattern : PoidGuidPattern, IPatternValueGetter<MemberInfo, IPersistentIdStrategy>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Get(MemberInfo element)
		{
			return new GuidOptimizedIdStrategy();
		}

		#endregion

		private class GuidOptimizedIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.GuidOptimized; }
			}

			public object Params
			{
				get { return null; }
			}

			#endregion
		}
	}
}