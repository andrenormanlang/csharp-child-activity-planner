using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Handles file management for saving and loading user data.
    /// </summary>
    public static class FileManager
    {
        private const string UserStartToken = "USER_START";
        private const string UserEndToken = "USER_END";
        private const string ChildStartToken = "CHILD_START";
        private const string ChildEndToken = "CHILD_END";
        private const string ActivityStartToken = "ACTIVITY_START";
        private const string ActivityEndToken = "ACTIVITY_END";

        public const string DefaultFilePath = "userData.txt";

        /// <summary>
        /// Saves the user data, including child's and their activities, to a specified text file.
        /// </summary>
        /// <param name="user">The User object containing all data.</param>
        /// <param name="filePath">The path to the file where data will be saved.</param>
        /// <returns>True if saved successfully; otherwise, false.</returns>
        public static bool SaveUserToFile(User user, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(UserStartToken);

                    foreach (var child in user.Children)
                    {
                        writer.WriteLine(ChildStartToken);
                        writer.WriteLine($"Name:{child.Name}");
                        writer.WriteLine($"DateOfBirth:{child.DateOfBirth:yyyy-MM-dd}");
                        writer.WriteLine($"ACTIVITIES_COUNT:{child.Activities.Count}");

                        foreach (var activity in child.Activities)
                        {
                            writer.WriteLine(ActivityStartToken);
                            writer.WriteLine($"Title:{activity.Title}");
                            writer.WriteLine($"Description:{activity.Description}");
                            writer.WriteLine($"ScheduledDate:{activity.ScheduledDate:yyyy-MM-dd HH:mm}");
                            writer.WriteLine($"Type:{activity.Type}");
                            writer.WriteLine($"Duration:{activity.Duration}");
                            writer.WriteLine($"IsCompleted:{activity.IsCompleted}");
                            writer.WriteLine(ActivityEndToken);
                        }

                        writer.WriteLine(ChildEndToken);
                    }

                    writer.WriteLine(UserEndToken);
                }

                return true;
            }
            catch (Exception)
            {
                // Optionally, log the exception: ex.Message
                return false;
            }
        }

        /// <summary>
        /// Loads user data, including child's and their activities, from a specified text file.
        /// </summary>
        /// <param name="filePath">The path to the file from which data will be loaded.</param>
        /// <returns>The User object loaded from the file; otherwise, null if loading fails.</returns>
        public static User? LoadUserFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"Data file not found: {filePath}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                User user = new User();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string? line;
                    Child? currentChild= null;
                    Activity? currentActivity = null;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string trimmedLine = line.Trim();

                        if (trimmedLine == UserStartToken || trimmedLine == UserEndToken)
                            continue;

                        if (trimmedLine == ChildStartToken)
                        {
                            currentChild = new Child();
                            continue;
                        }

                        if (trimmedLine == ChildEndToken)
                        {
                            if (currentChild!= null)
                            {
                                user.Children.Add(currentChild);
                                currentChild = null;
                            }
                            continue;
                        }

                        if (trimmedLine == ActivityStartToken)
                        {
                            currentActivity = new Activity();
                            continue;
                        }

                        if (trimmedLine == ActivityEndToken)
                        {
                            if (currentActivity != null && currentChild != null)
                            {
                                currentChild.Activities.Add(currentActivity);
                                currentActivity = null;
                            }
                            continue;
                        }

                        // Parse key-value pairs
                        int separatorIndex = trimmedLine.IndexOf(':');
                        if (separatorIndex == -1)
                            continue; // Invalid line format, skip

                        string key = trimmedLine.Substring(0, separatorIndex).Trim();
                        string value = trimmedLine.Substring(separatorIndex + 1).Trim();

                        if (currentActivity != null)
                        {
                            // Parsing Activity properties
                            switch (key)
                            {
                                case "Title":
                                    currentActivity.Title = value;
                                    break;
                                case "Description":
                                    currentActivity.Description = value;
                                    break;
                                case "ScheduledDate":
                                    if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime scheduledDate))
                                        currentActivity.ScheduledDate = scheduledDate;
                                    else
                                        MessageBox.Show($"Invalid ScheduledDate format: {value}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
                                case "Type":
                                    if (Enum.TryParse<ActivityType>(value, out ActivityType type))
                                        currentActivity.Type = type;
                                    else
                                        MessageBox.Show($"Invalid ActivityType: {value}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
                                case "Duration":
                                    if (TimeSpan.TryParse(value, out TimeSpan duration))
                                        currentActivity.Duration = duration;
                                    else
                                        MessageBox.Show($"Invalid Duration format: {value}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
                                case "IsCompleted":
                                    if (bool.TryParse(value, out bool isCompleted))
                                        currentActivity.IsCompleted = isCompleted;
                                    else
                                        MessageBox.Show($"Invalid IsCompleted value: {value}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
                                default:
                                    // Unknown key within Activity, can log or ignore
                                    break;
                            }
                        }
                        else if (currentChild != null)
                        {
                            // Parsing YoungChild properties
                            switch (key)
                            {
                                case "Name":
                                    currentChild.Name = value;
                                    break;
                                case "DateOfBirth":
                                    if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dob))
                                        currentChild.DateOfBirth = dob;
                                    else
                                        MessageBox.Show($"Invalid DateOfBirth format: {value}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
                                case "ACTIVITIES_COUNT":
                                    // Optional: Validate activity count if needed in the future
                                    break;
                                default:
                                    break;
                            }
                        }
                        
                    }
                    return user;
                }
            }
            catch (Exception)
            {
                // Optionally, log the exception: ex.Message
                return null;
            }
        }

        /// <summary>
        /// Saves the user data to the default file.
        /// </summary>
        /// <param name="user">The User object containing all data.</param>
        /// <returns>True if saved successfully; otherwise, false.</returns>
        public static bool SaveUserToDefaultFile(User user)
        {
            return SaveUserToFile(user, DefaultFilePath);
        }

        /// <summary>
        /// Loads the user data from the default file.
        /// </summary>
        /// <returns>The User object loaded from the file; otherwise, null if loading fails.</returns>
        public static User? LoadUserFromDefaultFile()
        {
            return LoadUserFromFile(DefaultFilePath);
        }
    }
}
