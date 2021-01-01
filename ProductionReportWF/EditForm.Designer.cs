
namespace ProductionReportWF
{
    partial class EditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditForm));
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.confirmButton = new System.Windows.Forms.Button();
            this.productNameLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.quantity = new System.Windows.Forms.TextBox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.tableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 4;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayout.Controls.Add(this.cancelButton, 0, 3);
            this.tableLayout.Controls.Add(this.confirmButton, 3, 3);
            this.tableLayout.Controls.Add(this.productNameLabel, 0, 0);
            this.tableLayout.Controls.Add(this.label1, 0, 1);
            this.tableLayout.Controls.Add(this.quantity, 2, 1);
            this.tableLayout.Controls.Add(this.errorLabel, 0, 2);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 4;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayout.Size = new System.Drawing.Size(419, 136);
            this.tableLayout.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cancelButton.Location = new System.Drawing.Point(3, 105);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(98, 28);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Anuluj";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // confirmButton
            // 
            this.confirmButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.confirmButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.confirmButton.Location = new System.Drawing.Point(315, 105);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(101, 28);
            this.confirmButton.TabIndex = 1;
            this.confirmButton.Text = "Zatwierdź";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // productNameLabel
            // 
            this.productNameLabel.AutoSize = true;
            this.tableLayout.SetColumnSpan(this.productNameLabel, 4);
            this.productNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productNameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.productNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.productNameLabel.Location = new System.Drawing.Point(3, 3);
            this.productNameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.productNameLabel.Name = "productNameLabel";
            this.productNameLabel.Size = new System.Drawing.Size(413, 28);
            this.productNameLabel.TabIndex = 2;
            this.productNameLabel.Text = "Produkt";
            this.productNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayout.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 28);
            this.label1.TabIndex = 3;
            this.label1.Text = "Podaj ilość";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // quantity
            // 
            this.tableLayout.SetColumnSpan(this.quantity, 2);
            this.quantity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quantity.Location = new System.Drawing.Point(211, 37);
            this.quantity.Name = "quantity";
            this.quantity.Size = new System.Drawing.Size(205, 20);
            this.quantity.TabIndex = 1;
            this.quantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.quantity.Enter += new System.EventHandler(this.quantity_Enter);
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.tableLayout.SetColumnSpan(this.errorLabel, 4);
            this.errorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.errorLabel.Location = new System.Drawing.Point(3, 71);
            this.errorLabel.Margin = new System.Windows.Forms.Padding(3);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(413, 28);
            this.errorLabel.TabIndex = 5;
            this.errorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(419, 136);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edycja pozycji";
            this.Load += new System.EventHandler(this.EditForm_Load);
            this.tableLayout.ResumeLayout(false);
            this.tableLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Label productNameLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox quantity;
        private System.Windows.Forms.Label errorLabel;
    }
}