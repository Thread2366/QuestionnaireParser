using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Locator
{
    interface ILocatorView
    {
        int SelectionHitRadius { get; }
        IEnumerable<Point> Selection { get; set; }

        void UpdatePage(int currentPage, int pagesCount, Image image);
        void UpdateLine(int currentLine);
        void PaintSelection(IEnumerable<Point> selection);
        string OpenDialog();
        string SaveDialog();
        void ShowHelp();

        event EventHandler SaveClick;
        event EventHandler OpenClick;
        event EventHandler PrevPageClick;
        event EventHandler NextPageClick;
        event EventHandler PrevLineClick;
        event EventHandler NextLineClick;
        event EventHandler HelpClick;
        event MouseEventHandler Selecting;
        event EventHandler Scrolling;
    }
}
