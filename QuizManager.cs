using System;
using System.Collections.Generic;
using System.Linq;

namespace Cybersecurity_Awareness_Chatbot_Part2
{

        public class QuizManager
        {
            // ============ FIELDS ============
            private List<Question_in_quiz> questions;
            private int currentQuestionIndex;
            private int score;
            private bool isQuizActive;
            private bool isAnswered;
            private Random random;
            private ActivityLogger logger;
            private int totalQuestions;
            private string currentCategory;

            // ============ PROPERTIES ============
            public int CurrentQuestionIndex => currentQuestionIndex;
            public int Score => score;
            public int TotalQuestions => totalQuestions;
            public bool IsQuizActive => isQuizActive;
            public bool IsAnswered => isAnswered;
            public string CurrentCategory => currentCategory;
            public Question_in_quiz CurrentQuestion => GetCurrentQuestion();
            public double Percentage => totalQuestions > 0 ? (double)score / totalQuestions * 100 : 0;

            // ============ CONSTRUCTORS ============

            public QuizManager(ActivityLogger logger = null)
            {
                this.logger = logger;
                random = new Random();
                questions = new List<Question_in_quiz>();
                currentQuestionIndex = 0;
                score = 0;
                isQuizActive = false;
                isAnswered = false;
                totalQuestions = 0;
                currentCategory = string.Empty;

                logger?.AddLog("QuizManager initialized");
            }

            
            public QuizManager(int questionCount, ActivityLogger logger = null) : this(logger)
            {
                LoadQuestions(questionCount);
            }

            // ============ QUESTION LOADING ============

            
            public void LoadQuestions(int questionCount = 10)
            {
                try
                {
                    var loader = new Quiz_Question_Load();
                    questions = loader.GetRandomQuestions(questionCount);
                    totalQuestions = questions.Count;
                    currentQuestionIndex = 0;
                    score = 0;
                    isQuizActive = true;
                    isAnswered = false;

                    if (questions.Count > 0)
                    {
                        currentCategory = questions[0].Category;
                    }

                    logger?.AddLog($"Loaded {totalQuestions} quiz questions");
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Failed to load quiz questions: {ex.Message}", ex.StackTrace);
                    questions = new List<Question_in_quiz>();
                    totalQuestions = 0;
                    isQuizActive = false;
                }
            }

