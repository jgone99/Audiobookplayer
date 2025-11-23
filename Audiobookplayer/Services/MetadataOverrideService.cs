using Audiobookplayer.Models;
using System.Text.Json;

namespace Audiobookplayer.Services
{
    public static class MetadataOverrideService
    {
        private const string FileName = "metadata_overrides.json";
        private static readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);
        private static Dictionary<string, MetadataOverride> cache;

        static MetadataOverrideService()
        {
            try
            {
                var json = File.ReadAllText(filePath);
                cache = JsonSerializer.Deserialize<Dictionary<string, MetadataOverride>>(json) ?? new();
            }
            catch
            {
                cache = new();
            }
        }

        public static MetadataOverride? GetOverrides(string bookId)
            => cache.TryGetValue(bookId, out var o) ? o : null;

        public static void SaveOverrides(string bookId, MetadataOverride overrides)
        {
            cache[bookId] = overrides;
            SaveToDisk();
        }

        public static void DeleteOverrides(string bookId)
        {
            cache.Remove(bookId);
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
