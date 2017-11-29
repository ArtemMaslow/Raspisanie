using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SozdanieRaspisaniya
{
   public class ExcelWork
    {
        private string path = @"C:\Users\Artem\Desktop\1.xls";

        //private Excel.Application _application = null;
        //private Excel.Workbook _workBook = null;
        //private Excel.Worksheet _workSheet = null;
        //private object _missingObj = System.Reflection.Missing.Value;

        //public ExcelWork()
        //{
        //    _application = new Excel.Application();
        //    _workBook = _application.Workbooks.Add(_missingObj);
        //    _workSheet = (Excel.Worksheet)_workBook.Worksheets.get_Item(1);
        //}

        //public ExcelWork(string path)
        //{
        //    object pathObj = path;

        //    _application = new Excel.Application();
        //    _workBook = _application.Workbooks.Add(this.path);
        //    _workSheet = (Excel.Worksheet)_workBook.Worksheets.get_Item(1);
        //}

        //public void SetCellValue(string cellValue, int rowIndex, int columnIndex)
        //{
        //    _workSheet.Cells[rowIndex, columnIndex] = cellValue;
        //}

        //public void Close()
        //{
        //    _workBook.Close(false, _missingObj, _missingObj);

        //    _application.Quit();

        //    System.Runtime.InteropServices.Marshal.ReleaseComObject(_application);

        //    _application = null;
        //    _workBook = null;
        //    _workSheet = null;

        //    System.GC.Collect();
        //}

    }
}
