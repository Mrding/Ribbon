using System;
using System.Reflection;

namespace ConfOrm
{
	public class RelationOn: Relation
	{
		private readonly int hashCode;

		public RelationOn(Type from, MemberInfo on, Type to, Declared declared)
			: base(from, to, declared)
		{
			if (on == null)
			{
				throw new ArgumentNullException("on");
			}
			On = on;
			hashCode = On.GetHashCode();
		}

		public RelationOn(Type from, MemberInfo on, Type to) : base(from, to)
		{
			if (on == null)
			{
				throw new ArgumentNullException("on");
			}
			On = on;
			hashCode = On.GetHashCode(); 
		}

		public MemberInfo On { get; set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as RelationOn);
		}

		public bool Equals(RelationOn that)
		{
			return that != null && On.Equals(that.On);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public override string ToString()
		{
			return string.Format("From {0} On {1} To {2}", From.FullName, On.Name, To.FullName);
		}
	}
}