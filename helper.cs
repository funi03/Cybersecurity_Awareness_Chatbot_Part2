using System;
using System.Text.RegularExpressions;

namespace Cybersecurity_Awareness_Chatbot_Part2
{
    internal class helper
    {
        // REMOVE SPECIAL CHARACTERS
        public string clean_text(string input)
        {
            // CHECK EMPTY
            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }

            // LOWERCASE
            input = input.ToLower();

            // REMOVE SPECIAL CHARACTERS
            input = Regex.Replace(input,
                @"[^a-zA-Z0-9\s]",
                "");

            // REMOVE EXTRA SPACES
            input = Regex.Replace(input,
                @"\s+",
                " ").Trim();

            return input;
        }

        // CHECK IF INPUT IS EMPTY
        public bool is_empty(string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        // FORMAT BOT MESSAGE
        public string bot_format(string name,
                                 string message)
        {
            return name + ": " + message;
        }

        // RANDOM RESPONSE
        public string random_response(string[] replies)
        {
            Random random = new Random();

            int index =
                random.Next(replies.Length);

            return replies[index];
        }
    }
}