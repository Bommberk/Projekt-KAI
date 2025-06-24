namespace Modules.WhisperSpeechToText;

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NAudio.Wave;

class WhisperProgram
{
    // Schwellenwerte für die automatische Aufnahme-Erkennung
    static readonly float StartThreshold = 0.02f;      // Start bei lautem Sprechen
    static readonly float ContinueThreshold = 0.005f;  // Weiter bei leisem Sprechen (aktuell nicht genutzt)
    static readonly float StopThreshold = 0.01f;       // Stoppen nur wenn sehr leise
    static readonly int SilenceDurationMs = 2000;      // 2 Sekunden Stille = Stop

    static WaveFileWriter? writer = null;
    static bool isRecording = false;
    static Stopwatch silenceTimer = new();

    // Hauptfunktion: Startet Aufnahme, erkennt Sprache, sendet an Whisper-Server und gibt Transkript zurück
    public static async Task<string> Run()
    {
        isRecording = false;
        silenceTimer.Reset();

        string wavPath = "assets/temp/stt_input.wav";
        string serverUrl = "http://127.0.0.1:8080";
        using var client = new HttpClient();
        var recordingFinished = new TaskCompletionSource();
        var whisperManager = new WhisperManager();

        Console.WriteLine("Sprich – ich starte/stopp automatisch...");

        var waveIn = new WaveInEvent
        {
            WaveFormat = new WaveFormat(16000, 1),
            BufferMilliseconds = 100,
            NumberOfBuffers = 3
        };

        writer = new WaveFileWriter(wavPath, waveIn.WaveFormat);

        waveIn.DataAvailable += (s, e) =>
        {
            float max = 0;
            for (int i = 0; i < e.BytesRecorded; i += 2)
            {
                short sample = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]);
                float sample32 = sample / 32768f;
                if (Math.Abs(sample32) > max) max = Math.Abs(sample32);
            }

            if (!isRecording && max > StartThreshold)
            {
                Console.WriteLine("🎙 Aufnahme gestartet...");
                isRecording = true;
                silenceTimer.Reset();
            }

            if (isRecording)
            {
                writer?.Write(e.Buffer, 0, e.BytesRecorded);
                if (max < StopThreshold)
                {
                    if (!silenceTimer.IsRunning)
                        silenceTimer.Start();
                    else if (silenceTimer.ElapsedMilliseconds > SilenceDurationMs)
                    {
                        Console.WriteLine("🛑 Aufnahme gestoppt.");
                        waveIn.StopRecording();
                        recordingFinished.SetResult();
                    }
                }
                else
                {
                    silenceTimer.Reset();
                }
            }
        };

        waveIn.StartRecording();
        await recordingFinished.Task;

        writer?.Flush();
        writer?.Dispose();
        waveIn.Dispose(); // <---- wichtig!

        string result = await whisperManager.SendAudioToServer(wavPath, serverUrl);
        return result;
    }
}
