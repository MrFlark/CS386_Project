using System;
using System.Collections.Generic;
using System.IO;

namespace CS386_Project.code
{
    public class CleanupManager
    {
        public static bool TryRemoveFile(string filepath)
        {
            try
            {
                if (!File.Exists(filepath))
                {
                    return true;
                }

                File.Delete(filepath);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static void CleanupTempDir()
        {
            //get all songs that have already been played
            var playedSongs = new List<Song>();
            foreach (var session in Server.Sessions)
            {
                var history = session.History;
                foreach (var song in history.SongHistory)
                {
                    if (!playedSongs.Contains(song))
                    {
                        playedSongs.Add(song);
                    }
                }
            }

            var allQueuedSongURLs = new List<string>();
            foreach (var session in Server.Sessions)
            {
                foreach (var song in session.Queue.Songs)
                {
                    var normalizedURL = song.Location.URL.ToLower().Trim();
                    allQueuedSongURLs.Add(normalizedURL);
                }
            }

            //iterate throguh all queued song URLs
            //if a played song is queued, do not add it to the toDelete list
            var toDelete = new List<Song>();
            foreach (var s in playedSongs)
            {
                var normalizedURL = s.Location.URL.ToLower().Trim();
                if(!allQueuedSongURLs.Contains(normalizedURL))
                {
                    toDelete.Add(s);
                }
            }

            //remove all songs marked for deletion
            toDelete.ForEach(s =>
            {
                TryRemoveFile(s.Location.Filepath);
            });
        }
    }
}
