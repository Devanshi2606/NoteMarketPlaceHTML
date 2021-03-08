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
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        


    }
    
}