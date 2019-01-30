using System;
using System.Collections.Generic;

namespace CS386_Project.code
{
    public class Session
    {

        public Guid SessionId { get; set; }
        public List<Client> Clients { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

        public Session()
        {
        }
    }
}
