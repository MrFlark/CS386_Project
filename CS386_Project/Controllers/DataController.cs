﻿using System;
using System.Collections.Generic;
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
        public JsonResult QueueSong(string clientId, string source, string url = "")
        {
            if (source == "youtube")
            {

            }
            else if (source == "spotify")
            {

            }

            return Json(new
            {
                StatusCode = 200,
                Message = "queued song successfully"
            });
        }

        [HttpPost]
        public JsonResult JoinSession(string sessionId, string password)
        {
            var client = new Client()
            {
                ClientRef = null,
                ClientId = Guid.NewGuid(),

            };

            return Json(new
            {
                StatusCode = 200,
                Message = "Opened socket connection",
                ClientId = client.ClientId.ToString()
            });
        }

        [HttpPost]
        public JsonResult CreateSession(string name, bool hasPassword, string password = null)
        {

            var session = new Session()
            {
                SessionId = Guid.NewGuid(),
                Clients = new List<Client>(),
                Password = password,
                Name = name
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
                        hasPassword = session.Password != null
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