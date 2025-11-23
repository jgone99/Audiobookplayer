
using Android.App;
using Android.Content;
using Android.Media;
using Android.Provider;
using Audiobookplayer.Models;
using Java.IO;
using Application = Android.App.Application;
using Uri = Android.Net.Uri;

namespace Audiobookplayer.Platforms.Android
{
    public static class AndroidHelpers
    {
        public static List<Uri> GetChildDocumentUris(Uri treeUri)
        {
            var context = Application.Context;
            var resolver = context.ContentResolver;

            var docId = DocumentsContract.GetDocumentId(treeUri);
            var childrenUri = DocumentsContract.BuildChildDocumentsUriUsingTree(treeUri, docId);

            var uris = new List<Uri>();

            using var cursor = resolver.Query(childrenUri,
                new[] { DocumentsContract.Document.ColumnDocumentId, DocumentsContract.Document.ColumnMimeType, DocumentsContract.Document.ColumnDisplayName },
                null, null, null);

            while (cursor?.MoveToNext() == true)
            {
                var childDocId = cursor.GetString(0);

                uris.Add(DocumentsContract.BuildDocumentUriUsingTree(treeUri, childDocId));
            }

            return uris;
        }

        

        public static async Task<List<Audiobook>> LoadAudiobooksFromUriAsync(Uri libraryUri)
        {
            List<Uri> sub_directories = new List<Uri>();
            var context = Application.Context;
            var resolver = context.ContentResolver;

            var audiobooks = new List<Audiobook>();

            var treeId = DocumentsContract.GetTreeDocumentId(libraryUri);
            var rootDocUri = DocumentsContract.BuildDocumentUriUsingTree(libraryUri, treeId);

            sub_directories.Add(rootDocUri);

            while (sub_directories.Count > 0)
            {
                var currentDir = sub_directories[0];
                sub_directories.RemoveAt(0);
                var children = GetChildDocumentUris(currentDir);

                string title = "";
                string author = "";
                string narrator = "";
                TimeSpan duration = TimeSpan.Zero;
                string filePath = "";
                ImageSource coverImage = null;

                try
                {
                    foreach (var fileUri in children)
                    {
                        using var cursor = resolver.Query(fileUri, null, null, null, null);
                        cursor?.MoveToFirst();
                        var name = cursor?.GetString(cursor.GetColumnIndex(DocumentsContract.Document.ColumnDisplayName)) ?? "";

                        if (name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                        {
                            coverImage = await MainThread.InvokeOnMainThreadAsync(() =>
                                ImageSource.FromStream(() => resolver.OpenInputStream(fileUri)));
                            break;
                        }
                    }

                    foreach (var fileUri in children)
                    {
                        var docType = resolver.GetType(fileUri);
                        if (docType == DocumentsContract.Document.MimeTypeDir)
                        {
                            var childDocUri = DocumentsContract.BuildDocumentUriUsingTree(
                            rootDocUri,
                            DocumentsContract.GetDocumentId(fileUri));

                            sub_directories.Add(childDocUri);
                            continue;
                        }
                        using var cursor = resolver.Query(fileUri, null, null, null, null);
                        cursor?.MoveToFirst();
                        var name = cursor?.GetString(cursor.GetColumnIndex(DocumentsContract.Document.ColumnDisplayName)) ?? "";

                        if (name.EndsWith(".m4b", StringComparison.OrdinalIgnoreCase))
                        {
                            var retriever = new MediaMetadataRetriever();
                            retriever.SetDataSource(context, fileUri);

                            title = retriever.ExtractMetadata(MetadataKey.Title) ?? "Unknown";
                            author = retriever.ExtractMetadata(MetadataKey.Artist) ?? "Unknown";
                            narrator = retriever.ExtractMetadata(MetadataKey.Writer) ?? "Unknown";
                            duration = TimeSpan.FromMilliseconds(long.Parse(retriever.ExtractMetadata(MetadataKey.Duration) ?? "0"));
                            filePath = fileUri.ToString();

                            audiobooks.Add(new Audiobook
                            {
                                Title = title,
                                Author = author,
                                Narrator = narrator,
                                Duration = duration,
                                FilePath = filePath,
                                CoverImage = coverImage
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error loading audiobooks: {ex.Message}");
                }
            }
            
            return audiobooks;
        }

        internal static string? GetDisplayPath(string? libraryUriString)
        {
            if (libraryUriString == null) 
                return string.Empty;
            var libraryUri = Uri.Parse(libraryUriString);
            if (libraryUri == null)
                return string.Empty;
            var id = DocumentsContract.GetTreeDocumentId(libraryUri);
            string? displayName = GenarateDisplayNameFromId(id);
            return displayName;
        }

        private static string? GenarateDisplayNameFromId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return id;
            string[] parts = id.Split(':', 2);
            string rootName = parts[0] switch
            {
                "primary" => "Internal Storage/",
                "home" => "Internal Storage/Documents/",
                "downloads" => "Internal Storage/Downloads/",
                _ => parts[0] + '/'
            };

            return rootName + parts[1];
        }

        public static System.IO.Stream OpenInputStream(string uriString)
        {
            var context = Application.Context;
            var resolver = context.ContentResolver;
            var uri = Uri.Parse(uriString);
            return resolver.OpenInputStream(uri);
        }

        public static void DeleteFile(Uri uri)
        {
            var context = Application.Context;
            var resolver = context.ContentResolver;

            try
            {
                DocumentsContract.DeleteDocument(resolver, uri);
            }
            catch (Exception ex)
            {
                throw new System.IO.IOException("Failed to delete file", ex);
            }
        }
    }
}