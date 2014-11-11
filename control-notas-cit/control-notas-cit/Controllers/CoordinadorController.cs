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
        private IRepositorioGenerico<Alumno> repoAlumnos = null;
        private IRepositorioGenerico<Minuta> repoMinutas = null;

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
            this.repoAlumnos = new RepositorioGenerico<Alumno>(AppContext);
            this.repoMinutas = new RepositorioGenerico<Minuta>(AppContext);
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
            model.MinutaSemana = GetCurrentMinuta();

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

        //
        // GET: /Coordinador/ListaAlumnos/
        public ActionResult ListaAlumnos()
        {
            var model = GetCurrentCelula().Alumnos.ToList();

            return View(model);
        }

        //
        // GET: /Coordinador/AgregarAlumno/
        public ActionResult AgregarAlumno()
        {
            return View();
        }

        //
        // POST: /Coordinador/AgregarAlumno/
        [HttpPost]
        public ActionResult AgregarAlumno(AlumnoViewModel model)
        {
            if(ModelState.IsValid)
            {
                Alumno alumno = new Alumno()
                {
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    Cedula = model.Cedula,
                    Telefono = model.Telefono,
                    Email = model.Email,
                    Celula = GetCurrentCelula()
                };

                repoAlumnos.Insert(alumno);
                repoAlumnos.Save();

                return RedirectToAction("ListaAlumnos");
            }

            return View(model);
        }

        //
        // GET: /Coordinador/EditarAlumno/1
        public ActionResult EditarAlumno(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("ListaAlumnos");
            }

            var alumno = repoAlumnos.SelectById(id);

            if (alumno == null)
            {
                return RedirectToAction("ListaAlumnos");
            }

            var model = new AlumnoViewModel()
            {
                Id = alumno.AlumnoID,
                Nombre = alumno.Nombre,
                Apellido = alumno.Apellido,
                Telefono = alumno.Telefono,
                Cedula = alumno.Cedula,
                Email = alumno.Email
            };

            return View(model);
        }

        //
        // POST: /Coordinador/EditarAlumno/
        [HttpPost]
        public ActionResult EditarAlumno(AlumnoViewModel model)
        {
            if(ModelState.IsValid)
            {
                Alumno alumno = repoAlumnos.SelectById(model.Id);

                if(alumno == null)
                {
                    ModelState.AddModelError("", "No se pudo encontrar el alumno");
                    return View(model);
                }

                alumno.Nombre = model.Nombre;
                alumno.Apellido = model.Apellido;
                alumno.Cedula = model.Cedula;
                alumno.Telefono = model.Telefono;
                alumno.Email = model.Email;

                repoAlumnos.Update(alumno);
                repoAlumnos.Save();

                return RedirectToAction("ListaAlumnos");
            }

            return View(model);
        }

        // POST: /Coordinador/BorrarAlumno/1
        [HttpPost]
        public ActionResult BorrarAlumno(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("ListaAlumnos");
            }

            repoAlumnos.Delete(id);
            repoAlumnos.Save();

            return RedirectToAction("ListaAlumnos");
        }

        //
        // GET: /Coordinador/AgregarMinuta/
        public ActionResult AgregarMinuta()
        {
            if(GetCurrentSemana() == null)
            {
                return RedirectToAction("Index");
            }

            var minutaActual = GetCurrentMinuta();
            if (minutaActual != null)
            {
                return View(new MinutaCelulaViewModel()
                {
                    Id = minutaActual.MinutaID,
                    Contenido = minutaActual.Contenido
                });
            }

            return View(new MinutaCelulaViewModel());
        }

        //
        // POST: /Coordinador/AgregarMinuta/
        [HttpPost]
        public ActionResult AgregarMinuta(MinutaCelulaViewModel model)
        {
            if(ModelState.IsValid)
            {
                var celula = GetCurrentCelula();
                var semana = GetCurrentSemana();

                if(celula == null || semana == null)
                {
                    ModelState.AddModelError("", "No se pudo encontrar la celula o la semana");
                    return View(model);
                }

                Minuta minuta = new Minuta()
                {
                    Contenido = model.Contenido,
                    Celula = celula,
                    Semana = semana
                };

                if (model.Id == 0)
                {
                    repoMinutas.Insert(minuta);
                    repoMinutas.Save();                    
                }
                else
                {
                    minuta.MinutaID = model.Id;

                    repoMinutas.Update(minuta);
                    repoMinutas.Save();
                }

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

        // Obtiene la minuta actual o devuelve null si no existe calendario aun
        private Minuta GetCurrentMinuta()
        {
            return repoMinutas.SelectAll().Where(m => m.Semana.SemanaID == GetCurrentSemana().SemanaID && m.Celula.CelulaID == GetCurrentCelula().CelulaID).SingleOrDefault();
        }
	}
}