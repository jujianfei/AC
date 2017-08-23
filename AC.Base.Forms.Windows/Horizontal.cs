using System;
using System.Collections.Generic;
using System.Text;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 水平线。
    /// </summary>
    public class Horizontal : System.Windows.Forms.Control
    {
        /// <summary>
        /// 初始化水平线的新实例。
        /// </summary>
        public Horizontal()
        {
            base.PaddingChanged += new EventHandler(Horizontal_PaddingChanged);
        }

        private void Horizontal_PaddingChanged(object sender, EventArgs e)
        {
            this.m_DefaultSize = new System.Drawing.Size(0, this.Padding.Top + this.Padding.Bottom + 2);
            this.MaximumSize = this.m_DefaultSize;
            this.MinimumSize = this.m_DefaultSize;
            this.Size = new System.Drawing.Size(this.Width, this.m_DefaultSize.Height);
        }

        private System.Drawing.Size m_DefaultSize = new System.Drawing.Size(0, 2);

        /// <summary>
        ///  获取以像素为单位的长度和高度，此长度和高度被指定为控件的默认最大大小。 
        /// </summary>
        protected override System.Drawing.Size DefaultMaximumSize
        {
            get
            {
                return this.m_DefaultSize;
            }
        }

        /// <summary>
        /// 获取以像素为单位的长度和高度，此长度和高度被指定为控件的默认最小大小。
        /// </summary>
        protected override System.Drawing.Size DefaultMinimumSize
        {
            get
            {
                return this.m_DefaultSize;
            }
        }

        /// <summary>
        /// 引发 Paint 事件。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            System.Drawing.Pen penGray = new System.Drawing.Pen(System.Drawing.Color.Silver);
            e.Graphics.DrawLine(penGray, base.Padding.Left, this.Padding.Top, this.Width - base.Padding.Right, this.Padding.Top);
            penGray.Dispose();

            System.Drawing.Pen penWhite = new System.Drawing.Pen(System.Drawing.Brushes.White);
            e.Graphics.DrawLine(penWhite, base.Padding.Left, this.Padding.Top + 1, this.Width - base.Padding.Right, this.Padding.Top + 1);
            penWhite.Dispose();

            base.OnPaint(e);
        }
    }
}
