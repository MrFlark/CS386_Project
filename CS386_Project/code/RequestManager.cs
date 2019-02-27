using System;
using System.IO;
using MediaToolkit;
using MediaToolkit.Model;
using VideoLibrary;

namespace CS386_Project.code
{
    public class RequestManager
    {

        //populates title and duration
        public static bool LoadSongData(Song s)
        {
            try
            {
                var youtube = YouTube.Default;
                var vid = youtube.GetVideo(s.Location.URL);
                s.Name = vid.Title;
                //todo get duration
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static string GetMP3FromURL(string url)
        {
            try
            {
                var youtube = YouTube.Default;
                var vid = youtube.GetVideo(url);
                File.WriteAllBytes(Server.TEMP_DIR + vid.FullName, vid.GetBytes());

                var inputFile = new MediaFile { Filename = Server.TEMP_DIR + vid.FullName };
                var outputFile = new MediaFile { Filename = $"{Server.TEMP_DIR + vid.FullName}.mp3" };

                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                    engine.Convert(inputFile, outputFile);
                }

                return outputFile.Filename;
            }
            catch (Exception e)
            {
                File.WriteAllText(@"test.txt", e.Message);
                return "";
            }
        }

        public bool DownloadSong(Song song)
        {

            var filepath = GetMP3FromURL(song.Location.URL);

            if (filepath != "" && filepath != null)
            {
                song.Location.Filepath = filepath;
                song.HasBeenDownloaded = true;
                return true;
            }

            return false;
        }

        public Song FindHighestPrioritySong()
        {

            Song oldestSong = null;

            foreach (var session in Server.Sessions)
            {

                var queue = session.Queue;

                foreach (var song in queue.Songs)
                {
                    if (song.HasBeenDownloaded)
                    {
                        //songs that have already been downloaded are not valid, skip
                        continue;
                    }

                    //songs with the earliest request timestamp are the highest priority
                    if (oldestSong == null || song.RequestTimeStamp < oldestSong.RequestTimeStamp)
                    {

                        oldestSong = song;

                    }
                }
            }

            return oldestSong;
        }
    }
}
