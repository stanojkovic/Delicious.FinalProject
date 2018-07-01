using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Delicious.Models
{
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Display(Name ="Naziv:")]
        public string RecipeName { get; set; }
        [Display(Name = "Priprema:")]
        public string Description { get; set; }
        [Display(Name = "OdaberiteSliku")]
        public string ImageName { get; set; }

      

        public string ImageNameToShow
        {
            get
            {
                return string.IsNullOrWhiteSpace(ImageName) ? "no_image.png" : string.Format("{0}", ImageName);
            }
        }

        [Display(Name = "Datum kreiranja recepta:")]
        public DateTime InputDate { get; set; }
        
        [Required]
        [Display(Name = "Kategorija:")]
        public virtual Category Category { get; set; }


        public virtual ICollection<RecipeIngredient> Ingredients { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}