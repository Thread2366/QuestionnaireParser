using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionnaireParser.Locator
{
    interface ILocatorModel
    {
        void AddPoint(Point point, int page, int line);
        void RemovePoint(Point point, int page, int line);
    }
}
