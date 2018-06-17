using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Delicious.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string IngredientName { get; set; }
      
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}