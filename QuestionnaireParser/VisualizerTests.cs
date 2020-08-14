using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;

namespace QuestionnaireParser
{
    [TestFixture]
    class VisualizerTests
    {
        string excelPath = @"\\prominn\RHO\SYNC\iabdullaev\Desktop\Задача с анкетами\Опрос 1\Результаты опроса.xlsx";
        string inputLocationsPath = @"\\prominn\RHO\SYNC\iabdullaev\Desktop\Задача с анкетами\Опрос 1\inputLocations.xml";

        Dictionary<int, int>[] answers = new Dictionary<int, int>[]
        {
            new Dictionary<int, int>()
            {
                { 1, 1 },
                { 2, 1 },
                { 3, 1 }
            }
        };

        [Test]
        public void VisualizeTest()
        {
            using (var visualizer = new Visualizer(excelPath, XElement.Parse(File.ReadAllText(inputLocationsPath))))
            {
                visualizer.Visualize(answers);
            }

            Assert.Pass();
        }

        [Test]
        public void ChartTest()
        {
            Action func = () =>
            {
                using (var visualizer = new Visualizer(excelPath, XElement.Parse(File.ReadAllText(inputLocationsPath))))
                {
                    visualizer.ChartTest();
                }
            };

            Assert.DoesNotThrow(new TestDelegate(func));
        }
    }
}
