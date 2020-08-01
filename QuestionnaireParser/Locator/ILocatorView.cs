using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuestionnaireParser.Locator
{
    interface ILocatorView
    {
        int SelectionHitRadius { get; }
        IEnumerable<Point> Selection { get; set; }

        void UpdatePage(int currentPage, int pagesCount, Image image);
        void UpdateLine(int currentLine);
        void PaintSelection(IEnumerable<Point> selection);
        string SaveDialog();

        event EventHandler PrevPageClick;
        event EventHandler NextPageClick;
        event EventHandler PrevLineClick;
        event EventHandler NextLineClick;
        event EventHandler SaveClick;
        event MouseEventHandler Selecting;
        event EventHandler Scrolling;
    }
}
