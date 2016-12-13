# CSCE4444

This is the repository for the CSCE4444 group 8 project

## Members
Andrew Thomas

Michael Thomas

Paul Gerard

Daniel Morgan

Zac Taylor

Narimon Kardani

## Basic rundown
This is a hub-like application (Windows desktop) that will integrate various social media platforms (i.e., Facebook, Twitter, Twitch) and calendars (i.e., Outlook, Google), as well as allowing the user to create actionable, context-aware notifications and reminders.

The application should give the user notifications from all integrated platforms that the user can then save for later (create a reminder), dismiss, or immediately act upon.

## How to Compile/Run Weamy
The main project is located in the UIPrototype folder. To successfully build and run this project, you might need to separately compile two additional DLL's. 

First, compile the WeamyNotifications project. This should build the DLL necessary for the notifications in the main project. 

Next, compile the TwitchCSharp project in the TwitchCSharp-master folder. This will build the DLL necessary for the twitch API to work. 

Finally, compile and run the main project in the UIPrototype folder. If the project fails to build, you may need to re-include the references for the DLL's compiled in the previous steps. Once everything is properly referenced, and all of the DLL's are compiled, the project should compile and work perfectly.
