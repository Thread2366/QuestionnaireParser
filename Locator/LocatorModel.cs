﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utils;

namespace Locator
{
    class LocatorModel : IDisposable
    {
        private Dictionary<int, List<Point>>[] Locations { get; set; }
        public Image[] TemplateImgs { get; }
        public string TemplatePdfPath { get; }

        public LocatorModel(string templatePdfPath)
        {
            TemplatePdfPath = templatePdfPath;
            using (var templates = Gs.PdfToJpeg(TemplatePdfPath, $"Templates_{Guid.NewGuid()}", "template"))
            {
                TemplateImgs = templates.Files.Select(path =>
                {
                    using (var stream = File.OpenRead(path))
                    {
                        return Image.FromStream(stream);
                    }
                }).ToArray();
            }
            if (TemplateImgs.Length == 0) throw new Exception("Invalid template pdf file");
            Locations = new Dictionary<int, List<Point>>[TemplateImgs.Length];
            for (int i = 0; i < Locations.Length; i++) Locations[i] = new Dictionary<int, List<Point>>();
        }

        public void AddPoint(Point point, int page, int line)
        {
            var len = TemplateImgs.Length;
            if (page > len) throw new ArgumentException($"Page {page} does not exist. There are only {len} pages in the template");
            if (!Locations[page].ContainsKey(line)) Locations[page].Add(line, new List<Point>());
            Locations[page][line].Add(point);
        }

        public void RemovePoint(Point point, int page, int line)
        {
            var len = TemplateImgs.Length;
            if (page > len) throw new ArgumentException($"Page {page} does not exist. There are only {len} pages in the template");
            if (!Locations[page].ContainsKey(line)) throw new ArgumentException($"Cannot delete from empty line {line}");
            if (!Locations[page][line].Contains(point)) throw new ArgumentException($"Point {point} does not exist");
            Locations[page][line].Remove(point);
            if (Locations[page][line].Count == 0) Locations[page].Remove(line);
        }

        public List<Point> GetPointsLine(int page, int line)
        {
            var len = TemplateImgs.Length;
            if (page > len) throw new ArgumentException($"Page {page} does not exist. There are only {len} pages in the template");
            if (!Locations[page].ContainsKey(line)) return new List<Point>();
            return Locations[page][line];
        }

        public void SaveToXml(string savePath)
        {
            if (Locations == null) return;
            var xml = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            xml.Add(new XElement("InputLocations",
                Locations.Select((page, i) => new XElement("Page", new XAttribute("Number", i.ToString()),
                    page.Select(line => new XElement("Line", new XAttribute("Number", line.Key),
                        line.Value.Select(point => new XElement("Point",
                            new XAttribute("X", point.X.ToString()),
                            new XAttribute("Y", point.Y.ToString())
                        ))
                    ))
                ))
            ));
            xml.Save(savePath);
        }

        public void Dispose()
        {
            foreach (var img in TemplateImgs)
            {
                img.Dispose();
            }
        }
    }
}
