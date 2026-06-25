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
    {//  start class


        // FIELDS 
        private string username = string.Empty;

        // Part 2 Components
        private responds data;
        private helper clean;
        private user_name user;
        private sentiment_detect sentiment;
        private memory_recall memory;
        private chatbot bot;
        private ActivityLogger logger;

        // Part 3 Components
        private TaskManager taskManager;
        private QuizManager quizManager;

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





        public MainWindow()
        {
            InitializeComponent();

            // chatbot engine
            bot = new chatbot(data.replies, data.ignore);

            // play voice
            voice_greeting greet = new voice_greeting();
            greet.greet();
        }

        private void start(object sender, RoutedEventArgs e)
        {
            //Hide home page grid and set Username grid visible
            home_grid.Visibility = Visibility.Hidden;
            username_grid.Visibility = Visibility.Visible;
        }


        //submit name  event handler

        private void submit_name(object sender, RoutedEventArgs e)
        {
            // validate username input
            if(string.IsNullOrWhiteSpace(usernames_input.Text))
            {
                MessageBox.Show("Please enter a valid username.", "Invalid Username", MessageBoxButton.OK, MessageBoxImage.Warning);
                usernames_input.Focus();
                return;
            }
            // GET USERNAME
            username = user.submit_name(usernames_input);

            // VALID USERNAME
            if (username != "")
            {
                // CHECK IF USER EXISTS
                bool oldUser = user.user_exists(username);

                // SAVE USER
                user.save_user(username);

                // HIDE USERNAME GRID
                username_grid.Visibility = Visibility.Hidden;

                // SHOW CHAT GRID
                chat_grid.Visibility = Visibility.Visible;

                // WELCOME MESSAGE
                if (oldUser)
                {
                    txtChat.AppendText(
                        "SecureBot: Welcome back " + username + "! Great to see you again.\n\n");
                }
                else
                {
                    txtChat.AppendText(
                        "SecureBot: Nice to meet you, " +username +"! Ask me anything about cybersecurity.\n\n");
                }
            }
            else
            {
                MessageBox.Show(
                    "Please enter a valid username."
                );
            }
        }
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string userInput = txtUserInput.Text.Trim();

            if (userInput == "")
            {
                MessageBox.Show("Please enter a message.");
                return;
            }

            // First message = username
            if (username == "")
            {
                username = userInput;

                txtChat.AppendText("You: " + username + "\n");

                txtChat.AppendText(
                    "SecureBot: Nice to meet you, "
                    + username +
                    "! Ask me anything about cybersecurity.\n\n");

                txtUserInput.Clear();
                return;
            }

            // Display user message
            txtChat.AppendText("You: " + userInput + "\n");

            // DETECT SENTIMENT
            string feeling = sentiment.detect_sentiment(userInput);

            // SHOW SENTIMENT
            if (feeling != "neutral")
            {
                string sentimentResponse = sentiment.sentiment_response(feeling);

                txtChat.AppendText("SecureBot: " + sentimentResponse + "\n\n");
            }
            // if no sentiment is found 
            if (feeling == "neutral")
            {
                // Get chatbot response
                string response = bot.GetResponse(userInput, username);

                txtChat.AppendText("SecureBot: " + response + "\n\n");
            }
        }
        // DISPLAY CHAT METHOD
        private void error_method(string name, string message)
        {
            Border messageBorder = new Border();

            messageBorder.Margin = new Thickness(5);

            messageBorder.Padding = new Thickness(5);

            messageBorder.CornerRadius = new CornerRadius(5);

            TextBlock text = new TextBlock();

            text.TextWrapping = TextWrapping.Wrap;

            text.Inlines.Add(new Run
            {
                Text = name + ": ",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue
            });

            text.Inlines.Add(new Run
            {
                Text = message,
                Foreground = Brushes.Black
            });

            messageBorder.Child = text;

            txtChat.AppendText("SecureBot: " + message + "\n\n");

            txtChat.ScrollToEnd();
        }

    }// end class
}// end namespace
