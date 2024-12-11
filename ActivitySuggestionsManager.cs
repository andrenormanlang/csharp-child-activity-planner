using System;
using System.Collections.Generic;
using System.Linq;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Generates activity suggestions based on completed activities and recommendations.
    /// </summary>
    public class ActivitySuggestionsManager
    {
        private readonly Child _child;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivitySuggestionsManager"/> class.
        /// </summary>
        /// <param name="child">The child whose activity suggestions are generated.</param>
        public ActivitySuggestionsManager(Child child)
        {
            _child = child;
        }

        /// <summary>
        /// Gets activity suggestions to balance the child's development.
        /// </summary>
        /// <returns>A list of activity suggestions.</returns>
        public List<string> GetSuggestions()
        {
            var completedToday = _child.Activities
                .Where(a => a.ScheduledDate.Date == DateTime.Today)
                .GroupBy(a => a.Type)
                .ToDictionary(g => g.Key, g => g.Sum(a => a.Duration.TotalMinutes));

            var suggestions = new List<string>();

            foreach (var activity in ActivityRecommendations.RecommendedActivities)
            {
                var type = activity.Key;
                var (daily, _, _) = activity.Value;

                completedToday.TryGetValue(type, out var todayMinutes);
                if (todayMinutes < daily)
                {
                    suggestions.Add($"Add more {type} activities today (Need {daily - todayMinutes} more minutes).");
                }
            }

            return suggestions.Any() ? suggestions : new List<string> { "All activity types are well-balanced!" };
        }
    }
}
