using TimeKeeper.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using TimeKeeper.Application.Common.Models;

namespace TimeKeeper.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles.All)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var defaultUser = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost" };
            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                await userManager.CreateAsync(defaultUser, "administrator");
                await userManager.AddToRoleAsync(defaultUser, Roles.Admin);
            }
        }
    }
}
