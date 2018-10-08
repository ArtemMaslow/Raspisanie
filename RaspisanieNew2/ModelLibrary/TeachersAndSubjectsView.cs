
using ModelLibrary;
using System;
using System.Collections.Generic;

namespace Raspisanie.Models
{
    public class TeachersAndSubjectsView
    {
       public Teacher Teacher { get; set; }
       public List<TeachersAndSubjectsViewHelper<Subject>> SubjectList { get; set; }
       public List<TeachersAndSubjectsViewHelper<DayOfWeek>> DayList { get; set; }
    }
}
