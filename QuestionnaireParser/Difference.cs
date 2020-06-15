using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionnaireParser
{
    class Difference
    {
        public Difference(double angle, double offsetX, double offsetY)
        {
            Angle = angle;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public double Angle { get; }
        public double OffsetX { get; }
        public double OffsetY { get; }
    }
}
