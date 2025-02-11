namespace Intech.FortnaRollerConveyor.WES.UI
{
    partial class WesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WesForm));
            btnSend = new Button();
            listClient = new ListView();
            ipEndPoint = new ColumnHeader();
            status = new ColumnHeader();
            msgReceived = new ColumnHeader();
            lastTime = new ColumnHeader();
            btnDisconnect = new Button();
            groupBox1 = new GroupBox();
            btnCompleteAdvance01 = new Button();
            btnComplete01 = new Button();
            btnAdvance01 = new Button();
            groupBox2 = new GroupBox();
            btnCompleteAdvance02 = new Button();
            btnComplete02 = new Button();
            btnAdvance02 = new Button();
            listBox1 = new ListBox();
            btnClear = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // btnSend
            // 
            btnSend.Location = new Point(311, 12);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 35);
            btnSend.TabIndex = 2;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            // 
            // listClient
            // 
            listClient.Columns.AddRange(new ColumnHeader[] { ipEndPoint, status, msgReceived, lastTime });
            listClient.Location = new Point(392, 12);
            listClient.Name = "listClient";
            listClient.Size = new Size(398, 132);
            listClient.TabIndex = 3;
            listClient.UseCompatibleStateImageBehavior = false;
            listClient.View = View.Details;
            // 
            // ipEndPoint
            // 
            ipEndPoint.Text = "IP";
            ipEndPoint.Width = 150;
            // 
            // status
            // 
            status.Text = "Status";
            status.Width = 100;
            // 
            // msgReceived
            // 
            msgReceived.Text = "MsgReceived";
            msgReceived.Width = 400;
            // 
            // lastTime
            // 
            lastTime.Text = "LastTime";
            lastTime.Width = 120;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(311, 53);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(75, 35);
            btnDisconnect.TabIndex = 4;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnCompleteAdvance01);
            groupBox1.Controls.Add(btnComplete01);
            groupBox1.Controls.Add(btnAdvance01);
            groupBox1.Location = new Point(9, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(142, 132);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "GTP-01";
            // 
            // btnCompleteAdvance01
            // 
            btnCompleteAdvance01.Location = new Point(6, 94);
            btnCompleteAdvance01.Name = "btnCompleteAdvance01";
            btnCompleteAdvance01.Size = new Size(120, 30);
            btnCompleteAdvance01.TabIndex = 0;
            btnCompleteAdvance01.Text = "CompleteAdvance";
            btnCompleteAdvance01.UseVisualStyleBackColor = true;
            // 
            // btnComplete01
            // 
            btnComplete01.Location = new Point(6, 58);
            btnComplete01.Name = "btnComplete01";
            btnComplete01.Size = new Size(120, 30);
            btnComplete01.TabIndex = 0;
            btnComplete01.Text = "Complete";
            btnComplete01.UseVisualStyleBackColor = true;
            // 
            // btnAdvance01
            // 
            btnAdvance01.Location = new Point(6, 22);
            btnAdvance01.Name = "btnAdvance01";
            btnAdvance01.Size = new Size(120, 30);
            btnAdvance01.TabIndex = 0;
            btnAdvance01.Text = "Advance";
            btnAdvance01.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnCompleteAdvance02);
            groupBox2.Controls.Add(btnComplete02);
            groupBox2.Controls.Add(btnAdvance02);
            groupBox2.Location = new Point(157, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(148, 132);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "GTP-02";
            // 
            // btnCompleteAdvance02
            // 
            btnCompleteAdvance02.Location = new Point(6, 94);
            btnCompleteAdvance02.Name = "btnCompleteAdvance02";
            btnCompleteAdvance02.Size = new Size(120, 30);
            btnCompleteAdvance02.TabIndex = 1;
            btnCompleteAdvance02.Text = "CompleteAdvance";
            btnCompleteAdvance02.UseVisualStyleBackColor = true;
            // 
            // btnComplete02
            // 
            btnComplete02.Location = new Point(6, 58);
            btnComplete02.Name = "btnComplete02";
            btnComplete02.Size = new Size(120, 30);
            btnComplete02.TabIndex = 2;
            btnComplete02.Text = "Complete";
            btnComplete02.UseVisualStyleBackColor = true;
            // 
            // btnAdvance02
            // 
            btnAdvance02.Location = new Point(6, 22);
            btnAdvance02.Name = "btnAdvance02";
            btnAdvance02.Size = new Size(120, 30);
            btnAdvance02.TabIndex = 3;
            btnAdvance02.Text = "Advance";
            btnAdvance02.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(9, 150);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(781, 394);
            listBox1.TabIndex = 7;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(715, 521);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(75, 23);
            btnClear.TabIndex = 8;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            // 
            // WesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 551);
            Controls.Add(btnClear);
            Controls.Add(listBox1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btnDisconnect);
            Controls.Add(btnSend);
            Controls.Add(listClient);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "WesForm";
            Text = "Server";
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button btnSend;
        private ListView listClient;
        private ColumnHeader ipEndPoint;
        private ColumnHeader status;
        private Button btnDisconnect;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button btnCompleteAdvance01;
        private Button btnComplete01;
        private Button btnAdvance01;
        private ColumnHeader msgReceived;
        private ColumnHeader lastTime;
        private Button btnCompleteAdvance02;
        private Button btnComplete02;
        private Button btnAdvance02;
        private ListBox listBox1;
        private Button btnClear;
    }
}