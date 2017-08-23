namespace AC.Base.Forms.Windows.Plugins
{
    partial class DeviceSearchSettingForm
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
            this.panButton = new System.Windows.Forms.Panel();
            this.chkUseDefaultConfig = new System.Windows.Forms.CheckBox();
            this.panButton2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panButton3 = new System.Windows.Forms.Panel();
            this.btnAccept = new System.Windows.Forms.Button();
            this.tacSetting = new System.Windows.Forms.TabControl();
            this.tapFilter = new System.Windows.Forms.TabPage();
            this.lvFilter = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tsFilter = new System.Windows.Forms.ToolStrip();
            this.tsbFilterUp = new System.Windows.Forms.ToolStripButton();
            this.tsbFilterDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbFilterSelectAll = new System.Windows.Forms.ToolStripButton();
            this.tsbFilterSelectClear = new System.Windows.Forms.ToolStripButton();
            this.tsbFilterSelectReverse = new System.Windows.Forms.ToolStripButton();
            this.tapList = new System.Windows.Forms.TabPage();
            this.lvList = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbListUp = new System.Windows.Forms.ToolStripButton();
            this.tsbListDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbListSelectAll = new System.Windows.Forms.ToolStripButton();
            this.tsbListSelectClear = new System.Windows.Forms.ToolStripButton();
            this.tsbListSelectReverse = new System.Windows.Forms.ToolStripButton();
            this.tapOrder = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkCanChildrenConfirm = new System.Windows.Forms.CheckBox();
            this.chkCanChildren = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.numPageSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.panButton.SuspendLayout();
            this.panButton2.SuspendLayout();
            this.tacSetting.SuspendLayout();
            this.tapFilter.SuspendLayout();
            this.tsFilter.SuspendLayout();
            this.tapList.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tapOrder.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPageSize)).BeginInit();
            this.SuspendLayout();
            // 
            // panButton
            // 
            this.panButton.Controls.Add(this.chkUseDefaultConfig);
            this.panButton.Controls.Add(this.panButton2);
            this.panButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panButton.Location = new System.Drawing.Point(8, 297);
            this.panButton.Name = "panButton";
            this.panButton.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panButton.Size = new System.Drawing.Size(456, 28);
            this.panButton.TabIndex = 7;
            // 
            // chkUseDefaultConfig
            // 
            this.chkUseDefaultConfig.AutoSize = true;
            this.chkUseDefaultConfig.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkUseDefaultConfig.Location = new System.Drawing.Point(0, 5);
            this.chkUseDefaultConfig.Name = "chkUseDefaultConfig";
            this.chkUseDefaultConfig.Size = new System.Drawing.Size(96, 23);
            this.chkUseDefaultConfig.TabIndex = 1;
            this.chkUseDefaultConfig.Text = "使用默认设置";
            this.chkUseDefaultConfig.UseVisualStyleBackColor = true;
            this.chkUseDefaultConfig.CheckedChanged += new System.EventHandler(this.chkUseDefaultConfig_CheckedChanged);
            // 
            // panButton2
            // 
            this.panButton2.Controls.Add(this.btnCancel);
            this.panButton2.Controls.Add(this.panButton3);
            this.panButton2.Controls.Add(this.btnAccept);
            this.panButton2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panButton2.Location = new System.Drawing.Point(293, 5);
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
            // tacSetting
            // 
            this.tacSetting.Controls.Add(this.tapFilter);
            this.tacSetting.Controls.Add(this.tapList);
            this.tacSetting.Controls.Add(this.tapOrder);
            this.tacSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tacSetting.Location = new System.Drawing.Point(8, 8);
            this.tacSetting.Name = "tacSetting";
            this.tacSetting.SelectedIndex = 0;
            this.tacSetting.Size = new System.Drawing.Size(456, 289);
            this.tacSetting.TabIndex = 8;
            this.tacSetting.SelectedIndexChanged += new System.EventHandler(this.tacSetting_SelectedIndexChanged);
            // 
            // tapFilter
            // 
            this.tapFilter.Controls.Add(this.lvFilter);
            this.tapFilter.Controls.Add(this.tsFilter);
            this.tapFilter.Location = new System.Drawing.Point(4, 22);
            this.tapFilter.Name = "tapFilter";
            this.tapFilter.Padding = new System.Windows.Forms.Padding(3);
            this.tapFilter.Size = new System.Drawing.Size(448, 263);
            this.tapFilter.TabIndex = 0;
            this.tapFilter.Text = "搜索条件";
            this.tapFilter.UseVisualStyleBackColor = true;
            // 
            // lvFilter
            // 
            this.lvFilter.CheckBoxes = true;
            this.lvFilter.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFilter.FullRowSelect = true;
            this.lvFilter.HideSelection = false;
            this.lvFilter.Location = new System.Drawing.Point(3, 28);
            this.lvFilter.Name = "lvFilter";
            this.lvFilter.ShowItemToolTips = true;
            this.lvFilter.Size = new System.Drawing.Size(442, 232);
            this.lvFilter.TabIndex = 1;
            this.lvFilter.UseCompatibleStateImageBehavior = false;
            this.lvFilter.View = System.Windows.Forms.View.Details;
            this.lvFilter.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvFilter_ItemChecked);
            this.lvFilter.SelectedIndexChanged += new System.EventHandler(this.lvFilter_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "搜索条件名称";
            this.columnHeader1.Width = 160;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "说明";
            this.columnHeader2.Width = 240;
            // 
            // tsFilter
            // 
            this.tsFilter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbFilterUp,
            this.tsbFilterDown,
            this.toolStripSeparator1,
            this.tsbFilterSelectAll,
            this.tsbFilterSelectClear,
            this.tsbFilterSelectReverse});
            this.tsFilter.Location = new System.Drawing.Point(3, 3);
            this.tsFilter.Name = "tsFilter";
            this.tsFilter.Size = new System.Drawing.Size(442, 25);
            this.tsFilter.TabIndex = 0;
            this.tsFilter.Text = "toolStrip1";
            // 
            // tsbFilterUp
            // 
            this.tsbFilterUp.Enabled = false;
            this.tsbFilterUp.Image = global::AC.Base.Forms.Windows.Properties.Resources.Arrowhead_Up;
            this.tsbFilterUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFilterUp.Name = "tsbFilterUp";
            this.tsbFilterUp.Size = new System.Drawing.Size(52, 22);
            this.tsbFilterUp.Text = "上移";
            this.tsbFilterUp.Click += new System.EventHandler(this.tsbFilterUp_Click);
            // 
            // tsbFilterDown
            // 
            this.tsbFilterDown.Enabled = false;
            this.tsbFilterDown.Image = global::AC.Base.Forms.Windows.Properties.Resources.Arrowhead_Down;
            this.tsbFilterDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFilterDown.Name = "tsbFilterDown";
            this.tsbFilterDown.Size = new System.Drawing.Size(52, 22);
            this.tsbFilterDown.Text = "下移";
            this.tsbFilterDown.Click += new System.EventHandler(this.tsbFilterDown_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbFilterSelectAll
            // 
            this.tsbFilterSelectAll.Image = global::AC.Base.Forms.Windows.Properties.Resources.SelectAll;
            this.tsbFilterSelectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFilterSelectAll.Name = "tsbFilterSelectAll";
            this.tsbFilterSelectAll.Size = new System.Drawing.Size(52, 22);
            this.tsbFilterSelectAll.Text = "全选";
            this.tsbFilterSelectAll.Click += new System.EventHandler(this.tsbFilterSelectAll_Click);
            // 
            // tsbFilterSelectClear
            // 
            this.tsbFilterSelectClear.Image = global::AC.Base.Forms.Windows.Properties.Resources.SelectClear;
            this.tsbFilterSelectClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFilterSelectClear.Name = "tsbFilterSelectClear";
            this.tsbFilterSelectClear.Size = new System.Drawing.Size(52, 22);
            this.tsbFilterSelectClear.Text = "全清";
            this.tsbFilterSelectClear.Click += new System.EventHandler(this.tsbFilterSelectClear_Click);
            // 
            // tsbFilterSelectReverse
            // 
            this.tsbFilterSelectReverse.Image = global::AC.Base.Forms.Windows.Properties.Resources.SelectReverse;
            this.tsbFilterSelectReverse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFilterSelectReverse.Name = "tsbFilterSelectReverse";
            this.tsbFilterSelectReverse.Size = new System.Drawing.Size(52, 22);
            this.tsbFilterSelectReverse.Text = "反选";
            this.tsbFilterSelectReverse.Click += new System.EventHandler(this.tsbFilterSelectReverse_Click);
            // 
            // tapList
            // 
            this.tapList.Controls.Add(this.lvList);
            this.tapList.Controls.Add(this.toolStrip1);
            this.tapList.Location = new System.Drawing.Point(4, 22);
            this.tapList.Name = "tapList";
            this.tapList.Padding = new System.Windows.Forms.Padding(3);
            this.tapList.Size = new System.Drawing.Size(448, 263);
            this.tapList.TabIndex = 1;
            this.tapList.Text = "搜索列表";
            this.tapList.UseVisualStyleBackColor = true;
            // 
            // lvList
            // 
            this.lvList.CheckBoxes = true;
            this.lvList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.lvList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvList.FullRowSelect = true;
            this.lvList.HideSelection = false;
            this.lvList.Location = new System.Drawing.Point(3, 28);
            this.lvList.Name = "lvList";
            this.lvList.ShowItemToolTips = true;
            this.lvList.Size = new System.Drawing.Size(442, 232);
            this.lvList.TabIndex = 3;
            this.lvList.UseCompatibleStateImageBehavior = false;
            this.lvList.View = System.Windows.Forms.View.Details;
            this.lvList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvList_ItemChecked);
            this.lvList.SelectedIndexChanged += new System.EventHandler(this.lvList_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "列名称";
            this.columnHeader3.Width = 400;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbListUp,
            this.tsbListDown,
            this.toolStripSeparator2,
            this.tsbListSelectAll,
            this.tsbListSelectClear,
            this.tsbListSelectReverse});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(442, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbListUp
            // 
            this.tsbListUp.Enabled = false;
            this.tsbListUp.Image = global::AC.Base.Forms.Windows.Properties.Resources.Arrowhead_Up;
            this.tsbListUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbListUp.Name = "tsbListUp";
            this.tsbListUp.Size = new System.Drawing.Size(52, 22);
            this.tsbListUp.Text = "上移";
            this.tsbListUp.Click += new System.EventHandler(this.tsbListUp_Click);
            // 
            // tsbListDown
            // 
            this.tsbListDown.Enabled = false;
            this.tsbListDown.Image = global::AC.Base.Forms.Windows.Properties.Resources.Arrowhead_Down;
            this.tsbListDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbListDown.Name = "tsbListDown";
            this.tsbListDown.Size = new System.Drawing.Size(52, 22);
            this.tsbListDown.Text = "下移";
            this.tsbListDown.Click += new System.EventHandler(this.tsbListDown_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbListSelectAll
            // 
            this.tsbListSelectAll.Image = global::AC.Base.Forms.Windows.Properties.Resources.SelectAll;
            this.tsbListSelectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbListSelectAll.Name = "tsbListSelectAll";
            this.tsbListSelectAll.Size = new System.Drawing.Size(52, 22);
            this.tsbListSelectAll.Text = "全选";
            this.tsbListSelectAll.Click += new System.EventHandler(this.tsbListSelectAll_Click);
            // 
            // tsbListSelectClear
            // 
            this.tsbListSelectClear.Image = global::AC.Base.Forms.Windows.Properties.Resources.SelectClear;
            this.tsbListSelectClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbListSelectClear.Name = "tsbListSelectClear";
            this.tsbListSelectClear.Size = new System.Drawing.Size(52, 22);
            this.tsbListSelectClear.Text = "全清";
            this.tsbListSelectClear.Click += new System.EventHandler(this.tsbListSelectClear_Click);
            // 
            // tsbListSelectReverse
            // 
            this.tsbListSelectReverse.Image = global::AC.Base.Forms.Windows.Properties.Resources.SelectReverse;
            this.tsbListSelectReverse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbListSelectReverse.Name = "tsbListSelectReverse";
            this.tsbListSelectReverse.Size = new System.Drawing.Size(52, 22);
            this.tsbListSelectReverse.Text = "反选";
            this.tsbListSelectReverse.Click += new System.EventHandler(this.tsbListSelectReverse_Click);
            // 
            // tapOrder
            // 
            this.tapOrder.Controls.Add(this.groupBox2);
            this.tapOrder.Controls.Add(this.panel2);
            this.tapOrder.Controls.Add(this.groupBox1);
            this.tapOrder.Location = new System.Drawing.Point(4, 22);
            this.tapOrder.Name = "tapOrder";
            this.tapOrder.Padding = new System.Windows.Forms.Padding(3);
            this.tapOrder.Size = new System.Drawing.Size(448, 263);
            this.tapOrder.TabIndex = 2;
            this.tapOrder.Text = "搜索排序";
            this.tapOrder.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.chkCanChildrenConfirm);
            this.groupBox2.Controls.Add(this.chkCanChildren);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 71);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(442, 64);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "显示下级设备";
            // 
            // chkCanChildrenConfirm
            // 
            this.chkCanChildrenConfirm.AutoSize = true;
            this.chkCanChildrenConfirm.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkCanChildrenConfirm.Enabled = false;
            this.chkCanChildrenConfirm.Location = new System.Drawing.Point(3, 39);
            this.chkCanChildrenConfirm.Name = "chkCanChildrenConfirm";
            this.chkCanChildrenConfirm.Padding = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.chkCanChildrenConfirm.Size = new System.Drawing.Size(436, 22);
            this.chkCanChildrenConfirm.TabIndex = 1;
            this.chkCanChildrenConfirm.Text = "通过查询仅在确实有下级设备时显示“+”加号。(速度稍慢)";
            this.chkCanChildrenConfirm.UseVisualStyleBackColor = true;
            // 
            // chkCanChildren
            // 
            this.chkCanChildren.AutoSize = true;
            this.chkCanChildren.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkCanChildren.Location = new System.Drawing.Point(3, 17);
            this.chkCanChildren.Name = "chkCanChildren";
            this.chkCanChildren.Padding = new System.Windows.Forms.Padding(3);
            this.chkCanChildren.Size = new System.Drawing.Size(436, 22);
            this.chkCanChildren.TabIndex = 0;
            this.chkCanChildren.Text = "在设备前显示“+”加号，展开后显示下级子设备。";
            this.chkCanChildren.UseVisualStyleBackColor = true;
            this.chkCanChildren.CheckedChanged += new System.EventHandler(this.chkCanChildren_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 66);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(442, 5);
            this.panel2.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(442, 63);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "每页显示记录数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label3.Location = new System.Drawing.Point(3, 38);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.label3.Size = new System.Drawing.Size(125, 22);
            this.label3.TabIndex = 1;
            this.label3.Text = "该值在 5 - 1000 之间";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.numPageSize);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(436, 21);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(175, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "条记录。";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numPageSize
            // 
            this.numPageSize.Dock = System.Windows.Forms.DockStyle.Left;
            this.numPageSize.Location = new System.Drawing.Point(95, 0);
            this.numPageSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPageSize.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numPageSize.Name = "numPageSize";
            this.numPageSize.Size = new System.Drawing.Size(80, 21);
            this.numPageSize.TabIndex = 1;
            this.numPageSize.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numPageSize.ValueChanged += new System.EventHandler(this.numPageSize_ValueChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "列表中每页显示";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DeviceSearchSettingForm
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(472, 333);
            this.Controls.Add(this.tacSetting);
            this.Controls.Add(this.panButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeviceSearchSettingForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "设备搜索设置";
            this.Load += new System.EventHandler(this.DeviceSearchSettingForm_Load);
            this.panButton.ResumeLayout(false);
            this.panButton.PerformLayout();
            this.panButton2.ResumeLayout(false);
            this.tacSetting.ResumeLayout(false);
            this.tapFilter.ResumeLayout(false);
            this.tapFilter.PerformLayout();
            this.tsFilter.ResumeLayout(false);
            this.tsFilter.PerformLayout();
            this.tapList.ResumeLayout(false);
            this.tapList.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tapOrder.ResumeLayout(false);
            this.tapOrder.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numPageSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panButton;
        private System.Windows.Forms.Panel panButton2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panButton3;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.TabControl tacSetting;
        private System.Windows.Forms.TabPage tapFilter;
        private System.Windows.Forms.TabPage tapList;
        private System.Windows.Forms.TabPage tapOrder;
        private System.Windows.Forms.ListView lvFilter;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStrip tsFilter;
        private System.Windows.Forms.ToolStripButton tsbFilterUp;
        private System.Windows.Forms.ToolStripButton tsbFilterDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbFilterSelectAll;
        private System.Windows.Forms.ToolStripButton tsbFilterSelectClear;
        private System.Windows.Forms.ToolStripButton tsbFilterSelectReverse;
        private System.Windows.Forms.ListView lvList;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbListUp;
        private System.Windows.Forms.ToolStripButton tsbListDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbListSelectAll;
        private System.Windows.Forms.ToolStripButton tsbListSelectClear;
        private System.Windows.Forms.ToolStripButton tsbListSelectReverse;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numPageSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkUseDefaultConfig;
        private System.Windows.Forms.CheckBox chkCanChildren;
        private System.Windows.Forms.CheckBox chkCanChildrenConfirm;
    }
}