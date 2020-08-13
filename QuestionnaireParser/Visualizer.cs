using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Office.Interop.Excel;

namespace QuestionnaireParser
{
    class Visualizer : IDisposable
    {
        string _answersAddress;
        string _percentsAddress;

        Application oApp;
        Workbook oWb;

        public string ExcelPath { get; }
        public XElement InputLocations { get; }

        public Visualizer(string excelPath, XElement inputLocations)
        {
            ExcelPath = excelPath;
            InputLocations = inputLocations;

            oApp = new Application();
            oApp.Visible = true;
            oApp.DisplayAlerts = false;
            oWb = oApp.Workbooks.Open(ExcelPath);
        }

        public void Visualize(Dictionary<int, int>[] answers)
        {
            if (oWb.ActiveSheet.Name == "Шаблон") Initialize();

            var sheets = oWb.Worksheets;

            for (int i = 0; i < answers.Length; i++)
            {
                Worksheet sheet = sheets[i + 1];

                Range answerCell;
                if (_answersAddress == null)
                {
                    answerCell = sheet.UsedRange.Find("!Ответы");
                    _answersAddress = answerCell.Address;
                }
                else answerCell = sheet.Range[_answersAddress];

                Range percentsCell;
                if (_percentsAddress == null)
                {
                    percentsCell = sheet.UsedRange.Find("!В процентах");
                    _percentsAddress = percentsCell.Address;
                }
                else percentsCell = sheet.Range[_percentsAddress];

                var dateCell = answerCell.EntireColumn
                    .Find("*", SearchOrder: XlSearchOrder.xlByRows, SearchDirection: XlSearchDirection.xlPrevious)
                    .Offset[RowOffset: 1];
                dateCell.Value = DateTime.Now.ToString("dd.MM.yyyy");

                foreach (var pair in answers[i])
                {
                    dateCell.Offset[ColumnOffset: pair.Key].Value = pair.Value;
                }

                if (dateCell.Row - 1 != percentsCell.Row)
                {
                    Range cell = sheet.Cells[dateCell.Row - 1, percentsCell.Column];
                    Range right = cell.End[XlDirection.xlToRight];
                    var row = sheet.Range[cell, right];
                    row.AutoFill(sheet.Range[cell, right.Offset[RowOffset: 1]]);
                }
            }

            throw new NotImplementedException();
        }

        private void Initialize()
        {
            Worksheet template = oWb.Worksheets["Шаблон"];
            var usedRange = template.UsedRange;
            var answersCell = usedRange.Find("!Ответы");
            var percentsCell = usedRange.Find("!В процентах");

            _answersAddress = answersCell.Address;
            _percentsAddress = percentsCell.Address;

            foreach (var question in InputLocations
                .Descendants("Line")
                .Reverse())
            {
                var queNum = int.Parse(question.Attribute("Number").Value);
                template.Copy(After: template);
                Worksheet sheet = oWb.Worksheets[2];
                sheet.Name = $"Вопрос {queNum + 1}";

                answersCell = sheet.Range[_answersAddress];
                percentsCell = sheet.Range[_percentsAddress];

                var answersCount = question.Descendants("Point").Count();
                for (int i = 1; i <= answersCount; i++)
                {
                    answersCell.Offset[ColumnOffset: i].Value = $"Ответ {i}";
                }

                if (answersCount > 1)
                {
                    var headerSource = percentsCell.Offset[ColumnOffset: 1];
                    var percentSource = headerSource.Offset[RowOffset: 1];
                    var formulaSource = sheet.Range[headerSource, percentSource];
                    var formulaDest = sheet.Range[formulaSource, formulaSource.Offset[ColumnOffset: answersCount - 1]];
                    formulaSource.AutoFill(formulaDest);
                }
            }

            template.Delete();
        }

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

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
