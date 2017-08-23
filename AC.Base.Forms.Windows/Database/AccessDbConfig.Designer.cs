namespace AC.Base.Forms.Windows.Database
{
    partial class AccessDbConfig
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.labPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tlpPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnCreateMDB = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tlpPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 27);
            this.label2.TabIndex = 0;
            this.label2.Text = "文件名:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFileName
            // 
            this.txtFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFileName.Location = new System.Drawing.Point(0, 0);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(237, 21);
            this.txtFileName.TabIndex = 0;
            // 
            // labPassword
            // 
            this.labPassword.AutoSize = true;
            this.labPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labPassword.Location = new System.Drawing.Point(3, 27);
            this.labPassword.Name = "labPassword";
            this.labPassword.Size = new System.Drawing.Size(74, 27);
            this.labPassword.TabIndex = 1;
            this.labPassword.Text = "密码:";
            this.labPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPassword
            // 
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPassword.Location = new System.Drawing.Point(83, 30);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(277, 21);
            this.txtPassword.TabIndex = 2;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSelectFile.Location = new System.Drawing.Point(237, 0);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(40, 21);
            this.btnSelectFile.TabIndex = 1;
            this.btnSelectFile.Text = "...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtFileName);
            this.panel1.Controls.Add(this.btnSelectFile);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(83, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(277, 21);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 29);
            this.label1.TabIndex = 3;
            // 
            // tlpPanel
            // 
            this.tlpPanel.AutoSize = true;
            this.tlpPanel.ColumnCount = 2;
            this.tlpPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPanel.Controls.Add(this.label2, 0, 0);
            this.tlpPanel.Controls.Add(this.labPassword, 0, 1);
            this.tlpPanel.Controls.Add(this.txtPassword, 1, 1);
            this.tlpPanel.Controls.Add(this.panel1, 1, 0);
            this.tlpPanel.Controls.Add(this.label1, 0, 2);
            this.tlpPanel.Controls.Add(this.btnCreateMDB, 1, 2);
            this.tlpPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpPanel.Location = new System.Drawing.Point(0, 0);
            this.tlpPanel.Name = "tlpPanel";
            this.tlpPanel.RowCount = 3;
            this.tlpPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPanel.Size = new System.Drawing.Size(363, 83);
            this.tlpPanel.TabIndex = 0;
            // 
            // btnCreateMDB
            // 
            this.btnCreateMDB.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCreateMDB.Location = new System.Drawing.Point(260, 57);
            this.btnCreateMDB.Name = "btnCreateMDB";
            this.btnCreateMDB.Size = new System.Drawing.Size(100, 23);
            this.btnCreateMDB.TabIndex = 4;
            this.btnCreateMDB.Text = "新建数据库";
            this.btnCreateMDB.UseVisualStyleBackColor = true;
            this.btnCreateMDB.Click += new System.EventHandler(this.btnCreateMDB_Click);
            // 
            // AccessDbConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpPanel);
            this.Name = "AccessDbConfig";
            this.Size = new System.Drawing.Size(363, 88);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tlpPanel.ResumeLayout(false);
            this.tlpPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label labPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tlpPanel;
        private System.Windows.Forms.Button btnCreateMDB;

    }
}
