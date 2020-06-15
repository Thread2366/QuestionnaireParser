using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuestionnaireParser
{
    public partial class ResultsForm : Form
    {
        ImageBox imgBoxLeft;
        ImageBox imgBoxRight;
        TableLayoutPanel panel;

        public ResultsForm(IImage imgLeft, IImage imgRight)
        {
            this.WindowState = FormWindowState.Maximized;
            imgBoxLeft = new ImageBox() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom };
            imgBoxRight = new ImageBox() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom };
            panel = new TableLayoutPanel() { Dock = DockStyle.Fill };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            panel.Controls.Add(imgBoxLeft, 0, 0);
            panel.Controls.Add(imgBoxRight, 1, 0);

            this.Controls.Add(panel);

            imgBoxLeft.Image = imgLeft;
            imgBoxRight.Image = imgRight;
        }
    }
}
