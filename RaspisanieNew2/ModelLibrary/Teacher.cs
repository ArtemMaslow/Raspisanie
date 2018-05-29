namespace Raspisanie.Models
{
    public class Teacher
    {
        public int CodeOfTeacher { get; set; }
        public string FIO { get; set; }
        public string Post { get; set; }
        public Department Department {get; set;}
        public Department DepartmentTwo { get; set; }
        public Department DepartmentThree { get; set; }

        public override string ToString()
        {
            return FIO;
        }
    }
}
