# Cybersecurity_Awareness_Chatbot_Part2

## Overview

SecureBot is a Cybersecurity Awareness Chatbot developed using **C# WPF (.NET Framework 4.7.2)**.
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

# Project Structure

## Classes Used

| Class            | Purpose                   |
| ---------------- | ------------------------- |
| MainWindow       | Main GUI window           |
| chatbot          | Handles chatbot responses |
| responds         | Stores chatbot replies    |
| sentiment_detect | Detects user emotions     |
| memory_recall    | Handles saved memory      |
| user_name        | Username validation       |
| helper           | Cleans user input         |
| voice_greeting   | Plays welcome audio       |

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

# Author

Developed by: Funanani Nelitshindwe 

---
