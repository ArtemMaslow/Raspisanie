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

        private readonly INotifyCommand connectToDataBase;

        private readonly INotifyCommand createCommand;
        private readonly INotifyCommand openCommand;
        private readonly INotifyCommand saveCommand;

        private readonly INotifyCommand addFaculty;
        private readonly INotifyCommand addDepartment;
        private readonly INotifyCommand addTeacher;
        private readonly INotifyCommand addGroup;
        private readonly INotifyCommand addClassRoom;
        private readonly INotifyCommand addSubject;

        private INotifyingValue<string> loggin;
        private INotifyingValue<string> password;
        private INotifyingValue<string> searchDB;

        private WindowGroupVM windowGroupVM;
        private WindowFacultyVM windowFacultyVM;
        private WindowClassroomVM windowClassroomVM;
        private WindowDepartmentVM windowDepartmentVM;
        private WindowTeacherVM windowTeacherVM;
        private WindowSubjectVM windowSubjectVM;

        private ConnectVM ConWin;
        
        public void ConnectToDataBase() 
        {
            var ConnWin = new ConnectToDataBase()
            {
                DataContext = ConWin
            };
            ConnWin.ShowDialog();
        }

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
            var winf = new WindowFaculty()
            {
                DataContext = windowFacultyVM
            };
            winf.ShowDialog();
            Console.WriteLine("AddFaculty");
        }

        public void ADepartment()
        {
            var winf = new WindowDepartment()
            {
                DataContext = windowDepartmentVM
            };
            winf.ShowDialog();
            Console.WriteLine("AddDepartment");
        }

        public void ATeacher()
        {
            var wint = new WindowTeacher()
            {
                DataContext = windowTeacherVM
            };
            wint.ShowDialog();
            Console.WriteLine("AddTeacher");
        }

        public void AGroup()
        {
            var wing = new WindowGroup()
            {
                DataContext = windowGroupVM
            };
            wing.ShowDialog();
            Console.WriteLine("AddGroup");
        }
        
        public void AClassRoom()
        {
            var winc = new WindowClassroom()
            {
                DataContext = windowClassroomVM
            };
            winc.ShowDialog();
            Console.WriteLine("AddClassRoom");
        }

        public void ASubject()
        {
            var wins = new WindowSubject()
            {
                DataContext = windowSubjectVM
            };
            wins.ShowDialog();
            Console.WriteLine("AddSubject");
        }

        public void Close()
        {
            XML.SaveClassroom(cclassroom, Path.ClassroomXml);
            XML.SaveFaculty(cfaculty, Path.FacultyXml);
            XML.SaveDepartment(cdepartment, Path.DepartmentXml);
            XML.SaveGroup(cgroup, Path.GroupXml);
            XML.SaveTeacher(cteacher, Path.TeacherXml);
            XML.SaveSubject(csubject, Path.SubjectXml);
           
        }

        private ObservableCollection<Faculty> cfaculty;
        private ObservableCollection<ClassRoom> cclassroom;
        private ObservableCollection<Department> cdepartment;
        private ObservableCollection<Group> cgroup;
        private ObservableCollection<Teacher> cteacher;
        private ObservableCollection<Subject> csubject;

        public MainVM()
        {
            var initfaculty = XML.ReadFaculty(Path.FacultyXml);
            cfaculty = new ObservableCollection<Faculty>(initfaculty);

            var initclassroom = XML.ReadClassroom(Path.ClassroomXml);
            cclassroom = new ObservableCollection<ClassRoom>(initclassroom);

            var initdepartment = XML.ReadDepartment(Path.DepartmentXml);
            cdepartment = new ObservableCollection<Department>(initdepartment);

            var initgroup = XML.ReadGroup(Path.GroupXml);
            cgroup = new ObservableCollection<Group>(initgroup);

            var initteacher = XML.ReadTeacher(Path.TeacherXml);
            cteacher = new ObservableCollection<Teacher>(initteacher);

            var initsubject = XML.ReadSubject(Path.SubjectXml);
            csubject = new ObservableCollection<Subject>(initsubject);

            ConWin = new ConnectVM();

            windowGroupVM = new WindowGroupVM(cgroup,cdepartment);
            windowFacultyVM = new WindowFacultyVM(cfaculty);
            windowClassroomVM = new WindowClassroomVM(cclassroom, cdepartment);
            windowDepartmentVM = new WindowDepartmentVM(cdepartment, cfaculty);
            windowTeacherVM = new WindowTeacherVM(cteacher, cdepartment);
            windowSubjectVM = new WindowSubjectVM(csubject, cdepartment);

            connectToDataBase = this.Factory.CommandSync(ConnectToDataBase);

            createCommand = this.Factory.CommandSync(Create);
            openCommand = this.Factory.CommandSync(Open);
            saveCommand = this.Factory.CommandSync(Save);

            addFaculty = this.Factory.CommandSync(AFaculty);
            addDepartment = this.Factory.CommandSync(ADepartment);
            addTeacher = this.Factory.CommandSync(ATeacher);
            addGroup = this.Factory.CommandSync(AGroup);
            addClassRoom = this.Factory.CommandSync(AClassRoom);
            addSubject = this.Factory.CommandSync(ASubject);

            closeWinCommand = this.Factory.CommandSync(Close);
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

        public ICommand CConnectToDataBase => connectToDataBase;

        public string Loggin { get { return loggin.Value; } set { loggin.Value = value; } }
        public string Password { get { return password.Value; } set { password.Value = value; } }
        public string SearchDB { get { return searchDB.Value; } set { searchDB.Value = value; } }
    }
}
