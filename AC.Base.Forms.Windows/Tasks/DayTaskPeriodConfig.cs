using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Tasks;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows.Tasks
{
    /// <summary>
    /// 每日执行一次的自动任务周期配置界面。
    /// </summary>
    [Control(typeof(AC.Base.Tasks.DayTaskPeriod))]
    public class DayTaskPeriodConfig : System.Windows.Forms.Control, ITaskPeriodConfigControl
    {
        private DayTaskPeriod m_TaskPeriod;
        private System.Windows.Forms.TableLayoutPanel tlpPeriod;
        private System.Windows.Forms.Label labDay;
        private System.Windows.Forms.NumericUpDown numHour;
        private System.Windows.Forms.Label labHour;
        private System.Windows.Forms.NumericUpDown numMinute;
        private System.Windows.Forms.Label labMinute;

        #region ITaskPeriodConfigControl 成员

        /// <summary>
        /// 设置需配置的任务周期对象。
        /// </summary>
        /// <param name="taskPeriod">任务周期。</param>
        /// <param name="currentTaskConfig">设置周期时所配置的任务配置对象。</param>
        public void SetTaskPeriod(TaskPeriod taskPeriod, TaskConfig currentTaskConfig)
        {
            this.m_TaskPeriod = taskPeriod as DayTaskPeriod;

            this.tlpPeriod = new System.Windows.Forms.TableLayoutPanel();
            this.labDay = new System.Windows.Forms.Label();
            this.numHour = new System.Windows.Forms.NumericUpDown();
            this.labHour = new System.Windows.Forms.Label();
            this.numMinute = new System.Windows.Forms.NumericUpDown();
            this.labMinute = new System.Windows.Forms.Label();

            this.tlpPeriod.ColumnCount = 6;
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPeriod.Controls.Add(this.labDay, 0, 0);
            this.tlpPeriod.Controls.Add(this.numHour, 1, 0);
            this.tlpPeriod.Controls.Add(this.labHour, 2, 0);
            this.tlpPeriod.Controls.Add(this.numMinute, 3, 0);
            this.tlpPeriod.Controls.Add(this.labMinute, 4, 0);
            this.tlpPeriod.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpPeriod.Location = new System.Drawing.Point(0, 0);
            this.tlpPeriod.RowCount = 1;
            this.tlpPeriod.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPeriod.Size = new System.Drawing.Size(554, 25);
            this.tlpPeriod.TabIndex = 0;

            this.labDay.AutoSize = true;
            this.labDay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labDay.Location = new System.Drawing.Point(3, 0);
            this.labDay.Size = new System.Drawing.Size(29, 25);
            this.labDay.TabIndex = 0;
            this.labDay.Text = "每天";
            this.labDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.numHour.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numHour.Location = new System.Drawing.Point(38, 3);
            this.numHour.Maximum = new decimal(new int[] { 23, 0, 0, 0 });
            this.numHour.Size = new System.Drawing.Size(50, 21);
            this.numHour.TabIndex = 1;

            this.labHour.AutoSize = true;
            this.labHour.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labHour.Location = new System.Drawing.Point(94, 0);
            this.labHour.Size = new System.Drawing.Size(17, 25);
            this.labHour.TabIndex = 2;
            this.labHour.Text = "时";
            this.labHour.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.numMinute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numMinute.Location = new System.Drawing.Point(117, 3);
            this.numMinute.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            this.numMinute.Size = new System.Drawing.Size(50, 21);
            this.numMinute.TabIndex = 3;

            this.labMinute.AutoSize = true;
            this.labMinute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labMinute.Location = new System.Drawing.Point(173, 0);
            this.labMinute.Size = new System.Drawing.Size(65, 25);
            this.labMinute.TabIndex = 4;
            this.labMinute.Text = "分开始执行";
            this.labMinute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Controls.Add(this.tlpPeriod);

            this.numHour.Value = this.m_TaskPeriod.TimeNum / 10000;
            this.numMinute.Value = (this.m_TaskPeriod.TimeNum / 100) % 100;
        }

        /// <summary>
        /// 获取配置的任务周期对象。
        /// </summary>
        /// <returns>任务周期。</returns>
        public TaskPeriod GetTaskPeriod()
        {
            this.m_TaskPeriod.TimeNum = (int)(this.numHour.Value) * 10000 + (int)(this.numMinute.Value) * 100;
            return this.m_TaskPeriod;
        }

        #endregion
    }
}
