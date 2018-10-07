
using System;
using System.Collections.Generic;

namespace Raspisanie.Models
{
    public class TeachersAndSubjectsView
    {
       public Teacher Teacher { get; set; }
       public List<Subject> SubjectList { get; set; }
       public List<DayOfWeek> DayList { get; set; }
    }
}
