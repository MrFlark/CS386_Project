using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using CS386_Project.code.ResponseModels;

namespace CS386_Project.code
{
    public class Server
    {
        public static readonly string TEMP_DIR = @"temp\\";

        public static readonly int PORT = 1234;
        public static readonly string SERVER_ADDRESS = "127.0.0.1";
        public static readonly int CLIENT_TIMEOUT_MS = 30 * 1000;//30 second timeout

        public static TcpListener server = null;

        private static List<Session> _sessions = null;
        public static List<Session> Sessions {
            get {
                if(_sessions == null){
                    _sessions = new List<Session>();
                }
                return _sessions;
            }
        }

        public static Client FindClient(Guid clientId) {
            foreach(var session in Sessions){
                foreach(var client in session.Clients){
                    if(client.ClientId == clientId){
                        return client;
                    }
                }
            }
            return null;
        }

        public static void StreamAudio(Guid clientId, string localPath){

            var client = FindClient(clientId).ClientRef;
            if(client == null){
                //todo error
            }

            var stream = client.GetStream();

            while (!stream.DataAvailable);

            var bytes = File.ReadAllBytes(localPath);

            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();

            var responseBytes = new byte[client.Available];
            stream.Read(responseBytes, 0, responseBytes.Length);
            var responseString = Encoding.UTF8.GetString(responseBytes);
            var response = JsonConvert.DeserializeObject<ClientStreamResponse>(responseString);

            if(!response.Success){
                //todo error
            }
        }

        public static void AcceptNewClient(string privateKey, Guid sessionId){
            new Thread(t => {
               
                var startTime = DateTime.Now;
                var acceptedClient = false;

                while (!acceptedClient)
                {
                    var client = server.AcceptTcpClient();
                    var stream = client.GetStream();

                    //write welcome message to client

                    var responseObject = new {
                        Message = "[welcome message here]"
                    };

                    var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseObject));
                    stream.Write(payload, 0, payload.Length);
                    stream.Flush();

                    //wait for response
                    while (!stream.DataAvailable);


                    var clientResponse = new byte[client.Available];
                    stream.Read(clientResponse, 0, clientResponse.Length);

                    var clientResponseStr = Encoding.UTF8.GetString(clientResponse);
                    var clientResponseObject = JsonConvert.DeserializeObject<ClientHandshakeResponse>(clientResponseStr);

                    if(clientResponseObject.PrivateKey != privateKey){
                        //todo error
                    }

                    var clientIdStr = clientResponseObject.ClientId;
                    if(!Guid.TryParse(clientIdStr, out Guid clientId)){
                        //todo error
                    }

                    //register tcp client w/ session client
                    FindClient(clientId).ClientRef = client;
                }
            }).Start();
        }

        public static void StartServer(){
            server = new TcpListener(IPAddress.Parse(SERVER_ADDRESS), PORT);
            server.Start();
        }

        public static bool sendPacket(IPAddress ip, string json){

            //todo

            return false;
        }

        private Server()
        {
        }
    }
}
