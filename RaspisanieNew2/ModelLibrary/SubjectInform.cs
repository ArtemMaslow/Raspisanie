using System;

namespace Models
{
    public class SubjectInform : ICloneable
    {
        public Subject Subject { get; set; }
        public int LectureHour { get; set; }
        public int ExerciseHour { get; set; }
        public int LaboratoryHour { get; set; }

        public object Clone()
        {
            var subjectInform = (SubjectInform)this.MemberwiseClone();
            subjectInform.Subject = (Subject)this.Subject.Clone();
            return subjectInform;
        }
    }
}
