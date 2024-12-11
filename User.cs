using System.Collections.Generic;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Represents a user of the YoungChild Activity Planner application.
    /// Manages a collection of children associated with the user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the list of children associated with the user.
        /// </summary>
        public List<Child> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            Children = new List<Child>();
        }

        /// <summary>
        /// Adds a new child to the user's collection of children".
        /// </summary>
        /// <param name="child">The <see cref="Child"/> instance to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/> is null.</exception>
        public void AddChild(Child child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child), "Child cannot be null.");

            Children.Add(child);
        }
    }
}
