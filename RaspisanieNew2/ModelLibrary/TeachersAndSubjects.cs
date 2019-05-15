using System;

namespace Models
{
    public class TeachersAndSubjects
    {
        public Teacher Teacher { get; set; }
        public Subject[] SubjectList { get; set; }
        public DayOfWeek[] DayList { get; set; }

    }
}
