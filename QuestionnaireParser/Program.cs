using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;
using System.Configuration;
using System.Diagnostics;

namespace QuestionnaireParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Users\virus\Desktop\Работа\Задача с анкетами";


            var excelTemplate = Path.Combine(path, "Шаблон.xlsx");

            //var dir = Path.Combine(path, "Опрос 1");

            //var excelPath = Path.Combine(dir, "Результаты опроса.xlsx");
            //var inputLocationsPath = Path.Combine(dir, "inputLocations.xml");
            //var inputLocations = XElement.Parse(File.ReadAllText(inputLocationsPath));

            //if (!File.Exists(excelPath))
            //    File.Copy(excelTemplate, excelPath);

            //var checks = Directory.EnumerateFiles(dir, ".pdf")
            //    .Select(scanPdf => new Parser(inputLocations).Parse(scanPdf));

            //var answers = Directory.EnumerateFiles(dir, ".pdf")
            //    .AsParallel()
            //    .Select(scanPdf => new Parser(inputLocations).Parse(scanPdf))
            //    .AsSequential()
            //    .SelectMany(qe => qe.SelectMany((qn, i) => qn.Select(ans => new { Answer = ans, Question = i })))
            //    .GroupBy(x => x.Question)
            //    .OrderBy(x => x.Key)
            //    .Select(grp => grp
            //        .GroupBy(x => x.Answer)
            //        .OrderBy(x => x.Key)
            //        .ToDictionary(x => x.Key, x => x.Count()))
            //    .ToList();

            Parallel.ForEach(Directory.EnumerateDirectories(path), dir =>
            {
                var excelPath = Path.Combine(dir, "Результаты опроса.xlsx");
                var inputLocationsPath = Path.Combine(dir, "inputLocations.xml");
                var inputLocations = XElement.Parse(File.ReadAllText(inputLocationsPath));

                if (!File.Exists(excelPath))
                    File.Copy(excelTemplate, excelPath);

                var answers = Directory.EnumerateFiles(dir, "*.pdf")
                    .AsParallel()
                    .Select(scanPdf => new Parser(inputLocations).Parse(scanPdf))
                    .ToArray()
                    .SelectMany(qe => qe.SelectMany((qn, i) => qn.Select(ans => new { Answer = ans, Question = i })))
                    .GroupBy(x => x.Question)
                    .OrderBy(x => x.Key)
                    .Select(grp => grp
                        .GroupBy(x => x.Answer)
                        .OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Count()))
                    .ToArray();

                using (var visualizer = new Visualizer(excelPath, inputLocations))
                {
                    visualizer.Visualize(answers);
                }
            });



            //var source = @"C:\Users\virus\Desktop\test.txt";
            //var dest = @"C:\Users\virus\Desktop";
            //var thread = new Thread(() =>
            //{
            //    for (int i = 0; i < 10; i++)
            //    {
            //        File.Copy(source, Path.Combine(dest, $"t{i}.txt"));
            //    }
            //});
            //thread.Start();
            //for (int i = 0; i < 10; i++)
            //{
            //    File.Copy(source, Path.Combine(dest, $"m{i}.txt"));
            //}
        }
    }
}
