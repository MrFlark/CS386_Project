using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CS386_Project.Controllers
{
    public class DataController : Controller
    {
        [HttpPost]
        public JsonResult Test(string param){
            return Json(new {
                StatusCode = 200,
                Message = "param: " + param
            });
        }
    }
}
