using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Represents a daily summary of activities.
    /// </summary>
    public class DailyActivitySummary
    {
        public DateTime Date { get; set; }
        public int Completed { get; set; }
        public int Incomplete { get; set; }
        public double TotalDuration { get; set; }
        public string CompletedTitles { get; set; } = string.Empty; 
        public string IncompleteTitles { get; set; } = string.Empty;
    }
}
