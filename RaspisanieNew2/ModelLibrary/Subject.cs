namespace Raspisanie.Models
{
    public class Subject
    {
        public int CodeOfSubject { get; set; }
        public string NameOfSubject { get; set; }
        public string Specific { get; set; }
        public Department Department { get; set; }

        public override string ToString()
        {
            return NameOfSubject;
        }
    }
}
