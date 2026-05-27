using System;
using System.Collections;

namespace Cybersecurity_Awareness_Chatbot_Part2
{
   public class chatbot
    {
        // ARRAYS
        private ArrayList replies;
        private ArrayList ignore;

        // MEMORY
        private string lastTopic = "";

        // RANDOM
        Random random = new Random();

        // CONSTRUCTOR
        public chatbot(ArrayList reply, ArrayList ignoring)
        {
            replies = reply;
            ignore = ignoring;
        }

        // MAIN CHATBOT METHOD
        public string GetResponse(string question, string username)
        {
            // LOWERCASE
            question = question.ToLower();

            // FOLLOW-UP QUESTIONS
            if (question.Contains("tell me more") ||
                question.Contains("another tip") ||
                question.Contains("explain more"))
            {
                return ContinueTopic();
            }

            // PASSWORD
            if (question.Contains("password"))
            {
                lastTopic = "password";

                string[] passwordTips =
                {
                    "Use strong passwords with symbols and numbers.",
                    "Avoid using your name in passwords.",
                    "Use different passwords for each account.",
                    "Enable two-factor authentication for extra security."
                };

                return passwordTips[random.Next(passwordTips.Length)];
            }

            // PHISHING
            else if (question.Contains("phishing") ||
                     question.Contains("scam"))
            {
                lastTopic = "phishing";

                string[] phishingTips =
                {
                    "Do not click suspicious email links.",
                    "Scammers pretend to be trusted companies.",
                    "Always verify email senders carefully.",
                    "Never share personal information through unknown links."
                };

                return phishingTips[random.Next(phishingTips.Length)];
            }

            // PRIVACY
            else if (question.Contains("privacy"))
            {
                lastTopic = "privacy";

                string[] privacyTips =
                {
                    "Review your privacy settings regularly.",
                    "Avoid sharing sensitive information online.",
                    "Use secure websites with HTTPS.",
                    "Protect your accounts with strong passwords."
                };

                return privacyTips[random.Next(privacyTips.Length)];
            }

            // MALWARE
            else if (question.Contains("malware") ||
                     question.Contains("virus"))
            {
                lastTopic = "malware";

                string[] malwareTips =
                {
                    "Install antivirus software.",
                    "Avoid downloading unknown files.",
                    "Keep your software updated.",
                    "Scan USB devices before opening them."
                };

                return malwareTips[random.Next(malwareTips.Length)];
            }

            // FIREWALL
            else if (question.Contains("firewall"))
            {
                lastTopic = "firewall";

                return "A firewall helps block unauthorized access to your network.";
            }

            // UNKNOWN INPUT
            return "I'm not sure I understand. Please ask about cybersecurity topics.";
        }

        // CONTINUE CONVERSATION FLOW
        private string ContinueTopic()
        {
            if (lastTopic == "password")
            {
                string[] morePassword =
                {
                    "Never share your password with anyone.",
                    "Change passwords regularly.",
                    "Password managers can help store passwords safely."
                };

                return morePassword[random.Next(morePassword.Length)];
            }

            else if (lastTopic == "phishing")
            {
                string[] morePhishing =
                {
                    "Be careful of urgent emails asking for money.",
                    "Check spelling mistakes in suspicious emails.",
                    "Avoid opening attachments from strangers."
                };

                return morePhishing[random.Next(morePhishing.Length)];
            }

            else if (lastTopic == "privacy")
            {
                string[] morePrivacy =
                {
                    "Limit what you share on social media.",
                    "Review app permissions carefully.",
                    "Use private browsing on shared devices."
                };

                return morePrivacy[random.Next(morePrivacy.Length)];
            }

            else if (lastTopic == "malware")
            {
                string[] moreMalware =
                {
                    "Keep your operating system updated.",
                    "Avoid cracked software downloads.",
                    "Use trusted antivirus tools."
                };

                return moreMalware[random.Next(moreMalware.Length)];
            }

            return "Please ask about a cybersecurity topic first.";
        }
    }
}