using ClosedXML.Excel;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace SozdanieRaspisaniya.ViewModel
{
    class WorkWithExcel
    {
        private ObservableCollection<string> columns { get; }
        private ObservableCollection<ObservableCollection<DropItem>> filtered { get; }

        private string[] strPair = { "I 8:30 - 10:05", "II 10:20 - 11:55", "III 12:10 - 13:45", "IV 14:15 - 15:50", "V 16:05 - 17:40", "VI 17:50 - 19:25" };
        private int maxpair;
        private int ch;

        public WorkWithExcel(ObservableCollection<string> Columns, ObservableCollection<ObservableCollection<DropItem>> Filtered, int maxpair, int ch)
        {
            columns = new ObservableCollection<string>();
            columns = Columns;
            filtered = new ObservableCollection<ObservableCollection<DropItem>>();
            filtered = Filtered;
            this.maxpair = maxpair;
            this.ch = ch;
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

            for (int c = 0; c < columns.Count; c++)
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

                worksheet.Cell(1, 3 + c).Value = columns[c];
            }

            for (int i = 0; i < filtered.Count; i++)
            {
                for (int j = 0; j < filtered[i].Count; j++)
                {
                    worksheet.Cell(2 * i + 2, 3 + j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Cell(2 * i + 2, 3 + j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(2 * i + 3, 3 + j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    if (ch == 0)
                    {
                        if (filtered[i][j].Item.Group != null)
                        {
                            if (filtered[i][j].State == 0)
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = filtered[i][j].Item.Subject + " " + filtered[i][j].Item.Specifics + " " + filtered[i][j].Item.NumberOfClassroom + " " + filtered[i][j].Item.Teacher;
                                worksheet.Range(2 * i + 2, 3 + j, 2 * i + 3, 3 + j).Merge();
                            }
                            else
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = filtered[i][j].Item.Subject + " " + filtered[i][j].Item.Specifics + " " + filtered[i][j].Item.NumberOfClassroom + " " + filtered[i][j].Item.Teacher;
                                worksheet.Cell(2 * i + 3, 3 + j).Value = filtered[i][j].ItemTwo.Subject + " " + filtered[i][j].ItemTwo.Specifics + " " + filtered[i][j].ItemTwo.NumberOfClassroom + " " + filtered[i][j].ItemTwo.Teacher;
                            }
                        }
                    }
                    else if (ch == -1)
                    {
                        if (filtered[i][j].Item.Teacher != null)
                        {
                            if (filtered[i][j].State == 0)
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = filtered[i][j].Item.Subject + " " + filtered[i][j].Item.Specifics + " " + filtered[i][j].Item.NumberOfClassroom + " " + string.Join("+", filtered[i][j].Item.Group.Select(gr => gr.NameOfGroup));
                                worksheet.Range(2 * i + 2, 3 + j, 2 * i + 3, 3 + j).Merge();
                            }
                            else
                            {
                                worksheet.Cell(2 * i + 2, 3 + j).Value = filtered[i][j].Item.Subject + " " + filtered[i][j].Item.Specifics + " " + filtered[i][j].Item.NumberOfClassroom + " " + string.Join("+", filtered[i][j].Item.Group.Select(gr => gr.NameOfGroup));
                                worksheet.Cell(2 * i + 3, 3 + j).Value = filtered[i][j].ItemTwo.Subject + " " + filtered[i][j].ItemTwo.Specifics + " " + filtered[i][j].ItemTwo.NumberOfClassroom + " " + string.Join("+", filtered[i][j].ItemTwo.Group.Select(gr => gr.NameOfGroup));
                            }
                        }
                    }
                    //else
                    //{
                    //    if (filtered[i][j].Item.NumberOfClassroom != null)
                    //    {
                    //        if (filtered[i][j].State == 0)
                    //        {
                    //            worksheet.Cell(2 * i + 2, 3 + j).Value = filtered[i][j].Item.Teacher + " " + filtered[i][j].Item.Subject + " " + filtered[i][j].Item.Specifics + " " + string.Join("+", filtered[i][j].Item.Group.Select(gr => gr.NameOfGroup));
                    //            worksheet.Range(2 * i + 2, 3 + j, 2 * i + 3, 3 + j).Merge();
                    //        }
                    //        else
                    //        {
                    //            worksheet.Cell(2 * i + 2, 3 + j).Value = filtered[i][j].Item.Teacher + " " + filtered[i][j].Item.Subject + " " + filtered[i][j].Item.Specifics + " " + string.Join("+", filtered[i][j].Item.Group.Select(gr => gr.NameOfGroup));
                    //            worksheet.Cell(2 * i + 3, 3 + j).Value = filtered[i][j].ItemTwo.Teacher + " " + filtered[i][j].ItemTwo.Subject + " " + filtered[i][j].ItemTwo.Specifics + " " + string.Join("+", filtered[i][j].ItemTwo.Group.Select(gr => gr.NameOfGroup));
                    //        }
                    //    }
                    //}
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

        public void ExportToExcelSeparately()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Книга Excel (*.xlsx)|*.xlsx";
            string path = "";
            if (saveFileDialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                {
                    path = saveFileDialog.FileName;
                }
            }

            for (int c = 0; c < columns.Count; c++)
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

                worksheet.Cell(1, 3).Value = columns[c];

                for (int i = 0; i < filtered.Count; i++)
                {
                    if (ch == 1)
                    {
                        if (filtered[i][c].Item.NumberOfClassroom != null || filtered[i][c].ItemTwo.NumberOfClassroom != null)
                        {
                            worksheet.Cell(2 * i + 2, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            worksheet.Cell(2 * i + 2, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(2 * i + 3, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            if (filtered[i][c].State == 0)
                            {
                                worksheet.Cell(2 * i + 2, 3).Value = filtered[i][c].Item.Subject + " " + filtered[i][c].Item.Specifics + " " + filtered[i][c].Item.Teacher + " " + string.Join(" ", filtered[i][c].Item.Group);
                                worksheet.Range(2 * i + 2, 3, 2 * i + 3, 3).Merge();
                            }
                            else
                            {
                                worksheet.Cell(2 * i + 2, 3).Value = filtered[i][c].Item.Subject + " " + filtered[i][c].Item.Specifics + " " + filtered[i][c].Item.Teacher + " " + string.Join(" ", filtered[i][c].Item.Group);
                                worksheet.Cell(2 * i + 3, 3).Value = filtered[i][c].ItemTwo.Subject + " " + filtered[i][c].ItemTwo.Specifics + " " + filtered[i][c].ItemTwo.Teacher + " " + string.Join(" ", filtered[i][c].ItemTwo.Group);
                            }
                        }
                    }
                    else if (ch == -1)
                    {
                        if (filtered[i][c].Item.Teacher != null || filtered[i][c].ItemTwo.Teacher != null)
                        {
                            worksheet.Cell(2 * i + 2, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            worksheet.Cell(2 * i + 2, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(2 * i + 3, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            if (filtered[i][c].State == 0)
                            {
                                worksheet.Cell(2 * i + 2, 3).Value = filtered[i][c].Item.Subject + " " + filtered[i][c].Item.Specifics + " " + filtered[i][c].Item.NumberOfClassroom + " " + string.Join(" ", filtered[i][c].Item.Group);
                                worksheet.Range(2 * i + 2, 3, 2 * i + 3, 3).Merge();
                            }
                            else
                            {
                                worksheet.Cell(2 * i + 2, 3).Value = filtered[i][c].Item.Subject + " " + filtered[i][c].Item.Specifics + " " + filtered[i][c].Item.NumberOfClassroom + " " + string.Join(" ", filtered[i][c].Item.Group);
                                worksheet.Cell(2 * i + 3, 3).Value = filtered[i][c].ItemTwo.Subject + " " + filtered[i][c].ItemTwo.Specifics + " " + filtered[i][c].ItemTwo.NumberOfClassroom + " " + string.Join(" ", filtered[i][c].ItemTwo.Group);
                            }
                        }
                    }
                }
                string fileName = " Расписание " + filtered[0][c].Key + ".xlsx";
                workbook.SaveAs(path + fileName);
            }
            MessageBox.Show("Сохранено");
        }

        public void SendExcelFile()
        {
            string mailLogin = XMLConfig.ReadMailLoginValue(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "XMLConfig.xml");
            string mailPassword = XMLConfig.ReadMailPasswordValue(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "XMLConfig.xml");
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(mailLogin, mailPassword);
            MailAddress from = new MailAddress(mailLogin);

            for (int c = 0; c < columns.Count; c++)
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

                worksheet.Cell(1, 3).Value = columns[c];

                for (int i = 0; i < filtered.Count; i++)
                {
                    if (filtered[i][c].Item.Teacher != null || filtered[i][c].ItemTwo.Teacher != null)
                    {
                        worksheet.Cell(2 * i + 2, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        worksheet.Cell(2 * i + 2, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(2 * i + 3, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        if (filtered[i][c].State == 0)
                        {
                            worksheet.Cell(2 * i + 2, 3).Value = filtered[i][c].Item.Subject + " " + filtered[i][c].Item.Specifics + " " + filtered[i][c].Item.NumberOfClassroom + " " + string.Join(" ", filtered[i][c].Item.Group);
                            worksheet.Range(2 * i + 2, 3, 2 * i + 3, 3).Merge();
                        }
                        else
                        {
                            worksheet.Cell(2 * i + 2, 3).Value = filtered[i][c].Item.Subject + " " + filtered[i][c].Item.Specifics + " " + filtered[i][c].Item.NumberOfClassroom + " " + string.Join(" ", filtered[i][c].Item.Group);
                            worksheet.Cell(2 * i + 3, 3).Value = filtered[i][c].ItemTwo.Subject + " " + filtered[i][c].ItemTwo.Specifics + " " + filtered[i][c].ItemTwo.NumberOfClassroom + " " + string.Join(" ", filtered[i][c].ItemTwo.Group);
                        }
                    }
                }

                string fileName = "Расписание " + filtered[0][c].Key + ".xlsx";
                workbook.SaveAs(fileName);

                //MailAddress to = new MailAddress(filtered[0][c].Item.Teacher.Mail);
                //MailMessage m = new MailMessage(from, to);
                //m.Subject = "Тест";
                //m.Body = "Письмо-тест работы отправки сообщения";
                //m.Attachments.Add(new Attachment(fileName));
                //smtp.Send(m);

                System.IO.File.Delete(fileName);
            }
            MessageBox.Show("Расписание отправленно преподавателям");
        }

        public bool IsValidate()
        {
            if (ch == 0)
            {
                int iindex = 0;
                int jindex = 0;
                for (int i = 0; i < maxpair; i++)
                {
                    for (int j = 0; j < columns.Count; j++)
                    {
                        if (filtered[i][j].Item.Teacher != null)
                        {
                            iindex = i;
                            jindex = j;
                            continue;
                        }
                    }
                }
                for (int j = 0; j < columns.Count; j++)
                {
                    if ((filtered[iindex][jindex].Item.Teacher == filtered[iindex][j].Item.Teacher) && (filtered[iindex][jindex].Item.NumberOfClassroom != filtered[iindex][j].Item.NumberOfClassroom))
                    {
                        MessageBox.Show($"{filtered[iindex][jindex].Item.Teacher} не может вести занятия в аудитории {filtered[iindex][jindex].Item.NumberOfClassroom} и { filtered[iindex][j].Item.NumberOfClassroom} одновременно у групп {filtered[iindex][jindex].Item.Group} и {filtered[iindex][j].Item.Group}");
                        return false;
                    }
                    if ((filtered[iindex][jindex].Item.Teacher == filtered[iindex][j].Item.Teacher) && (filtered[iindex][jindex].Item.Subject != filtered[iindex][j].Item.Subject))
                    {
                        MessageBox.Show($"{filtered[iindex][jindex].Item.Teacher} не может вести предметы {filtered[iindex][jindex].Item.Subject} и { filtered[iindex][j].Item.Subject} одновременно в группах {filtered[iindex][jindex].Item.Group} и {filtered[iindex][j].Item.Group}");
                        return false;
                    }
                }
                return true;
            }
            else return true;
        }
    }
}
