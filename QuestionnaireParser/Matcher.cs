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
using AForge.Math.Geometry;
using System.Collections.Specialized;

namespace QuestionnaireParser
{
    class Matcher
    {
        public Matcher()
        {
        }

        public void Match(Image<Bgr, byte> template, Image<Bgr, byte> scan)
        {
            var viewer = new ImageViewer();

            scan = scan.SmoothGaussian(5);
            var templateBin = Binarize(template, 200).Not();
            var scanBin = Binarize(scan, 200).Not();
            scanBin.ROI = new Rectangle(0, 0, templateBin.Width, scanBin.Height);
            scanBin = scanBin.Copy();

            scanBin = Deskew(scanBin);

            viewer.WindowState = FormWindowState.Maximized;
            viewer.Image = scanBin.Or(templateBin);
            viewer.ShowDialog();
        }

        public Image<Gray, byte> Deskew(Image<Gray, byte> source)
        {
            Mat linesImg = new Mat(source.Size, DepthType.Cv8U, 1);
            var lines = CvInvoke.HoughLinesP(source, 1, Math.PI / 180, 100, 200, 20);
            for (int i = 0; i < lines.Length; i++)
            {
                CvInvoke.Line(linesImg, lines[i].P1, lines[i].P2, new MCvScalar(255));
            }
            var horizontal = new LineSegment2D(Point.Empty, new Point(source.Width, 0));
            var angles = lines
                .Select(l => l.GetExteriorAngleDegree(horizontal))
                .Select(a => a > 45 ? a - 90 : a);

            //angles = angles
            //    .Where(a => Math.Abs(a) > 0.0000001);

            var count = angles.Count();
            var arr = angles.OrderBy(a => a).ToArray();
            var median = count % 2 == 0 ?
                arr[count / 2] :
                (arr[count / 2] + arr[count / 2 + 1]) / 2;

            var skewAngle = angles.Where(a => Math.Abs(a - median) < 0.5).Average();

            //var pts = Enumerable.Range(0, scanBin.Width)
            //    .SelectMany(x => Enumerable.Range(0, scanBin.Height)
            //        .Select(y => new Point(x, y)))
            //    .Where(p => scanBin[p].Intensity == 255)
            //    .Select(p => (PointF)p)
            //    .ToArray();

            //var rect = CvInvoke.MinAreaRect(pts);
            return source.Rotate(skewAngle, new Gray(0));
        }

        public void IndicateInputs()
        {
            
        }

        //public Difference Match(Image<Bgr, byte> template, Image<Bgr, byte> scan)
        //{
        //    var templateBin = Binarize(template, 150);
        //    var scanBin = Binarize(scan, 150);
        //    //var templateSobel = template.Sobel(1, 1, 3);
        //    //var scanSobel = scan.Sobel(1, 1, 3);
        //    //Application.Run(new ResultsForm(templateSobel, scanSobel));
        //    //Application.Run(new ResultsForm(template, scan));

        //    var templateKeypoints = new VectorOfKeyPoint();
        //    Mat templateDescriptors = new Mat();
        //    var scanKeypoints = new VectorOfKeyPoint();
        //    Mat scanDescriptors = new Mat();

        //    ORBDetector surf = new ORBDetector(300);
        //    surf.DetectAndCompute(templateBin, null, templateKeypoints, templateDescriptors, false);
        //    surf.DetectAndCompute(scanBin, null, scanKeypoints, scanDescriptors, false);


        //    var bfMatcher = new BFMatcher(DistanceType.Hamming);
        //    bfMatcher.Add(templateDescriptors);
        //    var matches = new VectorOfVectorOfDMatch();
        //    bfMatcher.KnnMatch(scanDescriptors, matches, 2, null);
        //    var mdata = MatchesToArray(matches);


        //    var mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
        //    mask.SetTo(new MCvScalar(255));
        //    var data = mask.GetData();

        //    Features2DToolbox.VoteForUniqueness(matches, 0.8, mask);
        //    data = mask.GetData();

        //    int count = Features2DToolbox.VoteForSizeAndOrientation(templateKeypoints, scanKeypoints, matches, mask, 1.5, 20);
        //    data = mask.GetData();

        //    count = VoteForDistance(templateKeypoints, scanKeypoints, matches, ref mask, 100);
        //    data = mask.GetData();

        //    var viewer = new ImageViewer();
        //    Mat matchesView = new Mat();
        //    Features2DToolbox.DrawMatches(template, templateKeypoints, scan, scanKeypoints, matches, matchesView,
        //        new MCvScalar(0, 0, 255), new MCvScalar(0, 255, 0), mask);
        //    viewer.WindowState = FormWindowState.Maximized;
        //    viewer.Image = matchesView;
        //    viewer.ShowDialog();

        //    //Mat homography = null;
        //    //if (count >= 4)
        //    //{
        //    //    homography =
        //    //        Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(
        //    //            templateKeypoints, scanKeypoints, matches, mask, 5);
        //    //}

        //    //if (homography != null)
        //    //{
        //    //    Rectangle rect = new Rectangle(Point.Empty, template.Size);
        //    //    PointF[] pts = new PointF[]
        //    //    {
        //    //        new PointF(rect.Left, rect.Bottom),
        //    //        new PointF(rect.Right, rect.Bottom),
        //    //        new PointF(rect.Right, rect.Top),
        //    //        new PointF(rect.Left, rect.Top)
        //    //    };

        //    //    pts = CvInvoke.PerspectiveTransform(pts, homography);
        //    //    Point[] points = Array.ConvertAll(pts, Point.Round);
        //    //    RotatedRect a = CvInvoke.MinAreaRect(pts);
        //    //    return new Difference(-a.Angle - 90, pts[3].X, pts[3].Y);
        //    //}
        //    return null;
        //}

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
