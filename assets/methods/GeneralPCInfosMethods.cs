namespace Methods;

using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio;
using Modules.CoquiTextToSpeech;

class GeneralPCInfosMethods
{
    // Sprechen
    CoquiProgram tts = new CoquiProgram();
    // Welche Lautstärke
    public void getCurrentVolume()
    {
        var defaultDevice = new CoreAudioController().DefaultPlaybackDevice;
        int currentVolume = (int)defaultDevice.Volume;
        tts.Speak($"Die aktuelle Lautstärke beträgt {currentVolume} Prozent.");
    }
    // Welches Media läuft
    // Welche Programme laufen
    // Welche Hardware ist verbaut
    // IP Config ausgabe
}