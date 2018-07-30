namespace Raspisanie.Models 
{
    public class Faculty 
    {
        public int CodeOfFaculty { get; set; }
        public string NameOfFaculty { get; set; }

        public override string ToString()
        {
            return NameOfFaculty;
        }

      
    }

  
}
