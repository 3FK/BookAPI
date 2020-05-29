using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookstore_UI.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="First Name")]
        public string Firstname { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }
        [Required]
        [Display(Name = "Biography")]
        [StringLength(250)]
        public string Bio { get; set; }

        public IList<Book> Books { get; set; }
    }
}
