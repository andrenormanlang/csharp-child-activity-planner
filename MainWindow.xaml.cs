using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using System.ComponentModel; // Added for CancelEventArgs

namespace YoungChildActivityPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// The main window of the YoungChild Activity Planner application.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Represents the current user using the application.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// Manages child-related operations.
        /// </summary>
        private ChildManager childManager;

        /// <summary>
        /// Manages activity-related operations.
        /// </summary>
        private ActivityManager activityManager;

        /// <summary>
        /// Timer to update the current time display every second.
        /// </summary>
        private DispatcherTimer? clockTimer;

        /// <summary>
        /// The default file path for user data.
        /// </summary>
        private const string DefaultDataFile = "userData.txt";

        /// <summary>
        /// Keeps track of the currently selected activity for updating or deleting.
        /// </summary>
        private Activity? selectedActivity = null;

        /// <summary>
        /// The file path of the currently loaded or saved user data.
        /// </summary>
        private string currentFilePath = FileManager.DefaultFilePath;

        /// <summary>
        /// Indicates whether the user data has been modified since the last save.
        /// </summary>
        private bool isDataModified = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Sets up the initial state of the application.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Load user data or create a new user if none exists
            currentUser = FileManager.LoadUserFromFile(currentFilePath) ?? new User();
            childManager = new ChildManager(currentUser);
            activityManager = new ActivityManager(currentUser);

            InitializeUI();
            LoadActivityTypes();
            InitializeTimeSelectors();
            StartClockTimer();

            // Mark data as modified if there are no child's
            if (!currentUser.Children.Any())
            {
                isDataModified = true;
            }

            // Save initial data
            FileManager.SaveUserToFile(currentUser, currentFilePath);
        }

        /// <summary>
        /// Overrides the OnClosing method to handle unsaved changes.
        /// Prompts the user to save changes before exiting.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (isDataModified)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before exiting?",
                                             "Unsaved Changes",
                                             MessageBoxButton.YesNoCancel,
                                             MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    bool saveResult = FileManager.SaveUserToFile(currentUser, currentFilePath);
                    if (!saveResult)
                    {
                        MessageBox.Show("Failed to save the schedule.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true; // Prevent closing if save fails
                    }
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true; // Prevent closing
                }
                // If No, proceed with closing without saving
            }
            base.OnClosing(e);
        }

        /// <summary>
        /// Initializes the user interface components.
        /// Sets up date pickers and loads childs.
        /// </summary>
        private void InitializeUI()
        {
            ChildDOBPicker.DisplayDateEnd = DateTime.Today.AddYears(-3);
            ChildDOBPicker.DisplayDateStart = DateTime.Today.AddYears(-6);

            LoadChildren();
            PrintButton.IsEnabled = false;
        }

        /// <summary>
        /// Loads activity types into the ActivityTypeComboBox.
        /// </summary>
        private void LoadActivityTypes()
        {
            ActivityTypeComboBox.ItemsSource = Enum.GetValues(typeof(ActivityType)).Cast<ActivityType>();
            ActivityTypeComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Initializes the time selection combo boxes for activity scheduling and duration.
        /// </summary>
        private void InitializeTimeSelectors()
        {
            // Initialize hours 0-23 for Activity Time
            for (int i = 0; i < 24; i++)
            {
                ActivityHourComboBox.Items.Add(i.ToString("D2"));
            }
            ActivityHourComboBox.SelectedIndex = 0;

            // Initialize minutes 0-59 for Activity Time
            for (int i = 0; i < 60; i++)
            {
                ActivityMinuteComboBox.Items.Add(i.ToString("D2"));
            }
            ActivityMinuteComboBox.SelectedIndex = 0;

            // Initialize hours 0-23 for Activity Duration
            for (int i = 0; i < 24; i++)
            {
                ActivityDurationHourComboBox.Items.Add(i.ToString("D2"));
            }
            ActivityDurationHourComboBox.SelectedIndex = 0;

            // Initialize minutes 0-59 for Activity Duration
            for (int i = 0; i < 60; i++)
            {
                ActivityDurationMinuteComboBox.Items.Add(i.ToString("D2"));
            }
            ActivityDurationMinuteComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Starts the clock timer to update the current time display every second.
        /// </summary>
        private void StartClockTimer()
        {
            clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            clockTimer.Tick += (s, e) =>
            {
                CurrentTimeTextBlock.Text = DateTime.Now.ToString("F");
            };
            clockTimer.Start();
        }

        /// <summary>
        /// Loads the list of children into the relevant combo boxes.
        /// </summary>
        private void LoadChildren()
        {
            var childrenNames = currentUser.Children.Select(t => t.Name).ToList();
            ChildComboBox.ItemsSource = childrenNames;
            ViewChildComboBox.ItemsSource = childrenNames;
            ManageChildComboBox.ItemsSource = childrenNames;
        }

        /// <summary>
        /// Handles the Add Child button click event.
        /// Adds a new child to the user's list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void AddChild_Click(object sender, RoutedEventArgs e)
        {
            string? name = ChildNameTextBox.Text?.Trim();
            DateTime? dob = ChildDOBPicker.SelectedDate;

            if (string.IsNullOrEmpty(name))
            {
                ShowError("Please enter the child's name.");
                return;
            }

            if (!dob.HasValue)
            {
                ShowError("Please select the child's date of birth.");
                return;
            }

            if (!childManager.AddChild(name, dob.Value))
            {
                ShowError("Child must be between 3 and 6 years old.");
                return;
            }

            FileManager.SaveUserToFile(currentUser, currentFilePath);
            isDataModified = true;
            ShowSuccess("Child added successfully!");
            ClearChildInputs();
            LoadChildren();
        }

        /// <summary>
        /// Handles the Edit Child button click event.
        /// Edits the selected chuld's information.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void EditChild_Click(object sender, RoutedEventArgs e)
        {
            string? selectedChildName = ManageChildComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedChildName))
            {
                ShowError("Please select a child to edit.");
                return;
            }

            string newName = EditChildNameTextBox.Text.Trim();
            DateTime? newDOB = EditChildDOBPicker.SelectedDate;

            if (string.IsNullOrEmpty(newName) || !newDOB.HasValue)
            {
                ShowError("Please provide both a new name and DOB.");
                return;
            }

            if (childManager.EditChild(selectedChildName, newName, newDOB.Value))
            {
                FileManager.SaveUserToFile(currentUser, currentFilePath);
                isDataModified = true;
                ShowSuccess("Child updated successfully.");
                LoadChildren();
            }
            else
            {
                ShowError("Failed to update child.");
            }
        }

        /// <summary>
        /// Handles the Delete YoungChild button click event.
        /// Deletes the selected child and all their activities.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void DeleteChild_Click(object sender, RoutedEventArgs e)
        {
            string? selectedChildName = ManageChildComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedChildName))
            {
                ShowError("Please select a child to delete.");
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {selectedChildName} and all their activities?",
                                         "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (childManager.DeleteChild(selectedChildName))
                {
                    FileManager.SaveUserToFile(currentUser, currentFilePath);
                    isDataModified = true;
                    ShowSuccess("Child deleted successfully.");
                    LoadChildren();
                }
                else
                {
                    ShowError("Failed to delete child.");
                }
            }
        }

        /// <summary>
        /// Handles the Schedule Activity button click event.
        /// Schedules a new activity for the selected child.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ScheduleActivity_Click(object sender, RoutedEventArgs e)
        {
            string? selectedChildName = ChildComboBox.SelectedItem as string;
            string? title = ActivityTitleTextBox.Text?.Trim();
            string? description = ActivityDescriptionTextBox.Text?.Trim();
            DateTime? scheduledDate = ActivityDatePicker.SelectedDate;
            ActivityType? selectedType = ActivityTypeComboBox.SelectedItem as ActivityType?;

            if (string.IsNullOrEmpty(selectedChildName) || string.IsNullOrEmpty(title) || !scheduledDate.HasValue || !selectedType.HasValue)
            {
                ShowError("Please enter all required fields for the activity.");
                return;
            }

            // Get time selection
            if (ActivityHourComboBox.SelectedItem == null || ActivityMinuteComboBox.SelectedItem == null)
            {
                ShowError("Please select a valid time.");
                return;
            }

            // Get duration selection
            if (ActivityDurationHourComboBox.SelectedItem == null || ActivityDurationMinuteComboBox.SelectedItem == null)
            {
                ShowError("Please select a valid duration.");
                return;
            }

            // Using null-forgiving operator since SelectedItem is guaranteed to be non-null
            int selectedHour = int.Parse(ActivityHourComboBox.SelectedItem!.ToString()!);
            int selectedMinute = int.Parse(ActivityMinuteComboBox.SelectedItem!.ToString()!);

            DateTime finalDateTime = scheduledDate.Value.Date.AddHours(selectedHour).AddMinutes(selectedMinute);

            // Get duration
            int durationHours = int.Parse(ActivityDurationHourComboBox.SelectedItem!.ToString()!);
            int durationMinutes = int.Parse(ActivityDurationMinuteComboBox.SelectedItem!.ToString()!);
            TimeSpan duration = new TimeSpan(durationHours, durationMinutes, 0);

            if (activityManager.AddActivityToChild(selectedChildName, title, description ?? string.Empty, finalDateTime, selectedType.Value, duration))
            {
                FileManager.SaveUserToFile(currentUser, currentFilePath);
                isDataModified = true;
                ShowSuccess("Activity scheduled successfully!");
                ClearActivityInputs();
                RefreshActivitiesGrid();
            }
            else
            {
                ShowError("Failed to schedule activity.");
            }
        }

        /// <summary>
        /// Handles the Update Activity button click event.
        /// Updates the selected activity with new details.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void UpdateActivity_Click(object sender, RoutedEventArgs e)
        {
            if (selectedActivity != null)
            {
                string? selectedChildName = ViewChildComboBox.SelectedItem as string;
                if (string.IsNullOrEmpty(selectedChildName))
                {
                    ShowError("Please select a child first.");
                    return;
                }

                string? newTitle = ActivityTitleTextBox.Text?.Trim();
                string? newDescription = ActivityDescriptionTextBox.Text?.Trim();
                DateTime? newDate = ActivityDatePicker.SelectedDate;

                if (string.IsNullOrEmpty(newTitle) || !newDate.HasValue)
                {
                    ShowError("Please enter a valid title and date for the activity.");
                    return;
                }

                // Update date with selected time
                if (ActivityHourComboBox.SelectedItem == null || ActivityMinuteComboBox.SelectedItem == null)
                {
                    ShowError("Please select a valid time.");
                    return;
                }

                // Get time selection
                int selectedHour = int.Parse(ActivityHourComboBox.SelectedItem!.ToString()!);
                int selectedMinute = int.Parse(ActivityMinuteComboBox.SelectedItem!.ToString()!);
                DateTime updatedDateTime = newDate.Value.Date.AddHours(selectedHour).AddMinutes(selectedMinute);

                // Get duration selection
                if (ActivityDurationHourComboBox.SelectedItem == null || ActivityDurationMinuteComboBox.SelectedItem == null)
                {
                    ShowError("Please select a valid duration.");
                    return;
                }

                int durationHours = int.Parse(ActivityDurationHourComboBox.SelectedItem!.ToString()!);
                int durationMinutes = int.Parse(ActivityDurationMinuteComboBox.SelectedItem!.ToString()!);
                TimeSpan updatedDuration = new TimeSpan(durationHours, durationMinutes, 0);

                ActivityType newType = (ActivityType?)ActivityTypeComboBox.SelectedItem ?? selectedActivity.Type;

                if (activityManager.UpdateActivity(selectedChildName, selectedActivity, newTitle, newDescription ?? string.Empty, updatedDateTime, newType, updatedDuration))
                {
                    FileManager.SaveUserToFile(currentUser, currentFilePath);
                    isDataModified = true;
                    ShowSuccess("Activity updated successfully.");
                    RefreshActivitiesGrid();
                    ClearActivityInputs();
                    selectedActivity = null;
                    ActivitiesDataGrid.SelectedItem = null;
                    UpdateActivityButton.IsEnabled = false;
                    DeleteActivityButton.IsEnabled = false;
                    ScheduleActivityButton.IsEnabled = true; // Re-enable Schedule Activity button

                    // Hide the Cancel button if it was visible
                    CancelUpdateButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ShowError("Failed to update activity.");
                }
            }
            else
            {
                ShowError("No activity selected for updating.");
            }
        }

        /// <summary>
        /// Handles the Delete Activity button click event.
        /// Deletes the selected activity.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void DeleteActivity_Click(object sender, RoutedEventArgs e)
        {
            if (selectedActivity != null)
            {
                string? selectedChildName = ViewChildComboBox.SelectedItem as string;
                if (string.IsNullOrEmpty(selectedChildName))
                {
                    ShowError("Please select a child first.");
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete the activity '{selectedActivity.Title}'?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (activityManager.DeleteActivity(selectedChildName, selectedActivity))
                    {
                        FileManager.SaveUserToFile(currentUser, currentFilePath);
                        isDataModified = true;
                        ShowSuccess("Activity deleted successfully.");
                        RefreshActivitiesGrid();
                        ClearActivityInputs();
                        selectedActivity = null;
                        ActivitiesDataGrid.SelectedItem = null;
                        UpdateActivityButton.IsEnabled = false;
                        DeleteActivityButton.IsEnabled = false;
                        ScheduleActivityButton.IsEnabled = true; // Re-enable Schedule Activity button

                        // Hide the Cancel button if it was visible
                        CancelUpdateButton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ShowError("Failed to delete activity.");
                    }
                }
            }
            else
            {
                ShowError("No activity selected for deletion.");
            }
        }

        /// <summary>
        /// Handles the selection change event for the ViewChildComboBox.
        /// Refreshes the activities grid based on the selected child.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ViewChildComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshActivitiesGrid();
        }

        /// <summary>
        /// Handles the selection change event for the ActivitiesDataGrid.
        /// Populates the activity details for editing or deleting.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ActivitiesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActivitiesDataGrid.SelectedItem is Activity activity)
            {
                selectedActivity = activity;

                // Populate the Schedule Activity fields with selected activity's data
                ActivityTitleTextBox.Text = activity.Title;
                ActivityDescriptionTextBox.Text = activity.Description;
                ActivityDatePicker.SelectedDate = activity.ScheduledDate.Date;
                ActivityHourComboBox.SelectedItem = activity.ScheduledDate.Hour.ToString("D2");
                ActivityMinuteComboBox.SelectedItem = activity.ScheduledDate.Minute.ToString("D2");
                ActivityTypeComboBox.SelectedItem = activity.Type;

                // Populate Duration fields
                ActivityDurationHourComboBox.SelectedItem = activity.Duration.Hours.ToString("D2");
                ActivityDurationMinuteComboBox.SelectedItem = activity.Duration.Minutes.ToString("D2");

                // Enable Update and Delete buttons
                UpdateActivityButton.IsEnabled = true;
                DeleteActivityButton.IsEnabled = true;

                // Keep Schedule Activity button enabled
                ScheduleActivityButton.IsEnabled = true;

                // Show the Cancel button
                CancelUpdateButton.Visibility = Visibility.Visible;
            }
            else
            {
                // No activity selected
                selectedActivity = null;
                ClearActivityInputs();
                UpdateActivityButton.IsEnabled = false;
                DeleteActivityButton.IsEnabled = false;

                // Schedule Activity button remains enabled
                ScheduleActivityButton.IsEnabled = true;

                // Hide the Cancel button
                CancelUpdateButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles the CellEditEnding event for the ActivitiesDataGrid.
        /// Updates the completion status of an activity when the checkbox is toggled.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ActivitiesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Check if the edited column is "Completed" and the edit is committed
            if (e.Column.Header.ToString() == "Completed" &&
                e.EditAction == DataGridEditAction.Commit &&
                e.Row.Item is Activity activity &&
                e.EditingElement is CheckBox checkBox)
            {
                // Update the IsCompleted property based on the CheckBox state
                activity.IsCompleted = checkBox.IsChecked == true;
                isDataModified = true;

                // Save the updated user data to the file
                FileManager.SaveUserToFile(currentUser, currentFilePath);

                // Retrieve the selected child's name from the ViewChildComboBox
                var selectedChildName = ViewChildComboBox.SelectedItem as string;
                if (!string.IsNullOrEmpty(selectedChildName))
                {
                    // Find the child object based on the selected name
                    var child = currentUser.Children.FirstOrDefault(t => t.Name == selectedChildName);
                    // Additional logic can be added here if needed
                }
            }
        }

        /// <summary>
        /// Refreshes the activities data grid based on the selected child.
        /// </summary>
        private void RefreshActivitiesGrid()
        {
            string? selectedChildName = ViewChildComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedChildName))
            {
                var activities = activityManager.GetActivities(selectedChildName);
                ActivitiesDataGrid.ItemsSource = null; // Reset the ItemsSource
                ActivitiesDataGrid.ItemsSource = activities;
                PrintButton.IsEnabled = activities.Any();
            }
            else
            {
                ActivitiesDataGrid.ItemsSource = null;
                PrintButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Clears the input fields related to adding a child.
        /// </summary>
        private void ClearChildInputs()
        {
            ChildNameTextBox.Text = string.Empty;
            ChildDOBPicker.SelectedDate = null;
        }

        /// <summary>
        /// Clears the input fields related to scheduling an activity.
        /// </summary>
        private void ClearActivityInputs()
        {
            ActivityTitleTextBox.Text = string.Empty;
            ActivityDescriptionTextBox.Text = string.Empty;
            ActivityDatePicker.SelectedDate = null;
            ActivityHourComboBox.SelectedIndex = 0;
            ActivityMinuteComboBox.SelectedIndex = 0;
            ActivityTypeComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Displays an error message to the user.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Displays a success message to the user.
        /// </summary>
        /// <param name="message">The success message to display.</param>
        private static void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #region File Menu Event Handlers

        /// <summary>
        /// Handles the File->New menu item click event.
        /// Creates a new user data set, clearing existing data.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MenuFileNew_Click(object sender, RoutedEventArgs e)
        {
            currentUser = new User();
            childManager = new ChildManager(currentUser);
            activityManager = new ActivityManager(currentUser);

            FileManager.SaveUserToFile(currentUser, DefaultDataFile);
            LoadChildren();
            ActivitiesDataGrid.ItemsSource = null;
            isDataModified = true; // Mark as modified since data has changed
            ShowSuccess("New data set created. All previous data cleared.");
        }

        /// <summary>
        /// Handles the File->Open menu item click event.
        /// Opens an existing user data file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Open Schedule File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                User? loadedUser = FileManager.LoadUserFromFile(openFileDialog.FileName);
                if (loadedUser != null)
                {
                    currentUser = loadedUser;
                    childManager = new ChildManager(currentUser);
                    activityManager = new ActivityManager(currentUser);
                    currentFilePath = openFileDialog.FileName; // Update current file path
                    isDataModified = false;
                    LoadChildren();
                    RefreshActivitiesGrid();
                    ShowSuccess("Schedule loaded successfully!");
                }
                else
                {
                    ShowError("Failed to load the schedule. The file may be corrupted or in an incorrect format.");
                }
            }
        }

        /// <summary>
        /// Handles the New button click event.
        /// Prompts the user for confirmation before creating a new schedule.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to create a new schedule? All unsaved changes will be lost.",
                                         "Confirm New", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                MenuFileNew_Click(sender, e); // Reuse existing New functionality
            }
        }

        /// <summary>
        /// Handles the File->Save menu item click event.
        /// Saves the current user data to the existing file path.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MenuFileSave_Click(object sender, RoutedEventArgs e)
        {
            bool saveResult = FileManager.SaveUserToFile(currentUser, currentFilePath);
            if (saveResult)
            {
                isDataModified = false;
                ShowSuccess("Schedule saved successfully!");
            }
            else
            {
                ShowError("Failed to save the schedule.");
            }
        }

        /// <summary>
        /// Handles the File->Save As menu item click event.
        /// Saves the current user data to a new file path specified by the user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MenuFileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Save Schedule As"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                bool saveResult = FileManager.SaveUserToFile(currentUser, saveFileDialog.FileName);
                if (saveResult)
                {
                    currentFilePath = saveFileDialog.FileName; // Update current file path
                    isDataModified = false;
                    ShowSuccess("Schedule saved successfully!");
                }
                else
                {
                    ShowError("Failed to save the schedule.");
                }
            }
        }

        /// <summary>
        /// Handles the File->Exit menu item click event.
        /// Prompts the user for confirmation before exiting the application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit",
                                         MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Close();
            }
        }

        #endregion

        #region Help Menu Event Handlers

        /// <summary>
        /// Handles the Help->About menu item click event.
        /// Opens the About window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MenuHelpAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow
            {
                Owner = this
            };
            aboutWindow.ShowDialog();
        }

        #endregion

        /// <summary>
        /// Handles the Print button click event.
        /// Prints the activities of the selected child.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            string? selectedChildName = ViewChildComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedChildName))
            {
                ShowError("Please select a child to print activities.");
                return;
            }

            var activities = activityManager.GetActivities(selectedChildName);
            if (activities.Count == 0)
            {
                ShowError("No activities to print.");
                return;
            }

            var doc = new FlowDocument
            {
                FontFamily = new FontFamily("Arial"),
                FontSize = 12,
                PageWidth = 800
            };

            Paragraph title = new Paragraph(new Run($"Activity Schedule for {selectedChildName}"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(title);

            Table table = new Table();
            doc.Blocks.Add(table);

            table.Columns.Add(new TableColumn() { Width = new GridLength(150) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(200) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(300) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(100) });

            TableRowGroup headerGroup = new TableRowGroup();
            table.RowGroups.Add(headerGroup);

            TableRow headerRow = new TableRow();
            headerGroup.Rows.Add(headerRow);
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Date & Time"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Title"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Description"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Type"))) { FontWeight = FontWeights.Bold });

            TableRowGroup dataGroup = new TableRowGroup();
            table.RowGroups.Add(dataGroup);

            foreach (var activity in activities.OrderBy(a => a.ScheduledDate))
            {
                TableRow row = new TableRow();
                dataGroup.Rows.Add(row);
                row.Cells.Add(new TableCell(new Paragraph(new Run(activity.ScheduledDate.ToString("g")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(activity.Title))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(activity.Description))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(activity.Type.ToString()))));
            }

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "YoungChild Activities");
            }
        }

        /// <summary>
        /// Handles the Cancel Update button click event.
        /// Cancels the current activity update operation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CancelUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Clear selection
            ActivitiesDataGrid.SelectedItem = null;
            ClearActivityInputs();
            UpdateActivityButton.IsEnabled = false;
            DeleteActivityButton.IsEnabled = false;
            ScheduleActivityButton.IsEnabled = true;

            // Hide the Cancel button
            CancelUpdateButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the Progress Dashboard button click event.
        /// Opens the development report for the selected child.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ProgressDashboard_Click(object sender, RoutedEventArgs e)
        {
            string? selectedChildName = ViewChildComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedChildName))
            {
                ShowError("Please select a child to generate the development report.");
                return;
            }

            var child = currentUser.Children.FirstOrDefault(t => t.Name == selectedChildName);
            if (child == null)
            {
                ShowError("Selected child not found.");
                return;
            }

            // Initialize ActivityManager if not already
            if (activityManager == null)
            {
                activityManager = new ActivityManager(currentUser);
            }

            // Create and show the DevelopmentReportWindow
            ProgressDashboard reportWindow = new ProgressDashboard(child, activityManager)
            {
                Owner = this
            };
            reportWindow.ShowDialog();
        }
    }
}
