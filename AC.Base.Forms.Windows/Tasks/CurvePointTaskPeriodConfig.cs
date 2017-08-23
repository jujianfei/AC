using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Tasks;
using AC.Base.Drives;

namespace AC.Base.Forms.Windows.Tasks
{
    /// <summary>
    /// 按设定周期运行的自动任务周期配置界面。
    /// </summary>
    [Control(typeof(AC.Base.Tasks.CurvePointTaskPeriod))]
    public class CurvePointTaskPeriodConfig : ComboBox, ITaskPeriodConfigControl
    {
        private CurvePointTaskPeriod m_TaskPeriod;

        /// <summary>
        /// 按设定周期运行的自动任务周期配置界面。
        /// </summary>
        public CurvePointTaskPeriodConfig()
        {
            this.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (int intEnumValue in Enum.GetValues(typeof(CurvePointOptions)))
            {
                //if (intEnumValue < (int)CurvePointOptions.Point1440)
                {
                    CurvePointOptions enumValue = (CurvePointOptions)intEnumValue;

                    this.Items.Add(new CurvePointItem(enumValue));
                }
            }
        }

        private class CurvePointItem
        {
            public CurvePointOptions CurvePoint;

            public CurvePointItem(CurvePointOptions curvePoint)
            {
                this.CurvePoint = curvePoint;
            }

            public override string ToString()
            {
                return CurvePointExtensions.GetDescription(this.CurvePoint) + " 每天运行" + CurvePointExtensions.GetPointCount(this.CurvePoint) + "次";
            }
        }

        #region ITaskPeriodConfigControl 成员

        /// <summary>
        /// 设置需配置的任务周期对象。
        /// </summary>
        /// <param name="taskPeriod">任务周期。</param>
        /// <param name="currentTaskConfig">设置周期时所配置的任务配置对象。</param>
        public void SetTaskPeriod(TaskPeriod taskPeriod, TaskConfig currentTaskConfig)
        {
            this.m_TaskPeriod = taskPeriod as CurvePointTaskPeriod;

            for (int intIndex = 0; intIndex < this.Items.Count; intIndex++)
            {
                CurvePointItem _CurvePointItem = this.Items[intIndex] as CurvePointItem;
                if (_CurvePointItem.CurvePoint == this.m_TaskPeriod.CurvePoint)
                {
                    this.SelectedIndex = intIndex;
                }
            }
        }

        /// <summary>
        /// 获取配置的任务周期对象。
        /// </summary>
        /// <returns>任务周期。</returns>
        public TaskPeriod GetTaskPeriod()
        {
            CurvePointItem _CurvePointItem = this.SelectedItem as CurvePointItem;
            this.m_TaskPeriod.CurvePoint = _CurvePointItem.CurvePoint;
            return this.m_TaskPeriod;
        }

        #endregion
    }
}
