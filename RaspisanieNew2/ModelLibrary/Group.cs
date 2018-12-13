using System;

namespace Raspisanie.Models
{
    public class Group : IEquatable<Group>
    {
        public int CodeOfGroup { get; set; }
        public string NameOfGroup { get; set; }
        public Department Department { get; set; }
        public int Semester { get; set; }

        public override string ToString()
        {
            return NameOfGroup;
        }

        public bool Equals(Group other)
        {
            if (other == null)
                return false;

            if ((CodeOfGroup == other.CodeOfGroup) 
                && (NameOfGroup == other.NameOfGroup)
                && (Semester == other.Semester)
                && (Department == other.Department))
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            Group groupObj = obj as Group;
            if (groupObj == null)
                return false;
            else
                return Equals(groupObj);
        }

        public override int GetHashCode()
        {
            return this.CodeOfGroup.GetHashCode();
        }

        public static bool operator ==(Group group1, Group group2)
        {
            if (((object)group1) == null || ((object)group2) == null)
                return Object.Equals(group1, group2);

            return group1.Equals(group2);
        }

        public static bool operator !=(Group group1, Group group2)
        {
            if (((object)group1) == null || ((object)group2) == null)
                return ! Object.Equals(group1, group2);

            return !(group1.Equals(group2));
        }
    }
}
