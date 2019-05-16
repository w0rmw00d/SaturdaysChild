using System.Web.Mvc;

/// <summary>
/// Class is used to return views in Articles folder. Views contain static content and may be viewed by any visitor.
/// </summary>
namespace SaturdaysChild.Controllers
{
    public class ArticlesController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult HowToResearch()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResearchAndSecurity()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SelfTeach()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult WhatIsResearch()
        {
            return View();
        }
    }
}