
using Audiobookplayer.Models;
using System.Net;
using System.Text.Json;

namespace Audiobookplayer.Services
{
    public static class PlaybackProgressService
    {
        private const string FileName = "playback_progress.json";
        private static readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);
        private static Dictionary<string, PlaybackProgress> cache;

        static PlaybackProgressService()
        {
            try
            {
                var json = File.ReadAllText(filePath);
                cache = JsonSerializer.Deserialize<Dictionary<string, PlaybackProgress>>(json) ?? new();
            }
            catch
            {
                cache = new();
            }
        }

        public static PlaybackProgress? GetProgress(string audiobookId) 
            => cache.TryGetValue(audiobookId, out var o) ? o : null;

        public static void Save(string audiobookId, PlaybackProgress progress)
        {
            cache[audiobookId] = progress;
            SaveToDisk();
        }

        private static void SaveToDisk()
        {
            var json = JsonSerializer.Serialize(cache, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }
    }
}
