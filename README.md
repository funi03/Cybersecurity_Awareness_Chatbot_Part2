# CybersecurityAwarenessChatbotPOE3

##PART2

## Overview

The chatbot was designed to help users learn about cybersecurity topics in an interactive and user-friendly way.

The application includes:

* A graphical user interface (GUI)
* Voice greeting
* Cybersecurity tips
* Sentiment detection
* Memory recall
* Dynamic chatbot responses
* Keyword recognition

---

# Features

## GUI Interface

* Modern WPF graphical interface
* Cybersecurity-themed design
* Interactive chat system
* User-friendly layout with colours and spacing

---

## Voice Greeting

SecureBot plays a welcome voice greeting when the application starts.

---

## Keyword Recognition

The chatbot recognises cybersecurity keywords such as:

* Password
* Phishing
* Scam
* Privacy
* Malware
* Antivirus
* Firewall

The bot responds with relevant cybersecurity awareness tips.

---

## Random Responses

SecureBot uses lists and collections to provide random responses for some topics, making conversations more dynamic and realistic.

---

## Sentiment Detection

The chatbot detects user emotions such as:

* Happy
* Worried
* Curious
* Frustrated

The bot responds with supportive and engaging messages.

Example:
User: *"I am worried about scams"*
Bot: *"It is understandable to feel worried. Cyber threats are common, but I can help you stay safe online."*

---

## Memory Recall

SecureBot remembers returning users by saving usernames to a text file.

Example:

* First login: *"Nice to meet you, Funie!"*
* Returning login: *"Welcome back Funie!"*

The chatbot can also remember user interests.

---

## Error Handling

The application handles:

* Empty inputs
* Invalid usernames
* Unknown chatbot questions
* Unexpected user input

This prevents crashes and improves user experience.

---

# Technologies Used

* C#
* WPF (.NET Framework 4.7.2)
* XAML
* Object-Oriented Programming (OOP)
* Lists and Arrays
* File Handling
* Regular Expressions (Regex)

---


---

# How to Run the Program

1. Open the project in Visual Studio
2. Build the solution
3. Run the application
4. Enter your username
5. Start chatting with SecureBot

---

# Example Questions

* Tell me about password safety
* What is phishing?
* How do I stay safe online?
* I am worried about scams
* Give me cybersecurity tips

---

# Educational Purpose

This project was created for educational purposes as part of a Cybersecurity Awareness Chatbot assignment.

---

# Youtube link
https://youtube.com/shorts/olQCV1FxqpY?feature=share

---

# Commit Screenshot

<img width="1348" height="635" alt="Screenshot 2026-05-28 233349" src="https://github.com/user-attachments/assets/d0127257-f97b-4100-bbe6-24bb7784d524" />

<img width="1348" height="635" alt="Screenshot 2026-05-28 233422" src="https://github.com/user-attachments/assets/5e9b0007-ca11-4a45-9257-87052eb13596" />

---

##PART3

##  Project Overview

**SECUREBOT** is a comprehensive Cybersecurity Awareness Chatbot designed to educate users about online safety and cybersecurity best practices. The application provides an interactive experience where users can learn about various cybersecurity topics, manage security-related tasks, and test their knowledge through quizzes.

### Purpose

In recent years, South Africa has seen a significant rise in cyberattacks targeting individuals, businesses, and government institutions (Pieterse, 2021). These attacks often include phishing scams, malware, and social engineering, leaving many people vulnerable to financial loss, identity theft, and psychological harm.

This chatbot serves as a **"Cybersecurity Awareness Assistant"** that:
- Simulates real-life cybersecurity scenarios
- Provides guidance on avoiding common online traps
- Covers topics like phishing emails, safe password practices, and recognising suspicious links
- Helps users manage cybersecurity tasks with reminders
- Tests user knowledge through interactive quizzes

### Module Context

This project was developed as part of the **PROG6221 Programming 2A** module at **The Independent Institute of Education (IIE)** .

---

##  Background & Context

### The Cyber Threat Landscape in South Africa

South Africa has experienced a significant increase in cyber threats over the past decade. According to research by Pieterse (2021), the country faces numerous cybersecurity challenges including:

- **Phishing Scams**: Fraudulent attempts to obtain sensitive information
- **Malware Attacks**: Malicious software designed to damage or disable computers
- **Social Engineering**: Psychological manipulation to trick users
- **Identity Theft**: Stealing personal information for fraudulent purposes

### Project Response

The Department of Cybersecurity launched a campaign aimed at educating citizens on identifying and mitigating cyber threats. To support this campaign, this chatbot was developed as a virtual assistant that interacts with users to educate them on cybersecurity topics in a conversational manner.

