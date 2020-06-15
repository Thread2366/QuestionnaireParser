using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.XFeatures2D;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.UI;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.ES11;
using Emgu.CV.Features2D;

namespace QuestionnaireParser
{
    class Matcher
    {
        public Matcher()
        {
        }

        public Difference Match(Image<Rgb, byte> template, Image<Rgb, byte> actual)
        {
            var templatePol = Polarize(template);
            var actualPol = Polarize(actual);
            Application.Run(new ResultsForm(templatePol, actualPol));
            //template = template.Sobel(1, 1, 3).Convert<Rgb, byte>();
            //actual = actual.Sobel(1, 1, 3).Convert<Rgb, byte>();
            Application.Run(new ResultsForm(template, actual));

            var vector1 = new VectorOfKeyPoint();
            var vector2 = new VectorOfKeyPoint();
            
            SURF surf = new SURF(300);
            surf.DetectAndCompute(template, null, vector1, template, false);
            surf.DetectAndCompute(actual, null, vector2, template, false);

            var bfMatcher = new BFMatcher(DistanceType.Hamming);
            bfMatcher.Add(vector1);
            var matches = new VectorOfVectorOfDMatch();
            bfMatcher.KnnMatch(vector2, matches, 3, null);


            var array1 = vector1.ToArray();
            var array2 = vector2.ToArray();

            var angle1 = array1.Average(p => p.Angle);
            var angle2 = array2.Average(p => p.Angle);

            var offsetX1 = array1.Average(p => p.Point.X);
            var offsetX2 = array2.Average(p => p.Point.X);

            var offsetY1 = array1.Average(p => p.Point.Y);
            var offsetY2 = array2.Average(p => p.Point.Y);

            return new Difference(angle2 - angle1, offsetX2 - offsetX1, offsetY2 - offsetY1);
        }

        private static Image<Rgb, byte> Polarize(Image<Rgb, byte> img)
        {
            var sourceData = img.Data;
            var resultData = new byte[img.Height, img.Width, 3];
            for (int x = 0; x < img.Width; x++)
                for (int y = 0; y < img.Height; y++)
                {
                    var gray = Enumerable.Range(0, 3).Average(i => sourceData[y, x, i]);
                    var polarized = gray < 200 ? (byte)0 : (byte)255;
                    resultData[y, x, 0] = polarized;
                    resultData[y, x, 1] = polarized;
                    resultData[y, x, 2] = polarized;
                }
            return new Image<Rgb, byte>(resultData);
        }
    }
}
