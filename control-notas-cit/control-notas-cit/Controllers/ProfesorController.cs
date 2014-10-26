using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace control_notas_cit.Controllers
{
    [Authorize(Roles = "Profesor")]
    public class ProfesorController : Controller
    {
        //
        // GET: /Profesor/
        public ActionResult Index()
        {
            return View();
        }
	}
}