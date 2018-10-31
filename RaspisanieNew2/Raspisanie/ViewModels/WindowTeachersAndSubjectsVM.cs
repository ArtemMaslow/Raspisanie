using Newtonsoft.Json;
using Raspisanie.Models;
using Raspisanie.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowTeachersAndSubjectsVM : ViewModelBase
    {
        private readonly INotifyingValue<int> teacherIndex;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;
        

        public WindowTeachersAndSubjectsVM(ObservableCollection<Teacher> classTeachers, TeachersAndSubjects[] teachersAndSubjects, List<Subject> allSubjectsList, List<DayOfWeek> allDayList)
        {
            ClassTeachers = classTeachers;
            AllSubjectList = allSubjectsList;
            AllDayList = allDayList;
            TeachersAndSubjects tsv;
            //здесь происходит ровно то о чем вчера писал, 
            //среди всех учителей проверяем если ли заполненная информация и 
            //если есть возвращаем ее если нет, то пустые данные
            var dct = teachersAndSubjects.ToDictionary(t => t.Teacher, t => t);            
            var all = classTeachers.Select(t => dct.TryGetValue(t, out  tsv) ? tsv : CreateEmpty(t));
            AllTeachersAndSubjects = new ObservableCollection<TeachersAndSubjects>(all);
            //foreach (var value in AllTeachersAndSubjects)
            //    Console.WriteLine(value.Teacher + "" + value.SubjectList[0]);
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            teacherIndex = this.Factory.Backing(nameof(TeacherIndex), -1);
        }

        public void Add()
        {
            var tas = AllTeachersAndSubjects[TeacherIndex];
            var context = new TeachersAndSubjectsVM(tas, AllSubjectList.ToArray(), AllDayList.ToArray());
            var wintands = new NewTeachersAndSubjects()
            {
                DataContext = context
            };
            wintands.ShowDialog();
            if (context.SelectedDays != null && context.SelectedSubjects != null)
            {
                var ls = JsonConvert.SerializeObject(context.SelectedSubjects);
                var ld = JsonConvert.SerializeObject(context.SelectedDays);
                if (RequestToDataBase.Instance.requestInsertIntoTeachersAndSubjects(tas, ls, ld))
                {
                    //    TeachersAndSubjects tsv;
                    //    AllTeachersAndSubjects.Clear();
                    //    var dct = new Dictionary<Teacher, TeachersAndSubjects>();
                    //    foreach (var value in RequestToDataBase.Instance.ReadTeacherAndSubjects()) dct.Add(value.Teacher,value);
                    //    var all = ClassTeachers.Select(t => dct.TryGetValue(t, out tsv) ? tsv : CreateEmpty(t));
                    //    foreach(var value in all)
                    //    AllTeachersAndSubjects.Add(value);
                   // Console.WriteLine(tas.CodeOftands);
                    AllTeachersAndSubjects.Clear();
                    foreach (var value in RequestToDataBase.Instance.ReadTeacherAndSubjects())
                        AllTeachersAndSubjects.Add(value);

                }
                //    var ts = new TeachersAndSubjects
                //{
                //    Teacher = tas.Teacher,
                //    SubjectList = context.SelectedSubjects,
                //    DayList = context.SelectedDays
                //};
                //AllTeachersAndSubjects.Remove(tas);
                //AllTeachersAndSubjects.Add(ts);
               // Console.WriteLine(AllTeachersAndSubjects[TeacherIndex].CodeOftands);
            }
        }

        public void Edit()
        {
            var tas = AllTeachersAndSubjects[TeacherIndex];
            Console.WriteLine(tas.CodeOftands);
            var context = new TeachersAndSubjectsVM(tas, AllSubjectList.ToArray(), AllDayList.ToArray());
            var wintands = new NewTeachersAndSubjects()
            {
                DataContext = context
            };
            wintands.ShowDialog();
            if (context.SelectedDays != null && context.SelectedSubjects != null)
            {
                var ls = JsonConvert.SerializeObject(context.SelectedSubjects);
                var ld = JsonConvert.SerializeObject(context.SelectedDays);
                if (RequestToDataBase.Instance.requestUpdateTeachersAndSubjects(tas, ls, ld))
                {
                    AllTeachersAndSubjects.Clear();
                    foreach (var value in RequestToDataBase.Instance.ReadTeacherAndSubjects())
                        AllTeachersAndSubjects.Add(value);
                    Console.WriteLine("good");

                }
            }
        }

        public void Remove()
        {

        }

        private TeachersAndSubjects CreateEmpty(Teacher teacher)
        {
            return new TeachersAndSubjects
            {
                Teacher = teacher,
                SubjectList = Enumerable.Empty<Subject>().ToArray(),
                DayList = Enumerable.Empty<DayOfWeek>().ToArray()
            };
        }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int TeacherIndex { get { return teacherIndex.Value; } set { teacherIndex.Value = value; } }
        public ObservableCollection<TeachersAndSubjects> AllTeachersAndSubjects { get; }
        public ObservableCollection<Teacher> ClassTeachers { get; }
        public List<Subject> SubjectList { get; }
        public List<DayOfWeek> DayList { get; }
        public List<Subject> AllSubjectList { get; }
        public List<DayOfWeek> AllDayList { get; }
        public TeachersAndSubjects[] TeachersAndSubjects { get; }

    }
}
