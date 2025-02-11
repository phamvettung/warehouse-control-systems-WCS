using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intech.FortnaRollerConveyor.WCS.UI.UserControls
{
    public class VerticalConveyor : Panel
    {
        public static readonly int VERTICAL_CONVEYOR_HEIGHT = (int)(1200 / MainConveyor.SCALE);
        public static readonly int VERTICAL_CONVEYOR_WIDTH = (int)(560 / MainConveyor.SCALE);

        public Panel pnStopperTop { get; set; }
        public Panel pnStopperBottom { get; set; }

        private Color Color = Color.LightGray;

        public VerticalConveyor() {

            this.Width = VERTICAL_CONVEYOR_WIDTH;
            this.Height = VERTICAL_CONVEYOR_HEIGHT;
            this.BackColor = Color.Transparent;

            CreateLine();
            CreateRoller();
            CreateStopper();
        }

        private void CreateLine()
        {
            Panel pnLineLeft = new Panel();
            pnLineLeft.BorderStyle = BorderStyle.FixedSingle;
            pnLineLeft.Width = 5;
            pnLineLeft.Height = this.Height;
            pnLineLeft.BackColor = Color.LightGray;
            pnLineLeft.Location = new Point(0, 0);
            this.Controls.Add(pnLineLeft);

            Panel pnLineRight = new Panel();
            pnLineRight.BorderStyle = BorderStyle.FixedSingle;
            pnLineRight.Width = 5;
            pnLineRight.Height = this.Height;
            pnLineRight.BackColor = Color.LightGray;
            pnLineRight.Location = new Point(this.Width - 5, 0);
            this.Controls.Add(pnLineRight);
        }

        private void CreateRoller()
        {
            for (int i = 0; i < 8; i++)
            {
                Panel pnRollor = new Panel();
                pnRollor.BorderStyle = BorderStyle.FixedSingle;
                pnRollor.Location = new Point(10, i * 25 + 5);
                pnRollor.BackColor = SystemColors.ControlLight;
                pnRollor.Width = 448 / MainConveyor.SCALE;
                pnRollor.Height = 70 / MainConveyor.SCALE;
                this.Controls.Add(pnRollor);
            }
        }

        private void CreateStopper()
        {
            pnStopperTop = new Panel();
            pnStopperTop.BorderStyle = BorderStyle.FixedSingle;
            pnStopperTop.Location = new Point(27, 15);
            pnStopperTop.BackColor = Color;
            pnStopperTop.Width = 250 / MainConveyor.SCALE;
            pnStopperTop.Height = 50 / MainConveyor.SCALE;
            this.Controls.Add(pnStopperTop);

            pnStopperBottom = new Panel();
            pnStopperBottom.BorderStyle = BorderStyle.FixedSingle;
            pnStopperBottom.Location = new Point(27, this.Height - 27);
            pnStopperBottom.BackColor = Color;
            pnStopperBottom.Width = 250 / MainConveyor.SCALE;
            pnStopperBottom.Height = 50 / MainConveyor.SCALE;
            this.Controls.Add(pnStopperBottom);
        }

    }
}
