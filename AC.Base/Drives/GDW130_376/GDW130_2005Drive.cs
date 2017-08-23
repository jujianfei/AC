using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.GDW130_376
{
    /// <summary>
    /// GDW130-2005 通讯规约
    /// 只需要处理与04版规约不同的地方即可
    /// </summary>
    public class GDW130_2005Drive : GDW130_2004Drive
    {
        #region 参数数据项定义
        /// <summary>
        /// 虚拟专网用户名、密码
        /// </summary>
        public class AFN04_F016 : ParamBase
        {
            /// <summary>
            /// 虚拟专网用户名
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// 虚拟专网密码
            /// </summary>
            public string Password { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    if (UserName.Length > 16 || Password.Length > 16)
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "用户名、密码长度不能超过16";
                        return false;
                    }
                    System.Text.Encoding.ASCII.GetBytes(UserName, 0, UserName.Length, Data, index);
                    index += 16;
                    System.Text.Encoding.ASCII.GetBytes(Password, 0, Password.Length, Data, index);
                    index += 16;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                UserName = System.Text.Encoding.ASCII.GetString(Data, index, 16);
                index += 16;
                Password = System.Text.Encoding.ASCII.GetString(Data, index, 16);
                index += 16;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 32; }
            }
        }
        /// <summary>
        /// F62：虚拟专网工作方式
        /// </summary>
        public class AFN04_F062 : ParamBase
        {
            /// <summary>
            /// 公网通信模块（GPRS或CDMA）工作模式
            /// 取值1~2依次表示永久在线模式、被动激活模式，其他值无效。
            /// </summary>
            public int WorkMode { get; set; }
            /// <summary>
            /// 永久在线模式重拨间隔。单位:s
            /// </summary>
            public int RedialInterval { get; set; }
            /// <summary>
            /// 被动激活模式重拨次数
            /// </summary>
            public int RedialTimes { get; set; }
            /// <summary>
            /// 被动激活模式连续无通信自动断线时间。单位：min
            /// </summary>
            public int AutoOfflineTime { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    Data[index++] = (byte)WorkMode;
                    Data[index++] = (byte)(RedialInterval % 256);
                    Data[index++] = (byte)(RedialInterval / 256);
                    Data[index++] = (byte)RedialTimes;
                    Data[index++] = (byte)AutoOfflineTime;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                WorkMode = Data[index++];
                RedialInterval = (int)Byte2Long(Data, index, 2);
                RedialTimes = Data[index++];
                AutoOfflineTime = Data[index++];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 5; }
            }
        }
        #endregion //参数数据项定义
    }
}

