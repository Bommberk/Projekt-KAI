namespace Modules.OllamaAssistent;

class OllamaPromt
{
    public string getOllamaPrompt(string userInput, string[] methodList, string[] methodRequirements)
    {
        // return userInput;
        string methods = string.Join(", ", methodList);
        string requirements = string.Join("\n- ", methodRequirements);
        // return $@"
        //     {userInput}
        //     {requirements}
        // ";
        return $@"
            Du bist ein Systemassistent. Unten findest du eine Liste von Funktionen mit kurzen Beschreibungen, wann sie verwendet werden sollen:
            Hier ist die Liste mit allen Funktionen und deren verlangeten Parametern:
            {requirements}

            Hier sind beispiele auf was du hören kannst aber du sollst auch selber entscheiden ab wann eine Funktion aufgerufen werden soll und wann nicht.
            closeProgram(programName): Diese Funktion wird beispielsweise verwendet, um ein Programm zu schließen oder zu beenden – z. B. „schließe WhatsApp“, „beende Discord“, „mach Firefox zu“, „schalte Chrome aus“ usw.

            shutDownPc(seconds): Fahre den PC herunter und schau ob sekunden angegeben wurden. Wenn nicht schreib einfach null.

            ...

            Jetzt folgt eine Eingabe von Jim.

            Deine Aufgabe ist:
            - Prüfe, ob Jims Aussage den Wunsch ausdrückt, eine dieser Funktionen auszuführen.
            - Es ist egal, ob er den Funktionsnamen wörtlich nennt oder eine Umschreibung verwendet.
            - Erkenne die Bedeutung – auch wenn er andere Wörter oder Synonyme benutzt.
            - Achte darauf, ob die Bedeutung einem Funktionsaufruf entspricht.

            Wenn **ja**, antworte folgender maßen: JA - funktionsname: parameter. 
            (Falls mehrere Parameter angegeben werden zum Beispiel schließe Spotify in 30 sekunden, hänge einfach ein den Wert mit einem komma getrennt hinter dran. 
            Also ungefähr so 'JA - closeProgram: spotify, value: 30')
            Oder bei beispielsweise 'Öffne whatsapp in 5 Minuten':
            'JA - openProgram: WhtasApp, value: 300')
            
            Wenn **nein**, antworte nur mit **NEIN**.  
            Antworte **niemals** mit etwas anderem.

            Jim: {userInput}
        ";
    }
    public string getProgramOllamaPrompt(string ollamaAnswer)
    {
        // return ollamaAnswer;
        return $@"
            Kannst du folgendes in ein JSON format packen.
            Bachte dabei folgendes:
            1. Das'JA' ganz am Anfang MUSS entfernt werden da es nicht mit ins JSON gehört!
            2. Alle Bindestriche MÜSSEN entfernt werden!
            3. Ein Wert NULL sollte mit der Zahl 0 ersetzt werden!
            4. Bei mehreren Parametern benutze immer den key 'value'.
            Hier ist wichtig das du ausschließlich nur dieses JSON dann als Antwort gibts und nichts weiteres schreibst. 
            {ollamaAnswer}
        ";
    }
    public string getNormalOllamaPromt(string userInput)
    {
        return $@"
            Bitte schreibe immer auf deutsch.
            {userInput}
        ";
    }
}