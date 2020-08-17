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
            oApp.Visible = false;
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

                if (dateCell.Row - 1 == percentsCell.Row) continue;

                Range colFrom = sheet.Cells[dateCell.Row - 1, percentsCell.Column];
                Range colTo = colFrom.End[XlDirection.xlToRight];
                var rowFrom = sheet.Range[colFrom, colTo];
                var rowTo = rowFrom.Offset[RowOffset: 1];
                rowFrom.AutoFill(sheet.Range[rowFrom, rowTo]);

                Range chartSource;
                var headers = sheet.Range[percentsCell, percentsCell.End[XlDirection.xlToRight]];
                if (rowTo.Row - percentsCell.Row > 7)
                {
                    rowFrom = rowTo.Offset[RowOffset: -6];
                    var data = sheet.Range[rowFrom, rowTo];
                    chartSource = oApp.Union(headers, data);
                }
                else
                {
                    chartSource = sheet.Range[headers, rowTo];
                }
                ChartObject chartObj = sheet.ChartObjects(1);
                chartObj.Chart.ChartWizard(
                    Source: chartSource,
                    CategoryLabels: 1,
                    SeriesLabels: 1);
            }

            oWb.Save();
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

                if (answersCount <= 1) continue;

                var headerFrom = percentsCell.Offset[ColumnOffset: 1];
                var percentsFrom = headerFrom.Offset[RowOffset: 1];
                var formulaFrom = sheet.Range[headerFrom, percentsFrom];
                var formulaTo = sheet.Range[formulaFrom, formulaFrom.Offset[ColumnOffset: answersCount - 1]];
                formulaFrom.AutoFill(formulaTo);

                var chartSourceFrom = formulaFrom.Offset[ColumnOffset: -1];
                var chartSourceTo = chartSourceFrom.Offset[ColumnOffset: answersCount];
                ChartObject chartObj = sheet.ChartObjects(1);
                chartObj.Chart.ChartWizard(
                    Source: sheet.Range[chartSourceFrom, chartSourceTo],
                    CategoryLabels: 1,
                    SeriesLabels: 1);
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
