using DAL.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Persistent
{
    public class AppDbContextInitializer
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AppDbContextInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (context.Database.IsSqlServer())
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SeedDatabase()
        {
            var adminRole = new IdentityRole("Admin");
            var userRole = new IdentityRole("User");

            if (roleManager.Roles.All(r => r.Name != adminRole.Name))
            {
                await roleManager.CreateAsync(adminRole);
            }

            if (roleManager.Roles.All(r => r.Name != userRole.Name))
            {
                await roleManager.CreateAsync(userRole);
            }

            var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost"};
            var user = new ApplicationUser { UserName = "oxyman21@mail.ru", Email = "oxyman21@mail.ru" };

            if (userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await userManager.CreateAsync(administrator, "Administrator1!");
                if (!string.IsNullOrWhiteSpace(adminRole.Name))
                {
                    await userManager.AddToRolesAsync(administrator, new[] { adminRole.Name });
                }
            }

            if (userManager.Users.All(u => u.UserName != user.UserName))
            {
                await userManager.CreateAsync(user, "Administrator1!");
                if (!string.IsNullOrWhiteSpace(userRole.Name))
                {
                    await userManager.AddToRolesAsync(user, new[] { userRole.Name });
                }
            }
        }
    }
}
