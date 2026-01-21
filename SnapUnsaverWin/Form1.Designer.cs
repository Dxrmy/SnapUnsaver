namespace SnapUnsaverWin
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.phonePanel = new System.Windows.Forms.Panel();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.picSavedPreview = new System.Windows.Forms.PictureBox();
            this.picUnsavePreview = new System.Windows.Forms.PictureBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.picSavedPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnsavePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // phonePanel
            // 
            this.phonePanel.BackColor = System.Drawing.Color.Black;
            this.phonePanel.Location = new System.Drawing.Point(12, 12);
            this.phonePanel.Name = "phonePanel";
            this.phonePanel.Size = new System.Drawing.Size(400, 800);
            this.phonePanel.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(418, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(150, 40);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start Automation";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(574, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(150, 40);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.Black;
            this.txtLog.ForeColor = System.Drawing.Color.Lime;
            this.txtLog.Location = new System.Drawing.Point(418, 58);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(306, 450);
            this.txtLog.TabIndex = 3;
            // 
            // picSavedPreview
            // 
            this.picSavedPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSavedPreview.Location = new System.Drawing.Point(418, 514);
            this.picSavedPreview.Name = "picSavedPreview";
            this.picSavedPreview.Size = new System.Drawing.Size(150, 60);
            this.picSavedPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picSavedPreview.TabIndex = 4;
            this.picSavedPreview.TabStop = false;
            // 
            // picUnsavePreview
            // 
            this.picUnsavePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picUnsavePreview.Location = new System.Drawing.Point(574, 514);
            this.picUnsavePreview.Name = "picUnsavePreview";
            this.picUnsavePreview.Size = new System.Drawing.Size(150, 60);
            this.picUnsavePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picUnsavePreview.TabIndex = 5;
            this.picUnsavePreview.TabStop = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 824);
            this.Controls.Add(this.picUnsavePreview);
            this.Controls.Add(this.picSavedPreview);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.phonePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "SnapUnsaver Utility (C#)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picSavedPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnsavePreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Panel phonePanel;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.PictureBox picSavedPreview;
        private System.Windows.Forms.PictureBox picUnsavePreview;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}
