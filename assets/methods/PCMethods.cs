using System.Diagnostics;
class PcMethods
{
    public void closeProgram(string programName)
    {
        if (!string.IsNullOrWhiteSpace(programName))
        {
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
        else
        {
            Console.WriteLine("Kein Parameter angegeben");
        }
    }
    public void startProgram(string programName)
    {
        if (!string.IsNullOrWhiteSpace(programName))
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
    }
    public void shutDownPc(int seconds)
    {
        // Process.Start("shutdown", $"/s /t {seconds}");
        Console.WriteLine("aösldkfj");
    }
    public void restartPc(int seconds)
    {
        // Process.Start("shutdown", $"/r /t {seconds}");
        Console.WriteLine("aösldkfj");
    }
    public void logOffPc(int seconds)
    {
        // Process.Start("shutdown", $"/l /t {seconds}");
        Console.WriteLine("aösldkfj");
    }
    public void lockPc()
    {
        Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
        Console.WriteLine("PC wurde gesperrt.");
    }
}