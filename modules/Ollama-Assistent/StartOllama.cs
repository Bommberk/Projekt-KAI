namespace Modules.OllamaAssistent;

using Methods;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class StartOllama
{
    OllamaPromt ollamaPromt = new OllamaPromt();
    InfoOverMethod infoOverMethod = new InfoOverMethod();

    public async Task<string> sendRequest(string userInput, string ollamaAnswer = null)
    {
        using var client = new HttpClient();
        const string url = "http://localhost:11434/api/generate";
        const string model = "llama3";


        // string myPrompt = ollamaPromt.getWhichProgramPromt(userInput, infoOverMethod.getListOfAllMethods());

        string myPrompt;
        if (ollamaAnswer == null)
        {
            myPrompt = ollamaPromt.getOllamaPrompt(userInput, infoOverMethod.getListOfAllMethods(), infoOverMethod.getMethodRequirement());
        }
        else if (ollamaAnswer == "NEIN")
        {
            myPrompt = ollamaPromt.getNormalOllamaPromt(userInput);
        }
        else
        {
            myPrompt = ollamaPromt.getProgramOllamaPrompt(ollamaAnswer);
        }

        var requestData = new
        {
            model,
            prompt = myPrompt,
            stream = false
        };

        string json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);
        string resultJson = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(resultJson);
        string output = doc.RootElement.GetProperty("response").GetString();
        output = output.Replace(". ", "." + Environment.NewLine);
        return output;
    }
}