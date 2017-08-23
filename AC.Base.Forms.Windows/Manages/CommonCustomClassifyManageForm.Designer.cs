namespace AC.Base.Forms.Windows.Manages
{
    partial class CommonCustomClassifyManageForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkEnabledDevice = new System.Windows.Forms.CheckBox();
            this.gbCommonClassify = new System.Windows.Forms.GroupBox();
            this.panButton2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panButton3 = new System.Windows.Forms.Panel();
            this.btnAccept = new System.Windows.Forms.Button();
            this.panButton = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.gbCommonClassify.SuspendLayout();
            this.panButton2.SuspendLayout();
            this.panButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "分类名称:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkEnabledDevice, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(316, 50);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(83, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(230, 21);
            this.txtName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "设备划分:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkEnabledDevice
            // 
            this.chkEnabledDevice.AutoSize = true;
            this.chkEnabledDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkEnabledDevice.Location = new System.Drawing.Point(83, 30);
            this.chkEnabledDevice.Name = "chkEnabledDevice";
            this.chkEnabledDevice.Size = new System.Drawing.Size(230, 17);
            this.chkEnabledDevice.TabIndex = 3;
            this.chkEnabledDevice.Text = "允许将设备划分到该分类下";
            this.chkEnabledDevice.UseVisualStyleBackColor = true;
            // 
            // gbCommonClassify
            // 
            this.gbCommonClassify.Controls.Add(this.tableLayoutPanel1);
            this.gbCommonClassify.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbCommonClassify.Location = new System.Drawing.Point(8, 8);
            this.gbCommonClassify.Name = "gbCommonClassify";
            this.gbCommonClassify.Size = new System.Drawing.Size(322, 83);
            this.gbCommonClassify.TabIndex = 2;
            this.gbCommonClassify.TabStop = false;
            // 
            // panButton2
            // 
            this.panButton2.Controls.Add(this.btnCancel);
            this.panButton2.Controls.Add(this.panButton3);
            this.panButton2.Controls.Add(this.btnAccept);
            this.panButton2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panButton2.Location = new System.Drawing.Point(159, 5);
            this.panButton2.Name = "panButton2";
            this.panButton2.Size = new System.Drawing.Size(163, 23);
            this.panButton2.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnCancel.Location = new System.Drawing.Point(80, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // panButton3
            // 
            this.panButton3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panButton3.Location = new System.Drawing.Point(75, 0);
            this.panButton3.Name = "panButton3";
            this.panButton3.Size = new System.Drawing.Size(5, 23);
            this.panButton3.TabIndex = 1;
            // 
            // btnAccept
            // 
            this.btnAccept.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAccept.Location = new System.Drawing.Point(0, 0);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "确定";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // panButton
            // 
            this.panButton.Controls.Add(this.panButton2);
            this.panButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panButton.Location = new System.Drawing.Point(8, 91);
            this.panButton.Name = "panButton";
            this.panButton.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panButton.Size = new System.Drawing.Size(322, 28);
            this.panButton.TabIndex = 3;
            // 
            // CommonCustomClassifyManageForm
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(338, 127);
            this.Controls.Add(this.gbCommonClassify);
            this.Controls.Add(this.panButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommonCustomClassifyManageForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "公共分类";
            this.Load += new System.EventHandler(this.CommonCustomClassifyManageForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gbCommonClassify.ResumeLayout(false);
            this.panButton2.ResumeLayout(false);
            this.panButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkEnabledDevice;
        private System.Windows.Forms.GroupBox gbCommonClassify;
        private System.Windows.Forms.Panel panButton2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panButton3;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Panel panButton;
    }
}