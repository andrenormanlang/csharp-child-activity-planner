using System;
using System.Collections.Generic;
using System.Linq;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Manages activities for child's in the activity planning system.
    /// Provides functionality for adding, updating, deleting, and retrieving activities.
    /// </summary>
    public class ActivityManager
    {
        /// <summary>
        /// The user containing all child and activity data.
        /// </summary>
        private readonly User _user;

        /// <summary>
        /// Initializes a new instance of the ActivityManager class.
        /// </summary>
        /// <param name="user">The user object containing child and activity data.</param>
        public ActivityManager(User user)
        {
            _user = user;
        }

        /// <summary>
        /// Adds a new activity to the specified child's schedule.
        /// </summary>
        /// <param name="childName">The name of the child to add the activity for.</param>
        /// <param name="title">The title of the activity.</param>
        /// <param name="description">The description of the activity.</param>
        /// <param name="scheduledDate">The date and time when the activity is scheduled.</param>
        /// <param name="type">The type of activity.</param>
        /// <param name="duration">The planned duration of the activity.</param>
        /// <returns>
        /// true if the activity was successfully added; 
        /// false if the child wasn't found or if an activity with the same title already exists.
        /// </returns>
        public bool AddActivityToChild(string childName, string title, string description, DateTime scheduledDate, ActivityType type, TimeSpan duration)
        {
            var child = _user.Children.FirstOrDefault(t => t.Name == childName);
            if (child == null)
                return false;

            // Check for duplicate titles or other validations as needed
            if (child.Activities.Any(a => a.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
                return false;

            Activity newActivity = new Activity
            {
                Title = title,
                Description = description,
                ScheduledDate = scheduledDate,
                Type = type,
                Duration = duration,
                IsCompleted = false
            };

            child.Activities.Add(newActivity);
            return true;
        }

        /// <summary>
        /// Updates an existing activity for a specified child.
        /// </summary>
        /// <param name="childName">The name of the child whose activity is being updated.</param>
        /// <param name="activityToUpdate">The activity object to be updated.</param>
        /// <param name="newTitle">The new title for the activity.</param>
        /// <param name="newDescription">The new description for the activity.</param>
        /// <param name="newScheduledDate">The new scheduled date and time for the activity.</param>
        /// <param name="newType">The new activity type.</param>
        /// <param name="newDuration">The new duration for the activity.</param>
        /// <returns>
        /// true if the activity was successfully updated;
        /// false if the child wasn't found or if another activity already has the new title.
        /// </returns>
        public bool UpdateActivity(string childName, Activity activityToUpdate, string newTitle, string newDescription, DateTime newScheduledDate, ActivityType newType, TimeSpan newDuration)
        {
            var child = _user.Children.FirstOrDefault(t => t.Name == childName);
            if (child == null)
                return false;

            // Check for duplicate titles if the title has changed
            if (!activityToUpdate.Title.Equals(newTitle, StringComparison.OrdinalIgnoreCase) &&
                child.Activities.Any(a => a.Title.Equals(newTitle, StringComparison.OrdinalIgnoreCase)))
                return false;

            activityToUpdate.Title = newTitle;
            activityToUpdate.Description = newDescription;
            activityToUpdate.ScheduledDate = newScheduledDate;
            activityToUpdate.Type = newType;
            activityToUpdate.Duration = newDuration;
            return true;
        }

        /// <summary>
        /// Deletes an activity from a specified child's schedule.
        /// </summary>
        /// <param name="childName">The name of the child whose activity is being deleted.</param>
        /// <param name="activity">The activity to delete.</param>
        /// <returns>
        /// true if the activity was successfully deleted;
        /// false if the child wasn't found or the activity couldn't be removed.
        /// </returns>
        public bool DeleteActivity(string childName, Activity activity)
        {
            var child = _user.Children.FirstOrDefault(t => t.Name == childName);
            if (child == null) return false;
            return child.Activities.Remove(activity);
        }

        /// <summary>
        /// Retrieves all activities for a specified child, ordered by scheduled date.
        /// </summary>
        /// <param name="childName">The name of the child whose activities to retrieve.</param>
        /// <returns>
        /// A list of activities ordered by scheduled date if the child is found;
        /// an empty list if the child is not found.
        /// </returns>
        public List<Activity> GetActivities(string childName)
        {
            var child = _user.Children.FirstOrDefault(t => t.Name == childName);
            return child?.Activities.OrderBy(a => a.ScheduledDate).ToList() ?? new List<Activity>();
        }
    }
}