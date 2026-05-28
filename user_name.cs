using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Cybersecurity_Awareness_Chatbot_Part2
{//  start namespace
    public class user_name
    {//  start class
        // FILE NAME
        string file = "users.txt";

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

            // CHECK NUMBERS ONLY
            if (Regex.IsMatch(username, @"^\d+$"))
            {
                return "";
            }

            // CHECK SPECIAL CHARACTERS
            if (!Regex.IsMatch(username, @"^[a-zA-Z\s]+$"))
            {
                return "";
            }

            // RETURN VALID USERNAME
            return username;
        }

        // SAVE USER
        public void save_user(string username)
        {
            // CREATE FILE IF NOT EXISTS
            if (!File.Exists(file))
            {
                File.Create(file).Close();
            }

            // READ USERS
            string[] users = File.ReadAllLines(file);

            // CHECK IF USER EXISTS
            bool found = false;

            foreach (string user in users)
            {
                if (user.ToLower() == username.ToLower())
                {
                    found = true;
                    break;
                }
            }

            // SAVE NEW USER
            if (!found)
            {
                File.AppendAllText(file, username + "\n");
            }
        }

        // CHECK IF USER EXISTS
        public bool user_exists(string username)
        {
            if (!File.Exists(file))
            {
                return false;
            }

            string[] users = File.ReadAllLines(file);

            foreach (string user in users)
            {
                if (user.ToLower() == username.ToLower())
                {
                    return true;
                }
            }

            return false;
        }//end

    }// end class
}//end namespace