using System.Collections;

namespace Cybersecurity_Awareness_Chatbot_Part2
{
    public class sentiment_detect
    {
     
        // METHOD TO DETECT SENTIMENT
        public string detect_sentiment(string message)
        {
            // LOWERCASE MESSAGE
            message = message.ToLower();

            // WORRIED
            if (message.Contains("worried") ||
                message.Contains("scared") ||
                message.Contains("afraid"))
            {
                return "worried";
            }

            // FRUSTRATED
            else if (message.Contains("frustrated") ||
                     message.Contains("angry") ||
                     message.Contains("annoyed"))
            {
                return "frustrated";
            }

            // CURIOUS
            else if (message.Contains("curious") ||
                     message.Contains("interested") ||
                     message.Contains("learn"))
            {
                return "curious";
            }

            // HAPPY
            else if (message.Contains("happy") ||
                     message.Contains("good") ||
                     message.Contains("great"))
            {
                return "happy";
            }

            // NO SENTIMENT FOUND
            return "neutral";
        }

        // METHOD TO RETURN RESPONSE
        public string sentiment_response(string feeling)
        {

            // WORRIED RESPONSE
            if (feeling == "worried")
            {
                return "It is understandable to feel worried. Cyber threats are common, but I can help you stay safe online.";
            }

            // FRUSTRATED RESPONSE
            else if (feeling == "frustrated")
            {
                return "I understand your frustration. Cybersecurity can feel overwhelming, but small safety steps make a big difference.";
            }

            // CURIOUS RESPONSE
            else if (feeling == "curious")
            {
                return "That is great! Learning about cybersecurity helps you stay protected online.";
            }

            // HAPPY RESPONSE
            else if (feeling == "happy")
            {
                return "I am glad you are feeling positive about cybersecurity awareness!";
            }

            // DEFAULT RESPONSE
            return "";
        }



    }
}