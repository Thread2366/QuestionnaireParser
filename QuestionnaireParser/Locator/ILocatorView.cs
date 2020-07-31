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
        IEnumerable<Point> Selection { get; set; }

        void UpdatePage(int currentPage, int pagesCount, Image image);
        void UpdateLine(int currentLine);
        void UpdateSelection(IEnumerable<Point> selection);

        event EventHandler PrevPageClick;
        event EventHandler NextPageClick;
        event EventHandler PrevLineClick;
        event EventHandler NextLineClick;
        event MouseEventHandler Selecting;
    }
}
