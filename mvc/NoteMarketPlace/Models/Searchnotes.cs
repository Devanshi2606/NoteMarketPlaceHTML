using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Models
{
    public class Searchnotes
    {
     
        public SelectList CategoryName { get; set; }
        public SelectList TypeName { get; set; }
        public SelectList UniversityName { get; set; }
        public SelectList CourseName { get; set; }
        public SelectList CountryName { get; set; }
        public string Rating { get; set; }




    }
    
}