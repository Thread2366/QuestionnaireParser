using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System.Xml.Linq;
using System.Configuration;
using System.Reflection;
using GsUtils;

namespace QuestionnaireParser
{
    class Parser : IDisposable
    {
        const int KernelSize = 5;
        const double BinThreshold = 200;
        const double HoughRho = 1;
        const double HoughThetaDegree = 1;
        const int HoughThreshold = 100;
        const double HoughMinLineLength = 200;
        const double HoughMaxGap = 20;
        const double SkewMaxDeviation = 0.5;
        const int Locality = 150;
        const int PolygonTestDistance = 10;
        const double IntensityThreshold = 20;

        public XElement InputLocations { get; }
        public string ScansFolder { get; }
        public string[] ScansPaths { get; }

        public Parser(XElement inputLocations, string scanPdfPath)
        {
            InputLocations = inputLocations;

            ScansFolder = Path.Combine(
                Environment.GetEnvironmentVariable("temp"),
                "QuestionnaireParser",
                $"Scans_{Guid.NewGuid()}");
            ScansPaths = Gs.PdfToJpeg(scanPdfPath, ScansFolder, "scan");
        }

        public List<List<int>> Parse()
        {
            var scanImgs = ScansPaths.Select(path => new Image<Bgr, byte>(path)).ToArray();
            var scansBin = new Image<Gray, byte>[scanImgs.Length];

            for (int i = 0; i < scansBin.Length; i++)
            {
                var scan = scanImgs[i];
                scan = scan.SmoothGaussian(KernelSize);
                var scanBin = Binarize(scan, BinThreshold).Not();

                scanBin = Deskew(scanBin);

                scansBin[i] = scanBin;
            }
            var checks = FindChecks(scansBin);

            return checks;

            //var result = string.Join(Environment.NewLine, checks.Select((line, i) => $"{i + 1}: {string.Join(",", line)}"));
            //File.WriteAllText(outputPath, result);
        }

        public Image<Gray, byte> Deskew(Image<Gray, byte> image)
        {
            Mat linesImg = new Mat(image.Size, DepthType.Cv8U, 1);
            var lines = CvInvoke.HoughLinesP(image, HoughRho, 
                Math.PI * HoughThetaDegree / 180, HoughThreshold,
                HoughMinLineLength, HoughMaxGap);
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

            var skewAngle = angles.Where(a => Math.Abs(a - median) < SkewMaxDeviation).Average();
            return image.Rotate(skewAngle, new Gray(0));
        }

        public List<List<int>> FindChecks(Image<Gray, byte>[] images)
        {
            var localitySize = new Size(Locality, Locality);
            var locality = new Rectangle(Point.Empty, localitySize);

            var result = new List<List<int>>();
            foreach (var page in InputLocations.Elements("Page"))
            {
                var image = images[int.Parse(page.Attribute("Number").Value)];
                foreach (var line in page.Elements("Line"))
                {
                    var lineResult = new List<int>();
                    var points = line.Elements("Point")
                        .Select(node => new Point(
                            int.Parse(node.Attribute("X").Value),
                            int.Parse(node.Attribute("Y").Value)))
                        .ToArray();
                    for (int i = 0; i < points.Length; i++)
                    {
                        var point = points[i];
                        locality.Location = new Point(point.X - localitySize.Width / 2, point.Y - localitySize.Height / 2);
                        image.ROI = locality;
                        var roi = image.Copy();

                        var contours = new VectorOfVectorOfPoint();
                        var hierarchy = CvInvoke.FindContourTree(roi, contours, ChainApproxMethod.ChainApproxSimple);
                        var arr = contours.ToArrayOfArray();

                        var enclosed = Enumerable.Range(0, hierarchy.GetLength(0))
                            .Where(idx => hierarchy[idx, 2] >= 0);
                        var contourPair = Enumerable
                            .Range(0, hierarchy.GetLength(0))
                            .GroupBy(idx => hierarchy[idx, 3])
                            .Where(gr => gr.Key >= 0)
                            .ToDictionary(gr =>
                                gr.Key,
                                gr => gr.Sum(idx => CvInvoke.ContourArea(contours[idx])))
                            .Aggregate((p1, p2) => p1.Value > p2.Value ? p1 : p2);
                        var contour = contours[contourPair.Key];

                        var ptsInContour = Enumerable.Range(0, roi.Width)
                            .SelectMany(x =>
                                Enumerable.Range(0, roi.Height)
                                .Select(y => new Point(x, y))
                                .Where(p => CvInvoke.PointPolygonTest(contour, p, true) > PolygonTestDistance));
                        if (ptsInContour.Select(p => roi[p].Intensity).DefaultIfEmpty().Average() > IntensityThreshold)
                            lineResult.Add(i + 1);
                    }
                    result.Add(lineResult);
                }
            }
            return result;
        }

        private static Image<Gray, byte> Binarize(Image<Bgr, byte> img, double threshold)
        {
            var gray = img.Convert<Gray, byte>();
            var binary = new Image<Gray, byte>(gray.Width, gray.Height, new Gray(0));
            CvInvoke.Threshold(gray, binary, threshold, 255, ThresholdType.Binary);
            return binary;
        }

        public void Dispose()
        {
            Directory.Delete(ScansFolder, true);
        }
    }
}
