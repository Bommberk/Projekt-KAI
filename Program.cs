using System;
using System.Threading.Tasks;
using Modules.OllamaAssistent;
using Modules.VoskSpeechToText;
using Modules.CoquiTextToSpeech;
using Modules.WhisperSpeechToText;

class Program
{
    // OllamaProgram ollamaProgram = new OllamaProgram();
    static async Task Main()
    {
        // Stimme starten
        var ttsProgram = new CoquiProgram();
        await ttsProgram.StartTtsServer();

        // Programm Start
        var whisperManager = new WhisperManager();
        string modelPath = "whisper.cpp/models/ggml-medium.bin";
        await whisperManager.StartWhisperServer(modelPath);

        while (true)
        {
            string output = await WhisperProgram.Run();
            await new OllamaProgram().Run(output);
        }



        AppDomain.CurrentDomain.ProcessExit += async (sender, e) =>
        {
            await ttsProgram.StopItsServer();
        };

    }
}