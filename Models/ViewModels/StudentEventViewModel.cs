using System.Collections.Generic;

namespace CheckInGWDN.Models.ViewModels
{
    public class StudentEventViewModel
    {
        public IEnumerable<Event> Events { get; set; }
        public Event Event { get; set; }

        //public IEnumerable<Student> Students { get; set; }
        public IEnumerable<UserEvent> UserEvents { get; set; }
    }
}