using System.Collections;

namespace Cybersecurity_Awareness_Chatbot_Part2
{
    public class responds
    {
        // RESPONSE ARRAY
        public ArrayList replies = new ArrayList();

        // IGNORE WORDS
        public ArrayList ignore = new ArrayList();

        // CONSTRUCTOR
        public responds()
        {
            // PASSWORD RESPONSES
            replies.Add("Use strong passwords with symbols and numbers.");
            replies.Add("Avoid using personal information in passwords.");
            replies.Add("Enable two-factor authentication for extra protection.");
            replies.Add("Use different passwords for different accounts.");

            // PHISHING RESPONSES
            replies.Add("Do not click suspicious links in emails.");
            replies.Add("Scammers often pretend to be trusted companies.");
            replies.Add("Always verify email senders carefully.");
            replies.Add("Avoid downloading unknown attachments.");

            // PRIVACY RESPONSES
            replies.Add("Review your privacy settings regularly.");
            replies.Add("Avoid oversharing personal information online.");
            replies.Add("Use secure websites with HTTPS.");
            replies.Add("Protect your accounts with strong passwords.");

            // MALWARE RESPONSES
            replies.Add("Install trusted antivirus software.");
            replies.Add("Keep your software updated.");
            replies.Add("Avoid downloading files from unknown websites.");
            replies.Add("Scan USB devices before opening files.");

            // FIREWALL RESPONSES
            replies.Add("A firewall helps block unauthorized access.");
            replies.Add("Firewalls improve network security.");
            replies.Add("Always keep your firewall enabled.");

            // CYBER HYGIENE
            replies.Add("Practice safe online behaviour.");
            replies.Add("Be careful when using public Wi-Fi.");
            replies.Add("Backup important files regularly.");

            // IGNORE WORDS
            ignore.Add("what");
            ignore.Add("is");
            ignore.Add("how");
            ignore.Add("why");
            ignore.Add("about");
            ignore.Add("can");
            ignore.Add("does");
            ignore.Add("the");
            ignore.Add("a");
            ignore.Add("an");
        }
    }
}