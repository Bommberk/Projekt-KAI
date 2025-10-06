namespace Methods.Spotify;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class SpotifyAuthService
{
    private const string clientId = "6f27c2b7d3064aca8140c2aaba53192f";
    private const string clientSecret = "6bb082b601e741e3829737af836dbb88";
    private const string redirectUri = "http://127.0.0.1:5000/callback/";
    private const string tokenFile = "assets/temp/spotify_token.txt";

    private string accessToken = string.Empty;
    private string refreshToken = string.Empty;
    private DateTime tokenExpiry;

    public async Task<string> GetAccessTokenAsync()
    {
        if (LoadTokenFromFile() && tokenExpiry > DateTime.UtcNow)
        {
            return accessToken;
        }

        if (!string.IsNullOrEmpty(refreshToken))
        {
            bool refreshed = await RefreshAccessTokenAsync();
            if (refreshed) return accessToken;
        }

        // Wenn kein gültiger oder auffrischbarer Token -> Browser öffnen
        string newToken = await StartAuthenticationFlowAsync();
        return newToken;
    }

    private async Task<string> StartAuthenticationFlowAsync()
    {
        string authUrl = $"https://accounts.spotify.com/authorize?response_type=code&client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope=user-read-playback-state user-modify-playback-state playlist-read-private playlist-read-collaborative user-library-read";

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = authUrl,
            UseShellExecute = true
        });

        string code = await WaitForCodeAsync();

        using var client = new HttpClient();
        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri)
        });

        var response = await client.SendAsync(request);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception("Fehler beim Token-Austausch: " + responseContent);

        var json = JsonDocument.Parse(responseContent).RootElement;
        accessToken = json.GetProperty("access_token").GetString() ?? string.Empty;
        refreshToken = json.GetProperty("refresh_token").GetString() ?? string.Empty;
        int expiresIn = json.GetProperty("expires_in").GetInt32();
        tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60);

        SaveTokenToFile();

        return accessToken;
    }

    private async Task<bool> RefreshAccessTokenAsync()
    {
        using var client = new HttpClient();
        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken)
        });

        var response = await client.SendAsync(request);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return false;

        var json = JsonDocument.Parse(responseContent).RootElement;
        accessToken = json.GetProperty("access_token").GetString() ?? string.Empty;
        int expiresIn = json.GetProperty("expires_in").GetInt32();
        tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60);

        SaveTokenToFile();
        return true;
    }

    private async Task<string> WaitForCodeAsync()
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add(redirectUri);
        listener.Start();

        Console.WriteLine("Warte auf Redirect von Spotify...");

        var context = await listener.GetContextAsync();
        var request = context.Request;
        string? code = request.QueryString["code"];

        string responseString = "<html><body><h1>Erfolgreich verbunden. Dieses Fenster kannst du jetzt schließen.</h1></body></html>";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        var response = context.Response;
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
        listener.Stop();

        return code ?? string.Empty;
    }

    private void SaveTokenToFile()
    {
        var lines = new[]
        {
            accessToken,
            refreshToken,
            tokenExpiry.ToString("o") // ISO 8601 Format
        };
        File.WriteAllLines(tokenFile, lines);
    }

    private bool LoadTokenFromFile()
    {
        if (!File.Exists(tokenFile)) return false;

        var lines = File.ReadAllLines(tokenFile);
        if (lines.Length < 3) return false;

        accessToken = lines[0];
        refreshToken = lines[1];
        if (!DateTime.TryParse(lines[2], null, System.Globalization.DateTimeStyles.RoundtripKind, out tokenExpiry))
            return false;

        return true;
    }
}
