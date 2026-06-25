using System;



namespace Cybersecurity_Awareness_Chatbot_Part2
    {
      
        public class CyberTask
        {
            // ============ PROPERTIES ============

            public int Id { get; set; }

        
            public string Title { get; set; }

        
            public string Description { get; set; }

           
            public DateTime DueDate { get; set; }

            
            public DateTime CreatedDate { get; set; }

            public DateTime? ReminderDate { get; set; }

          
            public bool IsCompleted { get; set; }

            
            public string Category { get; set; }

           
            public PriorityLevel Priority { get; set; }

          
            public string UserName { get; set; }

            // ============ ENUMS ============

          
            public enum PriorityLevel
            {
                Low = 0,
                Medium = 1,
                High = 2,
                Critical = 3
            }

            // ============ CONSTRUCTORS ============

          
            public CyberTask()
            {
                CreatedDate = DateTime.Now;
                IsCompleted = false;
                Priority = PriorityLevel.Medium;
                Category = "General";
                DueDate = DateTime.Now.AddDays(7);
                Title = string.Empty;
                Description = string.Empty;
                UserName = "default_user";
            }

           
            public CyberTask(string title, string description = "")
                : this()
            {
                Title = title;
                Description = description ?? string.Empty;
            }

           
            public CyberTask(string title, string description, DateTime dueDate)
                : this(title, description)
            {
                DueDate = dueDate;
            }

            public CyberTask(string title, string description, DateTime dueDate,
                             PriorityLevel priority, string category = "General")
                : this(title, description, dueDate)
            {
                Priority = priority;
                Category = category ?? "General";
            }

            // ============ PUBLIC METHODS ============

          
            public void Complete()
            {
                IsCompleted = true;
                ReminderDate = null; // Clear reminder when completed
            }

           
            public void Uncomplete()
            {
                IsCompleted = false;
            }

           
            public void SetReminder(int daysFromNow)
            {
                if (daysFromNow > 0)
                {
                    ReminderDate = DateTime.Now.AddDays(daysFromNow);
                }
                else
                {
                    ReminderDate = null;
                }
            }

        
            public void SetReminder(DateTime reminderDate)
            {
                if (reminderDate > DateTime.Now)
                {
                    ReminderDate = reminderDate;
                }
                else
                {
                    ReminderDate = null;
                }
            }

            public void ClearReminder()
            {
                ReminderDate = null;
            }

         
            public bool IsReminderDue()
            {
                if (!ReminderDate.HasValue)
                    return false;

                return DateTime.Now >= ReminderDate.Value && !IsCompleted;
            }

            
            public bool IsOverdue()
            {
                return DateTime.Now > DueDate && !IsCompleted;
            }

           
            public string GetStatusString()
            {
                if (IsCompleted)
                    return "✅ Completed";
                else if (IsReminderDue())
                    return "🔔 Reminder Due!";
                else if (IsOverdue())
                    return "⚠️ Overdue";
                else
                    return "⏳ Pending";
            }

           
            public string GetDatabaseStatus()
            {
                return IsCompleted ? "done" : "pending";
            }

          
            public string GetPriorityString()
            {
                switch (Priority)
                {
                    case PriorityLevel.Low:
                        return "🟢 Low";
                    case PriorityLevel.Medium:
                        return "🟡 Medium";
                    case PriorityLevel.High:
                        return "🟠 High";
                    case PriorityLevel.Critical:
                        return "🔴 Critical";
                    default:
                        return "🟡 Medium";
                }
            }

          
            public string GetPriorityIcon()
            {
                switch (Priority)
                {
                    case PriorityLevel.Low:
                        return "🟢";
                    case PriorityLevel.Medium:
                        return "🟡";
                    case PriorityLevel.High:
                        return "🟠";
                    case PriorityLevel.Critical:
                        return "🔴";
                    default:
                        return "🟡";
                }
            }

            public int GetPriorityLevel()
            {
                return (int)Priority;
            }

       
            public string GetDueDateString()
            {
                return DueDate.ToString("yyyy-MM-dd");
            }

            
            public string GetDueDateDisplay()
            {
                return DueDate.ToShortDateString();
            }

            
            public string GetReminderDateString()
            {
                return ReminderDate.HasValue ? ReminderDate.Value.ToString("yyyy-MM-dd") : "";
            }

           
            public string GetCreatedDateString()
            {
                return CreatedDate.ToString("yyyy-MM-dd HH:mm");
            }

           
            public int GetDaysUntilDue()
            {
                if (IsCompleted)
                    return 0;

                TimeSpan diff = DueDate - DateTime.Now;
                return (int)Math.Ceiling(diff.TotalDays);
            }

            
            public string GetUrgencyLevel()
            {
                if (IsCompleted)
                    return "Completed";

                int daysUntilDue = GetDaysUntilDue();

                if (daysUntilDue < 0)
                    return "🔴 Overdue";
                else if (daysUntilDue <= 2)
                    return "🔴 Urgent";
                else if (daysUntilDue <= 5)
                    return "🟠 Soon";
                else if (daysUntilDue <= 14)
                    return "🟡 Moderate";
                else
                    return "🟢 Not Urgent";
            }

            
            public string GetSummary()
            {
                return $"{GetStatusString()} | {Title} | {GetDueDateDisplay()} | {GetPriorityString()}";
            }

       
            public override string ToString()
            {
                return $"{GetStatusIcon()} {Title} - {GetDueDateDisplay()}";
            }

            public string GetStatusIcon()
            {
                if (IsCompleted)
                    return "✅";
                else if (IsReminderDue())
                    return "🔔";
                else if (IsOverdue())
                    return "⚠️";
                else
                    return "⏳";
            }

            
            public bool IsValid()
            {
                return !string.IsNullOrWhiteSpace(Title) && DueDate != DateTime.MinValue;
            }

         
            public CyberTask Clone()
            {
                return new CyberTask
                {
                    Id = this.Id,
                    Title = this.Title,
                    Description = this.Description,
                    DueDate = this.DueDate,
                    CreatedDate = this.CreatedDate,
                    ReminderDate = this.ReminderDate,
                    IsCompleted = this.IsCompleted,
                    Category = this.Category,
                    Priority = this.Priority,
                    UserName = this.UserName
                };
            }

          
            public override bool Equals(object obj)
            {
                if (obj is CyberTask other)
                {
                    return this.Id == other.Id &&
                           this.Title == other.Title &&
                           this.DueDate == other.DueDate;
                }
                return false;
            }

          
            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

           
            public static PriorityLevel ParsePriority(string priorityString)
            {
                if (string.IsNullOrWhiteSpace(priorityString))
                    return PriorityLevel.Medium;

                priorityString = priorityString.ToLower();

                if (priorityString.Contains("critical"))
                    return PriorityLevel.Critical;
                else if (priorityString.Contains("high"))
                    return PriorityLevel.High;
                else if (priorityString.Contains("low"))
                    return PriorityLevel.Low;
                else
                    return PriorityLevel.Medium;
            }

            public static PriorityLevel ParsePriority(int priorityInt)
            {
                if (Enum.IsDefined(typeof(PriorityLevel), priorityInt))
                    return (PriorityLevel)priorityInt;
                return PriorityLevel.Medium;
            }

           
            public static CyberTask FromDatabase(int id, string name, string description,
                                                string status, string dueDate,
                                                string username = "default_user")
            {
                DateTime parsedDueDate;
                if (!DateTime.TryParse(dueDate, out parsedDueDate))
                {
                    parsedDueDate = DateTime.Now.AddDays(7);
                }

                return new CyberTask
                {
                    Id = id,
                    Title = name ?? "Untitled Task",
                    Description = description ?? "",
                    DueDate = parsedDueDate,
                    IsCompleted = status?.ToLower() == "done",
                    UserName = username,
                    Category = "General",
                    Priority = PriorityLevel.Medium,
                    CreatedDate = DateTime.Now
                };
            }
        }
    }


