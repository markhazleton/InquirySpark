using ControlSpark.WebMvc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace InquirySpark.Admin.Data;

public static class SeedRoles
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ControlSparkUser>>();

        // Create roles if they don't exist
        string[] roles = { "Analyst", "Operator", "Consultant", "Executive", "Administrator" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Optional: Assign first registered user as Analyst for testing
        // Uncomment and replace with your email after registering
        /*
        var testUser = await userManager.FindByEmailAsync("your-email@example.com");
        if (testUser != null && !await userManager.IsInRoleAsync(testUser, "Analyst"))
        {
            await userManager.AddToRoleAsync(testUser, "Analyst");
        }
        */
    }
}
