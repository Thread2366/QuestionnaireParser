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

namespace QuestionnaireParser.Locator
{
    class LocatorView : Form, ILocatorView
    {
        PictureBox pictureBox;
        Button nextPage;
        Button prevPage;
        Label pageNum;
        Button nextLine;
        Button prevLine;
        Label lineNum;
        Button done;

        Panel picturePanel;
        TableLayoutPanel mainPanel;
        TableLayoutPanel controlPanel;

        Pen selectionPen = new Pen(Color.FromArgb(128, Color.Red));
        Size selectionSize = new Size(50, 50);

        public IEnumerable<Point> Selection { get; set; }

        public event EventHandler PrevPageClick;
        public event EventHandler NextPageClick;
        public event EventHandler PrevLineClick;
        public event EventHandler NextLineClick;
        public event MouseEventHandler Selecting;

        public LocatorView()
        {
            Initialize();
        }

        private void Initialize()
        {
            int controlHeight = 50;
            int buttonWidth = 100;
            int labelWidth = controlHeight;

            this.WindowState = FormWindowState.Maximized;

            pictureBox = new PictureBox() { SizeMode = PictureBoxSizeMode.AutoSize };

            prevPage = new Button() { Dock = DockStyle.Fill, Text = "Предыдущая страница", Tag = PagingButton.PrevPage, Enabled = false };
            nextPage = new Button() { Dock = DockStyle.Fill, Text = "Следующая страница", Tag = PagingButton.NextPage, Enabled = false };
            prevLine = new Button() { Dock = DockStyle.Fill, Text = "Предыдущий вопрос", Tag = PagingButton.PrevLine, Enabled = false };
            nextLine = new Button() { Dock = DockStyle.Fill, Text = "Следующий вопрос", Tag = PagingButton.NextLine, Enabled = false };

            pageNum = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Arial", 20) };
            lineNum = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Arial", 20) };

            done = new Button() { Dock = DockStyle.Fill, Text = "Готово!" };

            picturePanel = new Panel() { Dock = DockStyle.Fill, AutoScroll = true };
            controlPanel = new TableLayoutPanel() { Dock = DockStyle.Fill };
            mainPanel = new TableLayoutPanel() { Dock = DockStyle.Fill };

            this.Controls.Add(mainPanel);

            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, controlHeight));
            mainPanel.Controls.Add(picturePanel, 0, 0);
            mainPanel.Controls.Add(controlPanel, 0, 1);

            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, buttonWidth));
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, labelWidth));
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, buttonWidth));
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, buttonWidth));
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, buttonWidth));
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, labelWidth));
            controlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, buttonWidth));
            controlPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            controlPanel.Controls.Add(prevPage, 0, 0);
            controlPanel.Controls.Add(pageNum, 1, 0);
            controlPanel.Controls.Add(nextPage, 2, 0);
            controlPanel.Controls.Add(new Panel(), 3, 0);
            controlPanel.Controls.Add(done, 4, 0);
            controlPanel.Controls.Add(new Panel(), 5, 0);
            controlPanel.Controls.Add(prevLine, 6, 0);
            controlPanel.Controls.Add(lineNum, 7, 0);
            controlPanel.Controls.Add(nextLine, 8, 0);

            picturePanel.Controls.Add(pictureBox);

            prevPage.Click += (sender, e) => PrevPageClick(sender, e);
            nextPage.Click += (sender, e) => NextPageClick(sender, e);
            prevLine.Click += (sender, e) => PrevLineClick(sender, e);
            nextLine.Click += (sender, e) => NextLineClick(sender, e);
            pictureBox.MouseClick += (sender, e) => Selecting(sender, e);
        }

        public void UpdatePage(int currentPage, int pagesCount, Image image)
        {
            prevPage.Enabled = currentPage != 0;
            nextPage.Enabled = currentPage + 1 < pagesCount;

            pageNum.Text = currentPage.ToString();
            pictureBox.Image = image;
            UpdateSelection(Enumerable.Empty<Point>());
        }

        public void UpdateLine(int currentLine)
        {
            prevLine.Enabled = currentLine != 0;
            nextLine.Enabled = true;

            lineNum.Text = currentLine.ToString();
            UpdateSelection(Enumerable.Empty<Point>());
        }

        public void UpdateSelection(IEnumerable<Point> selection)
        {
            Graphics graphics;

            if (selection.Any()) graphics = pictureBox.CreateGraphics();
            else
            {
                pictureBox.Invalidate();
                return;
            }

            foreach (var pt in selection)
            {
                var selectionRect = new Rectangle(pt, selectionSize);
                graphics.DrawEllipse(selectionPen, selectionRect);
                pictureBox.Invalidate(selectionRect);
            }
        }
    }
}
