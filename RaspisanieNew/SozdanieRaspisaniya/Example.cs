using Raspisanie.Models;

namespace SozdanieRaspisaniya
{
    class Example
    {
        public string NameOfDepartment;
        public Group[] Group;
        public RepresentationSubject<Group,Subject>[] Subject;
        public Teacher[] Teacher;
        public ClassRoom[] Classroom;
    }
}
