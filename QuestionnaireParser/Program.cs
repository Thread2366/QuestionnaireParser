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
using Utils;

namespace QuestionnaireParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.GetDirectoryName(Directory.GetCurrentDirectory());

            var excelTemplate = Path.Combine(path, "Шаблон.xlsx");

            Parallel.ForEach(Directory.EnumerateDirectories(path)
                .Where(dir => Path.GetFileName(dir) != "bin"), 
            dir =>
            {
                var excelPath = Path.Combine(dir, $"{Path.GetFileName(dir)} - результаты.xlsx");
                var inputLocationsPath = Path.Combine(dir, "inputLocations.xml");
                if (!File.Exists(inputLocationsPath)) return;
                var inputLocations = XElement.Parse(File.ReadAllText(inputLocationsPath));
                var processedPath = Path.Combine(dir, "Обработано");

                if (!Directory.Exists(processedPath))
                    Directory.CreateDirectory(processedPath);
                if (!File.Exists(excelPath))
                    File.Copy(excelTemplate, excelPath);

                var parser = new Parser(inputLocations);
                var questionnaires = Directory.EnumerateFiles(dir, "*.pdf");
                var answers = questionnaires
                    .AsParallel()
                    .Select(scanPdf => parser.Parse(scanPdf))
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

                foreach (var file in questionnaires)
                {
                    var destPath = Path.Combine(processedPath, Path.GetFileName(file));
                    if (File.Exists(destPath)) File.Delete(destPath);
                    File.Move(file, destPath);
                }
            });
        }
    }
}
