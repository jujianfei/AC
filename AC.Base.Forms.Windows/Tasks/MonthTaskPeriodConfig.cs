using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Tasks;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows.Tasks
{
    /// <summary>
    /// 每月执行一次的自动任务周期配置界面。
    /// </summary>
    [Control(typeof(AC.Base.Tasks.MonthTaskPeriod))]
    public class MonthTaskPeriodConfig : System.Windows.Forms.Control, ITaskPeriodConfigControl
    {
        private MonthTaskPeriod m_TaskPeriod;
        private System.Windows.Forms.TableLayoutPanel tlpPeriod;
        private System.Windows.Forms.Label labMonth;
        private System.Windows.Forms.ComboBox cmbDay;
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
            this.m_TaskPeriod = taskPeriod as MonthTaskPeriod;

            this.tlpPeriod = new System.Windows.Forms.TableLayoutPanel();
            this.labMonth = new System.Windows.Forms.Label();
            this.cmbDay = new ComboBox();
            this.labDay = new Label();
            this.numHour = new System.Windows.Forms.NumericUpDown();
            this.labHour = new System.Windows.Forms.Label();
            this.numMinute = new System.Windows.Forms.NumericUpDown();
            this.labMinute = new System.Windows.Forms.Label();

            this.tlpPeriod.ColumnCount = 8;
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPeriod.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPeriod.Controls.Add(this.labMonth, 0, 0);
            this.tlpPeriod.Controls.Add(this.cmbDay, 1, 0);
            this.tlpPeriod.Controls.Add(this.labDay, 2, 0);
            this.tlpPeriod.Controls.Add(this.numHour, 3, 0);
            this.tlpPeriod.Controls.Add(this.labHour, 4, 0);
            this.tlpPeriod.Controls.Add(this.numMinute, 5, 0);
            this.tlpPeriod.Controls.Add(this.labMinute, 6, 0);
            this.tlpPeriod.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpPeriod.RowCount = 1;
            this.tlpPeriod.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPeriod.Size = new System.Drawing.Size(554, 25);

            this.labMonth.AutoSize = true;
            this.labMonth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labMonth.Size = new System.Drawing.Size(29, 25);
            this.labMonth.Text = "每月";
            this.labMonth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.cmbDay.Dock = DockStyle.Fill;
            this.cmbDay.Size = new System.Drawing.Size(50, 21);
            this.cmbDay.DropDownStyle = ComboBoxStyle.DropDownList;
            for (int intIndex = 1; intIndex <= 31; intIndex++)
            {
                this.cmbDay.Items.Add(intIndex);
                if ((this.m_TaskPeriod.DateTimeNum / 10000) == intIndex)
                {
                    this.cmbDay.SelectedIndex = this.cmbDay.Items.Count - 1;
                }
            }
            if (this.cmbDay.SelectedIndex == -1)
            {
                this.cmbDay.SelectedIndex = 0;
            }

            this.labDay.AutoSize = true;
            this.labDay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labDay.Text = "日";
            this.labDay.Size = new System.Drawing.Size(17, 25);
            this.labDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.numHour.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numHour.Maximum = new decimal(new int[] { 23, 0, 0, 0 });
            this.numHour.Size = new System.Drawing.Size(50, 21);

            this.labHour.AutoSize = true;
            this.labHour.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labHour.Text = "时";
            this.labHour.Size = new System.Drawing.Size(17, 25);
            this.labHour.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.numMinute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numMinute.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            this.numMinute.Size = new System.Drawing.Size(50, 21);

            this.labMinute.AutoSize = true;
            this.labMinute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labMinute.Text = "分开始执行";
            this.labMinute.Size = new System.Drawing.Size(65, 25);
            this.labMinute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Controls.Add(this.tlpPeriod);

            this.numHour.Value = (this.m_TaskPeriod.DateTimeNum / 100) % 100;
            this.numMinute.Value = this.m_TaskPeriod.DateTimeNum % 100;
        }

        /// <summary>
        /// 获取配置的任务周期对象。
        /// </summary>
        /// <returns>任务周期。</returns>
        public TaskPeriod GetTaskPeriod()
        {
            this.m_TaskPeriod.DateTimeNum = ((this.cmbDay.SelectedIndex + 1) * 10000) + (int)(this.numHour.Value) * 100 + (int)(this.numMinute.Value);
            return this.m_TaskPeriod;
        }

        #endregion
    }
}
