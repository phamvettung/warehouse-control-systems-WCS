using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intech.FortnaRollerConveyor.WCS.UI.UserControls
{
    internal class MainConveyor : Panel
    {
        public static readonly int SCALE = 6;
        public static readonly int MAIN_CONVEYOR_HEIGHT = (int)(560 / SCALE);
        public static readonly int MAIN_CONVEYOR_WIDTH = (int)(7130 / SCALE);
        public static readonly int START_X = 550; 
        public static readonly int START_Y = 130;
        public static readonly int L916 = 960 / SCALE;
        public static readonly int L1800 = 1800 / SCALE;
        public static readonly int L1000 = 1000 / SCALE;
        public static readonly int L560 = 560 / SCALE;
        public static readonly int L1200 = 1200 / SCALE;
        public static readonly int L680 = 680 / SCALE;

        public Panel pnStopperRight { get; set; }
        public Panel pnStopperLeft { get; set; }
        public Panel pnPusher08a { get; set; }
        public Panel pnPusher08b { get; set; }
        public Panel pnPusher08c { get; set; }
        public Panel pnPusher05a { get; set; }
        public Panel pnPusher05b { get; set; }
        public Panel pnPusher05c { get; set; }
        public Panel pnPusher04a { get; set; }
        public Panel pnPusher04b { get; set; }
        public Panel pnPusher04c { get; set; }
        public Panel pnPusher01a { get; set; }
        public Panel pnPusher01b { get; set; }
        public Panel pnPusher01c { get; set; }

        private Color Color = Color.LightGray;
        public MainConveyor()
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
        }

        private void CreateRoller()
        {
            for (int i = 0; i < 48; i++)
            {
                Panel pnRollor = new Panel();
                pnRollor.BorderStyle = BorderStyle.FixedSingle;
                pnRollor.Location = new Point(1 + i * 25, 10);
                pnRollor.BackColor = SystemColors.ControlLight;
                pnRollor.Width = 70/SCALE;
                pnRollor.Height = 448/SCALE;
                this.Controls.Add(pnRollor);
            }
        }

        private void CreateStopper()
        {
            pnStopperRight = new Panel();
            pnStopperRight.BorderStyle = BorderStyle.FixedSingle;
            pnStopperRight.Location = new Point((int)(L916 + L1800 + L1000 + L560 + L680) - 15, 27);
            pnStopperRight.BackColor = Color;
            pnStopperRight.Width = 50 / SCALE;
            pnStopperRight.Height = 250 / SCALE;
            this.Controls.Add(pnStopperRight);

            pnStopperLeft = new Panel();
            pnStopperLeft.BorderStyle = BorderStyle.FixedSingle;
            pnStopperLeft.Location = new Point((int)(L916 + L560 + L680), 27);
            pnStopperLeft.BackColor = Color;
            pnStopperLeft.Width = 50 / SCALE;
            pnStopperLeft.Height = 250 / SCALE;
            this.Controls.Add(pnStopperLeft);
        }

        private void CreatePusher() {
            pnPusher08a = new Panel();
            pnPusher08a.BorderStyle = BorderStyle.FixedSingle;
            pnPusher08a.Location = new Point(L916 + 7, 23);
            pnPusher08a.BackColor = Color;
            pnPusher08a.Width = 50 / SCALE;
            pnPusher08a.Height = 300 / SCALE;
            this.Controls.Add(pnPusher08a);

            pnPusher08b = new Panel();
            pnPusher08b.BorderStyle = BorderStyle.FixedSingle;
            pnPusher08b.Location = new Point(L916 + 7 + 25, 23);
            pnPusher08b.BackColor = Color;
            pnPusher08b.Width = 50 / SCALE;
            pnPusher08b.Height = 300 / SCALE;
            this.Controls.Add(pnPusher08b);

            pnPusher08c = new Panel();
            pnPusher08c.BorderStyle = BorderStyle.FixedSingle;
            pnPusher08c.Location = new Point(L916 + 7 + 50, 23);
            pnPusher08c.BackColor = Color;
            pnPusher08c.Width = 50 / SCALE;
            pnPusher08c.Height = 300 / SCALE;
            this.Controls.Add(pnPusher08c);

            pnPusher05a = new Panel();
            pnPusher05a.BorderStyle = BorderStyle.FixedSingle;
            pnPusher05a.Location = new Point(L916 +L560 + L680 + 23, 23);
            pnPusher05a.BackColor = Color;
            pnPusher05a.Width = 50 / SCALE;
            pnPusher05a.Height = 300 / SCALE;
            this.Controls.Add(pnPusher05a);

            pnPusher05b = new Panel();
            pnPusher05b.BorderStyle = BorderStyle.FixedSingle;
            pnPusher05b.Location = new Point(L916 + L560 + L680 + 23 + 25, 23);
            pnPusher05b.BackColor = Color;
            pnPusher05b.Width = 50 / SCALE;
            pnPusher05b.Height = 300 / SCALE;
            this.Controls.Add(pnPusher05b);

            pnPusher05c = new Panel();
            pnPusher05c.BorderStyle = BorderStyle.FixedSingle;
            pnPusher05c.Location = new Point(L916 + L560 + L680 + 23 + 50, 23);
            pnPusher05c.BackColor = Color;
            pnPusher05c.Width = 50 / SCALE;
            pnPusher05c.Height = 300 / SCALE;
            this.Controls.Add(pnPusher05c);

            pnPusher04a = new Panel();
            pnPusher04a.BorderStyle = BorderStyle.FixedSingle;
            pnPusher04a.Location = new Point(L916 + L1800 + L1000 + 15, 23);
            pnPusher04a.BackColor = Color;
            pnPusher04a.Width = 50 / SCALE;
            pnPusher04a.Height = 300 / SCALE;
            this.Controls.Add(pnPusher04a);

            pnPusher04b = new Panel();
            pnPusher04b.BorderStyle = BorderStyle.FixedSingle;
            pnPusher04b.Location = new Point(L916 + L1800 + L1000 + 15 + 25, 23);
            pnPusher04b.BackColor = Color;
            pnPusher04b.Width = 50 / SCALE;
            pnPusher04b.Height = 300 / SCALE;
            this.Controls.Add(pnPusher04b);

            pnPusher04c = new Panel();
            pnPusher04c.BorderStyle = BorderStyle.FixedSingle;
            pnPusher04c.Location = new Point(L916 + L1800 + L1000 + 15 + 50, 23);
            pnPusher04c.BackColor = Color;
            pnPusher04c.Width = 50 / SCALE;
            pnPusher04c.Height = 300 / SCALE;
            this.Controls.Add(pnPusher04c);

            pnPusher01a = new Panel();
            pnPusher01a.BorderStyle = BorderStyle.FixedSingle;
            pnPusher01a.Location = new Point(L916 + L1800 + L1000 + L560 + L680 + 10, 23);
            pnPusher01a.BackColor = Color;
            pnPusher01a.Width = 50 / SCALE;
            pnPusher01a.Height = 300 / SCALE;
            this.Controls.Add(pnPusher01a);

            pnPusher01b = new Panel();
            pnPusher01b.BorderStyle = BorderStyle.FixedSingle;
            pnPusher01b.Location = new Point(L916 + L1800 + L1000 + L560 + L680 + 10 + 25, 23);
            pnPusher01b.BackColor = Color;
            pnPusher01b.Width = 50 / SCALE;
            pnPusher01b.Height = 300 / SCALE;
            this.Controls.Add(pnPusher01b);

            pnPusher01c = new Panel();
            pnPusher01c.BorderStyle = BorderStyle.FixedSingle;
            pnPusher01c.Location = new Point(L916 + L1800 + L1000 + L560 + L680 + 10 + 50, 23);
            pnPusher01c.BackColor = Color;
            pnPusher01c.Width = 50 / SCALE;
            pnPusher01c.Height = 300 / SCALE;
            this.Controls.Add(pnPusher01c);
        }

    }
}
