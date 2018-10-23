
using ModelLibrary;
using System;
using System.Collections.Generic;

namespace Raspisanie.Models
{
    public class TeachersAndSubjects
    {
        public Teacher Teacher { get; set; }
        public Subject[] SubjectList { get; set; }
        public DayOfWeek[] DayList { get; set; }
    }
}
