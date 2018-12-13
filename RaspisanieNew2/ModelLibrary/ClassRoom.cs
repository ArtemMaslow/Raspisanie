using System;

namespace Raspisanie.Models
{
    public class ClassRoom : IEquatable<ClassRoom>
    {
        public int CodeOfClassroom { get; set; }
        public string NumberOfClassroom { get; set; }
        public string Specifics { get; set; }
        public Department Department { get; set; }

        public override string ToString()
        {
            return NumberOfClassroom;
        }

        public bool Equals(ClassRoom other)
        {
            if (other == null)
                return false;

            if ((CodeOfClassroom == other.CodeOfClassroom) 
                && (NumberOfClassroom == other.NumberOfClassroom) 
                && (Specifics == other.Specifics)
                && (Department == other.Department))
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            ClassRoom сlassRoomObj = obj as ClassRoom;
            if (сlassRoomObj == null)
                return false;
            else
                return Equals(сlassRoomObj);
        }

        public override int GetHashCode()
        {
            return this.CodeOfClassroom.GetHashCode();
        }

        public static bool operator ==(ClassRoom сlassRoom1, ClassRoom сlassRoom2)
        {
            if (((object)сlassRoom1) == null || ((object)сlassRoom2) == null)
                return Object.Equals(сlassRoom1, сlassRoom2);

            return сlassRoom1.Equals(сlassRoom2);
        }

        public static bool operator !=(ClassRoom сlassRoom1, ClassRoom сlassRoom2)
        {
            if (((object)сlassRoom1) == null || ((object)сlassRoom2) == null)
                return ! Object.Equals(сlassRoom1, сlassRoom2);

            return !(сlassRoom1.Equals(сlassRoom2));
        }


    }

    
}
