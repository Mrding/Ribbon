using System.Reflection;

namespace ConfOrm.Patterns
{
	public class IdentityPoidPattern : PoidIntPattern, IPatternValueGetter<MemberInfo, IPersistentIdStrategy>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Get(MemberInfo element)
		{
			return new IdentityIdStrategy();
		}

		#endregion

		private class IdentityIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.Identity; }
			}

			public object Params
			{
				get { return null; }
			}

			#endregion
		}
	}
}