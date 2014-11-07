using control_notas_cit.Models.Entidades;
using control_notas_cit.Models.Repositorios;
using control_notas_cit.Models.ViewModels;
using Microsoft.AspNet.Identity;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace control_notas_cit.Controllers
{
    [Authorize(Roles = "Profesor")]
    public class ProfesorController : Controller
    {
        private ApplicationDbContext AppContext;
        private IRepositorioGenerico<Celula> repoCelulas = null;
        private IRepositorioGenerico<ApplicationUser> repoUsers = null;
        private IRepositorioGenerico<Proyecto> repoProyectos = null;
        private IRepositorioGenerico<Calendario> repoCalendarios = null;
        private IRepositorioGenerico<Semana> repoSemanas = null;

        public ProfesorController()
        {
            // Obtengo el contexto que OWIN creó al iniciar la aplicación
            AppContext = ApplicationDbContext.GetDBContext();

            // Se lo paso a mi repositorio
            this.repoCelulas = new RepositorioGenerico<Celula>(AppContext);
            this.repoUsers = new RepositorioGenerico<ApplicationUser>(AppContext);
            this.repoProyectos = new RepositorioGenerico<Proyecto>(AppContext);
            this.repoCalendarios = new RepositorioGenerico<Calendario>(AppContext);
            this.repoSemanas = new RepositorioGenerico<Semana>(AppContext);
        }

        //
        // GET: /Profesor/Index/
        public ActionResult Index()
        {
            ProfesorIndexViewModel model = new ProfesorIndexViewModel();

            // Obtengo el usuario
            var currentUser = GetCurrentUser();

            if( currentUser == null )
            {
                return HttpNotFound();
            }

            // Utilizo el usuario para obtener los demas datos del modelo de la vista
            model.NombreProfesor = currentUser.Nombre + " " + currentUser.Apellido;
            model.Proyecto = repoProyectos.SelectAll().Where(p => p.ProyectoID == currentUser.Proyecto.ProyectoID).Single();

            // Calendario será null si la query no devuelve un valor, en el caso de que sea null, la vista mostrara un mensaje
            model.Calendario = repoCalendarios.SelectAll()
                                .Where(c => c.CalendarioID == model.Proyecto.CalendarioActualID)
                                .SingleOrDefault();

            return View(model);
        }

        //
        // GET: /Profesor/AgregarCalendario/
        public ActionResult AgregarCalendario()
        {
            List<Semana> semanas = new List<Semana>();

            for (int i = 1; i <= 12; i++ )
            {
                Semana s = new Semana()
                {
                    NumeroSemana = i
                };

                semanas.Add(s);
            }

            CalendarioViewModel model = new CalendarioViewModel()
            {
                FechaInicio = DateTime.Now,
                Semanas = semanas
            };

            return View(model);
        }

        //
        // POST: /Profesor/AgregarCalendario/
        [HttpPost]
        public ActionResult AgregarCalendario(CalendarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                Calendario calendario = new Calendario();

                // Cada calendario cuenta con 12 semanas
                int numeroSemanas = 12;

                calendario.FechaInicio = model.FechaInicio;
                calendario.FechaFinal = model.FechaInicio.AddDays(numeroSemanas * 7);

                List<Semana> semanas = model.Semanas;
                calendario.Semanas = new List<Semana>();

                foreach (Semana s in semanas)
                {
                    s.Fecha = model.FechaInicio.AddDays(s.NumeroSemana * 7);
                    calendario.Semanas.Add(s);
                }

                var user = GetCurrentUser();

                if( user == null )
                {
                    return HttpNotFound("Usuario no encontrado");
                }

                var proyecto = user.Proyecto;

                if(proyecto == null)
                {
                    return HttpNotFound("Proyecto no encontrado");
                }

                calendario.Proyecto = proyecto;

                repoCalendarios.Insert(calendario);
                repoCalendarios.Save();

                Semana sem = calendario.Semanas.Where(s => s.NumeroSemana == 1).Single();
                sem.Iniciada = true;
                calendario.SemanaActualID = sem.SemanaID;
                proyecto.CalendarioActualID = calendario.CalendarioID;

                repoProyectos.Update(proyecto);
                repoProyectos.Save();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        //
        // GET: /Profesor/EditarSemana/
        public ActionResult EditarSemana(int id)
        {
            Semana s = repoSemanas.SelectById(id);
            var model = new SemanaViewModel()
            {
                SemanaID = s.SemanaID,
                Fecha = s.Fecha,
                Actividad = s.Actividad,
                Descripcion = s.Descripcion,
                NumeroSemana = s.NumeroSemana
            };
            return View(model);
        }

        //
        // POST: /Profesor/EditarSemana/
        [HttpPost]
        public ActionResult EditarSemana(SemanaViewModel model)
        {
            if (ModelState.IsValid)
            {
                Semana semana = repoSemanas.SelectById(model.SemanaID);

                if (semana == null)
                {
                    ModelState.AddModelError("", "No se pudo encontrar esta semana");
                    return View(model);
                }

                semana.Actividad = model.Actividad;
                semana.Descripcion = model.Descripcion;

                repoSemanas.Update(semana);
                repoSemanas.Save();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // ### NOTA en esta vista se le pedira al profesor revisar las minutas faltantes por revisar
        // ### crear un modelo para esta vista y pasarle esas minutas
        // GET: /Profesor/FinalizarSemana/
        public ActionResult FinalizarSemana(int id)
        {
            Semana model = repoSemanas.SelectById(id);
            return View(model);
        }

        //
        // POST: /Profesor/FinalizarSemana/
        [HttpPost]
        public ActionResult FinalizarSemana(int? id)
        {
            if (id != null)
            {
                Semana semana = repoSemanas.SelectById(id);

                if (semana.Iniciada == true && semana.Finalizada == false)
                {
                    semana.Finalizada = true;
                    repoSemanas.Update(semana);

                    if (semana.NumeroSemana < 12)
                    {
                        Semana proximaSemana = repoSemanas.SelectAll().Where(s => s.NumeroSemana == (semana.NumeroSemana + 1)).Single();
                        proximaSemana.Iniciada = true;
                        proximaSemana.Calendario.SemanaActualID = proximaSemana.SemanaID;

                        repoSemanas.Update(proximaSemana);
                    }

                    repoSemanas.Save();
                }

                return RedirectToAction("Index");
            }

            return View();
        }

        // Obtiene el usuario logueado actualmente
        private ApplicationUser GetCurrentUser()
        {
            // Tira una excepcion cuando el explorador ya tiene una sesion iniciada, debido a que al ejecutar el Seed, el id es totalmente distinto
            return repoUsers.SelectAll().Where(u => u.Id == User.Identity.GetUserId()).Single();
        }

        // Estos métodos permiten acceder a la información de los usuarios, aunque también se pueden obtener a través de la tabla Users
        // Sin embargo, UserManager y RoleManager tienen métodos asincronicos mucho más optimizados
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
	}
}