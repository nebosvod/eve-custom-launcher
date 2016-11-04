namespace eveCustomLauncher
{
    partial class ErrorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorForm));
            this.lblError = new System.Windows.Forms.Label();
            this.lblLog = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnCopyErrorData = new System.Windows.Forms.Button();
            this.txtErrorMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblError.Location = new System.Drawing.Point(8, 9);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(83, 20);
            this.lblError.TabIndex = 0;
            this.lblError.Text = "Exception:";
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblLog.Location = new System.Drawing.Point(9, 134);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(44, 20);
            this.lblLog.TabIndex = 2;
            this.lblLog.Text = "Log: ";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(12, 157);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(452, 175);
            this.txtLog.TabIndex = 3;
            // 
            // btnCopyErrorData
            // 
            this.btnCopyErrorData.Location = new System.Drawing.Point(13, 338);
            this.btnCopyErrorData.Name = "btnCopyErrorData";
            this.btnCopyErrorData.Size = new System.Drawing.Size(134, 24);
            this.btnCopyErrorData.TabIndex = 4;
            this.btnCopyErrorData.Text = "Copy error information";
            this.btnCopyErrorData.UseVisualStyleBackColor = true;
            this.btnCopyErrorData.Click += new System.EventHandler(this.btnCopyErrorData_Click);
            // 
            // txtErrorMessage
            // 
            this.txtErrorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.txtErrorMessage.Location = new System.Drawing.Point(12, 32);
            this.txtErrorMessage.Multiline = true;
            this.txtErrorMessage.Name = "txtErrorMessage";
            this.txtErrorMessage.ReadOnly = true;
            this.txtErrorMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrorMessage.Size = new System.Drawing.Size(452, 99);
            this.txtErrorMessage.TabIndex = 5;
            // 
            // ErrorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 371);
            this.Controls.Add(this.txtErrorMessage);
            this.Controls.Add(this.btnCopyErrorData);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.lblError);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ErrorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Error :(";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnCopyErrorData;
        private System.Windows.Forms.TextBox txtErrorMessage;
    }
}