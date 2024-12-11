using System;
using System.Collections.Generic;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Represents a child with personal details and a list of scheduled activities.
    /// </summary>
    public class Child
    {
        /// <summary>
        /// Gets or sets the name of the young child.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the child.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the list of activities scheduled for the child.
        /// </summary>
        public List<Activity> Activities { get; set; } = new List<Activity>();

        /// <summary>
        /// Initializes a new instance of the <see cref="YoungChild"/> class with default values.
        /// </summary>
        public Child()
        {
            // Default constructor
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YoungChild"/> class with specified name and date of birth.
        /// </summary>
        /// <param name="name">The name of the child.</param>
        /// <param name="dateOfBirth">The date of birth of the child.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
        public Child(string name, DateTime dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            Name = name;
            DateOfBirth = dateOfBirth;
            Activities = new List<Activity>();
        }

        /// <summary>
        /// Adds a new activity to the child's activity list.
        /// </summary>
        /// <param name="activity">The <see cref="Activity"/> to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="activity"/> is null.</exception>
        public void AddActivity(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");

            Activities.Add(activity);
        }

        /// <summary>
        /// Calculates the age of the child in years and months.
        /// </summary>
        /// <returns>
        /// A tuple containing two integers:
        /// <list type="bullet">
        /// <item><description><c>Years</c> - The number of complete years.</description></item>
        /// <item><description><c>Months</c> - The number of additional months after the last complete year.</description></item>
        /// </list>
        /// </returns>
        public (int Years, int Months) GetAgeInYearsAndMonths()
        {
            var today = DateTime.Today;

            // Calculate years
            int years = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-years)) years--;

            // Calculate months
            int months = today.Month - DateOfBirth.Month;
            if (months < 0)
            {
                months += 12;
                years--;
            }

            return (years, months);
        }

        /// <summary>
        /// Removes an activity from the child's activity list.
        /// </summary>
        /// <param name="activity">The <see cref="Activity"/> to remove.</param>
        /// <returns>
        /// <c>true</c> if the activity was successfully removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="activity"/> is null.</exception>
        public bool RemoveActivity(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");

            return Activities.Remove(activity);
        }

        /// <summary>
        /// Clears all activities from the child's activity list.
        /// </summary>
        public void ClearActivities()
        {
            Activities.Clear();
        }

        /// <summary>
        /// Determines whether a specific activity exists in the child's activity list.
        /// </summary>
        /// <param name="activity">The <see cref="Activity"/> to locate.</param>
        /// <returns>
        /// <c>true</c> if the activity exists in the list; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="activity"/> is null.</exception>
        public bool ContainsActivity(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");

            return Activities.Contains(activity);
        }

        /// <summary>
        /// Sorts the child's activities based on their scheduled date and time in ascending order.
        /// </summary>
        public void SortActivitiesByDate()
        {
            Activities = Activities.OrderBy(a => a.ScheduledDate).ToList();
        }

        /// <summary>
        /// Provides a string representation of the child, including their name and age.
        /// </summary>
        /// <returns>A string that represents the current child.</returns>
        public override string ToString()
        {
            var age = GetAgeInYearsAndMonths();
            return $"{Name}, Age: {age.Years} years and {age.Months} months";
        }
    }
}

