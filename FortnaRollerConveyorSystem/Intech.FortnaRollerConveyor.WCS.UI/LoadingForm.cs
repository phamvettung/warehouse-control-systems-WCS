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
    public partial class LoadingForm : Form
    {
        private Action action;
        private string message;
        public LoadingForm(Action action, string message)
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            this.action = action;
            this.message = message;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.lbMessage.Text = message;
            base.OnLoad(e);
            Task.Factory.StartNew(this.action).ContinueWith(t => { this.Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
