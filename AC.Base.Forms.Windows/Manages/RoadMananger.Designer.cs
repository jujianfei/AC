namespace AC.Base.Forms.Windows.Manages
{
    partial class RoadMananger
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoadMananger));
            this.tvClassify = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddroot = new System.Windows.Forms.ToolStripButton();
            this.btnAddRoad = new System.Windows.Forms.ToolStripButton();
            this.btnAddlamp = new System.Windows.Forms.ToolStripButton();
            this.btnUpdate = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvClassify
            // 
            this.tvClassify.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvClassify.ImageIndex = 0;
            this.tvClassify.ImageList = this.imageList1;
            this.tvClassify.Location = new System.Drawing.Point(0, 25);
            this.tvClassify.Name = "tvClassify";
            this.tvClassify.SelectedImageIndex = 0;
            this.tvClassify.Size = new System.Drawing.Size(334, 393);
            this.tvClassify.TabIndex = 4;
            this.tvClassify.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvClassify_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 418);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(334, 36);
            this.panel1.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(256, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "确认";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddroot,
            this.btnAddRoad,
            this.btnAddlamp,
            this.btnUpdate,
            this.btnDelete});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(334, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAddroot
            // 
            this.btnAddroot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddroot.Image = ((System.Drawing.Image)(resources.GetObject("btnAddroot.Image")));
            this.btnAddroot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddroot.Name = "btnAddroot";
            this.btnAddroot.Size = new System.Drawing.Size(59, 22);
            this.btnAddroot.Text = "添根节点";
            this.btnAddroot.Click += new System.EventHandler(this.btnAddroot_Click);
            // 
            // btnAddRoad
            // 
            this.btnAddRoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddRoad.Image = ((System.Drawing.Image)(resources.GetObject("btnAddRoad.Image")));
            this.btnAddRoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddRoad.Name = "btnAddRoad";
            this.btnAddRoad.Size = new System.Drawing.Size(74, 22);
            this.btnAddRoad.Text = "添加路段(&C)";
            this.btnAddRoad.Click += new System.EventHandler(this.btnAddRoad_Click);
            // 
            // btnAddlamp
            // 
            this.btnAddlamp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddlamp.Image = ((System.Drawing.Image)(resources.GetObject("btnAddlamp.Image")));
            this.btnAddlamp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddlamp.Name = "btnAddlamp";
            this.btnAddlamp.Size = new System.Drawing.Size(72, 22);
            this.btnAddlamp.Text = "添加灯杆(&L)";
            this.btnAddlamp.Click += new System.EventHandler(this.btnAddlamp_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnUpdate.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdate.Image")));
            this.btnUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(50, 22);
            this.btnUpdate.Text = "修改(&A)";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(50, 22);
            this.btnDelete.Text = "删除(&D)";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "00032.ico");
            this.imageList1.Images.SetKeyName(1, "00721.ico");
            // 
            // RoadMananger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvClassify);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "RoadMananger";
            this.Size = new System.Drawing.Size(334, 454);
            this.panel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvClassify;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAddroot;
        private System.Windows.Forms.ToolStripButton btnAddRoad;
        private System.Windows.Forms.ToolStripButton btnAddlamp;
        private System.Windows.Forms.ToolStripButton btnUpdate;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ImageList imageList1;
    }
}
