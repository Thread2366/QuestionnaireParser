﻿using Emgu.CV;
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
using System.Configuration;

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

            //Locate(@"\\prominn\RHO\SYNC\iabdullaev\Desktop\Бланк обратной связи АЭХК (2).pdf");
            Parse(@"\\prominn\RHO\SYNC\iabdullaev\Desktop\inputLocations.xml",
                @"\\prominn\RHO\SYNC\iabdullaev\Desktop\Анкета 1.pdf",
                @"\\prominn\RHO\SYNC\iabdullaev\Desktop\result.txt");
        }

        static void Parse(string inputLocationsPath, string scanPath, string outputPath)
        {
            var inputLocations = XElement.Parse(File.ReadAllText(inputLocationsPath));
            var parser = new Parser(inputLocations);
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
