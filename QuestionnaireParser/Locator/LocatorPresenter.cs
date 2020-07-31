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
        private ILocatorModel Model { get; }

        public LocatorPresenter(ILocatorView view, ILocatorModel model)
        {
            if (view == null) throw new ArgumentException("View cannot be null");
            if (model == null) throw new ArgumentException("Model cannot be null");
        }

        public void PagingHandler(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            PagingButton buttonType;
            if (button.Tag is PagingButton) buttonType = (PagingButton)button.Tag;
            else return;

            switch (buttonType)
            {
                case PagingButton.PrevPage:
                    currentPage--;
                    break;
                case PagingButton.NextPage:
                    currentPage++;
                    break;
                case PagingButton.PrevLine:
                    currentLine--;
                    break;
                case PagingButton.NextLine:
                    currentLine++;
                    break;
            }

            UpdateForm();
        }
    }
}
