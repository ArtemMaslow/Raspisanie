using Raspisanie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public class GroupsAndSubjects
    {
        public Group Group { get; set; }
        public SubjectInform[] InformationAboutSubjects { get; set; }
    }
}
