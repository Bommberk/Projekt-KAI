namespace Methods.Spotify;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

public class SpotifyManager
{
    private readonly SpotifyAuthService _authService;
    private readonly HttpClient _httpClient;

    public SpotifyManager(SpotifyAuthService authService)
    {
        _authService = authService;
        _httpClient = new HttpClient();
    }

    // Hilfsmethode, um den HttpClient mit aktuellem Token zu konfigurieren
    private async Task SetAuthHeaderAsync()
    {
        var token = await _authService.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task PauseAsync()
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PutAsync("https://api.spotify.com/v1/me/player/pause", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task PlayAsync()
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PutAsync("https://api.spotify.com/v1/me/player/play", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task SkipNextAsync()
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PostAsync("https://api.spotify.com/v1/me/player/next", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task SkipPreviousAsync()
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PostAsync("https://api.spotify.com/v1/me/player/previous", null);
        response.EnsureSuccessStatusCode();
    }

    // Beispiel: Ein Song per Name (und optional Künstler) suchen und zufällig abspielen
    public async Task PlaySongByNameAsync(string title, string artist = null)
    {
        await SetAuthHeaderAsync();

        string query = $"track:{title}";
        if (!string.IsNullOrWhiteSpace(artist))
            query += $" artist:{artist}";

        var searchUrl = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit=50";

        var response = await _httpClient.GetAsync(searchUrl);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var tracks = doc.RootElement.GetProperty("tracks").GetProperty("items");

        // Liste für gefilterte Songs (Titel+Künstler eindeutig)
        var filtered = new List<(string id, string name, string artistName)>();
        var uniqueTracks = new HashSet<string>();

        foreach (var track in tracks.EnumerateArray())
        {
            string trackName = track.GetProperty("name").GetString();
            string firstArtist = track.GetProperty("artists")[0].GetProperty("name").GetString();
            string key = trackName.ToLower() + "|" + firstArtist.ToLower();

            if (!uniqueTracks.Contains(key))
            {
                uniqueTracks.Add(key);
                string trackId = track.GetProperty("id").GetString();
                filtered.Add((trackId, trackName, firstArtist));
            }
        }

        Console.WriteLine("Gefundene Songs:");
        foreach (var song in filtered)
            Console.WriteLine($"- {song.name} von {song.artistName}");

        if (filtered.Count == 0)
            throw new Exception("Kein Song gefunden.");

        // Zufälliges Lied abspielen
        var chosen = filtered.FirstOrDefault();

        // Play-Request mit track URI
        var playBody = new
        {
            uris = new[] { $"spotify:track:{chosen.id}" }
        };

        var content = new StringContent(JsonSerializer.Serialize(playBody));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var playResponse = await _httpClient.PutAsync("https://api.spotify.com/v1/me/player/play", content);
        playResponse.EnsureSuccessStatusCode();
    }

    // Beispiel: Alle Playlists (inklusive der, denen du folgst) auflisten
   public async Task ListAllPlaylistsAsync()
    {
        await SetAuthHeaderAsync();

        int limit = 50;
        int offset = 0;
        bool more = true;

        Console.WriteLine("Gefundene Playlists:");

        // 👉 Liked Songs als erste "Playlist" hinzufügen
        Console.WriteLine("Playlist: Lieblingssongs (Owner: Du selbst – gespeicherte Songs)");

        while (more)
        {
            var url = $"https://api.spotify.com/v1/me/playlists?limit={limit}&offset={offset}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var playlists = doc.RootElement.GetProperty("items");

            foreach (var playlist in playlists.EnumerateArray())
            {
                string name = playlist.GetProperty("name").GetString();
                string owner = playlist.GetProperty("owner").GetProperty("display_name").GetString();
                Console.WriteLine($"Playlist: {name} (Owner: {owner})");
            }

            int total = doc.RootElement.GetProperty("total").GetInt32();
            offset += limit;
            if (offset >= total)
                more = false;
        }
    }


    // Playlist per Namen suchen und abspielen
    public async Task PlayPlaylistByNameAsync(string playlistName)
    {
        await SetAuthHeaderAsync();

        // Sonderfall: Liked Songs (Lieblingssongs) abspielen
        if (string.Equals(playlistName, "Lieblingssongs", StringComparison.OrdinalIgnoreCase))
        {
            var playBody = new
            {
                context_uri = "spotify:collection:tracks"
            };

            var content = new StringContent(JsonSerializer.Serialize(playBody));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var playResponse = await _httpClient.PutAsync("https://api.spotify.com/v1/me/player/play", content);
            playResponse.EnsureSuccessStatusCode();

            Console.WriteLine("Lieblingssongs werden abgespielt.");
            return;
        }

        int limit = 50;
        int offset = 0;
        bool found = false;

        while (!found)
        {
            var url = $"https://api.spotify.com/v1/me/playlists?limit={limit}&offset={offset}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var playlists = doc.RootElement.GetProperty("items");

            foreach (var playlist in playlists.EnumerateArray())
            {
                string name = playlist.GetProperty("name").GetString();
                if (string.Equals(name, playlistName, StringComparison.OrdinalIgnoreCase))
                {
                    string uri = playlist.GetProperty("uri").GetString();

                    var playBody = new
                    {
                        context_uri = uri
                    };

                    var content = new StringContent(JsonSerializer.Serialize(playBody));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var playResponse = await _httpClient.PutAsync("https://api.spotify.com/v1/me/player/play", content);
                    playResponse.EnsureSuccessStatusCode();

                    Console.WriteLine($"Playlist '{playlistName}' wird abgespielt.");
                    found = true;
                    break;
                }
            }

            int total = doc.RootElement.GetProperty("total").GetInt32();
            offset += limit;
            if (offset >= total)
                break;
        }

        if (!found)
            Console.WriteLine($"Playlist '{playlistName}' nicht gefunden.");
    }
}
