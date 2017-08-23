using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 设备开关状态及控制。
    /// </summary>
    [DriveImplement("遥信、遥控")]
    public interface ISwitchDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 获取该设备开关数量。
        /// </summary>
        /// <returns></returns>
        int GetSwitchTotal();

        /// <summary>
        /// 召测所有开关的状态。
        /// </summary>
        /// <returns>返回数组的长度值等于 GetSwitchQuantity() 方法返回的值。true：表示开关为闭合、接通状态；false：表示开关未闭合、断开状态。</returns>
        bool[] CallSwitchState();

        /// <summary>
        /// 召测指定开关的状态。
        /// </summary>
        /// <param name="switchIndex">开关序号。该序号的有效值在 0 至 GetSwitchQuantity()-1 之间。</param>
        /// <returns>true：表示开关为闭合、接通状态；false：表示开关未闭合、断开状态。</returns>
        bool CallSwitchState(int switchIndex);

        /// <summary>
        /// 设置所有开关的状态。
        /// </summary>
        /// <param name="state">true：表示合闸，将所有开关置为闭合、接通状态；false：表示拉闸，将所有开关置为未闭合、断开状态。</param>
        void SetSwitchState(bool state);

        /// <summary>
        /// 设置指定开关的状态。
        /// </summary>
        /// <param name="switchIndex">开关序号。该序号的有效值在 0 至 GetSwitchQuantity()-1 之间。</param>
        /// <param name="state">true：表示合闸，将指定的开关置为闭合、接通状态；false：表示拉闸，将指定的开关置为未闭合、断开状态。</param>
        void SetSwitchState(int switchIndex, bool state);
    }
}
