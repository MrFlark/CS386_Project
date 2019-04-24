using System;
using Xunit;
using CS386_Project.code;
using CS386_Project.Controllers;

namespace CS386_UnitTests
{
    public class YouTubeTests
    {
        [Fact]
        public void TestDownload()
        {
            Song song = new Song
            {
                Location = new Location
                {
                    URL = ""
                },
                Source = Source.YouTube,
                Id = Guid.NewGuid(),
                RequestTimeStamp = DateTime.Now,
                HasBeenDownloaded = false
            };

            var loadDataResult = RequestManager.LoadSongData(song);
            if(!loadDataResult){
                throw new Exception("RequestManager returned false for loadSongData");
            }

            if(string.IsNullOrWhiteSpace(song.Name))
            {
                throw new Exception("Failed to load song name");
            }

            var downloadSongResult = RequestManager.DownloadSong(song);
            if(!downloadSongResult)
            {
                throw new Exception("Failed to download song");
            }

            if(song.HasBeenDownloaded == false)
            {
                throw new Exception("Failed to download song");
            }

            if (string.IsNullOrWhiteSpace(song.Location.Filepath))
            {
                throw new Exception("Failed to save song to disk");
            }
        }
    }
}