namespace Modules.WhisperSpeechToText;

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NAudio.Wave;

class WhisperManager
{
    // Prüfe, ob der Whisper-Server läuft, und starte ihn ggf.
    public async Task StartWhisperServer(string modelPath = "whisper.cpp/models/ggml-base.bin", string language = "de", string port = "8080")
    {
        if (Process.GetProcessesByName("whisper-server").Length == 0)
        {
            Console.WriteLine("Starte Whisper-Server...");
            var serverProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "whisper.cpp/build/bin/Release/whisper-server.exe",
                    Arguments = $"-m \"{modelPath}\" -l {language} --port {port}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            serverProcess.Start();
            await Task.Delay(3000);
        }
        else
        {
            Console.WriteLine("Whisper-Server läuft bereits.");
        }
    }
    // Sende Audiodatei an Server
    public async Task<string> SendAudioToServer(string wavPath, string serverUrl)
    {
        using var client = new HttpClient();
        try
        {
            Console.WriteLine("Sende an Whisper-Server...");
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(File.OpenRead(wavPath)), "file", Path.GetFileName(wavPath));
            content.Add(new StringContent("0.0"), "temperature");
            content.Add(new StringContent("0.2"), "temperature_inc");
            content.Add(new StringContent("text"), "response_format");

            // Sende Audiodatei an Whisper-Server zur Transkription
            var response = await client.PostAsync($"{serverUrl}/inference", content);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("\n📝 Transkription:\n" + result);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine("Fehler: " + e.Message);
        }
        return null;
    }
}