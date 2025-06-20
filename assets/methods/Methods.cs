namespace Methods;

using System.Diagnostics;
using Spotify;
using NameTextToSpeech;

public class Methods
{
    // Textausgabe
    NameProgram textToSpeech = new NameProgram();

    // PC Methods
    PcMethods pcMethods = new PcMethods();
    public void closeProgram(string programName) { pcMethods.closeProgram(programName); }
    public void startProgram(string programName) { pcMethods.startProgram(programName); }
    public void shutDownPc() { pcMethods.shutDownPc(0); }

    // Media Steuerung
    MediaMethods mediaMethods = new MediaMethods();
    public void playOrPause() { mediaMethods.sendPlayPause(); }
    public void nextMedia() { mediaMethods.sendNextTrack(); }
    public void previousMedia() { mediaMethods.sendPreviousTrack(); }
    public void setVolume(object volume)
    {
        try{
            int vol = Convert.ToInt32(volume);
            mediaMethods.setVolume(vol);
        }catch (Exception e)
        {
            textToSpeech.Run($"Fehler: {e.Message}");
        }
    }

    // Spotify Steuerung
    SpotifyMethods spotifyMethods = new SpotifyMethods();

    // Allgemeine PC Infos
    GeneralPCInfosMethods generalPCInfos = new GeneralPCInfosMethods();
    public void getCurrentVolume() { generalPCInfos.getCurrentVolume(); }

    // Test Methoden
    public void TestFunction(string parameter)
    {
        Console.WriteLine($"getestett {parameter}");
    }
}