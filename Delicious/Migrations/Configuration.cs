namespace Delicious.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Delicious.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
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

        }
    }
}
