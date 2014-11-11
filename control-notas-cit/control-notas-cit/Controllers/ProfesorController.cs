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
        private IRepositorioGenerico<IdentityRole> repoRoles = null;

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
            this.repoRoles = new RepositorioGenerico<IdentityRole>(AppContext);
        }

        //
        // GET: /Profesor/Index/
        public ActionResult Index()
        {
            ProfesorIndexViewModel model = new ProfesorIndexViewModel();

            // Obtengo el usuario
            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction("Logoff", "Account");
            }

            if(currentUser.Proyecto == null)
            {
                return RedirectToAction("Logoff", "Account");
            }

            // Utilizo el usuario para obtener los demas datos del modelo de la vista
            model.NombreProfesor = currentUser.Nombre + " " + currentUser.Apellido;
            model.Proyecto = GetCurrentProyecto();

            // Calendario será null si la query no devuelve un valor, en el caso de que sea null, la vista mostrara un mensaje
            model.Calendario = repoCalendarios.SelectAll()
                                .Where(c => c.CalendarioID == model.Proyecto.CalendarioActualID)
                                .SingleOrDefault();

            return View(model);
        }

        //
        // GET: /Profesor/EditarProyecto/
        //
        public ActionResult EditarProyecto()
        {
            var proyecto = GetCurrentProyecto();

            if (proyecto == null)
            {
                return RedirectToAction("Index");
            }

            var model = new ProjectEditViewModel()
            {
                Id = proyecto.ProyectoID,
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion
            };

            return View(model);
        }

        //
        // POST: /Profesor/EditarProyecto/
        [HttpPost]
        public ActionResult EditarProyecto(ProjectEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var proyecto = repoProyectos.SelectById(model.Id);

                if (proyecto == null)
                {
                    ModelState.AddModelError("", "No se pudo encontrar el proyecto");
                    return View(model);
                }

                proyecto.Nombre = model.Nombre;
                proyecto.Descripcion = model.Descripcion;

                repoProyectos.Update(proyecto);
                repoProyectos.Save();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        //
        // GET: /Profesor/AgregarCalendario/
        public ActionResult AgregarCalendario()
        {
            List<Semana> semanas = new List<Semana>();

            for (int i = 1; i <= 12; i++)
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

                if (user == null)
                {
                    return HttpNotFound("Usuario no encontrado");
                }

                var proyecto = user.Proyecto;

                if (proyecto == null)
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

            if(model == null)
            {
                return RedirectToAction("Index");
            }

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

        //
        // GET: /Profesor/Celulas/
        public ActionResult Celulas()
        {
            return View(repoCelulas.SelectAll().Where(c => c.Proyecto.ProyectoID == GetCurrentProyecto().ProyectoID).ToList());
        }

        //
        // GET: /Profesor/AgregarCelula/
        public ActionResult AgregarCelula()
        {
            return View(new CelulaViewModel()
            {
                Coordinadores = new MultiSelectList(GetCoordinadoresLibresList(), "Value", "Text")
            });
        }

        //
        // POST: /Profesor/AgregarCelula/
        [HttpPost]
        public ActionResult AgregarCelula(CelulaViewModel model)
        {
            model.Coordinadores = new MultiSelectList(GetCoordinadoresLibresList(), "Value", "Text");

            if (ModelState.IsValid)
            {
                List<ApplicationUser> coordinadores = repoUsers.SelectAll().Where(u => model.CoordinadoresID.Contains(u.Id)).ToList();

                Celula celula = new Celula()
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Coordinadores = coordinadores,
                    Proyecto = GetCurrentProyecto()
                };

                repoCelulas.Insert(celula);
                repoCelulas.Save();

                return RedirectToAction("Celulas");
            }
            return View(model);
        }

        //
        // GET: /Profesor/EditarCelula/
        public ActionResult EditarCelula(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Celulas");
            }

            var celula = repoCelulas.SelectById(id);

            if(celula == null)
            {
                return RedirectToAction("Celulas");
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
        // POST: /Profesor/EditarCelula/
        [HttpPost]
        public ActionResult EditarCelula(CelulaEditViewModel model)
        {
            if(ModelState.IsValid)
            {
                var celula = repoCelulas.SelectById(model.Id);

                if(celula == null)
                {
                    ModelState.AddModelError("", "No se pudo encontrar la celula");
                    return View(model);
                }

                celula.Nombre = model.Nombre;
                celula.Descripcion = model.Descripcion;

                repoCelulas.Update(celula);
                repoCelulas.Save();

                return RedirectToAction("Celulas");
            }

            return View(model);
        }

        //
        // GET: /Profesor/ListaCoordinadores/
        public ActionResult ListaCoordinadores()
        {
            var model = GetCoordinadores().OrderBy(c => c.Nombre).ThenBy(c => c.Apellido).ToList();
            return View(model);
        }

        //
        // GET: /Profesor/AgregarCoordinador/
        public ActionResult AgregarCoordinador()
        {
            return View(new CoordinadorViewModel()
            {
                Celulas = new SelectList(GetCelulasList(), "Value", "Text")
            });
        }

        //
        // POST: /Profesor/AgregarCoordinador/
        [HttpPost]
        public async Task<ActionResult> AgregarCoordinador(CoordinadorViewModel model)
        {
            model.Celulas = new SelectList(repoCelulas.SelectAll().Select(c => c.Nombre).ToList());

            if (ModelState.IsValid)
            {
                ApplicationUser coordinador;

                coordinador = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    Cedula = model.Cedula,
                    Proyecto = GetCurrentProyecto(),
                    PhoneNumber = model.Telefono
                };

                if (model.Celula != null)
                {
                    Celula celula = repoCelulas.SelectAll().Where(c => c.CelulaID == Int32.Parse(model.Celula)).Single();
                    coordinador.Celula = celula;
                }

                var coordinadorResult = await UserManager.CreateAsync(coordinador, model.PasswordHash);

                if (coordinadorResult.Succeeded)
                {
                    var roleResult = await UserManager.AddToRoleAsync(coordinador.Id, "Coordinador");

                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", roleResult.Errors.First());
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", coordinadorResult.Errors.First());
                    return View(model);
                }

                return RedirectToAction("ListaCoordinadores");
            }

            return View(model);
        }

        //
        // GET: /Profesor/EditarCoordinador/42
        public async Task<ActionResult> EditarCoordinador(string id)
        {
            if (id == null)
            {
                return RedirectToAction("ListaCoordinadores");
            }
            var coordinador = await UserManager.FindByIdAsync(id);
            if (coordinador == null)
            {
                return RedirectToAction("ListaCoordinadores");
            }

            var model = new CoordinadorEditViewModel()
            {
                Id = coordinador.Id,
                Nombre = coordinador.Nombre,
                Apellido = coordinador.Apellido,
                Email = coordinador.Email,
                Telefono = coordinador.PhoneNumber,
                Cedula = coordinador.Cedula
            };

            if(coordinador.Celula != null)
            {
                // bug
                model.Celulas = new SelectList(GetCelulasList(), "Value", "Text", coordinador.Celula.CelulaID.ToString());
            }
            else
            {
                model.Celulas = new SelectList(GetCelulasList(), "Value", "Text");
            }

            return View(model);
        }

        //
        // POST: /Profesor/EditarCoordinador/
        [HttpPost]
        public async Task<ActionResult> EditarCoordinador(CoordinadorEditViewModel model)
        {
            model.Celulas = new SelectList(GetCelulasList(), "Value", "Text");

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

                if (model.CelulaID != null)
                {
                    user.Celula = repoCelulas.SelectById(Int32.Parse(model.CelulaID));
                }
                else
                {
                    ModelState.AddModelError("", "Debes elegir una celula");
                    return View(model);
                }

                var result = await UserManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View(model);
                }
                return RedirectToAction("ListaCoordinadores");
            }
            ModelState.AddModelError("", "Algo falló.");
            return View(model);
        }

        //
        // POST: /Profesor/BorrarCoordinador/42
        [HttpPost]
        public async Task<ActionResult> BorrarCoordinador(string id)
        {
            if (id == null)
            {
                return RedirectToAction("ListaCoordinadores");
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("ListaCoordinadores");
            }
            var result = await UserManager.DeleteAsync(user);

            return RedirectToAction("ListaCoordinadores");
        }

        //
        // GET: /Profesor/MinutasSemana/1
        public ActionResult MinutasSemana(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            var semana = repoSemanas.SelectById(id);

            if(semana == null)
            {
                return RedirectToAction("Index");
            }

            var model = GetCurrentProyecto().Celulas.SelectMany(c => c.Minutas.Where(m => m.Semana.SemanaID == semana.SemanaID)).ToList();

            return View(model);
        }

        // Obtiene el usuario logueado actualmente
        private ApplicationUser GetCurrentUser()
        {
            // Tira una excepcion cuando el explorador ya tiene una sesion iniciada, debido a que al ejecutar el Seed, el id es totalmente distinto
            return repoUsers.SelectAll().Where(u => u.Id == User.Identity.GetUserId()).SingleOrDefault();
        }

        private Proyecto GetCurrentProyecto()
        {
            return GetCurrentUser().Proyecto;
        }

        private string GetCoordinadorRoleID()
        {
            return repoRoles.SelectAll().Where(r => r.Name == "Coordinador").Select(s => s.Id).Single();
        }

        private List<ApplicationUser> GetCoordinadores()
        {
            var proyecto = GetCurrentProyecto();
            return proyecto.Profesores.Where(u => u.Roles.Select(x => x.RoleId).Contains(GetCoordinadorRoleID())).ToList();
        }

        private List<SelectListItem> GetCelulasList()
        {
            var proyecto = GetCurrentProyecto();

            List<SelectListItem> items = proyecto.Celulas.Where(c => c.Proyecto.ProyectoID == proyecto.ProyectoID).Select(c => new SelectListItem() { Value = c.CelulaID.ToString(), Text = c.Nombre }).ToList();

            return items;
        }

        private List<SelectListItem> GetCoordinadoresList()
        {
            List<ApplicationUser> users = GetCoordinadores();
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (ApplicationUser u in users)
            {
                items.Add(new SelectListItem()
                {
                    Text = u.Nombre + " " + u.Apellido,
                    Value = u.Id
                });
            }

            return items;
        }

        private List<SelectListItem> GetCoordinadoresLibresList()
        {
            List<ApplicationUser> users = GetCoordinadores();

            List<ApplicationUser> libres = users.Where(x => x.Celula == null).ToList();

            List<SelectListItem> items = new List<SelectListItem>();

            foreach (ApplicationUser u in libres)
            {
                items.Add(new SelectListItem()
                {
                    Text = u.Nombre + " " + u.Apellido,
                    Value = u.Id
                });
            }

            return items;
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