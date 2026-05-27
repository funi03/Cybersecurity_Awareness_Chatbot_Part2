using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Cybersecurity_Awareness_Chatbot_Part2
{
   public class user_name
    {
        // METHOD TO VALIDATE USERNAME
        public string submit_name(TextBox username_input)
        {
            // GET USERNAME
            string username = username_input.Text.Trim();

            // CHECK EMPTY
            if (username == "")
            {
                return "";
            }

            // CHECK IF NAME CONTAINS ONLY NUMBERS
            if (Regex.IsMatch(username, @"^\d+$"))
            {
                return "";
            }

            // CHECK FOR SPECIAL CHARACTERS
            if (!Regex.IsMatch(username, @"^[a-zA-Z\s]+$"))
            {
                return "";
            }

            // RETURN VALID NAME
            return username;
        }
    }
}