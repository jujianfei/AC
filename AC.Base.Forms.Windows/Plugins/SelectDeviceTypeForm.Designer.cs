namespace AC.Base.Forms.Windows.Plugins
{
    partial class SelectDeviceTypeForm
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
            this.panButton2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panButton3 = new System.Windows.Forms.Panel();
            this.btnAccept = new System.Windows.Forms.Button();
            this.panChannel = new System.Windows.Forms.Panel();
            this.cmbChannel = new System.Windows.Forms.ComboBox();
            this.labChannel = new System.Windows.Forms.Label();
            this.panParentDevice = new System.Windows.Forms.Panel();
            this.txtParentDevice = new System.Windows.Forms.TextBox();
            this.labParentDevice = new System.Windows.Forms.Label();
            this.panDeviceName = new System.Windows.Forms.Panel();
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.labDeviceName = new System.Windows.Forms.Label();
            this.scSelect = new System.Windows.Forms.SplitContainer();
            this.tvSelectDeviceSort = new System.Windows.Forms.TreeView();
            this.scDeviceType = new System.Windows.Forms.SplitContainer();
            this.lvSelectDeviceType = new System.Windows.Forms.ListView();
            this.panDeviceDescription = new System.Windows.Forms.Panel();
            this.scDeviceDescription = new System.Windows.Forms.SplitContainer();
            this.picDevicePhoto = new System.Windows.Forms.PictureBox();
            this.labDeviceDescription = new System.Windows.Forms.Label();
            this.horizontal1 = new AC.Base.Forms.Windows.Horizontal();
            this.panButton.SuspendLayout();
            this.panButton2.SuspendLayout();
            this.panChannel.SuspendLayout();
            this.panParentDevice.SuspendLayout();
            this.panDeviceName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scSelect)).BeginInit();
            this.scSelect.Panel1.SuspendLayout();
            this.scSelect.Panel2.SuspendLayout();
            this.scSelect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDeviceType)).BeginInit();
            this.scDeviceType.Panel1.SuspendLayout();
            this.scDeviceType.Panel2.SuspendLayout();
            this.scDeviceType.SuspendLayout();
            this.panDeviceDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDeviceDescription)).BeginInit();
            this.scDeviceDescription.Panel1.SuspendLayout();
            this.scDeviceDescription.Panel2.SuspendLayout();
            this.scDeviceDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDevicePhoto)).BeginInit();
            this.SuspendLayout();
            // 
            // panButton
            // 
            this.panButton.Controls.Add(this.panButton2);
            this.panButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panButton.Location = new System.Drawing.Point(8, 251);
            this.panButton.Name = "panButton";
            this.panButton.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panButton.Size = new System.Drawing.Size(542, 28);
            this.panButton.TabIndex = 5;
            // 
            // panButton2
            // 
            this.panButton2.Controls.Add(this.btnCancel);
            this.panButton2.Controls.Add(this.panButton3);
            this.panButton2.Controls.Add(this.btnAccept);
            this.panButton2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panButton2.Location = new System.Drawing.Point(379, 5);
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
            this.btnAccept.Enabled = false;
            this.btnAccept.Location = new System.Drawing.Point(0, 0);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "确定";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // panChannel
            // 
            this.panChannel.AutoSize = true;
            this.panChannel.Controls.Add(this.cmbChannel);
            this.panChannel.Controls.Add(this.labChannel);
            this.panChannel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panChannel.Location = new System.Drawing.Point(8, 8);
            this.panChannel.Name = "panChannel";
            this.panChannel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.panChannel.Size = new System.Drawing.Size(542, 25);
            this.panChannel.TabIndex = 0;
            // 
            // cmbChannel
            // 
            this.cmbChannel.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChannel.FormattingEnabled = true;
            this.cmbChannel.Location = new System.Drawing.Point(80, 0);
            this.cmbChannel.Name = "cmbChannel";
            this.cmbChannel.Size = new System.Drawing.Size(462, 20);
            this.cmbChannel.TabIndex = 1;
            this.cmbChannel.SelectedIndexChanged += new System.EventHandler(this.cmbChannel_SelectedIndexChanged);
            // 
            // labChannel
            // 
            this.labChannel.Dock = System.Windows.Forms.DockStyle.Left;
            this.labChannel.Location = new System.Drawing.Point(0, 0);
            this.labChannel.Name = "labChannel";
            this.labChannel.Size = new System.Drawing.Size(80, 20);
            this.labChannel.TabIndex = 0;
            this.labChannel.Text = "所属通道:";
            this.labChannel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panParentDevice
            // 
            this.panParentDevice.AutoSize = true;
            this.panParentDevice.Controls.Add(this.txtParentDevice);
            this.panParentDevice.Controls.Add(this.labParentDevice);
            this.panParentDevice.Dock = System.Windows.Forms.DockStyle.Top;
            this.panParentDevice.Location = new System.Drawing.Point(8, 33);
            this.panParentDevice.Name = "panParentDevice";
            this.panParentDevice.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.panParentDevice.Size = new System.Drawing.Size(542, 26);
            this.panParentDevice.TabIndex = 1;
            // 
            // txtParentDevice
            // 
            this.txtParentDevice.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtParentDevice.Location = new System.Drawing.Point(80, 0);
            this.txtParentDevice.Name = "txtParentDevice";
            this.txtParentDevice.ReadOnly = true;
            this.txtParentDevice.Size = new System.Drawing.Size(462, 21);
            this.txtParentDevice.TabIndex = 1;
            // 
            // labParentDevice
            // 
            this.labParentDevice.Dock = System.Windows.Forms.DockStyle.Left;
            this.labParentDevice.Location = new System.Drawing.Point(0, 0);
            this.labParentDevice.Name = "labParentDevice";
            this.labParentDevice.Size = new System.Drawing.Size(80, 21);
            this.labParentDevice.TabIndex = 0;
            this.labParentDevice.Text = "上级设备:";
            this.labParentDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panDeviceName
            // 
            this.panDeviceName.AutoSize = true;
            this.panDeviceName.Controls.Add(this.txtDeviceName);
            this.panDeviceName.Controls.Add(this.labDeviceName);
            this.panDeviceName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panDeviceName.Location = new System.Drawing.Point(8, 213);
            this.panDeviceName.Name = "panDeviceName";
            this.panDeviceName.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panDeviceName.Size = new System.Drawing.Size(542, 26);
            this.panDeviceName.TabIndex = 3;
            // 
            // txtDeviceName
            // 
            this.txtDeviceName.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtDeviceName.Location = new System.Drawing.Point(80, 5);
            this.txtDeviceName.Name = "txtDeviceName";
            this.txtDeviceName.Size = new System.Drawing.Size(462, 21);
            this.txtDeviceName.TabIndex = 1;
            this.txtDeviceName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtDeviceName_KeyUp);
            // 
            // labDeviceName
            // 
            this.labDeviceName.Dock = System.Windows.Forms.DockStyle.Left;
            this.labDeviceName.Location = new System.Drawing.Point(0, 5);
            this.labDeviceName.Name = "labDeviceName";
            this.labDeviceName.Size = new System.Drawing.Size(80, 21);
            this.labDeviceName.TabIndex = 0;
            this.labDeviceName.Text = "设备名称:";
            this.labDeviceName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // scSelect
            // 
            this.scSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scSelect.Location = new System.Drawing.Point(8, 59);
            this.scSelect.Name = "scSelect";
            // 
            // scSelect.Panel1
            // 
            this.scSelect.Panel1.Controls.Add(this.tvSelectDeviceSort);
            // 
            // scSelect.Panel2
            // 
            this.scSelect.Panel2.Controls.Add(this.scDeviceType);
            this.scSelect.Size = new System.Drawing.Size(542, 154);
            this.scSelect.SplitterDistance = 144;
            this.scSelect.TabIndex = 2;
            // 
            // tvSelectDeviceSort
            // 
            this.tvSelectDeviceSort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSelectDeviceSort.HideSelection = false;
            this.tvSelectDeviceSort.Location = new System.Drawing.Point(0, 0);
            this.tvSelectDeviceSort.Name = "tvSelectDeviceSort";
            this.tvSelectDeviceSort.Size = new System.Drawing.Size(144, 154);
            this.tvSelectDeviceSort.TabIndex = 0;
            this.tvSelectDeviceSort.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSelectDeviceSort_AfterSelect);
            // 
            // scDeviceType
            // 
            this.scDeviceType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scDeviceType.Location = new System.Drawing.Point(0, 0);
            this.scDeviceType.Name = "scDeviceType";
            // 
            // scDeviceType.Panel1
            // 
            this.scDeviceType.Panel1.Controls.Add(this.lvSelectDeviceType);
            // 
            // scDeviceType.Panel2
            // 
            this.scDeviceType.Panel2.Controls.Add(this.panDeviceDescription);
            this.scDeviceType.Size = new System.Drawing.Size(394, 154);
            this.scDeviceType.SplitterDistance = 209;
            this.scDeviceType.TabIndex = 0;
            // 
            // lvSelectDeviceType
            // 
            this.lvSelectDeviceType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSelectDeviceType.FullRowSelect = true;
            this.lvSelectDeviceType.HideSelection = false;
            this.lvSelectDeviceType.Location = new System.Drawing.Point(0, 0);
            this.lvSelectDeviceType.MultiSelect = false;
            this.lvSelectDeviceType.Name = "lvSelectDeviceType";
            this.lvSelectDeviceType.Size = new System.Drawing.Size(209, 154);
            this.lvSelectDeviceType.TabIndex = 0;
            this.lvSelectDeviceType.UseCompatibleStateImageBehavior = false;
            this.lvSelectDeviceType.SelectedIndexChanged += new System.EventHandler(this.lvSelectDeviceType_SelectedIndexChanged);
            // 
            // panDeviceDescription
            // 
            this.panDeviceDescription.BackColor = System.Drawing.Color.White;
            this.panDeviceDescription.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panDeviceDescription.Controls.Add(this.scDeviceDescription);
            this.panDeviceDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panDeviceDescription.Location = new System.Drawing.Point(0, 0);
            this.panDeviceDescription.Name = "panDeviceDescription";
            this.panDeviceDescription.Size = new System.Drawing.Size(181, 154);
            this.panDeviceDescription.TabIndex = 0;
            // 
            // scDeviceDescription
            // 
            this.scDeviceDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scDeviceDescription.Location = new System.Drawing.Point(0, 0);
            this.scDeviceDescription.Name = "scDeviceDescription";
            this.scDeviceDescription.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scDeviceDescription.Panel1
            // 
            this.scDeviceDescription.Panel1.Controls.Add(this.picDevicePhoto);
            // 
            // scDeviceDescription.Panel2
            // 
            this.scDeviceDescription.Panel2.Controls.Add(this.labDeviceDescription);
            this.scDeviceDescription.Size = new System.Drawing.Size(177, 150);
            this.scDeviceDescription.SplitterDistance = 71;
            this.scDeviceDescription.TabIndex = 0;
            // 
            // picDevicePhoto
            // 
            this.picDevicePhoto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picDevicePhoto.Location = new System.Drawing.Point(0, 0);
            this.picDevicePhoto.Name = "picDevicePhoto";
            this.picDevicePhoto.Size = new System.Drawing.Size(177, 71);
            this.picDevicePhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDevicePhoto.TabIndex = 0;
            this.picDevicePhoto.TabStop = false;
            // 
            // labDeviceDescription
            // 
            this.labDeviceDescription.AutoEllipsis = true;
            this.labDeviceDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labDeviceDescription.Location = new System.Drawing.Point(0, 0);
            this.labDeviceDescription.Name = "labDeviceDescription";
            this.labDeviceDescription.Size = new System.Drawing.Size(177, 75);
            this.labDeviceDescription.TabIndex = 0;
            this.labDeviceDescription.DoubleClick += new System.EventHandler(this.labDeviceDescription_DoubleClick);
            // 
            // horizontal1
            // 
            this.horizontal1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.horizontal1.Location = new System.Drawing.Point(8, 239);
            this.horizontal1.Name = "horizontal1";
            this.horizontal1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.horizontal1.Size = new System.Drawing.Size(542, 12);
            this.horizontal1.TabIndex = 4;
            this.horizontal1.Text = "horizontal1";
            // 
            // SelectDeviceTypeForm
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(558, 287);
            this.Controls.Add(this.scSelect);
            this.Controls.Add(this.panDeviceName);
            this.Controls.Add(this.panParentDevice);
            this.Controls.Add(this.horizontal1);
            this.Controls.Add(this.panChannel);
            this.Controls.Add(this.panButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectDeviceTypeForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "新建设备";
            this.panButton.ResumeLayout(false);
            this.panButton2.ResumeLayout(false);
            this.panChannel.ResumeLayout(false);
            this.panParentDevice.ResumeLayout(false);
            this.panParentDevice.PerformLayout();
            this.panDeviceName.ResumeLayout(false);
            this.panDeviceName.PerformLayout();
            this.scSelect.Panel1.ResumeLayout(false);
            this.scSelect.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scSelect)).EndInit();
            this.scSelect.ResumeLayout(false);
            this.scDeviceType.Panel1.ResumeLayout(false);
            this.scDeviceType.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDeviceType)).EndInit();
            this.scDeviceType.ResumeLayout(false);
            this.panDeviceDescription.ResumeLayout(false);
            this.scDeviceDescription.Panel1.ResumeLayout(false);
            this.scDeviceDescription.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDeviceDescription)).EndInit();
            this.scDeviceDescription.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picDevicePhoto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panButton;
        private System.Windows.Forms.Panel panButton2;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Panel panChannel;
        private System.Windows.Forms.ComboBox cmbChannel;
        private System.Windows.Forms.Label labChannel;
        private Horizontal horizontal1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panButton3;
        private System.Windows.Forms.Panel panParentDevice;
        private System.Windows.Forms.TextBox txtParentDevice;
        private System.Windows.Forms.Label labParentDevice;
        private System.Windows.Forms.Panel panDeviceName;
        private System.Windows.Forms.TextBox txtDeviceName;
        private System.Windows.Forms.Label labDeviceName;
        private System.Windows.Forms.SplitContainer scSelect;
        private System.Windows.Forms.TreeView tvSelectDeviceSort;
        private System.Windows.Forms.SplitContainer scDeviceType;
        private System.Windows.Forms.ListView lvSelectDeviceType;
        private System.Windows.Forms.Panel panDeviceDescription;
        private System.Windows.Forms.SplitContainer scDeviceDescription;
        private System.Windows.Forms.PictureBox picDevicePhoto;
        private System.Windows.Forms.Label labDeviceDescription;
    }
}