using System;

namespace Models
{
    public class Teacher : IEquatable<Teacher>
    {
        public int CodeOfTeacher { get; set; }
        public string FIO { get; set; }
        public string Post { get; set; }
        public string Mail { get; set; }
        public bool IsReadLecture { get; set; }

        public Department Department {get; set;}
        public Department DepartmentTwo { get; set; }

        public override string ToString()
        {
            return FIO;
        }

        public bool Equals(Teacher other)
        {
            if (other == null)
                return false;

            if ((CodeOfTeacher == other.CodeOfTeacher)
                && (FIO == other.FIO)
                && (Post == other.Post)
                && (Mail == other.Mail)
                && (IsReadLecture == other.IsReadLecture)
                && (Department == other.Department)
                && (DepartmentTwo == other.DepartmentTwo))
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            Teacher teacherObj = obj as Teacher;
            if (teacherObj == null)
                return false;
            else
                return Equals(teacherObj);
        }

        public override int GetHashCode()
        {
            return this.CodeOfTeacher.GetHashCode();
        }

        public static bool operator ==(Teacher teacher1, Teacher teacher2)
        {
            if (((object)teacher1) == null || ((object)teacher2) == null)
                return Object.Equals(teacher1, teacher2);

            return teacher1.Equals(teacher2);
        }

        public static bool operator !=(Teacher teacher1, Teacher teacher2)
        {
            if (((object)teacher1) == null || ((object)teacher2) == null)
                return ! Object.Equals(teacher1, teacher2);

            return !(teacher1.Equals(teacher2));
        }
    }
}
