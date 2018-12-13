using System;

namespace Raspisanie.Models
{
    public class Subject : IEquatable<Subject>
    {
        public int CodeOfSubject { get; set; }
        public string NameOfSubject { get; set; }
        public Department Department { get; set; }

        public override string ToString()
        {
            return NameOfSubject;
        }

        public bool Equals(Subject other)
        {
            if (other == null)
                return false;

            if ((CodeOfSubject == other.CodeOfSubject) 
                && (NameOfSubject == other.NameOfSubject)
                && (Department == other.Department))
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            Subject subjectObj = obj as Subject;
            if (subjectObj == null)
                return false;
            else
                return Equals(subjectObj);
        }

        public override int GetHashCode()
        {
            return this.CodeOfSubject.GetHashCode();
        }

        public static bool operator ==(Subject subject1, Subject subject2)
        {
            if (((object)subject1) == null || ((object)subject2) == null)
                return Object.Equals(subject1, subject2);

            return subject1.Equals(subject2);
        }

        public static bool operator !=(Subject subject1, Subject subject2)
        {
            if (((object)subject1) == null || ((object)subject2) == null)
                return !Object.Equals(subject1, subject2);

            return !(subject1.Equals(subject2));
        }
    }
}
