using System.Diagnostics;
class PcMethods
{
    public void closeProgram(string programName)
    {
        return;
        var processes = Process.GetProcessesByName(programName);
        foreach (var p in processes)
        {
            try
            {
                p.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Schließen von {programName}: {ex.Message}");
            }
        }
    }
    public void startProgram(string programName)
    {
        var processes = Process.GetProcessesByName(programName);
        foreach (var p in processes)
        {
            try
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"{programName}:") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Öffnen von {programName}: {ex.Message}");
            }
        }
    }
    public void shutDownPc(int seconds)
    {
        // Process.Start("shutdown", $"/s /t {seconds}");
        Console.WriteLine("aösldkfj");
    }
}