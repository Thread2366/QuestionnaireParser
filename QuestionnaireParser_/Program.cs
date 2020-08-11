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
using System.Threading;

namespace QuestionnaireParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //var view = new LocatorView();
            //var locator = new LocatorPresenter(view, @"C:\Users\virus\Desktop\Работа\Задача с анкетами\Бланк обратной связи.pdf");
            //Application.Run(view);

            //LocateInputs();

            Locate(@"C:\Users\virus\Desktop\Работа\Задача с анкетами\Бланк обратной связи.pdf");
            //Parse(@"C:\Users\virus\Desktop\inputLocations.xml", @"C:\Users\virus\Desktop\Работа\Задача с анкетами\Анкета.pdf", @"C:\Users\virus\Desktop\result.txt");
        }

        static void Parse(string inputLocationsPath, string scanPath, string outputPath)
        {
            var parser = new Parser(inputLocationsPath);
            parser.Parse(scanPath, outputPath);
        }

        static void Locate(string templatePath)
        {
            var thread = new Thread(() =>
            {
                var view = new LocatorView();
                var locator = new LocatorPresenter(view, templatePath);
                Application.Run(view);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
    }
}
