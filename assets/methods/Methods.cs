namespace Methods;

using System.Diagnostics;
using Spotify;
using Modules.CoquiTextToSpeech;
using System.Threading.Tasks;

public class Methods
{
    // Sprechen 
    CoquiProgram tts = new CoquiProgram();

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
    public void repeatMedia() { mediaMethods.sendRepeatTrack(); }
    public async Task setVolume(object volume)
    {
        try
        {
            int vol = Convert.ToInt32(volume);
            mediaMethods.setVolume(vol);
        }
        catch (Exception e)
        {
            await tts.Speak($"Fehler beim Sezten der Lautstärke: {e.Message}");
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