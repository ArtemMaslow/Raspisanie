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

        private readonly INotifyCommand selectN_DCommand;

        private INotifyingValue<RowColumnIndex?> index;
        private INotifyingValue<int> departmentIndex;
        private int ch = 0;

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
                    clearItem = new DropInformation { Group = Filtered[value.Row][value.Column].Item.Group };
                else if (ch == 1)
                    clearItem = new DropInformation { NumberOfClassroom = Filtered[value.Row][value.Column].Item.NumberOfClassroom };
                else if (ch == -1)
                    clearItem = new DropInformation { Teacher = Filtered[value.Row][value.Column].Item.Teacher };

                Filtered[value.Row][value.Column].Item = clearItem;
            }
        }

        private Group[] filtered;
        private Teacher[] filteredTeacher;
        private ClassRoom[] filteredClassroom;
        private ObservableCollection<ObservableCollection<DropItem>> data;
        private void Numerator_Denominator(int to)
        { 
                for (int i = 0; i < Filtered.Count; i++)
                {
                    for (int j = 0; j < Filtered[i].Count; j++)
                    {
                    Filtered[i][j].N_DIndex = to;
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

            Columns.Clear();
            foreach (var key in Filtered.First().Select(x => x.Key))
                Columns.Add(key.ToString());
            Rows.Clear();
            if (Columns.Count != 0)
                foreach (var row in (Filtered.Select(x => x[0].Info)))
                    Rows.Add(row);
        }

        //public void ExportFromExcel()
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Filter = "Книга Excel (*.xlsx)|*.xlsx";
        //    string fileName = "";
        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        fileName = openFileDialog.FileName;
        //    }

        //    var workbook = new XLWorkbook(fileName);
        //    var worksheet = workbook.Worksheet(1);
        //    for (int i = 0; i < ClassGroups.Length; i++)
        //    {
        //        if ((string)worksheet.Cell(1, 3).Value == ClassGroups[i].NameOfGroup)
        //        {
        //            ch = 0;
        //            continue;
        //        }
        //    }
        //    if (ch == 0)
        //    {

        //    }
        //}

        //public void ExportToExcel()
        //{
        //    Dictionary<string, int> dct;
        //    Type keyType;
        //    if (ch == 0)
        //    {
        //        dct = filtered.Select((x, i) => new { i, x.NameOfGroup })
        //        .ToDictionary(k => k.NameOfGroup, e => e.i);
        //        keyType = typeof(Group);
        //    }
        //    else if (ch == -1)
        //    {
        //        dct = filteredTeacher.Select((x, i) => new { i, x.FIO })
        //        .ToDictionary(k => k.FIO, e => e.i);
        //        keyType = typeof(Teacher);
        //    }
        //    else
        //    {
        //        dct = filteredClassroom.Select((x, i) => new { i, x.NumberOfClassroom })
        //        .ToDictionary(k => k.NumberOfClassroom, e => e.i);
        //        keyType = typeof(ClassRoom);
        //    }

        //    var workbook = new XLWorkbook();
        //    var worksheet = workbook.Worksheets.Add("Лист1");

        //    for (int r = 0; r < maxpair; r++)
        //    {
        //        worksheet.Cell(r + 2, 1).Style.Alignment.TextRotation = 90;
        //        worksheet.Cell(r + 2, 1).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
        //        worksheet.Cell(r + 2, 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(r + 2, 1).Style.Border.TopBorderColor = XLColor.Black;
        //        worksheet.Cell(r + 2, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(r + 2, 1).Style.Border.RightBorderColor = XLColor.Black;
        //        worksheet.Cell(r + 2, 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(r + 2, 1).Style.Border.LeftBorderColor = XLColor.Black;
        //        worksheet.Cell(r + 2, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(r + 2, 1).Style.Border.BottomBorderColor = XLColor.Black;

        //        worksheet.Row(r + 2).Height = 25;

        //        worksheet.Cell(r + 2, 1).Style.Alignment.WrapText = true;
        //        worksheet.Cell(r + 2, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //        worksheet.Cell(r + 2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        worksheet.Cell(r + 2, 1).RichText.FontSize = 20;
        //        worksheet.Cell(r + 2, 1).RichText.FontColor = XLColor.Black;
        //        worksheet.Cell(r + 2, 1).RichText.FontName = "Broadway";
        //        string str = "";
        //        if (r / 6 < 1)
        //        {
        //            str = "Понедельник";
        //            worksheet.Cell(r + 2, 1).Value = str;
        //            worksheet.Range("A2:A7").Column(1).Merge();
        //        }
        //        else
        //        if ((r / 6 < 2) && (r / 6 >= 1))
        //        {
        //            str = "Вторник";
        //            worksheet.Cell(r + 2, 1).Value = str;
        //            worksheet.Range("A8:A13").Column(1).Merge();
        //        }
        //        else
        //        if ((r / 6 < 3) && (r / 6 >= 2))
        //        {
        //            str = "Среда";
        //            worksheet.Cell(r + 2, 1).Value = str;
        //            worksheet.Range("A14:A19").Column(1).Merge();
        //        }
        //        else
        //        if ((r / 6 < 4) && (r / 6 >= 3))
        //        {
        //            str = "Четверг";
        //            worksheet.Cell(r + 2, 1).Value = str;
        //            worksheet.Range("A20:A25").Column(1).Merge();
        //        }
        //        else
        //        if ((r / 6 < 5) && (r / 6 >= 4))
        //        {
        //            str = "Пятница";
        //            worksheet.Cell(r + 2, 1).Value = str;
        //            worksheet.Range("A26:A31").Column(1).Merge();
        //        }
        //        else
        //        {
        //            str = "Суббота";
        //            worksheet.Cell(r + 2, 1).Value = str;
        //            worksheet.Range("A32:A34").Column(1).Merge();
        //        }

        //    }
        //    string[] strPair = { "I 8:30 - 10:05", "II 10:20 - 11:55", "III 12:10 - 13:45", "IV 14:15 - 15:50", "V 16:05 - 17:40", "VI 17:50 - 19:25" };

        //    for (int r = 1; r <= maxpair; r++)
        //    {
        //        worksheet.Cell(r + 1, 2).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
        //        worksheet.Cell(r + 1, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(r + 1, 2).Style.Border.TopBorderColor = XLColor.Black;
        //        worksheet.Cell(r + 1, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(r + 1, 2).Style.Border.RightBorderColor = XLColor.Black;
        //        worksheet.Cell(r + 1, 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(r + 1, 2).Style.Border.LeftBorderColor = XLColor.Black;
        //        worksheet.Cell(r + 1, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(r + 1, 2).Style.Border.BottomBorderColor = XLColor.Black;

        //        worksheet.Cell(r + 1, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //        worksheet.Cell(r + 1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //        worksheet.Row(r + 1).Height = 25;
        //        worksheet.Column(2).Width = 20;
        //        worksheet.Cell(r + 1, 2).Value = strPair[(r - 1) % strPair.Length];
        //    }
        //    if (ch == 0)
        //    {
        //        for (int c = 0; c < filtered.Length; c++)
        //        {
        //            worksheet.Column(3 + c).Width = 40;
        //            worksheet.Cell(1, 3 + c).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
        //            worksheet.Cell(1, 3 + c).Style.Border.TopBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.TopBorderColor = XLColor.Black;
        //            worksheet.Cell(1, 3 + c).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.RightBorderColor = XLColor.Black;
        //            worksheet.Cell(1, 3 + c).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.LeftBorderColor = XLColor.Black;
        //            worksheet.Cell(1, 3 + c).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.BottomBorderColor = XLColor.Black;

        //            worksheet.Cell(1, 3 + c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            worksheet.Cell(1, 3 + c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //            worksheet.Cell(1, 3 + c).Value = filtered[c].NameOfGroup;
        //        }
        //    }
        //    else if (ch == -1)
        //    {
        //        for (int c = 0; c < filteredTeacher.Length; c++)
        //        {
        //            worksheet.Column(3 + c).Width = 40;
        //            worksheet.Cell(1, 3 + c).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
        //            worksheet.Cell(1, 3 + c).Style.Border.TopBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.TopBorderColor = XLColor.Black;
        //            worksheet.Cell(1, 3 + c).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.RightBorderColor = XLColor.Black;
        //            worksheet.Cell(1, 3 + c).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.LeftBorderColor = XLColor.Black;
        //            worksheet.Cell(1, 3 + c).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.BottomBorderColor = XLColor.Black;

        //            worksheet.Cell(1, 3 + c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            worksheet.Cell(1, 3 + c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //            worksheet.Cell(1, 3 + c).Value = filteredTeacher[c].FIO;
        //        }
        //    }
        //    else
        //    {
        //        for (int c = 0; c < filteredClassroom.Length; c++)
        //        {
        //            worksheet.Column(3 + c).Width = 40;
        //            worksheet.Cell(1, 3 + c).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
        //            worksheet.Cell(1, 3 + c).Style.Border.TopBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.TopBorderColor = XLColor.Black;
        //            worksheet.Cell(1, 3 + c).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.RightBorderColor = XLColor.Black;
        //            worksheet.Cell(1, 3 + c).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.LeftBorderColor = XLColor.Black;
        //            worksheet.Cell(1, 3 + c).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        //            worksheet.Cell(1, 3 + c).Style.Border.BottomBorderColor = XLColor.Black;

        //            worksheet.Cell(1, 3 + c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            worksheet.Cell(1, 3 + c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //            worksheet.Cell(1, 3 + c).Value = filteredClassroom[c].NumberOfClassroom;
        //        }
        //    }
        //    var temp = Data.Select(x => x.ToArray()).ToArray();
        //    for (int i = 0; i < temp.Length; i++)
        //    {
        //        for (int j = 0; j < temp[0].Length; j++)
        //        {
        //            if (ch == 0)
        //            {
        //                int cind;
        //                if (temp[i][j].Item.Group != null)
        //                    if (dct.TryGetValue(temp[i][j].Item.Group, out cind))
        //                    {
        //                        Data[i][cind].Item = temp[i][j].Item;
        //                        worksheet.Cell(i + 2, 2 + cind + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //                        worksheet.Cell(i + 2, 2 + cind + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //                        worksheet.Cell(i + 2, 2 + cind + 1).Value = Data[i][cind].Item.NumberOfClassroom + " " + Data[i][cind].Item.Subject + " " + Data[i][cind].Item.Teacher;
        //                    }
        //            }
        //            else if (ch == -1)
        //            {
        //                int cind;
        //                if (temp[i][j].Item.Teacher != null)
        //                    if (dct.TryGetValue(temp[i][j].Item.Teacher, out cind))
        //                    {
        //                        Data[i][cind].Item = temp[i][j].Item;
        //                        worksheet.Cell(i + 2, 2 + cind + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //                        worksheet.Cell(i + 2, 2 + cind + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //                        worksheet.Cell(i + 2, 2 + cind + 1).Value = Data[i][cind].Item.NumberOfClassroom + " " + Data[i][cind].Item.Subject + " " + Data[i][cind].Item.Group;
        //                    }
        //            }
        //            else
        //            {
        //                int cind;
        //                if (temp[i][j].Item.NumberOfClassroom != null)
        //                    if (dct.TryGetValue(temp[i][j].Item.NumberOfClassroom, out cind))
        //                    {
        //                        Data[i][cind].Item = temp[i][j].Item;
        //                        worksheet.Cell(i + 2, 2 + cind + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //                        worksheet.Cell(i + 2, 2 + cind + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //                        worksheet.Cell(i + 2, 2 + cind + 1).Value = Data[i][cind].Item.Subject + " " + Data[i][cind].Item.Group + " " + Data[i][cind].Item.Teacher;
        //                    }
        //            }
        //        }
        //    }
        //    if (IsValidate() == true)
        //    {
        //        SaveFileDialog saveFileDialog = new SaveFileDialog();
        //        saveFileDialog.Filter = "Книга Excel (*.xlsx)|*.xlsx";
        //        string path = "";
        //        if (saveFileDialog.ShowDialog() == true)
        //        {
        //            if (!string.IsNullOrEmpty(saveFileDialog.FileName))
        //            {
        //                path = saveFileDialog.FileName;
        //                workbook.SaveAs(path);
        //                MessageBox.Show("Сохранено");
        //            }
        //        }

        //    }
        //}

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

        public MainVM()
        {
            ClassClassrooms = RequestToDataBase.Instance.ReadClassrooms().ToArray();
            ClassGroups = RequestToDataBase.Instance.ReadGroups().ToArray();
            ClassTeachers = RequestToDataBase.Instance.ReadTeachers().ToArray();
            ClassSubjects = RequestToDataBase.Instance.ReadSubjects().ToArray();
            ClassDepartments = RequestToDataBase.Instance.ReadDepartments().ToArray();

            Filtered = new ObservableCollection<ObservableCollection<DropItem>>();
            for (int i = 0; i < maxpair; i++)
                Filtered.Add(new ObservableCollection<DropItem>());

            openCommand = this.Factory.CommandSync(Open);
            //saveToExcel = this.Factory.CommandSync(ExportToExcel);
            selectCommand = this.Factory.CommandSyncParam<int>(Transform);
            closeWinCommand = this.Factory.CommandSync(Close);
            clearCommand = this.Factory.CommandSync(Clear);
            selectN_DCommand = this.Factory.CommandSyncParam<int>(Numerator_Denominator); 

            index = this.Factory.Backing<RowColumnIndex?>(nameof(Index), null);
            departmentIndex = this.Factory.Backing<int>(nameof(DepartmentIndex), 0);

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
                        if (Filtered[i][j].Item.Teacher != null)
                        {
                            iindex = i;
                            jindex = j;
                            continue;
                        }
                    }
                }
                for (int j = 0; j < Columns.Count; j++)
                {
                    if ((Filtered[iindex][jindex].Item.Teacher == Filtered[iindex][j].Item.Teacher) && (Filtered[iindex][jindex].Item.NumberOfClassroom != Filtered[iindex][j].Item.NumberOfClassroom))
                    {
                        MessageBox.Show($"{Filtered[iindex][jindex].Item.Teacher} не может вести занятия в аудитории {Filtered[iindex][jindex].Item.NumberOfClassroom} и { Filtered[iindex][j].Item.NumberOfClassroom} одновременно у групп {Filtered[iindex][jindex].Item.Group} и {Filtered[iindex][j].Item.Group}");
                        return false;
                    }
                    if ((Filtered[iindex][jindex].Item.Teacher == Filtered[iindex][j].Item.Teacher) && (Filtered[iindex][jindex].Item.Subject != Filtered[iindex][j].Item.Subject))
                    {
                        MessageBox.Show($"{Filtered[iindex][jindex].Item.Teacher} не может вести предметы {Filtered[iindex][jindex].Item.Subject} и { Filtered[iindex][j].Item.Subject} одновременно в группах {Filtered[iindex][jindex].Item.Group} и {Filtered[iindex][j].Item.Group}");
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

        public ObservableCollection<ObservableCollection<DropItem>> Filtered { get; }
        public ObservableCollection<string> Columns { get; }
        public ObservableCollection<PairInfo> Rows { get; }
        public Group[] ClassGroups { get; }
        public Subject[] ClassSubjects { get; }
        public Teacher[] ClassTeachers { get; }
        public ClassRoom[] ClassClassrooms { get; }
        public Department[] ClassDepartments { get; }

        public RowColumnIndex? Index { get { return index.Value; } set { index.Value = value; } }
        public int DepartmentIndex { get { return departmentIndex.Value; } set { departmentIndex.Value = value; Filter(); } }
        
        public ICommand CloseWinCommand => closeWinCommand;
        public ICommand OpenCommand => openCommand;
        public ICommand SaveToExcel => saveToExcel;
        public ICommand SelectCommand => selectCommand;
        public ICommand ClearCommand => clearCommand;
        public ICommand SelectN_DCommand => selectN_DCommand;
    }

}
