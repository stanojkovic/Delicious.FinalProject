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
        //public DateTime CommentInputDate { get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}