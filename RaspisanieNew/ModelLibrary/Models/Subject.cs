namespace Raspisanie.Models
{
    public class Subject
    {
        //public enum specifics
        //{
        //    seminar = 1,
        //    laboratory,
        //    lecture
        //}
        public int CodeOfSubject { get; set; }
        public string NameOfSubject { get; set; }
        public string Specifics { get; set; }
        public int CommonHours { get; set; }
        public int LectureHours { get; set; }
        public int ExerciseHours { get; set; }
        public int LaboratoryHours { get; set; }
        public int CodeOfDepartment { get; set; }
        //public Department Department { get; set; }
    }
}
