using System;

namespace Raspisanie.Models
{
    public class Lesson
    {
        public ClassRoom ClassRoom { get; set; }
        public Course Course { get; set; }
        public DayOfWeek DayOfLesson { get; set; }
        public DateTime TimeOfLesson { get; set; }
    }
}
