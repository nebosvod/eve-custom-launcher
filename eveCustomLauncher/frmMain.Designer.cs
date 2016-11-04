namespace eveCustomLauncher
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.mainTab = new System.Windows.Forms.TabPage();
            this.txtNewProfile = new System.Windows.Forms.TextBox();
            this.btnRunEVE = new System.Windows.Forms.Button();
            this.btnSaveProfile = new System.Windows.Forms.Button();
            this.lblSettingsProfile = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.cmbSettings = new System.Windows.Forms.ComboBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.tabs = new System.Windows.Forms.TabControl();
            this.diaSaveKey = new System.Windows.Forms.SaveFileDialog();
            this.mainTab.SuspendLayout();
            this.tabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTab
            // 
            this.mainTab.BackColor = System.Drawing.SystemColors.Control;
            this.mainTab.Controls.Add(this.txtNewProfile);
            this.mainTab.Controls.Add(this.btnRunEVE);
            this.mainTab.Controls.Add(this.btnSaveProfile);
            this.mainTab.Controls.Add(this.lblSettingsProfile);
            this.mainTab.Controls.Add(this.lblPassword);
            this.mainTab.Controls.Add(this.txtPassword);
            this.mainTab.Controls.Add(this.lblUsername);
            this.mainTab.Controls.Add(this.cmbSettings);
            this.mainTab.Controls.Add(this.txtUsername);
            this.mainTab.Location = new System.Drawing.Point(4, 22);
            this.mainTab.Name = "mainTab";
            this.mainTab.Padding = new System.Windows.Forms.Padding(3);
            this.mainTab.Size = new System.Drawing.Size(300, 160);
            this.mainTab.TabIndex = 2;
            this.mainTab.Text = "Create profile / Log in";
            // 
            // txtNewProfile
            // 
            this.txtNewProfile.Location = new System.Drawing.Point(136, 70);
            this.txtNewProfile.Name = "txtNewProfile";
            this.txtNewProfile.Size = new System.Drawing.Size(139, 20);
            this.txtNewProfile.TabIndex = 8;
            this.txtNewProfile.TabStop = false;
            this.txtNewProfile.Visible = false;
            this.txtNewProfile.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewProfile_KeyDown);
            // 
            // btnRunEVE
            // 
            this.btnRunEVE.Location = new System.Drawing.Point(150, 110);
            this.btnRunEVE.Name = "btnRunEVE";
            this.btnRunEVE.Size = new System.Drawing.Size(125, 31);
            this.btnRunEVE.TabIndex = 3;
            this.btnRunEVE.Text = "Run EVE client";
            this.btnRunEVE.UseVisualStyleBackColor = true;
            this.btnRunEVE.Click += new System.EventHandler(this.btnRunEVE_Click);
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.Location = new System.Drawing.Point(14, 110);
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.Size = new System.Drawing.Size(125, 31);
            this.btnSaveProfile.TabIndex = 4;
            this.btnSaveProfile.Text = "Save profile";
            this.btnSaveProfile.UseVisualStyleBackColor = true;
            this.btnSaveProfile.Click += new System.EventHandler(this.btnSaveProfile_Click);
            // 
            // lblSettingsProfile
            // 
            this.lblSettingsProfile.AutoSize = true;
            this.lblSettingsProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSettingsProfile.Location = new System.Drawing.Point(10, 70);
            this.lblSettingsProfile.Name = "lblSettingsProfile";
            this.lblSettingsProfile.Size = new System.Drawing.Size(119, 20);
            this.lblSettingsProfile.TabIndex = 5;
            this.lblSettingsProfile.Text = "Settings profile:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblPassword.Location = new System.Drawing.Point(10, 40);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(82, 20);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(136, 40);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(139, 20);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblUsername.Location = new System.Drawing.Point(10, 10);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(87, 20);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Username:";
            // 
            // cmbSettings
            // 
            this.cmbSettings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSettings.FormattingEnabled = true;
            this.cmbSettings.Location = new System.Drawing.Point(136, 70);
            this.cmbSettings.Name = "cmbSettings";
            this.cmbSettings.Size = new System.Drawing.Size(139, 21);
            this.cmbSettings.TabIndex = 2;
            this.cmbSettings.SelectedIndexChanged += new System.EventHandler(this.cmbSettings_SelectedIndexChanged);
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(136, 10);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(139, 20);
            this.txtUsername.TabIndex = 0;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.mainTab);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(308, 186);
            this.tabs.TabIndex = 0;
            this.tabs.TabStop = false;
            // 
            // diaSaveKey
            // 
            this.diaSaveKey.Filter = "EVE Custom Launcher profile (*.eclp)|*.eclp|All files|*.*";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 204);
            this.Controls.Add(this.tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMain";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.mainTab.ResumeLayout(false);
            this.mainTab.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage mainTab;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.ComboBox cmbSettings;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.Label lblSettingsProfile;
        private System.Windows.Forms.Button btnSaveProfile;
        private System.Windows.Forms.Button btnRunEVE;
        private System.Windows.Forms.TextBox txtNewProfile;
        private System.Windows.Forms.SaveFileDialog diaSaveKey;

    }
}