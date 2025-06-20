namespace Methods;

using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio;
using NameTextToSpeech;

class MediaMethods
{
    // Textausgabe
    NameProgram textToSpeech = new NameProgram();

    // Media Steuerung
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    // Virtuelle Tastencodes für Media-Tasten
    private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
    private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
    private const byte VK_MEDIA_PREV_TRACK = 0xB1;

    private const int KEYEVENTF_KEYDOWN = 0x0000; // Tastendruck
    private const int KEYEVENTF_KEYUP = 0x0002;   // Loslassen

    public void sendPlayPause()
    {
        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, 0);
    }
    public void sendNextTrack()
    {
        keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYUP, 0);
    }
    public void sendPreviousTrack()
    {
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYUP, 0);
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYUP, 0);
        textToSpeech.Run("Vorheriger Track wird abgespielt");
    }

    // Lautstärke ändern
    public void setVolume(int volume)
    {
        var defaultDevice = new CoreAudioController().DefaultPlaybackDevice;
        volume = Math.Max(0, Math.Min(100, volume));
        defaultDevice.Volume = volume;
        textToSpeech.Run($"Lautstärke wurde auf {volume}% gesetzt");
    }
}