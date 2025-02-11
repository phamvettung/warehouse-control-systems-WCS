using Intech.FortnaRollerConveyor.Shared.Messages;
using Intech.FortnaRollerConveyor.WCS.UI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intech.FortnaRollerConveyor.WCS.UI
{
    public partial class ContainerDetailForm : Form
    {
        public Container Container { get; set; }
        public int[] id { get; set; }
        public ContainerDetailForm()
        {
            InitializeComponent();
            RegisterEvents();
            Container = new Container(); 
        }

        private void RegisterEvents()
        {
            this.Resize += ContainerDetailForm_Resize;
            this.Load += ContainerDetailForm_Load;
            btnReject.Click += BtnReject_Click;
        }

        private void BtnReject_Click(object? sender, EventArgs e)
        {
            HomeForm.grapher.Containers[id[0], id[1]].RemoveAt(id[2]);
            this.Close();
        }

        private void ContainerDetailForm_Load(object? sender, EventArgs e)
        {
            txtMessageType.Text = Container.MessageType;
            txtReturnCode.Text = Container.CodeReturn.ToString();
            txtSequenceNumber.Text = Container.SequenceNumber.ToString();
            txtMessage.Text = Container.Message;
            txtBarcode.Text = Container.Barcode;
            txtDeviceId.Text = Container.DeviceId;
            txtScannerName.Text = Container.ScannerName;
            txtCommand.Text = Container.Command;

            for (int i = 0; i < Container.ResponseMessages.Count; i++)
            {
                ResponseMessage response = Container.ResponseMessages[i];
                String[] str = new String[5];
                str[0] = response.messageType;
                str[1] = response.sequenceNumber.ToString();
                str[2] = response.codeReturn.ToString();
                str[3] = response.message;
                str[4] = response.data.ToString();
                ListViewItem item = new ListViewItem(str);
                listView1.Items.Add(item);
            }
        }

        private void ContainerDetailForm_Resize(object? sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
