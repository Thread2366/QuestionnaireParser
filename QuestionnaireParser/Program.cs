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
            //var gsPath = @"C:\Program Files\gs\gs9.52\bin\gswin64c";
            ////imgs = TemplatePdfImages.GetInstance(gsPath, templatePath, outputPath);

            //GsUtils.PdfToJpeg(gsPath, @"C:\Users\virus\Desktop\Работа\Задача с анкетами\Анкета.pdf",
            //    Path.Combine(appdata, "QuestionnaireParser", "ActualImages\\1"), "actual");

            if (Directory.Exists(outputPath))
                imgs = TemplatePdfImages.GetInstance(GetFilesArray(outputPath));
            else throw new DirectoryNotFoundException($"Directory \"{outputPath}\" does not exist");

            Image<Rgb, byte>[] pdfImgs = null;
            //Mat[] pdfImgs = null;

            var pdfDir = @"C:\Users\virus\AppData\Roaming\QuestionnaireParser\ActualImages\1";
            if (Directory.Exists(pdfDir))
                pdfImgs = new ScanPdfImages(GetFilesArray(pdfDir)).Images;
            else throw new DirectoryNotFoundException($"Directory \"{pdfDir}\" does not exist");


            var diff = new Matcher().Match(imgs[0], pdfImgs[0]);
            pdfImgs[0] = pdfImgs[0].Rotate(-1, new Rgb(Color.White));
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
