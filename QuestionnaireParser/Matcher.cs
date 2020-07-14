using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
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
using System.Xml.Linq;

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
            //viewer.Image = scanBin.ConcateHorizontal(templateBin);
            //viewer.ShowDialog();
            FindInputs(scanBin, 0);

            viewer.WindowState = FormWindowState.Maximized;
            viewer.Image = scanBin.Or(templateBin);
            viewer.ShowDialog();
        }

        public Image<Gray, byte> Deskew(Image<Gray, byte> image)
        {
            Mat linesImg = new Mat(image.Size, DepthType.Cv8U, 1);
            var lines = CvInvoke.HoughLinesP(image, 1, Math.PI / 180, 100, 200, 20);
            for (int i = 0; i < lines.Length; i++)
            {
                CvInvoke.Line(linesImg, lines[i].P1, lines[i].P2, new MCvScalar(255));
            }
            var horizontal = new LineSegment2D(Point.Empty, new Point(image.Width, 0));
            var angles = lines
                .Select(l => l.GetExteriorAngleDegree(horizontal))
                .Select(a => a > 45 ? a - 90 : a);

            var count = angles.Count();
            var arr = angles.OrderBy(a => a).ToArray();
            var median = count % 2 == 0 ?
                arr[count / 2] :
                (arr[count / 2] + arr[count / 2 + 1]) / 2;

            var skewAngle = angles.Where(a => Math.Abs(a - median) < 0.5).Average();
            return image.Rotate(skewAngle, new Gray(0));
        }

        public Point[] FindInputs(Image<Gray, byte> image, int page)
        {
            var path = @"C:\Users\virus\Source\Repos\QuestionnaireParser\QuestionnaireParser\inputLocations.xml";
            var xml = XElement.Parse(File.ReadAllText(path));

            var originalPoints = xml
                .Elements("Page")
                .First(node => node.Attribute("Number").Value.Equals(page.ToString()))
                .Descendants("Point")
                .Select(node => new Point(int.Parse(node.Attribute("X").Value), int.Parse(node.Attribute("Y").Value)));

            var inpNode = xml.Element("InputSize");
            var rectSize = new Size(int.Parse(inpNode.Attribute("Width").Value), int.Parse(inpNode.Attribute("Height").Value));
            var localitySize = new Size(200, 200);

            var rectPts = Enumerable.Range(0, rectSize.Width)
                .SelectMany(x =>
                    Enumerable.Range(0, rectSize.Height)
                    .Where(y => x == 0 || y == 0 || x == rectSize.Width - 1 || y == rectSize.Height - 1)
                    .Select(y => new Point(x, y)));

            var viewer = new ImageViewer() { WindowState = FormWindowState.Maximized };

            var locality = new Rectangle(Point.Empty, localitySize);
            foreach (var origPt in originalPoints)
            {
                locality.Location = new Point(origPt.X - localitySize.Width / 2, origPt.Y - localitySize.Height / 2);
                image.ROI = locality;
                var roi = image.Copy();

                viewer.Image = roi;
                viewer.ShowDialog();

                var b = new SimpleBlobDetector();
                var points = b.Detect(roi);
            }

            throw new NotImplementedException();
        }

        public void LocateInputs()
        {
            var pts = new Point[][][]
            {
                new Point[][]
                {
                    new Point[]
                    {
                        new Point(276, 1140),
                        new Point(620, 1140),
                        new Point(964, 1140),
                        new Point(1308, 1140),
                        new Point(1652, 1140),
                        new Point(1996, 1140)
                    },
                    new Point[]
                    {
                        new Point(276, 1470),
                        new Point(620, 1470),
                        new Point(964, 1470),
                        new Point(1308, 1470),
                        new Point(1652, 1470),
                        new Point(1996, 1470)
                    },
                    new Point[]
                    {
                        new Point(276, 1800),
                        new Point(620, 1800),
                        new Point(964, 1800),
                        new Point(1308, 1800),
                        new Point(1652, 1800),
                        new Point(1996, 1800)
                    },
                    new Point[]
                    {
                        new Point(276, 2134),
                        new Point(620, 2134),
                        new Point(964, 2134),
                        new Point(1308, 2134),
                        new Point(1652, 2134),
                        new Point(1996, 2134)
                    },
                    new Point[]
                    {
                        new Point(276, 2464),
                        new Point(620, 2464),
                        new Point(964, 2464),
                        new Point(1308, 2464),
                        new Point(1652, 2464),
                        new Point(1996, 2464)
                    },
                    new Point[]
                    {
                        new Point(276, 2794),
                        new Point(964, 2794),
                        new Point(1652, 2794)
                    },
                    new Point[]
                    {
                        new Point(276, 3126),
                        new Point(964, 3126),
                        new Point(1652, 3126)
                    },
                },
                new Point[][]
                {
                    new Point[]
                    {
                        new Point(276, 400),
                        new Point(620, 400),
                        new Point(964, 400),
                        new Point(1308, 400),
                        new Point(1652, 400),
                        new Point(1996, 400)
                    },
                    new Point[]
                    {
                        new Point(276, 732),
                        new Point(620, 732),
                        new Point(964, 732),
                        new Point(1308, 732),
                        new Point(1652, 732),
                        new Point(1996, 732)
                    },
                    new Point[]
                    {
                        new Point(276, 1064),
                        new Point(620, 1064),
                        new Point(964, 1064),
                        new Point(1308, 1064),
                        new Point(1652, 1064),
                        new Point(1996, 1064)
                    },
                    new Point[]
                    {
                        new Point(276, 1464),
                        new Point(620, 1464),
                        new Point(964, 1464),
                        new Point(1308, 1464),
                        new Point(1652, 1464),
                        new Point(1996, 1464)
                    },
                    new Point[]
                    {
                        new Point(276, 1794),
                        new Point(620, 1794),
                        new Point(964, 1794),
                        new Point(1308, 1794),
                        new Point(1652, 1794),
                        new Point(1996, 1794)
                    },
                    new Point[]
                    {
                        new Point(1308, 2054),
                        new Point(1652, 2054),
                        new Point(1996, 2054)
                    },
                    new Point[]
                    {
                        new Point(276, 2340),
                        new Point(464, 2340),
                        new Point(652, 2340),
                        new Point(840, 2340),
                        new Point(1028, 2340),
                        new Point(1216, 2340),
                        new Point(1404, 2340),
                        new Point(1592, 2340),
                        new Point(1780, 2340),
                        new Point(1968, 2340),
                        new Point(2156, 2340)
                    },
                }
            };

            var xml = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            xml.Add(new XElement("InputLocations",
                pts.Select((page, i) => new XElement("Page", new XAttribute("Number", i.ToString()),
                    page.Select(line => new XElement("Line",
                        line.Select(point => new XElement("Point", 
                            new XAttribute("X", point.X.ToString()), 
                            new XAttribute("Y", point.Y.ToString())
                        ))
                    ))
                )), 
                new XElement("InputSize", 
                    new XAttribute("Width", 48), 
                    new XAttribute("Height", 48))
            ));
            xml.Save(@"..\..\inputLocations.xml");
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
