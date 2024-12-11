using System;
using System.Linq;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Manages operations related to child's within a user's profile, including adding, editing, and deleting children.
    /// </summary>
    public class ChildManager
    {
        /// <summary>
        /// The user whose children are being managed.
        /// </summary>
        private readonly User _user;

        /// <summary>
        /// Initializes a new instance of the <see cref="YoungChild"/> class.
        /// </summary>
        /// <param name="user">The <see cref="User"/> whose children will be managed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="user"/> is null.</exception>
        public ChildManager(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        /// <summary>
        /// Adds a new child to the user's profile if the child's age is between 3 and 6 years.
        /// </summary>
        /// <param name="name">The name of the child to add.</param>
        /// <param name="dob">The date of birth of the child.</param>
        /// <returns>
        /// <c>true</c> if the child was successfully added; otherwise, <c>false</c> if the age constraints are not met.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
        public bool AddChild(string name, DateTime dob)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Child name cannot be null or empty.", nameof(name));

            int age = CalculateAge(dob);
            if (age < 3 || age > 6)
                return false;

            var newChild = new Child
            {
                Name = name,
                DateOfBirth = dob
            };
            _user.AddChild(newChild);
            return true;
        }

        /// <summary>
        /// Edits the details of an existing child identified by their current name.
        /// </summary>
        /// <param name="oldName">The current name of the child to edit.</param>
        /// <param name="newName">The new name to assign to the child.</param>
        /// <param name="newDob">The new date of birth to assign to the child.</param>
        /// <returns>
        /// <c>true</c> if the child was found and updated successfully; otherwise, <c>false</c> if the child was not found.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="oldName"/> or <paramref name="newName"/> is null or empty.</exception>
        public bool EditChild(string oldName, string newName, DateTime newDob)
        {
            if (string.IsNullOrWhiteSpace(oldName))
                throw new ArgumentException("Old name cannot be null or empty.", nameof(oldName));

            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("New name cannot be null or empty.", nameof(newName));

            var child = _user.Children.FirstOrDefault(t => t.Name.Equals(oldName, StringComparison.OrdinalIgnoreCase));
            if (child == null)
                return false;

            child.Name = newName;
            child.DateOfBirth = newDob;
            return true;
        }

        /// <summary>
        /// Deletes a child'' and all associated activities from the user's profile based on the child'''s name.
        /// </summary>
        /// <param name="name">The name of the child'' to delete.</param>
        /// <returns>
        /// <c>true</c> if one or more child''s were successfully deleted; otherwise, <c>false</c> if no matching child''s were found.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
        public bool DeleteChild(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("YoungChild name cannot be null or empty.", nameof(name));

            int removed = _user.Children.RemoveAll(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return removed > 0;
        }

        /// <summary>
        /// Calculates the age of a child'' in years based on their date of birth.
        /// </summary>
        /// <param name="dob">The date of birth of the child''.</param>
        /// <returns>The age in complete years.</returns>
        public static int CalculateAge(DateTime dob)
        {
            var today = DateTime.Today;
            int age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
