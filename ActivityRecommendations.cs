using System.Collections.Generic;
using System.Collections.Immutable;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Provides recommended activity durations for children.
    /// </summary>
    public static class ActivityRecommendations
    {
        /// <summary>
        /// Recommended activity durations by type.
        /// Daily, Weekly, and Monthly durations in minutes.
        /// </summary>
        public static readonly IReadOnlyDictionary<ActivityType, (int Daily, int Weekly, int Monthly)> RecommendedActivities =
            new Dictionary<ActivityType, (int, int, int)>
            {
                { ActivityType.Physical, (60, 300, 1200) },
                { ActivityType.Educational, (30, 180, 720) },
                { ActivityType.Social, (20, 120, 480) },
                { ActivityType.Creative, (15, 75, 300) },
                { ActivityType.Recreational, (30, 150, 600) }
            }.ToImmutableDictionary();
    }
}
