namespace Raspisanie.Models
{
    public class ClassRoom
    {
        public int CodeOfClassroom { get; set; }
        public string NumberOfClassroom { get; set; }
        public string Specifics { get; set; }
        public Department Department { get; set; }
    }
}
