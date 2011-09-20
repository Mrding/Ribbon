using System.Reflection;

namespace ConfOrm.Patterns
{
	public class GuidPoidPattern: PoidGuidPattern, IPatternValueGetter<MemberInfo, IPersistentIdStrategy>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Get(MemberInfo element)
		{
			return new GuidIdStrategy();
		}

		#endregion

		private class GuidIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.Guid; }
			}

			public object Params
			{
				get { return null; }
			}

			#endregion
		}
	}
}