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

        public ActionResult ManualTest()
        {
            return View();
        }

        public FileResult GetSong(string songId)
        {

            if (!Guid.TryParse(songId, out Guid songGuid))
            {
                Response.StatusCode = 500;
                Response.Headers.Add("ErrorMsg", "Invalid value for parameter songId");
                return null;
            }

            foreach (var session in Server.Sessions)
            {
                foreach (var song in session.Queue.Songs)
                {
                    if (song.Id == songGuid)
                    {

                        //return file
                        if (song.HasBeenDownloaded)
                        {
                            var fi = new FileInfo(song.Location.Filepath);
                            if (fi.Exists)
                            {
                                return File(fi.OpenRead(), "audio/mpeg");
                            }
                            else
                            {
                                Response.StatusCode = 500;
                                Response.Headers.Add("ErrorMsg", "Song was marked as downloaded but the file did not exist");
                                return null;
                            }
                        }
                        else
                        {
                            //download the song now
                            if (song.Source == Source.YouTube)
                            {
                                song.Location.Filepath = RequestManager.GetMP3FromURL(song.Location.URL);
                                song.HasBeenDownloaded = true;

                                var fi = new FileInfo(song.Location.Filepath);
                                if (fi.Exists)
                                {
                                    return File(fi.OpenRead(), "audio/mpeg");
                                }
                                else
                                {
                                    Response.StatusCode = 500;
                                    Response.Headers.Add("ErrorMsg", "Song was marked as downloaded but the file did not exist");
                                    return null;
                                }
                            }
                            else
                            {
                                Response.StatusCode = 500;
                                Response.Headers.Add("ErrorMsg", "On-Demand downloading of spotify songs is not yet supported");
                                return null;
                            }
                        }
                    }
                }
            }

            return null;
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
            if (!sourcesDictionary.ContainsKey(normalizedSource))
            {
                return Json(new
                {
                    Status = 500,
                    Message = "invalid value '" + source + "'  for parameter 'source'"
                });
            }

            if (!Guid.TryParse(clientId, out Guid parsedClientId))
            {
                return Json(new
                {
                    Status = 500,
                    Message = "invalid clientId (unable to parse)"
                });
            }

            s = sourcesDictionary[normalizedSource];

            var client = Server.FindClient(parsedClientId);
            if (client == null)
            {
                return Json(new
                {
                    Status = 500,
                    Message = "unable to find client"
                });
            }

            var query = Server.Sessions.ToList().Where(_session => _session.SessionId == client.SessionId);
            if (!query.Any())
            {
                return Json(new
                {
                    Status = 500,
                    Message = "unable to find session with SessionId=" + client.SessionId.ToString()
                });
            }

            if (query.Count() > 1)
            {
                return Json(new
                {
                    Status = 500,
                    Message = "more than 1 session with SessionId=" + client.SessionId.ToString()
                });
            }

            var session = query.FirstOrDefault();

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

            session.Queue.Songs.Add(song);

            return Json(new
            {
                StatusCode = 200,
                Message = "queued song successfully",
                Songs = session.Queue.Songs,
                Position = 1
            });
        }

        [HttpPost]
        public JsonResult JoinSession(string DisplayName, string Password)
        {
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                return Json(new
                {
                    Status = 500,
                    Message = "invalid value for parameter 'DisplayName'"
                });
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
                return Json(new
                {
                    Status = 500,
                    Message = "bad password/invalid join key"
                });
            }

            var client = new Client()
            {
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

            if (string.IsNullOrWhiteSpace(Name))
            {
                return Json(new
                {
                    Status = 500,
                    Message = "invalid value for parameter 'Name'"
                });
            }

            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                return Json(new
                {
                    Status = 500,
                    Message = "invalid value for parameter 'DisplayName'"
                });
            }

            var session = new Session()
            {
                SessionId = Guid.NewGuid(),
                Clients = new List<Client>(),
                Password = Password,
                Name = Name
            };

            Server.Sessions.Add(session);

            var client = new Client()
            {
                ClientId = Guid.NewGuid(),
                SessionId = session.SessionId,
                Name = DisplayName,
                StreamingTo = true
            };

            session.Clients.Add(client);

            return Json(new
            {
                StatusCode = 200,
                Message = "created session",
                SessionId = session.SessionId,
                ClientId = client.ClientId
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
                StatusCode = 200,
                Sessions = _sessionList
            });
        }

        [HttpPost]
        public JsonResult GetSongList(string sessionId)
        {
            if (!Guid.TryParse(sessionId, out Guid sessionGuid))
            {
                return Json(new
                {
                    Status = 500,
                    Message = "invalid value for parameter 'sessionId' (unable to parse)"
                });
            }

            foreach (var session in Server.Sessions)
            {
                if (session.SessionId == sessionGuid)
                {
                    return Json(new
                    {
                        Status = 200,
                        Songs = session.Queue.Songs
                    });
                }
            }

            return Json(new
            {
                Status = 500,
                Message = "no session with that id"
            });
        }

        [HttpPost]
        public JsonResult NotifyDonePlaying(string sessionId, string songId)
        {
            //todo
            return Json(new
            {
                Status = 500,
                Message = "TODO"
            });
        }
    }
}