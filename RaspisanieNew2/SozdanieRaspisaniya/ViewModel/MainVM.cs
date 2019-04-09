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
using System.Reflection;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Diagnostics;
using Microsoft.FSharp.Core;
using ModelLibrary;
using SozdanieRaspisaniya.ViewModel;
using static SozdanieRaspisaniya.ViewModel.GenerateSchedule;

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
                                //Console.Write("Compare...Group....");
                                //Console.WriteLine(k.Equals(temp[i][j].Item.Group));
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
                            //foreach (var k in dct.Keys)
                            //{
                            //    Console.Write("Compare...Teachers...");
                            //    Console.WriteLine(k.Equals(temp[i][j].Item.Group));
                            //}
                            if (dct.TryGetValue(temp[i][j].Item.Teacher, out cind))
                            {
                                //data[i][cind].Item = temp[i][j].Item;
                                //data[i][cind].State = temp[i][j].State;
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
                                //data[i][cind].ItemTwo = temp[i][j].ItemTwo;
                                //data[i][cind].State = temp[i][j].State;
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
                            //foreach (var k in dct.Keys)
                            //{
                            //    Console.Write("Compare...Rooms...");
                            //    Console.WriteLine(k.Equals(temp[i][j].Item.NumberOfClassroom));
                            //}
                            if (dct.TryGetValue(temp[i][j].Item.NumberOfClassroom, out cind))
                            {
                                //data[i][cind].Item = temp[i][j].Item;
                                //data[i][cind].State = temp[i][j].State;
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
                                //data[i][cind].ItemTwo = temp[i][j].ItemTwo;
                                //data[i][cind].State = temp[i][j].State;
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
                    worksheet.Cell(2 * i + 2, 3 + j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Cell(2 * i + 2, 3 + j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(2 * i + 3, 3 + j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    if (ch == 0)
                    {

                        if (Filtered[i][j].Item.Group != null)
                        {
                            if (Filtered[i][j].State == 0)
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " + Filtered[i][j].Item.NumberOfClassroom + " " + Filtered[i][j].Item.Teacher;
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
                            if (Filtered[i][j].State == 0)
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " + Filtered[i][j].Item.NumberOfClassroom + " " + string.Join("+", Filtered[i][j].Item.Group.Select(gr => gr.NameOfGroup));
                                worksheet.Range(2 * i + 2, 3 + j, 2 * i + 3, 3 + j).Merge();
                            }
                            else
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " + Filtered[i][j].Item.NumberOfClassroom + " " + string.Join("+", Filtered[i][j].Item.Group.Select(gr => gr.NameOfGroup));
                                worksheet.Cell(2 * i + 3, 3 + j).Value = Filtered[i][j].ItemTwo.Subject + " " + Filtered[i][j].ItemTwo.Specifics + " " + Filtered[i][j].ItemTwo.NumberOfClassroom + " " + string.Join("+", Filtered[i][j].ItemTwo.Group.Select(gr => gr.NameOfGroup));
                            }
                        }
                    }
                    else
                    {
                        if (Filtered[i][j].Item.NumberOfClassroom != null)
                        {
                            if (Filtered[i][j].State == 0)
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = Filtered[i][j].Item.Teacher + " " + Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " + string.Join("+", Filtered[i][j].Item.Group.Select(gr => gr.NameOfGroup));
                                worksheet.Range(2 * i + 2, 3 + j, 2 * i + 3, 3 + j).Merge();
                            }
                            else
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = Filtered[i][j].Item.Teacher + " " + Filtered[i][j].Item.Subject + " " + Filtered[i][j].Item.Specifics + " " + string.Join("+", Filtered[i][j].Item.Group.Select(gr => gr.NameOfGroup));
                                worksheet.Cell(2 * i + 3, 3 + j).Value = Filtered[i][j].ItemTwo.Teacher + " " + Filtered[i][j].ItemTwo.Subject + " " + Filtered[i][j].ItemTwo.Specifics + " " + string.Join("+", Filtered[i][j].ItemTwo.Group.Select(gr => gr.NameOfGroup));
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

        public void SendExcelFile()
        {
            Transform(-1);
            string mailLogin = XMLConfig.ReadMailLoginValue(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "XMLConfig.xml");
            string mailPassword = XMLConfig.ReadMailPasswordValue(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "XMLConfig.xml");
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(mailLogin, mailPassword);
            MailAddress from = new MailAddress(mailLogin);

            for (int c = 0; c < Columns.Count; c++)
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

                worksheet.Column(3).Width = 40;
                worksheet.Cell(1, 3).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
                worksheet.Cell(1, 3).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(1, 3).Style.Border.TopBorderColor = XLColor.Black;
                worksheet.Cell(1, 3).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(1, 3).Style.Border.RightBorderColor = XLColor.Black;
                worksheet.Cell(1, 3).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(1, 3).Style.Border.LeftBorderColor = XLColor.Black;
                worksheet.Cell(1, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(1, 3).Style.Border.BottomBorderColor = XLColor.Black;

                worksheet.Cell(1, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(1, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(1, 3).Value = Columns[c];
                // Console.WriteLine(Filtered.Count);
                // Console.WriteLine(Filtered[1].Count);
                for (int i = 0; i < Filtered.Count; i++)
                {
                    if (Filtered[i][c].Item.Teacher != null || Filtered[i][c].ItemTwo.Teacher != null)
                    {
                        worksheet.Cell(i + 2, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        worksheet.Cell(i + 2, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        if (Filtered[i][c].State == 0)
                        {
                            worksheet.Cell(2 * i + 2, 3).Value = Filtered[i][c].Item.Subject + " " + Filtered[i][c].Item.Specifics + " " + Filtered[i][c].Item.NumberOfClassroom + " " + Filtered[i][c].Item.Group;
                            worksheet.Range(2 * i + 2, 3, 2 * i + 3, 3).Merge();
                        }
                        else
                        {
                            worksheet.Cell(2 * i + 2, 3).Value = Filtered[i][c].Item.Subject + " " + Filtered[i][c].Item.Specifics + " " + Filtered[i][c].Item.NumberOfClassroom + " " + Filtered[i][c].Item.Group;
                            worksheet.Cell(2 * i + 3, 3).Value = Filtered[i][c].ItemTwo.Subject + " " + Filtered[i][c].ItemTwo.Specifics + " " + Filtered[i][c].ItemTwo.NumberOfClassroom + " " + Filtered[i][c].ItemTwo.Group;
                        }
                    }

                }

                string fileName = "Расписание" + c + ".xlsx";
                // Console.WriteLine(Filtered[0][c].Item.Teacher.Mail);
                workbook.SaveAs(fileName);

                MailAddress to = new MailAddress(Filtered[0][c].Item.Teacher.Mail);
                MailMessage m = new MailMessage(from, to);
                m.Subject = "Тест";
                m.Body = "Письмо-тест работы отправки сообщения";
                m.Attachments.Add(new Attachment(fileName));
                smtp.Send(m);
            }
            MessageBox.Show("Расписание отправленно преподавателям");
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

        private string[] specifics = { "лекц.", "упр.", "лаб." };

        public MainVM(int term)
        {
            ClassClassrooms = RequestToDataBase.Instance.ReadClassrooms().ToArray();
            ClassGroups = RequestToDataBase.Instance.ReadGroups(term).ToArray();
            ClassTeachers = RequestToDataBase.Instance.ReadTeachers().ToArray();
            ClassSubjects = RequestToDataBase.Instance.ReadSubjects().ToArray();
            ClassDepartments = RequestToDataBase.Instance.ReadDepartments().ToArray();
            Specifics = specifics;

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

            // ----------------Тестирование генерации------------------------------

            var list = PrepareListLessons(specifics, ClassClassrooms, AllGroupsAndSubjects, AllTeachersAndSubjects);
            Console.WriteLine(list.Count);

            var solver = new Solver();

            Plan.DaysPerWeek = 2;
            Plan.HoursPerDay = 6;

            solver.FitnessFunctions.Add(FitnessFunctions.Windows);
            var res = solver.Solve(list);

            Console.WriteLine(res);
            
            //----------------------------------

            openCommand = this.Factory.CommandSync(Open);
            saveToExcel = this.Factory.CommandSync(ExportToExcel);
            selectCommand = this.Factory.CommandSyncParam<int>(Transform);
            closeWinCommand = this.Factory.CommandSync(Close);
            saveToDataBase = this.Factory.CommandSync(SaveSheduleToDataBase);
            readClasses = this.Factory.CommandSync(ReadFromClasses);
            clearCommand = this.Factory.CommandSync(Clear);
            selectN_DCommand = this.Factory.CommandSyncParam<int>(Numerator_Denominator);
            exelFileToTeacher = this.Factory.CommandSync(SendExcelFile);

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
            var listLessons = new List<Lesson>();

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
                            var tempListTeacher = new List<Teacher>();
                            foreach(var valueTeacher in AllTeachersAndSubjects)
                            {
                                foreach (var valueSubject in valueTeacher.SubjectList)
                                {
                                    if(valueGroupAndSubjects.Subject == valueSubject && valueTeacher.Teacher.IsReadLecture == true)
                                    {
                                        tempListTeacher.Add(valueTeacher.Teacher);
                                    }
                                }
                            }

                            var tempListClassrooms = new List<ClassRoom>();
                            foreach (var valueClassroms in ClassClassrooms)
                            {
                                if(valueClassroms.Specific.Equals(Specifics[0]))
                                {
                                    tempListClassrooms.Add(valueClassroms);
                                }
                            }

                            if (tempListTeacher.Count > 1)
                            {
                                listLessons.Add(new Lesson(new DropInformation(group, tempListTeacher.ElementAt(rnd.Next(tempListTeacher.Count)), valueGroupAndSubjects.Subject, Specifics[0], tempListClassrooms.ElementAt(rnd.Next(tempListClassrooms.Count)))));
                                valueGroupAndSubjects.LectureHour -= 2;
                            }
                            else
                            {
                                listLessons.Add(new Lesson(new DropInformation(group, tempListTeacher.ElementAt(0), valueGroupAndSubjects.Subject, Specifics[0], tempListClassrooms.ElementAt(rnd.Next(tempListClassrooms.Count)))));
                                valueGroupAndSubjects.LectureHour -= 2;
                            }
                           
                        }
                        else if(valueGroupAndSubjects.ExerciseHour != 0)
                        {
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

                            var tempListClassrooms = new List<ClassRoom>();
                            foreach (var valueClassroms in ClassClassrooms)
                            {
                                if (valueClassroms.Specific.Equals(Specifics[0]) || valueClassroms.Specific.Equals(Specifics[1]))
                                {
                                    tempListClassrooms.Add(valueClassroms);
                                }
                            }

                            if (tempListTeacher.Count > 1)
                            {
                                listLessons.Add(new Lesson(new DropInformation(group, tempListTeacher.ElementAt(rnd.Next(tempListTeacher.Count)), valueGroupAndSubjects.Subject, Specifics[1], tempListClassrooms.ElementAt(rnd.Next(tempListClassrooms.Count)))));
                                valueGroupAndSubjects.ExerciseHour -= 2;
                            }
                            else
                            {
                                listLessons.Add(new Lesson(new DropInformation(group, tempListTeacher.ElementAt(0), valueGroupAndSubjects.Subject, Specifics[1], tempListClassrooms.ElementAt(rnd.Next(tempListClassrooms.Count)))));
                                valueGroupAndSubjects.ExerciseHour -= 2;
                            }
                        }
                        else if (valueGroupAndSubjects.LaboratoryHour != 0)
                        {
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

                            var tempListClassrooms = new List<ClassRoom>();
                            foreach (var valueClassroms in ClassClassrooms)
                            {
                                if (valueClassroms.Specific.Equals(Specifics[2]))
                                {
                                    tempListClassrooms.Add(valueClassroms);
                                }
                            }

                            if (tempListTeacher.Count > 1)
                            {
                                listLessons.Add(new Lesson(new DropInformation(group, tempListTeacher.ElementAt(rnd.Next(tempListTeacher.Count)), valueGroupAndSubjects.Subject, Specifics[2], tempListClassrooms.ElementAt(rnd.Next(tempListClassrooms.Count)))));
                                valueGroupAndSubjects.LaboratoryHour -= 2;
                            }
                            else
                            {
                                listLessons.Add(new Lesson(new DropInformation(group, tempListTeacher.ElementAt(0), valueGroupAndSubjects.Subject, Specifics[2], tempListClassrooms.ElementAt(rnd.Next(tempListClassrooms.Count)))));
                                valueGroupAndSubjects.LaboratoryHour -= 2;
                            }
                        }
                    }
                }
            }

            return listLessons;
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

        public void ValidationForDrop(object element)
        {
            //объявляем переменную данных перетаскиваемого элемента
            element.GetType();
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
                                if (value.Teacher.CodeOfTeacher == teacher.CodeOfTeacher
                                    && value.Teacher.Department.CodeOfDepartment == teacher.Department.CodeOfDepartment)
                                {
                                    item.IsValueValid = value.DayList.ToList().Exists(t => t == item.Info.Day);
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
                                        if (value.Teacher.CodeOfTeacher == item.Item.Teacher.CodeOfTeacher
                                            && value.Teacher.Department.CodeOfDepartment == item.Item.Teacher.Department.CodeOfDepartment
                                                && item.Item.Group.Exists(g => g.CodeOfGroup == groupvalue.Group.CodeOfGroup))
                                        {
                                            item.IsValueValid = (value.SubjectList.ToList().Exists(t => t.CodeOfSubject == subject.CodeOfSubject)) && (groupvalue.InformationAboutSubjects.ToList().Exists(g => g.Subject.CodeOfSubject == subject.CodeOfSubject));
                                        }
                                }
                            }
                            else
                            if (item.ItemTwo.Teacher != null && item.N_DIndex == -1)
                            {
                                foreach (var groupvalue in AllGroupsAndSubjects)
                                {
                                    foreach (var value in AllTeachersAndSubjects)
                                        if (value.Teacher.CodeOfTeacher == item.ItemTwo.Teacher.CodeOfTeacher
                                            && value.Teacher.Department.CodeOfDepartment == item.ItemTwo.Teacher.Department.CodeOfDepartment
                                                && item.ItemTwo.Group.Exists(g => g.CodeOfGroup == groupvalue.Group.CodeOfGroup))
                                        {
                                            item.IsValueValid = (value.SubjectList.ToList().Exists(t => t.CodeOfSubject == subject.CodeOfSubject)) && (groupvalue.InformationAboutSubjects.ToList().Exists(g => g.Subject.CodeOfSubject == subject.CodeOfSubject));
                                        }
                                }
                            }
                        }
                        else
                        {
                            item.IsValueValid = true;
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
                    Transform(0);
                    Console.Clear();
                    for (int i = 0; i < Filtered.Count; i++)
                    {
                        for (int j = 0; j < Filtered[i].Count; j++)
                        {
                            if ((Filtered[i][j].Item.Group != null) && (Filtered[i][j].Item.NumberOfClassroom != null) && (Filtered[i][j].Item.Specifics != null) && (Filtered[i][j].Item.Subject != null) && (Filtered[i][j].Item.Teacher != null))
                            {
                                //Console.WriteLine("Day:" + Filtered[i][j].Info.Day + " pair:" + Filtered[i][j].Info.Pair + " Key:" + Filtered[i][j].Key + " KeyType: " + Filtered[i][j].KeyType + " State:" + Filtered[i][j].State + " ND:" + Filtered[i][j].Item.Ndindex + " NDNUM " + Filtered[i][j].N_DIndex);
                                RequestToDataBase.Instance.requestInsertIntoClassesItemOne(Filtered[i][j], name);
                            }
                            if ((Filtered[i][j].ItemTwo.Group != null) && (Filtered[i][j].ItemTwo.NumberOfClassroom != null) && (Filtered[i][j].ItemTwo.Specifics != null) && (Filtered[i][j].ItemTwo.Subject != null) && (Filtered[i][j].ItemTwo.Teacher != null))
                            {
                                // Console.WriteLine("Day:" + Filtered[i][j].Info.Day + " pair:" + Filtered[i][j].Info.Pair + " Key:" + Filtered[i][j].Key + " KeyType: " + Filtered[i][j].KeyType + " State:" + Filtered[i][j].State + " ND:" + Filtered[i][j].ItemTwo.Ndindex + "NDNUM " + Filtered[i][j].N_DIndex);
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
    }

}
