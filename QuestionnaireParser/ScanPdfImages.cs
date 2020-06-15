using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionnaireParser
{
    class ScanPdfImages
    {
        public Image<Rgb, byte>[] Images { get; }
        //public Mat[] Images { get; }
        
        public ScanPdfImages(string[] imgPaths)
        {
            Images = imgPaths.Select(path =>
            {
                using (var bmp = new Bitmap(path))
                    return new Image<Rgb, byte>(bmp);
            })
                .ToArray();

            //Images = imgPaths.Select(path => new Mat(path, LoadImageType.Unchanged)).ToArray();
        }
        
    }
}
