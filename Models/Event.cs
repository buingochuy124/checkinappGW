using System.ComponentModel.DataAnnotations;

namespace CheckInGWDN.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = ("Event"))]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool status { get; set; }
    }
}