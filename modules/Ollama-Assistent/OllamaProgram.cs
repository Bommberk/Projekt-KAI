namespace Modules.OllamaAssistent;

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

        // return;

        if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == "exit" || userInput.ToLower() == "nun" || userInput.ToLower() == "tun")
            return;

        string output = await startOllama.sendRequest(userInput);
        // Console.WriteLine($"\nOllama:\n {output}");
        // return;
        output = await startOllama.sendRequest(userInput, output);


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
            using (JsonDocument doc = JsonDocument.Parse(output))
            {
                foreach (JsonProperty property in doc.RootElement.EnumerateObject())
                {
                    Console.WriteLine($"Key: {property.Name}");
                    Console.WriteLine($"Value: {property.Value}");
                }
            }
        }
        // if (output == "NEIN")
        // {
        //     output = await startOllama.sendRequest(userInput, output);
        // }
        Console.WriteLine($"\nOllama:\n {output}");
    }
}