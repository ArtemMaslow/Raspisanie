﻿using Raspisanie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public class GroupsAndSubjects
    {
        public int CodeOfGands { get; set; }
        public Group Group { get; set; }
        public SubjectInform[] InformationAboutSubjects { get; set; }
    }
}