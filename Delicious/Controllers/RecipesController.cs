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

namespace Delicious.Controllers
{
    public class RecipesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Recipes
        public ActionResult Index()
        {
            return View(db.Recipes.ToList());
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
        public ActionResult Create(Recipe recipe, int categoryId , HttpPostedFileBase img)  //ICollection<Ingredient> ingredientId
        {
            if (ModelState.IsValid)
            {
                //snimanje u bazu
                //recipe.Category = db.Categories.Find(recipe.Category.Id);
                recipe.Category = db.Categories.Find(categoryId);

                //cast conversion, lista sastojaka , lista ingredientId

                //recipe.Ingredients = db.Ingredients.Find(ingredientId);
                //recipe.Ingredients = db.Ingredients.Find(recipe.Ingredients.Id);
                


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
        public ActionResult Edit(Recipe recipeForm, HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {

                //nadji proizvod iz baze
                var recipeBase = db.Recipes.Find(recipeForm.Id);
                //update vrednostima iz forme, proizvodIzBaze!!!
                TryUpdateModel(recipeBase, new string[] { "RecipeName", "Description", });
                //dodela kategorije
                //recipeBase.Category = db.Categories.Find(recipeForm.Category.Id);

                

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
    }
}
