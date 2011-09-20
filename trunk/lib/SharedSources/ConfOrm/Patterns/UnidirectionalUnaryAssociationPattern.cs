namespace ConfOrm.Patterns
{
	public class UnidirectionalUnaryAssociationPattern : BidirectionalUnaryAssociationPattern
	{
		public override bool Match(System.Reflection.MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			return !base.Match(subject);
		}
	}
}