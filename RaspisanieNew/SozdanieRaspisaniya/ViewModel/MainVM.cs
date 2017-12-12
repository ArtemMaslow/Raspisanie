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

namespace SozdanieRaspisaniya.ViewModel
{
    class MainVM : ViewModelBase
    {
        private readonly INotifyCommand closeWinCommand;
        private readonly INotifyCommand selectCommand;

        private readonly INotifyCommand createCommand;
        private readonly INotifyCommand openCommand;
        private readonly INotifyCommand saveCommand;
        private readonly INotifyCommand saveToExcel;


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
        public void Remove()
        {

        }
        public void Save()
        {
            Console.WriteLine("SaveCommand");
        }

        private void Transform(int to)
        {
            Dictionary<string, int> dct;//объявляем словарь
            Type keyType;//тип ключа
            if (to == 0)//если параметр 0
            {
                //x - группа, i - индекс 
                //k - селекторор ключа(название группы), i - селектор эелемента(номер столбца) 
                dct = ClassGroups.Select((x, i) => new { i, x.NameOfGroup })
                .ToDictionary(k => k.NameOfGroup, e => e.i);//выбираем группу и позиию в массиве. Каждому заголовку столбца ставится в соответствие его индекс.
                keyType = typeof(Group);//Тип группа
            }
            else if (to == -1)//аналогично 
            {
                dct = ClassTeachers.Select((x, i) => new { i, x.FIO })
                .ToDictionary(k => k.FIO, e => e.i);
                keyType = typeof(Teacher);
            }
            else
            {
                dct = ClassClassrooms.Select((x, i) => new { i, x.NumberOfClassroom })
                .ToDictionary(k => k.NumberOfClassroom, e => e.i);
                keyType = typeof(ClassRoom);
            }
            var temp = Data.Select(x => x.ToArray()).ToArray();//в временную переменную переносим массив коллекции
            Data.Clear();//отчищаем данные
            Columns.Clear();//отчищаем колонки
            int maxpair = 5 * SheduleSettings.WeekDayMaxCount + SheduleSettings.SaturdayMaxCount;//кол-во пар
            foreach (var r in Rows)
            {
               var row = new ObservableCollection<DropItem>();
               foreach (var key in dct.Keys)
               {
                   DropItem item = new DropItem(key, keyType, r);
                   if (to == 0)
                       item.Item.Group = key;
                   else if (to == -1)
                       item.Item.Teacher = key;
                   else
                       item.Item.NumberOfClassroom = key;

                    row.Add(item);
                    Columns.Add(key);
               }
               Data.Add(row);
            }           
            for (int i = 0; i < temp.Length; i++)
            {
                for (int j = 0; j < temp[0].Length; j++)
                {
                    if (to == 0)
                    {
                        int cind;
                        if (temp[i][j].Item.Group != null)//если в ячейке есть поле Группы
                            if (dct.TryGetValue(temp[i][j].Item.Group, out cind))//находим значение это значение и возвращаем его номер в массиве
                                Data[i][cind].Item = temp[i][j].Item;//вставляем данные в этот столбец
                    }
                    else if (to == -1)
                    {
                        int cind;                        
                        if (temp[i][j].Item.Teacher != null)
                            if (dct.TryGetValue(temp[i][j].Item.Teacher, out cind))
                                Data[i][cind].Item = temp[i][j].Item;
                    }
                    else
                    {
                        int cind;
                        if (temp[i][j].Item.NumberOfClassroom != null)
                            if (dct.TryGetValue(temp[i][j].Item.NumberOfClassroom, out cind))
                                Data[i][cind].Item = temp[i][j].Item;
                    }
                }
            }
        }

