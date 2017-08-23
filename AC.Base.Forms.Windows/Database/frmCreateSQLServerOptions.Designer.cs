namespace AC.Base.Forms.Windows.Database
{
    partial class frmCreateSQLServerOptions
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
            this.labName = new System.Windows.Forms.Label();
            this.labSizeUnit = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.labPath = new System.Windows.Forms.Label();
            this.numSize = new System.Windows.Forms.NumericUpDown();
            this.panPath = new System.Windows.Forms.Panel();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnPath = new System.Windows.Forms.Button();
            this.labSize = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.panSize = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.gbCreate = new System.Windows.Forms.GroupBox();
            this.tlpCreate = new System.Windows.Forms.TableLayoutPanel();
            this.chkDefault = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numSize)).BeginInit();
            this.panPath.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panSize.SuspendLayout();
            this.gbCreate.SuspendLayout();
            this.tlpCreate.SuspendLayout();
            this.SuspendLayout();
            // 
            // labName
            // 
            this.labName.AutoSize = true;
            this.labName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labName.Location = new System.Drawing.Point(3, 0);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(74, 27);
            this.labName.TabIndex = 0;
            this.labName.Text = "数据库名:";
            this.labName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labSizeUnit
            // 
            this.labSizeUnit.Dock = System.Windows.Forms.DockStyle.Left;
            this.labSizeUnit.Location = new System.Drawing.Point(80, 0);
            this.labSizeUnit.Name = "labSizeUnit";
            this.labSizeUnit.Size = new System.Drawing.Size(24, 22);
            this.labSizeUnit.TabIndex = 1;
            this.labSizeUnit.Text = "MB";
            this.labSizeUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(83, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(254, 21);
            this.txtName.TabIndex = 1;
            // 
            // labPath
            // 
            this.labPath.AutoSize = true;
            this.labPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labPath.Enabled = false;
            this.labPath.Location = new System.Drawing.Point(3, 49);
            this.labPath.Name = "labPath";
            this.labPath.Size = new System.Drawing.Size(74, 27);
            this.labPath.TabIndex = 4;
            this.labPath.Text = "存放路径:";
            this.labPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numSize
            // 
            this.numSize.Dock = System.Windows.Forms.DockStyle.Left;
            this.numSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSize.Location = new System.Drawing.Point(0, 0);
            this.numSize.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.numSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSize.Name = "numSize";
            this.numSize.Size = new System.Drawing.Size(80, 21);
            this.numSize.TabIndex = 0;
            this.numSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // panPath
            // 
            this.panPath.Controls.Add(this.txtPath);
            this.panPath.Controls.Add(this.btnPath);
            this.panPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panPath.Enabled = false;
            this.panPath.Location = new System.Drawing.Point(83, 52);
            this.panPath.Name = "panPath";
            this.panPath.Size = new System.Drawing.Size(254, 21);
            this.panPath.TabIndex = 3;
            // 
            // txtPath
            // 
            this.txtPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPath.Location = new System.Drawing.Point(0, 0);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(215, 21);
            this.txtPath.TabIndex = 0;
            // 
            // btnPath
            // 
            this.btnPath.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnPath.Location = new System.Drawing.Point(215, 0);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(39, 21);
            this.btnPath.TabIndex = 1;
            this.btnPath.Text = "...";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // labSize
            // 
            this.labSize.AutoSize = true;
            this.labSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labSize.Enabled = false;
            this.labSize.Location = new System.Drawing.Point(3, 76);
            this.labSize.Name = "labSize";
            this.labSize.Size = new System.Drawing.Size(74, 28);
            this.labSize.TabIndex = 5;
            this.labSize.Text = "初始大小:";
            this.labSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(8, 136);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(346, 29);
            this.panel2.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnAccept);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(179, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(167, 29);
            this.panel3.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(84, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(3, 3);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "确定";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // panSize
            // 
            this.panSize.Controls.Add(this.labSizeUnit);
            this.panSize.Controls.Add(this.numSize);
            this.panSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panSize.Enabled = false;
            this.panSize.Location = new System.Drawing.Point(83, 79);
            this.panSize.Name = "panSize";
            this.panSize.Size = new System.Drawing.Size(254, 22);
            this.panSize.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 22);
            this.label4.TabIndex = 2;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbCreate
            // 
            this.gbCreate.Controls.Add(this.tlpCreate);
            this.gbCreate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbCreate.Location = new System.Drawing.Point(8, 8);
            this.gbCreate.Name = "gbCreate";
            this.gbCreate.Size = new System.Drawing.Size(346, 157);
            this.gbCreate.TabIndex = 2;
            this.gbCreate.TabStop = false;
            this.gbCreate.Text = "创建选项";
            // 
            // tlpCreate
            // 
            this.tlpCreate.ColumnCount = 2;
            this.tlpCreate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpCreate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCreate.Controls.Add(this.labName, 0, 0);
            this.tlpCreate.Controls.Add(this.txtName, 1, 0);
            this.tlpCreate.Controls.Add(this.labPath, 0, 2);
            this.tlpCreate.Controls.Add(this.panPath, 1, 2);
            this.tlpCreate.Controls.Add(this.labSize, 0, 3);
            this.tlpCreate.Controls.Add(this.panSize, 1, 3);
            this.tlpCreate.Controls.Add(this.label4, 0, 1);
            this.tlpCreate.Controls.Add(this.chkDefault, 1, 1);
            this.tlpCreate.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpCreate.Location = new System.Drawing.Point(3, 17);
            this.tlpCreate.Name = "tlpCreate";
            this.tlpCreate.RowCount = 4;
            this.tlpCreate.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCreate.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCreate.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCreate.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCreate.Size = new System.Drawing.Size(340, 104);
            this.tlpCreate.TabIndex = 0;
            // 
            // chkDefault
            // 
            this.chkDefault.AutoSize = true;
            this.chkDefault.Checked = true;
            this.chkDefault.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDefault.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkDefault.Location = new System.Drawing.Point(83, 30);
            this.chkDefault.Name = "chkDefault";
            this.chkDefault.Size = new System.Drawing.Size(254, 16);
            this.chkDefault.TabIndex = 3;
            this.chkDefault.Text = "使用默认值";
            this.chkDefault.UseVisualStyleBackColor = true;
            this.chkDefault.CheckedChanged += new System.EventHandler(this.chkDefault_CheckedChanged);
            // 
            // frmCreateSQLServerOptions
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(362, 173);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.gbCreate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCreateSQLServerOptions";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "创建 SQL Server 数据库";
            ((System.ComponentModel.ISupportInitialize)(this.numSize)).EndInit();
            this.panPath.ResumeLayout(false);
            this.panPath.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panSize.ResumeLayout(false);
            this.gbCreate.ResumeLayout(false);
            this.tlpCreate.ResumeLayout(false);
            this.tlpCreate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labName;
        private System.Windows.Forms.Label labSizeUnit;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label labPath;
        private System.Windows.Forms.NumericUpDown numSize;
        private System.Windows.Forms.Panel panPath;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.Label labSize;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Panel panSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbCreate;
        private System.Windows.Forms.TableLayoutPanel tlpCreate;
        private System.Windows.Forms.CheckBox chkDefault;
    }
}