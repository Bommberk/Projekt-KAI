# Projekt KAI
## A .NET Project where you can communicate with an AI using your voice and execute commands.
## Features
- Speech to Text
- Recognize Commands with Ollama AI
- Execute Methods and Functions
- Or just talk to the AI
- Text-to-Speech

## How to Use
1. Install .NET
2. Setup a new .NET project  
    `dotnet new console -n ProjektKAI`
3. Clone this repositoy  
    `cd ProjektKAI`  
    `git clone https://github.com/Bommberk/Projekt-KAI.git`
4. Copy the files from the cloned repository to your project folder and delete the Repository folder:
    - Copy the files from the `Projekt-KAI` folder to your project folder
    - Delete the `Projekt-KAI` folder
5. Add the required packages to your project

## Required Packages
1. NAudio  
    `dotnet add package NAudio`
2. Vosk  
    `dotnet add package Vosk`
3. AudioSwitcher.AudioApi.CoreAudio  
    `dotnet add package AudioSwitcher.AudioApi.CoreAudio`

## To Install
1. Python 3.11.8  
    [Download Link](https://www.python.org/downloads/release/python-3118/)
     1. pip
          1. tts