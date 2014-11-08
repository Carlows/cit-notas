using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using control_notas_cit.Models.Repositorios;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using control_notas_cit.Models.ViewModels;
using control_notas_cit.Models.Entidades;

namespace control_notas_cit.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private ApplicationDbContext AppContext;
        private IRepositorioGenerico<Celula> repoCelulas = null;
        private IRepositorioGenerico<ApplicationUser> repoUsers = null;
        private IRepositorioGenerico<Proyecto> repoProyectos = null;
        private IRepositorioGenerico<Calendario> repoCalendarios = null;
        private IRepositorioGenerico<Semana> repoSemanas = null;
        private IRepositorioGenerico<IdentityRole> repoRoles = null;

        public CoordinadorController()
        {
            // Obtengo el contexto que OWIN creó al iniciar la aplicación
            AppContext = ApplicationDbContext.GetDBContext();

            // Se lo paso a los repositorios
            this.repoCelulas = new RepositorioGenerico<Celula>(AppContext);
            this.repoUsers = new RepositorioGenerico<ApplicationUser>(AppContext);
            this.repoProyectos = new RepositorioGenerico<Proyecto>(AppContext);
            this.repoCalendarios = new RepositorioGenerico<Calendario>(AppContext);
            this.repoSemanas = new RepositorioGenerico<Semana>(AppContext);
            this.repoRoles = new RepositorioGenerico<IdentityRole>(AppContext);
        }

        //
        // GET: /Coordinador/
        public ActionResult Index()
        {
            CoordinadorIndexViewModel model = new CoordinadorIndexViewModel();

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction("Logoff", "Account");
            }

            if (currentUser.Celula == null)
            {
                return RedirectToAction("Logoff", "Account");
            }

            model.Celula = GetCurrentCelula();
            model.Semana = GetCurrentSemana();

            return View(model);
        }

        //
        // GET: /Coordinador/EditarCelula/
        public ActionResult EditarCelula()
        {
            var celula = GetCurrentCelula();

            if (celula == null)
            {
                return RedirectToAction("Index");
            }

            var model = new CelulaEditViewModel()
            {
                Id = celula.CelulaID,
                Nombre = celula.Nombre,
                Descripcion = celula.Descripcion
            };

            return View(model);
        }

        //
        // POST: /Coordinador/EditarCelula/
        [HttpPost]
        public ActionResult EditarCelula(CelulaEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var celula = repoCelulas.SelectById(model.Id);

                if (celula == null)
                {
                    ModelState.AddModelError("", "No se pudo encontrar la celula");
                    return View(model);
                }

                celula.Nombre = model.Nombre;
                celula.Descripcion = model.Descripcion;

                repoCelulas.Update(celula);
                repoCelulas.Save();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // Obtiene el usuario logueado actualmente
        private ApplicationUser GetCurrentUser()
        {
            // Tira una excepcion cuando el explorador ya tiene una sesion iniciada, debido a que al ejecutar el Seed, el id es totalmente distinto
            return repoUsers.SelectAll().Where(u => u.Id == User.Identity.GetUserId()).SingleOrDefault();
        }

        // Obtiene la celula
        private Celula GetCurrentCelula()
        {
            return repoCelulas.SelectAll().Where(c => c.CelulaID == GetCurrentUser().Celula.CelulaID).Single();
        }
        
        // Obtiene el calendario o devuelve null si no existe ninguno
        private Calendario GetCurrentCalendario()
        {
            return repoCalendarios.SelectAll().Where(c => c.CalendarioID == GetCurrentCelula().Proyecto.CalendarioActualID).SingleOrDefault();
        }

        // Obtiene la semana actual o devuelve null si no existe calendario aun
        private Semana GetCurrentSemana()
        {
            if (GetCurrentCalendario() == null)
                return null;

            return repoSemanas.SelectAll().Where(s => s.SemanaID == GetCurrentCalendario().SemanaActualID).SingleOrDefault();
        }
	}
}