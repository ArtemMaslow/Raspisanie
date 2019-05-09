using Models;
using Newtonsoft.Json;
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
        
        public WindowTeachersAndSubjectsVM(ObservableCollection<Teacher> classTeachers, ObservableCollection<TeachersAndSubjects> teachersAndSubjects, List<Subject> allSubjectsList, List<DayOfWeek> allDayList)
        {
            ClassTeachers = classTeachers;
            AllSubjectList = allSubjectsList;
            AllDayList = allDayList;
            AllTeachersAndSubjects = teachersAndSubjects;
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            teacherIndex = this.Factory.Backing(nameof(TeacherIndex), -1);
        }

        public void Add()
        {
            if (TeacherIndex >= 0)
            {
                var tas = AllTeachersAndSubjects[TeacherIndex];
                var context = new TeachersAndSubjectsVM(tas, AllSubjectList.ToArray(), AllDayList.ToArray());
                var wind = new NewTeachersAndSubjects()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if(wind.DialogResult == true) {
                    if (context.SelectedDays != null && context.SelectedSubjects != null)
                    {
                        var ld = JsonConvert.SerializeObject(context.SelectedDays);
                        tas.SubjectList = context.SelectedSubjects;
                        foreach (var item in tas.SubjectList)
                        {
                            if (RequestToDataBase.Instance.requestInsertIntoTeachersAndSubjects(tas, item, ld))
                            {
                                RefreshAllTeachersAndSubjects();
                            }
                        }
                    }
                }
            }
        }

        public void Edit()
        {
            if (TeacherIndex >= 0)
            {
                var tas = AllTeachersAndSubjects[TeacherIndex];
                var context = new TeachersAndSubjectsVM(tas, AllSubjectList.ToArray(), AllDayList.ToArray());
                var wind = new NewTeachersAndSubjects()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (wind.DialogResult == true)
                {
                    if (context.SelectedDays != null && context.SelectedSubjects != null)
                    {
                        RequestToDataBase.Instance.requestDeleteFromTeachersAndSubjects(tas);
                        var ld = JsonConvert.SerializeObject(context.SelectedDays);
                        tas.SubjectList = context.SelectedSubjects;
                        foreach (var item in tas.SubjectList)
                        {
                            if (RequestToDataBase.Instance.requestInsertIntoTeachersAndSubjects(tas, item, ld))
                            {
                                RefreshAllTeachersAndSubjects();
                            }
                        }
                    }
                }
            }
        }

        public void Remove()
        {
            if (TeacherIndex >= 0)
                if (RequestToDataBase.Instance.requestDeleteFromTeachersAndSubjects(AllTeachersAndSubjects[TeacherIndex]))
                {
                    RefreshAllTeachersAndSubjects();
                }
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

        private void RefreshAllTeachersAndSubjects()
        {         
            AllTeachersAndSubjects.Clear();
            var dct = new Dictionary<(int,int), TeachersAndSubjects>();
            foreach (var value in RequestToDataBase.Instance.ReadTeacherAndSubjects()) dct.Add((value.Teacher.CodeOfTeacher,value.Teacher.Department.CodeOfDepartment), value);
            var all = ClassTeachers.Select(t => dct.TryGetValue((t.CodeOfTeacher,t.Department.CodeOfDepartment), out TeachersAndSubjects tsv) ? tsv : CreateEmpty(t));
            foreach (var value in all)
                AllTeachersAndSubjects.Add(value);
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
