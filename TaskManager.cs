
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Cybersecurity_Awareness_Chatbot_Part2
{
 
        public class TaskManager
        {
            // ============ FIELDS ============
            private string connectionString;
            private ActivityLogger logger;
            private bool isConnected;

            // ============ CONSTRUCTORS ============

        
            public TaskManager(ActivityLogger logger = null)
            {
                this.logger = logger;

                // Configure connection string for SQL Server
                // Update with your SQL Server credentials
                connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=chatbot_task;Integrated Security=True;";

                // Test connection
                TestConnection();

                // Create table if it doesn't exist
                CreateTableIfNotExists();

                logger?.LogDatabaseConnection(isConnected, "TaskManager initialized");
            }

       
            public TaskManager(string connectionString, ActivityLogger logger = null)
            {
                this.connectionString = connectionString;
                this.logger = logger;

                TestConnection();
                CreateTableIfNotExists();

                logger?.LogDatabaseConnection(isConnected, "TaskManager initialized with custom connection");
            }

            //  CONNECTION MANAGEMENT 

           
            private void TestConnection()
            {
                try
                {
                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        isConnected = true;
                        conn.Close();
                    }
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

            //  TABLE CREATION 

           
            private void CreateTableIfNotExists()
            {
                string query = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tasks' AND xtype='U')
                BEGIN
                    CREATE TABLE tasks(
                        tasks_id INT PRIMARY KEY IDENTITY(1,1),
                        tasks_name VARCHAR(255),
                        tasks_description VARCHAR(255),
                        tasks_status VARCHAR(50),
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

            //  CRUD OPERATIONS

           
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
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@created_date", DateTime.Now);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                logger?.LogTaskAdded(task.Title);
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error adding task: {ex.Message}", ex.StackTrace);
                }
                return false;
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

            public List<CyberTask> GetTasksByStatus(string status, string username = null)
            {
                List<CyberTask> tasks = new List<CyberTask>();

                try
                {
                    string query = "SELECT * FROM tasks WHERE tasks_status = @status";

                    if (!string.IsNullOrEmpty(username))
                    {
                        query += " AND username = @username";
                    }

                    query += " ORDER BY tasks_id DESC";

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@status", status);

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
                    logger?.LogError($"Error getting tasks by status: {ex.Message}", ex.StackTrace);
                }

                return tasks;
            }

         
            public List<CyberTask> GetPendingTasks(string username = null)
            {
                return GetTasksByStatus("pending", username);
            }

          
            public List<CyberTask> GetCompletedTasks(string username = null)
            {
                return GetTasksByStatus("done", username);
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
                    // Get task title for logging
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

            public bool UncompleteTask(int taskId)
            {
                try
                {
                    string query = "UPDATE tasks SET tasks_status = 'pending' WHERE tasks_id = @tasks_id";

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@tasks_id", taskId);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            return rowsAffected > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error uncompleting task: {ex.Message}", ex.StackTrace);
                }
                return false;
            }

            public bool DeleteTask(int taskId)
            {
                try
                {
                    // Get task title for logging before deletion
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

            public bool DeleteAllTasks(string username)
            {
                try
                {
                    string query = "DELETE FROM tasks WHERE username = @username";
                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            logger?.AddLog($"Deleted {rowsAffected} tasks for user '{username}'");
                            return rowsAffected > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error deleting all tasks: {ex.Message}", ex.StackTrace);
                }
                return false;
            }

            public bool UpdateTaskStatus(int taskId, string status)
            {
                try
                {
                    string query = "UPDATE tasks SET tasks_status = @status WHERE tasks_id = @tasks_id";

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@tasks_id", taskId);
                            cmd.Parameters.AddWithValue("@status", status);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                if (status == "done")
                                {
                                    logger?.LogTaskCompleted(GetTaskTitleById(taskId));
                                }
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error updating task status: {ex.Message}", ex.StackTrace);
                }
                return false;
            }

            // ============ HELPER METHODS ============

            
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
                // Get the status string from database
                string status = reader["tasks_status"].ToString();
                bool isCompleted = status.ToLower() == "done";

                // Parse due date
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
                    Priority = CyberTask.PriorityLevel.Medium
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

          
            public int GetCompletedTaskCount(string username = null)
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM tasks WHERE tasks_status = 'done'";

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
                    logger?.LogError($"Error getting completed count: {ex.Message}", ex.StackTrace);
                }
                return 0;
            }

            public bool IsConnected()
            {
                return isConnected;
            }

            
            public List<CyberTask> SearchTasks(string searchTerm, string username = null)
            {
                List<CyberTask> tasks = new List<CyberTask>();

                try
                {
                    string query = @"
                    SELECT * FROM tasks 
                    WHERE tasks_name LIKE @searchTerm 
                    OR tasks_description LIKE @searchTerm";

                    if (!string.IsNullOrEmpty(username))
                    {
                        query += " AND username = @username";
                    }

                    query += " ORDER BY tasks_id DESC";

                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");

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
                    logger?.LogError($"Error searching tasks: {ex.Message}", ex.StackTrace);
                }

                return tasks;
            }
        }
    }

