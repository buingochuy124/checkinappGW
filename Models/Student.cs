using System;
using System.ComponentModel.DataAnnotations;

namespace CheckInGWDN.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = ("Student Id"))]
        public string StudentId { get; set; }

        [Display(Name = ("Student Name"))]
        public string Name { get; set; }

        public string Avatar { get; set; }

        [Display(Name = "Student Email")]
        public string Email { get; set; }

        private DateTime CreateAt { get; }

        public DateTime? UpdateAt { get; set; }

        [Display(Name = "QR Code")]
        public string Qr { get; set; }

        public Student()
        {
            this.CreateAt = DateTime.Now;
        }
    }
}