        public void ExportToExcel(int to)
        {
            Dictionary<string, int> dct;//объявляем словарь
            Type keyType;//тип ключа
            if (to == 0)//если параметр 0
            {
                //x - группа, i - индекс 
                //k - селекторор ключа(название группы), i - селектор эелемента(номер столбца) 
                dct = ClassGroups.Select((x, i) => new { i, x.NameOfGroup })
                .ToDictionary(k => k.NameOfGroup, e => e.i);//выбираем группу и позиию в массиве. Каждому заголовку столбца ставится в соответствие его индекс.
                keyType = typeof(Group);//Тип группа
            }
            else if (to == -1)//аналогично 
            {
                dct = ClassTeachers.Select((x, i) => new { i, x.FIO })
                .ToDictionary(k => k.FIO, e => e.i);
                keyType = typeof(Teacher);
            }
            else
            {
                dct = ClassClassrooms.Select((x, i) => new { i, x.NumberOfClassroom })
                .ToDictionary(k => k.NumberOfClassroom, e => e.i);
                keyType = typeof(ClassRoom);
            }
            
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Лист1");
            int maxpair = 5 * SheduleSettings.WeekDayMaxCount + SheduleSettings.SaturdayMaxCount;

            for (int r = 0; r < maxpair; r++)
            {
                worksheet.Cell(r + 2, 1).Style.Alignment.TextRotation = 90;
                worksheet.Cell(r + 2, 1).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
                worksheet.Cell(r + 2, 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(r + 2, 1).Style.Border.TopBorderColor = XLColor.Black;
                worksheet.Cell(r + 2, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(r + 2, 1).Style.Border.RightBorderColor = XLColor.Black;
                worksheet.Cell(r + 2, 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(r + 2, 1).Style.Border.LeftBorderColor = XLColor.Black;
                worksheet.Cell(r + 2, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(r + 2, 1).Style.Border.BottomBorderColor = XLColor.Black;

                worksheet.Row(r + 2).Height = 25;

                worksheet.Cell(r + 2, 1).Style.Alignment.WrapText = true;
                worksheet.Cell(r + 2, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(r + 2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(r + 2, 1).RichText.FontSize = 20;
                worksheet.Cell(r + 2, 1).RichText.FontColor = XLColor.Black;
                worksheet.Cell(r + 2, 1).RichText.FontName = "Broadway";
                string str = "";
                if (r / 6 < 1)
                {
                    str = "Понедельник";
                    worksheet.Cell(r + 2, 1).Value = str;
                    worksheet.Range("A2:A7").Column(1).Merge();
                }
                else
                if ((r / 6 < 2) && (r / 6 >= 1))
                {
                    str = "Вторник";
                    worksheet.Cell(r + 2, 1).Value = str;
                    worksheet.Range("A8:A13").Column(1).Merge();
                }
                else
                if ((r / 6 < 3) && (r / 6 >= 2))
                {
                    str = "Среда";
                    worksheet.Cell(r + 2, 1).Value = str;
                    worksheet.Range("A14:A19").Column(1).Merge();
                }
                else
                if ((r / 6 < 4) && (r / 6 >= 3))
                {
                    str = "Четверг";
                    worksheet.Cell(r + 2, 1).Value = str;
                    worksheet.Range("A20:A25").Column(1).Merge();
                }
                else
                if ((r / 6 < 5) && (r / 6 >= 4))
                {
                    str = "Пятница";
                    worksheet.Cell(r + 2, 1).Value = str;
                    worksheet.Range("A26:A31").Column(1).Merge();
                }
                else
                {
                    str = "Суббота";
                    worksheet.Cell(r + 2, 1).Value = str;
                    worksheet.Range("A32:A34").Column(1).Merge();
                }

            }
            string[] strPair = { "I\n 8:30-10:05", "II\n 10:20-11:55", "III\n 12:10-13:45", "IV\n 14:15-15:50", "V\n 16:05-17:40", "VI\n 17:50-19:25" };

            for (int r = 1; r <= maxpair; r++)
            {                
                worksheet.Cell(r + 1, 2).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
                worksheet.Cell(r + 1, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(r + 1, 2).Style.Border.TopBorderColor = XLColor.Black;
                worksheet.Cell(r + 1, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(r + 1, 2).Style.Border.RightBorderColor = XLColor.Black;
                worksheet.Cell(r + 1, 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(r + 1, 2).Style.Border.LeftBorderColor = XLColor.Black;
                worksheet.Cell(r + 1, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(r + 1, 2).Style.Border.BottomBorderColor = XLColor.Black;
            
                worksheet.Row(r + 1).Height = 25;
                worksheet.Cell(r + 1, 2).Value = strPair[(r-1) % strPair.Length];            
            }
                if (to == 0)
            {
                for (int c = 0; c < ClassGroups.Length; c++)
                {
                    worksheet.Column(3 + c).Width = 25;
                    worksheet.Cell(1, 3 + c).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
                    worksheet.Cell(1, 3 + c).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.TopBorderColor = XLColor.Black;
                    worksheet.Cell(1, 3 + c).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.RightBorderColor = XLColor.Black;
                    worksheet.Cell(1, 3 + c).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.LeftBorderColor = XLColor.Black;
                    worksheet.Cell(1, 3 + c).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.BottomBorderColor = XLColor.Black;

                    worksheet.Cell(1, 3 + c).Value = ClassGroups[c].NameOfGroup;
                }
            }
            else if (to == -1)
            {
                for (int c = 0; c <= ClassTeachers.Length; c++)
                {
                    worksheet.Column(3 + c).Width = 25;
                    worksheet.Cell(1, 3 + c).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
                    worksheet.Cell(1, 3 + c).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.TopBorderColor = XLColor.Black;
                    worksheet.Cell(1, 3 + c).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.RightBorderColor = XLColor.Black;
                    worksheet.Cell(1, 3 + c).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.LeftBorderColor = XLColor.Black;
                    worksheet.Cell(1, 3 + c).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.BottomBorderColor = XLColor.Black;

                    worksheet.Cell(1, 3 + c).Value = ClassTeachers[c].FIO;
                }
            }
            else
            {
                for (int c = 0; c < ClassClassrooms.Length; c++)
                {
                    worksheet.Column(3 + c).Width = 25;
                    worksheet.Cell(1, 3 + c).Style.Fill.BackgroundColor = XLColor.FromIndex(22);
                    worksheet.Cell(1, 3 + c).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.TopBorderColor = XLColor.Black;
                    worksheet.Cell(1, 3 + c).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.RightBorderColor = XLColor.Black;
                    worksheet.Cell(1, 3 + c).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.LeftBorderColor = XLColor.Black;
                    worksheet.Cell(1, 3 + c).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(1, 3 + c).Style.Border.BottomBorderColor = XLColor.Black;

                    worksheet.Cell(1, 3 + c).Value = ClassClassrooms[c].NumberOfClassroom;
                }
            }
            var temp = Data.Select(x => x.ToArray()).ToArray();
            for (int i = 0; i < temp.Length; i++)
            {
                for (int j = 0; j < temp[0].Length; j++)
                {
                    if (to == 0)
                    {
                        int cind;
                        if (temp[i][j].Item.Group != null)//если в ячейке есть поле Группы
                            if (dct.TryGetValue(temp[i][j].Item.Group, out cind))//находим значение это значение и возвращаем его номер в массиве
                            {
                                Data[i][cind].Item = temp[i][j].Item;//вставляем данные в этот столбец                  
                                worksheet.Cell(i + 2, 2 + cind + 1).Value = Data[i][cind].Item.NumberOfClassroom + " " + Data[i][cind].Item.Subject + " " + Data[i][cind].Item.Teacher;
                            }

                    }
                    else if (to == -1)
                    {
                        int cind;
                        if (temp[i][j].Item.Teacher != null)
                            if (dct.TryGetValue(temp[i][j].Item.Teacher, out cind))
                            {
                                Data[i][cind].Item = temp[i][j].Item;
                                worksheet.Cell(i + 2, 2 + cind + 1).Value = Data[i][cind].Item.NumberOfClassroom + " " + Data[i][cind].Item.Subject + " " + Data[i][cind].Item.Group;
                            }
                    }
                    else
                    {
                        int cind;
                        if (temp[i][j].Item.NumberOfClassroom != null)
                            if (dct.TryGetValue(temp[i][j].Item.NumberOfClassroom, out cind))
                            {
                                Data[i][cind].Item = temp[i][j].Item;
                                worksheet.Cell(i + 2, 2 + cind + 1).Value = Data[i][cind].Item.Subject + " " + Data[i][cind].Item.Group + " " + Data[i][cind].Item.Teacher;
                            }
                    }
                }
            }
            workbook.SaveAs(@"C:\Users\Artem\Desktop\1.xlsx");
            MessageBox.Show("all done");
        }
        public MainVM()
        {
            ClassClassrooms = XMLRead.ReadClassroom(Path.ClassroomXml).ToArray();
            ClassGroups = XMLRead.ReadGroup(Path.GroupXml).ToArray();
            ClassTeachers = XMLRead.ReadTeacher(Path.TeacherXml).ToArray();
            ClassSubjects = XMLRead.ReadSubject(Path.SubjectXml).ToArray();

            int maxpair = 5 * SheduleSettings.WeekDayMaxCount + SheduleSettings.SaturdayMaxCount;

            Data = new ObservableCollection<ObservableCollection<DropItem>>();
            for (int i = 0; i < maxpair; i++)
                Data.Add(new ObservableCollection<DropItem>());
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
                        Data[j].Add(new DropItem(ClassGroups[i].NameOfGroup, typeof(Group), pair));
                        Data[j][i].Item = new DropInformation { Group = ClassGroups[i].NameOfGroup };
                        j++;
                    }
                }
            }
            Columns = new ObservableCollection<string>(Data.First().Select(x => x.Key));
            Rows = new ObservableCollection<PairInfo>(Data.Select(x => x[0].Info));

            createCommand = this.Factory.CommandSync(Create);
            openCommand = this.Factory.CommandSync(Open);
            saveCommand = this.Factory.CommandSync(Save);
            saveToExcel = this.Factory.CommandSyncParam<int>(ExportToExcel);
            selectCommand = this.Factory.CommandSyncParam<int>(Transform) ;
            closeWinCommand = this.Factory.CommandSync(Close);
        }
        public ObservableCollection<ObservableCollection<DropItem>> Data { get; }
        public ObservableCollection<string> Columns { get; }
        public ObservableCollection<PairInfo> Rows { get; }

        public Group[] ClassGroups { get; }
        public Subject[] ClassSubjects { get; }
        public Teacher[] ClassTeachers { get; }
        public ClassRoom[] ClassClassrooms { get; }

        public ICommand CloseWinCommand => closeWinCommand;
        public ICommand CreateCommand => createCommand;
        public ICommand OpenCommand => openCommand;
        public ICommand SaveCommand => saveCommand;
        public ICommand SaveToExcel => saveToExcel;
        public ICommand SelectCommand => selectCommand;
    }

}
