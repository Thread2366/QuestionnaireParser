using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OpenCvSharp;
using System.Xml.Linq;
using System.Configuration;
using System.Reflection;
using System.Threading;
using Utils;

namespace QuestionnaireParser
{
    class Parser
    {
        const int KernelSize = 5;
        const double GaussianSigma = 2;
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

        private XElement InputLocations { get; }

        public Parser(XElement inputLocations)
        {
            InputLocations = new XElement(inputLocations);
        }

        public List<List<int>> Parse(string scanPdfPath)
        {
            Mat[] scansBin;
            using (var scansPath = Gs.PdfToJpeg(scanPdfPath, $"Scans_{Guid.NewGuid()}", "scan"))
            {
                scansBin = scansPath.Files
                    .Select(path => Cv2.ImRead(path, ImreadModes.Grayscale))
                    .Select(mat => mat.GaussianBlur(
                        new Size(KernelSize, KernelSize),
                        GaussianSigma,
                        GaussianSigma))
                    .Select(mat => mat.Threshold(BinThreshold, 255, ThresholdTypes.Binary))
                    .ToArray();
            }

            for (int i = 0; i < scansBin.Length; i++)
            {
                var scan = scansBin[i];
                Cv2.BitwiseNot(scan, scan);

                Deskew(scan);
            }
            var checks = FindChecks(scansBin);

            return checks;
        }

        public void Deskew(Mat image)
        {
            var lines = Cv2.HoughLinesP(image, HoughRho,
                Math.PI * HoughThetaDegree / 180, HoughThreshold,
                HoughMinLineLength, HoughMaxGap);
            var angles = lines
                .Select(l => Math.Atan((double)(l.P2.Y - l.P1.Y)/(l.P2.X - l.P1.X)) * 180 / Math.PI);


            var count = angles.Count();
            var arr = angles.OrderBy(a => a).ToArray();
            var median = count % 2 == 0 ?
                arr[count / 2] :
                (arr[count / 2] + arr[count / 2 + 1]) / 2;

            var skewAngle = angles.Where(a => Math.Abs(a - median) < SkewMaxDeviation).Average();
            Rotate(image, skewAngle);
        }

        public List<List<int>> FindChecks(Mat[] images)
        {
            var localitySize = new Size(Locality, Locality);
            var locality = new Rect(new Point(0, 0), localitySize);

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
                        if (IsChecked(image, points[i]))
                            lineResult.Add(i + 1);
                    }
                    result.Add(lineResult);
                }
            }
            return result;
        }

        public bool IsChecked(Mat image, Point point)
        {
            var x = point.X - Locality / 2;
            var y = point.Y - Locality / 2;
            var localityRect = new Rect(
                x < 0 ? 0 : x, 
                y < 0 ? 0 : y, 
                Locality, 
                Locality);

            var roi = new Mat(image, localityRect); 
            
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(roi, out contours, out hierarchy,
                RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            var largestContourIdx = FindLargestContourIndex(contours, hierarchy);
            if (largestContourIdx < 0) return false;
            var contour = contours[largestContourIdx];

            return GetEnclosedAverageIntensity(roi, contour) > IntensityThreshold;
        }

        private double GetEnclosedAverageIntensity(Mat image, Point[] contour)
        {
            return Enumerable.Range(0, image.Width)
                .SelectMany(x => Enumerable.Range(0, image.Height)
                    .Select(y => new Point(x, y)))
                .Where(p => Cv2.PointPolygonTest(contour, p, true) > PolygonTestDistance)
                .Select(p => (int)image.Get<byte>(p.Y, p.X))
                .DefaultIfEmpty(-1)
                .Average();
        }

        private int FindLargestContourIndex(Point[][] contours, HierarchyIndex[] hierarchy)
        {
            var contoursDict = Enumerable
                .Range(0, hierarchy.Length)
                .GroupBy(idx => hierarchy[idx].Parent)
                .Where(gr => gr.Key >= 0)
                .ToDictionary(gr =>
                    gr.Key,
                    gr => gr.Sum(idx => Cv2.ContourArea(contours[idx])));

            if (contoursDict.Count == 0) return -1;

            return contoursDict
                .Aggregate((p1, p2) => p1.Value > p2.Value ? p1 : p2)
                .Key;
        }

        private void Rotate(Mat src, double angle)
        {
            var center = new Point2f(src.Width / 2f, src.Height / 2f);
            var rotationMatrix = Cv2.GetRotationMatrix2D(center, angle, 1);
            Cv2.WarpAffine(src, src, rotationMatrix, src.Size());
        }
    }
}
