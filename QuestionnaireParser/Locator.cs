using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;

namespace QuestionnaireParser
{
    class Locator : Form
    {
        public List<List<List<Point>>> Locations { get; private set; }
        public Image[] TemplateImgs { get; }

        private int currentPage = 0;
        private int currentLine = 0;

        PictureBox pictureBox;
        Button nextPage;
        Button prevPage;
        Label pageNum;
        Button nextLine;
        Button prevLine;
        Label lineNum;
        Button done;

        TableLayoutPanel mainPanel;
        TableLayoutPanel controlPanel;

        public Locator(string templatePdfPath)
        {
            var templateImgsPath = Path.Combine(Environment.GetEnvironmentVariable("temp"), "QuestionnaireTemplates");
            var templates = GsUtils.PdfToJpeg(templatePdfPath, templateImgsPath, "template");
            TemplateImgs = templates.Select(path => Image.FromFile(path)).ToArray();
            if (TemplateImgs.Length == 0) throw new Exception("Invalid template pdf file");
            Locations = new List<List<List<Point>>>() { new List<List<Point>>() };

            pictureBox = new PictureBox() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom };
            prevPage = new Button() { Dock = DockStyle.Fill, Text = "Предыдущая страница" };
            nextPage = new Button() { Dock = DockStyle.Fill, Text = "Следующая страница" };
            pageNum = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Arial", 20) };
            prevLine = new Button() { Dock = DockStyle.Fill, Text = "Предыдущий вопрос" };
            nextLine = new Button() { Dock = DockStyle.Fill, Text = "Следующий вопрос" };
            lineNum = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Arial", 20) };
            done = new Button() { Dock = DockStyle.Fill, Text = "Готово!" };

            int buttonWidth = 100;
            int labelWidth = 50;
            int rowHeight = 50;
            int controlPanelWidth = buttonWidth * 2 + labelWidth;

            mainPanel = new TableLayoutPanel() { Dock = DockStyle.Fill };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            controlPanel = new TableLayoutPanel() { Dock = DockStyle.Top };
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, buttonWidth));
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, labelWidth));
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, buttonWidth));
            controlPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
            controlPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
            controlPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));

            controlPanel.Controls.Add(prevPage, 0, 0);
            controlPanel.Controls.Add(pageNum, 1, 0);
            controlPanel.Controls.Add(nextPage, 2, 0);
            controlPanel.Controls.Add(prevLine, 0, 1);
            controlPanel.Controls.Add(lineNum, 1, 1);
            controlPanel.Controls.Add(nextLine, 2, 1);
            controlPanel.Controls.Add(done, 0, 2);
            controlPanel.SetColumnSpan(done, 3);
            mainPanel.Controls.Add(pictureBox, 0, 0);
            mainPanel.Controls.Add(controlPanel, 1, 0);
            this.Controls.Add(mainPanel);

            UpdateForm();
        }

        public void SaveToXml()
        {
            if (Locations == null) return;
            var xml = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            xml.Add(new XElement("InputLocations",
                Locations.Select((page, i) => new XElement("Page", new XAttribute("Number", i.ToString()),
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

        private void PrevPageClick(object sender, MouseEventArgs e)
        {
            currentPage--;
            if (currentPage == 0) prevPage.Enabled = false;
            if (!nextPage.Enabled) nextPage.Enabled = true;

            pictureBox.Image = TemplateImgs[currentPage];
        }

        private void NextPageClick(object sender, MouseEventArgs e)
        {
            currentPage++;
            if (currentPage >= TemplateImgs.Length - 1) nextPage.Enabled = false;
            if (!prevPage.Enabled) prevPage.Enabled = true;

            pictureBox.Image = TemplateImgs[currentPage];
        }

        private void PrevLineClick(object sender, MouseEventArgs e)
        {
            currentLine--;
            if (currentLine == 0) prevLine.Enabled = false;
        }

        private void NextLineClick(object sender, MouseEventArgs e)
        {
            currentLine++;
            if (!prevLine.Enabled) prevLine.Enabled = true;
        }

        private void UpdateForm()
        {
            if (currentPage > Locations.Count) Locations.Add(new List<List<Point>>());
            prevPage.Enabled = currentPage != 0;
            nextPage.Enabled = currentPage < TemplateImgs.Length - 1;

            if (currentLine > Locations[currentPage].Count) Locations[currentPage].Add(new List<Point>());
            prevLine.Enabled = currentLine != 0;

            pageNum.Text = (currentPage + 1).ToString();
            lineNum.Text = (currentLine + 1).ToString();

            pictureBox.Image = TemplateImgs[currentPage];
        }
    }
}
