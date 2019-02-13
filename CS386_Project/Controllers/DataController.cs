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

        [HttpPost]
        public FileResult GetMP3(string url)
        {
            var mp3path = YoutubeDemo.GetMP3FromURL(url);
            var info = new FileInfo(mp3path);
            return File(info.OpenRead(), "audio/x-wave");
        }

        [HttpGet]
        public FileResult GetMP3_GET(string url)
        {
            var mp3path = YoutubeDemo.GetMP3FromURL(url);
            var info = new FileInfo(mp3path);
            return File(info.OpenRead(), "audio/x-wave");
        }

        public ActionResult YoutubeDL()
        {
            return View();
        }

        [HttpPost]
        public JsonResult QueueSong(string clientId, string source, string url = "")
        {
            if (source == "youtube")
            {

            }
            else if (source == "spotify")
            {

            }

            var _songs = new List<object>();

            return Json(new
            {
                StatusCode = 200,
                Message = "queued song successfully",
                Songs = _songs,//todo
                Position = 1
            });
        }

        [HttpPost]
        public JsonResult JoinSession(string SessionId, string DisplayName, string Password)
        {
            if(string.IsNullOrWhiteSpace(DisplayName)){
                //todo error
            }

            var client = new Client() {
                ClientRef = null,
                ClientId = Guid.NewGuid(),
                Name = DisplayName
            };

            return Json(new
            {
                StatusCode = 200,
                Message = "Opened socket connection",
                ClientId = client.ClientId.ToString()
            });
        }

        [HttpPost]
        public JsonResult CreateSession(string Name, string Password = null, string DisplayName)
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