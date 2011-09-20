using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Patterns;

namespace ConfOrm
{
	public static class PatternExtensions
	{
		public static TResult GetValueOfFirstMatch<TSubject, TResult>(this IEnumerable<IPatternValueGetter<TSubject, TResult>> appliers, TSubject subject)
		{
			if (appliers == null)
			{
				return default(TResult);
			}
			var patternApplier = appliers.FirstGetterrOrDefault(subject);
			return patternApplier != null ? patternApplier.Get(subject) : default(TResult);
		}

		public static IPatternValueGetter<TSubject, TResult> FirstGetterrOrDefault<TSubject, TResult>(this IEnumerable<IPatternValueGetter<TSubject, TResult>> appliers, TSubject subject)
		{
			return appliers.Reverse().FirstOrDefault(a => a.Match(subject));
		}

		public static bool Match<T>(this IEnumerable<IPattern<T>> patterns, T subject)
		{
			if (patterns == null)
			{
				return false;
			}
			return patterns.Any(p => p.Match(subject));
		}

		public static void Add(this ICollection<IPattern<MemberInfo>> patterns, Predicate<MemberInfo> matcher)
		{
			if (patterns == null)
			{
				throw new ArgumentNullException("patterns");
			}
			patterns.Add(new DelegatedPattern<MemberInfo>(matcher));
		}
	}
}