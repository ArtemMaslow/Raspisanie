using FirebirdSql.Data.FirebirdClient;
using ModelLibrary;
using Raspisanie.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{

    public class MainVM : ViewModelBase
    {
        private readonly INotifyCommand closeWinCommand;

        private readonly INotifyCommand createCommand;
        private readonly INotifyCommand openCommand;
        private readonly INotifyCommand saveCommand;

        private readonly INotifyCommand addFaculty;
        private readonly INotifyCommand addDepartment;
        private readonly INotifyCommand addTeacher;
        private readonly INotifyCommand addGroup;
        private readonly INotifyCommand addClassRoom;
        private readonly INotifyCommand addSubject;
        private readonly INotifyCommand addTeachersAndSubjects;
        private readonly INotifyCommand addGroupsAndSubjects;

        private WindowGroupVM windowGroupVM;
        private WindowFacultyVM windowFacultyVM;
        private WindowClassroomVM windowClassroomVM;
        private WindowDepartmentVM windowDepartmentVM;
        private WindowTeacherVM windowTeacherVM;
        private WindowSubjectVM windowSubjectVM;
        private WindowTeachersAndSubjectsVM windowTeachersAndSubjectsVM;
        private WindowGroupsAndSubjectsVM windowGroupsAndSubjectsVM;

        public void Create()
        {
            Console.WriteLine("CreateCommand");
        }

        public void Open()
        {
            Console.WriteLine("OpenCommand");
        }

        public void Save()
        {
            Console.WriteLine("SaveCommand");
        }

        public void AFaculty()
        {
            RefreshCollection();
            var winf = new WindowFaculty()
            {
                DataContext = windowFacultyVM
            };
            winf.ShowDialog();
            Console.WriteLine("AddFaculty");
        }

        public void ADepartment()
        {
            RefreshCollection();
            var winf = new WindowDepartment()
            {
                DataContext = windowDepartmentVM
            };
            winf.ShowDialog();
            Console.WriteLine("AddDepartment");
        }

        public void ATeacher()
        {
            RefreshCollection();
            var wint = new WindowTeacher()
            {
                DataContext = windowTeacherVM
            };
            wint.ShowDialog();
            Console.WriteLine("AddTeacher");
        }

        public void AGroup()
        {
            RefreshCollection();
            var wing = new WindowGroup()
            {
                DataContext = windowGroupVM
            };
            wing.ShowDialog();
            Console.WriteLine("AddGroup");
        }

        public void AClassRoom()
        {
            RefreshCollection();
            var winc = new WindowClassroom()
            {
                DataContext = windowClassroomVM
            };
            winc.ShowDialog();
            Console.WriteLine("AddClassRoom");
        }

        public void ASubject()
        {
            RefreshCollection();
            var wins = new WindowSubject()
            {
                DataContext = windowSubjectVM
            };
            wins.ShowDialog();
            Console.WriteLine("AddSubject");
        }

        public void ATeachersAndSubjects()
        {
            RefreshCollection();
            var wintands = new WindowTeachersAndSubjects()
            {
                DataContext = windowTeachersAndSubjectsVM
            };
            wintands.ShowDialog();
            Console.WriteLine("AddTeachersAndSubjects");
        }

        public void AGroupsAndSubjects()
        {
            RefreshCollection();
            var wingands = new WindowGroupsAndSubjects()
            {
                DataContext = windowGroupsAndSubjectsVM
            };
            wingands.ShowDialog();
            Console.WriteLine("AddGroupsAndSubjects");
        }

        public void Close()
        {

        }

        private ObservableCollection<Faculty> cfaculty;
        private ObservableCollection<ClassRoom> cclassroom;
        private ObservableCollection<Department> cdepartment;
        private ObservableCollection<Group> cgroup;
        private ObservableCollection<Teacher> cteacher;
        private ObservableCollection<Subject> csubject;
        private TeachersAndSubjects[] tands;
        private GroupsAndSubjects[] gands;
        private List<Subject> lsubject;
        private List<DayOfWeek> lday;
        private ObservableCollection<TeachersAndSubjects> allTeachersAndSubjects = new ObservableCollection<TeachersAndSubjects>();
        private ObservableCollection<GroupsAndSubjects> groupsAndSubjects = new ObservableCollection<GroupsAndSubjects>();
        public MainVM()
        {
            lday = new List<DayOfWeek>();
            lday.Add(DayOfWeek.Monday);
            lday.Add(DayOfWeek.Tuesday);
            lday.Add(DayOfWeek.Wednesday);
            lday.Add(DayOfWeek.Thursday);
            lday.Add(DayOfWeek.Friday);
            lday.Add(DayOfWeek.Saturday);

            var initfaculty = RequestToDataBase.Instance.ReadFaculty();
            cfaculty = new ObservableCollection<Faculty>(initfaculty);

            var initclassroom = RequestToDataBase.Instance.ReadClassrooms();
            cclassroom = new ObservableCollection<ClassRoom>(initclassroom);

            var initdepartment = RequestToDataBase.Instance.ReadDepartments();
            cdepartment = new ObservableCollection<Department>(initdepartment);

            var initgroup = RequestToDataBase.Instance.ReadGroups();
            cgroup = new ObservableCollection<Group>(initgroup);

            var initteacher = RequestToDataBase.Instance.ReadTeachers();
            cteacher = new ObservableCollection<Teacher>(initteacher);

            var initsubject = RequestToDataBase.Instance.ReadSubjects();
            csubject = new ObservableCollection<Subject>(initsubject);

            var initlistsubjects = RequestToDataBase.Instance.ReadSubjects();
            lsubject = new List<Subject>(initlistsubjects);

            //var initTeachersAndSubjects = RequestToDataBase.Instance.ReadTeacherAndSubjects();
            //tands = initTeachersAndSubjects.ToArray();

            tands = RequestToDataBase.Instance.ReadTeacherAndSubjects();
            gands = RequestToDataBase.Instance.ReadGroupsAndSubjects();

            windowGroupVM = new WindowGroupVM(cgroup, cdepartment);
            windowFacultyVM = new WindowFacultyVM(cfaculty);
            windowClassroomVM = new WindowClassroomVM(cclassroom, cdepartment);
            windowDepartmentVM = new WindowDepartmentVM(cdepartment, cfaculty);
            windowTeacherVM = new WindowTeacherVM(cteacher, cdepartment);
            windowSubjectVM = new WindowSubjectVM(csubject, cdepartment);
            windowTeachersAndSubjectsVM = new WindowTeachersAndSubjectsVM(cteacher, allTeachersAndSubjects, lsubject, lday);
            windowGroupsAndSubjectsVM = new WindowGroupsAndSubjectsVM(cgroup, groupsAndSubjects, csubject);

            createCommand = this.Factory.CommandSync(Create);
            openCommand = this.Factory.CommandSync(Open);
            saveCommand = this.Factory.CommandSync(Save);

            addFaculty = this.Factory.CommandSync(AFaculty);
            addDepartment = this.Factory.CommandSync(ADepartment);
            addTeacher = this.Factory.CommandSync(ATeacher);
            addGroup = this.Factory.CommandSync(AGroup);
            addClassRoom = this.Factory.CommandSync(AClassRoom);
            addSubject = this.Factory.CommandSync(ASubject);
            addTeachersAndSubjects = this.Factory.CommandSync(ATeachersAndSubjects);
            addGroupsAndSubjects = this.Factory.CommandSync(AGroupsAndSubjects);
            closeWinCommand = this.Factory.CommandSync(Close);
        }

        public void RefreshCollection()
        {
            cfaculty.Clear();
            foreach (var value in RequestToDataBase.Instance.ReadFaculty()) cfaculty.Add(value);

            cclassroom.Clear();
            foreach (var value in RequestToDataBase.Instance.ReadClassrooms()) cclassroom.Add(value);

            cdepartment.Clear();
            foreach (var value in RequestToDataBase.Instance.ReadDepartments()) cdepartment.Add(value);

            cgroup.Clear();
            foreach (var value in RequestToDataBase.Instance.ReadGroups()) cgroup.Add(value);

            cteacher.Clear();
            foreach (var value in RequestToDataBase.Instance.ReadTeachers()) cteacher.Add(value);

            csubject.Clear();
            foreach (var value in RequestToDataBase.Instance.ReadSubjects()) csubject.Add(value);

            var tands = RequestToDataBase.Instance.ReadTeacherAndSubjects();
            allTeachersAndSubjects.Clear();
            var dct = tands.ToDictionary(t => (t.Teacher.CodeOfTeacher, t.Teacher.Department.CodeOfDepartment), t => t);
            var all = cteacher.Select(t => dct.TryGetValue((t.CodeOfTeacher, t.Department.CodeOfDepartment), out TeachersAndSubjects tsv) ? tsv : CreateTeacherAndSubjects(t));
            foreach(var value in all)
                allTeachersAndSubjects.Add(value);

            var gands = RequestToDataBase.Instance.ReadGroupsAndSubjects();
            groupsAndSubjects.Clear();
            var dctGroup = gands.ToDictionary(g => g.Group.CodeOfGroup, g => g);
            var allGroups = cgroup.Select(g => dctGroup.TryGetValue(g.CodeOfGroup, out GroupsAndSubjects gs) ? gs : CreateEmptyGroupsAndSubjects(g));
            foreach (var value in allGroups)
                groupsAndSubjects.Add(value);
        }

        private TeachersAndSubjects CreateTeacherAndSubjects(Teacher teacher)
        {
            return new TeachersAndSubjects
            {
                Teacher = teacher,
                SubjectList = Enumerable.Empty<Subject>().ToArray(),
                DayList = Enumerable.Empty<DayOfWeek>().ToArray()
                //Teacher = teacher,
                //SubjectList = lsubject.ToArray(),
                //DayList = lday.ToArray()
            };
        }

        private GroupsAndSubjects CreateEmptyGroupsAndSubjects(Group group)
        {
            return new GroupsAndSubjects
            {
                Group = group,
                InformationAboutSubjects = Enumerable.Empty<SubjectInform>().ToArray()
            };
        }
        public ICommand CloseWinCommand => closeWinCommand;

        public ICommand CreateCommand => createCommand;
        public ICommand OpenCommand => openCommand;
        public ICommand SaveCommand => saveCommand;

        public ICommand AddFaculty => addFaculty;
        public ICommand AddDepartment => addDepartment;
        public ICommand AddTeacher => addTeacher;
        public ICommand AddGroup => addGroup;
        public ICommand AddClassRoom => addClassRoom;
        public ICommand AddSubject => addSubject;
        public ICommand AddTeachersAndSubjects => addTeachersAndSubjects;
        public ICommand AddGroupsAndSubjects => addGroupsAndSubjects;
    }
}
