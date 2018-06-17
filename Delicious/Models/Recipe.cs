using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Delicious.Models
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }

        public string ImageNameToShow
        {
            get
            {
                return string.IsNullOrWhiteSpace(ImageName) ? "no_image.png" : string.Format("{0}", ImageName);
            }
        }

        public DateTime InputDate
        {
            get { return DateTime.Now; }
        }
        public Category Category { get; set; }


        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }
}