using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace control_notas_cit.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        //
        // GET: /Coordinador/
        public ActionResult Index()
        {
            return View();
        }
	}
}