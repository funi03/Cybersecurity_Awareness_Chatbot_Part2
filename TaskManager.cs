
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Cybersecurity_Awareness_Chatbot_Part2
{

  
        public class TaskManager
        {
            private string connectionString;
            private ActivityLogger logger;
            private bool isConnected;

            public TaskManager(ActivityLogger logger = null)
            {
                this.logger = logger;

                // ================================================
                // TRY BOTH CONNECTION STRINGS
                // ================================================
                // Try LocalDB first
                connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=chatbot_task;Integrated Security=True;";

                // If that fails, try SQL Express
                // connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=chatbot_task;Integrated Security=True;";

                TestConnection();
                CreateTableIfNotExists();
                logger?.LogDatabaseConnection(isConnected, "TaskManager initialized");
            }

            private void TestConnection()
            {
                try
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        isConnected = true;
                        conn.Close();
                    }
                    logger?.AddLog("Database connection successful");
                }
                catch (Exception ex)
                {
                    isConnected = false;
                    logger?.LogError($"Database connection failed: {ex.Message}", ex.StackTrace);
                }
            }

            private SqlConnection GetConnection()
            {
                return new SqlConnection(connectionString);
            }

            private void CreateTableIfNotExists()
            {
                string query = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tasks' AND xtype='U')
                BEGIN
                    CREATE TABLE tasks(
                        tasks_id INT PRIMARY KEY IDENTITY(1,1),
                        tasks_name VARCHAR(255),
                        tasks_description VARCHAR(255),
                        tasks_status VARCHAR(50) DEFAULT 'pending',
                        tasks_duedate VARCHAR(50),
                        username VARCHAR(100) DEFAULT 'default_user',
                        created_date DATETIME DEFAULT GETDATE()
                    );
                END";

                try
                {
                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                    logger?.AddLog("Task table created/verified successfully");
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Failed to create task table: {ex.Message}", ex.StackTrace);
                }
            }

            public bool AddTask(CyberTask task, string username = "default_user")
            {
                try
                {
                    string query = @"
                    INSERT INTO tasks 
                    (tasks_name, tasks_description, tasks_status, tasks_duedate, username, created_date)
                    VALUES 
                    (@tasks_name, @tasks_description, @tasks_status, @tasks_duedate, @username, @created_date)";

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@tasks_name", task.Title);
                            cmd.Parameters.AddWithValue("@tasks_description", task.Description ?? "");
                            cmd.Parameters.AddWithValue("@tasks_status", task.IsCompleted ? "done" : "pending");
                            cmd.Parameters.AddWithValue("@tasks_duedate", task.DueDate.ToString("yyyy-MM-dd"));
                            

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                logger?.LogTaskAdded(task.Title);
                                return true;
                            }
                            else
                            {
                                logger?.AddLog("AddTask: No rows affected");
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error adding task: {ex.Message}", ex.StackTrace);
                    // Show the error in a message box for debugging
                    System.Windows.MessageBox.Show($"Database Error: {ex.Message}\n\n{ex.StackTrace}",
                        "Task Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return false;
                }
            }

            public List<CyberTask> GetAllTasks(string username = null)
            {
                List<CyberTask> tasks = new List<CyberTask>();

                try
                {
                    string query = "SELECT * FROM tasks";

                    if (!string.IsNullOrEmpty(username))
                    {
                        query += " WHERE username = @username";
                    }

                    query += " ORDER BY tasks_id DESC";

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            if (!string.IsNullOrEmpty(username))
                            {
                                cmd.Parameters.AddWithValue("@username", username);
                            }

                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    tasks.Add(MapReaderToTask(reader));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error getting tasks: {ex.Message}", ex.StackTrace);
                }

                return tasks;
            }

            public CyberTask GetTaskById(int taskId)
            {
                try
                {
                    string query = "SELECT * FROM tasks WHERE tasks_id = @tasks_id";

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@tasks_id", taskId);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    return MapReaderToTask(reader);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error getting task by ID: {ex.Message}", ex.StackTrace);
                }
                return null;
            }

            public bool UpdateTask(CyberTask task)
            {
                try
                {
                    string query = @"
                    UPDATE tasks 
                    SET tasks_name = @tasks_name, 
                        tasks_description = @tasks_description, 
                        tasks_status = @tasks_status,
                        tasks_duedate = @tasks_duedate
                    WHERE tasks_id = @tasks_id";

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@tasks_id", task.Id);
                            cmd.Parameters.AddWithValue("@tasks_name", task.Title);
                            cmd.Parameters.AddWithValue("@tasks_description", task.Description ?? "");
                            cmd.Parameters.AddWithValue("@tasks_status", task.IsCompleted ? "done" : "pending");
                            cmd.Parameters.AddWithValue("@tasks_duedate", task.DueDate.ToString("yyyy-MM-dd"));

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                if (task.IsCompleted)
                                {
                                    logger?.LogTaskCompleted(task.Title);
                                }
                                else
                                {
                                    logger?.LogTaskUpdated(task.Title, "Task details updated");
                                }
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error updating task: {ex.Message}", ex.StackTrace);
                }
                return false;
            }

            public bool CompleteTask(int taskId)
            {
                try
                {
                    string title = GetTaskTitleById(taskId);
                    string query = "UPDATE tasks SET tasks_status = 'done' WHERE tasks_id = @tasks_id";

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@tasks_id", taskId);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0 && !string.IsNullOrEmpty(title))
                            {
                                logger?.LogTaskCompleted(title);
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error completing task: {ex.Message}", ex.StackTrace);
                }
                return false;
            }

            public bool DeleteTask(int taskId)
            {
                try
                {
                    string title = GetTaskTitleById(taskId);
                    string query = "DELETE FROM tasks WHERE tasks_id = @tasks_id";

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@tasks_id", taskId);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0 && !string.IsNullOrEmpty(title))
                            {
                                logger?.LogTaskDeleted(title);
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error deleting task: {ex.Message}", ex.StackTrace);
                }
                return false;
            }

            private string GetTaskTitleById(int taskId)
            {
                try
                {
                    string query = "SELECT tasks_name FROM tasks WHERE tasks_id = @tasks_id";
                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@tasks_id", taskId);
                            var result = cmd.ExecuteScalar();
                            return result?.ToString() ?? "";
                        }
                    }
                }
                catch
                {
                    return "";
                }
            }

            private CyberTask MapReaderToTask(SqlDataReader reader)
            {
                string status = reader["tasks_status"].ToString();
                bool isCompleted = status.ToLower() == "done";

                DateTime dueDate;
                string dueDateStr = reader["tasks_duedate"].ToString();
                if (!DateTime.TryParse(dueDateStr, out dueDate))
                {
                    dueDate = DateTime.Now.AddDays(7);
                }

                return new CyberTask
                {
                    Id = Convert.ToInt32(reader["tasks_id"]),
                    Title = reader["tasks_name"].ToString(),
                    Description = reader["tasks_description"]?.ToString() ?? "",
                    DueDate = dueDate,
                    IsCompleted = isCompleted,
                    Category = "General",
                    Priority = CyberTask.PriorityLevel.Medium,
                    UserName = reader["username"]?.ToString() ?? ""
                };
            }

            public int GetPendingTaskCount(string username = null)
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM tasks WHERE tasks_status = 'pending'";

                    if (!string.IsNullOrEmpty(username))
                    {
                        query += " AND username = @username";
                    }

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            if (!string.IsNullOrEmpty(username))
                            {
                                cmd.Parameters.AddWithValue("@username", username);
                            }
                            return Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error getting pending count: {ex.Message}", ex.StackTrace);
                }
                return 0;
            }

            public bool IsConnected()
            {
                return isConnected;
            }
        }
    }
