using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace QuestionnaireParser
{
    class Excel : IDisposable
    {
        Application oApp = null;
        Workbooks oWbs = null;
        Workbook oWb = null;
        Sheets oSheets = null;

        public int SheetsCount { get => oSheets.Count; }

        public Excel(string path)
        {
            oApp = new Application();
            oWbs = oApp.Workbooks;
            oWb = oWbs.Open(path);
            oSheets = oWb.Worksheets;
        }

        public void CopySheet(int sourceIndex, int destIndex, string destName)
        {
            Worksheet oSheet = null;
            try
            {
                oSheet = oSheets[sourceIndex];

                Worksheet destAdjacent = null;
                try
                {
                    if (destIndex == 1)
                    {
                        destAdjacent = oSheets[1];
                        oSheet.Copy(Before: destAdjacent);
                    }
                    else
                    {
                        destAdjacent = oSheets[destIndex - 1];
                        oSheet.Copy(After: destAdjacent);
                    }
                }
                finally
                {
                    if (destAdjacent != null) Marshal.ReleaseComObject(destAdjacent);
                }

                Worksheet destSheet = null;
                try
                {
                    destSheet = oSheets[destIndex];
                    destSheet.Name = destName;
                }
                finally
                {
                    if (destSheet != null) Marshal.ReleaseComObject(destSheet);
                }
            }
            finally
            {
                if (oSheet != null) Marshal.ReleaseComObject(oSheet);
            }

            oWb.Save();
        }

        public void Dispose()
        {
            if (oSheets != null) Marshal.ReleaseComObject(oSheets);
            if (oWb != null)
            {
                oWb.Close(false);
                Marshal.ReleaseComObject(oWb);
            }
            if (oWbs != null) Marshal.ReleaseComObject(oWbs);
            if (oApp != null)
            {
                oApp.Quit();
                Marshal.ReleaseComObject(oApp);
            }
        }
    }
}
