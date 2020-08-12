using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace QuestionnaireParser
{
    [TestFixture]
    class VisualizerTests
    {
        string excelPath = @"C:\Users\virus\Desktop\Работа\Задача с анкетами\Опрос 1\Результаты опроса.xlsx";

        [Test]
        public void ReleaseComTest()
        {
            using (var excel = new Visualizer(excelPath))
            {
                //excel.CopySheet(1, 2, "Вопрос 2");
                //var count = excel.SheetsCount;
            }

            Assert.Pass();
        }
    }
}
