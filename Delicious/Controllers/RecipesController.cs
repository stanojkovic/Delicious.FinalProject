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
    public class RecipesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public object IngredientName { get; private set; }

        // GET: Recipes
        public ActionResult Index(RecipeGridViewModel viewModel)
        {
            if (Request.HttpMethod == "POST")
            {
                viewModel.Page = 1;
            }

            IQueryable<Recipe> recipes = db.Recipes;

            if (viewModel.Query != null)
            {
                recipes = recipes.Where(r => r.RecipeName.Contains(viewModel.Query));
            }

            if (viewModel.SortBy != null && viewModel.SortDirection != null)
            {
                recipes = recipes.OrderBy(string.Format("{0} {1}", viewModel.SortBy, viewModel.SortDirection));
            }

            ViewBag.Direction = viewModel.SortDirection == "ASC" ? "DESC" : "ASC";

            viewModel.Count = recipes.Count();
            recipes = recipes.Skip((viewModel.Page - 1) * viewModel.PageSize).Take(viewModel.PageSize);

            //vrati podatke iz baze
            viewModel.Recipes = recipes.ToList();


            return View(viewModel);
        }

        // GET: Recipes/Details/5
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
        public ActionResult Create()
        {
            SetCategory();
            SetIngredients();
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,RecipeName,Description,ImageNameToShow,InputDate")] Recipe recipe, HttpPostedFileBase img)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //recipe.Category = db.Categories.Find(recipe.Category.Id);
        //        recipe.Id = Guid.NewGuid();
        //        db.Recipes.Add(recipe);
        //        db.SaveChanges();
        //        SaveImage(recipe, img);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    SetCategory();
        //    return View(recipe);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Recipe recipe, int categoryId , int[] IngredientsIds, HttpPostedFileBase img)  //ICollection<Ingredient> ingredientId
        {
            if (ModelState.IsValid)
            {
                //snimanje u bazu
                //recipe.Category = db.Categories.Find(recipe.Category.Id);
                recipe.Category = db.Categories.Find(categoryId);

                if (IngredientsIds != null)
                {
                    recipe.Ingredients = db.Ingredients.Where(x => IngredientsIds.Contains(x.Id)).ToList();
                }


              
                recipe.Id = Guid.NewGuid();
                db.Recipes.Add(recipe);
                db.SaveChanges();
                SaveImage(recipe, img);
                db.SaveChanges();
                return RedirectToAction("Index");

            }

                  
            SetCategory();
            SetIngredients();
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public ActionResult Edit(Guid? id)
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
            SetCategory();
            SetIngredients();
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,RecipeName,Description,ImageNameToShow,InputDate")] Recipe recipe, HttpPostedFileBase img)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(recipe).State = EntityState.Modified;
        //        db.SaveChanges();
        //        SaveImage(recipe, img);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    SetCategory();
        //    return View(recipe);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Recipe recipeForm, int categoryId, int[] IngredientsIds, HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {
                //nadji proizvod iz baze
                var recipeBase = db.Recipes.Find(recipeForm.Id);
                TryUpdateModel(recipeBase, new string[] { "RecipeName", "Description", "Category", "Ingredients"});
                recipeBase.Category = db.Categories.Find(categoryId);

                //recipe.Ingredients.Clear();
                //if (IngredientsIds != null)
                //{
                //    recipe.Ingredients = db.Ingredients.Where(x => IngredientsIds.Contains(x.Id)).ToList();
                //}

                //update vrednostima iz forme, proizvodIzBaze!!!
                
                

                SaveImage(recipeBase, img);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
           
            SetCategory();
            SetIngredients();
            return View(recipeForm); 
        }


        // GET: Recipes/Delete/5
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
        public ActionResult DeleteConfirmed(Guid id)
        {
            Recipe recipe = db.Recipes.Find(id);
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void SaveImage(Recipe recipe, HttpPostedFileBase img)
        {
            if (img != null)
            {
                //recipe.ImageName = img.FileName;
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

        public void SetIngredients()
        {
            ViewBag.Ingredients = db.Ingredients.ToList();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //public ActionResult AddIngredients(int id, int []IngredientsIds)
        //{
        //    var item = db.Recipes.Find(id);
        //    var ingredients = db.Ingredients.Where(x => IngredientsIds.Contains(x.Id)).ToList();

            
           

        //}
    }
}
