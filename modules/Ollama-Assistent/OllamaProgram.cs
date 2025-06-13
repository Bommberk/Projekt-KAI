namespace Modules.OllamaAssistent;

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Methods;

class OllamaProgram
{
    StartOllama startOllama = new StartOllama();
    InfoOverMethod infoOverMethod = new InfoOverMethod();
    
    public async Task Run(string? userInput)
    {
        // Console.WriteLine("🧠 OLLAMA CHAT");
        // Console.WriteLine("Gib eine Frage ein (oder 'exit' zum Beenden):");

        // Console.WriteLine(string.Join(", ",infoOverMethod.getListOfAllMethods()));
        // Console.WriteLine(infoOverMethod.getListOfAllMethods());

        // string answer = await startOllama.sendRequest(userInput);
        // Console.WriteLine($"\nOllama:\n {answer}");



        // return;


        if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == "exit" || userInput.ToLower() == "nun" || userInput.ToLower() == "tun")
            return;

        string output = await startOllama.sendRequest(userInput);
        Console.WriteLine($"\nOllama:\n {output}");
        // return;
        output = await startOllama.sendRequest(userInput, output);
        Console.WriteLine($"\nOllama:\n {output}");


        bool isJson = false;
        try
        {
            JsonDocument.Parse(output);
            isJson = true;
        }
        catch (JsonException)
        {
            isJson = false;
        }

        if (isJson)
        {
            var jsonOutput = JsonDocument.Parse(output);
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