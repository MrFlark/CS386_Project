using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VideoLibrary;

namespace CS386_Project.code
{
    public class YoutubeDemo
    {
        public static string GetMP3FromURL(string url)
        {
            try
            {
                var youtube = YouTube.Default;
                var vid = youtube.GetVideo(url);
                File.WriteAllBytes(Server.TEMP_DIR + vid.FullName, vid.GetBytes());

                var inputFile = new MediaFile { Filename = Server.TEMP_DIR + vid.FullName };
                var outputFile = new MediaFile { Filename = $"{Server.TEMP_DIR + vid.FullName}.wav" };

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
    }
}
