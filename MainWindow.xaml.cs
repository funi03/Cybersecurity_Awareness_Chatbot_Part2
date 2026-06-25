using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Cybersecurity_Awareness_Chatbot_Part2
{//  start namespace

    
        public partial class MainWindow : Window
        {
            // ============ FIELDS ============
            private string username = string.Empty;

            // Part 2 Components
            private responds data;
            private helper clean;
            private user_name user;
            private sentiment_detect sentiment;
            private memory_recall memory;
            private chatbot bot;
            private ActivityLogger logger;

            // Part 3 Components - TASK MANAGER
            private TaskManager taskManager;  // <-- DECLARED HERE

            // Quiz State
            private List<Question_in_quiz> quizQuestions;
            private int currentQuizIndex = 0;
            private int quizScore = 0;
            private bool isQuizAnswered = false;
            private List<RadioButton> quizOptionButtons;

            // Task Display
            private ObservableCollection<TaskDisplayItem> taskItems;

            // Log Display
            private ObservableCollection<LogDisplayItem> logItems;

            // Counter for interests
            private int counting = 0;

            // ============ CONSTRUCTOR ============
            public MainWindow()
            {
                InitializeComponent();
                InitializeComponents();

                // Show home page
                home_grid.Visibility = Visibility.Visible;
                username_grid.Visibility = Visibility.Hidden;
                chat_grid.Visibility = Visibility.Hidden;
                tasks_grid.Visibility = Visibility.Hidden;
                quiz_grid.Visibility = Visibility.Hidden;
                log_grid.Visibility = Visibility.Hidden;

                // Play voice greeting
                PlayVoiceGreeting();
            }

            // ============ INITIALIZATION ============
            private void InitializeComponents()
            {
                // Initialize Part 2 components
                data = new responds();
                clean = new helper();
                user = new user_name();
                sentiment = new sentiment_detect();
                memory = new memory_recall();
                bot = new chatbot(data.replies, data.ignore);

                // Initialize logger (Part 3)
                logger = new ActivityLogger();
                logger.AddLog("Application started");

                // ================================================
                // TASK MANAGER INITIALIZATION - ADDED HERE
                // ================================================
                // Initialize task manager with logger
                taskManager = new TaskManager(logger);
                // ================================================

                // Initialize quiz manager (Part 3)
                // quizManager = new QuizManager(logger); // If you have this class

                // Initialize task display
                taskItems = new ObservableCollection<TaskDisplayItem>();
                TaskListBox.ItemsSource = taskItems;

                // Initialize log display
                logItems = new ObservableCollection<LogDisplayItem>();
                LogListBox.ItemsSource = logItems;

                // Initialize quiz option buttons
                quizOptionButtons = new List<RadioButton>
            {
                QuizOptionA,
                QuizOptionB,
                QuizOptionC,
                QuizOptionD
            };

                // Clear error message
                txtError.Text = "";

                // Load tasks for the user (once username is set)
                // This will be called after login

                logger.AddLog("Components initialized");
            }

            // ============ VOICE GREETING ============
            private void PlayVoiceGreeting()
            {
                try
                {
                    voice_greeting greet = new voice_greeting();
                    greet.greet();
                    logger.AddLog("Voice greeting played");
                }
                catch (Exception ex)
                {
                    logger.AddLog($"Voice greeting failed: {ex.Message}");
                }
            }

            // ============ NAVIGATION ============
            private void start(object sender, RoutedEventArgs e)
            {
                home_grid.Visibility = Visibility.Hidden;
                username_grid.Visibility = Visibility.Visible;
                usernames_input.Focus();
                logger.AddLog("User clicked Start");
            }

            private void submit_name(object sender, RoutedEventArgs e)
            {
                SubmitUsername();
            }

            private void UsernamesInput_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Enter)
                {
                    SubmitUsername();
                    e.Handled = true;
                }
            }

            private void SubmitUsername()
            {
                if (string.IsNullOrWhiteSpace(usernames_input.Text))
                {
                    txtError.Text = "⚠️ Please enter a valid username.";
                    txtError.Foreground = new SolidColorBrush(Colors.Red);
                    usernames_input.Focus();
                    return;
                }

                username = user.submit_name(usernames_input);

                if (string.IsNullOrEmpty(username))
                {
                    txtError.Text = "⚠️ Username must contain only letters and spaces.";
                    txtError.Foreground = new SolidColorBrush(Colors.Red);
                    usernames_input.Focus();
                    return;
                }

                // Save user
                user.save_user(username);

                // Check if returning user
                bool oldUser = user.user_exists(username);

                // Clear error
                txtError.Text = "";

                // Show chat
                username_grid.Visibility = Visibility.Hidden;
                chat_grid.Visibility = Visibility.Visible;

                // Update user display
                UserDisplay.Text = $" Welcome, {username}!";

              
                // LOAD TASKS FOR USER - ADDED HERE
            
                // Load tasks for the logged-in user
                RefreshTaskList();
                
                // Recall interests
                string recalledInterest = memory.recall_interest(username);

                // Welcome message
                if (oldUser)
                {
                    AddChatMessage($"Welcome back, {username}! Great to see you again.");
                    if (!string.IsNullOrEmpty(recalledInterest))
                    {
                        AddChatMessage($"I remember you're interested in {recalledInterest}. Would you like to learn more?");
                    }
                }
                else
                {
                    AddChatMessage($"Hello, {username}! I'm SecureBot, your cybersecurity awareness assistant.");
                    AddChatMessage("I can help you with:\n" +
                                  "•  Password safety tips\n" +
                                  "•  Phishing awareness\n" +
                                  "•  Privacy protection\n" +
                                  "•  Task management\n" +
                                  "•  Cybersecurity quizzes\n\n" +
                                  "Ask me anything about cybersecurity!");
                }

                logger.LogUserLogin(username);
                txtUserInput.Focus();
            }

            // ============ CHAT FUNCTIONALITY ============
            private void btnSend_Click(object sender, RoutedEventArgs e)
            {
                SendMessage();
            }

            private void TxtUserInput_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Enter)
                {
                    SendMessage();
                    e.Handled = true;
                }
            }

            private void SendMessage()
            {
                string userInput = txtUserInput.Text.Trim();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    return;
                }

                // Display user message
                AddChatMessage($"You: {userInput}");

                // Log user question
                logger.LogUserQuestion(userInput);

                // Process message
                ProcessUserMessage(userInput);

                // Clear input
                txtUserInput.Clear();

                // Auto show interests
                AutoShowInterest();
            }

            private void ProcessUserMessage(string userInput)
            {
                string lowerInput = userInput.ToLower();

                // Check for special commands
                if (ProcessSpecialCommand(userInput))
                    return;

                // Check for quiz commands
                if (lowerInput.Contains("start quiz") || lowerInput.Contains("take quiz") ||
                    lowerInput.Contains("play quiz"))
                {
                    ShowQuizPage();
                    return;
                }

                if (lowerInput.Contains("show tasks") || lowerInput.Contains("list tasks"))
                {
                    ShowTasksPage();
                    return;
                }

                if (lowerInput.Contains("show log") || lowerInput.Contains("activity log"))
                {
                    ShowLogPage();
                    return;
                }

                // Get sentiment
                string feeling = sentiment.detect_sentiment(userInput);

                // If sentiment detected, show sentiment response first
                if (feeling != "neutral")
                {
                    string sentimentResponse = sentiment.sentiment_response(feeling);
                    AddChatMessage($"SecureBot: {sentimentResponse}");
                    logger.LogSentimentDetected(feeling, userInput);

                    // Save interest if curious
                    if (feeling == "curious")
                    {
                        string topic = ExtractTopic(userInput);
                        if (!string.IsNullOrEmpty(topic))
                        {
                            memory.save_interest(username, topic);
                            AddChatMessage($"SecureBot: I'll remember your interest in {topic}!");
                        }
                    }
                }

                // Get chatbot response
                string response = bot.GetResponse(userInput, username);
                AddChatMessage($"SecureBot: {response}");
                logger.LogChatbotResponse(response);
            }

            private bool ProcessSpecialCommand(string userInput)
            {
                string lowerInput = userInput.ToLower();

               
                // ADD TASK FROM CHAT - ADDED HERE
                
                if (lowerInput.StartsWith("add task") || lowerInput.StartsWith("addtask"))
                {
                    string title = userInput.Substring(lowerInput.IndexOf("add task") + 8).Trim();
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        AddChatMessage("SecureBot: Please specify a task. Example: 'add task Review privacy settings'");
                        return true;
                    }

                    var task = new CyberTask
                    {
                        Title = title,
                        Description = "Task added via chat",
                        DueDate = DateTime.Now.AddDays(7),
                        Category = "General",
                        Priority = CyberTask.PriorityLevel.Medium
                    };

                    // ADD TASK TO DATABASE
                    if (taskManager.AddTask(task, username))
                    {
                        AddChatMessage($"SecureBot: ✅ Task '{title}' added successfully! Would you like to set a reminder?");
                        RefreshTaskList(); // Refresh the task list
                    }
                    else
                    {
                        AddChatMessage("SecureBot:  Failed to add task. Please try again.");
                    }
                    return true;
                }

          
                // COMPLETE TASK FROM CHAT - ADDED HERE
                
                if (lowerInput.StartsWith("complete task") || lowerInput.StartsWith("completetask"))
                {
                    string title = userInput.Substring(lowerInput.IndexOf("complete task") + 13).Trim();
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        AddChatMessage("SecureBot: Please specify which task to complete. Example: 'complete task Review privacy settings'");
                        return true;
                    }

                    // GET ALL TASKS AND FIND MATCHING
                    var tasks = taskManager.GetAllTasks(username);
                    var task = tasks.FirstOrDefault(t =>
                        t.Title.ToLower().Contains(title.ToLower()) && !t.IsCompleted);

                    if (task != null)
                    {
                        // COMPLETE THE TASK
                        if (taskManager.CompleteTask(task.Id))
                        {
                            AddChatMessage($"SecureBot: Task '{task.Title}' marked as completed! Great job!");
                            RefreshTaskList();
                        }
                    }
                    else
                    {
                        AddChatMessage($"SecureBot:  Could not find a pending task with title containing '{title}'.");
                    }
                    return true;
                }

                
                // DELETE TASK FROM CHAT - ADDED HERE
            
                if (lowerInput.StartsWith("delete task") || lowerInput.StartsWith("deletetask"))
                {
                    string title = userInput.Substring(lowerInput.IndexOf("delete task") + 11).Trim();
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        AddChatMessage("SecureBot: Please specify which task to delete. Example: 'delete task Review privacy settings'");
                        return true;
                    }

                    // GET ALL TASKS AND FIND MATCHING
                    var tasks = taskManager.GetAllTasks(username);
                    var task = tasks.FirstOrDefault(t =>
                        t.Title.ToLower().Contains(title.ToLower()));

                    if (task != null)
                    {
                        // DELETE THE TASK
                        if (taskManager.DeleteTask(task.Id))
                        {
                            AddChatMessage($"SecureBot:  Task '{task.Title}' deleted.");
                            RefreshTaskList();
                        }
                    }
                    else
                    {
                        AddChatMessage($"SecureBot:  Could not find a task with title containing '{title}'.");
                    }
                    return true;
                }

                return false;
            }

            private string ExtractTopic(string userInput)
            {
                string lowerInput = userInput.ToLower();
                string[] topics = { "password", "phishing", "privacy", "malware", "firewall",
                               "security", "hacking", "2fa", "ransomware" };

                foreach (string topic in topics)
                {
                    if (lowerInput.Contains(topic))
                    {
                        return topic;
                    }
                }
                return null;
            }

            // ============ CHAT UI HELPERS ============
            private void AddChatMessage(string message)
            {
                try
                {
                    Paragraph paragraph = new Paragraph();
                    Run run = new Run(message);
                    run.Foreground = new SolidColorBrush(Colors.White);
                    paragraph.Inlines.Add(run);
                    paragraph.Margin = new Thickness(0, 3, 0, 3);
                    txtChat.Document.Blocks.Add(paragraph);
                    txtChat.ScrollToEnd();
                }
                catch (Exception ex)
                {
                    logger.AddLog($"Error adding chat message: {ex.Message}");
                }
            }

            private void ClearChat()
            {
                txtChat.Document.Blocks.Clear();
            }

            // ============ AUTO SHOW INTERESTS ============
            private void AutoShowInterest()
            {
                counting++;
                if (counting >= 3)
                {
                    string recalledInterest = memory.recall_interest(username);
                    if (!string.IsNullOrEmpty(recalledInterest))
                    {
                        AddChatMessage($"SecureBot: Just a reminder, you're interested in {recalledInterest}. Would you like to learn more?");
                    }
                    counting = 0;
                }
            }

            // ============ PAGE NAVIGATION ============
            private void ShowTasksPage()
            {
                chat_grid.Visibility = Visibility.Hidden;
                tasks_grid.Visibility = Visibility.Visible;
                quiz_grid.Visibility = Visibility.Hidden;
                log_grid.Visibility = Visibility.Hidden;
                RefreshTaskList();
            }

            private void ShowQuizPage()
            {
                chat_grid.Visibility = Visibility.Hidden;
                tasks_grid.Visibility = Visibility.Hidden;
                quiz_grid.Visibility = Visibility.Visible;
                log_grid.Visibility = Visibility.Hidden;
                StartQuiz();
            }

            private void ShowLogPage()
            {
                chat_grid.Visibility = Visibility.Hidden;
                tasks_grid.Visibility = Visibility.Hidden;
                quiz_grid.Visibility = Visibility.Hidden;
                log_grid.Visibility = Visibility.Visible;
                RefreshLogDisplay();
            }

            private void BackToChat()
            {
                chat_grid.Visibility = Visibility.Visible;
                tasks_grid.Visibility = Visibility.Hidden;
                quiz_grid.Visibility = Visibility.Hidden;
                log_grid.Visibility = Visibility.Hidden;
                txtUserInput.Focus();
            }

            
            // TASKS BUTTONS - ALL TASK MANAGER METHODS HERE
            
            private void TasksButton_Click(object sender, RoutedEventArgs e)
            {
                ShowTasksPage();
            }

            private void RefreshTasksButton_Click(object sender, RoutedEventArgs e)
            {
                RefreshTaskList();
            }

        private void ClearChatButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear all chat messages?",
                "Clear Chat", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                txtChat.Document.Blocks.Clear();
                logger?.AddLog("Chat cleared by user");
                AddChatMessage("🔄 Chat cleared. Start a new conversation!");
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
            {
                string title = TaskTitleInput.Text.Trim();
                string description = TaskDescriptionInput.Text.Trim();

                if (string.IsNullOrWhiteSpace(title))
                {
                    MessageBox.Show("Please enter a task title.", "Missing Info",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var task = new CyberTask
                {
                    Title = title,
                    Description = string.IsNullOrEmpty(description) ? "No description" : description,
                    DueDate = TaskDueDatePicker.SelectedDate ?? DateTime.Now.AddDays(7),
                    Category = "General",
                    Priority = GetPriorityFromCombo()
                };

                
                // ADD TASK TO DATABASE - ADDED HERE
                
                if (taskManager.AddTask(task, username))
                {
                    MessageBox.Show($"Task '{title}' added successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    TaskTitleInput.Clear();
                    TaskDescriptionInput.Clear();
                    RefreshTaskList();
                }
                else
                {
                    MessageBox.Show("Failed to add task. Please try again.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        // showinput
        // ================================================
        // CUSTOM INPUT DIALOG - ADD THIS METHOD
        // ================================================
        private string ShowInputDialog(string title, string prompt, string defaultValue = "")
        {
            // Create a simple input dialog window
            Window dialog = new Window
            {
                Title = title,
                Width = 450,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = new SolidColorBrush(Color.FromRgb(16, 23, 40)),
                Foreground = System.Windows.Media.Brushes.White,
                ResizeMode = ResizeMode.NoResize,
                Owner = this
            };

            // Main panel
            StackPanel panel = new StackPanel { Margin = new Thickness(20) };

            // Prompt text
            TextBlock promptText = new TextBlock
            {
                Text = prompt,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(promptText);

            // Input box
            TextBox inputBox = new TextBox
            {
                Text = defaultValue,
                Height = 35,
                FontSize = 14,
                Background = new SolidColorBrush(Color.FromRgb(16, 23, 40)),
                Foreground = System.Windows.Media.Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 229, 255)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10, 5, 10, 5)
            };
            panel.Children.Add(inputBox);

            // Buttons panel
            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 15, 0, 0)
            };

            // OK Button
            Button okButton = new Button
            {
                Content = "✅ OK",
                Width = 90,
                Height = 35,
                Margin = new Thickness(0, 0, 10, 0),
                Background = new SolidColorBrush(Color.FromRgb(0, 229, 255)),
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Cursor = Cursors.Hand
            };
            okButton.Click += (s, e) => { dialog.DialogResult = true; dialog.Close(); };
            buttonPanel.Children.Add(okButton);

            // Cancel Button
            Button cancelButton = new Button
            {
                Content = "❌ Cancel",
                Width = 90,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 14,
                Cursor = Cursors.Hand
            };
            cancelButton.Click += (s, e) => { dialog.DialogResult = false; dialog.Close(); };
            buttonPanel.Children.Add(cancelButton);

            panel.Children.Add(buttonPanel);
            dialog.Content = panel;

            // Set enter key to submit
            inputBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    dialog.DialogResult = true;
                    dialog.Close();
                }
            };

            // Focus the input box
            dialog.Loaded += (s, e) => inputBox.Focus();

            // Show dialog and get result
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                return inputBox.Text.Trim();
            }
            return null;
        }

        private void SetReminderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Step 1: Check if a task is selected
                var selectedItem = TaskListBox.SelectedItem as TaskDisplayItem;

                if (selectedItem == null)
                {
                    MessageBox.Show("📋 Please select a task from the list first.\n\n" +
                                    "Tip: Click on a task in the list to highlight it, then click 'Set Reminder'.",
                        "No Task Selected",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // Step 2: Get the full task from database
                var task = taskManager.GetTaskById(selectedItem.Id);

                if (task == null)
                {
                    MessageBox.Show("❌ Task not found in database. Please refresh the list.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // Step 3: Check if task is already completed
                if (task.IsCompleted)
                {
                    MessageBox.Show($"✅ Task '{task.Title}' is already completed.\n\n" +
                                    "Cannot set reminder for completed tasks.",
                        "Task Completed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Step 4: Show current reminder status
                string currentReminder = task.ReminderDate.HasValue ?
                    $"Current reminder: {task.ReminderDate.Value.ToShortDateString()}" :
                    "No reminder set";

                // Step 5: Show input dialog
                string defaultDays = task.ReminderDate.HasValue ?
                    ((int)(task.ReminderDate.Value - DateTime.Now).TotalDays).ToString() :
                    "7";

                string input = ShowInputDialog(
                    "Set Reminder",
                    $"🔔 Set Reminder for Task\n\n" +
                    $"Task: {task.Title}\n" +
                    $"Due Date: {task.DueDate.ToShortDateString()}\n" +
                    $"{currentReminder}\n\n" +
                    $"Enter number of days from now:\n" +
                    $"(Enter 0 to clear reminder)",
                    defaultDays);

                // Step 6: Check if user cancelled
                if (string.IsNullOrEmpty(input))
                {
                    return;
                }

                // Step 7: Validate input
                if (!int.TryParse(input, out int days))
                {
                    MessageBox.Show("❌ Please enter a valid number.",
                        "Invalid Input",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Step 8: Check for negative days
                if (days < 0)
                {
                    MessageBox.Show("❌ Please enter a positive number of days.",
                        "Invalid Input",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Step 9: Handle clearing reminder
                if (days == 0)
                {
                    task.ClearReminder();
                    if (taskManager.UpdateTask(task))
                    {
                        logger.AddLog($"Reminder cleared for '{task.Title}'");
                        MessageBox.Show($"✅ Reminder cleared for '{task.Title}'.",
                            "Reminder Cleared",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        RefreshTaskList();
                    }
                    return;
                }

                // Step 10: Set the reminder
                task.SetReminder(days);

                if (taskManager.UpdateTask(task))
                {
                    logger.LogReminderSet(task.Title, task.ReminderDate.Value);
                    MessageBox.Show($"✅ Reminder set for '{task.Title}'!\n\n" +
                                    $"📅 Reminder Date: {task.ReminderDate.Value.ToShortDateString()}\n" +
                                    $"📆 Days from now: {days}\n\n" +
                                    $"You will be reminded on {task.ReminderDate.Value.ToShortDateString()}.",
                        "Reminder Set",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    RefreshTaskList();
                }
                else
                {
                    MessageBox.Show("❌ Failed to set reminder. Please try again.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"SetReminderButton_Click error: {ex.Message}", ex.StackTrace);
                MessageBox.Show($"❌ Error: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BackToChatFromTasks_Click(object sender, RoutedEventArgs e)
            {
                BackToChat();
            }

            private CyberTask.PriorityLevel GetPriorityFromCombo()
            {
                if (TaskPriorityCombo.SelectedItem == null)
                    return CyberTask.PriorityLevel.Medium;

                string selected = (TaskPriorityCombo.SelectedItem as ComboBoxItem).Content.ToString();
                if (selected.Contains("Low")) return CyberTask.PriorityLevel.Low;
                if (selected.Contains("Medium")) return CyberTask.PriorityLevel.Medium;
                if (selected.Contains("High")) return CyberTask.PriorityLevel.High;
                if (selected.Contains("Critical")) return CyberTask.PriorityLevel.Critical;
                return CyberTask.PriorityLevel.Medium;
            }

            private void TaskListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                var selected = TaskListBox.SelectedItem as TaskDisplayItem;
                if (selected == null) return;

                
                // GET TASK BY ID - ADDED HERE
                
                var task = taskManager.GetTaskById(selected.Id);
                if (task == null) return;

                if (task.IsCompleted)
                {
                    if (MessageBox.Show($"Delete task '{task.Title}'?", "Delete Task",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                   
                        // DELETE TASK - ADDED HERE
                      
                        taskManager.DeleteTask(task.Id);
                        RefreshTaskList();
                    }
                }
                else
                {
                    if (MessageBox.Show($"Mark '{task.Title}' as completed?", "Complete Task",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                    
                        // COMPLETE TASK - ADDED HERE
                        
                        taskManager.CompleteTask(task.Id);
                        RefreshTaskList();
                    }
                }
            }


        // REFRESH TASK LIST - ADDED HERE

        private void RefreshTaskList()
        {
            try
            {
                taskItems.Clear();
                var tasks = taskManager.GetAllTasks(username);

                foreach (var task in tasks)
                {
                    // Determine priority color
                    Brush priorityColor;
                    string priorityText;

                    switch (task.Priority)
                    {
                        case CyberTask.PriorityLevel.Low:
                            priorityColor = new SolidColorBrush(Color.FromRgb(0, 255, 100));  // Green
                            priorityText = "🟢 Low Priority";
                            break;
                        case CyberTask.PriorityLevel.Medium:
                            priorityColor = new SolidColorBrush(Color.FromRgb(255, 255, 0));  // Yellow
                            priorityText = "🟡 Medium Priority";
                            break;
                        case CyberTask.PriorityLevel.High:
                            priorityColor = new SolidColorBrush(Color.FromRgb(255, 165, 0));  // Orange
                            priorityText = "🟠 High Priority";
                            break;
                        case CyberTask.PriorityLevel.Critical:
                            priorityColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));    // Red
                            priorityText = "🔴 Critical Priority";
                            break;
                        default:
                            priorityColor = new SolidColorBrush(Color.FromRgb(255, 255, 0));  // Yellow
                            priorityText = "🟡 Medium Priority";
                            break;
                    }

                    taskItems.Add(new TaskDisplayItem
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        DueDateDisplay = $"📅 {task.DueDate.ToShortDateString()}",
                        StatusIcon = task.IsCompleted ? "✅" : "⏳",
                        PriorityIcon = task.GetPriorityIcon(),
                        PriorityText = priorityText,
                        PriorityColor = priorityColor,
                        BorderBackground = task.IsCompleted ?
                            new SolidColorBrush(Color.FromRgb(40, 60, 40)) :
                            new SolidColorBrush(Color.FromRgb(40, 40, 60))
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error refreshing tasks: {ex.Message}", ex.StackTrace);
            }
        }
        private string GetPriorityIcon(CyberTask.PriorityLevel priority)
            {
                switch (priority)
                {
                    case CyberTask.PriorityLevel.Low: return "yellow";
                    case CyberTask.PriorityLevel.Medium: return "orange";
                    case CyberTask.PriorityLevel.High: return "red";
                    case CyberTask.PriorityLevel.Critical: return "darkred";
                    default: return "";
                }
            }



           
            // QUIZ FUNCTIONALITY
            
            private void QuizButton_Click(object sender, RoutedEventArgs e)
            {
                ShowQuizPage();
            }

            private void StartQuiz()
            {
                try
                {
                    var quizLoader = new Quiz_Question_Load();
                    quizQuestions = quizLoader.GetRandomQuestions(10);
                    currentQuizIndex = 0;
                    quizScore = 0;
                    isQuizAnswered = false;

                    QuizScoreDisplay.Text = "⭐ Score: 0";
                    QuizNextButton.IsEnabled = false;
                    QuizFeedbackArea.Visibility = Visibility.Collapsed;

                    LoadQuizQuestion();
                    logger.LogQuizStarted();
                }
                catch (Exception ex)
                {
                    logger.AddLog($"Error starting quiz: {ex.Message}");
                    MessageBox.Show("Failed to start quiz. Please try again.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            private void LoadQuizQuestion()
            {
                if (currentQuizIndex >= quizQuestions.Count)
                {
                    EndQuiz();
                    return;
                }

                var question = quizQuestions[currentQuizIndex];

                QuizCategoryDisplay.Text = question.Category;
                QuizQuestionText.Text = question.Text;
                QuizCounter.Text = $"{currentQuizIndex + 1} / {quizQuestions.Count}";

                var options = question.GetAllOptionsShuffled();

                for (int i = 0; i < quizOptionButtons.Count && i < options.Count; i++)
                {
                    quizOptionButtons[i].Content = options[i];
                    quizOptionButtons[i].Tag = options[i];
                    quizOptionButtons[i].IsChecked = false;
                    quizOptionButtons[i].IsEnabled = true;
                }

                for (int i = options.Count; i < quizOptionButtons.Count; i++)
                {
                    quizOptionButtons[i].Visibility = Visibility.Collapsed;
                }

                isQuizAnswered = false;
                QuizNextButton.IsEnabled = false;
                QuizFeedbackArea.Visibility = Visibility.Collapsed;

                foreach (var button in quizOptionButtons)
                {
                    button.Background = null;
                    button.Visibility = Visibility.Visible;
                }
            }

            private void QuizOption_Checked(object sender, RoutedEventArgs e)
            {
                if (isQuizAnswered) return;

                var selected = sender as RadioButton;
                if (selected == null || selected.Tag == null) return;

                string selectedAnswer = selected.Tag.ToString();
                var question = quizQuestions[currentQuizIndex];
                bool isCorrect = question.IsCorrect(selectedAnswer);

                if (isCorrect)
                {
                    quizScore += 10;
                    QuizScoreDisplay.Text = $"⭐ Score: {quizScore}";
                    selected.Background = new SolidColorBrush(Color.FromRgb(0, 128, 0));
                    QuizFeedbackText.Text = $" Correct! {question.Explanation}";
                    QuizFeedbackText.Foreground = new SolidColorBrush(Colors.LightGreen);
                }
                else
                {
                    selected.Background = new SolidColorBrush(Color.FromRgb(128, 0, 0));
                    QuizFeedbackText.Text = $" Incorrect. {question.Explanation}";
                    QuizFeedbackText.Foreground = new SolidColorBrush(Colors.LightCoral);

                    foreach (var button in quizOptionButtons)
                    {
                        if (button.Tag != null && button.Tag.ToString() == question.CorrectAnswer)
                        {
                            button.Background = new SolidColorBrush(Color.FromRgb(0, 128, 0));
                            break;
                        }
                    }
                }

                foreach (var button in quizOptionButtons)
                {
                    button.IsEnabled = false;
                }

                isQuizAnswered = true;
                QuizNextButton.IsEnabled = true;
                QuizFeedbackArea.Visibility = Visibility.Visible;

                logger.LogQuizQuestionAnswered(currentQuizIndex + 1, isCorrect);
            }

            private void QuizNextButton_Click(object sender, RoutedEventArgs e)
            {
                currentQuizIndex++;
                LoadQuizQuestion();
            }

            private void QuizBackButton_Click(object sender, RoutedEventArgs e)
            {
                if (currentQuizIndex > 0 || quizScore > 0)
                {
                    var result = MessageBox.Show($"Your current score is {quizScore}. Are you sure you want to quit the quiz?",
                        "Quit Quiz?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        logger.AddLog($"Quiz quit early. Score: {quizScore}");
                        BackToChat();
                    }
                }
                else
                {
                    BackToChat();
                }
            }

            private void EndQuiz()
            {
                double percentage = (double)quizScore / (quizQuestions.Count * 10) * 100;
                string feedback;

                if (percentage >= 80)
                    feedback = "⭐ Excellent! You're a cybersecurity expert!";
                else if (percentage >= 60)
                    feedback = " Good job! Keep learning to improve your cybersecurity knowledge!";
                else
                    feedback = " Keep learning! Practice makes perfect!";

                MessageBox.Show($" Quiz Complete!\n\nScore: {quizScore} points\n{feedback}",
                    "Quiz Complete", MessageBoxButton.OK, MessageBoxImage.Information);

                logger.LogQuizCompleted(quizScore / 10, quizQuestions.Count);
                BackToChat();
            }

            // ================================================
            // LOG FUNCTIONALITY
            // ================================================
            private void LogButton_Click(object sender, RoutedEventArgs e)
            {
                ShowLogPage();
            }

            private void RefreshLogButton_Click(object sender, RoutedEventArgs e)
            {
                RefreshLogDisplay();
            }

            private void ClearLogButton_Click(object sender, RoutedEventArgs e)
            {
                if (MessageBox.Show("Clear all activity logs?", "Clear Logs",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    logger.ClearLogs();
                    RefreshLogDisplay();
                }
            }

            private void RefreshLogDisplay()
            {
                logItems.Clear();
                var logs = logger.GetRecentLogs(50);

                foreach (var log in logs)
                {
                    bool isError = log.Contains("Error") || log.Contains("Failed");
                    logItems.Add(new LogDisplayItem
                    {
                        Entry = log,
                        LogBackground = new SolidColorBrush(isError ?
                            Color.FromRgb(40, 10, 10) : Color.FromRgb(20, 30, 40)),
                        Foreground = new SolidColorBrush(isError ?
                            Color.FromRgb(255, 100, 100) : Color.FromRgb(200, 200, 200))
                    });
                }
            }

        private void BackToChatFromLog_Click(object sender, RoutedEventArgs e)
        {
            BackToChat();
        }

        // ============ EXIT ============
        private void ExitButton_Click(object sender, RoutedEventArgs e)
            {
                logger.AddLog("Application closed");
                Close();
            }

            protected override void OnClosed(EventArgs e)
            {
                logger.AddLog("Window closed");
                base.OnClosed(e);
            }
        }

        // ============ DISPLAY CLASSES ============

        public class TaskDisplayItem
        {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DueDateDisplay { get; set; }
        public string StatusIcon { get; set; }
        public string PriorityIcon { get; set; }
        public string PriorityText { get; set; }
        public Brush PriorityColor { get; set; }
        public Brush BorderBackground { get; set; }
    }

        public class LogDisplayItem
        {
            public string Entry { get; set; }
            public Brush LogBackground { get; set; }
            public Brush Foreground { get; set; }
        }
    }
