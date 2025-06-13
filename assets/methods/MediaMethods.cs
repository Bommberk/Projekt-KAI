namespace Methods;

using System.Runtime.InteropServices;

class MediaMethods
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    // Virtuelle Tastencodes für Media-Tasten
    private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
    private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
    private const byte VK_MEDIA_PREV_TRACK = 0xB1;

    private const int KEYEVENTF_KEYDOWN = 0x0000; // Tastendruck
    private const int KEYEVENTF_KEYUP = 0x0002;   // Loslassen

    public static void SendPlayPause()
    {
        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, 0);
    }

    public static void SendNextTrack()
    {
        keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYUP, 0);
    }

    public static void SendPreviousTrack()
    {
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYUP, 0);
    }
}