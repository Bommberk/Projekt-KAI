namespace Modules.CoquiTextToSpeech;

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NAudio.Wave;

public class TTSManager
{
    private Process? ttsProcess;
    private readonly HttpClient httpClient = new();

    public void StartTtsServer()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/C tts-server --model_name \"tts_models/de/thorsten/vits\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            ttsProcess = new Process { StartInfo = startInfo };
            ttsProcess.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Fehler beim Starten des TTS-Servers: {e.Message}");
            // throw;
        }
    }

    public void StopTtsServer()
    {
        if (ttsProcess != null && !ttsProcess.HasExited)
        {
            ttsProcess.Kill();
            ttsProcess.Dispose();
        }
    }

    public async Task WaitUntilServerReadyAsync(int timeoutMs = 100000)
    {
        var startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
        {
            try
            {
                var response = await httpClient.GetAsync("http://localhost:5002");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("TTS-Server ist bereit!");
                    return;
                }
            }
            catch
            {
                // Server noch nicht erreichbar, weitermachen
            }

            await Task.Delay(500);
        }

        throw new Exception("TTS-Server wurde nicht rechtzeitig bereit.");
    }

    public async Task Speak(string text)
    {
        string url = $"http://localhost:5002/api/tts?text={Uri.EscapeDataString(text)}";
        string outputPath = "assets/temp/tts_output.wav";

        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var audioBytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(outputPath, audioBytes);

            // NAudio: WAV abspielen
            using var audioFile = new AudioFileReader(outputPath);
            using var outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);
            outputDevice.Play();

            // Warten, bis fertig gespielt
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(100);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler bei TTS: {ex.Message}");
        }
        CoquiProgram.isSpeaking = false; // TTS abgeschlossen
    }
}
