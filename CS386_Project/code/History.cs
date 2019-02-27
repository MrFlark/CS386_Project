using System;
using System.Collections.Generic;

namespace CS386_Project.code
{
    public class History
    {
        public History()
        {
        }

        private List<Song> _songHistory = null;
        public List<Song> SongHistory
        {
            get
            {
                if (_songHistory == null)
                {
                    _songHistory = new List<Song>();
                }

                return _songHistory;
            }

            set
            {
                if (value != null)
                {
                    _songHistory = value;
                }
            }
        }
    }
}
