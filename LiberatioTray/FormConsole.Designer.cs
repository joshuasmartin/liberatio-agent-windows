namespace LiberatioTray
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnSaveAndRestart = new System.Windows.Forms.Button();
            this.lblRegistered = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblRegisteredStatus = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.cmbRole = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtUuid
            // 
            this.txtUuid.Location = new System.Drawing.Point(152, 55);
            this.txtUuid.Name = "txtUuid";
            this.txtUuid.Size = new System.Drawing.Size(215, 20);
            this.txtUuid.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Unique Identifier (UUID)";
            // 
            // btnSaveAndRestart
            // 
            this.btnSaveAndRestart.Location = new System.Drawing.Point(279, 202);
            this.btnSaveAndRestart.Name = "btnSaveAndRestart";
            this.btnSaveAndRestart.Size = new System.Drawing.Size(143, 23);
            this.btnSaveAndRestart.TabIndex = 2;
            this.btnSaveAndRestart.Text = "Save and Restart Service";
            this.btnSaveAndRestart.UseVisualStyleBackColor = true;
            this.btnSaveAndRestart.Click += new System.EventHandler(this.btnSaveAndRestart_Click);
            // 
            // lblRegistered
            // 
            this.lblRegistered.AutoSize = true;
            this.lblRegistered.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegistered.ForeColor = System.Drawing.Color.Red;
            this.lblRegistered.Location = new System.Drawing.Point(343, 9);
            this.lblRegistered.Name = "lblRegistered";
            this.lblRegistered.Size = new System.Drawing.Size(79, 13);
            this.lblRegistered.TabIndex = 4;
            this.lblRegistered.Text = "Unregistered";
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
            // lblRegisteredStatus
            // 
            this.lblRegisteredStatus.AutoSize = true;
            this.lblRegisteredStatus.Location = new System.Drawing.Point(241, 9);
            this.lblRegisteredStatus.Name = "lblRegisteredStatus";
            this.lblRegisteredStatus.Size = new System.Drawing.Size(96, 13);
            this.lblRegisteredStatus.TabIndex = 8;
            this.lblRegisteredStatus.Text = "Registration Status";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(98, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Location";
            // 
            // txtLocation
            // 
            this.txtLocation.Location = new System.Drawing.Point(152, 81);
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
            this.cmbRole.Location = new System.Drawing.Point(152, 107);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Size = new System.Drawing.Size(165, 21);
            this.cmbRole.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(117, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Role";
            // 
            // FormConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 261);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbRole);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.lblRegisteredStatus);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lblRegistered);
            this.Controls.Add(this.btnSaveAndRestart);
            this.Controls.Add(this.label1);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSaveAndRestart;
        private System.Windows.Forms.Label lblRegistered;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label lblRegisteredStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.ComboBox cmbRole;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripProgressBar progressBar;

    }
}