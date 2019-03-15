using System;
using System.Net.Sockets;

namespace CS386_Project.code
{
    public class Client
    {
        public Guid ClientId { get; set; }
        public bool StreamingTo { get; set; }
        public string Name { get; set; }
        public Guid SessionId { get; set; }
    }
}
