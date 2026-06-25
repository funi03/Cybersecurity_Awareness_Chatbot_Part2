using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cybersecurity_Awareness_Chatbot_Part2
{
    
        public class ActivityLogger
        {
            // ============ FIELDS ============
            private List<string> logs;
            private string logFilePath;
            private const int MaxLogEntries = 200;
            private const int DisplayLimit = 50;

            // ============ CONSTRUCTOR ============
            public ActivityLogger()
            {
                logs = new List<string>();
                logFilePath = "activity_log.txt";
                LoadLogsFromFile();
            }

            // ============ PUBLIC METHODS ============

            
            public void AddLog(string action, string details = "")
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logEntry = $"[{timestamp}] {action}";

                if (!string.IsNullOrEmpty(details))
                {
                    logEntry += $" - {details}";
                }

                // Add to memory
                logs.Add(logEntry);

                // Trim if too many entries
                if (logs.Count > MaxLogEntries)
                {
                    logs.RemoveAt(0);
                }

                // Save to file
                SaveLogsToFile();
            }

            
            public List<string> GetRecentLogs(int count = 10)
            {
                if (count > DisplayLimit)
                {
                    count = DisplayLimit;
                }

                return logs.Skip(Math.Max(0, logs.Count - count)).ToList();
            }

            
            public List<string> GetAllLogs()
            {
                return new List<string>(logs);
            }

            
            public List<string> GetLogsByKeyword(string keyword)
            {
                return logs.Where(log => log.ToLower().Contains(keyword.ToLower())).ToList();
            }

            
            // Gets logs by date range
            
            public List<string> GetLogsByDateRange(DateTime startDate, DateTime endDate)
            {
                return logs.Where(log =>
                {
                    // Extract timestamp from log entry
                    string timestampStr = log.Substring(1, 19); // [yyyy-MM-dd HH:mm:ss]
                    if (DateTime.TryParse(timestampStr, out DateTime logDate))
                    {
                        return logDate >= startDate && logDate <= endDate;
                    }
                    return false;
                }).ToList();
            }

          
            // Gets error logs only
           
            public List<string> GetErrorLogs()
            {
                return logs.Where(log =>
                    log.ToLower().Contains("error") ||
                    log.ToLower().Contains("failed") ||
                    log.ToLower().Contains("exception")).ToList();
            }

            
            // Clears all activity logs
            
            public void ClearLogs()
            {
                logs.Clear();
                if (File.Exists(logFilePath))
                {
                    File.Delete(logFilePath);
                }
                AddLog("Activity log cleared");
            }

         
            // Gets the total number of log entries
            
            public int GetLogCount()
            {
                return logs.Count;
            }

            
            // Gets the count of error logs
            
            public int GetErrorCount()
            {
                return GetErrorLogs().Count;
            }

        
            // Exports logs to a file
            
            public void ExportLogs(string filePath)
            {
                try
                {
                    File.WriteAllLines(filePath, logs);
                }
                catch (Exception ex)
                {
                    AddLog($"Failed to export logs: {ex.Message}");
                }
            }

            // SPECIFIC LOGGING METHODS 

         
            // Logs when a task is added
           
            public void LogTaskAdded(string taskTitle)
            {
                AddLog($" Task Added: '{taskTitle}'");
            }

       
            // Logs when a task is completed
         
            public void LogTaskCompleted(string taskTitle)
            {
                AddLog($"Task Completed: '{taskTitle}'");
            }

            
            // Logs when a task is deleted
            
            public void LogTaskDeleted(string taskTitle)
            {
                AddLog($" Task Deleted: '{taskTitle}'");
            }

       
            // Logs when a reminder is set
            
            public void LogReminderSet(string taskTitle, DateTime reminderDate)
            {
                AddLog($"Reminder Set: '{taskTitle}' on {reminderDate.ToShortDateString()}");
            }

          
            // Logs when a quiz is started
          
            public void LogQuizStarted()
            {
                AddLog($" Quiz Started");
            }

            
            // Logs when a quiz is completed
       
            public void LogQuizCompleted(int score, int totalQuestions)
            {
                AddLog($" Quiz Completed: Score {score}/{totalQuestions}");
            }

           
            // Logs when a quiz question is answered
            
            public void LogQuizQuestionAnswered(int questionNumber, bool isCorrect)
            {
                AddLog($" Quiz Q{questionNumber}: {(isCorrect ? "Correct " : "Incorrect ")}");
            }

            // Logs when a user asks a question
            
            public void LogUserQuestion(string question)
            {
                string truncated = question.Length > 50 ? question.Substring(0, 50) + "..." : question;
                AddLog($" User Question: '{truncated}'");
            }

           
            // Logs when the chatbot responds
            
            public void LogChatbotResponse(string response)
            {
                if (!string.IsNullOrEmpty(response) && response.Length > 5)
                {
                    string truncated = response.Length > 50 ? response.Substring(0, 50) + "..." : response;
                    AddLog($"Chatbot Response: '{truncated}'");
                }
            }

            // Logs user login
            
            public void LogUserLogin(string username)
            {
                AddLog($" User Login: '{username}'");
            }

            
            // Logs user logout
            
            public void LogUserLogout(string username)
            {
                AddLog($" User Logout: '{username}'");
            }

            //Logs sentiment detected
      
            public void LogSentimentDetected(string sentiment, string context)
            {
                string truncated = context.Length > 30 ? context.Substring(0, 30) + "..." : context;
                AddLog($" Sentiment Detected: '{sentiment}' in '{truncated}'");
            }

           
            // Logs NLP interpretation
           
            public void LogNLPInterpretation(string userInput, string intent)
            {
                string truncated = userInput.Length > 30 ? userInput.Substring(0, 30) + "..." : userInput;
                AddLog($" NLP Interpretation: Intent '{intent}' from '{truncated}'");
            }

            
            public void LogTaskUpdated(string taskTitle, string changes)
            {
                AddLog($" Task Updated: '{taskTitle}' - {changes}");
            }

           
            public void LogError(string errorMessage, string stackTrace = "")
            {
                AddLog($" Error: {errorMessage}");
                if (!string.IsNullOrEmpty(stackTrace))
                {
                    AddLog($"   Stack Trace: {stackTrace.Substring(0, Math.Min(200, stackTrace.Length))}");
                }
            }

            
            public void LogSystemStartup()
            {
                AddLog($" System Started");
            }

            
            public void LogSystemShutdown()
            {
                AddLog($" System Shutdown");
            }

            
            public void LogDatabaseConnection(bool isConnected, string details = "")
            {
                string status = isConnected ? "Connected " : "Disconnected ";
                AddLog($"🗄️ Database {status}", details);
            }

           
            public void LogVoiceGreeting(bool isPlayed)
            {
                string status = isPlayed ? "Played " : "Failed ";
                AddLog($"🔊 Voice Greeting: {status}");
            }

            
            public void LogMemoryRecall(string username, string recalledData)
            {
                AddLog($" Memory Recall: '{username}' -> '{recalledData}'");
            }

           
            public void LogMemorySave(string username, string savedData)
            {
                AddLog($" Memory Saved: '{username}' -> '{savedData}'");
            }

            // ============ PRIVATE METHODS ============

            
            private void LoadLogsFromFile()
            {
                try
                {
                    if (File.Exists(logFilePath))
                    {
                        string[] lines = File.ReadAllLines(logFilePath);
                        logs = new List<string>(lines);

                        // Trim if too many entries
                        if (logs.Count > MaxLogEntries)
                        {
                            logs = logs.Skip(logs.Count - MaxLogEntries).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // If file loading fails, start with empty logs
                    logs = new List<string>();
                    AddLog($" Error loading logs: {ex.Message}");
                }
            }

            
            private void SaveLogsToFile()
            {
                try
                {
                    File.WriteAllLines(logFilePath, logs);
                }
                catch (Exception ex)
                {
                    // If saving fails, continue with in-memory logs only
                    // We don't want to lose functionality if file operations fail
                    Console.WriteLine($"Failed to save logs: {ex.Message}");
                }
            }

            
            private string FormatLogEntry(string action, string details = "")
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string entry = $"[{timestamp}] {action}";
                if (!string.IsNullOrEmpty(details))
                {
                    entry += $" - {details}";
                }
                return entry;
            }

            
            public string GetActivitySummary(int count = 5)
            {
                var recent = GetRecentLogs(count);
                if (recent.Count == 0)
                {
                    return "No recent activity.";
                }

                var summary = new System.Text.StringBuilder();
                summary.AppendLine(" Recent Activity Summary:");
                foreach (var log in recent)
                {
                    // Extract just the action part (remove timestamp)
                    int startIndex = log.IndexOf(']') + 2;
                    if (startIndex < log.Length)
                    {
                        summary.AppendLine($"• {log.Substring(startIndex)}");
                    }
                }
                return summary.ToString();
            }
        }
    }