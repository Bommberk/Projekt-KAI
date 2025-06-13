namespace Methods;

using System.Diagnostics;
using Spotify;

public class Methods
{
    // PC Methods
    PcMethods pcMethods = new PcMethods();
    public void closeProgram(string programName) { pcMethods.closeProgram(programName); }
    public void startProgram(string programName) { pcMethods.startProgram(programName); }
    public void shutDownPc(int seconds = 0) { pcMethods.shutDownPc(seconds); }

    // Media Steuerung
    public void playOrPause() { MediaMethods.SendPlayPause(); }
    public void nextMedia() { MediaMethods.SendNextTrack(); }
    public void previousMedia() { MediaMethods.SendPreviousTrack(); }

    // Spotify Steuerung
    SpotifyMethods spotifyMethods = new SpotifyMethods();

    // Allgemeine PC Steuerung

    // Test Methoden
    public void TestFunction(string parameter)
    {
        Console.WriteLine($"getestett {parameter}");
    }
}