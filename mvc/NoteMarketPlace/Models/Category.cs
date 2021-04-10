using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class Category
    { 
        public int? categoryID { get; set; }
        public string categoryName { get; set; }
        public string Description { get; set; }
    }
}