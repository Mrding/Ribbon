using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class HighLowPoidPattern : PoidIntPattern, IPatternValueGetter<MemberInfo, IPersistentIdStrategy>
	{
		private readonly Func<MemberInfo, object> parametersGetter;
		private readonly object parameters;
		public HighLowPoidPattern() {}
		public HighLowPoidPattern(object parameters)
		{
			this.parameters = parameters;
		}

		public HighLowPoidPattern(Func<MemberInfo,object> parametersGetter)
		{
			if (parametersGetter == null)
			{
				throw new ArgumentNullException("parametersGetter");
			}
			this.parametersGetter = parametersGetter;
		}

		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public virtual IPersistentIdStrategy Get(MemberInfo element)
		{
			if (parametersGetter != null)
			{
				return new HighLowIdStrategy { Params = parametersGetter(element) };
			}
			return new HighLowIdStrategy {Params = parameters};
		}

		#endregion

		private class HighLowIdStrategy: IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.HighLow; }
			}

			public object Params{get; set;}

			#endregion
		}
	}
}