using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Delicious.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string CommentContent { get; set; }
<<<<<<< HEAD
        public DateTime CommentInputDate { get; set; }
=======
        //public DateTime CommentInputDate { get; set; }
>>>>>>> cc0d17abea2472cfb078b0417a4f0689e63e17a0

        public virtual Recipe Recipe { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}