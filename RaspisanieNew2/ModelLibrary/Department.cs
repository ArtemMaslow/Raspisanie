using System;

namespace Models
{
    public class Department : IEquatable<Department>, ICloneable
    {
        public int CodeOfDepartment { get; set; }
        public string NameOfDepartment { get; set; }
        public Faculty Faculty { get; set; }

        public override string ToString()
        {
            return NameOfDepartment;
        }

        public bool Equals(Department other)
        {
            if (other == null)
                return false;

            if ((CodeOfDepartment == other.CodeOfDepartment)
                && (NameOfDepartment == other.NameOfDepartment)
                && (Faculty == other.Faculty))
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            Department departmentObj = obj as Department;
            if (departmentObj == null)
                return false;
            else
                return Equals(departmentObj);
        }

        public override int GetHashCode()
        {
            return this.CodeOfDepartment.GetHashCode();
        }

        public object Clone()
        {
            var department = (Department)this.MemberwiseClone();
            return department;
        }

        public static bool operator ==(Department department1, Department department2)
        {
            if (((object)department1) == null || ((object)department2) == null)
                return Object.Equals(department1, department2);

            return department1.Equals(department2);
        }

        public static bool operator !=(Department department1, Department department2)
        {
            if (((object)department1) == null || ((object)department2) == null)
                return !Object.Equals(department1, department2);

            return !(department1.Equals(department2));
        }
    }
}
