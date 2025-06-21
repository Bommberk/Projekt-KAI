namespace Modules.OllamaAssistent;

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Methods;
using Modules.CoquiTextToSpeech;

class OllamaProgram
{
    StartOllama startOllama = new StartOllama();
    InfoOverMethod infoOverMethod = new InfoOverMethod();
    CoquiProgram ttsProgram = new CoquiProgram();
    
    public async Task Run(string? userInput)
    {
        // var ttsProgram = new CoquiProgram();

        if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == "exit" || userInput.ToLower() == "nun" || userInput.ToLower() == "tun" || userInput.ToLower() == "einen")
            return;

        string output1 = await startOllama.sendRequest(userInput);
        Console.WriteLine($"\nOllama:\n {output1}");
        string output2 = await startOllama.sendRequest(userInput, output1);
        if (output1 == "NEIN")
        {
            try
            {
                // Console.WriteLine($"\nOllama:\n {output2}");
                string ollamaOutput = output2.ToString();
                await ttsProgram.Speak(ollamaOutput);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Fehler beim Sprechen: {e.Message}");
            }
        }
        else
        {
            Console.WriteLine($"\nOllama:\n {output2}");
        }


        bool isJson = false;
        try
        {
            JsonDocument.Parse(output2);
            isJson = true;
        }
        catch (JsonException)
        {
            isJson = false;
        }

        if (isJson)
        {
            var jsonOutput = JsonDocument.Parse(output2);
            JsonElement root = jsonOutput.RootElement;
            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in root.EnumerateObject())
                {
                    string methodName = property.Name;
                    string parameter = property.Value.ToString();
                    infoOverMethod.callMethod(methodName, parameter);
                }
            }
        }
    }
}