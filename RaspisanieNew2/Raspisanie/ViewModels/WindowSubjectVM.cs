using Microsoft.Win32;
using Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowSubjectVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;
        private readonly INotifyCommand loadCommand;

        public WindowSubjectVM(ObservableCollection<Subject> classSubject, ObservableCollection<Department> departments)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);
            loadCommand = this.Factory.CommandSync(Load);

            ClassSubject = classSubject;
            this.departments = departments;
            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new SubjectVM(departments.ToArray());
            var wind = new NewSubject()
            {
                DataContext = context
            };
            wind.ShowDialog();
            if (wind.DialogResult == true)
            {
                if (context.Subject != null)
                    if (RequestToDataBase.Instance.requestInsertIntoSubject(context.Subject))
                    {
                        ClassSubject.Add(context.Subject);
                    }
            }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var subject = ClassSubject[Index];
                var context = new SubjectVM(subject, departments.ToArray());
                var wind = new NewSubject()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (wind.DialogResult == true)
                {
                    if (context.Subject != null)
                    {
                        if (RequestToDataBase.Instance.requestUpdateSubject(context.Subject, ClassSubject, Index))
                        {
                            ClassSubject[Index] = context.Subject;
                        }
                    }
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
                if (RequestToDataBase.Instance.requestDeleteFromSubject(ClassSubject, Index))
                {
                    ClassSubject.RemoveAt(Index);
                }
        }

        private void Load()
        {
            string pathToCsv = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файл csv|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                pathToCsv = openFileDialog.FileName;
            }
            if (File.Exists(pathToCsv))
            {
                char[] delimiters = new char[] { ';' };
                using (StreamReader reader = new StreamReader(pathToCsv, System.Text.Encoding.Default))
                {
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                        {
                            break;
                        }
                        string[] parts = line.Split(delimiters);
                        bool exist = false;
                        foreach (var subject in ClassSubject)
                        {
                            if (subject.NameOfSubject.Equals(parts[0].Trim(' ')))
                            {
                                exist = true;
                            }
                        }
                        if (!exist)
                        {
                            Department Department = null;
                            foreach (var dep in departments)
                            {
                                if (dep.NameOfDepartment.Equals(parts[1].Trim(' ')))
                                {
                                    Department = dep;
                                }
                            }
                            if (Department != null)
                            {
                                Subject subject = new Subject
                                {
                                    NameOfSubject = parts[0].Trim(' '),
                                    Department = Department
                                };

                                if (RequestToDataBase.Instance.requestInsertIntoSubject(subject))
                                {
                                    ClassSubject.Add(subject);
                                }
                            }
                        }
                    }
                }
            }
        }

        private ObservableCollection<Department> departments { get; }

        public ObservableCollection<Subject> ClassSubject { get; }

        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;
        public ICommand LoadCommand => loadCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
