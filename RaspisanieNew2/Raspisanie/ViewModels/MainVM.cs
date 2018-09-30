using FirebirdSql.Data.FirebirdClient;
using Raspisanie.Models;
using System;
using System.Collections.ObjectModel;
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

        private WindowGroupVM windowGroupVM;
        private WindowFacultyVM windowFacultyVM;
        private WindowClassroomVM windowClassroomVM;
        private WindowDepartmentVM windowDepartmentVM;
        private WindowTeacherVM windowTeacherVM;
        private WindowSubjectVM windowSubjectVM;
        private WindowTeachersAndDepartmentsVM windowTeachersAndSubjectsVM;

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
            var wintands = new WindowTeachersAndDepartments()
            {
                DataContext = windowTeachersAndSubjectsVM
            };
            wintands.ShowDialog();
            Console.WriteLine("AddTeachersAndSubjects");
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

        public MainVM()
        {
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

            windowGroupVM = new WindowGroupVM(cgroup, cdepartment);
            windowFacultyVM = new WindowFacultyVM(cfaculty);
            windowClassroomVM = new WindowClassroomVM(cclassroom, cdepartment);
            windowDepartmentVM = new WindowDepartmentVM(cdepartment, cfaculty);
            windowTeacherVM = new WindowTeacherVM(cteacher, cdepartment);
            windowSubjectVM = new WindowSubjectVM(csubject, cdepartment);
            windowTeachersAndSubjectsVM = new WindowTeachersAndDepartmentsVM();

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
    }
}