---

##  Features

###  Chat Functionality

| Feature | Description |
|---------|-------------|
| **Voice Greeting** | Audio welcome message on application start |
| **Personalised Conversations** | Uses user's name throughout the chat |
| **Sentiment Detection** | Detects user mood (worried, curious, frustrated, happy, sad) |
| **Memory Recall** | Remembers user interests and previous conversations |
| **AI-Powered Responses** | Intelligent responses to cybersecurity questions |
| **Follow-up Questions** | Handles "tell me more", "another tip", "explain more" |
| **Rich Chat Display** | Formatted messages with colors and styling |
| **Interest Tracking** | Automatically remembers topics you're interested in |
| **Clear Chat** | Option to clear all chat messages |

###  Task Management

| Feature | Description |
|---------|-------------|
| **Add Tasks** | Create cybersecurity-related tasks |
| **Priority Levels** | Low, Medium, High, Critical |
| **Due Dates** | Set deadlines for tasks |
| **Reminders** | Set reminders with custom days |
| **Complete Tasks** | Mark tasks as completed |
| **Delete Tasks** | Remove unwanted tasks |
| **Database Storage** | SQL Server database for persistence |
| **Color Coding** | Visual priority indicators |

### Cybersecurity Quiz

| Feature | Description |
|---------|-------------|
| **10+ Questions** | Comprehensive cybersecurity knowledge test |
| **Multiple Categories** | Password, Phishing, Privacy, Browsing, Malware, Network, Security |
| **Difficulty Levels** | Easy, Medium, Hard questions |
| **Immediate Feedback** | Correct/incorrect with explanations |
| **Score Tracking** | Track your progress and final score |
| **Randomized Questions** | Different questions each time |
| **Category Display** | Shows the topic category |
| **Progress Indicator** | Shows current question number |

### Activity Log

| Feature | Description |
|---------|-------------|
| **Action Tracking** | Records all user interactions |
| **Timestamps** | Each action is time-stamped |
| **Log Viewing** | View recent activity log |
| **Clear Logs** | Option to clear activity history |
| **Error Logging** | Captures and displays errors |
| **Export** | Logs saved to file |

###  User Interface

| Feature | Description |
|---------|-------------|
| **Dark Theme** | Modern dark design with cyan accents |
| **Responsive Layout** | Clean, organized interface |
| **Easy Navigation** | Quick access to all features |
| **Professional Design** | Consistent styling throughout |
| **Animated Effects** | Smooth transitions |
| **Accessibility** | High contrast text |

---

## Technologies Used

### Programming Languages & Frameworks

| Technology | Version | Purpose |
|------------|---------|---------|
| **C#** | .NET Framework 4.8+ | Core programming language |
| **WPF** | - | Windows Presentation Foundation UI Framework |
| **XAML** | - | User Interface Design |
| **SQL** | T-SQL | Database queries and management |

### Databases

| Technology | Purpose |
|------------|---------|
| **SQL Server LocalDB** | Local database for task storage |
| **SQL Server Express** | Alternative database option |
| **Text Files** | User data, memory, and logs storage |

### Libraries & Packages

| Package | Purpose |
|---------|---------|
| **System.Data.SqlClient** | Database connectivity |
| **Microsoft.VisualBasic** | Input dialog helper |
| **System.Media** | Voice greeting playback |
| **System.Collections** | Data structures (ArrayList, etc.) |

### Version Control

| Tool | Purpose |
|------|---------|
| **Git** | Version control system |
| **GitHub** | Repository hosting |
| **GitHub Actions** | Continuous Integration |

 Requirements

### Software Requirements

| Software | Version | Notes |
|----------|---------|-------|
| **Windows** | 10/11 | Required for WPF application |
| **Visual Studio** | 2022 or later | With .NET Desktop Development |
| **SQL Server** | LocalDB or Express | Database storage |
| **.NET Framework** | 4.8 or later | Runtime environment |
| **Git** | Latest | For version control |

### Hardware Requirements

| Component | Minimum | Recommended |
|-----------|---------|-------------|
| **Processor** | 1.5 GHz | 2.0 GHz+ |
| **RAM** | 2 GB | 4 GB+ |
| **Storage** | 200 MB | 500 MB+ |
| **Display** | 1024x768 | 1920x1080 |

### NuGet Packages Required

xml
<PackageReference Include="System.Data.SqlClient" Version="4.8.5" />
<PackageReference Include="Microsoft.VisualBasic" Version="10.3.0" />

# Author

Developed by: Funanani Nelitshindwe

---

