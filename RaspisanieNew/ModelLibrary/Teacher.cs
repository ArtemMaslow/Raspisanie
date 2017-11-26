namespace Raspisanie.Models
{
    public class Teacher
    {
        public int CodeOfTeacher { get; set; }
        public string FIO { get; set; }
        public string Post { get; set; }
        public int CodeOfDepartment { get; set; }
        //public Department Department { get; set; }
        public int HourOfLoad { get; set; }
    }
}
