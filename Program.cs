using System;
using System.Threading.Tasks;
using Modules.OllamaAssistent;
using Modules.VoskSpeechToText;

class Program
{
    // OllamaProgram ollamaProgram = new OllamaProgram();
    static async Task Main()
    {
        while (true)
        {
            Console.Write("\nDu: ");
            string? userInput = Console.ReadLine();
            await new OllamaProgram().Run(userInput);
        }
        // var voskProgram = new VoskProgram();
        // voskProgram.Run();

        

    }
}