using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckInGWDN.Models
{
    public class UserEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        [Display(Name = "Event")]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}