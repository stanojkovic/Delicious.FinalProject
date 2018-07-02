using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Delicious.Models;
using System.Linq.Dynamic;


namespace Delicious.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Recipes
        [AllowAnonymous]
        public ActionResult Index(RecipeGridViewModel viewModel, string kategorija)
        {
            IQueryable<Recipe> recipes = db.Recipes;

            if (kategorija != null)
            {
                recipes = recipes.Where(r => r.Category.CategoriesName == kategorija);
            }

            if (viewModel.Query != null)
            {
                recipes = recipes.Where(r => r.RecipeName.Contains(viewModel.Query));
            }

            if (viewModel.SortBy != null && viewModel.SortDirection != null)
            {
                recipes = recipes.OrderBy(string.Format("{0} {1}", viewModel.SortBy, viewModel.SortDirection));
            }

            viewModel.Count = recipes.Count();
            recipes = recipes.Skip((viewModel.Page - 1) * viewModel.PageSize).Take(viewModel.PageSize);

            //vrati podatke iz baze
            viewModel.Recipes = recipes.ToList();


            return View(viewModel);
        }

        // GET: Recipes/Details/5
        [AllowAnonymous]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // GET: Recipes/Create
        [Authorize(Roles = RolesConfig.USER)]
        public ActionResult Create()
        {
            SetCategory();

            var allIngredients = db.Ingredients.ToList();
            var recipeIngredientList = new List<RecipeIngredient>();

            foreach (var item in allIngredients)
            {
                recipeIngredientList.Add(new RecipeIngredient() { Ingredient = item });
            }

            var model = new Recipe()
            {
                // napravi mi listu RecipeIngredient objekata
                Ingredients = recipeIngredientList
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.USER)]
        public ActionResult Create(Recipe formRecipeData, HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {
                var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

                var selectedRecipeIngredients = formRecipeData.Ingredients.Where(x => x.Selected);

                var recipeForDB = new Recipe()
                {
                    InputDate = DateTime.Now,
                    Category = db.Categories.Find(formRecipeData.Category.Id),
                    Description = formRecipeData.Description,
                    RecipeName = formRecipeData.RecipeName,
                    User = currentUser
                };

                db.Recipes.Add(recipeForDB);
                db.SaveChanges();

                recipeForDB.Ingredients = selectedRecipeIngredients.Select(ri =>
                                               new RecipeIngredient()
                                               {
                                                   Ingredient = db.Ingredients.Find(ri.Ingredient.Id),
                                                   Quantity = ri.Quantity,
                                                   Recipe = recipeForDB,
                                                   UnitOfMeasure = ri.UnitOfMeasure
                                               }
                                           ).ToList();


                db.SaveChanges();

                SaveImage(recipeForDB, img);
                db.SaveChanges();
                return RedirectToAction("MyRecipes");
            }

            SetCategory();
            return View(formRecipeData);
        }

        // GET: Recipes/Edit/5
        [Authorize(Roles = RolesConfig.USER)]
        public ActionResult Edit(Guid? id)
        {
            Recipe recipe = db.Recipes.Find(id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            if (recipe == null)
            {
                return HttpNotFound();
            }

            //da se pored cekiranih sastojaka dodaju i necekirani iz baze
            var allIngredients = db.Ingredients.ToList();
            foreach (Ingredient item in allIngredients)
            {
                bool n = false;

                foreach (RecipeIngredient ri in recipe.Ingredients)
                {
                    if (ri.Ingredient.Equals(item))
                    {
                        n = true;
                        ri.Selected = true;
                        break;
                    }
                }

                if (!n)
                    recipe.Ingredients.Add(new RecipeIngredient { Ingredient = item });
            }

            SetCategory();

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.USER)]
        public ActionResult Edit(Recipe recipeForm, HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {
                //nadji proizvod iz baze
                var recipeBase = db.Recipes.Find(recipeForm.Id);
                TryUpdateModel(recipeBase, new string[] { "RecipeName", "Description" });

                db.RecipeIngredients.RemoveRange(recipeBase.Ingredients);

                foreach (RecipeIngredient ri in recipeForm.Ingredients)
                {
                    if (ri.Selected)
                    {
                        recipeBase.Ingredients.Add(new RecipeIngredient()
                        {
                            Ingredient = db.Ingredients.Find(ri.Ingredient.Id),
                            Quantity = ri.Quantity,
                            Recipe = recipeBase,
                            UnitOfMeasure = ri.UnitOfMeasure
                        });
                    }
                }

                recipeBase.Category = db.Categories.Find(recipeForm.Category.Id);
                recipeForm.InputDate = DateTime.Now;

                SaveImage(recipeBase, img);
                db.SaveChanges();

                return RedirectToAction("MyRecipes");
            }

            SetCategory();

            return View(recipeForm);
        }


        // GET: Recipes/Delete/5
        [Authorize(Roles = RolesConfig.USER)]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.USER)]
        public ActionResult Delete(Guid id)
        {
            Recipe recipe = db.Recipes.Find(id);

            //brisemo sliku najpre iz Content foldera
            //ako stavim ImageNameToShow izbrisace i no_image.png
            var imagePath = Server.MapPath($"~/Content/img/{recipe.ImageName}");

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            //mora prvo da se obrisu redovi iz tabele RecipeIngredients zbog stranog kljuca Recipe_Id
            var recepieIngredients = db.RecipeIngredients.Where(ri => ri.Recipe.Id == id);
            foreach(var ri in recepieIngredients)
            {
                db.RecipeIngredients.Remove(ri);
            }

            //moraju da se izbrisu i komentari vezani za recept koji se brise
            var comments = db.Comments.Where(c => c.Recipe.Id == id);
            foreach(var comment in comments)
            {
                db.Comments.Remove(comment);
            }

            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("MyRecipes");
        }

        private void SaveImage(Recipe recipe, HttpPostedFileBase img)
        {
            if (img != null)
            {
                recipe.ImageName = Path.GetFileName(img.FileName);
                var imagePath = Server.MapPath($"~/Content/img/{recipe.ImageNameToShow}");
                img.SaveAs(imagePath);
            }
        }

        //izvuci podatke iz druge klase
        public void SetCategory()
        {
            ViewBag.Categories = db.Categories.ToList();
        }

        //za prikaz recepata koje je kreirao ulogovani korisnik
        [Authorize(Roles = RolesConfig.USER)]
        public ActionResult MyRecipes(RecipeGridViewModel viewModel)
        {
            IQueryable<Recipe> recipes = db.Recipes;

            if (User.IsInRole(RolesConfig.USER))
            {
                recipes = recipes.Where(r => r.User.UserName == User.Identity.Name);
            }

            if (viewModel.Query != null)
            {
                recipes = recipes.Where(r => r.RecipeName.Contains(viewModel.Query));
            }

            if (viewModel.SortBy != null && viewModel.SortDirection != null)
            {
                recipes = recipes.OrderBy(string.Format("{0} {1}", viewModel.SortBy, viewModel.SortDirection));
            }

            viewModel.Count = recipes.Count();
            recipes = recipes.Skip((viewModel.Page - 1) * viewModel.PageSize).Take(viewModel.PageSize);

            //vrati podatke iz baze
            viewModel.Recipes = recipes.ToList();


            return View("MyRecipes", viewModel);
        }

        //za ostavljenja komentara
        public ActionResult LeaveComment(Guid recipeId, string comment)
        {
            var commentedRecipe = db.Recipes.Find(recipeId);
            var user = db.Users.First(u => u.UserName == User.Identity.Name);

            var commentForDB = new Comment()
            {
                CommentContent = comment,
                Recipe = commentedRecipe,
                User = user,
                CommentInputDate = DateTime.Now
            };

            db.Comments.Add(commentForDB);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = recipeId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
