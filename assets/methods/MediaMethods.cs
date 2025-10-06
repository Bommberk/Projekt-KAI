namespace Methods;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio;
using Modules.CoquiTextToSpeech;


class MediaMethods
{
    // Sprechen
    CoquiProgram tts = new CoquiProgram();

    GeneralPCInfosMethods PCInfosMethods = new GeneralPCInfosMethods();

    // Media Steuerung
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    // Virtuelle Tastencodes für Media-Tasten
    private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
    private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
    private const byte VK_MEDIA_PREV_TRACK = 0xB1;

    private const int KEYEVENTF_KEYDOWN = 0x0000; // Tastendruck
    private const int KEYEVENTF_KEYUP = 0x0002;   // Loslassen

    public async Task sendPlayPause()
    {
        if (new GeneralPCInfosMethods().IsMediaRunning())
        {
            await tts.Speak("Medium wird pausiert.");
        } else {
            await tts.Speak("Medium wird gestartet.");
        }

        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, 0);
    }
    public async Task sendNextTrack()
    {
        await tts.Speak("Nächstes Medium wird abgespielt.");
        keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYUP, 0);
    }
    public async Task sendPreviousTrack()
    {
        await tts.Speak("Vorheriges Medium wird abgespielt.");
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYUP, 0);
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYUP, 0);
    }
    public async Task sendRepeatTrack()
    {
        await tts.Speak("Medium wird wiederholt.");
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYUP, 0);
    }

    // Lautstärke ändern
    public async Task setVolume(int volume)
    {
        await tts.Speak($"Lautstärke wird auf {volume} Prozent gesetzt.");
        var defaultDevice = new CoreAudioController().DefaultPlaybackDevice;
        volume = Math.Max(0, Math.Min(100, volume));
        defaultDevice.Volume = volume;
    }
}