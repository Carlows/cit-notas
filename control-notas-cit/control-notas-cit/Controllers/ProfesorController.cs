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

        public ProfesorController()
        {
            // Obtengo el contexto que OWIN creó al iniciar la aplicación
            AppContext = ApplicationDbContext.GetDBContext();

            // Se lo paso a mi repositorio
            this.repoCelulas = new RepositorioGenerico<Celula>(AppContext);
            this.repoUsers = new RepositorioGenerico<ApplicationUser>(AppContext);
            this.repoProyectos = new RepositorioGenerico<Proyecto>(AppContext);
            this.repoCalendarios = new RepositorioGenerico<Calendario>(AppContext);
        }

        //
        // GET: /Profesor/Index/
        public ActionResult Index()
        {
            ProfesorIndexViewModel model = new ProfesorIndexViewModel();

            // Obtengo el usuario
            var currentUser = repoUsers.SelectAll().Where(u => u.Id == User.Identity.GetUserId()).Single();

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