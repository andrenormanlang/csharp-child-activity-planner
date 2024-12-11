using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Analyzes child activity progress and provides chart data and comparisons.
    /// </summary>
    public class ActivityProgressAnalyzer
    {
        private readonly Child _child;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityProgressAnalyzer"/> class.
        /// </summary>
        /// <param name="child">The child whose activity data is analyzed.</param>
        public ActivityProgressAnalyzer(Child child)
        {
            _child = child;
        }

        /// <summary>
        /// Analyzes the activity progress and returns data for charts.
        /// </summary>
        /// <returns>Progress data for the dashboard charts.</returns>
        public ProgressData AnalyzeActivityProgress()
        {
            var groupedActivities = _child.Activities
                .GroupBy(a => a.ScheduledDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => new DailyActivitySummary
                {
                    Date = g.Key,
                    Completed = g.Count(a => a.IsCompleted),
                    Incomplete = g.Count(a => !a.IsCompleted),
                    TotalDuration = g.Sum(a => a.Duration.TotalMinutes),
                    CompletedTitles = string.Join("\n", g.Where(a => a.IsCompleted).Select(a => a.Title)),
                    IncompleteTitles = string.Join("\n", g.Where(a => !a.IsCompleted).Select(a => a.Title))
                })
                .ToList();

            var dates = groupedActivities.Select(g => g.Date.ToString("MM/dd")).ToArray();
            var completedCounts = groupedActivities.Select(g => (double)g.Completed).ToArray();
            var incompleteCounts = groupedActivities.Select(g => (double)g.Incomplete).ToArray();
            var totalDurations = groupedActivities.Select(g => g.TotalDuration).ToArray();

            // Create chart series for progress and durations
            return new ProgressData
            {
                ProgressChartSeries = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Completed",
                        Values = new ChartValues<double>(completedCounts),
                        Fill = System.Windows.Media.Brushes.Green,
                        DataLabels = true,
                        LabelPoint = point => groupedActivities[(int)point.X].CompletedTitles
                    },
                    new ColumnSeries
                    {
                        Title = "Incomplete",
                        Values = new ChartValues<double>(incompleteCounts),
                        Fill = System.Windows.Media.Brushes.Red,
                        DataLabels = true,
                        LabelPoint = point => groupedActivities[(int)point.X].IncompleteTitles
                    }
                },
                DurationChartSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Total Duration (Minutes)",
                        Values = new ChartValues<double>(totalDurations),
                        PointGeometry = DefaultGeometries.Circle,
                        DataLabels = true,
                        LabelPoint = point => $"{totalDurations[(int)point.X]:N0} minutes"
                    }
                },
                AxisX = new Axis { Title = "Date", Labels = dates, LabelsRotation = 45 },
                AxisY = new Axis { Title = "Count", MinValue = 0 },
                DurationAxisX = new Axis { Title = "Date", Labels = dates, LabelsRotation = 15 },
                DurationAxisY = new Axis { Title = "Minutes", MinValue = 0 }
            };
        }

        /// <summary>
        /// Retrieves the titles of completed activities.
        /// </summary>
        /// <returns>A list of completed activity titles.</returns>
        public List<string> GetCompletedActivityTitles()
        {
            return _child.Activities
                .Where(a => a.IsCompleted)
                .Select(a => a.Title)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Compares completed activities with recommended values.
        /// </summary>
        /// <returns>A list of comparison data objects.</returns>
        public List<ComparisonData> GetComparisonData()
        {
            var completedPerType = _child.Activities
                .Where(a => a.IsCompleted)
                .GroupBy(a => a.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            return ActivityRecommendations.RecommendedActivities
                .Select(kvp => new ComparisonData
                {
                    ActivityType = kvp.Key,
                    Completed = completedPerType.TryGetValue(kvp.Key, out var completed) ? completed : 0,
                    Recommended = kvp.Value.Weekly
                })
                .ToList();
        }
    }

    /// <summary>
    /// Represents data for the dashboard progress and duration charts.
    /// </summary>
    public class ProgressData
    {
        public SeriesCollection ProgressChartSeries { get; set; } = new SeriesCollection();
        public SeriesCollection DurationChartSeries { get; set; } = new SeriesCollection();
        public Axis AxisX { get; set; } = new Axis();
        public Axis AxisY { get; set; } = new Axis();
        public Axis DurationAxisX { get; set; } = new Axis();
        public Axis DurationAxisY { get; set; } = new Axis();
    }
}
