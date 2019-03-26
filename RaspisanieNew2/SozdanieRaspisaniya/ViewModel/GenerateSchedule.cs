using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raspisanie.Models;

namespace SozdanieRaspisaniya.ViewModel
{
    class GenerateSchedule
    {

        //План на час
        class HourPlan
        {
            //Хранит группа - (преподаватель, предмет,специфика, аудитория)
            public Dictionary<int, (int, int, int, int, int)> GroupToTeacher = new Dictionary<int, (int, int, int, int, int)>();
            
            // Хранит преподаватель - (группа, предмет, специфика, аудитория)
            //public Dictionary<(int, int), (int, int, int, int)> TeacherToGroup = new Dictionary<(int, int), (int, int, int, int)>();

            public bool AddLesson(int group, (int, int, int, int, int) teacher)
            {
                if (GroupToTeacher.ContainsKey(group) /*|| TeacherToGroup.ContainsKey(teacher)*/)
                    return false;//в этот час уже есть пара у группы //у препода или 

                GroupToTeacher[group] = teacher;
                //TeacherToGroup[teacher] = group;

                return true;
            }

            public void RemoveLesson(int group, (int,int) teacher)
            {
                GroupToTeacher.Remove(group);
               // TeacherToGroup.Remove(teacher);
            }

            public HourPlan Clone()
            {
                var res = new HourPlan();
                res.GroupToTeacher = new Dictionary<int, (int, int, int, int, int)>(GroupToTeacher);
            //    res.TeacherToGroup = new Dictionary<(int, int), int>(TeacherToGroup);

                return res;
            }
        }

        //пара
        class Lesson
        {
            public PairInfo PairInfo;
            public Group Group;
            public Teacher Teacher;
            public ClassRoom Classroom;
            public Subject Subject;
            public string Specific;

            public Lesson(Group group, Teacher teacher, Subject subject, ClassRoom classroom, string specific)
            {
                Group = group;
                Teacher = teacher;
                Subject = subject;
                Classroom = classroom;
                Specific = specific;
            }

            public Lesson(PairInfo pairInfo, Group group, Teacher teacher, Subject subject, ClassRoom classroom, string specific) : this(group, teacher, subject, classroom, specific)
            {
                PairInfo = pairInfo;
            }
        }
    }
}
