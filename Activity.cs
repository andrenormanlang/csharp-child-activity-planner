using System;
using System.ComponentModel;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Represents an activity scheduled for a child, including its details and status.
    /// Implements <see cref="INotifyPropertyChanged"/> to notify clients of property value changes.
    /// </summary>
    public class Activity : INotifyPropertyChanged
    {
        /// <summary>
        /// Backing field for the <see cref="IsCompleted"/> property.
        /// </summary>
        private bool _isCompleted;

        /// <summary>
        /// Backing field for the <see cref="Title"/> property.
        /// </summary>
        private string _title = string.Empty;

        /// <summary>
        /// Backing field for the <see cref="Description"/> property.
        /// </summary>
        private string _description = string.Empty;

        /// <summary>
        /// Backing field for the <see cref="ScheduledDate"/> property.
        /// </summary>
        private DateTime _scheduledDate;

        /// <summary>
        /// Backing field for the <see cref="Type"/> property.
        /// </summary>
        private ActivityType _type;

        /// <summary>
        /// Backing field for the <see cref="Duration"/> property.
        /// </summary>
        private TimeSpan _duration;

        /// <summary>
        /// Gets or sets the title of the activity.
        /// </summary>
        /// <remarks>
        /// The title should be a concise description of the activity.
        /// </remarks>
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value ?? string.Empty;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        /// <summary>
        /// Gets or sets the description of the activity.
        /// </summary>
        /// <remarks>
        /// The description can provide additional details about the activity.
        /// </remarks>
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value ?? string.Empty;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        /// <summary>
        /// Gets or sets the scheduled date and time for the activity.
        /// </summary>
        /// <remarks>
        /// The <see cref="ScheduledDate"/> should be set to a future date and time when the activity is intended to occur.
        /// </remarks>
        public DateTime ScheduledDate
        {
            get => _scheduledDate;
            set
            {
                if (_scheduledDate != value)
                {
                    _scheduledDate = value;
                    OnPropertyChanged(nameof(ScheduledDate));
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the activity.
        /// </summary>
        /// <remarks>
        /// The <see cref="Type"/> categorizes the activity, aiding in organization and filtering.
        /// </remarks>
        public ActivityType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the activity has been completed.
        /// </summary>
        /// <remarks>
        /// Setting this property to <c>true</c> marks the activity as completed.
        /// </remarks>
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                }
            }
        }

        /// <summary>
        /// Gets or sets the duration of the activity.
        /// </summary>
        /// <remarks>
        /// The <see cref="Duration"/> specifies how long the activity is expected to last.
        /// </remarks>
        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event to notify listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// </summary>
        public Activity()
        {
            // Default constructor
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class with specified details.
        /// </summary>
        /// <param name="title">The title of the activity.</param>
        /// <param name="description">The description of the activity.</param>
        /// <param name="scheduledDate">The scheduled date and time for the activity.</param>
        /// <param name="type">The type/category of the activity.</param>
        /// <param name="duration">The duration of the activity.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="title"/> or <paramref name="type"/> is invalid.</exception>
        public Activity(string title, string description, DateTime scheduledDate, ActivityType type, TimeSpan duration)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            Title = title;
            Description = description ?? string.Empty;
            ScheduledDate = scheduledDate;
            Type = type;
            Duration = duration;
            IsCompleted = false;
        }

        /// <summary>
        /// Toggles the completion status of the activity.
        /// </summary>
        public void ToggleCompletion()
        {
            IsCompleted = !IsCompleted;
        }

        /// <summary>
        /// Provides a string representation of the activity, including its title and scheduled date.
        /// </summary>
        /// <returns>A string that represents the current activity.</returns>
        public override string ToString()
        {
            return $"{Title} scheduled on {ScheduledDate:g} ({Type})";
        }
    }
}

