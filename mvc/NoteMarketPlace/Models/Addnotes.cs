using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class Addnotes
    {   
        public int? noteid { get; set; }
        [Required]
        public string Title { get; set; }
        [Key]
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public HttpPostedFileBase uploadPicture { get; set; }
        public string uploadPictureGet { get; set; }
        public HttpPostedFileBase uploadNotes { get; set; }
        public string uploadNotesGet { get; set; }
        [Key]
        public string TypeID { get; set; }
        public string TypeName { get; set; }
        public int NoOfPages { get; set; }
        [Required]
        public string Description { get; set; }
        [Key]
        public string CountryID { get; set; }
        public string CountryName { get; set; }
        public string InstitutionName { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string ProfessorName { get; set; }
        [Required]
        public string SellMode { get; set; }
        [Required]
        public float SellPrice { get; set; }
        public HttpPostedFileBase uploadPreview { get; set; }
        public string uploadPreviewGet { get; set; }



    }
}