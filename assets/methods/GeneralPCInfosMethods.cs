namespace Methods;

using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio;
using Modules.CoquiTextToSpeech;
using NAudio.CoreAudioApi;

class GeneralPCInfosMethods
{
    // Sprechen
    CoquiProgram tts = new CoquiProgram();
    // Welche Lautstärke
    public async Task getCurrentVolume()
    {
        var defaultDevice = new CoreAudioController().DefaultPlaybackDevice;
        int currentVolume = (int)defaultDevice.Volume;
        await tts.Speak($"Die aktuelle Lautstärke beträgt {currentVolume} Prozent.");
    }
    // Läuft Medium?
    public bool IsMediaRunning()
    {
        var enumerator = new MMDeviceEnumerator();
        var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        var sessionManager = device.AudioSessionManager;
        var sessions = sessionManager.Sessions;
        
        for (int i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];
            float peak = session.AudioMeterInformation.MasterPeakValue;

            if (peak > 0.01f)
            {
                return true;
            }
        }
        return false;
    }

    // Welches Media läuft
    public void getCurrentMedia()
    {
        
    }
    // Welche Programme laufen
    // Welche Hardware ist verbaut
    // IP Config ausgabe
}