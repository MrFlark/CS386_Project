﻿using System;
using System.Net.Sockets;

namespace CS386_Project.code
{
    public class Client
    {
        public Guid ClientId { get; set; }
        public bool StreamingTo { get; set; }
        public string InetAddress { get; set; }
        public TcpClient ClientRef { get; set; }
        public string Name { get; set; }

        public Client()
        {
        }
    }
}