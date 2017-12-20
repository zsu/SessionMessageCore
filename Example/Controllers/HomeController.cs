using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Example.Models;
using SessionMessages.Core;

namespace Example.Controllers
{
    public class HomeController : Controller
    {
        ISessionMessageManager _sessionMessageManager;
        public HomeController(ISessionMessageManager sessionMessageManager)
        {
            _sessionMessageManager = sessionMessageManager;
        }
        public IActionResult Index()
        {
            _sessionMessageManager.SetMessage(MessageType.Info, MessageBehaviors.StatusBar, "test");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
