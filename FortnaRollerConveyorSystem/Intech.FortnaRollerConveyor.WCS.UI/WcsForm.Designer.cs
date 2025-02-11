namespace Intech.FortnaRollerConveyor.WCS.UI
{
    partial class WcsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WcsForm));
            menuStrip1 = new MenuStrip();
            tsWES = new ToolStripMenuItem();
            menuReconnectToWES = new ToolStripMenuItem();
            menuDisconnectToWES = new ToolStripMenuItem();
            tsScanner = new ToolStripMenuItem();
            menuReconnectToSC01 = new ToolStripMenuItem();
            menuDisconnectToScanner01 = new ToolStripMenuItem();
            menuReconnectToScanner02 = new ToolStripMenuItem();
            menuDisconnectToScanner02 = new ToolStripMenuItem();
            tsPLC = new ToolStripMenuItem();
            menuReconnectToPLC = new ToolStripMenuItem();
            menuDisconnectToPLC = new ToolStripMenuItem();
            tsSetting = new ToolStripMenuItem();
            tsiReset = new ToolStripMenuItem();
            tsiSave = new ToolStripMenuItem();
            menuLayout = new ToolStripMenuItem();
            menuExit = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            lbReady = new ToolStripStatusLabel();
            pnTop = new Panel();
            panel2 = new Panel();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            pnBottom = new Panel();
            pnBody = new Panel();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            pnTop.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Control;
            menuStrip1.Font = new Font("Siemens Sans", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            menuStrip1.GripStyle = ToolStripGripStyle.Visible;
            menuStrip1.Items.AddRange(new ToolStripItem[] { tsWES, tsScanner, tsPLC, tsSetting, menuLayout, menuExit });
            menuStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(784, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // tsWES
            // 
            tsWES.DropDownItems.AddRange(new ToolStripItem[] { menuReconnectToWES, menuDisconnectToWES });
            tsWES.ForeColor = SystemColors.WindowText;
            tsWES.Name = "tsWES";
            tsWES.Size = new Size(44, 20);
            tsWES.Text = "WES";
            // 
            // menuReconnectToWES
            // 
            menuReconnectToWES.Name = "menuReconnectToWES";
            menuReconnectToWES.Size = new Size(137, 22);
            menuReconnectToWES.Text = "Reconnect";
            // 
            // menuDisconnectToWES
            // 
            menuDisconnectToWES.Name = "menuDisconnectToWES";
            menuDisconnectToWES.Size = new Size(137, 22);
            menuDisconnectToWES.Text = "Disconnect";
            // 
            // tsScanner
            // 
            tsScanner.DropDownItems.AddRange(new ToolStripItem[] { menuReconnectToSC01, menuDisconnectToScanner01, menuReconnectToScanner02, menuDisconnectToScanner02 });
            tsScanner.ForeColor = SystemColors.WindowText;
            tsScanner.Name = "tsScanner";
            tsScanner.Size = new Size(72, 20);
            tsScanner.Text = "Scanners";
            // 
            // menuReconnectToSC01
            // 
            menuReconnectToSC01.Name = "menuReconnectToSC01";
            menuReconnectToSC01.Size = new Size(219, 22);
            menuReconnectToSC01.Text = "Reconnect to Scanner 01";
            // 
            // menuDisconnectToScanner01
            // 
            menuDisconnectToScanner01.Name = "menuDisconnectToScanner01";
            menuDisconnectToScanner01.Size = new Size(219, 22);
            menuDisconnectToScanner01.Text = "Disconnect to Scanner 01";
            // 
            // menuReconnectToScanner02
            // 
            menuReconnectToScanner02.Name = "menuReconnectToScanner02";
            menuReconnectToScanner02.Size = new Size(219, 22);
            menuReconnectToScanner02.Text = "Reconnect to Scanner 02";
            // 
            // menuDisconnectToScanner02
            // 
            menuDisconnectToScanner02.Name = "menuDisconnectToScanner02";
            menuDisconnectToScanner02.Size = new Size(219, 22);
            menuDisconnectToScanner02.Text = "Disconnect to Scanner 02";
            // 
            // tsPLC
            // 
            tsPLC.DropDownItems.AddRange(new ToolStripItem[] { menuReconnectToPLC, menuDisconnectToPLC });
            tsPLC.ForeColor = SystemColors.WindowText;
            tsPLC.Name = "tsPLC";
            tsPLC.Size = new Size(40, 20);
            tsPLC.Text = "PLC";
            // 
            // menuReconnectToPLC
            // 
            menuReconnectToPLC.Name = "menuReconnectToPLC";
            menuReconnectToPLC.Size = new Size(175, 22);
            menuReconnectToPLC.Text = "Reconnect to PLC";
            // 
            // menuDisconnectToPLC
            // 
            menuDisconnectToPLC.Name = "menuDisconnectToPLC";
            menuDisconnectToPLC.Size = new Size(175, 22);
            menuDisconnectToPLC.Text = "Disconnect to PLC";
            // 
            // tsSetting
            // 
            tsSetting.DropDownItems.AddRange(new ToolStripItem[] { tsiReset, tsiSave });
            tsSetting.ForeColor = SystemColors.WindowText;
            tsSetting.Name = "tsSetting";
            tsSetting.Size = new Size(81, 20);
            tsSetting.Text = "Containers";
            // 
            // tsiReset
            // 
            tsiReset.Name = "tsiReset";
            tsiReset.Size = new Size(183, 22);
            tsiReset.Text = "Clear all containers";
            tsiReset.ToolTipText = "Delete all containers on the conveyor";
            // 
            // tsiSave
            // 
            tsiSave.Name = "tsiSave";
            tsiSave.Size = new Size(183, 22);
            tsiSave.Text = "Save all containers";
            tsiSave.ToolTipText = "Save all containers on the conveyor";
            // 
            // menuLayout
            // 
            menuLayout.Name = "menuLayout";
            menuLayout.Size = new Size(61, 20);
            menuLayout.Text = "Layouts";
            // 
            // menuExit
            // 
            menuExit.DisplayStyle = ToolStripItemDisplayStyle.Text;
            menuExit.ForeColor = SystemColors.WindowText;
            menuExit.Name = "menuExit";
            menuExit.Size = new Size(39, 20);
            menuExit.Text = "Exit";
            menuExit.ToolTipText = "Close application";
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = Color.FromArgb(77, 96, 130);
            statusStrip1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            statusStrip1.Items.AddRange(new ToolStripItem[] { lbReady });
            statusStrip1.Location = new Point(0, 439);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(784, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // lbReady
            // 
            lbReady.Font = new Font("Siemens Sans", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbReady.ForeColor = Color.White;
            lbReady.Name = "lbReady";
            lbReady.Size = new Size(41, 17);
            lbReady.Text = "Ready";
            // 
            // pnTop
            // 
            pnTop.BackColor = SystemColors.Control;
            pnTop.Controls.Add(panel2);
            pnTop.Controls.Add(panel1);
            pnTop.Dock = DockStyle.Top;
            pnTop.Location = new Point(0, 24);
            pnTop.Name = "pnTop";
            pnTop.Size = new Size(784, 50);
            pnTop.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Transparent;
            panel2.Controls.Add(pictureBox2);
            panel2.Controls.Add(pictureBox1);
            panel2.Dock = DockStyle.Right;
            panel2.Location = new Point(534, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(250, 50);
            panel2.TabIndex = 1;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.Image = Properties.Resources.Intech_logo;
            pictureBox2.Location = new Point(134, 2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(116, 44);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.Fortna_logo;
            pictureBox1.Location = new Point(0, -12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(136, 72);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 50);
            panel1.TabIndex = 0;
            // 
            // pnBottom
            // 
            pnBottom.BackColor = SystemColors.Control;
            pnBottom.Dock = DockStyle.Bottom;
            pnBottom.Location = new Point(0, 434);
            pnBottom.Name = "pnBottom";
            pnBottom.Size = new Size(784, 5);
            pnBottom.TabIndex = 3;
            // 
            // pnBody
            // 
            pnBody.BackColor = SystemColors.Control;
            pnBody.Dock = DockStyle.Fill;
            pnBody.Location = new Point(0, 74);
            pnBody.Name = "pnBody";
            pnBody.Size = new Size(784, 360);
            pnBody.TabIndex = 4;
            // 
            // WcsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(pnBody);
            Controls.Add(pnBottom);
            Controls.Add(pnTop);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "WcsForm";
            Text = "Warehouse Control Systems";
            WindowState = FormWindowState.Maximized;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            pnTop.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private StatusStrip statusStrip1;
        private Panel pnTop;
        private Panel pnBottom;
        private Panel pnBody;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
        private Panel panel2;
        private Panel panel1;
        private ToolStripMenuItem tsWES;
        private ToolStripMenuItem tsScanner;
        private ToolStripMenuItem tsPLC;
        private ToolStripMenuItem tsSetting;
        private ToolStripMenuItem tsiReset;
        private ToolStripMenuItem menuReconnectToSC01;
        private ToolStripMenuItem menuDisconnectToScanner01;
        private ToolStripMenuItem menuReconnectToScanner02;
        private ToolStripMenuItem menuDisconnectToScanner02;
        private ToolStripMenuItem menuReconnectToPLC;
        private ToolStripMenuItem menuDisconnectToPLC;
        private ToolStripMenuItem menuReconnectToWES;
        private ToolStripMenuItem menuDisconnectToWES;
        private ToolStripStatusLabel lbReady;
        private ToolStripMenuItem tsiSave;
        private ToolStripMenuItem menuExit;
        private ToolStripMenuItem menuLayout;
    }
}