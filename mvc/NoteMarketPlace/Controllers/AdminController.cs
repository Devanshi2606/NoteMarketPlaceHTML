using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Controllers
{
    public class AdminController : Controller
    {
        private Notes_MarketplaceEntities Db = new Notes_MarketplaceEntities();

        [HttpGet]
        public ActionResult Dashboard_admin()
        {
            return View();
        }

    }
}