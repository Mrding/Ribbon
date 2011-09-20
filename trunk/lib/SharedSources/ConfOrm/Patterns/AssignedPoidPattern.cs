using System.Reflection;

namespace ConfOrm.Patterns
{
	/// <summary>
	/// Match with any Property of any type.
	/// </summary>
	/// <remarks>
	/// To be used as base class or to be registered as first (last to match) Poid strategy.
	/// </remarks>
	public class AssignedPoidPattern : IPatternValueGetter<MemberInfo, IPersistentIdStrategy>
	{
		#region Implementation of IPattern<MemberInfo>

		public virtual bool Match(MemberInfo subject)
		{
			return true;
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Get(MemberInfo element)
		{
			return new AssignedIdStrategy();
		}

		#endregion

		private class AssignedIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.Assigned; }
			}

			public object Params { get { return null; } }


			#endregion
		}
	}
}