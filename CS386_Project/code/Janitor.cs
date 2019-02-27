using System;
using System.Collections.Generic;
using System.IO;

namespace CS386_Project.code
{
    public class Janitor
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

            //iterate throguh all queued songs.
            //if a played song is queued, do not add it to the toDelete list
            var toDelete = new List<Song>();
            foreach (var session in Server.Sessions)
            {
                foreach (var song in session.Queue.Songs)
                {
                    var url = song.Location.URL.ToLower().Trim();
                    //search for this URL in the list of songs marked for deletion
                    foreach (var playedSong in playedSongs)
                    {
                        if (playedSong.Location.URL.ToLower().Trim() != url)
                        {
                            toDelete.Add(playedSong);
                        }
                    }
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
