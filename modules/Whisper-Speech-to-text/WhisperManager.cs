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
    static WaveInEvent? waveIn = null;
    static bool isRecording = false;
    static WaveFileWriter? writer = null;
    static Process? serverProcess = null;
    static string wavPath = "assets/temp/stt_input.wav";
    // Prüfe, ob der Whisper-Server läuft, und starte ihn ggf.
    public async Task StartWhisperServer(string modelPath = "modules/Whisper-Speech-to-text/whisper.cpp/models/ggml-base.bin", string language = "de", string port = "8080")
    {
        if (Process.GetProcessesByName("whisper-server").Length == 0)
        {
            Console.WriteLine("Starte Whisper-Server...");
            serverProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "modules/Whisper-Speech-to-text/whisper.cpp/build/bin/Release/whisper-server.exe",
                    Arguments = $"-m \"{modelPath}\" -l {language} --port {port} --no-context",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            try
            {
                serverProcess.Start();
                Console.WriteLine($"Erfolgreich gestartet");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Fehler beim starten von Whisperserver: {e}");
            }
            await Task.Delay(3000);
        }
        else
        {
            Console.WriteLine("Whisper-Server läuft bereits.");
        }
    }
    // Sende Audiodatei an Server
    public async Task<string> SendAudioToServer(string serverUrl = "http://127.0.0.1:8080")
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
        return string.Empty;
    }

    // Server stoppen
    public void StopWhisperServer()
    {
        if (serverProcess != null && !serverProcess.HasExited)
        {
            serverProcess.Kill();
            serverProcess.WaitForExit();
            serverProcess.Dispose();
            serverProcess = null;
            Console.WriteLine("Whisper-Server wurde gestoppt.");
        }
        else
        {
            // Fallback, falls man den Prozess nicht gespeichert hat
            foreach (var p in Process.GetProcessesByName("whisper-server"))
            {
                p.Kill();
            }
            Console.WriteLine("Whisper-Server-Prozesse beendet.");
        }
    }

    // Startet die Audioaufnahme
    public void StartRecording()
    {
        if (isRecording) return; // Bereits am Aufnehmen

        Console.WriteLine("🎙 Aufnahme gestartet...");
        isRecording = true;
        waveIn = new WaveInEvent
        {
            WaveFormat = new WaveFormat(16000, 1),
            BufferMilliseconds = 100,
            NumberOfBuffers = 3
        };

        writer = new WaveFileWriter(wavPath, waveIn.WaveFormat);

        waveIn.DataAvailable += (s, e) =>
        {
            if (isRecording && writer != null)
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
            }
        };

        waveIn.StartRecording();
    }

    // Stoppt die Audioaufnahme
    public void StopRecording()
    {
        if (!isRecording) return; // Bereits gestoppt

        Console.WriteLine("🛑 Aufnahme gestoppt.");
        isRecording = false;

        waveIn?.StopRecording();
        writer?.Flush();
        writer?.Dispose();
        waveIn?.Dispose();
        
        writer = null;
        waveIn = null;
    }

    // Überprüft, ob gerade aufgenommen wird
    public static bool IsRecording()
    {
        return isRecording;
    }
}