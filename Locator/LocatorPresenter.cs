using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Locator
{
    class LocatorPresenter
    {
        private ILocatorView View { get; }
        private LocatorModel Model { get; set; }

        private int currentPage = 0;
        private int currentLine = 0;

        private IEnumerable<Point> SelectedPoints { get => Model.GetPointsLine(currentPage, currentLine); }

        public LocatorPresenter(ILocatorView view)
        {
            if (view == null) throw new ArgumentException("View cannot be null");

            View = view;

            View.PrevPageClick += OnPrevPageClick;
            View.NextPageClick += OnNextPageClick;
            View.PrevLineClick += OnPrevLineClick;
            View.NextLineClick += OnNextLineClick;
            View.Selecting += OnSelecting;
            View.Scrolling += OnScroll;
            View.SaveClick += OnSave;
            View.OpenClick += OnOpen;
            View.HelpClick += OnHelp;
        }

        private void OnPrevPageClick(object sender, EventArgs e)
        {
            if (currentPage > 0) currentPage--;
            UpdatePage();
        }

        private void OnNextPageClick(object sender, EventArgs e)
        {
            if (currentPage + 1 < Model.TemplateImgs.Length) currentPage++;
            UpdatePage();
        }

        private void OnPrevLineClick(object sender, EventArgs e)
        {
            if (currentLine > 0) currentLine--;
            UpdateLine();
        }

        private void OnNextLineClick(object sender, EventArgs e)
        {
            currentLine++;
            UpdateLine();
        }

        private void OnSelecting(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Model.AddPoint(e.Location, currentPage, currentLine);
            }
            else if (e.Button == MouseButtons.Right)
            {
                var toRemove = SelectedPoints
                    .Select(pt => new { Point = pt, Distance = GetPointsDistance(pt, e.Location) })
                    .Where(x => x.Distance < View.SelectionHitRadius)
                    .OrderBy(x => x.Distance)
                    .FirstOrDefault();
                if (toRemove != null) Model.RemovePoint(toRemove.Point, currentPage, currentLine);
            }
            View.PaintSelection(SelectedPoints);
        }

        private void OnScroll(object sender, EventArgs e)
        {
            View.PaintSelection(SelectedPoints);
        }

        private void OnSave(object sender, EventArgs e)
        {
            var savePath = View.SaveDialog();
            if (savePath == null) return;
            Model.SaveToXml(Path.Combine(savePath, "inputLocations.xml"));
        }

        private void OnOpen(object sender, EventArgs e)
        {
            var templatePdfPath = View.OpenDialog();
            if (templatePdfPath == null) return;
            if (Model != null) Model.Dispose();
            Model = new LocatorModel(templatePdfPath);

            currentPage = 0;
            currentLine = 0;
            UpdatePage();
            UpdateLine();
        }

        private void OnHelp(object sender, EventArgs e)
        {
            View.ShowHelp();
        }

        private void UpdatePage()
        {
            View.UpdatePage(currentPage, Model.TemplateImgs.Length, Model.TemplateImgs[currentPage]);
            View.PaintSelection(SelectedPoints);
        }
        private void UpdateLine()
        {
            View.UpdateLine(currentLine);
            View.PaintSelection(SelectedPoints);
        }

        private double GetPointsDistance(Point p1, Point p2)
        {
            var dx = p1.X - p2.X;
            var dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
