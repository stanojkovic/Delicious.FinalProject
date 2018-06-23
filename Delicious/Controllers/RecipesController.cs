﻿using System;
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

        // GET: Recipes
        public ActionResult Index(RecipeGridViewModel viewModel, string kategorija)
        {
            IQueryable<Recipe> recipes = db.Recipes;

            if (kategorija != null)
            {
                recipes = recipes.Where(r => r.Category.CategoriesName == kategorija);
                ViewBag.Kategorija = kategorija;
            }

            if (viewModel.Query != null)
            {
                recipes = recipes.Where(r => r.RecipeName.Contains(viewModel.Query));
            }

            //&& (kategorija =="Kolači" || kategorija == "Peciva" || kategorija == "Zdrava Hrana")
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
            //SetIngredients();

            var allIngredients = db.Ingredients.ToList();
            var recipeIngredientList = new List<RecipeIngredient>();

            // ekvivalentno allIngredients.Select(i => new RecipeIngredient() { Ingredient = i }).ToList()
            foreach (var item in allIngredients)
            {
                recipeIngredientList.Add(new RecipeIngredient() { Ingredient = item });
            }

            var model = new Recipe()
            {
                // napravi mi listu RecipeIngredient objekata
                Ingredients = recipeIngredientList // allIngredients.Select(i => new RecipeIngredient() { Ingredient = i }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Recipe formRecipeData, HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {
                var selectedRecipeIngredients = formRecipeData.Ingredients.Where(x => x.Selected);

                var recipeForDB = new Recipe()
                {
                    InputDate = DateTime.Now,
                    Category = db.Categories.Find(formRecipeData.Category.Id),
                    Description = formRecipeData.Description,
                    RecipeName = formRecipeData.RecipeName
                };

                db.Recipes.Add(recipeForDB);
                db.SaveChanges();

                recipeForDB.Ingredients = selectedRecipeIngredients.Select(ri =>
                                               new RecipeIngredient()
                                               {
                                                   Ingredient = db.Ingredients.Find(ri.Ingredient.Id),
                                                   Quantity = ri.Quantity,
                                                   Recipe = recipeForDB
                                               }
                                           ).ToList();


                db.SaveChanges();

                //da bi se nakon kreiranja proizvoda vratili na stranicu na kojoj se prikazuje
                //odgovarajuca kategorija, a ne recepti svih kategorija
                var kategorija = recipeForDB.Category.CategoriesName;

                SaveImage(recipeForDB, img);
                db.SaveChanges();
                return RedirectToAction("Index", new { kategorija });

            }

            SetCategory();
            return View(formRecipeData);
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

            //var allIngredients = db.Ingredients.ToList();
            //var recipeIngredientList = new List<RecipeIngredient>();


            //foreach (var item in allIngredients)
            //{
            //    recipeIngredientList.Add(new RecipeIngredient() { Ingredient = item });
            //}

            //var model = new Recipe()
            //{
            //    // napravi mi listu RecipeIngredient objekata
            //    Ingredients = recipeIngredientList
            //};

            SetCategory();
            //SetIngredients();
            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Recipe recipeForm, HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {
                //nadji proizvod iz baze
                var recipeBase = db.Recipes.Find(recipeForm.Id);
                TryUpdateModel(recipeBase, new string[] { "RecipeName", "Description" });

                db.RecipeIngredients.RemoveRange(recipeBase.Ingredients);               

                recipeBase.Ingredients = recipeForm.Ingredients.Select(ri =>
                               new RecipeIngredient()
                               {
                                   Ingredient = db.Ingredients.Find(ri.Ingredient.Id),
                                   Quantity = ri.Quantity,
                                   Recipe = recipeBase
                               }
                           ).ToList();

                recipeBase.Category = db.Categories.Find(recipeForm.Category.Id);
                recipeForm.InputDate = DateTime.Now;

                //da bi se nakon kreiranja proizvoda vratili na stranicu na kojoj se prikazuje
                //odgovarajuca kategorija, a ne recepti svih kategorija
                var kategorija = recipeBase.Category.CategoriesName;

                SaveImage(recipeBase, img);
                db.SaveChanges();

                return RedirectToAction("Index", new { kategorija });
            }

            SetCategory();
            //SetIngredients();
            return View(recipeForm);
        }


        // GET: Recipes/Delete/5
        [Authorize(Roles = RolesConfig.ADMIN)]
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
        [Authorize(Roles = RolesConfig.ADMIN)]
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

            //ne radi, a nece kao kod create i edit, nije radilo jer je zaboravljeno virtual u  public virtual Category Category { get; set; }
            var kategorija = recipe.Category.CategoriesName;

            //mora prvo da se obrisu redovi iz tabele RecipeIngredients zbog stranog kljuca Recipe_Id
            var recpieIngredients = db.RecipeIngredients.Where(ri => ri.Recipe.Id == id);
            foreach(var ri in recpieIngredients)
            {
                db.RecipeIngredients.Remove(ri);
            }

            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("Index", new { kategorija });
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
