namespace AC.Base.Forms.Windows.Database
{
    partial class SQLServerDbConfig
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtLoginPassword = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.labInstance = new System.Windows.Forms.Label();
            this.cmbInstance = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labInstanceMsg = new System.Windows.Forms.Label();
            this.labLoginSecure = new System.Windows.Forms.Label();
            this.radLoginWindows = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.radLoginSQLServer = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtLoginUsername = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tlpPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.labDatabase = new System.Windows.Forms.Label();
            this.cmbDatabase = new System.Windows.Forms.ComboBox();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tlpPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtLoginPassword);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(83, 117);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 21);
            this.panel2.TabIndex = 11;
            // 
            // txtLoginPassword
            // 
            this.txtLoginPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLoginPassword.Enabled = false;
            this.txtLoginPassword.Location = new System.Drawing.Point(70, 0);
            this.txtLoginPassword.Name = "txtLoginPassword";
            this.txtLoginPassword.PasswordChar = '*';
            this.txtLoginPassword.Size = new System.Drawing.Size(244, 21);
            this.txtLoginPassword.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Left;
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Name = "label9";
            this.label9.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.label9.Size = new System.Drawing.Size(70, 21);
            this.label9.TabIndex = 0;
            this.label9.Text = "密码：";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labInstance
            // 
            this.labInstance.AutoSize = true;
            this.labInstance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labInstance.Location = new System.Drawing.Point(3, 0);
            this.labInstance.Name = "labInstance";
            this.labInstance.Size = new System.Drawing.Size(74, 26);
            this.labInstance.TabIndex = 0;
            this.labInstance.Text = "服务器实例:";
            this.labInstance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbInstance
            // 
            this.cmbInstance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbInstance.FormattingEnabled = true;
            this.cmbInstance.Location = new System.Drawing.Point(83, 3);
            this.cmbInstance.Name = "cmbInstance";
            this.cmbInstance.Size = new System.Drawing.Size(314, 20);
            this.cmbInstance.TabIndex = 1;
            this.cmbInstance.SelectedIndexChanged += new System.EventHandler(this.cmbInstance_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 17);
            this.label2.TabIndex = 2;
            // 
            // labInstanceMsg
            // 
            this.labInstanceMsg.AutoSize = true;
            this.labInstanceMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labInstanceMsg.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labInstanceMsg.Location = new System.Drawing.Point(83, 26);
            this.labInstanceMsg.Name = "labInstanceMsg";
            this.labInstanceMsg.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.labInstanceMsg.Size = new System.Drawing.Size(314, 17);
            this.labInstanceMsg.TabIndex = 3;
            this.labInstanceMsg.Text = "如果数据库在本机，可以用“.”代替";
            // 
            // labLoginSecure
            // 
            this.labLoginSecure.AutoSize = true;
            this.labLoginSecure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labLoginSecure.Location = new System.Drawing.Point(3, 43);
            this.labLoginSecure.Name = "labLoginSecure";
            this.labLoginSecure.Size = new System.Drawing.Size(74, 22);
            this.labLoginSecure.TabIndex = 4;
            this.labLoginSecure.Text = "身份验证:";
            this.labLoginSecure.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // radLoginWindows
            // 
            this.radLoginWindows.AutoSize = true;
            this.radLoginWindows.Checked = true;
            this.radLoginWindows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radLoginWindows.Location = new System.Drawing.Point(83, 46);
            this.radLoginWindows.Name = "radLoginWindows";
            this.radLoginWindows.Size = new System.Drawing.Size(314, 16);
            this.radLoginWindows.TabIndex = 5;
            this.radLoginWindows.TabStop = true;
            this.radLoginWindows.Text = "使用 Windows 身份验证";
            this.radLoginWindows.UseVisualStyleBackColor = true;
            this.radLoginWindows.CheckedChanged += new System.EventHandler(this.radLoginWindows_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 22);
            this.label5.TabIndex = 6;
            // 
            // radLoginSQLServer
            // 
            this.radLoginSQLServer.AutoSize = true;
            this.radLoginSQLServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radLoginSQLServer.Location = new System.Drawing.Point(83, 68);
            this.radLoginSQLServer.Name = "radLoginSQLServer";
            this.radLoginSQLServer.Size = new System.Drawing.Size(314, 16);
            this.radLoginSQLServer.TabIndex = 7;
            this.radLoginSQLServer.Text = "使用 SQL Server 身份验证";
            this.radLoginSQLServer.UseVisualStyleBackColor = true;
            this.radLoginSQLServer.CheckedChanged += new System.EventHandler(this.radLoginSQLServer_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 27);
            this.label6.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtLoginUsername);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(83, 90);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 21);
            this.panel1.TabIndex = 9;
            // 
            // txtLoginUsername
            // 
            this.txtLoginUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLoginUsername.Enabled = false;
            this.txtLoginUsername.Location = new System.Drawing.Point(70, 0);
            this.txtLoginUsername.Name = "txtLoginUsername";
            this.txtLoginUsername.Size = new System.Drawing.Size(244, 21);
            this.txtLoginUsername.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.Dock = System.Windows.Forms.DockStyle.Left;
            this.label7.Location = new System.Drawing.Point(0, 0);
            this.label7.Name = "label7";
            this.label7.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.label7.Size = new System.Drawing.Size(70, 21);
            this.label7.TabIndex = 0;
            this.label7.Text = "用户名：";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlpPanel
            // 
            this.tlpPanel.AutoSize = true;
            this.tlpPanel.ColumnCount = 2;
            this.tlpPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPanel.Controls.Add(this.panel2, 0, 5);
            this.tlpPanel.Controls.Add(this.labInstance, 0, 0);
            this.tlpPanel.Controls.Add(this.cmbInstance, 1, 0);
            this.tlpPanel.Controls.Add(this.label2, 0, 1);
            this.tlpPanel.Controls.Add(this.labInstanceMsg, 1, 1);
            this.tlpPanel.Controls.Add(this.labLoginSecure, 0, 2);
            this.tlpPanel.Controls.Add(this.radLoginWindows, 1, 2);
            this.tlpPanel.Controls.Add(this.label5, 0, 3);
            this.tlpPanel.Controls.Add(this.radLoginSQLServer, 1, 3);
            this.tlpPanel.Controls.Add(this.label6, 0, 4);
            this.tlpPanel.Controls.Add(this.panel1, 1, 4);
            this.tlpPanel.Controls.Add(this.label8, 0, 5);
            this.tlpPanel.Controls.Add(this.labDatabase, 0, 6);
            this.tlpPanel.Controls.Add(this.cmbDatabase, 1, 6);
            this.tlpPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpPanel.Location = new System.Drawing.Point(0, 0);
            this.tlpPanel.Name = "tlpPanel";
            this.tlpPanel.RowCount = 7;
            this.tlpPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPanel.Size = new System.Drawing.Size(400, 167);
            this.tlpPanel.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(3, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 27);
            this.label8.TabIndex = 9;
            // 
            // labDatabase
            // 
            this.labDatabase.AutoSize = true;
            this.labDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labDatabase.Location = new System.Drawing.Point(3, 141);
            this.labDatabase.Name = "labDatabase";
            this.labDatabase.Size = new System.Drawing.Size(74, 26);
            this.labDatabase.TabIndex = 10;
            this.labDatabase.Text = "数据库:";
            this.labDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbDatabase
            // 
            this.cmbDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbDatabase.FormattingEnabled = true;
            this.cmbDatabase.Location = new System.Drawing.Point(83, 144);
            this.cmbDatabase.Name = "cmbDatabase";
            this.cmbDatabase.Size = new System.Drawing.Size(314, 20);
            this.cmbDatabase.TabIndex = 11;
            this.cmbDatabase.DropDown += new System.EventHandler(this.cmbDatabase_DropDown);
            this.cmbDatabase.SelectedIndexChanged += new System.EventHandler(this.cmbDatabase_SelectedIndexChanged);
            this.cmbDatabase.Leave += new System.EventHandler(this.cmbDatabase_Leave);
            // 
            // SQLServerDbConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpPanel);
            this.Name = "SQLServerDbConfig";
            this.Size = new System.Drawing.Size(400, 167);
            this.Load += new System.EventHandler(this.SQLServerDbConfig_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tlpPanel.ResumeLayout(false);
            this.tlpPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtLoginPassword;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labInstance;
        private System.Windows.Forms.ComboBox cmbInstance;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labInstanceMsg;
        private System.Windows.Forms.Label labLoginSecure;
        private System.Windows.Forms.RadioButton radLoginWindows;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radLoginSQLServer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtLoginUsername;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TableLayoutPanel tlpPanel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labDatabase;
        private System.Windows.Forms.ComboBox cmbDatabase;
    }
}
