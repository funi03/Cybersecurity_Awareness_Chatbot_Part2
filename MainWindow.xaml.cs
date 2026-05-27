using System;
using System.Collections;
using System.Collections.Generic;
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
{
   
    public partial class MainWindow : Window
    {
       

        // variables
        string username = string.Empty;
       

        // Objects classes
        responds data = new responds();

        helper clean = new helper();

        user_name user = new user_name();

        sentiment_detect sentiment = new sentiment_detect();

        memory_recall memory = new memory_recall();

        chatbot bot;
         
        
       
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
            username = user.submit_name(usernames_input);

            if (username != "")
            {
                username_grid.Visibility = Visibility.Hidden;

                chat_grid.Visibility = Visibility.Visible;

                error_method("ChatBot",
                    "Welcome " + username +
                    "! Ask me anything about cybersecurity.");
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
            if (feeling != "")
            {
                string sentimentResponse = sentiment.sentiment_response(feeling);
                txtChat.AppendText("SecureBot: " + sentimentResponse + "\n\n");
            }

            // Get chatbot response
            string response = bot.GetResponse(userInput, username);

            txtChat.AppendText("SecureBot: " + response + "\n\n");
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

    }
}
