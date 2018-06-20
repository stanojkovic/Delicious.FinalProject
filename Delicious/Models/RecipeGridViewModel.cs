using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Delicious.Models
{
    public class RecipeGridViewModel
    {

        public string Query { get; set; }


        //za sortiranje
        public string SortBy { get; set; } = "RecipeName";
        //public string SortByCategory { get; set; }
        public string SortDirection { get; set; } = "ASC";
        //lista za grid
        public List<Recipe> Recipes { get; set; }

        //za ispis heading na Index
        public string kategorija { get; set; }

        //za paginaciju
        public int PageSize { get; set; } = 6;
        public int Page { get; set; } = 1;
        public int Count { get; set; }


        public int TotalPages
        {
            get { return (Count + PageSize - 1) / PageSize; }
        }

        //za sorting
        public object GetSortingParameters(string field)        //string kategorija
        {
            //default direction
            var direction = "ASC";

            //menjamo smer sortiranja ako je vec sortiran po istom parametru
            //&& SortByCategory == kategorija
            if (SortBy == field )
            {
                //ako je bio asc bice desc i obratno
                direction = SortDirection == "ASC" ? "DESC" : "ASC";
            }


            return new
            {
                SortBy = field,
                //SortByCategory = kategorija,
                SortDirection = direction,

                Query = Query,
                PageSize,
                Page
            };
        }

        public object GetPagingParameters(int i)
        {
            return new
            {
                SortBy,
                //SortByCategory,
                SortDirection,

                Query,
                PageSize,
                Page = i       //Page je atribut ili stavimo parametar "int page" i dole page
                               //page
            };
        }
    }

}
