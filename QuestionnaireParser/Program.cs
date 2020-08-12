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
            //Parse(@"\\prominn\RHO\SYNC\iabdullaev\Desktop\inputLocations.xml",
            //    @"\\prominn\RHO\SYNC\iabdullaev\Desktop\Анкета 1.pdf",
            //    @"\\prominn\RHO\SYNC\iabdullaev\Desktop\result.txt");
            var path = @"C:\Users\virus\Desktop\Работа\Задача с анкетами";
            var excelTemplate = Path.Combine(path, "Шаблон.xlsx");

            //Parse(Path.Combine(dir, "inputLocations.xml"), Path.Combine(dir, "Анкета.pdf"), Path.Combine(dir, "result.txt"));

            Parallel.ForEach(Directory.EnumerateDirectories(path), dir =>
            {
                var excelPath = Path.Combine(dir, "Результаты опроса.xlsx");
                if (!File.Exists(excelPath))
                    File.Copy(excelTemplate, excelPath);
                //Parallel.ForEach(Directory.EnumerateFiles(dir, "*.pdf"), pdf =>
                //{
                //    Parse(Path.Combine(dir, "inputLocations.xml"), Path.Combine(dir, "Анкета.pdf"));
                //});
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

        static void Parse(string inputLocationsPath, string scanPath)
        {
            var inputLocations = XElement.Parse(File.ReadAllText(inputLocationsPath));
            var parser = new Parser(inputLocations);
            parser.Parse(scanPath);
        }
    }
}
