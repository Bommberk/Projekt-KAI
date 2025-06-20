namespace Methods;

using AudioSwitcher.AudioApi.CoreAudio;
using NameTextToSpeech;

class GeneralPCInfosMethods
{
    NameProgram textToSpeech = new NameProgram();
    // Welche Lautstärke
    public void getCurrentVolume()
    {
        var defaultDevice = new CoreAudioController().DefaultPlaybackDevice;
        int currentVolume = (int)defaultDevice.Volume;
        textToSpeech.Run($"Die aktuelle Lautstärke beträgt {currentVolume}%");
    }
    // Welches Media läuft
    // Welche Programme laufen
    // Welche Hardware ist verbaut
    // IP Config ausgabe
}