using System;
using System.Collections.Generic;

namespace CS386_Project.code
{
    public class Queue
    {
        public Queue()
        {
        }

        private List<Song> _songs = null;
        public List<Song> Songs
        {
            get
            {
                if (_songs == null)
                {
                    _songs = new List<Song>();
                }

                return _songs;
            }

            set
            {
                if (value != null)
                {
                    _songs = value;
                }
            }
        }
    }
}
