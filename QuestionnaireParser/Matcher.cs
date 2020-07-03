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
using System.Globalization;

namespace QuestionnaireParser
{
    class Matcher
    {
        public Matcher()
        {
        }

        //public Difference Match(Image<Bgr, byte> template, Image<Bgr, byte> scan)
        //{
        //    var viewer = new ImageViewer();

        //    var templateBin = Binarize(template, 150);
        //    var scanBin = Binarize(scan, 150);

        //    templateBin.ROI = new Rectangle(246, 1109, 63, 63);
        //    var roiBin = templateBin.Copy();
        //    templateBin.ROI = Rectangle.Empty;

        //    var templateKeypoints = new VectorOfKeyPoint();
        //    Mat templateDescriptors = new Mat();
        //    var scanKeypoints = new VectorOfKeyPoint();
        //    Mat scanDescriptors = new Mat();

        //    ORBDetector orb = new ORBDetector(300);
        //    orb.DetectAndCompute(roiBin, null, templateKeypoints, templateDescriptors, false);
        //    orb.DetectAndCompute(scanBin, null, scanKeypoints, scanDescriptors, false);

        //    var bfMatcher = new BFMatcher(DistanceType.Hamming);
        //    bfMatcher.Add(templateDescriptors);
        //    var matches = new VectorOfVectorOfDMatch();
        //    bfMatcher.KnnMatch(scanDescriptors, matches, 2, null);
        //    var mdata = MatchesToArray(matches);

        //    var mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
        //    mask.SetTo(new MCvScalar(255));
        //    Features2DToolbox.VoteForUniqueness(matches, 0.8, mask);
        //    int count = Features2DToolbox.VoteForSizeAndOrientation(templateKeypoints, scanKeypoints, matches, mask, 1.5, 20);

        //    Mat matchesView = new Mat();
        //    Features2DToolbox.DrawMatches(templateBin, templateKeypoints, scanBin, scanKeypoints, matches, matchesView,
        //        new MCvScalar(255, 0, 0), new MCvScalar(0, 255, 0), mask);

        //    //viewer.Image = roiBin.ConcateHorizontal(scanBin);
        //    viewer.Image = matchesView;
        //    viewer.ShowDialog();

        //    return null;
        //}

        public Difference Match(Image<Bgr, byte> template, Image<Bgr, byte> scan)
        {
            var templateBin = Binarize(template, 150);
            var scanBin = Binarize(scan, 150);
            //var templateSobel = template.Sobel(1, 1, 3);
            //var scanSobel = scan.Sobel(1, 1, 3);
            //Application.Run(new ResultsForm(templateSobel, scanSobel));
            //Application.Run(new ResultsForm(template, scan));

            var templateKeypoints = new VectorOfKeyPoint();
            Mat templateDescriptors = new Mat();
            var scanKeypoints = new VectorOfKeyPoint();
            Mat scanDescriptors = new Mat();

            ORBDetector surf = new ORBDetector(300);
            surf.DetectAndCompute(templateBin, null, templateKeypoints, templateDescriptors, false);
            surf.DetectAndCompute(scanBin, null, scanKeypoints, scanDescriptors, false);


            var bfMatcher = new BFMatcher(DistanceType.Hamming);
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

            count = VoteForDistance(templateKeypoints, scanKeypoints, matches, ref mask, 100);
            data = mask.GetData();

            var viewer = new ImageViewer();
            Mat matchesView = new Mat();
            Features2DToolbox.DrawMatches(template, templateKeypoints, scan, scanKeypoints, matches, matchesView,
                new MCvScalar(0, 0, 255), new MCvScalar(0, 255, 0), mask);
            viewer.WindowState = FormWindowState.Maximized;
            viewer.Image = matchesView;
            viewer.ShowDialog();

            

            //Mat homography = null;
            //if (count >= 4)
            //{
            //    homography =
            //        Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(
            //            templateKeypoints, scanKeypoints, matches, mask, 5);
            //}

            //if (homography != null)
            //{
            //    Rectangle rect = new Rectangle(Point.Empty, template.Size);
            //    PointF[] pts = new PointF[]
            //    {
            //        new PointF(rect.Left, rect.Bottom),
            //        new PointF(rect.Right, rect.Bottom),
            //        new PointF(rect.Right, rect.Top),
            //        new PointF(rect.Left, rect.Top)
            //    };

            //    pts = CvInvoke.PerspectiveTransform(pts, homography);
            //    Point[] points = Array.ConvertAll(pts, Point.Round);
            //    RotatedRect a = CvInvoke.MinAreaRect(pts);
            //    return new Difference(-a.Angle - 90, pts[3].X, pts[3].Y);
            //}
            return null;
        }

        private static Image<Gray, byte> Binarize(Image<Bgr, byte> img, double threshold)
        {
            var gray = img.Convert<Gray, byte>();
            var binary = new Image<Gray, byte>(gray.Width, gray.Height, new Gray(0));
            CvInvoke.Threshold(gray, binary, threshold, 255, ThresholdType.Binary);
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

        //private static int VoteForRotation(VectorOfKeyPoint modelKeypoints, VectorOfKeyPoint observedKeypoints,
        //    VectorOfVectorOfDMatch matches, Mat mask, float distanceThreshold)
        //{

        //}

        private static int VoteForDistance(VectorOfKeyPoint modelKeypoints, VectorOfKeyPoint observedKeypoints,
            VectorOfVectorOfDMatch matches, ref Mat mask, float distanceThreshold)
        {
            int count = 0;
            var img = mask.ToImage<Gray, byte>();
            for (int i = 0; i < matches.Size; i++)
            {
                if (img[i, 0].Intensity == 0) continue;
                var vec = matches[i];
                var match = vec[0];
                for (int j = 1; j < vec.Size; j++)
                    if (vec[j].Distance < match.Distance) match = vec[j];
                var mKp = modelKeypoints[match.TrainIdx].Point;
                var oKp = observedKeypoints[match.QueryIdx].Point;
                var dx = mKp.X - oKp.X;
                var dy = mKp.Y - oKp.Y;
                var distance = Math.Sqrt(dx * dx + dy * dy);
                if (distance > distanceThreshold) 
                    img[i, 0] = new Gray(0);
                if (img[i, 0].Intensity > 0) 
                    count++;
            }
            mask = img.Mat;
            return count;
        }
    }
}
