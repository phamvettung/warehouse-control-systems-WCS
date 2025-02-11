using Intech.FortnaRollerConveyor.WCS.UI.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Intech.FortnaRollerConveyor.WCS.UI
{
    public partial class WcsForm : Form
    {
        //Forms
        Form currentForm;
        HomeForm homeForm;

        //States
        bool saved = false, routing_mode = false, sorting_mode = false;
        int count_saved = 0, count_routing = 0, count_sorting = 0;
        Color normalcolorReady = Color.Gray;

        //Tasks
        Task taskSave = null;


        #region Contructors
        public WcsForm()
        {
            InitializeComponent();
            InitializeForm();
            InitializeTimer();
            RegisterEvents();
        }

        private void InitializeTimer()
        {

        }

        private void InitializeForm()
        {
            homeForm = new HomeForm();
        }

        private void RegisterEvents()
        {
            Load += WcsForm_Load;
            Shown += WcsForm_Shown;
            FormClosing += WcsForm_FormClosing;
            FormClosed += WcsForm_FormClosed;
            tsiReset.Click += TsiReset_Click;
            tsiSave.Click += TsiSave_Click;
            menuExit.Click += MenuExit_Click;
            menuReconnectToWES.Click += MenuReconnectToWES_Click;
            menuDisconnectToWES.Click += MenuDisconnectToWES_Click;
            menuReconnectToSC01.Click += MenuReconnectToSC01_Click;
            menuDisconnectToScanner01.Click += MenuDisconnectToScanner01_Click;
            menuReconnectToScanner02.Click += MenuReconnectToScanner02_Click;
            menuDisconnectToScanner02.Click += MenuDisconnectToScanner02_Click;
            menuReconnectToPLC.Click += MenuReconnectToPLC_Click;
            menuDisconnectToPLC.Click += MenuDisconnectToPLC_Click;
            menuLayout.Click += MenuLayout_Click;

            Grapher.OnShowReady += Grapher_OnShowReady;
            Grapher.OnShowMode += Grapher_OnShowMode;
            HomeForm.OnShowReady += HomeForm_OnShowReady;
        }

        #endregion


        #region Events

        private void WcsForm_Shown(object? sender, EventArgs e)
        {
            
        }

        private void WcsForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            
        }

        private void MenuLayout_Click(object? sender, EventArgs e)
        {
            Fader.FadeIn(new LayoutForm(), Fader.FadeSpeed.Faster);
        }

        private void Grapher_OnShowMode(PlcEnum mode)
        {

        }

        private void HomeForm_OnShowReady(string message, Color color)
        {
            Invoke(new Action(() =>
            {
                Task task = new Task(() =>
                {
                    lbReady.Text = message;
                    statusStrip1.BackColor = color;
                });
                task.Start();
                task.Wait();
            }));
        }

        private void Grapher_OnShowReady(string text, Color color)
        {
            try
            {
                Invoke(new Action(() =>
                    {
                        lbReady.Text = text;
                        statusStrip1.BackColor = color;
                    }));
            }
            catch (Exception ex)
            {

            }
        }

        private void MenuExit_Click(object? sender, EventArgs e)
        {
            if(HomeForm.grapher != null)
                HomeForm.grapher.SaveContainer();
            lbReady.Text = "Container(s) Saved";
            DialogResult dr = MessageBox.Show("All the current containers have been saved. Do you want to exit?", "WCS Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Fader.FadeOut(this, Fader.FadeSpeed.Faster);
            }
        }

        private void TsiSave_Click(object? sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                statusStrip1.BackColor = Color.DodgerBlue;
                lbReady.Text = "Container(s) Saved";
                HomeForm.grapher.SaveContainer();
                Thread.Sleep(2000);
                statusStrip1.BackColor = Color.LimeGreen;
                lbReady.Text = "Ready";
            }));
        }

        private void MenuDisconnectToPLC_Click(object? sender, EventArgs e)
        {
            OnConnection(7);
        }

        private void MenuReconnectToPLC_Click(object? sender, EventArgs e)
        {
            OnConnection(6);
        }

        private void MenuDisconnectToScanner02_Click(object? sender, EventArgs e)
        {
            OnConnection(5);
        }

        private void MenuReconnectToScanner02_Click(object? sender, EventArgs e)
        {
            OnConnection(4);
        }

        private void MenuDisconnectToScanner01_Click(object? sender, EventArgs e)
        {
            OnConnection(3);
        }

        private void MenuReconnectToSC01_Click(object? sender, EventArgs e)
        {
            OnConnection(2);
        }

        private void MenuDisconnectToWES_Click(object? sender, EventArgs e)
        {
            OnConnection(1);
        }

        private void MenuReconnectToWES_Click(object? sender, EventArgs e)
        {
            OnConnection(0);
        }

        private void WcsForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            try
            {
                if (HomeForm.Scanner01Socket.TcpClient != null)
                    HomeForm.Scanner01Socket.Disconnect();

                if (HomeForm.Scanner02Socket.TcpClient != null)
                    HomeForm.Scanner02Socket.Disconnect();

                if (HomeForm.WesSocket != null)
                    HomeForm.WesSocket.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

                if(HomeForm.grapher != null)
                {
                    HomeForm.grapher.SaveContainer();
                    HomeForm.grapher.Stop();
                }
                    
                if (HomeForm.Plc != null)
                    HomeForm.Plc.Close();

                HomeForm.Scanner01Socket = null;
                HomeForm.Scanner02Socket = null;
                HomeForm.WesSocket = null;
                HomeForm.grapher = null;
                HomeForm.Plc = null;
            }
        }

        private void WcsForm_Load(object? sender, EventArgs e)
        {
            OpenForm(homeForm);
            lbReady.Text = "Welcome to Warehouse Control Systems!";
        }
        private void TsiReset_Click(object? sender, EventArgs e)
        {
            HomeForm.grapher.ResetContainer();
        }
        #endregion


        #region Methods
        private void OpenForm(Form frm)
        {
            currentForm = frm;
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            this.pnBody.Controls.Add(frm);
            this.pnBody.Tag = frm;
            frm.BringToFront();
            frm.Show();
        }

        #endregion


        #region Delegates
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">0: Reconnect to WES, 1: Disconnect to WES, 2: Reconnect to SC01, 3: Disconnect to SC01,  4: Reconnect to SC02, 5: Disconnect to SC02, 6: Reconnect to PLC, 7: Disconnect to PLC</param>
        public delegate void ConnectionDelegate(int options);
        public static event ConnectionDelegate OnConnection;

        #endregion
    
    
    }
}
