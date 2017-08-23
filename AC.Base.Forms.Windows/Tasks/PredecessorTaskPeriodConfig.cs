using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Tasks;

namespace AC.Base.Forms.Windows.Tasks
{
    /// <summary>
    /// 指定任务完成后运行的自动任务周期配置界面。
    /// </summary>
    [Control(typeof(AC.Base.Tasks.PredecessorTaskPeriod))]
    public class PredecessorTaskPeriodConfig : Control, ITaskPeriodConfigControl
    {
        private PredecessorTaskPeriod m_TaskPeriod;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTask;
        private System.Windows.Forms.Label label2;

        /// <summary>
        /// 指定任务完成后运行的自动任务周期配置界面。
        /// </summary>
        public PredecessorTaskPeriodConfig()
        {
            this.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTask = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbTask, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(292, 26);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "在";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbTask
            // 
            this.cmbTask.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTask.FormattingEnabled = true;
            this.cmbTask.Location = new System.Drawing.Point(26, 3);
            this.cmbTask.Name = "cmbTask";
            this.cmbTask.Size = new System.Drawing.Size(204, 20);
            this.cmbTask.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(236, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "之后运行";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form7
            // 
            this.Controls.Add(this.tableLayoutPanel1);

        }

        #region ITaskPeriodConfigControl 成员

        /// <summary>
        /// 设置需配置的任务周期对象。
        /// </summary>
        /// <param name="taskPeriod">任务周期。</param>
        /// <param name="currentTaskConfig">设置周期时所配置的任务配置对象。</param>
        public void SetTaskPeriod(TaskPeriod taskPeriod, TaskConfig currentTaskConfig)
        {
            this.m_TaskPeriod = taskPeriod as PredecessorTaskPeriod;

            foreach (TaskConfig _TaskConfig in currentTaskConfig.Group.TaskConfigs)
            {
                if (_TaskConfig.Equals(currentTaskConfig) == false)
                {
                    this.cmbTask.Items.Add(_TaskConfig);

                    if (this.m_TaskPeriod.PredecessorTask != null && this.m_TaskPeriod.PredecessorTask.Equals(_TaskConfig))
                    {
                        this.cmbTask.SelectedIndex = this.cmbTask.Items.Count - 1;
                    }
                }
            }

            if (this.cmbTask.SelectedIndex == -1 && this.cmbTask.Items.Count > 0)
            {
                this.cmbTask.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 获取配置的任务周期对象。
        /// </summary>
        /// <returns>任务周期。</returns>
        public TaskPeriod GetTaskPeriod()
        {
            if (this.cmbTask.SelectedItem == null)
            {
                throw new Exception("必须选择一个前置任务。");
            }
            else
            {
                this.m_TaskPeriod.PredecessorTask = this.cmbTask.SelectedItem as TaskConfig;
                return this.m_TaskPeriod;
            }
        }

        #endregion
    }
}
