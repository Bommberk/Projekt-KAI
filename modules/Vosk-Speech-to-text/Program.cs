namespace Modules.VoskSpeechToText;

using System.Diagnostics;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using Vosk;
using System.Text.Json;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Threading.Tasks.Dataflow;
using Modules.OllamaAssistent;
using Modules.WhisperSpeechToText;
using System.Net;
using Modules.CoquiTextToSpeech;

// Wie oben, aber mit showRecordings & showSentences Unterstützung

class VoskProgram
{
    private WaveInEvent waveIn = null!;
    private VoskRecognizer recognizer = null!;
    private Model model = null!;
    private WhisperManager whisperManager = new WhisperManager();
    private OllamaProgram ollamaProgram = new OllamaProgram();

    public async Task Run()
    {
        Vosk.SetLogLevel(0);
        string modelPath = "assets/vosk-models/vosk-model-small-de-0.15";

        if (!Directory.Exists(modelPath))
        {
            Console.WriteLine("Modell nicht gefunden: " + modelPath);
            return;
        }

        model = new Model(modelPath);
        recognizer = new VoskRecognizer(model, 16000.0f);

        waveIn = new WaveInEvent
        {
            DeviceNumber = 0,
            WaveFormat = new WaveFormat(16000, 1)
        };

        waveIn.DataAvailable += async (s, a) =>
        {
            if (CoquiProgram.isSpeaking)
            {
                return;
            }
            if (recognizer.AcceptWaveform(a.Buffer, a.BytesRecorded))
            {
                string result = recognizer.Result();
                string text = ExtractText(result);
                int wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                if (wordCount < 2)
                {
                    Console.WriteLine("❗ Kein richtigen Satz erkannt, bitte lauter sprechen.");
                    whisperManager.StopRecording();
                    return;
                }
                else
                {
                    whisperManager.StopRecording();
                    result = await whisperManager.SendAudioToServer();
                    await ollamaProgram.Run(result);
                }
            }
            else
            {
                string partial = recognizer.PartialResult();
                if (ExtractPartial(partial).Length > 0)
                {
                    try
                    {
                        whisperManager.StartRecording();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Fehler beim Starten der Aufnahme: " + ex.Message);
                    }
                }
            }
        };

        waveIn.StartRecording();
        Console.WriteLine("🎙️ Sprich jetzt. Beende mit [Enter].");
        Console.ReadLine();
        waveIn.StopRecording();
        waveIn.Dispose();
        recognizer.Dispose();
        model.Dispose();
    }

    private string ExtractText(string jsonResult)
    {
        using var doc = JsonDocument.Parse(jsonResult);
        return doc.RootElement.GetProperty("text").GetString() ?? "";
    }

    private string ExtractPartial(string jsonResult)
    {
        using var doc = JsonDocument.Parse(jsonResult);
        return doc.RootElement.GetProperty("partial").GetString() ?? "";
    }
}