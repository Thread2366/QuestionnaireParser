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
            //var path = @"\\prominn\RHO\SYNC\iabdullaev\Desktop\Задача с анкетами";
            var path = Directory.GetCurrentDirectory();

            var excelTemplate = Path.Combine(path, "Шаблон.xlsx");

            Parallel.ForEach(Directory.EnumerateDirectories(path), dir =>
            {
                var excelPath = Path.Combine(dir, $"Результаты ({Path.GetFileName(dir)}).xlsx");
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
        }
    }
}
