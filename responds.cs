using System.Collections;

namespace Cybersecurity_Awareness_Chatbot_Part2
{
    public class responds
    {
        // RESPONSE ARRAY
        public ArrayList replies = new ArrayList();
        public ArrayList add_answers = new ArrayList();

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

            //sentiment detection

            add_answers.Add("frustrated i understand you're frustrated. let's work through the issue step by step.");
            add_answers.Add("frustrated it's okay to feel frustrated when things aren't working. i'm here to help.");
            add_answers.Add("frustrated take a breath, we'll fix this together.");


            add_answers.Add("confused that's okay, confusion is normal. i'll explain it clearly for you.");
            add_answers.Add("confused let me break it down step by step so it makes sense.");
            add_answers.Add("confused no worries, i'll help you understand it better.");


            add_answers.Add("worried it's okay to feel worried. i'm here to help you stay safe online.");
            add_answers.Add("worried don't panic, most cybersecurity issues can be fixed quickly.");
            add_answers.Add("worried i understand your concern. let's make sure your information is safe.");


            add_answers.Add("happy that's great to hear! i'm glad things are going well.");
            add_answers.Add("happy awesome! positivity is always good.");
            add_answers.Add("happy i'm happy for you! let me know if you need anything.");


            add_answers.Add("sad i'm sorry you're feeling this way. i'm here for you.");
            add_answers.Add("sad that sounds tough, take things one step at a time.");
            add_answers.Add("sad i hope things improve soon. you can talk to me anytime.");


            add_answers.Add("angry i understand you're angry. let's try solve the issue together.");
            add_answers.Add("angry it's okay to feel angry, but i'll help you fix the problem.");
            add_answers.Add("angry take your time, i'm here to help you sort it out.");


        }
    }
}