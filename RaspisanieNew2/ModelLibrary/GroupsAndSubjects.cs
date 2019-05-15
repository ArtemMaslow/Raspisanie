using System;
using System.Collections.Generic;

namespace Models
{
    public class GroupsAndSubjects : ICloneable
    {
        public Group Group { get; set; }
        public SubjectInform[] InformationAboutSubjects { get; set; }

        public object Clone()
        {
            var groupAndSubjects = (GroupsAndSubjects)this.MemberwiseClone();
            groupAndSubjects.Group = (Group)this.Group.Clone();
            List<SubjectInform> temp = new List<SubjectInform>();
            foreach (var subject in groupAndSubjects.InformationAboutSubjects)
            {
                temp.Add((SubjectInform)subject.Clone());
            }
            groupAndSubjects.InformationAboutSubjects = temp.ToArray();
            return groupAndSubjects;
        }
    }
}
