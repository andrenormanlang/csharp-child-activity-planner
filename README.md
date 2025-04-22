# 🌟 C# Child Activity Planner 🧸

The **C# Child Activity Planner** is an intuitive application designed to help parents or guardians plan, schedule, and manage activities for children aged 3 to 6 years. With features for tracking progress, scheduling, and generating reports, this tool is perfect for organized caregiving. 🎉

---

## 📋 Features

### 🧒 Child Management
- Add, edit, or delete children profiles with ease
- Validate child age (3-6 years)

### 📅 Activity Planner
- Schedule activities with specific date, time, and duration
- Categorize activities into types (e.g., educational, physical)
- Mark activities as completed

### 📊 Dashboard & Reporting
- Generate **Progress Dashboards** for individual children
- Compare completed activities with recommendations
- Visualize total activity duration over time

### 💾 File Management
- Save and load user data, including children and their activities, in a structured file format
- Supports "Save As" functionality for flexible file management

---

## 🚀 Getting Started

### Prerequisites
- **.NET Core SDK** (version 6.0 or higher) ⚙️
- An IDE like **Visual Studio** or **Rider** 🖥️
- A Windows machine for running WPF applications 🪟

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/young-child-activity-planner.git
   ```
2. Open the solution file (YoungChildActivityPlanner.sln) in your IDE
3. Restore NuGet packages and build the solution

## 🖥️ Usage

### Add a Child 👶
1. Enter the child's name and date of birth in the Add Child section
2. Click the "Add Child" button to save

### Schedule an Activity 📅
1. Select a child from the child dropdown
2. Enter the title, description, date, time, and duration of the activity
3. Choose the activity type and click "Schedule Activity"

### Manage Activities 🛠️
1. View activities in the Activities Table
2. Use the Update and Delete buttons to modify or remove activities

### View Progress 📊
1. Open the Progress Dashboard to:
   - View activity completion charts
   - See suggested activities
   - Compare completed and recommended activities
2. Use the Print Report button for printable summaries

## 🛠️ File Management

### Save and Load Data
- Save your current schedule using the File > Save menu
- Load an existing schedule using File > Open File

### Export Data
- Use File > Save As to save the current data to a new file

## 🔧 Technical Details

### File Structure
- MainWindow.xaml: The main application interface
- ChildManager.cs: Handles CRUD operations for children
- ActivityManager.cs: Manages activities (add, edit, delete, update)
- FileManager.cs: Handles file operations (save/load)
- ProgressDashboard.xaml: Displays child progress using visual charts

### Key Components
- WPF (Windows Presentation Foundation) for the user interface
- LiveCharts for visualizing progress and activity trends
- .NET Core for backend logic and data management

### Data Format
Data is saved in a structured .txt file format:

```
USER_START
CHILD_START
Name: <Child Name>
DateOfBirth: <YYYY-MM-DD>
ACTIVITIES_COUNT: <Number>
ACTIVITY_START
Title: <Activity Title>
Description: <Activity Description>
ScheduledDate: <YYYY-MM-DD HH:mm>
Type: <Activity Type>
Duration: <HH:mm>
IsCompleted: <true/false>
ACTIVITY_END
CHILD_END
USER_END
```

## 🎨 User Interface

### Main Features
- Add Child Section: Input fields for adding new children
- Schedule Activity Section: Fields to add and categorize activities
- View Activities Section: Data grid displaying activities for the selected child
- Action Buttons: Options to update, delete, or print activities

### Dashboard
- Progress Dashboard: View activity completion trends, suggested activities, and a comparison with recommended goals

## 🖋️ License

This project is licensed under the MIT License. See the LICENSE file for details.

## 👨‍💻 Contributors

Your Name: [GitHub]

Feel free to fork the project, open issues, or submit pull requests to contribute! 🙌


## 🌈 Happy Planning! 🎉
