using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Represents a daily schedule containing activities for a specific date.
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// Gets or sets the date for which the schedule is created.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the list of activities scheduled for the day.
        /// </summary>
        public List<Activity> DailyActivities { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// Sets the schedule date to the current date and initializes the activities list.
        /// </summary>
        public Schedule()
        {
            DailyActivities = new List<Activity>();
            Date = DateTime.Today;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class with a specified date.
        /// </summary>
        /// <param name="date">The date for which the schedule is created.</param>
        public Schedule(DateTime date)
        {
            DailyActivities = new List<Activity>();
            Date = date.Date;
        }

        /// <summary>
        /// Adds a new activity to the daily schedule.
        /// </summary>
        /// <param name="activity">The <see cref="Activity"/> to add to the schedule.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="activity"/> is null.</exception>
        public void AddActivity(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");

            DailyActivities.Add(activity);
        }

        /// <summary>
        /// Retrieves all activities scheduled for the day.
        /// </summary>
        /// <returns>A list of <see cref="Activity"/> instances scheduled for the day.</returns>
        public List<Activity> GetSchedule()
        {
            return new List<Activity>(DailyActivities);
        }

        /// <summary>
        /// Removes an activity from the daily schedule.
        /// </summary>
        /// <param name="activity">The <see cref="Activity"/> to remove from the schedule.</param>
        /// <returns>True if the activity was successfully removed; otherwise, false.</returns>
        public bool RemoveActivity(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");

            return DailyActivities.Remove(activity);
        }

        /// <summary>
        /// Clears all activities from the daily schedule.
        /// </summary>
        public void ClearSchedule()
        {
            DailyActivities.Clear();
        }

        /// <summary>
        /// Determines whether a specific activity exists in the schedule.
        /// </summary>
        /// <param name="activity">The <see cref="Activity"/> to locate in the schedule.</param>
        /// <returns>True if the activity exists in the schedule; otherwise, false.</returns>
        public bool ContainsActivity(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");

            return DailyActivities.Contains(activity);
        }

        /// <summary>
        /// Sorts the activities in the schedule based on their scheduled date and time.
        /// </summary>
        public void SortActivities()
        {
            DailyActivities = DailyActivities.OrderBy(a => a.ScheduledDate).ToList();
        }

        /// <summary>
        /// Provides a string representation of the schedule, including the date and number of activities.
        /// </summary>
        /// <returns>A string that represents the current schedule.</returns>
        public override string ToString()
        {
            return $"Schedule for {Date.ToShortDateString()}: {DailyActivities.Count} activities.";
        }
    }
}
