using System;
using System.Threading.Tasks;
using Modules.OllamaAssistent;
using Modules.VoskSpeechToText;
using Modules.CoquiTextToSpeech;

class Program
{
    // OllamaProgram ollamaProgram = new OllamaProgram();
    static async Task Main()
    {
        // Stimme starten
        var ttsProgram = new CoquiProgram();
        await ttsProgram.StartTtsServer();

        while (true)
        {
            Console.Write("\nDu: ");
            string? userInput = Console.ReadLine();
            await new OllamaProgram().Run(userInput);
        }

        // var voskProgram = new VoskProgram();
        // voskProgram.Run();

        AppDomain.CurrentDomain.ProcessExit += async (sender, e) =>
        {
            await ttsProgram.StopItsServer();
        };

    }
}