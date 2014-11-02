using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;

namespace IdentitySample.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = false,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = false;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole,string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }
    
    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext> 
    {
        protected override void Seed(ApplicationDbContext context) {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db) {

            if (db == null)
            {
                throw new ArgumentNullException("db", "Context no puede ser nulo.");
            }
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

            string roleName = "Admin";
            string coordinadorRole = "Coordinador";
            string profesorRole = "Profesor";

            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }

            if (!roleManager.RoleExists(coordinadorRole))
            {
                roleManager.Create(new IdentityRole(coordinadorRole));
            }

            if (!roleManager.RoleExists(profesorRole))
            {
                roleManager.Create(new IdentityRole(profesorRole));
            }

            //// Admin
            string userName = "admin@cit.com";
            string password = "admin123";
            string email = "admin@cit.com";

            ApplicationUser user = userManager.FindByName(userName);

            if (user == null)
            {
                IdentityResult result = userManager.Create(new ApplicationUser { UserName = userName, Email = email }, password);
                user = userManager.FindByName(userName);
            }

            if (!userManager.IsInRole(user.Id, roleName))
            {
                userManager.AddToRole(user.Id, roleName);
            }

            ///////////////////////////////

            //// Coordinador

            string coorUsername = "coordinador@cit.com";
            string coorEmail = "coordinador@cit.com";
            string coorPwd = "coordinador123";

            ApplicationUser coorUser = userManager.FindByName(coorUsername);

            if (coorUser == null)
            {
                IdentityResult result = userManager.Create(new ApplicationUser { UserName = coorUsername, Email = coorEmail }, coorPwd);
                coorUser = userManager.FindByName(coorUsername);
            }

            if (!userManager.IsInRole(coorUser.Id, coordinadorRole))
            {
                userManager.AddToRole(coorUser.Id, coordinadorRole);
            }

            //////////////////////////////

            //// Profesor

            string profUsername = "profesor@cit.com";
            string profEmail = "profesor@cit.com";
            string profPwd = "profesor123";

            ApplicationUser profUser = userManager.FindByName(profUsername);

            if (profUser == null)
            {
                IdentityResult result = userManager.Create(new ApplicationUser { UserName = profUsername, Email = profEmail, Nombre = "Carlos", Apellido = "Martinez", Cedula = "23.522.896" }, profPwd);
                profUser = userManager.FindByName(profUsername);
            }

            if (!userManager.IsInRole(profUser.Id, profesorRole))
            {
                userManager.AddToRole(profUser.Id, profesorRole);
            }

            ////////////////////////////

            profUsername = "profesor2@cit.com";
            profEmail = "profesor2@cit.com";
            profPwd = "profesor123";

            profUser = userManager.FindByName(profUsername);

            if (profUser == null)
            {
                IdentityResult result = userManager.Create(new ApplicationUser { UserName = profUsername, Email = profEmail, Nombre = "Joseito", Apellido = "Martinez", Cedula = "23.522.896" }, profPwd);
                profUser = userManager.FindByName(profUsername);
            }

            if (!userManager.IsInRole(profUser.Id, profesorRole))
            {
                userManager.AddToRole(profUser.Id, profesorRole);
            }

            //////////////////////////////
        }
    }



    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : 
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}