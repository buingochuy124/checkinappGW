using System;
using System.ComponentModel.DataAnnotations;

namespace CheckInGWDN.Models
{
    public class Download
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = ("Zip Name"))]
        public string Name { get; set; }

        public string ZipPath { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public Download()
        {

            CreateAt = DateTime.Now;
        }
    }
}