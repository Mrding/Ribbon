namespace Luna.Common
{
    public interface IVisibleTerm : ITerm
    {
        string Text { get; }

        string Remark { get; set; }
    }

    public interface IHierarchicalTerm : ITerm
    {
        int Level { get;  }
    }

    public interface IStyledTerm
    {
        string Background { get;  }
    }

   
}