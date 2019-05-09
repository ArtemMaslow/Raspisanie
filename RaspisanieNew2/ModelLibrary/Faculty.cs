using System;

namespace Models
{
    public class Faculty : IEquatable<Faculty>
    {
        public int CodeOfFaculty { get; set; }
        public string NameOfFaculty { get; set; }

        public override string ToString()
        {
            return NameOfFaculty;
        }

        public bool Equals(Faculty other)
        {
            if (other == null)
                return false;

            if ((CodeOfFaculty == other.CodeOfFaculty) 
                && (NameOfFaculty==other.NameOfFaculty))
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            Faculty facultyObj = obj as Faculty;
            if (facultyObj == null)
                return false;
            else
                return Equals(facultyObj);
        }

        public override int GetHashCode()
        {
            return this.CodeOfFaculty.GetHashCode();
        }

        public static bool operator ==(Faculty faculty1, Faculty faculty2)
        {
            if (((object)faculty1) == null || ((object)faculty2) == null)
                return Object.Equals(faculty1, faculty2);

            return faculty1.Equals(faculty2);
        }

        public static bool operator !=(Faculty faculty1, Faculty faculty2)
        {
            if (((object)faculty1) == null || ((object)faculty2) == null)
                return ! Object.Equals(faculty1, faculty2);

            return !(faculty1.Equals(faculty2));
        }

    }


}
