namespace Modules.CoquiTextToSpeech;

using System;
using System.Threading.Tasks;

class CoquiProgram
{
    TTSManager tts = new TTSManager();
    public async Task StartTtsServer()
    {
        tts.StartTtsServer();

        Console.WriteLine("Warte auf Serverstart...");
        await tts.WaitUntilServerReadyAsync();
    }

    public async Task StopItsServer()
    {
        tts.StopTtsServer();
        Console.WriteLine("Server gestoppt.");
    }

    public async Task Speak(string text)
    {
        await tts.Speak(text);
    }

    // public static async Task Main(string? text = null)
    // {

    //     if (text == null)
    //         return;

    //     await tts.Speak(text);
    // }
}
