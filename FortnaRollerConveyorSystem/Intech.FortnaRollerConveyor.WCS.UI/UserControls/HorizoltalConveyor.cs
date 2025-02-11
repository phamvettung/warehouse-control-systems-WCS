using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intech.FortnaRollerConveyor.WCS.UI.UserControls
{
    public class HorizoltalConveyor : Panel
    {
        public static readonly int MAIN_CONVEYOR_HEIGHT = (int)(560 / MainConveyor.SCALE);
        public static readonly int MAIN_CONVEYOR_WIDTH = (int)(1800 / MainConveyor.SCALE);

        public Panel pnStopper { get; set; }
        public Panel pnPusherLeftA { get; set; }
        public Panel pnPusherLeftB { get; set; }
        public Panel pnPusherLeftC { get; set; }

        public Panel pnPusherRightA { get; set; }
        public Panel pnPusherRightB { get; set; }
        public Panel pnPusherRightC { get; set; }

        private Color Color = Color.LightGray;

        public HorizoltalConveyor() 
        {
            this.Width = MAIN_CONVEYOR_WIDTH;
            this.Height = MAIN_CONVEYOR_HEIGHT;
            this.BackColor = Color.Transparent;

            CreateLine();
            CreateRoller();
            CreateStopper();
            CreatePusher();
        }

        public void CreateLine()
        {
            Panel pnLineTop = new Panel();
            pnLineTop.BorderStyle = BorderStyle.FixedSingle;
            pnLineTop.Width = this.Width;
            pnLineTop.Height = 5;
            pnLineTop.BackColor = Color.LightGray;
            pnLineTop.Location = new Point(0, 0);
            this.Controls.Add(pnLineTop);

            Panel pnLineBottom = new Panel();
            pnLineBottom.BorderStyle = BorderStyle.FixedSingle;
            pnLineBottom.Width = this.Width;
            pnLineBottom.Height = 5;
            pnLineBottom.BackColor = Color.LightGray;
            pnLineBottom.Location = new Point(0, this.Height - 5);
            this.Controls.Add(pnLineBottom);

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
            for (int i = 0; i < 12; i++)
            {
                Panel pnRollor = new Panel();
                pnRollor.BorderStyle = BorderStyle.FixedSingle;
                pnRollor.Location = new Point(1 + i * 25 + 5, 10);
                pnRollor.BackColor = SystemColors.ControlLight;
                pnRollor.Width = 70 / MainConveyor.SCALE;
                pnRollor.Height = 448 / MainConveyor.SCALE;
                this.Controls.Add(pnRollor);
            }
        }

        private void CreateStopper()
        {
            pnStopper = new Panel();
            pnStopper.BorderStyle = BorderStyle.FixedSingle;
            pnStopper.Location = new Point(MainConveyor.L560, 27);
            pnStopper.BackColor = Color;
            pnStopper.Width = 50 / MainConveyor.SCALE;
            pnStopper.Height = 250 / MainConveyor.SCALE;
            this.Controls.Add(pnStopper);
        }
        private void CreatePusher()
        {
            pnPusherLeftA = new Panel();
            pnPusherLeftA.BorderStyle = BorderStyle.FixedSingle;
            pnPusherLeftA.Location = new Point(20, 23);
            pnPusherLeftA.BackColor = Color;
            pnPusherLeftA.Width = 50 / MainConveyor.SCALE;
            pnPusherLeftA.Height = 300 / MainConveyor.SCALE;
            this.Controls.Add(pnPusherLeftA);

            pnPusherLeftB = new Panel();
            pnPusherLeftB.BorderStyle = BorderStyle.FixedSingle;
            pnPusherLeftB.Location = new Point(20 + 25, 23);
            pnPusherLeftB.BackColor = Color;
            pnPusherLeftB.Width = 50 / MainConveyor.SCALE;
            pnPusherLeftB.Height = 300 / MainConveyor.SCALE;
            this.Controls.Add(pnPusherLeftB);

            pnPusherLeftC = new Panel();
            pnPusherLeftC.BorderStyle = BorderStyle.FixedSingle;
            pnPusherLeftC.Location = new Point(20 + 50, 23);
            pnPusherLeftC.BackColor = Color;
            pnPusherLeftC.Width = 50 / MainConveyor.SCALE;
            pnPusherLeftC.Height = 300 / MainConveyor.SCALE;
            this.Controls.Add(pnPusherLeftC);

            pnPusherRightA = new Panel();
            pnPusherRightA.BorderStyle = BorderStyle.FixedSingle;
            pnPusherRightA.Location = new Point(MainConveyor.L560 + MainConveyor.L680 + 15, 23);
            pnPusherRightA.BackColor = Color;
            pnPusherRightA.Width = 50 / MainConveyor.SCALE;
            pnPusherRightA.Height = 300 / MainConveyor.SCALE;
            this.Controls.Add(pnPusherRightA);

            pnPusherRightB = new Panel();
            pnPusherRightB.BorderStyle = BorderStyle.FixedSingle;
            pnPusherRightB.Location = new Point(MainConveyor.L560 + MainConveyor.L680 + 15 + 25, 23);
            pnPusherRightB.BackColor = Color;
            pnPusherRightB.Width = 50 / MainConveyor.SCALE;
            pnPusherRightB.Height = 300 / MainConveyor.SCALE;
            this.Controls.Add(pnPusherRightB);

            pnPusherRightC = new Panel();
            pnPusherRightC.BorderStyle = BorderStyle.FixedSingle;
            pnPusherRightC.Location = new Point(MainConveyor.L560 + MainConveyor.L680 + 15 + 50, 23);
            pnPusherRightC.BackColor = Color;
            pnPusherRightC.Width = 50 / MainConveyor.SCALE;
            pnPusherRightC.Height = 300 / MainConveyor.SCALE;
            this.Controls.Add(pnPusherRightC);

        }

    }
}
