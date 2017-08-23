namespace AC.Base.Forms.Windows.Manages
{
    partial class CommonCustomClassifyManage
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
            this.panButton = new System.Windows.Forms.Panel();
            this.btnUpdateRoot = new System.Windows.Forms.Button();
            this.btnAddRoot = new System.Windows.Forms.Button();
            this.tabClassify = new System.Windows.Forms.TabControl();
            this.btnDeleteRoot = new System.Windows.Forms.Button();
            this.panButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // panButton
            // 
            this.panButton.Controls.Add(this.btnDeleteRoot);
            this.panButton.Controls.Add(this.btnUpdateRoot);
            this.panButton.Controls.Add(this.btnAddRoot);
            this.panButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panButton.Location = new System.Drawing.Point(5, 292);
            this.panButton.Name = "panButton";
            this.panButton.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panButton.Size = new System.Drawing.Size(583, 28);
            this.panButton.TabIndex = 2;
            // 
            // btnUpdateRoot
            // 
            this.btnUpdateRoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateRoot.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnUpdateRoot.Enabled = false;
            this.btnUpdateRoot.Location = new System.Drawing.Point(374, 2);
            this.btnUpdateRoot.Name = "btnUpdateRoot";
            this.btnUpdateRoot.Size = new System.Drawing.Size(100, 23);
            this.btnUpdateRoot.TabIndex = 4;
            this.btnUpdateRoot.Text = "修改分类";
            this.btnUpdateRoot.UseVisualStyleBackColor = true;
            this.btnUpdateRoot.Click += new System.EventHandler(this.btnUpdateRoot_Click);
            // 
            // btnAddRoot
            // 
            this.btnAddRoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddRoot.Location = new System.Drawing.Point(268, 2);
            this.btnAddRoot.Name = "btnAddRoot";
            this.btnAddRoot.Size = new System.Drawing.Size(100, 23);
            this.btnAddRoot.TabIndex = 0;
            this.btnAddRoot.Text = "添加分类";
            this.btnAddRoot.UseVisualStyleBackColor = true;
            this.btnAddRoot.Click += new System.EventHandler(this.btnAddRoot_Click);
            // 
            // tabClassify
            // 
            this.tabClassify.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabClassify.Location = new System.Drawing.Point(5, 5);
            this.tabClassify.Name = "tabClassify";
            this.tabClassify.SelectedIndex = 0;
            this.tabClassify.Size = new System.Drawing.Size(583, 287);
            this.tabClassify.TabIndex = 3;
            this.tabClassify.SelectedIndexChanged += new System.EventHandler(this.tabClassify_SelectedIndexChanged);
            // 
            // btnDeleteRoot
            // 
            this.btnDeleteRoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteRoot.Location = new System.Drawing.Point(480, 2);
            this.btnDeleteRoot.Name = "btnDeleteRoot";
            this.btnDeleteRoot.Size = new System.Drawing.Size(100, 23);
            this.btnDeleteRoot.TabIndex = 5;
            this.btnDeleteRoot.Text = "删除分类";
            this.btnDeleteRoot.UseVisualStyleBackColor = true;
            this.btnDeleteRoot.Click += new System.EventHandler(this.btnDeleteRoot_Click);
            // 
            // CommonCustomClassifyManage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabClassify);
            this.Controls.Add(this.panButton);
            this.Name = "CommonCustomClassifyManage";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(593, 325);
            this.panButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panButton;
        private System.Windows.Forms.Button btnAddRoot;
        private System.Windows.Forms.TabControl tabClassify;
        private System.Windows.Forms.Button btnUpdateRoot;
        private System.Windows.Forms.Button btnDeleteRoot;
    }
}
