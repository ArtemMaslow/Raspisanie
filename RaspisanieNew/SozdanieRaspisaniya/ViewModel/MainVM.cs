using Raspisanie.Models;
using ClosedXML.Excel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Linq;

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

        private string path = "C:\\Users\\Artem\\Desktop\\2.xlsx";

        public MainVM()
        {

            ClassDropListt = new ObservableCollection<DropInformation>();
            for (int i = 0; i < 76; i++)
            {
                DropInformation example = new DropInformation { NameOfSubject = "", Specifics = "", NumberOfClassroom = "", NameOfGroup = "" };
                ClassDropListt.Add(example);
            }

            indexClassrom = this.Factory.Backing(nameof(IndexClassroom), -1);
            indexGroup = this.Factory.Backing(nameof(IndexGroup), -1);
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

            //============================================================================================
            DropInformation dropInformation = new DropInformation();
            IndexGroup = 1;
            IndexSubject = 2;
            IndexTeacher = 3;
            IndexClassroom = 4;

            dropInformation.NameOfGroup = cgroup[IndexGroup].NameOfGroup;
            dropInformation.NameOfSubject = csubject[IndexSubject].NameOfSubject;
            dropInformation.Specifics = csubject[IndexSubject].Specifics;
            dropInformation.NameOfTeacher = cteacher[IndexTeacher].FIO;
            dropInformation.NumberOfClassroom = cclassroom[IndexClassroom].NumberOfClassroom;

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("1");
            worksheet.Columns(3,cgroup.Count+2).Width = 30;

            for (IndexGroup = 0; IndexGroup < cgroup.Count; IndexGroup++)
            {
                    worksheet.Cell(1, IndexGroup+3).Value = cgroup[IndexGroup].NameOfGroup;
                if (IndexGroup + 1 == cgroup[IndexGroup].CodeOfGroup)
                {
                    worksheet.Cell(2, IndexGroup + 3).Value = dropInformation.NameOfSubject + " " + dropInformation.Specifics + " " + dropInformation.NumberOfClassroom + " " + dropInformation.NameOfTeacher;
                }
            }
            IndexGroup = 3;
            worksheet.Cell(2, 3).Value = dropInformation.NameOfSubject+ " " + dropInformation.Specifics + " " + dropInformation.NumberOfClassroom + " " + dropInformation.NameOfTeacher;
            workbook.SaveAs("2.xlsx");

            //============================================================================================
            createCommand = this.Factory.CommandSync(Create);
            openCommand = this.Factory.CommandSync(Open);
            saveCommand = this.Factory.CommandSync(Save);

            closeWinCommand = this.Factory.CommandSync(Close);
        }

        public ObservableCollection<Group> ClassGroups { get; }
        public ObservableCollection<Subject> ClassSubjects { get; }
        public ObservableCollection<Teacher> ClassTeachers { get; }
        public ObservableCollection<ClassRoom> ClassClassrooms { get; }
        public ObservableCollection<DropInformation> ClassDropListt { get; }

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
