namespace Methods;

using System.Diagnostics;
using Spotify;

public class Methods
{
    // PC Methods
    PcMethods pcMethods = new PcMethods();
    public void closeProgram(string programName) { pcMethods.closeProgram(programName); }
    public void startProgram(string programName, int seconds = 0) { pcMethods.startProgram(programName); }
    public void shutDownPc(int seconds = 0) { pcMethods.shutDownPc(seconds); }

    // Media Steuerung
    MediaMethods mediaMethods = new MediaMethods();

    // Spotify Steuerung
    SpotifyMethods spotifyMethods = new SpotifyMethods();

    // Allgemeine PC Steuerung
}