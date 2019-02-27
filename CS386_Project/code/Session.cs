using System;
using System.Collections.Generic;

namespace CS386_Project.code
{
    public class Session
    {
        public Guid SessionId { get; set; }
        public Queue Queue { get; set; }
        public List<Client> Clients { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public History History { get; set; }

        //returns list of songs that have had their name loaded by a separate thread
        public List<Song> GetQueuedSongList()
        {
            var l = new List<Song>();
            foreach (var s in Queue.Songs)
            {
                if (s.Name != null)
                {
                    l.Add(s);
                }
            }

            return l;
        }
    }
}