            /// <summary>
            /// Loads questions by category
            /// </summary>
            public void LoadQuestionsByCategory(string category, int questionCount = 10)
            {
                try
                {
                    var loader = new Quiz_Question_Load();
                    var allQuestions = loader.LoadAllQuestions();

                    // Filter by category
                    var filteredQuestions = allQuestions
                        .Where(q => q.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    // Shuffle and take the requested number
                    var shuffled = filteredQuestions.OrderBy(x => random.Next()).ToList();
                    questions = shuffled.Take(Math.Min(questionCount, shuffled.Count)).ToList();

                    totalQuestions = questions.Count;
                    currentQuestionIndex = 0;
                    score = 0;
                    isQuizActive = true;
                    isAnswered = false;

                    if (questions.Count > 0)
                    {
                        currentCategory = questions[0].Category;
                    }

                    logger?.AddLog($"Loaded {totalQuestions} quiz questions in category '{category}'");
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Failed to load questions by category: {ex.Message}", ex.StackTrace);
                    questions = new List<Question_in_quiz>();
                    totalQuestions = 0;
                    isQuizActive = false;
                }
            }

            /// <summary>
            /// Loads questions by difficulty
            /// </summary>
            public void LoadQuestionsByDifficulty(int difficulty, int questionCount = 10)
            {
                try
                {
                    var loader = new Quiz_Question_Load();
                    var allQuestions = loader.LoadAllQuestions();

                    // Filter by difficulty
                    var filteredQuestions = allQuestions
                        .Where(q => q.Difficulty == difficulty)
                        .ToList();

                    // Shuffle and take the requested number
                    var shuffled = filteredQuestions.OrderBy(x => random.Next()).ToList();
                    questions = shuffled.Take(Math.Min(questionCount, shuffled.Count)).ToList();

                    totalQuestions = questions.Count;
                    currentQuestionIndex = 0;
                    score = 0;
                    isQuizActive = true;
                    isAnswered = false;

                    if (questions.Count > 0)
                    {
                        currentCategory = questions[0].Category;
                    }

                    logger?.AddLog($"Loaded {totalQuestions} quiz questions with difficulty {difficulty}");
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Failed to load questions by difficulty: {ex.Message}", ex.StackTrace);
                    questions = new List<Question_in_quiz>();
                    totalQuestions = 0;
                    isQuizActive = false;
                }
            }

            // ============ QUIZ FLOW METHODS ============

            /// <summary>
            /// Gets the current question
            /// </summary>
            public Question_in_quiz GetCurrentQuestion()
            {
                if (!isQuizActive || currentQuestionIndex >= questions.Count)
                    return null;

                return questions[currentQuestionIndex];
            }

            /// <summary>
            /// Gets the current question's shuffled options
            /// </summary>
            public List<string> GetCurrentOptions()
            {
                var question = GetCurrentQuestion();
                if (question == null)
                    return new List<string>();

                return question.GetAllOptionsShuffled();
            }

            /// <summary>
            /// Checks if the user has answered the current question
            /// </summary>
            public bool HasAnswered()
            {
                return isAnswered;
            }

            /// <summary>
            /// Answers the current question
            /// </summary>
            public bool AnswerQuestion(string selectedAnswer)
            {
                if (!isQuizActive || isAnswered || currentQuestionIndex >= questions.Count)
                    return false;

                var question = questions[currentQuestionIndex];
                bool isCorrect = question.IsCorrect(selectedAnswer);

                if (isCorrect)
                {
                    score += 10; // 10 points per correct answer
                }

                isAnswered = true;

                logger?.LogQuizQuestionAnswered(currentQuestionIndex + 1, isCorrect);

                return isCorrect;
            }

            /// <summary>
            /// Gets the explanation for the current question
            /// </summary>
            public string GetExplanation()
            {
                var question = GetCurrentQuestion();
                if (question == null)
                    return string.Empty;

                return question.Explanation ?? "No explanation available.";
            }

            /// <summary>
            /// Gets the correct answer for the current question
            /// </summary>
            public string GetCorrectAnswer()
            {
                var question = GetCurrentQuestion();
                if (question == null)
                    return string.Empty;

                return question.CorrectAnswer;
            }

            /// <summary>
            /// Moves to the next question
            /// </summary>
            public bool NextQuestion()
            {
                if (!isAnswered)
                    return false;

                currentQuestionIndex++;
                isAnswered = false;

                // Update category if we have more questions
                if (currentQuestionIndex < questions.Count)
                {
                    currentCategory = questions[currentQuestionIndex].Category;
                }

                return true;
            }

            /// <summary>
            /// Checks if there are more questions
            /// </summary>
            public bool HasMoreQuestions()
            {
                return currentQuestionIndex < questions.Count;
            }

            /// <summary>
            /// Checks if the quiz is complete
            /// </summary>
            public bool IsQuizComplete()
            {
                return currentQuestionIndex >= questions.Count;
            }

            /// <summary>
            /// Gets the quiz progress as a percentage
            /// </summary>
            public double GetProgress()
            {
                if (totalQuestions == 0)
                    return 0;

                return (double)currentQuestionIndex / totalQuestions * 100;
            }

            /// <summary>
            /// Gets the user's feedback based on their score
            /// </summary>
            public string GetFeedback()
            {
                if (totalQuestions == 0)
                    return "No quiz completed.";

                double percentage = Percentage;

                if (percentage >= 90)
                    return "⭐ Exceptional! You're a cybersecurity expert! Keep up the great work!";
                else if (percentage >= 80)
                    return " Excellent! You have strong cybersecurity knowledge!";
                else if (percentage >= 70)
                    return " Good job! You're well on your way to becoming cybersecurity savvy!";
                else if (percentage >= 60)
                    return " Good effort! Keep learning to improve your cybersecurity knowledge!";
                else if (percentage >= 50)
                    return " You're making progress! Review the topics you missed to improve.";
                else
                    return " Keep learning! Cybersecurity is important. Review the explanations and try again!";
            }

            // ============ SCORING METHODS ============

            /// <summary>
            /// Gets the score as a formatted string
            /// </summary>
            public string GetScoreDisplay()
            {
                return $"⭐ Score: {score} points";
            }

            /// <summary>
            /// Gets the score with percentage
            /// </summary>
            public string GetDetailedScore()
            {
                return $"⭐ Score: {score} points ({Percentage:F1}%)";
            }

            /// <summary>
            /// Gets the number of correct answers
            /// </summary>
            public int GetCorrectCount()
            {
                return score / 10; // Each correct answer is worth 10 points
            }

            /// <summary>
            /// Gets the number of incorrect answers
            /// </summary>
            public int GetIncorrectCount()
            {
                return totalQuestions - GetCorrectCount();
            }

            /// <summary>
            /// Gets the score summary
            /// </summary>
            public string GetScoreSummary()
            {
                return $"Questions: {totalQuestions}\n" +
                       $"Correct: {GetCorrectCount()}\n" +
                       $"Incorrect: {GetIncorrectCount()}\n" +
                       $"Score: {score} points ({Percentage:F1}%)";
            }

            // ============ RESET METHODS ============

            /// <summary>
            /// Resets the quiz
            /// </summary>
            public void ResetQuiz()
            {
                currentQuestionIndex = 0;
                score = 0;
                isAnswered = false;
                isQuizActive = false;
                logger?.AddLog("Quiz reset");
            }

            /// <summary>
            /// Resets the quiz and loads new questions
            /// </summary>
            public void RestartQuiz(int questionCount = 10)
            {
                ResetQuiz();
                LoadQuestions(questionCount);
                logger?.AddLog("Quiz restarted");
            }

            // ============ STATISTICS METHODS ============

            /// <summary>
            /// Gets the category breakdown of questions
            /// </summary>
            public Dictionary<string, int> GetCategoryBreakdown()
            {
                var breakdown = new Dictionary<string, int>();

                foreach (var question in questions)
                {
                    if (breakdown.ContainsKey(question.Category))
                        breakdown[question.Category]++;
                    else
                        breakdown[question.Category] = 1;
                }

                return breakdown;
            }

            /// <summary>
            /// Gets the difficulty breakdown of questions
            /// </summary>
            public Dictionary<int, int> GetDifficultyBreakdown()
            {
                var breakdown = new Dictionary<int, int>();

                foreach (var question in questions)
                {
                    if (breakdown.ContainsKey(question.Difficulty))
                        breakdown[question.Difficulty]++;
                    else
                        breakdown[question.Difficulty] = 1;
                }

                return breakdown;
            }

            /// <summary>
            /// Gets a summary of the quiz
            /// </summary>
            public string GetQuizSummary()
            {
                if (!isQuizActive && totalQuestions == 0)
                    return "No quiz loaded.";

                var categoryBreakdown = GetCategoryBreakdown();
                var difficultyBreakdown = GetDifficultyBreakdown();

                string summary = $" Quiz Summary\n";
                summary += $"━━━━━━━━━━━━━━━━━━━━━━━\n";
                summary += $" Total Questions: {totalQuestions}\n";
                summary += $"⭐ Current Score: {score} points\n";
                summary += $" Progress: {GetProgress():F1}%\n\n";

                summary += " Categories:\n";
                foreach (var cat in categoryBreakdown)
                {
                    summary += $"   • {cat.Key}: {cat.Value} questions\n";
                }

                summary += "\n Difficulties:\n";
                foreach (var diff in difficultyBreakdown)
                {
                    string diffText = diff.Key == 1 ? "Easy" : diff.Key == 2 ? "Medium" : "Hard";
                    summary += $"   • {diffText}: {diff.Value} questions\n";
                }

                return summary;
            }
        }
    }

