namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Represents comparison data between completed and recommended activities.
    /// </summary>
    public class ComparisonData
    {
        public ActivityType ActivityType { get; set; }
        public int Completed { get; set; }
        public int Recommended { get; set; }
    }
}
