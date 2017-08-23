using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.GDW130_376
{
    /// <summary>
    /// GDW376-2009 通讯规约
    /// </summary>
    public class GDW376_2009Drive : GDW130_2005Drive
    {
        #region 参数数据项定义
        /// <summary>
        /// F7：终端IP地址和端口
        /// </summary>;
        public new class AFN04_F007 : ParamBase
        {
            /// <summary>
            /// 终端IP地址
            /// </summary>
            public string RtuIP { get; set; }
            /// <summary>
            /// 子网掩码
            /// </summary>
            public string SubMask { get; set; }
            /// <summary>
            /// 网关IP
            /// </summary>
            public string GateIP { get; set; }
            /// <summary>
            /// 代理类型
            /// 数值范围0～3，依次表示：不使用代理、http connect代理、socks4代理、socks5代理。
            /// </summary>
            public int ProxyType { get; set; }
            /// <summary>
            /// 代理IP
            /// </summary>
            public string ProxyIP { get; set; }
            /// <summary>
            /// 代理端口
            /// </summary>
            public int ProxyPort { get; set; }
            /// <summary>
            /// 代理连接方式
            /// 数值范围0～1，依次表示：无需验证、需要用户名/密码。
            /// </summary>
            public int ConnStyle { get; set; }
            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// 密码
            /// </summary>
            public string Password { get; set; }
            /// <summary>
            /// 终端监听端口
            /// </summary>
            public int ListenPort { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[24 + UserName.Length + Password.Length];
                    if (UserName.Length > 20 || Password.Length > 20)
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "用户名、密码长度不能超过20";
                        return false;
                    }
                    IP2Bytes(RtuIP, Data, index);
                    index += 4;
                    IP2Bytes(SubMask, Data, index);
                    index += 4;
                    IP2Bytes(GateIP, Data, index);
                    index += 4;
                    Data[index++] = (byte)ProxyType;
                    IP2Bytes(ProxyIP, Data, index);
                    index += 4;
                    Data[index++] = (byte)(ProxyPort % 256);
                    Data[index++] = (byte)(ProxyPort / 256);
                    Data[index++] = (byte)ConnStyle;
                    Data[index++] = (byte)UserName.Length;
                    System.Text.Encoding.ASCII.GetBytes(UserName, 0, UserName.Length, Data, index);
                    index += 20;
                    Data[index++] = (byte)Password.Length;
                    System.Text.Encoding.ASCII.GetBytes(Password, 0, Password.Length, Data, index);
                    index += 20;
                    Data[index++] = (byte)(ListenPort % 256);
                    Data[index++] = (byte)(ListenPort / 256);
                }
                else
                {
                    RespLen = 64;
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
                get { return 0; }
            }
        }
        #endregion //参数数据项定义
        /// <summary>
        /// 获取Fn的返回帧长度
        /// </summary>
        /// <param name="AFN"></param>
        /// <param name="dataUnit"></param>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        protected override int GetFnDataLen(GDWDriveBase.AFNOption AFN, GDWDriveBase.DataUnitBase dataUnit, byte[] data, int start)
        {
            int fixLen = 0;
            fixLen = base.GetFnDataLen(AFN, dataUnit, data, start);
            switch (AFN)
            {
                case AFNOption.AFN_PARAM_QUERY:
                    {
                        switch (dataUnit.Fn)
                        {
                            case 7: //终端IP地址和端口
                                {
                                    fixLen = 24 + data[start + 19] + data[start + 21];
                                }
                                break;
                        }
                    }
                    break;
            }
            return fixLen;
        }
    }
}
