using System;

namespace ConfOrm
{
	public enum Declared
	{
		Explicit,
		Implicit
	}
	public class Relation
	{
		private readonly int hashCode;

		public Relation(Type from, Type to)
			: this(from, to, Declared.Explicit) {}

		public Relation(Type @from, Type to, Declared declaredAs)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from");
			}
			if (to == null)
			{
				throw new ArgumentNullException("to");
			}
			From = from;
			To = to;
			DeclaredAs = declaredAs;
			hashCode = (37 * from.GetHashCode()) ^ to.GetHashCode();
		}

		public Type From { get; private set; }
		public Type To { get; private set; }
		public Declared DeclaredAs { get; private set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as Relation);
		}

		public bool Equals(Relation that)
		{
			if(ReferenceEquals(null, that))
			{
				return false;
			}

			return From.Equals(that.From) && To.Equals(that.To);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public override string ToString()
		{
			return string.Format("From {0} To {1}", From.FullName, To.FullName);
		}
	}
}