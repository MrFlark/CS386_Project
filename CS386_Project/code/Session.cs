using System;
using System.Collections.Generic;

namespace CS386_Project.code
{
    public class Session
    {
        public Guid SessionId { get; set; }

        private Queue _queue = null;
        public Queue Queue {
            get {
                if (_queue == null)
                {
                    _queue = new Queue();
                }

                return _queue;
            }

            set {
                _queue = value;
            }
        }

        private List<Client> _clients = null;
        public List<Client> Clients {
            get {
                if (_clients == null)
                {
                    _clients = new List<Client>();
                }

                return _clients;
            }

            set {
                _clients = value;
            }
        }

        public string Password { get; set; }
        public string Name { get; set; }

        private History _history = null;
        public History History {
            get {
                if (_history == null)
                {
                    _history = new History();
                }

                return _history;
            }

            set {
                _history = value;
            }
        }

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
