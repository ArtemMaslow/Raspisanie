using Gu.Wpf.DataGrid2D;
using Microsoft.FSharp.Core;
using Models;
using SozdanieRaspisaniya.ViewModel.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using static Models.GeneticAlgorithm.GenerateSchedule;

namespace SozdanieRaspisaniya.ViewModel
{
    class MainVM : ViewModelBase
    {
        private readonly INotifyCommand closeWinCommand;
        private readonly INotifyCommand selectCommand;
        private readonly INotifyCommand openCommand;
        private readonly INotifyCommand saveToExcel;
        private readonly INotifyCommand clearCommand;
        private readonly INotifyCommand saveToDataBase;
        private readonly INotifyCommand selectN_DCommand;
        private readonly INotifyCommand readClasses;
        private readonly INotifyCommand exelFileToTeacher;
        private readonly INotifyCommand generateSchedule;
        private readonly INotifyCommand checkSchedule;

        private INotifyingValue<RowColumnIndex?> index;
        private INotifyingValue<int> departmentIndex;
        private INotifyingValue<bool> generalShedule;
        private INotifyingValue<object> element;

        private int ch = 0;
        private int itemstate = 0;

        int maxpair = 5 * SheduleSettings.WeekDayMaxCount + SheduleSettings.SaturdayMaxCount;

        public void Close()
        {

        }

        public void Open()
        {

        }

        public void Remove()
        {

        }

        public void Clear()
        {
            if (Index != null)
            {
                var value = Index.Value;
                DropInformation clearItem = null;
                if (ch == 0)
                {
                    clearItem = new DropInformation
                    {
                        Group = Filtered[value.Row][value.Column].Item.Group
                    };
                }
                else if (ch == 1)
                {
                    clearItem = new DropInformation
                    {
                        NumberOfClassroom = Filtered[value.Row][value.Column].Item.NumberOfClassroom
                    };
                }
                else if (ch == -1)
                {
                    clearItem = new DropInformation
                    {
                        Teacher = Filtered[value.Row][value.Column].Item.Teacher
                    };
                }
                Filtered[value.Row][value.Column].State = 0;
                Filtered[value.Row][value.Column].Item = clearItem;
                Filtered[value.Row][value.Column].ItemTwo = clearItem.Copy();
            }
        }

        private ObservableCollection<ObservableCollection<DropItem>> data;

