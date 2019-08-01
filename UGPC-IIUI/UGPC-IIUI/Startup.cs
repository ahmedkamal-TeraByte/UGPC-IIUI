using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using UGPC_IIUI.Models;

[assembly: OwinStartupAttribute(typeof(UGPC_IIUI.Startup))]
namespace UGPC_IIUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //to create a default admin user
                {
                    //Here we create a Admin super user who will maintain the website                  

                    //                var user = new ApplicationUser();
                    //                user.UserName = "shanu";
                    //                user.Email = "syedshanumcain@gmail.com";
                    //
                    //                string userPWD = "A@Z200711";
                    //
                    //                var chkUser = userManager.Create(user, userPWD);
                    //
                    //                //Add default User to Role Admin   
                    //                if (chkUser.Succeeded)
                    //                {
                    //                    var result1 = userManager.AddToRole(user.Id, "Admin");
                    //
                    //                }
                }
            }

            // Creating Student role    
            if (!roleManager.RoleExists("Student"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Student";
                roleManager.Create(role);

            }

            // Creating Supervisor role    
            if (!roleManager.RoleExists("Supervisor"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Supervisor";
                roleManager.Create(role);

            }
        }

    }
}
