using Raspisanie.Models;
using ClosedXML.Excel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Linq;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Collections.Generic;
using Microsoft.Win32;
using Gu.Wpf.DataGrid2D;
using System.Windows.Controls;

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

        private INotifyingValue<RowColumnIndex?> index;
        private INotifyingValue<int> departmentIndex;
        private INotifyingValue<bool> generalShedule;

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
            Dictionary<object, int> dct;
            Type keyType;
            if (to == 0)
            {
                //x - группа, i - индекс
                //k - селекторор ключа(название группы), i - селектор эелемента(номер столбца)
                dct = ClassGroups.Select((x, i) => new { i, x })
                .ToDictionary(k => (object)(k.x), e => e.i);//Каждому заголовку столбца ставится в соответствие его индекс.
                keyType = typeof(Group);
            }
            else if (to == -1)
            {
                dct = ClassTeachers.Select((x, i) => new { i, x })
                .ToDictionary(k => (object)k.x, e => e.i);
                keyType = typeof(Teacher);
            }
            else
            {
                dct = ClassClassrooms.Select((x, i) => new { i, x })
                .ToDictionary(k => (object)k.x, e => e.i);
                keyType = typeof(ClassRoom);
            }
            var temp = data.Select(x => x.ToArray()).ToArray();
            data.Clear();
            Columns.Clear();
            foreach (var key in dct.Keys)
                Columns.Add(key.ToString());
            foreach (var r in Rows)
            {
                var row = new ObservableCollection<DropItem>();
                foreach (var key in dct.Keys)
                {
                    DropItem item = new DropItem(key, keyType, r);
                    item.N_DIndex = itemstate;
                    if (to == 0)
                    {
                        item.Item.Group = (Group)key;
                        item.ItemTwo.Group = (Group)key;
                    }
                    else if (to == -1)
                    {
                        item.Item.Teacher = (Teacher)key;
                        item.ItemTwo.Teacher = (Teacher)key;
                    }
                    else
                    {
                        item.Item.NumberOfClassroom = (ClassRoom)key;
                        item.ItemTwo.NumberOfClassroom = (ClassRoom)key;
                    }

                    row.Add(item);
                }
                data.Add(row);
            }
            for (int i = 0; i < temp.Length; i++)
            {
                for (int j = 0; j < temp[0].Length; j++)
                {
                    if (to == 0)
                    {
                        int cind;
                        if (temp[i][j].Item.Group != null)
                        {
                            if (dct.TryGetValue(temp[i][j].Item.Group, out cind))
                            {
                                data[i][cind].Item = temp[i][j].Item;
                                data[i][cind].State = temp[i][j].State;
                            }
                        }
                        if (temp[i][j].ItemTwo.Group != null)
                        {
                            if (dct.TryGetValue(temp[i][j].ItemTwo.Group, out cind))
                            {
                                data[i][cind].ItemTwo = temp[i][j].ItemTwo;
                                data[i][cind].State = temp[i][j].State;
                            }
                        }
                    }
                    else if (to == -1)
                    {
                        int cind;
                        if (temp[i][j].Item.Teacher != null)
                        {
                            if (dct.TryGetValue(temp[i][j].Item.Teacher, out cind))
                            {
                                data[i][cind].Item = temp[i][j].Item;
                                data[i][cind].State = temp[i][j].State;
                            }
                        }
                        if (temp[i][j].ItemTwo.Teacher != null)
                        {
                            if (dct.TryGetValue(temp[i][j].ItemTwo.Teacher, out cind))
                            {
                                data[i][cind].ItemTwo = temp[i][j].ItemTwo;
                                data[i][cind].State = temp[i][j].State;
                            }
                        }
                    }
                    else
                    {
                        int cind;
                        if (temp[i][j].Item.NumberOfClassroom != null)
                        {
                            if (dct.TryGetValue(temp[i][j].Item.NumberOfClassroom, out cind))
                            {
                                data[i][cind].Item = temp[i][j].Item;
                                data[i][cind].State = temp[i][j].State;
                            }
                        }
                        if (temp[i][j].ItemTwo.NumberOfClassroom != null)
                        {
                            if (dct.TryGetValue(temp[i][j].ItemTwo.NumberOfClassroom, out cind))
                            {
                                data[i][cind].ItemTwo = temp[i][j].ItemTwo;
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
                        f = x.Where(di => di.Item.Group.Department.CodeOfDepartment
                                        == ClassDepartments[DepartmentIndex].CodeOfDepartment);
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
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Лист1");

            for (int r = 1; r <= SheduleSettings.WeekDayMaxCount; r++)
            {
                worksheet.Cell(12 * r - 10, 1).Style.Alignment.TextRotation = 90;
                worksheet.Cell(12 * r - 10, 1).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
                worksheet.Cell(12 * r - 10, 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(12 * r - 10, 1).Style.Border.TopBorderColor = XLColor.Black;
                worksheet.Cell(12 * r - 10, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(12 * r - 10, 1).Style.Border.RightBorderColor = XLColor.Black;
                worksheet.Cell(12 * r - 10, 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(12 * r - 10, 1).Style.Border.LeftBorderColor = XLColor.Black;
                worksheet.Cell(12 * r - 10, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(12 * r - 10, 1).Style.Border.BottomBorderColor = XLColor.Black;

                worksheet.Row(12 * r - 10).Height = 25;

                worksheet.Cell(12 * r - 10, 1).Style.Alignment.WrapText = true;
                worksheet.Cell(12 * r - 10, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(12 * r - 10, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(12 * r - 10, 1).RichText.FontSize = 20;
                worksheet.Cell(12 * r - 10, 1).RichText.FontColor = XLColor.Black;
                worksheet.Cell(12 * r - 10, 1).RichText.FontName = "Broadway";
                string str = "";
                if (r == 1)
                {
                    str = "Понедельник";
                    worksheet.Cell(12 * r - 10, 1).Value = str;
                }
                else if (r == 2)
                {
                    str = "Вторник";
                    worksheet.Cell(12 * r - 10, 1).Value = str;
                }
                else if (r == 3)
                {
                    str = "Среда";
                    worksheet.Cell(12 * r - 10, 1).Value = str;
                }
                else if (r == 4)
                {
                    str = "Четверг";
                    worksheet.Cell(12 * r - 10, 1).Value = str;
                }
                else if (r == 5)
                {
                    str = "Пятница";
                    worksheet.Cell(12 * r - 10, 1).Value = str;
                }
                else
                {
                    str = "Суббота";
                    worksheet.Cell(12 * r - 10, 1).Value = str;
                }
                if (r < SheduleSettings.WeekDayMaxCount)
                    worksheet.Range(12 * r - 10, 1, 12 * r - 10 + 11, 1).Merge();
                else
                    worksheet.Range(12 * r - 10, 1, 12 * r - 10 + 5, 1).Merge();

            }
            string[] strPair = { "I 8:30 - 10:05", "II 10:20 - 11:55", "III 12:10 - 13:45", "IV 14:15 - 15:50", "V 16:05 - 17:40", "VI 17:50 - 19:25" };

            for (int r = 1; r <= maxpair; r++)
            {
                worksheet.Cell(2 * r, 2).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
                worksheet.Cell(2 * r, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2 * r, 2).Style.Border.TopBorderColor = XLColor.Black;
                worksheet.Cell(2 * r, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2 * r, 2).Style.Border.RightBorderColor = XLColor.Black;

                worksheet.Cell(2 * r + 1, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2 * r + 1, 2).Style.Border.RightBorderColor = XLColor.Black;

                worksheet.Cell(2 * r, 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2 * r, 2).Style.Border.LeftBorderColor = XLColor.Black;

                worksheet.Cell(2 * r + 1, 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2 * r + 1, 2).Style.Border.LeftBorderColor = XLColor.Black;

                worksheet.Cell(2 * r, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2 * r, 2).Style.Border.BottomBorderColor = XLColor.Black;

                worksheet.Cell(2 * r + 1, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2 * r + 1, 2).Style.Border.BottomBorderColor = XLColor.Black;

                worksheet.Cell(2 * r, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(2 * r, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Row(2 * r).Height = 20;
                worksheet.Column(2).Width = 20;
                worksheet.Cell(2 * r, 2).Value = strPair[(r - 1) % strPair.Length];
                worksheet.Range(2 * r, 2, 2 * r + 1, 2).Merge();
            }

            for (int c = 0; c < Columns.Count; c++)
            {
                worksheet.Column(3 + c).Width = 40;
                worksheet.Cell(1, 3 + c).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
                worksheet.Cell(1, 3 + c).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(1, 3 + c).Style.Border.TopBorderColor = XLColor.Black;
                worksheet.Cell(1, 3 + c).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(1, 3 + c).Style.Border.RightBorderColor = XLColor.Black;
                worksheet.Cell(1, 3 + c).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(1, 3 + c).Style.Border.LeftBorderColor = XLColor.Black;
                worksheet.Cell(1, 3 + c).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(1, 3 + c).Style.Border.BottomBorderColor = XLColor.Black;

                worksheet.Cell(1, 3 + c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(1, 3 + c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(1, 3 + c).Value = Columns[c];
            }


            for (int i = 0; i < Filtered.Count; i++)
            {
                for (int j = 0; j < Filtered[i].Count; j++)
                {
                    if (ch == 0)
                    {

                        if (Filtered[i][j].Item.Group != null)
                        {
                            worksheet.Cell(i + 2, 3 + j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            worksheet.Cell(i + 2, 3 + j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            if (Filtered[i][j].State == 0)
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " +Filtered[i][j].Item.NumberOfClassroom + " " + Filtered[i][j].Item.Teacher;
                                worksheet.Range(2 * i + 2, 3 + j, 2 * i + 3, 3 + j).Merge();
                            }
                            else
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " + Filtered[i][j].Item.NumberOfClassroom + " " + Filtered[i][j].Item.Teacher;
                                worksheet.Cell(2 * i + 3, 3 + j).Value = Filtered[i][j].ItemTwo.Subject + " " + Filtered[i][j].ItemTwo.Specifics + " " + Filtered[i][j].ItemTwo.NumberOfClassroom + " " + Filtered[i][j].ItemTwo.Teacher;
                            }
                        }
                    }
                    else if (ch == -1)
                    {
                        if (Filtered[i][j].Item.Teacher != null)
                        {
                            worksheet.Cell(i + 2, 3 + j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            worksheet.Cell(i + 2, 3 + j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            if (Filtered[i][j].State == 0)
                            {
                                worksheet.Cell(i + 2, 3 + j).Value = Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " + Filtered[i][j].Item.NumberOfClassroom + " " + Filtered[i][j].Item.Group;
                                worksheet.Range(2 * i + 2, 3 + j, 2 * i + 3, 3 + j).Merge();
                            }
                            else
                            {
                                worksheet.Cell(i + 2, 3 + j).Value = Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " + Filtered[i][j].Item.NumberOfClassroom + " " + Filtered[i][j].Item.Group;
                                worksheet.Cell(i + 2, 3 + j).Value = Filtered[i][j].ItemTwo.Subject + " " + Filtered[i][j].ItemTwo.Specifics + " " + Filtered[i][j].ItemTwo.NumberOfClassroom + " " + Filtered[i][j].ItemTwo.Group;
                            }
                        }
                    }
                    else
                    {
                        if (Filtered[i][j].Item.NumberOfClassroom != null)
                        {
                            worksheet.Cell(i + 2, 3 + j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            worksheet.Cell(i + 2, 3 + j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            if (Filtered[i][j].State == 0)
                            {
                                worksheet.Cell(i + 2, 3 + j).Value = Filtered[i][j].Item.Teacher + " " + Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " + Filtered[i][j].Item.Group;
                                worksheet.Range(2 * i + 2, 3 + j, 2 * i + 3, 3 + j).Merge();
                            }
                            else
                            {
                                worksheet.Cell(i + 2, 3 + j).Value = Filtered[i][j].Item.Teacher + " " + Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " + Filtered[i][j].Item.Group;
                                worksheet.Cell(i + 2, 3 + j).Value = Filtered[i][j].ItemTwo.Teacher + " " + Filtered[i][j].ItemTwo.Subject + " " + Filtered[i][j].ItemTwo.Specifics + " " + Filtered[i][j].ItemTwo.Group;
                            }
                        }
                    }
                }
            }
            if (IsValidate())
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Книга Excel (*.xlsx)|*.xlsx";
                string path = "";
                if (saveFileDialog.ShowDialog() == true)
                {
                    if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                    {
                        path = saveFileDialog.FileName;
                        workbook.SaveAs(path);
                        MessageBox.Show("Сохранено");
                    }
                }

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
                        data[j].Add(new DropItem(ClassGroups[i].NameOfGroup, typeof(Group), pair)
                        {
                            Item = new DropInformation { Group = ClassGroups[i] },
                            ItemTwo = new DropInformation { Group = ClassGroups[i] }
                        });
                        j++;
                    }
                }
            }

            Filter();
        }
        private string[] specifics = { "Лек.", "Упр.", "Лаб." };
        
        public MainVM()
        {
            ClassClassrooms = RequestToDataBase.Instance.ReadClassrooms().ToArray();
            ClassGroups = RequestToDataBase.Instance.ReadGroups().ToArray();
            ClassTeachers = RequestToDataBase.Instance.ReadTeachers().ToArray();
            ClassSubjects = RequestToDataBase.Instance.ReadSubjects().ToArray();
            ClassDepartments = RequestToDataBase.Instance.ReadDepartments().ToArray();
            Specifics = specifics;

            Elem = RequestToDataBase.Instance.ReadClasses().ToArray();
            Console.WriteLine(Elem[0].Item.Group.NameOfGroup + " " + Elem[0].Item.Teacher.FIO + " " + Elem[0].Info.Day + " " + Elem[0].Info.Pair+" t: "+Elem[0].KeyType);

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

            index = this.Factory.Backing<RowColumnIndex?>(nameof(Index), null);
            departmentIndex = this.Factory.Backing<int>(nameof(DepartmentIndex), 0);
            generalShedule = this.Factory.Backing<bool>(nameof(GeneralShedule), true);

            Columns = new ObservableCollection<string>();
            Rows = new ObservableCollection<PairInfo>();
            Init();
        }

        public bool IsValidate()
        {
            if (ch == 0)
            {
                int iindex = 0;
                int jindex = 0;
                for (int i = 0; i < maxpair; i++)
                {
                    for (int j = 0; j < Columns.Count; j++)
                    {
                        if (data[i][j].Item.Teacher != null)
                        {
                            iindex = i;
                            jindex = j;
                            continue;
                        }
                    }
                }
                for (int j = 0; j < Columns.Count; j++)
                {
                    if ((data[iindex][jindex].Item.Teacher == data[iindex][j].Item.Teacher) && (data[iindex][jindex].Item.NumberOfClassroom != data[iindex][j].Item.NumberOfClassroom))
                    {
                        MessageBox.Show($"{data[iindex][jindex].Item.Teacher} не может вести занятия в аудитории {data[iindex][jindex].Item.NumberOfClassroom} и { data[iindex][j].Item.NumberOfClassroom} одновременно у групп {data[iindex][jindex].Item.Group} и {data[iindex][j].Item.Group}");
                        return false;
                    }
                    if ((data[iindex][jindex].Item.Teacher == data[iindex][j].Item.Teacher) && (data[iindex][jindex].Item.Subject != data[iindex][j].Item.Subject))
                    {
                        MessageBox.Show($"{data[iindex][jindex].Item.Teacher} не может вести предметы {data[iindex][jindex].Item.Subject} и { data[iindex][j].Item.Subject} одновременно в группах {data[iindex][jindex].Item.Group} и {data[iindex][j].Item.Group}");
                        return false;
                    }
                }
                return true;
            }
            if (ch == -1)
            {
                return true;
            }
            if (ch == 1)
            {
                return true;
            }
            return false;
        }

        public void SaveSheduleToDataBase()
        {
            GeneralShedule = true;
            Filter();
            Transform(0);
            RequestToDataBase.Instance.clearClasses();
            Console.Clear();
            for (int i = 0; i < Filtered.Count; i++)
            {
                for (int j = 0; j < Filtered[i].Count; j++)
                {
                    if ((Filtered[i][j].Item.Group != null) && (Filtered[i][j].Item.NumberOfClassroom != null) && (Filtered[i][j].Item.Specifics != null) && (Filtered[i][j].Item.Subject != null) && (Filtered[i][j].Item.Teacher != null)) 
                    {
                        Console.WriteLine("Day:" + Filtered[i][j].Info.Day + " pair:" + Filtered[i][j].Info.Pair+" Key:"+Filtered[i][j].Key+" KeyType: "+Filtered[i][j].KeyType+" State:"+Filtered[i][j].State+" ND:"+Filtered[i][j].Item.Ndindex + " NDNUM " + Filtered[i][j].N_DIndex);                 
                        RequestToDataBase.Instance.requestInsertIntoClassesItemOne(Filtered[i][j]);
                    }

                    if ((Filtered[i][j].ItemTwo.Group != null) && (Filtered[i][j].ItemTwo.NumberOfClassroom != null) && (Filtered[i][j].ItemTwo.Specifics != null) && (Filtered[i][j].ItemTwo.Subject != null) && (Filtered[i][j].ItemTwo.Teacher != null))
                    {                       
                        Console.WriteLine("Day:" + Filtered[i][j].Info.Day + " pair:" + Filtered[i][j].Info.Pair + " Key:" + Filtered[i][j].Key + " KeyType: " + Filtered[i][j].KeyType + " State:" + Filtered[i][j].State + " ND:" + Filtered[i][j].ItemTwo.Ndindex + "NDNUM "+Filtered[i][j].N_DIndex);                       
                        RequestToDataBase.Instance.requestInsertIntoClassesItemTwo(Filtered[i][j]);
                    }
                }
            }
        }

        public void ReadFromClasses()
        {
            GeneralShedule = true;
            Filter();
            Transform(0);
            for (int k = 0; k < Elem.Length; k++)
            {
                for (int i = 0; i < Filtered.Count; i++)
                {
                    for (int j = 0; j < Filtered[i].Count; j++)
                    {
                        if ((Elem[k].Info.Day == Filtered[i][j].Info.Day) && (Elem[k].Info.Pair == Filtered[i][j].Info.Pair) && /*(Elem[k].Key == Filtered[i][j].Key) &&*/ (Elem[k].KeyType == Filtered[i][j].KeyType))
                        {
                            data[i][j] = Elem[k];
                        }
                    }
                }
            }
        }

        public ObservableCollection<ObservableCollection<DropItem>> Filtered { get; }
        public ObservableCollection<string> Columns { get; }
        public ObservableCollection<PairInfo> Rows { get; }
        public Group[] ClassGroups { get; }
        public Subject[] ClassSubjects { get; }
        public Teacher[] ClassTeachers { get; }
        public ClassRoom[] ClassClassrooms { get; }
        public Department[] ClassDepartments { get; }
        public string[] Specifics { get; }
        
        public DropItem[] Elem { get; }

        public RowColumnIndex? Index { get { return index.Value; } set { index.Value = value; } }
        public int DepartmentIndex { get { return departmentIndex.Value; } set { departmentIndex.Value = value; Filter(); } }
        public bool GeneralShedule { get { return generalShedule.Value; } set { generalShedule.Value = value; Filter(); } }
        
        public ICommand CloseWinCommand => closeWinCommand;
        public ICommand OpenCommand => openCommand;
        public ICommand SaveToExcel => saveToExcel;
        public ICommand SelectCommand => selectCommand;
        public ICommand ClearCommand => clearCommand;
        public ICommand SelectN_DCommand => selectN_DCommand;
        public ICommand SaveToDataBase => saveToDataBase;
        public ICommand ReadClasses => readClasses;
    }

}
