namespace Delicious.Migrations
{
    using Delicious.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Configuration;

    internal sealed class Configuration : DbMigrationsConfiguration<Delicious.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "Delicious.Models.ApplicationDbContext";
        }

        protected override void Seed(Delicious.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Categories.AddOrUpdate(
                p => p.CategoriesName,
                 new Models.Category { CategoriesName = "Kolаci" },
                 new Models.Category { CategoriesName = "Peciva" },
                 new Models.Category { CategoriesName = "Zdrava Hrana" }
                );

            context.Ingredients.AddOrUpdate(
                p => p.IngredientName,
                 new Models.Ingredient { IngredientName = "So" },
                 new Models.Ingredient { IngredientName = "Secer" },
                 new Models.Ingredient { IngredientName = "Brasno" }
                );


            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            CreateRoleIfDoesNotExist(RolesConfig.ADMIN, roleManager);
            CreateRoleIfDoesNotExist(RolesConfig.USER, roleManager);


            var adminEmail = WebConfigurationManager.AppSettings["AdminEmail"];
            var adminPassword = WebConfigurationManager.AppSettings["AdminPassword"];

            if (!context.Users.Any(u => u.Email == adminEmail))
            {
                var adminManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                var admin = new ApplicationUser()
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    PasswordHash = adminManager.PasswordHasher.HashPassword(adminPassword),
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                context.Users.Add(admin);
                context.SaveChanges();

                adminManager.AddToRole(admin.Id, RolesConfig.ADMIN);
            }
        }


        private static void CreateRoleIfDoesNotExist(string roleName, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }
    }
}
