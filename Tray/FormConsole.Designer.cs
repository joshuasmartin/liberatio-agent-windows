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
            this.txtUuid = new System.Windows.Forms.TextBox();
            this.lblUuid = new System.Windows.Forms.Label();
            this.btnSaveAndRestart = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblLocation = new System.Windows.Forms.Label();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.cmbRole = new System.Windows.Forms.ComboBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.lblConnectionStatusValue = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtUuid
            // 
            this.txtUuid.Location = new System.Drawing.Point(152, 93);
            this.txtUuid.Name = "txtUuid";
            this.txtUuid.ReadOnly = true;
            this.txtUuid.Size = new System.Drawing.Size(215, 20);
            this.txtUuid.TabIndex = 0;
            // 
            // lblUuid
            // 
            this.lblUuid.AutoSize = true;
            this.lblUuid.Location = new System.Drawing.Point(26, 96);
            this.lblUuid.Name = "lblUuid";
            this.lblUuid.Size = new System.Drawing.Size(120, 13);
            this.lblUuid.TabIndex = 1;
            this.lblUuid.Text = "Unique Identifier (UUID)";
            // 
            // btnSaveAndRestart
            // 
            this.btnSaveAndRestart.Location = new System.Drawing.Point(152, 181);
            this.btnSaveAndRestart.Name = "btnSaveAndRestart";
            this.btnSaveAndRestart.Size = new System.Drawing.Size(103, 23);
            this.btnSaveAndRestart.TabIndex = 2;
            this.btnSaveAndRestart.Text = "Save Changes";
            this.btnSaveAndRestart.UseVisualStyleBackColor = true;
            this.btnSaveAndRestart.Click += new System.EventHandler(this.btnSaveAndRestart_Click);
            this.btnSaveAndRestart.Enter += new System.EventHandler(this.btnSaveAndRestart_Enter);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.progressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 239);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(434, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(115, 17);
            this.toolStripStatusLabel1.Text = "Liberatio Agent 1.0.0";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.Value = 20;
            this.progressBar.Visible = false;
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(98, 122);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(48, 13);
            this.lblLocation.TabIndex = 10;
            this.lblLocation.Text = "Location";
            // 
            // txtLocation
            // 
            this.txtLocation.Location = new System.Drawing.Point(152, 119);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(165, 20);
            this.txtLocation.TabIndex = 9;
            // 
            // cmbRole
            // 
            this.cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRole.FormattingEnabled = true;
            this.cmbRole.Items.AddRange(new object[] {
            "Workstation",
            "Server"});
            this.cmbRole.Location = new System.Drawing.Point(152, 145);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Size = new System.Drawing.Size(165, 21);
            this.cmbRole.TabIndex = 11;
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(117, 148);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(29, 13);
            this.lblRole.TabIndex = 12;
            this.lblRole.Text = "Role";
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Location = new System.Drawing.Point(106, 21);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(37, 13);
            this.lblConnectionStatus.TabIndex = 14;
            this.lblConnectionStatus.Text = "Status";
            // 
            // lblConnectionStatusValue
            // 
            this.lblConnectionStatusValue.AutoSize = true;
            this.lblConnectionStatusValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionStatusValue.ForeColor = System.Drawing.Color.Red;
            this.lblConnectionStatusValue.Location = new System.Drawing.Point(149, 21);
            this.lblConnectionStatusValue.Name = "lblConnectionStatusValue";
            this.lblConnectionStatusValue.Size = new System.Drawing.Size(79, 13);
            this.lblConnectionStatusValue.TabIndex = 13;
            this.lblConnectionStatusValue.Text = "Unregistered";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(152, 46);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(152, 23);
            this.btnConnect.TabIndex = 15;
            this.btnConnect.Text = "Register with Liberatio.com";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // FormConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 261);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lblConnectionStatus);
            this.Controls.Add(this.lblConnectionStatusValue);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.cmbRole);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnSaveAndRestart);
            this.Controls.Add(this.lblUuid);
            this.Controls.Add(this.txtUuid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(450, 300);
            this.MinimumSize = new System.Drawing.Size(450, 300);
            this.Name = "FormConsole";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Liberatio Agent Console";
            this.Load += new System.EventHandler(this.FormConsole_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtUuid;
        private System.Windows.Forms.Label lblUuid;
        private System.Windows.Forms.Button btnSaveAndRestart;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.ComboBox cmbRole;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Label lblConnectionStatusValue;
        private System.Windows.Forms.Button btnConnect;

    }
}