        private void Numerator_Denominator(int to)
        {
            itemstate = to;
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Count; j++)
                {
                    data[i][j].N_DIndex = to;
                }
            }
        }

        private void Transform(int to)
        {
            ch = to;

            var temp = data.Select(x => x.ToArray()).ToArray();
            data.Clear();
            Columns.Clear();

            if (to == 0)
            {
                //x - группа, i - индекс
                //k - селекторор ключа(название группы), i - селектор эелемента(номер столбца)
                Dictionary<Group, int> dct = ClassGroups.Select((x, i) => new { i, x })
                .ToDictionary(k => k.x, e => e.i);//Каждому заголовку столбца ставится в соответствие его индекс.
                Type keyType = typeof(Group);
                foreach (var key in dct.Keys)
                    Columns.Add(key.ToString());

                foreach (var r in Rows)
                {
                    var row = new ObservableCollection<DropItem>();
                    foreach (var key in dct.Keys)
                    {
                        DropItem item = new DropItem(key, keyType, r);
                        item.N_DIndex = itemstate;
                        item.Item.Group.Add(key);
                        item.ItemTwo.Group.Add(key);

                        row.Add(item);
                    }
                    data.Add(row);
                }

                for (int i = 0; i < temp.Length; i++)
                {
                    for (int j = 0; j < temp[0].Length; j++)
                    {
                        int cind;
                        if (temp[i][j].Item.Group.Count >= 1)
                        {
                            foreach (var k in temp[i][j].Item.Group)
                            {
                                if (dct.TryGetValue(k, out cind))
                                {
                                    data[i][cind].Item = temp[i][j].Item.Copy();
                                    data[i][cind].Item.Group = new List<Group> { k };
                                    data[i][cind].State = temp[i][j].State;
                                }
                            }
                        }
                        if (temp[i][j].ItemTwo.Group.Count >= 1)
                        {
                            foreach (var k in temp[i][j].ItemTwo.Group)
                            {
                                if (dct.TryGetValue(k, out cind))
                                {
                                    data[i][cind].ItemTwo = temp[i][j].ItemTwo.Copy();
                                    data[i][cind].ItemTwo.Group = new List<Group> { k };
                                    data[i][cind].State = temp[i][j].State;
                                }
                            }
                        }
                    }
                }
            }
            else if (to == -1)
            {
                Dictionary<Teacher, int> dct = ClassTeachers.Select((x, i) => new { i, x })
                .ToDictionary(k => k.x, e => e.i);
                Type keyType = typeof(Teacher);
                foreach (var key in dct.Keys)
                    Columns.Add(key.ToString());

                foreach (var r in Rows)
                {
                    var row = new ObservableCollection<DropItem>();
                    foreach (var key in dct.Keys)
                    {
                        DropItem item = new DropItem(key, keyType, r);
                        item.N_DIndex = itemstate;
                        item.Item.Teacher = key;
                        item.ItemTwo.Teacher = key;

                        row.Add(item);
                    }
                    data.Add(row);
                }

                for (int i = 0; i < temp.Length; i++)
                {
                    for (int j = 0; j < temp[0].Length; j++)
                    {
                        int cind;
                        var current = temp[i][j];
                        if (current.Item.Teacher != null)
                        {
                            if (dct.TryGetValue(temp[i][j].Item.Teacher, out cind))
                            {

                                if (data[i][cind].Item.Group.Count >= 1)
                                    data[i][cind].Item.Group.AddRange(temp[i][j].Item.Group);
                                else
                                    data[i][cind].Item = temp[i][j].Item.Copy();
                                data[i][cind].State = temp[i][j].State;
                            }
                        }
                        if (current.ItemTwo.Teacher != null)
                        {
                            if (dct.TryGetValue(temp[i][j].ItemTwo.Teacher, out cind))
                            {
                                if (data[i][cind].ItemTwo.Group.Count >= 1)
                                    data[i][cind].ItemTwo.Group.AddRange(temp[i][j].ItemTwo.Group);
                                else
                                    data[i][cind].ItemTwo = temp[i][j].ItemTwo.Copy();
                                data[i][cind].State = temp[i][j].State;
                            }
                        }
                    }
                }
            }
            else
            {
                Dictionary<ClassRoom, int> dct = ClassClassrooms.Select((x, i) => new { i, x })
                .ToDictionary(k => k.x, e => e.i);
                Type keyType = typeof(ClassRoom);
                foreach (var key in dct.Keys)
                    Columns.Add(key.ToString());

                foreach (var r in Rows)
                {
                    var row = new ObservableCollection<DropItem>();
                    foreach (var key in dct.Keys)
                    {
                        DropItem item = new DropItem(key, keyType, r);
                        item.N_DIndex = itemstate;
                        item.Item.NumberOfClassroom = key;
                        item.ItemTwo.NumberOfClassroom = key;

                        row.Add(item);
                    }
                    data.Add(row);
                }
                for (int i = 0; i < temp.Length; i++)
                {
                    for (int j = 0; j < temp[0].Length; j++)
                    {
                        int cind;
                        if (temp[i][j].Item.NumberOfClassroom != null)
                        {
                            if (dct.TryGetValue(temp[i][j].Item.NumberOfClassroom, out cind))
                            {
                                if (data[i][cind].Item.Group.Count >= 1)
                                    data[i][cind].Item.Group.AddRange(temp[i][j].Item.Group);
                                else
                                    data[i][cind].Item = temp[i][j].Item.Copy();
                                data[i][cind].State = temp[i][j].State;
                            }
                        }
                        if (temp[i][j].ItemTwo.NumberOfClassroom != null)
                        {
                            if (dct.TryGetValue(temp[i][j].ItemTwo.NumberOfClassroom, out cind))
                            {
                                if (data[i][cind].ItemTwo.Group.Count >= 1)
                                    data[i][cind].ItemTwo.Group.AddRange(temp[i][j].ItemTwo.Group);
                                else
                                    data[i][cind].ItemTwo = temp[i][j].ItemTwo.Copy();
                                data[i][cind].State = temp[i][j].State;
                            }
                        }
                    }

                }
            }

            Filter();
        }

        public void Filter()
        {

            Filtered.Clear();

            foreach (var x in data)
            {
                if (GeneralShedule)
                {
                    Filtered.Add(x);
                }
                else
                {
                    IEnumerable<DropItem> f = Enumerable.Empty<DropItem>();
                    if (ch == 0)
                        f = x.Where(di => di.Item.Group.First().Department.CodeOfDepartment == ClassDepartments[DepartmentIndex].CodeOfDepartment);
                    else if (ch == -1)
                        f = x.Where(di => di.Item.Teacher.Department.CodeOfDepartment
                                        == ClassDepartments[DepartmentIndex].CodeOfDepartment);
                    else
                        f = x.Where(di => di.Item.NumberOfClassroom.Department.CodeOfDepartment
                                        == ClassDepartments[DepartmentIndex].CodeOfDepartment);
                    var inner = new ObservableCollection<DropItem>(f);
                    Filtered.Add(inner);
                }
            }
            Columns.Clear();

            foreach (var key in Filtered.First().Select(x => x.Key))
                Columns.Add(key.ToString());
            Rows.Clear();
            if (Columns.Count != 0)
                foreach (var row in (Filtered.Select(x => x[0].Info)))
                    Rows.Add(row);

        }

        public void ExportToExcel()
        {
            if (ch != 1)
            {
                var saveSchedule = new WorkWithExcel(Columns, Filtered, maxpair, ch);
                saveSchedule.ExportToExcel();
            }
            else
            {
                var saveSchedule = new WorkWithExcel(Columns, Filtered, maxpair, ch);
                saveSchedule.ExportToExcelClassrooms();
            }
        }

        public void SendExcelFile()
        {
            Transform(-1);
            var sendSchedule = new WorkWithExcel(Columns, Filtered, maxpair, ch);
            sendSchedule.SendExcelFile();
        }

        public void Init()
        {
            var limit = SheduleSettings.WeekDayMaxCount;

            for (int i = 0; i < ClassGroups.Length; i++)
            {
                int j = 0;
                foreach (DayOfWeek week in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (week == DayOfWeek.Sunday)
                        continue;
                    if (week == DayOfWeek.Saturday)
                        limit = SheduleSettings.SaturdayMaxCount;
                    else
                        limit = SheduleSettings.WeekDayMaxCount;

                    for (int k = 0; k < limit; k++)
                    {
                        var pair = new PairInfo(k + 1, week);
                        data[j].Add(new DropItem(ClassGroups[i], typeof(Group), pair)
                        {
                            Item = new DropInformation { Group = new List<Group> { ClassGroups[i] } },
                            ItemTwo = new DropInformation { Group = new List<Group> { ClassGroups[i] } }
                        });
                        j++;
                    }
                }
            }

            Filter();
        }

        public MainVM(int term)
        {
            ClassClassrooms = RequestToDataBase.Instance.ReadClassrooms().ToArray();
            ClassGroups = RequestToDataBase.Instance.ReadGroups(term).ToArray();
            ClassTeachers = RequestToDataBase.Instance.ReadTeachers().ToArray();
            ClassSubjects = RequestToDataBase.Instance.ReadSubjects().ToArray();
            ClassDepartments = RequestToDataBase.Instance.ReadDepartments().ToArray();
            Specifics = SheduleSettings.specifics;

            AllTeachersAndSubjects = new ObservableCollection<TeachersAndSubjects>();
            foreach (var value in RequestToDataBase.Instance.ReadTeacherAndSubjects())
            {
                AllTeachersAndSubjects.Add(value);
            }

            AllGroupsAndSubjects = new ObservableCollection<GroupsAndSubjects>();
            foreach (var value in RequestToDataBase.Instance.ReadGroupsAndSubjects(term))
            {
                AllGroupsAndSubjects.Add(value);
            }

            NameOfSchedule = RequestToDataBase.Instance.ReadFromClasses();

            data = new ObservableCollection<ObservableCollection<DropItem>>();
            Filtered = new ObservableCollection<ObservableCollection<DropItem>>();
            for (int i = 0; i < maxpair; i++)
                data.Add(new ObservableCollection<DropItem>());

            openCommand = this.Factory.CommandSync(Open);
            saveToExcel = this.Factory.CommandSync(ExportToExcel);
            selectCommand = this.Factory.CommandSyncParam<int>(Transform);
            closeWinCommand = this.Factory.CommandSync(Close);
            saveToDataBase = this.Factory.CommandSync(SaveSheduleToDataBase);
            readClasses = this.Factory.CommandSync(ReadFromClasses);
            clearCommand = this.Factory.CommandSync(Clear);
            selectN_DCommand = this.Factory.CommandSyncParam<int>(Numerator_Denominator);
            exelFileToTeacher = this.Factory.CommandSync(SendExcelFile);
            generateSchedule = this.Factory.CommandSync(ScheduleGeneration);
            checkSchedule = this.Factory.CommandSync(ScheduleCheck);

            index = this.Factory.Backing<RowColumnIndex?>(nameof(Index), null);
            departmentIndex = this.Factory.Backing<int>(nameof(DepartmentIndex), 0);
            generalShedule = this.Factory.Backing<bool>(nameof(GeneralShedule), true);
            element = this.Factory.Backing<object>(nameof(Element), null);
            Columns = new ObservableCollection<string>();
            Rows = new ObservableCollection<PairInfo>();
            Init();
        }

        public List<Lesson> PrepareListLessons(string[] Specifics, ClassRoom[] ClassClassrooms, ObservableCollection<GroupsAndSubjects> AllGroupsAndSubjects, ObservableCollection<TeachersAndSubjects> AllTeachersAndSubjects)
        {
            if (ClassClassrooms.Length > 0 && AllGroupsAndSubjects.Count > 0 && AllTeachersAndSubjects.Count > 0)
            {
                var listLessons = new List<Lesson>();

                int general = 0;
                int[] numdenum = { 1, -1 };
                int ndindex = 0;

                for (int i = 0; i < AllGroupsAndSubjects.Count; i++)
                {
                    var group = new List<Group>();
                    group.Add(AllGroupsAndSubjects[i].Group);
                    Random rnd = new Random();

                    foreach (var valueGroupAndSubjects in AllGroupsAndSubjects[i].InformationAboutSubjects)
                    {
                        while ((valueGroupAndSubjects.LectureHour != 0) || (valueGroupAndSubjects.ExerciseHour != 0) || (valueGroupAndSubjects.LaboratoryHour != 0))
                        {
                            if (valueGroupAndSubjects.LectureHour != 0)
                            {
                                if (valueGroupAndSubjects.LectureHour % 2 == 0)
                                {
                                    ndindex = general;
                                    valueGroupAndSubjects.LectureHour -= 2;
                                }
                                else
                                {
                                    ndindex = numdenum.ElementAt(rnd.Next(numdenum.Length));
                                    valueGroupAndSubjects.LectureHour -= 1;
                                }

                                var tempListTeacher = new List<Teacher>();
                                foreach (var valueTeacher in AllTeachersAndSubjects)
                                {
                                    foreach (var valueSubject in valueTeacher.SubjectList)
                                    {
                                        if (valueGroupAndSubjects.Subject == valueSubject && valueTeacher.Teacher.IsReadLecture == true)
                                        {
                                            tempListTeacher.Add(valueTeacher.Teacher);
                                        }
                                    }
                                }

                                var lectureListClassrooms = new List<ClassRoom>();
                                foreach (var valueClassroms in ClassClassrooms)
                                {
                                    if (valueClassroms.Specific.Equals(Specifics[0]) && valueGroupAndSubjects.Subject.Department.CodeOfDepartment == valueClassroms.Department.CodeOfDepartment)
                                    {
                                        lectureListClassrooms.Add(valueClassroms);
                                    }
                                }

                                if (tempListTeacher.Count > 0 && lectureListClassrooms.Count > 0)
                                {
                                    listLessons.Add(new Lesson(new DropInformation(group, tempListTeacher.ElementAt(rnd.Next(tempListTeacher.Count)), valueGroupAndSubjects.Subject, Specifics[0], lectureListClassrooms.ElementAt(rnd.Next(lectureListClassrooms.Count)), ndindex)));
                                }
                                else
                                {
                                    Console.WriteLine("лекц." + valueGroupAndSubjects.Subject.NameOfSubject + " " + AllGroupsAndSubjects[i].Group.NameOfGroup);
                                }
                            }
                            else if (valueGroupAndSubjects.ExerciseHour != 0)
                            {

                                if (valueGroupAndSubjects.ExerciseHour % 2 == 0)
                                {
                                    ndindex = general;
                                    valueGroupAndSubjects.ExerciseHour -= 2;
                                }
                                else
                                {
                                    ndindex = numdenum.ElementAt(rnd.Next(numdenum.Length));
                                    valueGroupAndSubjects.ExerciseHour -= 1;
                                }

                                var tempListTeacher = new List<Teacher>();
                                foreach (var valueTeacher in AllTeachersAndSubjects)
                                {
                                    foreach (var valueSubject in valueTeacher.SubjectList)
                                    {
                                        if (valueGroupAndSubjects.Subject == valueSubject)
                                        {
                                            tempListTeacher.Add(valueTeacher.Teacher);
                                        }
                                    }
                                }

                                var lectureAndExerciseListClassrooms = new List<ClassRoom>();
                                foreach (var valueClassroms in ClassClassrooms)
                                {
                                    if ((valueClassroms.Specific.Equals(Specifics[0]) || valueClassroms.Specific.Equals(Specifics[1])) && valueGroupAndSubjects.Subject.Department.CodeOfDepartment == valueClassroms.Department.CodeOfDepartment)
                                    {
                                        lectureAndExerciseListClassrooms.Add(valueClassroms);
                                    }
                                }

                                if (tempListTeacher.Count > 0 && lectureAndExerciseListClassrooms.Count > 0)
                                {
                                    listLessons.Add(new Lesson(new DropInformation(group, tempListTeacher.ElementAt(rnd.Next(tempListTeacher.Count)), valueGroupAndSubjects.Subject, Specifics[1], lectureAndExerciseListClassrooms.ElementAt(rnd.Next(lectureAndExerciseListClassrooms.Count)), ndindex)));
                                }
                                else
                                {
                                    Console.WriteLine("лекц." + valueGroupAndSubjects.Subject.NameOfSubject + " " + AllGroupsAndSubjects[i].Group.NameOfGroup);
                                }
                            }
                            else if (valueGroupAndSubjects.LaboratoryHour != 0)
                            {
                                if (valueGroupAndSubjects.LaboratoryHour % 2 == 0)
                                {
                                    ndindex = general;
                                    valueGroupAndSubjects.LaboratoryHour -= 2;
                                }
                                else
                                {
                                    ndindex = numdenum.ElementAt(rnd.Next(numdenum.Length));
                                    valueGroupAndSubjects.LaboratoryHour -= 1;
                                }

                                var tempListTeacher = new List<Teacher>();
                                foreach (var valueTeacher in AllTeachersAndSubjects)
                                {
                                    foreach (var valueSubject in valueTeacher.SubjectList)
                                    {
                                        if (valueGroupAndSubjects.Subject == valueSubject)
                                        {
                                            tempListTeacher.Add(valueTeacher.Teacher);
                                        }
                                    }
                                }

                                var labListClassrooms = new List<ClassRoom>();
                                foreach (var valueClassroms in ClassClassrooms)
                                {
                                    if (valueClassroms.Specific.Equals(Specifics[2]) && valueGroupAndSubjects.Subject.Department.CodeOfDepartment == valueClassroms.Department.CodeOfDepartment)
                                    {
                                        labListClassrooms.Add(valueClassroms);
                                    }
                                }
                                if (tempListTeacher.Count > 0 && labListClassrooms.Count > 0)
                                {
                                    listLessons.Add(new Lesson(new DropInformation(group, tempListTeacher.ElementAt(rnd.Next(tempListTeacher.Count)), valueGroupAndSubjects.Subject, Specifics[2], labListClassrooms.ElementAt(rnd.Next(labListClassrooms.Count)), ndindex)));
                                }
                                else
                                {
                                    Console.WriteLine("лекц." + valueGroupAndSubjects.Subject.NameOfSubject + " " + AllGroupsAndSubjects[i].Group.NameOfGroup);
                                }
                            }
                        }
                    }
                }
                return listLessons;
            }
            return null;
        }

        public void ValidationForDrop(object element)
        {
            //объявляем переменную данных перетаскиваемого элемента
            var sourceItem = element is Subject || element is Teacher || element is Group || element is ClassRoom || element is string;
            //объявляем переменную и смотрим на соответствие одного из 4 шаблонов
            foreach (var row in Filtered)
            {
                foreach (var item in row)
                {
                    if (sourceItem && element.GetType() != item.KeyType)//если шаблон данных и тип перетаскиваемого элемента не равен типу ключа 
                    {
                        if (element is Teacher teacher)
                        {
                            item.IsValueValid = false;
                            foreach (var value in AllTeachersAndSubjects)
                            {
                                if (value.Teacher.CodeOfTeacher == teacher.CodeOfTeacher
                                    && value.Teacher.Department.CodeOfDepartment == teacher.Department.CodeOfDepartment)
                                {
                                    foreach (var group in AllGroupsAndSubjects)
                                    {
                                        if (item.N_DIndex == 0 || item.N_DIndex == 1)
                                        {
                                            if (item.Item.Group.Exists(g => g.CodeOfGroup == group.Group.CodeOfGroup))
                                            {
                                                foreach (var valuesub in value.SubjectList)
                                                {
                                                    if ((value.DayList.ToList().Exists(t => t == item.Info.Day)
                                                        && (group.InformationAboutSubjects.ToList().Exists(s => s.Subject.CodeOfSubject == valuesub.CodeOfSubject))))
                                                    {
                                                        item.IsValueValid = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (item.N_DIndex == -1)
                                        {
                                            if (item.ItemTwo.Group.Exists(g => g.CodeOfGroup == group.Group.CodeOfGroup))
                                            {
                                                foreach (var valuesub in value.SubjectList)
                                                {
                                                    item.IsValueValid = value.DayList.ToList().Exists(t => t == item.Info.Day)
                                                        && (group.InformationAboutSubjects.ToList().Exists(s => s.Subject.CodeOfSubject == valuesub.CodeOfSubject));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (element is Group group)
                        {
                            item.IsValueValid = false;
                            if (item.Item.Teacher != null && (item.N_DIndex == 0 || item.N_DIndex == 1))
                            {
                                foreach (var value in AllTeachersAndSubjects)
                                {
                                    if (value.Teacher.CodeOfTeacher == item.Item.Teacher.CodeOfTeacher
                                        && value.Teacher.Department.CodeOfDepartment == item.Item.Teacher.Department.CodeOfDepartment)
                                    {
                                        if (value.DayList.ToList().Exists(t => t == item.Info.Day))
                                        {
                                            foreach (var groupvalue in AllGroupsAndSubjects)
                                            {
                                                if (groupvalue.Group.CodeOfGroup == group.CodeOfGroup)
                                                {
                                                    foreach (var valuesub in groupvalue.InformationAboutSubjects)
                                                    {
                                                        if ((value.SubjectList.ToList().Exists(s => s.CodeOfSubject == valuesub.Subject.CodeOfSubject)))
                                                        {
                                                            item.IsValueValid = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            if (item.ItemTwo.Teacher != null && item.N_DIndex == -1)
                            {
                                foreach (var value in AllTeachersAndSubjects)
                                {
                                    if (value.Teacher.CodeOfTeacher == item.ItemTwo.Teacher.CodeOfTeacher
                                        && value.Teacher.Department.CodeOfDepartment == item.ItemTwo.Teacher.Department.CodeOfDepartment)
                                    {
                                        if (value.DayList.ToList().Exists(t => t == item.Info.Day))
                                        {
                                            foreach (var groupvalue in AllGroupsAndSubjects)
                                            {
                                                if (groupvalue.Group.CodeOfGroup == group.CodeOfGroup)
                                                {
                                                    foreach (var valuesub in groupvalue.InformationAboutSubjects)
                                                    {
                                                        if ((value.SubjectList.ToList().Exists(s => s.CodeOfSubject == valuesub.Subject.CodeOfSubject)))
                                                        {
                                                            item.IsValueValid = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (element is Subject subject)
                        {
                            item.IsValueValid = false;
                            if (item.Item.Teacher != null && (item.N_DIndex == 0 || item.N_DIndex == 1))
                            {
                                foreach (var groupvalue in AllGroupsAndSubjects)
                                {
                                    foreach (var value in AllTeachersAndSubjects)
                                    {
                                        if (value.Teacher.CodeOfTeacher == item.Item.Teacher.CodeOfTeacher
                                            && value.Teacher.Department.CodeOfDepartment == item.Item.Teacher.Department.CodeOfDepartment
                                                && item.Item.Group.Exists(g => g.CodeOfGroup == groupvalue.Group.CodeOfGroup))
                                        {
                                            item.IsValueValid = (value.SubjectList.ToList().Exists(t => t.CodeOfSubject == subject.CodeOfSubject)) && (groupvalue.InformationAboutSubjects.ToList().Exists(g => g.Subject.CodeOfSubject == subject.CodeOfSubject));
                                        }
                                    }
                                }
                            }
                            else
                            if (item.ItemTwo.Teacher != null && item.N_DIndex == -1)
                            {
                                foreach (var groupvalue in AllGroupsAndSubjects)
                                {
                                    foreach (var value in AllTeachersAndSubjects)
                                    {
                                        if (value.Teacher.CodeOfTeacher == item.ItemTwo.Teacher.CodeOfTeacher
                                            && value.Teacher.Department.CodeOfDepartment == item.ItemTwo.Teacher.Department.CodeOfDepartment
                                                && item.ItemTwo.Group.Exists(g => g.CodeOfGroup == groupvalue.Group.CodeOfGroup))
                                        {
                                            item.IsValueValid = (value.SubjectList.ToList().Exists(t => t.CodeOfSubject == subject.CodeOfSubject)) && (groupvalue.InformationAboutSubjects.ToList().Exists(g => g.Subject.CodeOfSubject == subject.CodeOfSubject));
                                        }
                                    }
                                }
                            }
                        }
                        else if (element is string specific)
                        {
                            item.IsValueValid = false;
                            if (item.Item.Subject != null && (item.N_DIndex == 0 || item.N_DIndex == 1))
                            {
                                foreach (var groupvalue in AllGroupsAndSubjects)
                                {
                                    if (item.Item.Group.Exists(g => g.CodeOfGroup == groupvalue.Group.CodeOfGroup))
                                    {
                                        foreach (var groupsubject in groupvalue.InformationAboutSubjects)
                                        {
                                            if (specific == SheduleSettings.specifics[0] && groupsubject.LectureHour > 0)
                                            {
                                                //groupsubject.LectureHour -= 2;
                                                item.IsValueValid = true;
                                            }

                                            if (specific == SheduleSettings.specifics[1] && groupsubject.ExerciseHour > 0)
                                            {
                                                //groupsubject.ExerciseHour -= 2;
                                                item.IsValueValid = true;
                                            }

                                            if (specific == SheduleSettings.specifics[2] && groupsubject.LaboratoryHour > 0)
                                            {
                                                //groupsubject.LaboratoryHour -= 2;
                                                item.IsValueValid = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (item.ItemTwo.Subject != null && item.N_DIndex == -1)
                            {
                                item.IsValueValid = false;
                                foreach (var groupvalue in AllGroupsAndSubjects)
                                {
                                    if (item.ItemTwo.Group.Exists(g => g.CodeOfGroup == groupvalue.Group.CodeOfGroup))
                                    {
                                        foreach (var groupsubject in groupvalue.InformationAboutSubjects)
                                        {
                                            if (specific == SheduleSettings.specifics[0] && groupsubject.LectureHour > 0)
                                            {
                                                //groupsubject.LectureHour -= 1;
                                                item.IsValueValid = true;
                                            }

                                            if (specific == SheduleSettings.specifics[1] && groupsubject.ExerciseHour > 0)
                                            {
                                                //groupsubject.ExerciseHour -= 1;
                                                item.IsValueValid = true;
                                            }

                                            if (specific == SheduleSettings.specifics[2] && groupsubject.LaboratoryHour > 0)
                                            {
                                                //groupsubject.LaboratoryHour -= 1;
                                                item.IsValueValid = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (element is ClassRoom classroom)
                        {
                            item.IsValueValid = false;
                            if (item.Item.Specifics != null && (item.N_DIndex == 0 || item.N_DIndex == 1))
                            {
                                if (item.Item.Specifics == SheduleSettings.specifics[1])
                                {
                                    if (classroom.Specific == SheduleSettings.specifics[0] || classroom.Specific == SheduleSettings.specifics[1])
                                    {
                                        item.IsValueValid = true;
                                    }
                                }
                                else if (item.Item.Specifics == classroom.Specific)
                                {
                                    item.IsValueValid = true;
                                }
                            }
                            else if (item.ItemTwo.Specifics != null && item.N_DIndex == -1)
                            {
                                if (item.ItemTwo.Specifics == SheduleSettings.specifics[1])
                                {
                                    if (classroom.Specific == SheduleSettings.specifics[0] || classroom.Specific == SheduleSettings.specifics[1])
                                    {
                                        item.IsValueValid = true;
                                    }
                                }
                                else if (item.ItemTwo.Specifics == classroom.Specific)
                                {
                                    item.IsValueValid = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        item.IsValueValid = false;
                    }
                }
            }
        }

        public void SaveSheduleToDataBase()
        {
            NameOfSchedule = RequestToDataBase.Instance.ReadFromClasses();

            var context = new SaveScheduleVM(NameOfSchedule.ToArray());
            var winsave = new SaveSchedule()
            {
                DataContext = context
            };
            winsave.ShowDialog();

            if (winsave.DialogResult == true)
            {
                string name;
                if (context.Name.IsChoice1Of2)
                    name = ((FSharpChoice<string, string>.Choice1Of2)context.Name).Item;
                else
                {
                    name = ((FSharpChoice<string, string>.Choice2Of2)context.Name).Item;
                    RequestToDataBase.Instance.clearClasses(name);
                }
                if (name != null)
                {
                    GeneralShedule = true;
                    Filter();
                    //Transform(0);
                    Console.Clear();
                    for (int i = 0; i < Filtered.Count; i++)
                    {
                        for (int j = 0; j < Filtered[i].Count; j++)
                        {
                            if ((Filtered[i][j].Item.Group != null) && (Filtered[i][j].Item.NumberOfClassroom != null) && (Filtered[i][j].Item.Specifics != null) && (Filtered[i][j].Item.Subject != null) && (Filtered[i][j].Item.Teacher != null))
                            {
                                Console.WriteLine("Day:" + Filtered[i][j].Info.Day + " pair:" + Filtered[i][j].Info.Pair + " Key:" + Filtered[i][j].Key + " KeyType: " + Filtered[i][j].KeyType + " State:" + Filtered[i][j].State + " ND:" + Filtered[i][j].Item.Ndindex + " NDNUM " + Filtered[i][j].N_DIndex);
                                RequestToDataBase.Instance.requestInsertIntoClassesItemOne(Filtered[i][j], name);
                            }
                            if ((Filtered[i][j].ItemTwo.Group != null) && (Filtered[i][j].ItemTwo.NumberOfClassroom != null) && (Filtered[i][j].ItemTwo.Specifics != null) && (Filtered[i][j].ItemTwo.Subject != null) && (Filtered[i][j].ItemTwo.Teacher != null))
                            {
                                Console.WriteLine("Day:" + Filtered[i][j].Info.Day + " pair:" + Filtered[i][j].Info.Pair + " Key:" + Filtered[i][j].Key + " KeyType: " + Filtered[i][j].KeyType + " State:" + Filtered[i][j].State + " ND:" + Filtered[i][j].ItemTwo.Ndindex + "NDNUM " + Filtered[i][j].N_DIndex);
                                RequestToDataBase.Instance.requestInsertIntoClassesItemTwo(Filtered[i][j], name);
                            }
                        }
                    }
                }
                MessageBox.Show("Save");
            }
        }

        public void ReadFromClasses()
        {
            NameOfSchedule = RequestToDataBase.Instance.ReadFromClasses();

            var context = new ReadFromClassesVM(NameOfSchedule.ToArray());
            var winrfc = new ReadFromClasses()
            {
                DataContext = context
            };
            winrfc.ShowDialog();

            if (context.Name != null)
            {
                DropItem[] Elem;
                Elem = RequestToDataBase.Instance.ReadClasses(ClassGroups, context.Name).ToArray();
                GeneralShedule = true;
                Filter();
                Transform(0);
                for (int k = 0; k < Elem.Length; k++)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        for (int j = 0; j < data[i].Count; j++)
                        {
                            if ((Elem[k].Info.Day == data[i][j].Info.Day) && (Elem[k].Info.Pair == data[i][j].Info.Pair) && (Elem[k].Key is Group eg && data[i][j].Key is Group kg && eg.CodeOfGroup == kg.CodeOfGroup) /*(Elem[k].Key == data[i][j].Key)*/ && (Elem[k].KeyType == data[i][j].KeyType))
                            {
                                data[i][j].State = Elem[k].State;
                                data[i][j].N_DIndex = Elem[k].N_DIndex;

                                if (Elem[k].State == 0 || Elem[k].State == 1)
                                {
                                    data[i][j].Item = Elem[k].Item;
                                }
                                else
                                {
                                    data[i][j].ItemTwo = Elem[k].ItemTwo;
                                }
                            }
                        }
                    }
                }
                Filter();
            }
        }

        public void ScheduleGeneration()
        {

            // ----------------Тестирование генерации------------------------------
            //Stopwatch mywatch = new Stopwatch();

            var list = PrepareListLessons(SheduleSettings.specifics, ClassClassrooms, AllGroupsAndSubjects, AllTeachersAndSubjects);

            //mywatch.Start();

            var solver = new Solver();

            Plan.DaysPerWeek = 6;
            Plan.HoursPerDay = 6;
            FitnessFunctions.gas = AllGroupsAndSubjects.ToArray();
            FitnessFunctions.tas = AllTeachersAndSubjects.ToArray();

            //solver.FitnessFunctions.Add(FitnessFunctions.Windows);
            solver.FitnessFunctions.Add(FitnessFunctions.CountPairTeachers);
            solver.FitnessFunctions.Add(FitnessFunctions.CountLecturePairTeachers);
            solver.FitnessFunctions.Add(FitnessFunctions.CountPairGroups);
            solver.FitnessFunctions.Add(FitnessFunctions.CountLecturePairGroups);
            //solver.FitnessFunctions.Add(FitnessFunctions.CountMoveFromFiveHousingToOtherAndConversely);

            var res = solver.Solve(list);
            Representation(res);
            //mywatch.Stop();
            //Console.WriteLine("Работа алгоритма время в секундах: " + mywatch.ElapsedMilliseconds / 1000);
            Console.WriteLine(res);

            //----------------------------------
        }

        public void Representation(Plan plan)
        {
            for (int i = 0; i < Filtered.Count; i++)
            {
                for (int j = 0; j < Filtered[i].Count; j++)
                {
                    for (int day = 0; day < Plan.DaysPerWeek; day++)
                    {
                        for (int hour = 0; hour < Plan.HoursPerDay; hour++)
                        {
                            foreach (var p in plan.HourPlans[day, hour].GroupInform)
                            {
                                if (((int)Filtered[i][j].Info.Day == (day + 1)) && (Filtered[i][j].Info.Pair == (hour + 1)) && (Filtered[i][j].Key == (object)ClassGroups.Single(g => g.CodeOfGroup == p.Key)))
                                {
                                    Filtered[i][j].N_DIndex = p.Value.Ndindex;
                                    Filtered[i][j].State = p.Value.Ndindex;
                                    if (p.Value.Ndindex == 0 || p.Value.Ndindex == 1)
                                    {
                                        Filtered[i][j].Item = p.Value;
                                    }
                                    else if (p.Value.Ndindex == -1)
                                    {
                                        Filtered[i][j].ItemTwo = p.Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ScheduleCheck()
        {
            //Transform(0);
            var val = new RuleEngine(Filtered);
            val.AddRule(new CountPair());
            val.ApplyRules();
            val.ShowErrors();
        }

        public ObservableCollection<TeachersAndSubjects> AllTeachersAndSubjects { get; }
        public ObservableCollection<GroupsAndSubjects> AllGroupsAndSubjects { get; }

        public ObservableCollection<ObservableCollection<DropItem>> Filtered { get; }
        public ObservableCollection<string> Columns { get; }
        public ObservableCollection<PairInfo> Rows { get; }
        public Group[] ClassGroups { get; }
        public Subject[] ClassSubjects { get; }
        public Teacher[] ClassTeachers { get; }
        public ClassRoom[] ClassClassrooms { get; }
        public Department[] ClassDepartments { get; }
        public string[] Specifics { get; }

        public List<string> NameOfSchedule;

        public RowColumnIndex? Index { get { return index.Value; } set { index.Value = value; } }
        public int DepartmentIndex { get { return departmentIndex.Value; } set { departmentIndex.Value = value; Filter(); } }
        public bool GeneralShedule { get { return generalShedule.Value; } set { generalShedule.Value = value; Filter(); } }

        public object Element { get { return element.Value; } set { element.Value = value; ValidationForDrop(value); } }

        public ICommand CloseWinCommand => closeWinCommand;
        public ICommand OpenCommand => openCommand;
        public ICommand SaveToExcel => saveToExcel;
        public ICommand SelectCommand => selectCommand;
        public ICommand ClearCommand => clearCommand;
        public ICommand SelectN_DCommand => selectN_DCommand;
        public ICommand SaveToDataBase => saveToDataBase;
        public ICommand ReadClasses => readClasses;
        public ICommand ExelFileToTeacher => exelFileToTeacher;
        public ICommand GenerateSchedule => generateSchedule;
        public ICommand CheckSchedule => checkSchedule;
    }

}
