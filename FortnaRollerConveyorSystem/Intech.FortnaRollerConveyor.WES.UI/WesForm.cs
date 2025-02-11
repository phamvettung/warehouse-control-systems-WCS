using Intech.FortnaRollerConveyor.Server;
using Intech.FortnaRollerConveyor.Shared.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intech.FortnaRollerConveyor.WES.UI
{
    public partial class WesForm : Form
    {
        private Intech.FortnaRollerConveyor.Server.Server server;
        private System.Windows.Forms.Timer updateListTimer;

        int indexClient = -1;
        public WesForm()
        {
            server = new Server.Server(9051);
            InitializeComponent();
            RegisterEvents();

            updateListTimer = new System.Windows.Forms.Timer();
            updateListTimer.Interval = 1000;
            updateListTimer.Tick += updateListTimer_Tick;
            updateListTimer.Start();
        }

        private void updateListTimer_Tick(object? sender, EventArgs e)
        {
            UpdateClientsList();
        }

        private void UpdateClientsList()
        {
            this.Invoke(() =>
            {
                listClient.Items.Clear();

                foreach (var receiver in server.Receivers)
                {
                    String[] str = new String[4];
                    str[0] = receiver.TcpClient.Client.RemoteEndPoint.ToString();
                    str[1] = receiver.Status.ToString();
                    str[2] = string.Empty;
                    str[3] = DateTime.Now.ToString("HH:mm:ss");
                    ListViewItem item = new ListViewItem(str);
                    listClient.Items.Add(item);
                }
            });
        }

        private void RegisterEvents()
        {
            Load += WesForm_Load;
            btnSend.Click += BtnSend_Click;
            btnDisconnect.Click += BtnDisconnect_Click;
            listClient.SelectedIndexChanged += ListClient_SelectedIndexChanged;
            btnAdvance01.Click += BtnAdvance01_Click;
            btnAdvance02.Click += BtnAdvance02_Click;
            btnComplete01.Click += BtnComplete01_Click;
            btnComplete02.Click += BtnComplete02_Click;
            btnCompleteAdvance01.Click += BtnCompleteAdvance01_Click;
            btnCompleteAdvance02.Click += BtnCompleteAdvance02_Click;
            btnClear.Click += BtnClear_Click;
            Receiver.OnClientSentMessage += Receiver_OnClientSentMessage;
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void Receiver_OnClientSentMessage(Receiver receiver, string msgReceived)
        {
            Invoke(() =>
            {
                listBox1.Items.Add(msgReceived);
            });

        }

        private void BtnCompleteAdvance02_Click(object? sender, EventArgs e)
        {
            if (indexClient == -1) return;
            DeviceCommandRequest deviceCommandRequest = new DeviceCommandRequest();
            deviceCommandRequest.command = "COMPLETEADVANCE";
            deviceCommandRequest.deviceId = "GTP-02";
            server.Receivers[indexClient].OnRequestMessage(deviceCommandRequest);
        }

        private void BtnCompleteAdvance01_Click(object? sender, EventArgs e)
        {
            if (indexClient == -1) return;
            DeviceCommandRequest deviceCommandRequest = new DeviceCommandRequest();
            deviceCommandRequest.command = "COMPLETEADVANCE";
            deviceCommandRequest.deviceId = "GTP-01";
            server.Receivers[indexClient].OnRequestMessage(deviceCommandRequest);
        }

        private void BtnComplete02_Click(object? sender, EventArgs e)
        {
            if (indexClient == -1) return;
            DeviceCommandRequest deviceCommandRequest = new DeviceCommandRequest();
            deviceCommandRequest.command = "COMPLETE";
            deviceCommandRequest.deviceId = "GTP-02";
            server.Receivers[indexClient].OnRequestMessage(deviceCommandRequest);
        }

        private void BtnComplete01_Click(object? sender, EventArgs e)
        {
            if (indexClient == -1) return;
            DeviceCommandRequest deviceCommandRequest = new DeviceCommandRequest();
            deviceCommandRequest.command = "COMPLETE";
            deviceCommandRequest.deviceId = "GTP-01";
            server.Receivers[indexClient].OnRequestMessage(deviceCommandRequest);
        }

        private void BtnAdvance02_Click(object? sender, EventArgs e)
        {
            if (indexClient == -1) return;
            DeviceCommandRequest deviceCommandRequest = new DeviceCommandRequest();
            deviceCommandRequest.command = "ADVANCE";
            deviceCommandRequest.deviceId = "GTP-02";
            server.Receivers[indexClient].OnRequestMessage(deviceCommandRequest);
        }

        private void BtnAdvance01_Click(object? sender, EventArgs e)
        {
            if (indexClient == -1) return;
            DeviceCommandRequest deviceCommandRequest = new DeviceCommandRequest();
            deviceCommandRequest.command = "ADVANCE";
            deviceCommandRequest.deviceId = "GTP-01";
            server.Receivers[indexClient].OnRequestMessage(deviceCommandRequest);
        }

        private void BtnDisconnect_Click(object? sender, EventArgs e)
        {
            if (indexClient == -1) return;
            server.Receivers[indexClient].Disconnect();
        }

        private void ListClient_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (listClient.SelectedItems.Count > 0)
            {
                indexClient = listClient.SelectedItems[0].Index;
            }
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
 
        }

        private void WesForm_Load(object? sender, EventArgs e)
        {
            server.Start();
        }
    }
}
