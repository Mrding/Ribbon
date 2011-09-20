using System.Reflection;

namespace ConfOrm.Patterns
{
	public class NativePoidPattern : PoidIntPattern, IPatternValueGetter<MemberInfo, IPersistentIdStrategy>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Get(MemberInfo element)
		{
			return new NativeIdStrategy();
		}

		#endregion

		private class NativeIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.Native; }
			}

			public object Params
			{
				get { return null; }
			}

			#endregion
		}
	}
}