
using System;
using System.Collections.Generic;

namespace Raspisanie.Models
{
    public class TeachersAndSubjectsView
    {
       public Teacher teacher { get; set; }
       public List<Subject> subjectList { get; set; }
       public List<DayOfWeek> dayList { get; set; }
    }
}
