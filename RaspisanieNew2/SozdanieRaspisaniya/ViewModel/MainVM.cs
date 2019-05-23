using Gu.Wpf.DataGrid2D;
using Microsoft.FSharp.Core;
using Models;
using SozdanieRaspisaniya.ViewModel.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
                                {
                                    data[i][cind].Item.Group.AddRange(temp[i][j].Item.Group);
                                    var listGroup = data[i][cind].Item.Group.Distinct().ToList();
                                    data[i][cind].Item.Group.Clear();
                                    data[i][cind].Item.Group.AddRange(listGroup);
                                }
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
                                {
                                    data[i][cind].ItemTwo.Group.AddRange(temp[i][j].ItemTwo.Group);
                                    var listGroup = data[i][cind].ItemTwo.Group.Distinct().ToList();
                                    data[i][cind].ItemTwo.Group.Clear();
                                    data[i][cind].ItemTwo.Group.AddRange(listGroup);
                                }
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
                                {
                                    data[i][cind].Item.Group.AddRange(temp[i][j].Item.Group);
                                    var listGroup = data[i][cind].Item.Group.Distinct().ToList();
                                    data[i][cind].Item.Group.Clear();
                                    data[i][cind].Item.Group.AddRange(listGroup);
                                }
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
                                {
                                    data[i][cind].ItemTwo.Group.AddRange(temp[i][j].ItemTwo.Group);
                                    var listGroup = data[i][cind].ItemTwo.Group.Distinct().ToList();
                                    data[i][cind].ItemTwo.Group.Clear();
                                    data[i][cind].ItemTwo.Group.AddRange(listGroup);
                                }
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
            if (ch == 0)
            {
                var saveSchedule = new WorkWithExcel(Columns, Filtered, maxpair, ch);
                saveSchedule.ExportToExcel();
            }
            else if (ch == 1)
            {
                var saveSchedule = new WorkWithExcel(Columns, Filtered, maxpair, ch);
                saveSchedule.ExportToExcelSeparately();
            }
            else if (ch == -1)
            {
                MessageBoxResult result = MessageBox.Show("Как сохранить расписание? \nВ один файл (Да) Раздельно (Нет)", "Выбор расписания", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    //общее
                    var saveSchedule = new WorkWithExcel(Columns, Filtered, maxpair, ch);
                    saveSchedule.ExportToExcel();
                }
                else if (result == MessageBoxResult.No)
                {
                    //раздельное
                    var saveSchedule = new WorkWithExcel(Columns, Filtered, maxpair, ch);
                    saveSchedule.ExportToExcelSeparately();
                }
                else
                {
                    this.Close();
                }
            }
        }

        public void SendExcelFile()
        {
            if (ch == -1)
            {
                var sendSchedule = new WorkWithExcel(Columns, Filtered, maxpair, ch);
                sendSchedule.SendExcelFile();
            }
            else
            {
                MessageBox.Show("Для выполнения данного действия перейдите в категорию расписания для преподователей", "Внимание");
            }

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
            List<GroupsAndSubjects> AllGroupsAndSubjectsValue = new List<GroupsAndSubjects>();
            foreach (var item in AllGroupsAndSubjects)
            {
                AllGroupsAndSubjectsValue.Add((GroupsAndSubjects)item.Clone());
            }

            List<TeachersAndSubjects> AllTeachersAndSubjectsValue = new List<TeachersAndSubjects>();
            foreach (var item in AllTeachersAndSubjects)
            {
                AllTeachersAndSubjectsValue.Add(item);
            }

            if (ClassClassrooms.Length > 0 && AllGroupsAndSubjectsValue.Count > 0 && AllTeachersAndSubjectsValue.Count > 0)
            {
                var listLessons = new List<Lesson>();

                int general = 0;
                int[] numdenum = { 1, -1 };
                int ndindex = 0;

                Random rnd = new Random();
                for (int i = 0; i < AllGroupsAndSubjectsValue.Count; i++)
                {
                    var group = new List<Group>();
                    group.Add(AllGroupsAndSubjectsValue[i].Group);

                    foreach (var valueGroupAndSubjects in AllGroupsAndSubjectsValue[i].InformationAboutSubjects)
                    {
                        bool condition = (valueGroupAndSubjects.LectureHour != 0) || (valueGroupAndSubjects.ExerciseHour != 0) || (valueGroupAndSubjects.LaboratoryHour != 0);
                        while (condition)
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
                                foreach (var valueTeacher in AllTeachersAndSubjectsValue)
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
                                    var teacher = tempListTeacher[rnd.Next(tempListTeacher.Count)];
                                    var subject = valueGroupAndSubjects.Subject;
                                    var specific = Specifics[0];
                                    var room = lectureListClassrooms[rnd.Next(lectureListClassrooms.Count)];
                                    var di = new DropInformation(group, teacher, subject, specific, room, ndindex);
                                    listLessons.Add(new Lesson(di));
                                }
                                else
                                {
                                    Console.WriteLine("лекц." + valueGroupAndSubjects.Subject.NameOfSubject + " " + AllGroupsAndSubjectsValue[i].Group.NameOfGroup);
                                }
                            }

                            if (valueGroupAndSubjects.ExerciseHour != 0)
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
                                foreach (var valueTeacher in AllTeachersAndSubjectsValue)
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
                                    var teacher = tempListTeacher[rnd.Next(tempListTeacher.Count)];
                                    var subject = valueGroupAndSubjects.Subject;
                                    var specific = Specifics[1];
                                    var room = lectureAndExerciseListClassrooms[rnd.Next(lectureAndExerciseListClassrooms.Count)];
                                    var di = new DropInformation(group, teacher, subject, specific, room, ndindex);

                                    listLessons.Add(new Lesson(di));
                                }
                                else
                                {
                                    Console.WriteLine("упр." + valueGroupAndSubjects.Subject.NameOfSubject + " " + AllGroupsAndSubjectsValue[i].Group.NameOfGroup);
                                }
                            }

                            if (valueGroupAndSubjects.LaboratoryHour != 0)
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
                                foreach (var valueTeacher in AllTeachersAndSubjectsValue)
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
                                    var teacher = tempListTeacher[rnd.Next(tempListTeacher.Count)];
                                    var subject = valueGroupAndSubjects.Subject;
                                    var specific = Specifics[2];
                                    var room = labListClassrooms[rnd.Next(labListClassrooms.Count)];
                                    var di = new DropInformation(group, teacher, subject, specific, room, ndindex);
                                    listLessons.Add(new Lesson(di));
                                }
                                else
                                {
                                    Console.WriteLine("лаб." + valueGroupAndSubjects.Subject.NameOfSubject + " " + AllGroupsAndSubjectsValue[i].Group.NameOfGroup);
                                }
                            }
                            condition = (valueGroupAndSubjects.LectureHour != 0) || (valueGroupAndSubjects.ExerciseHour != 0) || (valueGroupAndSubjects.LaboratoryHour != 0);
                        }
                    }
                }
                Console.WriteLine(listLessons.Count);
                return listLessons;
            }
            return null;
        }

        public List<Lesson> UnionPair(List<Lesson> listLessons)
        {
            var lessonsList = listLessons
                .Select(group => (getID(group), group))
                .GroupBy(group => group.Item1, (_, v) => v.Select(kv => kv.Item2)
                .Aggregate(MergeGroup)).ToList();

            Console.WriteLine(lessonsList.Count);
            return lessonsList;
        }

        public Lesson MergeGroup(Lesson l1, Lesson l2)
        {
            Lesson lesson;
            l1.dropInfo.Group.AddRange(l2.dropInfo.Group);
            lesson = new Lesson(l1.dropInfo);
            return lesson;
        }

        public string getID(Lesson les)
        {
            if (les.dropInfo != null && les.dropInfo.Specifics == SheduleSettings.specifics[0])
            {
                var subject = les.dropInfo.Subject.CodeOfSubject;
                var nameOfGroup = les.dropInfo.Group.Single().NameOfGroup;
                var term = nameOfGroup.Substring(nameOfGroup.Length - 2, 1);
                var specific = les.dropInfo.Specifics;
                return $"{subject}_{term}_{specific}";
            }
            else if (les.dropInfoTwo != null && les.dropInfoTwo.Specifics == SheduleSettings.specifics[0])
            {
                var subject = les.dropInfoTwo.Subject.CodeOfSubject;
                var nameOfGroup = les.dropInfoTwo.Group.Single().NameOfGroup;
                var term = nameOfGroup.Substring(nameOfGroup.Length - 2, 1);
                var specific = les.dropInfoTwo.Specifics;
                return $"{subject}_{term}_{specific}";
            }
            else
            {
                return Guid.NewGuid().ToString();
            }
        }

        public void ValidationForDrop(object element)
        {
            //объявляем переменную данных перетаскиваемого элемента
            var sourceItem = element is Subject || element is Teacher || element is Group || element is ClassRoom || element is string;
            //объявляем переменную и смотрим на соответствие одного из 4 шаблонов
            var tempAllGroupsAndSubjects = new List<GroupsAndSubjects>();
            foreach (var item in AllGroupsAndSubjects)
            {
                tempAllGroupsAndSubjects.Add((GroupsAndSubjects)item.Clone());
            }

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
                                    foreach (var group in tempAllGroupsAndSubjects)
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
                                                    if (value.DayList.ToList().Exists(t => t == item.Info.Day)
                                                        && (group.InformationAboutSubjects.ToList().Exists(s => s.Subject.CodeOfSubject == valuesub.CodeOfSubject)))
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
                                            foreach (var groupvalue in tempAllGroupsAndSubjects)
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
                                            foreach (var groupvalue in tempAllGroupsAndSubjects)
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
                                foreach (var groupvalue in tempAllGroupsAndSubjects)
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
                                foreach (var groupvalue in tempAllGroupsAndSubjects)
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
                                foreach (var groupvalue in tempAllGroupsAndSubjects)
                                {
                                    if (item.Item.Group.Exists(g => g.CodeOfGroup == groupvalue.Group.CodeOfGroup))
                                    {
                                        foreach (var groupsubject in groupvalue.InformationAboutSubjects)
                                        {
                                            if (specific == SheduleSettings.specifics[0] && groupsubject.LectureHour > 0)
                                            {
                                                item.IsValueValid = true;
                                            }

                                            if (specific == SheduleSettings.specifics[1] && groupsubject.ExerciseHour > 0)
                                            {
                                                item.IsValueValid = true;
                                            }

                                            if (specific == SheduleSettings.specifics[2] && groupsubject.LaboratoryHour > 0)
                                            {
                                                item.IsValueValid = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (item.ItemTwo.Subject != null && item.N_DIndex == -1)
                            {
                                item.IsValueValid = false;
                                foreach (var groupvalue in tempAllGroupsAndSubjects)
                                {
                                    if (item.ItemTwo.Group.Exists(g => g.CodeOfGroup == groupvalue.Group.CodeOfGroup))
                                    {
                                        foreach (var groupsubject in groupvalue.InformationAboutSubjects)
                                        {
                                            if (specific == SheduleSettings.specifics[0] && groupsubject.LectureHour > 0)
                                            {
                                                item.IsValueValid = true;
                                            }

                                            if (specific == SheduleSettings.specifics[1] && groupsubject.ExerciseHour > 0)
                                            {
                                                item.IsValueValid = true;
                                            }

                                            if (specific == SheduleSettings.specifics[2] && groupsubject.LaboratoryHour > 0)
                                            {
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
            if (ch == 0)
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
                    MessageBox.Show("Save", "Сохранение расписания в базу данных");
                }
            }
            else
            {
                MessageBox.Show("Для выполнения данного действия перейдите в категорию расписания для групп", "Внимание");
            }
        }

        public void ReadFromClasses()
        {
            if (ch == 0)
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
            else
            {
                MessageBox.Show("Для выполнения данного действия перейдите в категорию расписания для групп", "Внимание");
            }
        }

        public void ScheduleGeneration()
        {
            if (ch == 0)
            {
                // ----------------Тестирование генерации------------------------------
                //Stopwatch mywatch = new Stopwatch();
                var list = new List<Lesson>(PrepareListLessons(SheduleSettings.specifics, ClassClassrooms, AllGroupsAndSubjects, AllTeachersAndSubjects));
                var unionList = new List<Lesson>(UnionPair(list));
                //mywatch.Start();

                var solver = new Solver();

                Plan.DaysPerWeek = 6;
                Plan.HoursPerDay = 6;
                FitnessFunctions.gas = AllGroupsAndSubjects.ToArray();
                FitnessFunctions.tas = AllTeachersAndSubjects.ToArray();
                FitnessFunctions.Specifics = SheduleSettings.specifics;

                //solver.FitnessFunctions.Add(FitnessFunctions.Windows);
                solver.FitnessFunctions.Add(FitnessFunctions.CountPairTeachers);
                solver.FitnessFunctions.Add(FitnessFunctions.CountLecturePairTeachers);
                solver.FitnessFunctions.Add(FitnessFunctions.CountPairGroups);
                solver.FitnessFunctions.Add(FitnessFunctions.CountLecturePairGroups);
                //solver.FitnessFunctions.Add(FitnessFunctions.CountMoveFromFiveHousingToOtherAndConversely);

                var res = solver.Solve(unionList);
                Representation(res);
                //mywatch.Stop();
                //Console.WriteLine("Работа алгоритма время в секундах: " + mywatch.ElapsedMilliseconds / 1000);
                //Console.WriteLine(res);

                //----------------------------------
            }
            else
            {
                MessageBox.Show("Для выполнения данного действия перейдите в категорию расписания для групп", "Внимание");
            }
        }

        public void Representation(Plan plan)
        {
            for (int i = 0; i < Filtered.Count; i++)
            {
                for (int j = 0; j < Filtered[i].Count; j++)
                {
                   // Filtered[i][j].Item = null;
                    //Filtered[i][j].ItemTwo = null;
                    Filtered[i][j].State = 0;
                    Filtered[i][j].N_DIndex = 0;
                    for (int day = 0; day < Plan.DaysPerWeek; day++)
                    {
                        for (int hour = 0; hour < Plan.HoursPerDay; hour++)
                        {
                            foreach (var p in plan.HourPlans[day, hour].GroupInform)
                            {
                                if (((int)Filtered[i][j].Info.Day == (day + 1))
                                    && (Filtered[i][j].Info.Pair == (hour + 1))
                                    && (Filtered[i][j].Key ==
                                        (object)ClassGroups.Single(g => g.CodeOfGroup == p.Key)))
                                {

                                    Filtered[i][j].Item = p.Value.dropInfo;
                                    Filtered[i][j].N_DIndex = p.Value.dropInfo.Ndindex;
                                    Filtered[i][j].State = p.Value.dropInfo.Ndindex;
                                }
                            }

                            foreach (var p in plan.HourPlans[day, hour].GroupInformTwo)
                            {
                                if (((int)Filtered[i][j].Info.Day == (day + 1))
                                    && (Filtered[i][j].Info.Pair == (hour + 1))
                                    && (Filtered[i][j].Key == (object)ClassGroups.Single(g => g.CodeOfGroup == p.Key)))
                                {
                                    Filtered[i][j].N_DIndex = p.Value.dropInfoTwo.Ndindex;
                                    Filtered[i][j].State = p.Value.dropInfoTwo.Ndindex;
                                    Filtered[i][j].ItemTwo = p.Value.dropInfoTwo;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ScheduleCheck()
        {
            if (ch == 0)
            {
                var context = new ChooseRulesVM(CreateListOfRules(AllGroupsAndSubjects));
                var winchooserules = new ChooseRules()
                {
                    DataContext = context
                };
                winchooserules.ShowDialog();

                if (winchooserules.DialogResult == true)
                {
                    var val = new RuleEngine(Filtered);
                    foreach (var rule in context.SelectedRules)
                    {
                        val.AddRule(rule);
                    }
                    val.ApplyRules();

                    var contextErrors = new ShowErrorsVM(val.CreateStringErrors());
                    var winshowerrrors = new ShowErrors()
                    {
                        DataContext = contextErrors
                    };
                    winshowerrrors.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Для выполнения данного действия перейдите в категорию расписания для групп", "Внимание");
            }
        }

        private List<IRule> CreateListOfRules(ObservableCollection<GroupsAndSubjects> allGroupsAndSubjects)
        {
            List<IRule> rules = new List<IRule>();
            rules.Add(new NoOverlay());
            rules.Add(new CountPair());
            rules.Add(new Windows());
            rules.Add(new PlanCompleted(AllGroupsAndSubjects));

            return rules;
        }

        public ObservableCollection<TeachersAndSubjects> AllTeachersAndSubjects;
        public ObservableCollection<GroupsAndSubjects> AllGroupsAndSubjects;

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
