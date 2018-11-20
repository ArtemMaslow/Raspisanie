using Raspisanie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public class SubjectInform
    {
        public Subject Subject { get; set; }
        public int LectureHour { get; set; }
        public int ExerciseHour { get; set; }
        public int LaboratoryHour { get; set; }
    }
}
