using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Example.Models;
using SessionMessage.Core;

namespace Example.Controllers
{
    public class HomeController : Controller
    {
        ISessionMessageManager _sessionMessageManager;
        public HomeController(ISessionMessageManager sessionMessageManager)
        {
            _sessionMessageManager = sessionMessageManager;
        }
        [HttpGet]
        public ActionResult Index()
        {
            var vm = new SessionMessageViewModel { Type = MessageType.Success, Behaviors = MessageBehaviors.StatusBar };
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(SessionMessageViewModel vm)
        {
            ViewBag.Message = "Message page.";
            _sessionMessageManager.SetMessage(vm.Type, vm.Behaviors, vm.Message);
            //return RedirectToAction("About");
            return View(vm);
        }
        [HttpPost]
        public ActionResult AjaxMessage([FromBody] SessionMessageViewModel vm)
        {
            _sessionMessageManager.SetMessage(vm.Type, vm.Behaviors, vm.Message);
            return View("Index");
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
    public class SessionMessageViewModel
    {
        public string Caption { get; set; }
        public string Message { get; set; }
        public MessageType Type { get; set; }
        public MessageBehaviors Behaviors { get; set; }
    }
}
