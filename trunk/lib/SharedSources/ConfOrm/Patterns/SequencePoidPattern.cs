using System.Reflection;

namespace ConfOrm.Patterns
{
	public class SequencePoidPattern: PoidIntPattern, IPatternValueGetter<MemberInfo, IPersistentIdStrategy>
	{
		private readonly object parameters;
		public SequencePoidPattern() {}
		public SequencePoidPattern(object parameters)
		{
			this.parameters = parameters;
		}

		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Get(MemberInfo element)
		{
			return new SequenceIdStrategy { Params = parameters };
		}

		#endregion

		private class SequenceIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.Sequence; }
			}

			public object Params{get; set;}

			#endregion
		}
	}
}