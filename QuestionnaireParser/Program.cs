using AForge.Imaging;
using AForge.Imaging.Filters;
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

namespace QuestionnaireParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test();
            //return;

            Image<Bgr, byte>[] imgs = null;

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

            Image<Bgr, byte>[] pdfImgs = null;

            var pdfDir = @"C:\Users\virus\AppData\Roaming\QuestionnaireParser\Scans\1";
            if (Directory.Exists(pdfDir))
                pdfImgs = new ScanPdfImages(GetFilesArray(pdfDir)).Images;
            else throw new DirectoryNotFoundException($"Directory \"{pdfDir}\" does not exist");


            var diff = new Matcher().Match(imgs[0], pdfImgs[0]);
            //pdfImgs[0] = pdfImgs[0].Rotate(diff.Angle, new Rgb(Color.White));
            //pdfImgs[0].WarpAffine()
        }

        //private static void Test()
        //{
        //    var template = new Bitmap(@"C:\Users\virus\AppData\Roaming\QuestionnaireParser\TemplateImages\template_1.jpg");
        //    var scan = new Bitmap(@"C:\Users\virus\AppData\Roaming\QuestionnaireParser\Scans\1\scan_1.jpg");

        //    var templateCv = new Image<Gray, byte>(template);
        //    CvInvoke.Threshold(templateCv, templateCv, 150, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
        //    var scanCv = new Image<Gray, byte>(scan);
        //    CvInvoke.Threshold(scanCv, scanCv, 150, 255, Emgu.CV.CvEnum.ThresholdType.Binary);

        //    template = templateCv.ToBitmap();
        //    scan = scanCv.ToBitmap();

        //    //var grayscale = new Grayscale(1, 1, 1);
        //    //template = grayscale.Apply(template);
        //    //scan = grayscale.Apply(scan);

        //    //template.Save(@"C:\Users\virus\Desktop\templateGray.jpg");
        //    //scan.Save(@"C:\Users\virus\Desktop\scanGray.jpg");

        //    //var threshold = new BradleyLocalThresholding();
        //    //template = threshold.Apply(template);
        //    //scan = threshold.Apply(scan);

        //    template.Save(@"C:\Users\virus\Desktop\templateBin.jpg");
        //    scan.Save(@"C:\Users\virus\Desktop\scanBin.jpg");

        //    var skewChecker = new DocumentSkewChecker();
        //    var angle = skewChecker.GetSkewAngle(scan);
        //    var rotate = new RotateBilinear(-angle);
        //    scan = rotate.Apply(scan);
        //    scan.Save(@"C:\Users\virus\Desktop\scanBin.jpg");
        //}

        private static string[] GetFilesArray(string dirPath)
        {
            return new DirectoryInfo(dirPath)
                .EnumerateFiles()
                .Select(f => f.FullName)
                .ToArray();
        }
    }
}
