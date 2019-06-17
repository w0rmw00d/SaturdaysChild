using Owin;
using Microsoft.Owin;
using SaturdaysChild.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartupAttribute(typeof(SaturdaysChild.Startup))]
namespace SaturdaysChild
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUsers();
        }

        /// <summary>
        /// Creates site roles if none currently exist and default super user.
        /// </summary>
        private void CreateRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // creating first admin account if none is present (default admin)
            // NOTE: admin role represents site admin
            if (!roleManager.RoleExists("admin"))
            {
                // creating admin role
                var role = new IdentityRole() { Name = "admin" };
                roleManager.Create(role);
                // creating default admin user/superuser
                var user = new ApplicationUser() { UserName = "defaultadmin", Email = "saturdays.child.designs@gmail.com" };
                var password = "sampleboreddev904682";
                var defaultUser = userManager.Create(user, password);
                // adding default user to Role Admin
                if (defaultUser.Succeeded) userManager.AddToRole(user.Id, "admin");
            }

            // creating manager role
            // NOTE: manager role represents management which may or may not have admin privileges
            if (!roleManager.RoleExists("manager"))
            {
                var role = new IdentityRole() { Name = "manager" };
                roleManager.Create(role);
            }
            
            // creating employee role
            // NOTE: employee role represents employees, which do not have manager or admin privileges
            if (!roleManager.RoleExists("employee"))
            {
                var role = new IdentityRole() { Name = "employee" };
                roleManager.Create(role);
            }
            // creating user role
            // NOTE: user role represents site members/clients, with primarily read only privileges
            if (!roleManager.RoleExists("user"))
            {
                var role = new IdentityRole() { Name = "user" };
                roleManager.Create(role);
            }
        }
    }
}