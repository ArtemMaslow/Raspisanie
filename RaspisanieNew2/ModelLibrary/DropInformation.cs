using System.Collections.Generic;

namespace Models
{
    public class DropInformation
    {
        //информация об уроке       
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }
        public List<Group> Group { get; set; }
        public ClassRoom NumberOfClassroom { get; set; }
        public string Specifics { get; set; }
        public int Ndindex { get; set; }

        public DropInformation()
        {
            Group = new List<Group>();
        }

        public DropInformation(List<Group> group, Teacher teacher, Subject subject, string specific, ClassRoom numberOfClassroom, int ndindex)
        {
            Group = group;
            Teacher = teacher;
            Subject = subject;
            Specifics = specific;
            NumberOfClassroom = numberOfClassroom;
            Ndindex = ndindex;
        }

        public DropInformation Copy()
        {
            return new DropInformation
            {
                Subject = this.Subject,
                Teacher = this.Teacher,
                Group = new List<Group>(this.Group),
                Specifics = this.Specifics,
                NumberOfClassroom = this.NumberOfClassroom,
                Ndindex = this.Ndindex
            };
        }
    }
}

