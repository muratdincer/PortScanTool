namespace PortScanTool.App
{
    partial class FrmMain
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
            this.TxtIPRange = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.LblParallelTaskCount = new System.Windows.Forms.Label();
            this.TbParallelTaskCount = new System.Windows.Forms.TrackBar();
            this.DgwScanResults = new System.Windows.Forms.DataGridView();
            this.LblDgwResult = new System.Windows.Forms.Label();
            this.LblStatus = new System.Windows.Forms.Label();
            this.LblStatusText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.TbParallelTaskCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgwScanResults)).BeginInit();
            this.SuspendLayout();
            // 
            // TxtIPRange
            // 
            this.TxtIPRange.Location = new System.Drawing.Point(89, 28);
            this.TxtIPRange.Mask = "###.###.###.###\\-###.###.###.###";
            this.TxtIPRange.Name = "TxtIPRange";
            this.TxtIPRange.Size = new System.Drawing.Size(228, 22);
            this.TxtIPRange.TabIndex = 0;
            this.TxtIPRange.Text = "192168  1  1192168  1 10";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP Range";
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(380, 28);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(75, 36);
            this.BtnStart.TabIndex = 2;
            this.BtnStart.Text = "Start";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.Enabled = false;
            this.BtnStop.Location = new System.Drawing.Point(469, 28);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(74, 36);
            this.BtnStop.TabIndex = 3;
            this.BtnStop.Text = "Stop";
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // LblParallelTaskCount
            // 
            this.LblParallelTaskCount.AutoSize = true;
            this.LblParallelTaskCount.Location = new System.Drawing.Point(17, 71);
            this.LblParallelTaskCount.Name = "LblParallelTaskCount";
            this.LblParallelTaskCount.Size = new System.Drawing.Size(131, 17);
            this.LblParallelTaskCount.TabIndex = 4;
            this.LblParallelTaskCount.Text = "Parallel Task Count";
            // 
            // TbParallelTaskCount
            // 
            this.TbParallelTaskCount.LargeChange = 1;
            this.TbParallelTaskCount.Location = new System.Drawing.Point(153, 71);
            this.TbParallelTaskCount.Margin = new System.Windows.Forms.Padding(2);
            this.TbParallelTaskCount.Name = "TbParallelTaskCount";
            this.TbParallelTaskCount.Size = new System.Drawing.Size(164, 56);
            this.TbParallelTaskCount.TabIndex = 5;
            this.TbParallelTaskCount.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TbParallelTaskCount_MouseUp);
            // 
            // DgwScanResults
            // 
            this.DgwScanResults.AllowUserToAddRows = false;
            this.DgwScanResults.AllowUserToDeleteRows = false;
            this.DgwScanResults.AllowUserToOrderColumns = true;
            this.DgwScanResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgwScanResults.Location = new System.Drawing.Point(20, 159);
            this.DgwScanResults.Name = "DgwScanResults";
            this.DgwScanResults.ReadOnly = true;
            this.DgwScanResults.RowHeadersWidth = 51;
            this.DgwScanResults.RowTemplate.Height = 24;
            this.DgwScanResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DgwScanResults.Size = new System.Drawing.Size(523, 325);
            this.DgwScanResults.TabIndex = 6;
            // 
            // LblDgwResult
            // 
            this.LblDgwResult.AutoSize = true;
            this.LblDgwResult.Location = new System.Drawing.Point(17, 132);
            this.LblDgwResult.Name = "LblDgwResult";
            this.LblDgwResult.Size = new System.Drawing.Size(55, 17);
            this.LblDgwResult.TabIndex = 7;
            this.LblDgwResult.Text = "Results";
            // 
            // LblStatus
            // 
            this.LblStatus.AutoSize = true;
            this.LblStatus.Location = new System.Drawing.Point(329, 136);
            this.LblStatus.Name = "LblStatus";
            this.LblStatus.Size = new System.Drawing.Size(56, 17);
            this.LblStatus.TabIndex = 8;
            this.LblStatus.Text = "Status :";
            // 
            // LblStatusText
            // 
            this.LblStatusText.AutoSize = true;
            this.LblStatusText.Location = new System.Drawing.Point(391, 137);
            this.LblStatusText.Name = "LblStatusText";
            this.LblStatusText.Size = new System.Drawing.Size(0, 17);
            this.LblStatusText.TabIndex = 9;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 490);
            this.Controls.Add(this.LblStatusText);
            this.Controls.Add(this.LblStatus);
            this.Controls.Add(this.LblDgwResult);
            this.Controls.Add(this.DgwScanResults);
            this.Controls.Add(this.TbParallelTaskCount);
            this.Controls.Add(this.LblParallelTaskCount);
            this.Controls.Add(this.BtnStop);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtIPRange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Port Scan Tool";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TbParallelTaskCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgwScanResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox TxtIPRange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.Label LblParallelTaskCount;
        private System.Windows.Forms.TrackBar TbParallelTaskCount;
        private System.Windows.Forms.DataGridView DgwScanResults;
        private System.Windows.Forms.Label LblDgwResult;
        private System.Windows.Forms.Label LblStatus;
        private System.Windows.Forms.Label LblStatusText;
    }
}

