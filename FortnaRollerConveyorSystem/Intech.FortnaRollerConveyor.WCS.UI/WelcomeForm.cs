using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intech.FortnaRollerConveyor.WCS.UI
{
    public partial class WelcomeForm : Form
    {
        System.Windows.Forms.Timer timer;
        public WelcomeForm()
        {
            InitializeComponent();
            Load += WelcomeForm_Load;
            Shown += WelcomeForm_Shown;
            timer = new System.Windows.Forms.Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1000;
            timer.Start();
        }

        int count = 0;
        private void Timer_Tick(object? sender, EventArgs e)
        {
            count ++;
            if(count == 3)
            {
                timer.Stop();
                this.Hide();
                WcsForm wcsForm = new WcsForm();
                wcsForm.ShowDialog();
                this.Close();
            }
        }

        private void WelcomeForm_Shown(object? sender, EventArgs e)
        {
            Fader.FadeIn(this, Fader.FadeSpeed.Slower);
        }

        private void WelcomeForm_Load(object? sender, EventArgs e)
        {

        }
    }
}
