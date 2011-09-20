namespace Luna.Common
{
    public interface IVisibleLinerData
    {
        string Color { get; set; }
        double MaxValue { get; set; }
        double[] Values { get; set; }
        string Text { get; set; }

        string Format { get; set; }
        int Category { get; set; }
        object Source { get; set; }

        double ZeroBaseValue { get; set; }
    }
}