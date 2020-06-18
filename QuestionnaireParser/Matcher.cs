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
using Emgu.CV.Features2D;
using Emgu.CV.CvEnum;

namespace QuestionnaireParser
{
    class Matcher
    {
        public Matcher()
        {
        }

        public Difference Match(Image<Rgb, byte> template, Image<Rgb, byte> scan)
        {
            var templateBin = Binarize(template);
            var scanBin = Binarize(scan);
            Application.Run(new ResultsForm(templateBin, scanBin));
            //var templateSobel = template.Sobel(1, 1, 3);
            //var scanSobel = scan.Sobel(1, 1, 3);
            //Application.Run(new ResultsForm(templateSobel, scanSobel));
            Application.Run(new ResultsForm(template, scan));

            var templateKeypoints = new VectorOfKeyPoint();
            Mat templateDescriptors = new Mat();
            var scanKeypoints = new VectorOfKeyPoint();
            Mat scanDescriptors = new Mat();
            
            SURF surf = new SURF(300);
            surf.DetectAndCompute(templateBin, null, templateKeypoints, templateDescriptors, false);
            var d1 = templateDescriptors.GetData();
            surf.DetectAndCompute(scanBin, null, scanKeypoints, scanDescriptors, false);
            var d2 = scanDescriptors.GetData();

            //templateDescriptors.ConvertTo(templateDescriptors, DepthType.Cv8U);
            //d1 = templateDescriptors.GetData();
            //scanDescriptors.ConvertTo(scanDescriptors, DepthType.Cv8U);
            //d2 = scanDescriptors.GetData();

            var bfMatcher = new BFMatcher(DistanceType.L2);
            bfMatcher.Add(templateDescriptors);
            var matches = new VectorOfVectorOfDMatch();
            bfMatcher.KnnMatch(scanDescriptors, matches, 2, null);
            var mdata = MatchesToArray(matches);


            var mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(255));
            var data = mask.GetData();

            Features2DToolbox.VoteForUniqueness(matches, 0.8, mask);
            data = mask.GetData();

            int count = Features2DToolbox.VoteForSizeAndOrientation(templateKeypoints, scanKeypoints, matches, mask, 1.5, 20);
            data = mask.GetData();

            Mat homography = null;
            if (count >= 4)
            {
                homography = 
                    Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(
                        templateKeypoints, scanKeypoints, matches, mask, 5);
            }

            if (homography != null)
            {
                Rectangle rect = new Rectangle(Point.Empty, template.Size);
                PointF[] pts = new PointF[]
                {
                    new PointF(rect.Left, rect.Bottom),
                    new PointF(rect.Right, rect.Bottom),
                    new PointF(rect.Right, rect.Top),
                    new PointF(rect.Left, rect.Top)
                };

                pts = CvInvoke.PerspectiveTransform(pts, homography);
                Point[] points = Array.ConvertAll(pts, Point.Round);
                RotatedRect a = CvInvoke.MinAreaRect(pts);
                return new Difference(-a.Angle - 90, pts[3].X, pts[3].Y);
            }
            return null;
        }

        private static Image<Gray, byte> Binarize(Image<Rgb, byte> img)
        {
            var gray = img.Convert<Gray, byte>();
            var binary = new Image<Gray, byte>(gray.Width, gray.Height, new Gray(0));
            CvInvoke.Threshold(gray, binary, 150, 255, ThresholdType.Binary);
            return binary;
        }

        private static List<MDMatch>[] MatchesToArray(VectorOfVectorOfDMatch matches)
        {
            var result = new List<MDMatch>[matches.Size];
            for (int i = 0; i < matches.Size; i++)
            {
                var vec = matches[i];
                result[i] = new List<MDMatch>();
                for (int j = 0; j < vec.Size; j++)
                {
                    result[i].Add(vec[j]);
                }
            }
            return result;
        }
    }
}
