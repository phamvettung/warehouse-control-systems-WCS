namespace Intech.FortnaRollerConveyor.WCS.UI
{
    partial class ContainerDetailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContainerDetailForm));
            label1 = new Label();
            txtMessageType = new TextBox();
            txtReturnCode = new TextBox();
            label2 = new Label();
            txtMessage = new TextBox();
            label3 = new Label();
            txtBarcode = new TextBox();
            label4 = new Label();
            txtDeviceId = new TextBox();
            label5 = new Label();
            txtCommand = new TextBox();
            label6 = new Label();
            txtScannerName = new TextBox();
            label7 = new Label();
            label8 = new Label();
            listView1 = new ListView();
            messageType = new ColumnHeader();
            sequenceNumber = new ColumnHeader();
            codeReturn = new ColumnHeader();
            message = new ColumnHeader();
            data = new ColumnHeader();
            txtSequenceNumber = new TextBox();
            label9 = new Label();
            label10 = new Label();
            btnReject = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Siemens Sans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(-279, 185);
            label1.Name = "label1";
            label1.Size = new Size(123, 19);
            label1.TabIndex = 0;
            label1.Text = "1. Message type";
            // 
            // txtMessageType
            // 
            txtMessageType.Anchor = AnchorStyles.Top;
            txtMessageType.BorderStyle = BorderStyle.None;
            txtMessageType.Font = new Font("Siemens Sans Black", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtMessageType.Location = new Point(-150, 177);
            txtMessageType.Name = "txtMessageType";
            txtMessageType.ReadOnly = true;
            txtMessageType.Size = new Size(240, 23);
            txtMessageType.TabIndex = 1;
            // 
            // txtReturnCode
            // 
            txtReturnCode.Anchor = AnchorStyles.Top;
            txtReturnCode.BorderStyle = BorderStyle.None;
            txtReturnCode.Font = new Font("Siemens Sans Black", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtReturnCode.Location = new Point(-150, 209);
            txtReturnCode.Name = "txtReturnCode";
            txtReturnCode.ReadOnly = true;
            txtReturnCode.Size = new Size(240, 23);
            txtReturnCode.TabIndex = 3;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Siemens Sans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(-279, 217);
            label2.Name = "label2";
            label2.Size = new Size(110, 19);
            label2.TabIndex = 2;
            label2.Text = "2. Code return";
            // 
            // txtMessage
            // 
            txtMessage.Anchor = AnchorStyles.Top;
            txtMessage.BorderStyle = BorderStyle.None;
            txtMessage.Font = new Font("Siemens Sans Black", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtMessage.Location = new Point(-150, 305);
            txtMessage.Name = "txtMessage";
            txtMessage.ReadOnly = true;
            txtMessage.Size = new Size(610, 23);
            txtMessage.TabIndex = 5;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top;
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Siemens Sans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(-279, 313);
            label3.Name = "label3";
            label3.Size = new Size(88, 19);
            label3.TabIndex = 4;
            label3.Text = "4. Message";
            // 
            // txtBarcode
            // 
            txtBarcode.Anchor = AnchorStyles.Top;
            txtBarcode.BorderStyle = BorderStyle.None;
            txtBarcode.Font = new Font("Siemens Sans Black", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtBarcode.Location = new Point(220, 177);
            txtBarcode.Name = "txtBarcode";
            txtBarcode.ReadOnly = true;
            txtBarcode.Size = new Size(240, 23);
            txtBarcode.TabIndex = 7;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top;
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Siemens Sans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(115, 185);
            label4.Name = "label4";
            label4.Size = new Size(83, 19);
            label4.TabIndex = 6;
            label4.Text = "5. Barcode";
            // 
            // txtDeviceId
            // 
            txtDeviceId.Anchor = AnchorStyles.Top;
            txtDeviceId.BorderStyle = BorderStyle.None;
            txtDeviceId.Font = new Font("Siemens Sans Black", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtDeviceId.Location = new Point(220, 209);
            txtDeviceId.Name = "txtDeviceId";
            txtDeviceId.ReadOnly = true;
            txtDeviceId.Size = new Size(240, 23);
            txtDeviceId.TabIndex = 9;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top;
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Siemens Sans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(115, 217);
            label5.Name = "label5";
            label5.Size = new Size(89, 19);
            label5.TabIndex = 8;
            label5.Text = "6. Device id";
            // 
            // txtCommand
            // 
            txtCommand.Anchor = AnchorStyles.Top;
            txtCommand.BorderStyle = BorderStyle.None;
            txtCommand.Font = new Font("Siemens Sans Black", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtCommand.Location = new Point(220, 273);
            txtCommand.Name = "txtCommand";
            txtCommand.ReadOnly = true;
            txtCommand.Size = new Size(240, 23);
            txtCommand.TabIndex = 11;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top;
            label6.AutoSize = true;
            label6.BackColor = Color.Transparent;
            label6.Font = new Font("Siemens Sans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label6.Location = new Point(115, 281);
            label6.Name = "label6";
            label6.Size = new Size(99, 19);
            label6.TabIndex = 10;
            label6.Text = "8. Command";
            // 
            // txtScannerName
            // 
            txtScannerName.Anchor = AnchorStyles.Top;
            txtScannerName.BorderStyle = BorderStyle.None;
            txtScannerName.Font = new Font("Siemens Sans Black", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtScannerName.Location = new Point(220, 241);
            txtScannerName.Name = "txtScannerName";
            txtScannerName.ReadOnly = true;
            txtScannerName.Size = new Size(240, 23);
            txtScannerName.TabIndex = 13;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Top;
            label7.AutoSize = true;
            label7.BackColor = Color.Transparent;
            label7.Font = new Font("Siemens Sans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label7.Location = new Point(115, 249);
            label7.Name = "label7";
            label7.Size = new Size(82, 19);
            label7.TabIndex = 12;
            label7.Text = "7. Scanner";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.Transparent;
            label8.Font = new Font("Siemens Sans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label8.ForeColor = Color.FromArgb(0, 90, 206);
            label8.Location = new Point(12, 339);
            label8.Name = "label8";
            label8.Size = new Size(227, 19);
            label8.TabIndex = 14;
            label8.Text = "- History of receiving messages";
            // 
            // listView1
            // 
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView1.Columns.AddRange(new ColumnHeader[] { messageType, sequenceNumber, codeReturn, message, data });
            listView1.Location = new Point(29, 361);
            listView1.Name = "listView1";
            listView1.Size = new Size(128, 0);
            listView1.TabIndex = 15;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // messageType
            // 
            messageType.Text = "Message type";
            messageType.Width = 150;
            // 
            // sequenceNumber
            // 
            sequenceNumber.Text = "Sequence number";
            // 
            // codeReturn
            // 
            codeReturn.Text = "Code return";
            codeReturn.Width = 50;
            // 
            // message
            // 
            message.Text = "Message";
            message.Width = 200;
            // 
            // data
            // 
            data.Text = "Data";
            data.Width = 350;
            // 
            // txtSequenceNumber
            // 
            txtSequenceNumber.Anchor = AnchorStyles.Top;
            txtSequenceNumber.BorderStyle = BorderStyle.None;
            txtSequenceNumber.Font = new Font("Siemens Sans Black", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtSequenceNumber.Location = new Point(-150, 241);
            txtSequenceNumber.Name = "txtSequenceNumber";
            txtSequenceNumber.ReadOnly = true;
            txtSequenceNumber.Size = new Size(240, 23);
            txtSequenceNumber.TabIndex = 17;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Top;
            label9.AutoSize = true;
            label9.BackColor = Color.Transparent;
            label9.Font = new Font("Siemens Sans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label9.Location = new Point(-279, 249);
            label9.Name = "label9";
            label9.Size = new Size(93, 19);
            label9.TabIndex = 16;
            label9.Text = "3. Sequence";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BackColor = Color.Transparent;
            label10.Font = new Font("Siemens Sans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label10.ForeColor = Color.FromArgb(0, 90, 206);
            label10.Location = new Point(27, 144);
            label10.Name = "label10";
            label10.Size = new Size(166, 19);
            label10.TabIndex = 18;
            label10.Text = "- Message Information";
            // 
            // btnReject
            // 
            btnReject.FlatAppearance.BorderSize = 0;
            btnReject.FlatStyle = FlatStyle.Flat;
            btnReject.Font = new Font("Siemens Sans", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            btnReject.ForeColor = Color.FromArgb(0, 90, 206);
            btnReject.Location = new Point(12, 12);
            btnReject.Name = "btnReject";
            btnReject.Size = new Size(164, 91);
            btnReject.TabIndex = 19;
            btnReject.Text = "Reject";
            btnReject.UseVisualStyleBackColor = true;
            // 
            // ContainerDetailForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(189, 113);
            Controls.Add(btnReject);
            Controls.Add(label10);
            Controls.Add(txtSequenceNumber);
            Controls.Add(label9);
            Controls.Add(listView1);
            Controls.Add(label8);
            Controls.Add(txtScannerName);
            Controls.Add(label7);
            Controls.Add(txtCommand);
            Controls.Add(label6);
            Controls.Add(txtDeviceId);
            Controls.Add(label5);
            Controls.Add(txtBarcode);
            Controls.Add(label4);
            Controls.Add(txtMessage);
            Controls.Add(label3);
            Controls.Add(txtReturnCode);
            Controls.Add(label2);
            Controls.Add(txtMessageType);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ContainerDetailForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "WCS";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtMessageType;
        private TextBox txtReturnCode;
        private Label label2;
        private TextBox txtMessage;
        private Label label3;
        private TextBox txtBarcode;
        private Label label4;
        private TextBox txtDeviceId;
        private Label label5;
        private TextBox txtCommand;
        private Label label6;
        private TextBox txtScannerName;
        private Label label7;
        private Label label8;
        private ListView listView1;
        private ColumnHeader messageType;
        private ColumnHeader codeReturn;
        private ColumnHeader message;
        private ColumnHeader data;
        private TextBox txtSequenceNumber;
        private Label label9;
        private ColumnHeader sequenceNumber;
        private Label label10;
        private Button btnReject;
    }
}