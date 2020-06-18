using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuestionnaireParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Image<Rgb, byte>[] imgs = null;

            var appdata = Environment.GetEnvironmentVariable("appdata");
            var outputPath = Path.Combine(appdata, "QuestionnaireParser", "TemplateImages");
            //var templatePath = @"C:\Users\virus\Desktop\Работа\Задача с анкетами\Бланк обратной связи.pdf";
            ////imgs = TemplatePdfImages.GetInstance(gsPath, templatePath, outputPath);

            //var gsPath = @"C:\Program Files\gs\gs9.52\bin\gswin64c";
            //GsUtils.PdfToJpeg(gsPath, @"C:\Users\virus\Desktop\Работа\Задача с анкетами\Анкета.pdf",
            //    Path.Combine(appdata, "QuestionnaireParser", "Scans\\1"), "scan");

            if (Directory.Exists(outputPath))
                imgs = TemplatePdfImages.GetInstance(GetFilesArray(outputPath));
            else throw new DirectoryNotFoundException($"Directory \"{outputPath}\" does not exist");

            Image<Rgb, byte>[] pdfImgs = null;

            var pdfDir = @"C:\Users\virus\AppData\Roaming\QuestionnaireParser\Scans\1";
            if (Directory.Exists(pdfDir))
                pdfImgs = new ScanPdfImages(GetFilesArray(pdfDir)).Images;
            else throw new DirectoryNotFoundException($"Directory \"{pdfDir}\" does not exist");


            var diff = new Matcher().Match(imgs[0], pdfImgs[0]);
            pdfImgs[0] = pdfImgs[0].Rotate(diff.Angle, new Rgb(Color.White));
            pdfImgs[0].WarpAffine()
        }

        private static string[] GetFilesArray(string dirPath)
        {
            return new DirectoryInfo(dirPath)
                .EnumerateFiles()
                .Select(f => f.FullName)
                .ToArray();
        }
    }
}
