using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Cybersecurity_Awareness_Chatbot_Part2
{
   public class user_name
    {
        // METHOD TO VALIDATE USERNAME
        // METHOD TO VALIDATE USERNAME
        public string submit_name(TextBox username_input)
        {
            // STORE USERNAME
            string username = username_input.Text.Trim();

            // CHECK IF EMPTY
            if (username == "")
            {
                return "";
            }

            // CHECK IF USER ENTERED ONLY NUMBERS
            if (Regex.IsMatch(username, @"^\d+$"))
            {
                return "";
            }

            // CHECK FOR SPECIAL CHARACTERS
            // ONLY LETTERS AND SPACES ALLOWED
            if (!Regex.IsMatch(username, @"^[a-zA-Z\s]+$"))
            {
                return "";
            }

            // RETURN VALID USERNAME
            return username;
        }
    }
}