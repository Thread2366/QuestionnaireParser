using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace QuestionnaireParser
{
    class Visualizer : IDisposable
    {
        Application oApp;
        Workbook oWb;

        public string ExcelPath { get; }

        public Visualizer(string excelPath)
        {
            ExcelPath = excelPath;

            oApp = new Application();
            oWb = oApp.Workbooks.Open(ExcelPath);
        }

        public void Visualize(IEnumerable<Dictionary<int, int>> answers)
        {
            throw new NotImplementedException();
        }


        //public void CopySheet(int sourceIndex, int destIndex, string destName)
        //{
        //    Worksheet oSheet = null;
        //    try
        //    {
        //        oSheet = oSheets[sourceIndex];

        //        Worksheet destAdjacent = null;
        //        try
        //        {
        //            if (destIndex == 1)
        //            {
        //                destAdjacent = oSheets[1];
        //                oSheet.Copy(Before: destAdjacent);
        //            }
        //            else
        //            {
        //                destAdjacent = oSheets[destIndex - 1];
        //                oSheet.Copy(After: destAdjacent);
        //            }
        //        }
        //        finally
        //        {
        //            if (destAdjacent != null) Marshal.ReleaseComObject(destAdjacent);
        //        }

        //        Worksheet destSheet = null;
        //        try
        //        {
        //            destSheet = oSheets[destIndex];
        //            destSheet.Name = destName;
        //        }
        //        finally
        //        {
        //            if (destSheet != null) Marshal.ReleaseComObject(destSheet);
        //        }
        //    }
        //    finally
        //    {
        //        if (oSheet != null) Marshal.ReleaseComObject(oSheet);
        //    }

        //    oWb.Save();
        //}

        public void Dispose()
        {
            if (oWb != null)
            {
                oWb.Close(false);
                oWb = null;
            }
            if (oApp != null)
            {
                oApp.Quit();
                oApp = null;
            }
        }
    }
}
