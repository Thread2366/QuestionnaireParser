using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuestionnaireParser
{
    class TemplatePdfImages
    {
        private static Image<Rgb, byte>[] images;

        private TemplatePdfImages() { }

        public static Image<Rgb, byte>[] GetInstance(string gsPath, string templatePdfPath, string outputPath)
        {
            if (images != null) return images;

            GsUtils.PdfToJpeg(gsPath, templatePdfPath, outputPath, "template");

            images = new DirectoryInfo(outputPath)
                .EnumerateFiles()
                .Select(file =>
                {
                    using (var bmp = new Bitmap(file.FullName))
                        return new Image<Rgb, byte>(bmp);
                })
                .ToArray();

            return images;
        }

        public static Image<Rgb, byte>[] GetInstance(string[] imgPaths)
        {
            if (images != null) return images;

            images = imgPaths.Select(path =>
            {
                using (var bmp = new Bitmap(path))
                    return new Image<Rgb, byte>(bmp);
            })
                .ToArray();

            return images;
        }
    }
}
