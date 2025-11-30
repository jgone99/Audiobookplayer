using AndroidX.Media3.Common;

namespace Audiobookplayer.Platforms.Android
{
    public class CustomPlayerListener : Java.Lang.Object, IPlayerListener
    {
        public event Action<bool>? IsPlayingChanged;
        public void OnMediaMetadataChanged(MediaMetadata? mediaMetadata)
        {
            string title = mediaMetadata?.Title?.ToString() ?? string.Empty;
            string author = mediaMetadata?.Artist?.ToString() ?? string.Empty;

            System.Diagnostics.Debug.WriteLine($"Metadata changed: title {title}, author {author}");
        }

        public void OnIsPlayingChanged(bool isPlaying)
        {
            IsPlayingChanged.Invoke(isPlaying);
            System.Diagnostics.Debug.WriteLine($"Playback state changed: {isPlaying}");
        }

        // This is the key: Process the metadata when it is available
        //public static string GetCurrentChapterTitle(IExoPlayer player)
        //{
        //    var staticMetadata = player.;
        //    foreach (var metadata in staticMetadata.Metadata)
        //    {
        //        // In M4B, chapters are typically ID3 frames.
        //        if (metadata is Id3Frame id3Frame)
        //        {
        //            // Look for ChapterFrame
        //            if (id3Frame is ChapterFrame chapterFrame)
        //            {
        //                // ChapterFrame often contains a TextInformationFrame sub-frame with the title
        //                foreach (var innerFrame in chapterFrame.SubFrames)
        //                {
        //                    if (innerFrame is TextInformationFrame textFrame)
        //                    {
        //                        // Return the chapter title
        //                        return textFrame.Value;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    // If no specific chapter title found in the current static metadata, 
        //    // you might fall back to the general media item title or return null.
        //    return player.CurrentMediaMetadata?.Title;
        //}
    }

}
