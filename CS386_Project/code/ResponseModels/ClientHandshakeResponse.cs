using System;

namespace CS386_Project.code.ResponseModels
{
    public class ClientHandshakeResponse : ClientResponse
    {
        public string PrivateKey { get; set; }
    }
}
