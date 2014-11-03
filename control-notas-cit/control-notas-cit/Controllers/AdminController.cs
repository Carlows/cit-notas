using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using control_notas_cit.Models.Repositorios;
using control_notas_cit.Models.Entidades;
using control_notas_cit.Models.ViewModels;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace control_notas_cit.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext AppContext;
        private IRepositorioGenerico<Proyecto> repoProyectos = null;
        private IRepositorioGenerico<ApplicationUser> repoUsers = null;
        private string profesor_rol_id;

        public AdminController()
        {
            // Obtengo el contexto que OWIN creó al iniciar la aplicación
            AppContext = ApplicationDbContext.GetDBContext();

            // Se lo paso a mi repositorio
            this.repoProyectos = new RepositorioGenerico<Proyecto>(AppContext);
            this.repoUsers = new RepositorioGenerico<ApplicationUser>(AppContext);

            // Necesito el id del rol del profesor para usarlo en este controlador
            this.profesor_rol_id = (AppContext.Roles.Where(x => x.Name == "Profesor")).Select(y => y.Id).Single();
        }

        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View(repoProyectos.SelectAll());
        }

        //
        // GET: /Admin/Crear/
        public ActionResult Crear()
        {
            List<ApplicationUser> users = repoUsers.SelectAll().Where(x => x.Roles.Select(y => y.RoleId).Contains(profesor_rol_id)).ToList();
            List<string> nombres = new List<string>();

            foreach( ApplicationUser u in users )
            {
                nombres.Add(string.Concat(u.Nombre + " " + u.Apellido));
            }
            
            return View(new ProjectViewModel
            {
                Profesores = nombres
            });
        }

        //
        // POST: /Admin/Crear/
        [HttpPost]
        public ActionResult Crear(ProjectViewModel model)
        {
            if( ModelState.IsValid )
            {
                List<ApplicationUser> profesores = (from u in repoUsers.SelectAll()
                                                   where model.Profesores.Contains(string.Concat(u.Nombre, " ", u.Apellido))
                                                   select u).ToList();
                Proyecto p = new Proyecto()
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Profesores = profesores
                };
                repoProyectos.Insert(p);
                repoProyectos.Save();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        //
        // GET: /Admin/ListaProfesores/
        public ActionResult ListaProfesores()
        {
            List<ApplicationUser> model = repoUsers.SelectAll().Where(u => u.Roles.Select(r => r.RoleId).Contains(profesor_rol_id)).ToList();
            return View(model);
        }

        //
        // GET: /Admin/AgregarProfesor/
        public ActionResult AgregarProfesor()
        {
            return View(new ProfesorViewModel() 
            {
                Proyectos = new SelectList(repoProyectos.SelectAll().Select(p => p.Nombre).ToList())
            });
        }

        //
        // POST: /Admin/AgregarProfesor/
        [HttpPost]
        public async Task<ActionResult> AgregarProfesor(ProfesorViewModel model)
        {
            model.Proyectos = new SelectList(repoProyectos.SelectAll().Select(p => p.Nombre).ToList());

            if( ModelState.IsValid )
            {
                ApplicationUser profesor;

                if (model.Proyecto != null)
                {
                    Proyecto p = AppContext.Proyectos.AsNoTracking().Where(z => z.Nombre == model.Proyecto).Single();

                    profesor = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        Nombre = model.Nombre,
                        Apellido = model.Apellido,
                        Cedula = model.Cedula,
                        PhoneNumber = model.Telefono,
                        Proyecto = p
                    };
                }
                else
                {
                    profesor = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        Nombre = model.Nombre,
                        Apellido = model.Apellido,
                        Cedula = model.Cedula,
                        PhoneNumber = model.Telefono
                    };
                }

                var profesorResult = await UserManager.CreateAsync(profesor, model.PasswordHash);

                if( profesorResult.Succeeded )
                {
                    var roleResult = await UserManager.AddToRoleAsync(profesor.Id, "Profesor");

                    if( !roleResult.Succeeded )
                    {
                        ModelState.AddModelError("", roleResult.Errors.First());
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", profesorResult.Errors.First());
                    return View(model);
                }

                return RedirectToAction("ListaProfesores");
            }

            return View(model);
        }

        //
        // GET: /Admin/EditarProfesor/1
        public async Task<ActionResult> EditarProfesor(string id)
        {
            if (id == null)
            {
                return RedirectToAction("ListaProfesores");
            }
            var profesor = await UserManager.FindByIdAsync(id);
            if (profesor == null)
            {
                return RedirectToAction("ListaProfesores");
            }

            if (profesor.Proyecto != null)
            {
                return View(new ProfesorEditViewModel()
                {
                    Id = profesor.Id,
                    Nombre = profesor.Nombre,
                    Apellido = profesor.Apellido,
                    Email = profesor.Email,
                    Telefono = profesor.PhoneNumber,
                    Cedula = profesor.Cedula,
                    Proyectos = new SelectList(repoProyectos.SelectAll().Select(p => p.Nombre).ToList(), profesor.Proyecto.Nombre)
                });
            }
            else
            {
                return View(new ProfesorEditViewModel()
                {
                    Id = profesor.Id,
                    Nombre = profesor.Nombre,
                    Apellido = profesor.Apellido,
                    Email = profesor.Email,
                    Telefono = profesor.PhoneNumber,
                    Cedula = profesor.Cedula,
                    Proyectos = new SelectList(repoProyectos.SelectAll().Select(p => p.Nombre).ToList())
                });
            }
        }

        //
        // POST: /Admin/EditarProfesor/1
        [HttpPost]
        public async Task<ActionResult> EditarProfesor(ProfesorEditViewModel model)
        {
            model.Proyectos = new SelectList(repoProyectos.SelectAll().Select(p => p.Nombre).ToList());

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = model.Email;
                user.Email = model.Email;
                user.Nombre = model.Nombre;
                user.Apellido = model.Apellido;
                user.Cedula = model.Cedula;
                user.PhoneNumber = model.Telefono;

                if( model.Proyecto != null )
                {
                    user.Proyecto = AppContext.Proyectos.Where(p => p.Nombre == model.Proyecto).Single();
                }
                else
                {
                    user.Proyecto = null;
                }

                var result = await UserManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View(model);
                }
                return RedirectToAction("ListaProfesores");
            }
            ModelState.AddModelError("", "Algo falló.");
            return View(model);
        }

        //
        // POST: /Admin/BorrarProfesor/5
        [HttpPost]
        public async Task<ActionResult> BorrarProfesor(string id)
        {
            if (id == null)
            {
                return RedirectToAction("ListaProfesores");
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("ListaProfesores");
            }
            var result = await UserManager.DeleteAsync(user);

            return RedirectToAction("ListaProfesores");
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