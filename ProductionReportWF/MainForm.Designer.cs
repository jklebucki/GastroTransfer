namespace ProductionReportWF
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.timeFrom = new System.Windows.Forms.DateTimePicker();
            this.timeTo = new System.Windows.Forms.DateTimePicker();
            this.labDate = new System.Windows.Forms.Label();
            this.labTime = new System.Windows.Forms.Label();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.infoLab = new System.Windows.Forms.Label();
            this.btnReport = new System.Windows.Forms.Button();
            this.dgReport = new System.Windows.Forms.DataGridView();
            this.contextMenuRowEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editQuantityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeDocumentInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgReport)).BeginInit();
            this.contextMenuRowEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgReport);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 120;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.dtFrom, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.dtTo, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.timeFrom, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.timeTo, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.labDate, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labTime, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnSettings, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnSubmit, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.button1, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.infoLab, 5, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnReport, 4, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(794, 114);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dtFrom
            // 
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(152, 12);
            this.dtFrom.Margin = new System.Windows.Forms.Padding(2);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(116, 20);
            this.dtFrom.TabIndex = 0;
            // 
            // dtTo
            // 
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(272, 12);
            this.dtTo.Margin = new System.Windows.Forms.Padding(2);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(116, 20);
            this.dtTo.TabIndex = 1;
            // 
            // timeFrom
            // 
            this.timeFrom.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeFrom.Location = new System.Drawing.Point(152, 40);
            this.timeFrom.Margin = new System.Windows.Forms.Padding(2);
            this.timeFrom.Name = "timeFrom";
            this.timeFrom.ShowUpDown = true;
            this.timeFrom.Size = new System.Drawing.Size(116, 20);
            this.timeFrom.TabIndex = 2;
            this.timeFrom.Value = new System.DateTime(2020, 1, 1, 0, 0, 0, 0);
            // 
            // timeTo
            // 
            this.timeTo.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeTo.Location = new System.Drawing.Point(272, 40);
            this.timeTo.Margin = new System.Windows.Forms.Padding(2);
            this.timeTo.Name = "timeTo";
            this.timeTo.ShowUpDown = true;
            this.timeTo.Size = new System.Drawing.Size(116, 20);
            this.timeTo.TabIndex = 3;
            this.timeTo.Value = new System.DateTime(2020, 1, 1, 23, 59, 59, 0);
            // 
            // labDate
            // 
            this.labDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labDate.AutoSize = true;
            this.labDate.Location = new System.Drawing.Point(3, 17);
            this.labDate.Margin = new System.Windows.Forms.Padding(3);
            this.labDate.Name = "labDate";
            this.labDate.Size = new System.Drawing.Size(144, 13);
            this.labDate.TabIndex = 4;
            this.labDate.Text = "Zakres dat";
            // 
            // labTime
            // 
            this.labTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labTime.AutoSize = true;
            this.labTime.Location = new System.Drawing.Point(3, 45);
            this.labTime.Margin = new System.Windows.Forms.Padding(3);
            this.labTime.Name = "labTime";
            this.labTime.Size = new System.Drawing.Size(144, 13);
            this.labTime.TabIndex = 5;
            this.labTime.Text = "Zakres godzin";
            // 
            // btnSettings
            // 
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSettings.Location = new System.Drawing.Point(152, 68);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(2);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(116, 24);
            this.btnSettings.TabIndex = 6;
            this.btnSettings.Text = "Ustawienia";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSubmit.Location = new System.Drawing.Point(272, 68);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(2);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(116, 24);
            this.btnSubmit.TabIndex = 7;
            this.btnSubmit.Text = "Wykonaj raport";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(393, 69);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 22);
            this.button1.TabIndex = 8;
            this.button1.Text = "Eksport do Excela";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.exportToExcelBtn_Click);
            // 
            // infoLab
            // 
            this.infoLab.AutoSize = true;
            this.infoLab.Location = new System.Drawing.Point(653, 69);
            this.infoLab.Margin = new System.Windows.Forms.Padding(3);
            this.infoLab.Name = "infoLab";
            this.infoLab.Size = new System.Drawing.Size(0, 13);
            this.infoLab.TabIndex = 9;
            // 
            // btnReport
            // 
            this.btnReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReport.Location = new System.Drawing.Point(513, 69);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(134, 22);
            this.btnReport.TabIndex = 10;
            this.btnReport.Text = "Podsumowanie do Excel";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // dgReport
            // 
            this.dgReport.AllowUserToAddRows = false;
            this.dgReport.AllowUserToDeleteRows = false;
            this.dgReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgReport.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgReport.Location = new System.Drawing.Point(3, 3);
            this.dgReport.Name = "dgReport";
            this.dgReport.ReadOnly = true;
            this.dgReport.RowHeadersVisible = false;
            this.dgReport.Size = new System.Drawing.Size(794, 323);
            this.dgReport.TabIndex = 0;
            this.dgReport.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgReport_RowEnter);
            // 
            // contextMenuRowEdit
            // 
            this.contextMenuRowEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editQuantityToolStripMenuItem,
            this.removeDocumentInfoToolStripMenuItem});
            this.contextMenuRowEdit.Name = "contextMenuRowEdit";
            this.contextMenuRowEdit.Size = new System.Drawing.Size(176, 48);
            // 
            // editQuantityToolStripMenuItem
            // 
            this.editQuantityToolStripMenuItem.Name = "editQuantityToolStripMenuItem";
            this.editQuantityToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.editQuantityToolStripMenuItem.Text = "Edytuj ilość";
            this.editQuantityToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // removeDocumentInfoToolStripMenuItem
            // 
            this.removeDocumentInfoToolStripMenuItem.Name = "removeDocumentInfoToolStripMenuItem";
            this.removeDocumentInfoToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.removeDocumentInfoToolStripMenuItem.Text = "Wycofaj dokument";
            this.removeDocumentInfoToolStripMenuItem.Click += new System.EventHandler(this.removeDocumentInfoToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Raport produkcji";
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgReport)).EndInit();
            this.contextMenuRowEdit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgReport;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.DateTimePicker timeFrom;
        private System.Windows.Forms.DateTimePicker timeTo;
        private System.Windows.Forms.Label labDate;
        private System.Windows.Forms.Label labTime;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.ContextMenuStrip contextMenuRowEdit;
        private System.Windows.Forms.ToolStripMenuItem editQuantityToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label infoLab;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.ToolStripMenuItem removeDocumentInfoToolStripMenuItem;
    }
}

