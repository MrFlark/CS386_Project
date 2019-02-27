using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CS386_Project.code;
using Microsoft.AspNetCore.Mvc;

namespace CS386_Project.Controllers
{
    public class DataController : Controller
    {
        public JsonResult Test(string param)
        {
            return Json(new
            {
                value = param
            });
        }

        public ActionResult YoutubeDL()
        {
            return View();
        }

        [HttpPost]
        public JsonResult QueueSong(string clientId, string source, string url = "")
        {
            Source s = Source.UNDEFINED;
            var sourcesDictionary = new Dictionary<string, Source>() {
                { "youtube", Source.YouTube },
                { "spotify", Source.Spotify }
            };

            var normalizedSource = source.ToLower().Trim();
            if(!sourcesDictionary.ContainsKey(normalizedSource))
            {
                //todo error (bad source parameter)
            }

            s = sourcesDictionary[normalizedSource];

            var song = new Song()
            {
                Source = s,
                Name = null,//to be populated later in RequestManager.LoadSongData
                RequestTimeStamp = DateTime.Now,
                Location = new Location
                {
                    URL = url,
                    Filepath = null
                },
                Id = Guid.NewGuid(),
                HasBeenDownloaded = false
            };


            return Json(new
            {
                StatusCode = 200,
                Message = "queued song successfully",
                Songs = new List<object>(),//todo
                Position = 1
            });
        }

        [HttpPost]
        public JsonResult JoinSession(string DisplayName, string Password)
        {
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                //todo error
            }

            Session sessionToJoin = null;
            foreach (var s in Server.Sessions)
            {
                if (s.Password == Password)
                {
                    sessionToJoin = s;
                }
            }

            if (sessionToJoin == null)
            {
                //todo error (bad password / invalid join key)
            }

            var client = new Client()
            {
                ClientRef = null,
                ClientId = Guid.NewGuid(),
                Name = DisplayName,
                SessionId = sessionToJoin.SessionId
            };

            return Json(new
            {
                StatusCode = 200,
                Message = "Opened socket connection",
                ClientId = client.ClientId.ToString()
            });
        }

        [HttpPost]
        public JsonResult CreateSession(string Name, string DisplayName, string Password = null)
        {

            var session = new Session()
            {
                SessionId = Guid.NewGuid(),
                Clients = new List<Client>(),
                Password = Password,
                Name = Name
            };

            Server.Sessions.Add(session);

            var privateKey = Guid.NewGuid().ToString();
            Server.AcceptNewClient(privateKey, session.SessionId);

            return Json(new
            {
                StatusCode = 200,
                Message = "created session with id: " + session.SessionId,
                PrivateKey = privateKey
            });
        }

        [HttpPost]
        public JsonResult GetSessionList()
        {
            var _sessionList = new List<object>();

            foreach (var session in Server.Sessions)
            {
                if (session != null)
                {
                    _sessionList.Add(new
                    {
                        SessionId = session.SessionId,
                        Name = session.Name,
                        ClientCount = session.Clients.Count,
                        HasPassword = session.Password != null
                    });
                }
            }

            return Json(new
            {
                Sessions = _sessionList
            });
        }
    }
}