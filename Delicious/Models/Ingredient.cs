using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Delicious.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        [Display(Name = "Sastojci")]
        public string IngredientName { get; set; }
      
        public virtual ICollection<RecipeIngredient> Recipes { get; set; }
    }
}