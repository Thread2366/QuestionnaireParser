using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using QuestionnaireParser.Locator;

namespace QuestionnaireParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var locator = new LocatorView(@"\\prominn\RHO\SYNC\iabdullaev\Desktop\Бланк обратной связи АЭХК (2).pdf");
            locator.ShowDialog();

            //LocateInputs();

            //var parser = new Parser(@"inputLocations.xml");
            //parser.Parse(@"\\prominn\RHO\SYNC\iabdullaev\Desktop\Анкета 2.pdf");
        }


        public static void LocateInputs()
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
            xml.Save(@"inputLocations.xml");
        }
    }
}
