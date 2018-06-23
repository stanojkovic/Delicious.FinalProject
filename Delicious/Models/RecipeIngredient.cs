using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Delicious.Models
{
    public class RecipeIngredient
    {
        public enum Unit
        {
            kg,
            gr,
            l,
            dl,
            ml,
            kasicica
        }

        public int Id { get; set; }

        public decimal Quantity { get; set; }
        public Unit UnitOfMeasure { get; set; }

        [NotMapped]
        public bool Selected { get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}