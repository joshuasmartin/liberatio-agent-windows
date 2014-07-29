namespace Liberatio.Agent.Tray
{
    partial class FormConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConsole));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblVersionNumber = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.cmbRole = new System.Windows.Forms.ComboBox();
            this.lblLocation = new System.Windows.Forms.Label();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.btnSaveAndRestart = new System.Windows.Forms.Button();
            this.lblUuid = new System.Windows.Forms.Label();
            this.txtUuid = new System.Windows.Forms.TextBox();
            this.pictureWaiting = new System.Windows.Forms.PictureBox();
            this.pictureStatus = new System.Windows.Forms.PictureBox();
            this.statusStrip1.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureWaiting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 420);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(624, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(50, 17);
            this.lblStatus.Text = "Loading";
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionStatus.ForeColor = System.Drawing.Color.Firebrick;
            this.lblConnectionStatus.Location = new System.Drawing.Point(285, 129);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(155, 25);
            this.lblConnectionStatus.TabIndex = 13;
            this.lblConnectionStatus.Text = "Not Registered";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(440, 166);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(152, 23);
            this.btnConnect.TabIndex = 15;
            this.btnConnect.Text = "Register with Liberatio.com";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.Gray;
            this.pnlHeader.Controls.Add(this.pictureBox2);
            this.pnlHeader.Controls.Add(this.lblVersionNumber);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(624, 86);
            this.pnlHeader.TabIndex = 17;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Liberatio.Agent.Tray.Properties.Resources.banner_logo;
            this.pictureBox2.Location = new System.Drawing.Point(21, 21);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(160, 44);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 20;
            this.pictureBox2.TabStop = false;
            // 
            // lblVersionNumber
            // 
            this.lblVersionNumber.AutoSize = true;
            this.lblVersionNumber.ForeColor = System.Drawing.Color.White;
            this.lblVersionNumber.Location = new System.Drawing.Point(534, 63);
            this.lblVersionNumber.Name = "lblVersionNumber";
            this.lblVersionNumber.Size = new System.Drawing.Size(78, 13);
            this.lblVersionNumber.TabIndex = 0;
            this.lblVersionNumber.Text = "Version 1.0.0.0";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblRole);
            this.groupBox1.Controls.Add(this.cmbRole);
            this.groupBox1.Controls.Add(this.lblLocation);
            this.groupBox1.Controls.Add(this.txtLocation);
            this.groupBox1.Controls.Add(this.btnSaveAndRestart);
            this.groupBox1.Controls.Add(this.lblUuid);
            this.groupBox1.Controls.Add(this.txtUuid);
            this.groupBox1.Location = new System.Drawing.Point(12, 208);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(600, 200);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(108, 88);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(29, 13);
            this.lblRole.TabIndex = 19;
            this.lblRole.Text = "Role";
            // 
            // cmbRole
            // 
            this.cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRole.FormattingEnabled = true;
            this.cmbRole.Items.AddRange(new object[] {
            "Workstation",
            "Server"});
            this.cmbRole.Location = new System.Drawing.Point(143, 85);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Size = new System.Drawing.Size(165, 21);
            this.cmbRole.TabIndex = 18;
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(89, 62);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(48, 13);
            this.lblLocation.TabIndex = 17;
            this.lblLocation.Text = "Location";
            // 
            // txtLocation
            // 
            this.txtLocation.Location = new System.Drawing.Point(143, 59);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(165, 20);
            this.txtLocation.TabIndex = 16;
            // 
            // btnSaveAndRestart
            // 
            this.btnSaveAndRestart.Location = new System.Drawing.Point(143, 121);
            this.btnSaveAndRestart.Name = "btnSaveAndRestart";
            this.btnSaveAndRestart.Size = new System.Drawing.Size(103, 23);
            this.btnSaveAndRestart.TabIndex = 15;
            this.btnSaveAndRestart.Text = "Save Changes";
            this.btnSaveAndRestart.UseVisualStyleBackColor = true;
            this.btnSaveAndRestart.Click += new System.EventHandler(this.btnSaveAndRestart_Click);
            // 
            // lblUuid
            // 
            this.lblUuid.AutoSize = true;
            this.lblUuid.Location = new System.Drawing.Point(17, 36);
            this.lblUuid.Name = "lblUuid";
            this.lblUuid.Size = new System.Drawing.Size(120, 13);
            this.lblUuid.TabIndex = 14;
            this.lblUuid.Text = "Unique Identifier (UUID)";
            // 
            // txtUuid
            // 
            this.txtUuid.Location = new System.Drawing.Point(143, 33);
            this.txtUuid.Name = "txtUuid";
            this.txtUuid.ReadOnly = true;
            this.txtUuid.Size = new System.Drawing.Size(215, 20);
            this.txtUuid.TabIndex = 13;
            // 
            // pictureWaiting
            // 
            this.pictureWaiting.Image = global::Liberatio.Agent.Tray.Properties.Resources.waiting;
            this.pictureWaiting.Location = new System.Drawing.Point(177, 128);
            this.pictureWaiting.Name = "pictureWaiting";
            this.pictureWaiting.Size = new System.Drawing.Size(28, 28);
            this.pictureWaiting.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureWaiting.TabIndex = 16;
            this.pictureWaiting.TabStop = false;
            // 
            // pictureStatus
            // 
            this.pictureStatus.Image = global::Liberatio.Agent.Tray.Properties.Resources.disconnected;
            this.pictureStatus.Location = new System.Drawing.Point(231, 119);
            this.pictureStatus.Name = "pictureStatus";
            this.pictureStatus.Size = new System.Drawing.Size(48, 48);
            this.pictureStatus.TabIndex = 20;
            this.pictureStatus.TabStop = false;
            // 
            // FormConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.pictureStatus);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pictureWaiting);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lblConnectionStatus);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "FormConsole";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Liberatio Agent";
            this.Load += new System.EventHandler(this.FormConsole_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureWaiting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.PictureBox pictureWaiting;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblVersionNumber;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.ComboBox cmbRole;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Button btnSaveAndRestart;
        private System.Windows.Forms.Label lblUuid;
        private System.Windows.Forms.TextBox txtUuid;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureStatus;

    }
}