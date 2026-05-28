using System;
using System.IO;

namespace Cybersecurity_Awareness_Chatbot_Part2
{//  start namespace
    public class memory_recall
    {//  start class
        // FILE NAME
        private string filename = "memory.txt";

        // SAVE USER INTEREST
        public void save_interest(string username, string topic)
        {
            try
            {
                // CHECK IF FILE EXISTS
                if (!File.Exists(filename))
                {
                    File.Create(filename).Close();
                }

                // READ ALL LINES
                string[] lines = File.ReadAllLines(filename);

                bool found = false;

                // CHECK IF USER ALREADY EXISTS
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith(username + ":"))
                    {
                        // UPDATE USER INTEREST
                        lines[i] = username + ":" + topic;

                        found = true;

                        break;
                    }
                }

                // SAVE UPDATED DATA
                File.WriteAllLines(filename, lines);

                // NEW USER
                if (!found)
                {
                    File.AppendAllText(
                        filename,
                        username + ":" + topic + Environment.NewLine);
                }
            }
            catch
            {

            }
        }

        // RECALL USER INTEREST
        public string recall_interest(string username)
        {
            try
            {
                if (File.Exists(filename))
                {
                    string[] lines = File.ReadAllLines(filename);

                    foreach (string line in lines)
                    {
                        if (line.StartsWith(username + ":"))
                        {
                            string topic =
                                line.Split(':')[1];

                            return topic;
                        }
                    }
                }
            }
            catch
            {

            }

            return "";
        }

    }// end class
}// end namespace   