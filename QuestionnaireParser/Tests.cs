﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenCvSharp;
using NUnit.Framework;

namespace QuestionnaireParser
{
    [TestFixture]
    class Tests
    {
        string excelPath = @"\\prominn\RHO\SYNC\iabdullaev\Desktop\Задача с анкетами\Опрос 1\Результаты опроса.xlsx";
        string inputLocationsPath = @"C:\Users\virus\Desktop\Работа\Задача с анкетами\Опрос 1\inputLocations.xml";
        string scanPdfPath = @"C:\Users\virus\Desktop\Работа\Задача с анкетами\Опрос 1\Анкета.pdf";

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
        public void ParseTest()
        {
            var parser = new Parser(XElement.Parse(File.ReadAllText(inputLocationsPath)));
            var result = parser.Parse(scanPdfPath);

            Assert.Pass();
        }
    }
}
