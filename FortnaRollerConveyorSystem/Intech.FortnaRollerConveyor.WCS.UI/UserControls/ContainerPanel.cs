using Intech.FortnaRollerConveyor.WCS.UI.Models;
using Intech.FortnaRollerConveyor.WCS.UI.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.WCS.UI.UserControls
{
    public class ContainerPanel : Panel
    {
        /// <summary>
        /// 
        /// </summary>
        public Container container;
        /// <summary>
        /// index of container in the list container
        /// </summary>
        public int[] indexOfContainer {  get; set; }


        private System.Windows.Forms.Timer tmrUpdated;
        private Label lbDeviceName;
        private Label lbBarcode;

        public ContainerPanel() 
        {
            InitializeObject();
            InitializeComponent();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            lbDeviceName.Click += LbDeviceName_Click;
            lbBarcode.Click += LbBarcode_Click;
        }

        private void LbBarcode_Click(object? sender, EventArgs e)
        {
            ContainerDetailForm containerDetailForm = new ContainerDetailForm();
            containerDetailForm.Container = this.container;
            containerDetailForm.id = indexOfContainer;
            containerDetailForm.ShowDialog();
        }

        private void LbDeviceName_Click(object? sender, EventArgs e)
        {
            ContainerDetailForm containerDetailForm = new ContainerDetailForm();
            containerDetailForm.Container = this.container;
            containerDetailForm.id = indexOfContainer;
            containerDetailForm.ShowDialog();
        }

        private void InitializeObject()
        {
            container = new Container();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.Size = new Size(460/MainConveyor.SCALE, 330/MainConveyor.SCALE);

            Panel pnDeviceName = new Panel();
            pnDeviceName.Dock = DockStyle.Top;
            pnDeviceName.Height = 165 / MainConveyor.SCALE;
            this.Controls.Add(pnDeviceName);

            Panel pnBarcode = new Panel();
            pnBarcode.Dock = DockStyle.Bottom;
            pnBarcode.Height = 165 / MainConveyor.SCALE;
            this.Controls.Add(pnBarcode);

            this.lbDeviceName = new Label();
            lbDeviceName.AutoSize = false;
            lbDeviceName.Dock = DockStyle.Fill;
            lbDeviceName.TextAlign = ContentAlignment.MiddleCenter;
            lbDeviceName.ForeColor = Color.White;
            lbDeviceName.Text = container.DeviceId;
            pnDeviceName.Controls.Add(lbDeviceName);

            this.lbBarcode = new Label();
            lbBarcode.AutoSize = false;
            lbBarcode.Dock = DockStyle.Fill;
            lbBarcode.TextAlign = ContentAlignment.MiddleCenter;
            lbBarcode.ForeColor = Color.White;
            lbBarcode.Text = container.Barcode;
            pnBarcode.Controls.Add(lbBarcode);

            tmrUpdated = new System.Windows.Forms.Timer();
            tmrUpdated.Tick += TmrUpdated_Tick;
            tmrUpdated.Interval = 500;
            tmrUpdated.Start();

        }

        private void TmrUpdated_Tick(object? sender, EventArgs e)
        {
            switch (container.Flag)
            {
                case ContainerFlag.None:
                    this.BackColor = Color.Blue;
                    lbDeviceName.ForeColor = Color.White;
                    lbBarcode.ForeColor = Color.White;
                    lbDeviceName.Text = container.DeviceId;
                    lbBarcode.Text = container.Barcode;
                    break;
                case ContainerFlag.Command:
                    if (container.Command == AppConfig.ADVANCE)
                    {
                        this.BackColor = Color.Yellow;
                        lbDeviceName.ForeColor = Color.Red;
                        lbBarcode.ForeColor = Color.Red;
                        lbDeviceName.Text = container.DeviceId;
                        lbBarcode.Text = container.Barcode;
                    }
                    else if (container.Command == AppConfig.COMPLETE)
                    {
                        this.BackColor = Color.LimeGreen;
                        lbDeviceName.ForeColor = Color.White;
                        lbBarcode.ForeColor = Color.White;
                        lbDeviceName.Text = container.DeviceId;
                        lbBarcode.Text = container.Barcode;
                    }
                    else if (container.Barcode == AppConfig.NG)
                    {
                        this.BackColor = Color.Red;
                        lbBarcode.Text = container.Barcode;
                        lbBarcode.ForeColor = Color.White;
                        lbDeviceName.Text = container.DeviceId;
                        lbBarcode.Text = container.Barcode;
                    }
                    break;
                case ContainerFlag.DeviceId:
                    if (container.DeviceId == AppConfig.GTP_01)
                    {
                        this.BackColor = Color.DodgerBlue;
                        lbDeviceName.ForeColor = Color.White;
                        lbDeviceName.Text = container.DeviceId;
                        lbBarcode.Text = container.Barcode;
                        lbBarcode.ForeColor = Color.White;
                    }
                    else if (container.DeviceId == AppConfig.GTP_02)
                    {
                        this.BackColor = Color.RoyalBlue;
                        lbDeviceName.ForeColor = Color.White;
                        lbDeviceName.Text = container.DeviceId;
                        lbBarcode.Text = container.Barcode;
                        lbBarcode.ForeColor = Color.White;
                    }
                    else if (container.DeviceId == AppConfig.END)
                    {
                        this.BackColor = Color.LightSalmon;
                        lbDeviceName.ForeColor = Color.White;
                        lbDeviceName.Text = container.DeviceId;
                        lbBarcode.Text = container.Barcode;
                        lbBarcode.ForeColor = Color.White;
                    }
                    break;
                default:
                    break;
            }

            if (container.waitingCommand)
            {
                lbDeviceName.Text = AppConfig.WAITING_TEXT;
                lbBarcode.Text = container.Barcode;
            }
            else
            {
                if (container.Barcode == AppConfig.NG)
                {
                    lbDeviceName.Text = AppConfig.WAITING_TEXT;
                    lbBarcode.Text = container.Barcode;
                    this.BackColor = Color.Red;
                }
                else
                {
                    lbDeviceName.Text = container.DeviceId;
                    lbBarcode.Text = container.Barcode;
                }
            }
                
        }
    }
}
