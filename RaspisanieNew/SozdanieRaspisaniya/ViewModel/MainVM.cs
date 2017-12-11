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

        public void ExportToExcel()
        {
            int maxpair = 5 * SheduleSettings.WeekDayMaxCount + SheduleSettings.SaturdayMaxCount;
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Преподователи");
            for (int i = 1; i < maxpair; i++)
            {
                for (int j = 1; j < ClassTeachers.Length; j++)
                {
                    worksheet.Cell(i, j).Value =Data[i][j].ToString();
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
            saveToExcel = this.Factory.CommandSync(ExportToExcel);
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
