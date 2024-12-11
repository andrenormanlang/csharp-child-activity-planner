using LiveCharts;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Represents the main dashboard for tracking child activity progress.
    /// </summary>
    public partial class ProgressDashboard : Window
    {

        private readonly ActivitySuggestionsManager _suggestionsManager;
        private readonly ActivityProgressAnalyzer _progressAnalyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDashboard"/> class.
        /// </summary>
        /// <param name="child">The child whose activities are being tracked.</param>
        /// <param name="activityManager">Manages all activities for the child.</param>
        public ProgressDashboard(Child child, ActivityManager activityManager)
        {
            InitializeComponent();

            // Initialize helper classes
            _suggestionsManager = new ActivitySuggestionsManager(child);
            _progressAnalyzer = new ActivityProgressAnalyzer(child);

            // Get age in years and months
            var (years, months) = child.GetAgeInYearsAndMonths();

            // Display child info
            ChildInfoTextBlock.Text = $"Child: {child.Name}, Age: ({years} years and {months} months)";

            // Load data and populate UI
            LoadProgressData();
            GenerateActivitySuggestions();
            DisplayActivityTitles();
            CompareWithAverage();
        }


        /// <summary>
        /// Loads progress data and updates the charts.
        /// </summary>
        private void LoadProgressData()
        {
            var progressData = _progressAnalyzer.AnalyzeActivityProgress();

            // Bind progress data to the ProgressChart
            ProgressChart.Series = progressData.ProgressChartSeries;
            ProgressChart.AxisX.Add(progressData.AxisX);
            ProgressChart.AxisY.Add(progressData.AxisY);
            ProgressChart.LegendLocation = LegendLocation.Top;

            // Bind duration data to the DurationChart
            DurationChart.Series = progressData.DurationChartSeries;
            DurationChart.AxisX.Add(progressData.DurationAxisX);
            DurationChart.AxisY.Add(progressData.DurationAxisY);
            DurationChart.LegendLocation = LegendLocation.Top;
        }

        /// <summary>
        /// Generates activity suggestions and populates the suggestion list.
        /// </summary>
        private void GenerateActivitySuggestions()
        {
            var suggestions = _suggestionsManager.GetSuggestions();
            SuggestionsList.ItemsSource = suggestions;
        }

        /// <summary>
        /// Displays titles of completed activities in the UI.
        /// </summary>
        private void DisplayActivityTitles()
        {
            var titles = _progressAnalyzer.GetCompletedActivityTitles();
            CompletedActivitiesList.ItemsSource = titles.Any()
                ? titles
                : new List<string> { "No activities completed yet." };
        }

        /// <summary>
        /// Compares completed activities with recommended values and updates the comparison grid.
        /// </summary>
        private void CompareWithAverage()
        {
            var comparisonData = _progressAnalyzer.GetComparisonData();
            ComparisonDataGrid.ItemsSource = comparisonData;
        }

        /// <summary>
        /// Handles the click event for the "Print Report" button.
        /// Prepares a printable document of the child's activity progress and sends it to the printer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private void PrintReportButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a printable FlowDocument
            var doc = new FlowDocument
            {
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 12,
                PageWidth = 800
            };

            // Add a title for the report
            doc.Blocks.Add(new Paragraph(new Run($"Progress Report for {ChildInfoTextBlock.Text}"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

            // Add section headers and details
            doc.Blocks.Add(new Paragraph(new Run("Activity Completion Over Time:"))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 10)
            });

            doc.Blocks.Add(new Paragraph(new Run("Suggested Activities:"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 10)
            });

            foreach (var suggestion in SuggestionsList.Items)
            {
                doc.Blocks.Add(new Paragraph(new Run(suggestion.ToString()))
                {
                    Margin = new Thickness(10, 0, 0, 0)
                });
            }

            // Add completed activities section
            doc.Blocks.Add(new Paragraph(new Run("Completed Activities:"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 10)
            });

            foreach (var activity in CompletedActivitiesList.Items)
            {
                doc.Blocks.Add(new Paragraph(new Run(activity.ToString()))
                {
                    Margin = new Thickness(10, 0, 0, 0)
                });
            }

            // Open the Print Dialog
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "YoungChild Progress Report");
            }
        }


    }
}
