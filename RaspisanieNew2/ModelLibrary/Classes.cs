using System;

namespace Raspisanie.Models
{
    public class Classes
    {
        public int CodeOfcourse { get; set; }
        public int CodeOfTeacher { get; set; }
        public int CodeOfSubject { get; set; }
        public int CodeOfGroup { get; set; }
        public DateTime DayTime { get; set; }
        public DayOfWeek DayOfLesson { get; set; }
        public int Numerator_Denoinator { get; set; }
    }
}