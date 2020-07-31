using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuestionnaireParser.Locator
{
    class LocatorPresenter
    {
        private ILocatorView View { get; }
        private LocatorModel Model { get; set; }

        private int currentPage = 0;
        private int currentLine = 0;

        public LocatorPresenter(ILocatorView view, string templatePdfPath)
        {
            if (view == null) throw new ArgumentException("View cannot be null");

            View = view;
            Model = new LocatorModel(templatePdfPath);

            View.PrevPageClick += OnPrevPageClick;
            View.NextPageClick += OnNextPageClick;
            View.PrevLineClick += OnPrevLineClick;
            View.NextLineClick += OnNextLineClick;

            UpdatePage();
            UpdateLine();
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

        private void UpdatePage() => View.UpdatePage(currentPage, Model.TemplateImgs.Length, Model.TemplateImgs[currentPage]);
        private void UpdateLine() => View.UpdateLine(currentLine);
    }
}
