using Raspisanie.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;


namespace SozdanieRaspisaniya.ViewModel
{
    class MainVM : ViewModelBase
    {
        private readonly INotifyCommand closeWinCommand;

        private readonly INotifyingValue<int> indexClassrom;
        private readonly INotifyingValue<int> indexGroup;
        private readonly INotifyingValue<int> indexTeacher;
        private readonly INotifyingValue<int> indexSubject;

        private readonly INotifyCommand createCommand;
        private readonly INotifyCommand openCommand;
        private readonly INotifyCommand saveCommand;

        public void Close()
        {
            Console.WriteLine("Close");
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

        private ObservableCollection<ClassRoom> cclassroom;
        private ObservableCollection<Group> cgroup;
        private ObservableCollection<Teacher> cteacher;
        private ObservableCollection<Subject> csubject;
       
        public MainVM()
        {

            ClassDropListt = new ObservableCollection<ToDoItem>();
            for (int i = 0; i < 76; i++)
            {
                ToDoItem example = new ToDoItem { NameOfSubject = "", Specifics = "", NumberOfClassroom = 0, NameOfGroup = "" };
                ClassDropListt.Add(example);
            }

            indexClassrom = this.Factory.Backing(nameof(IndexClassroom), -1);
            indexGroup= this.Factory.Backing(nameof(IndexGroup), -1);
            indexTeacher = this.Factory.Backing(nameof(IndexTeacher), -1);
            indexSubject = this.Factory.Backing(nameof(IndexSubject), -1);

            var initclassroom = XMLRead.ReadClassroom(Path.ClassroomXml);
            cclassroom = new ObservableCollection<ClassRoom>(initclassroom);

            var initgroup = XMLRead.ReadGroup(Path.GroupXml);
            cgroup = new ObservableCollection<Group>(initgroup);

            var initteacher = XMLRead.ReadTeacher(Path.TeacherXml);
            cteacher = new ObservableCollection<Teacher>(initteacher);

            var initsubject = XMLRead.ReadSubject(Path.SubjectXml);
            csubject = new ObservableCollection<Subject>(initsubject);
            
            ClassGroups = cgroup;
            ClassSubjects = csubject;
            ClassTeachers = cteacher;
            ClassClassrooms = cclassroom;

            createCommand = this.Factory.CommandSync(Create);
            openCommand = this.Factory.CommandSync(Open);
            saveCommand = this.Factory.CommandSync(Save);

            closeWinCommand = this.Factory.CommandSync(Close);
        }

        public ObservableCollection<Group> ClassGroups { get; }
        public ObservableCollection<Subject> ClassSubjects { get; }
        public ObservableCollection<Teacher> ClassTeachers { get; }
        public ObservableCollection<ClassRoom> ClassClassrooms { get; }
        public ObservableCollection<ToDoItem> ClassDropListt { get; }

        public int IndexClassroom { get { return indexClassrom.Value; } set { indexClassrom.Value = value; } }
        public int IndexTeacher { get { return indexTeacher.Value; } set { indexTeacher.Value = value; } }
        public int IndexSubject { get { return indexSubject.Value; } set { indexSubject.Value = value; } }
        public int IndexGroup { get { return indexGroup.Value; } set { indexGroup.Value = value; } }

        public ICommand CloseWinCommand => closeWinCommand;
        
        public ICommand CreateCommand => createCommand;
        public ICommand OpenCommand => openCommand;
        public ICommand SaveCommand => saveCommand;
    }



}
