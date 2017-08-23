using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives.Electrics;

namespace AC.Base.Drives.GDW130_376
{
    /*
     * 电力负荷管理系统数据传输规约－2004
     * 作者：高书明
     * 日期：2011-11-7
     */
    public class GDW130_2004Drive : GDWDriveBase
    {
        public GDW130_2004Drive()
        {
            //comm.Open();
        }
        ~GDW130_2004Drive()
        {
            //comm.Close();
        }
        /// <summary>
        /// 将Pn、Fn转换成Da、Dt
        /// </summary>
        /// <param name="Pn"></param>
        /// <param name="Fn"></param>
        /// <param name="data"></param>
        /// <param name="start"></param>
        protected override void DaDt(int Pn, int Fn, byte[] data, int start)
        {
            //DA
            if (Pn == 0)
            {
                data[start] = 0;
                data[start + 1] = 0;
            }
            else
            {
                data[start] = (byte)(1 << ((Pn - 1) % 8));
                data[start + 1] = (byte)(1 << ((Pn - 1) / 8));
            }
            //DT
            data[start + 2] = (byte)(1 << ((Fn - 1) % 8));
            data[start + 3] = (byte)((Fn - 1) / 8);
        }

        /// <summary>
        /// 根据数据单元标识DA、DT获取Pn、Fn
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="Pn"></param>
        /// <param name="Fn"></param>
        protected override void GetPnFn(byte[] data, int start, out int[] Pn, out int[] Fn)
        {
            //DA
            if (data[start] == 0 || data[start + 1] == 0)
            {
                Pn = new int[1];
                Pn[0] = 0;
            }
            else
            {
                byte da1 = data[start];
                byte da2 = data[start + 1];
                List<int> PnList = new List<int>();
                for (int i = 0; i < 8; i++)
                {
                    if (((da1 >> i) & 0x1) > 0)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (((da2 >> j) & 0x1) > 0)
                            {
                                PnList.Add(i + 1 + j * 8);
                            }
                        }
                    }
                }
                Pn = new int[PnList.Count];
                int x = 0;
                foreach (int val in PnList)
                {
                    Pn[x++] = val;
                }
            }
            //DT
            if (data[start + 2] == 0)
                Fn = null;
            else
            {
                byte dt1 = data[start + 2];
                byte dt2 = data[start + 3];
                List<int> FnList = new List<int>();
                for (int i = 0; i < 8; i++)
                {
                    if (((dt1 >> i) & 0x1) > 0)
                    {
                        FnList.Add(i + 1 + dt2 * 8);
                    }
                }
                Fn = new int[FnList.Count];
                int x = 0;
                foreach (int val in FnList)
                {
                    Fn[x++] = val;
                }
            }
        }
        /// <summary>
        /// 生成CRC校验码
        /// </summary>
        /// <param name="srcKey">原始密码</param>
        /// <param name="data">需要加密的数据</param>
        /// <param name="start">data起始位置</param>
        /// <param name="len">需要加密数据长度</param>
        /// <returns>2字节CRC校验码</returns>
        protected static short CRC_Generate(short srcKey, byte[] data, int start, int len)
        {
            short ret = 0;
            byte crc16Lo = 0x00;
            byte crc16Hi = 0x00;
            byte Cl = (byte)(srcKey % 256);
            byte Ch = (byte)(srcKey / 256);
            for (int j = 0; j < len; j++)
            {
                crc16Lo ^= data[j];
                for (int k = 0; k < 8; k++)
                {
                    byte save16Hi = crc16Hi;
                    byte save16Lo = crc16Lo;
                    crc16Lo = (byte)(crc16Lo >> 1);
                    crc16Hi = (byte)(crc16Hi >> 1);
                    if ((save16Hi & 0x01) > 0)
                        crc16Lo = (byte)(crc16Lo | 0x80);
                    if ((save16Lo & 0x01) > 0)
                    {
                        crc16Hi ^= Ch;
                        crc16Lo ^= Cl;
                    }
                }
            }

            ret = (short)(crc16Lo + crc16Hi * 256);
            return ret;
        }

        /// <summary>
        /// 生成密码
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override int MakePWD(byte[] data, int index)
        {
            short key = CRC_Generate((short)PwdKey, data, 6, 12);
            data[index] = (byte)(key % 256);
            data[index + 1] = (byte)(key / 256);
            return 2;
        }

        #region 规约数据项定义
        #region 参数数据项定义
        public abstract class ParamBase : DataUnitBase
        {
            public ParamBase()
            {
                IsCombined = false;
                Data = new byte[0];//参数召测默认情况下不需要数据
            }
        }
        /// <summary>
        /// F1：终端通信参数设置
        /// </summary>
        public class AFN04_F001 : ParamBase
        {
            /// <summary>
            /// 终端数传机延时时间RTS。单位:ms
            /// </summary>
            public int RTS { get; set; }
            /// <summary>
            /// 终端作为启动站允许发送传输延时时间。单位：min
            /// </summary>
            public int TransDelay { get; set; }
            /// <summary>
            /// 终端等待从动站响应的超时时间。单位：s
            /// </summary>
            public int Timeout { get; set; }
            /// <summary>
            /// 终端未收到从动站响应重发次数。
            /// </summary>
            public byte Retry { get; set; }
            /// <summary>
            /// 主动上报重要事件是否需要确认
            /// </summary>
            public bool IsConfimERC1 { get; set; }
            /// <summary>
            /// 主动上报一般事件是否需要确认
            /// </summary>
            public bool IsConfimERC2 { get; set; }
            /// <summary>
            /// 心跳周期。单位:min
            /// </summary>
            public byte Heartbeat { get; set; }

            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    //数传延时
                    Data[index++] = (byte)(RTS / 20);
                    //传送延时
                    Data[index++] = (byte)TransDelay;
                    //超时时间
                    Data[index++] = (byte)(Timeout % 256);
                    Data[index] = (byte)((Timeout / 256) & 0xF);
                    //重发次数
                    Data[index++] = (byte)(Retry << 4);
                    //上报时间确认
                    Data[index++] = (byte)((IsConfimERC1 ? 1 : 0) | (IsConfimERC2 ? 2 : 0));
                    //心跳周期
                    Data[index++] = Heartbeat;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                RTS = Data[index++] * 20;
                TransDelay = Data[index++];
                Timeout = Data[index] + (Data[index + 1] & 0xF) * 256;
                index++;
                Retry = (byte)(Data[index++] >> 4);
                IsConfimERC1 = (Data[index] & 0x1) > 0;
                IsConfimERC2 = (Data[index] & 0x2) > 0;
                index++;
                Heartbeat = Data[index];
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 6; }
            }
        }
        /// <summary>
        /// F2：终端中继转发设置：p0
        /// </summary>
        public class AFN04_F002 : ParamBase
        {
            /// <summary>
            /// 是否允许转发
            /// </summary>
            public bool IsAllow { get; set; }
            /// <summary>
            /// 被转发终端地址列表
            /// </summary>
            private List<int> addrList = new List<int>();
            public void AddAddr(int addr)
            {
                addrList.Add(addr);
            }
            /// <summary>
            /// 被转发终端地址列表
            /// </summary>
            public List<int> AddrList
            {
                get { return addrList; }
            }

            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    if (addrList.Count > 16)
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "被转发的终端个数不能超过16。";
                        return false;
                    }
                    Data = new byte[addrList.Count * 2 + 1];
                    Data[index] = (byte)addrList.Count;
                    if (IsAllow)
                        Data[index] |= 0x80;
                    index++;
                    foreach (int addr in addrList)
                    {
                        Data[index++] = (byte)(addr % 256);
                        Data[index++] = (byte)(addr / 256);
                    }
                }
                else
                {
                    RespLen = 16 * 2 + 1;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                IsAllow = (Data[index] & 0x80) > 0;
                int count = Data[index] & 0x7F;
                index++;
                for (int i = 0; i < count; i++)
                {
                    int addr = Data[index] + Data[index + 1] * 256;
                    index += 2;
                    addrList.Add(addr);
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 2 ; }
            }
        }
        /// <summary>
        /// F3：主站IP地址和端口：p0
        /// </summary>
        public class AFN04_F003 : ParamBase
        {
            /// <summary>
            /// 主用IP
            /// </summary>
            public string MasterIP { get; set; }
            /// <summary>
            /// 主用端口
            /// </summary>
            public int MasterPort { get; set; }
            /// <summary>
            /// 备用IP
            /// </summary>
            public string BackupIP { get; set; }
            /// <summary>
            /// 备用端口
            /// </summary>
            public int BackupPort { get; set; }
            /// <summary>
            /// 网关IP
            /// </summary>
            public string GateIP { get; set; }
            /// <summary>
            /// 网关端口
            /// </summary>
            public int GatePort { get; set; }
            /// <summary>
            /// 代理IP
            /// </summary>
            public string ProxyIP { get; set; }
            /// <summary>
            /// 代理端口
            /// </summary>
            public int ProxyPort { get; set; }
            /// <summary>
            /// APN
            /// </summary>
            public string APN { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    IP2Bytes(MasterIP, Data, index);
                    index += 4;
                    Data[index++] = (byte)(MasterPort % 256);
                    Data[index++] = (byte)(MasterPort / 256);
                    IP2Bytes(BackupIP, Data, index);
                    index += 4;
                    Data[index++] = (byte)(BackupPort % 256);
                    Data[index++] = (byte)(BackupPort / 256);
                    IP2Bytes(GateIP, Data, index);
                    index += 4;
                    Data[index++] = (byte)(GatePort % 256);
                    Data[index++] = (byte)(GatePort / 256);
                    IP2Bytes(ProxyIP, Data, index);
                    index += 4;
                    Data[index++] = (byte)(ProxyPort % 256);
                    Data[index++] = (byte)(ProxyPort / 256);
                    System.Text.Encoding.ASCII.GetBytes(APN, 0, APN.Length, Data, index);
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                MasterIP = Data[index] + "." + Data[index + 1] + "." + Data[index + 2] + "." + Data[index + 3];
                index += 4;
                MasterPort = Data[index] + Data[index] * 256;
                index += 2;
                BackupIP = Data[index] + "." + Data[index + 1] + "." + Data[index + 2] + "." + Data[index + 3];
                index += 4;
                BackupPort = Data[index] + Data[index] * 256;
                index += 2;
                GateIP = Data[index] + "." + Data[index + 1] + "." + Data[index + 2] + "." + Data[index + 3];
                index += 4;
                GatePort = Data[index] + Data[index] * 256;
                index += 2;
                ProxyIP = Data[index] + "." + Data[index + 1] + "." + Data[index + 2] + "." + Data[index + 3];
                index += 4;
                ProxyPort = Data[index] + Data[index] * 256;
                index += 2;
                APN = System.Text.Encoding.ASCII.GetString(Data, index, 16);
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 40; }
            }
        }
        /// <summary>
        /// F4：主站电话号码和短信中心号码：p0
        /// </summary>
        public class AFN04_F004 : ParamBase
        {
            /// <summary>
            /// 主站电话号码
            /// </summary>
            public string Phone { get; set; }
            /// <summary>
            /// 短信中心号码
            /// </summary>
            public string SMCenter { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];
                    for (int i = 0; i < Phone.Length; i++)
                    {
                        if (Phone[i] < '0' || Phone[i] > '9' || Phone[i] != '#' || Phone[i] != ',')
                        {
                            OperResult = OperResultOption.InvalidValue;
                            OperResultDesc = "主站电话号码不合法。";
                            return false;
                        }
                    }
                    string str = Phone.Replace(',', 'A');
                    str = str.Replace('#', 'B');
                    for (int i = 0; i < 8; i++)
                    {
                        if (str.Length > 2)
                        {
                            Data[i] = System.Convert.ToByte(str.Substring(1,1),16);
                            Data[i] |= (byte)(System.Convert.ToByte(str.Substring(0,1),16)<<4);
                            str = str.Remove(0,2);
                        }
                        else if(str.Length>1)
                        {
                            Data[i] = (byte)(System.Convert.ToByte(str.Substring(0,1),16)<<4);
                            Data[i] |= 0x0F;
                            str = str.Remove(0,1);
                        }
                        else
                        {
                            Data[i] = 0xFF;
                        }
                    }
                    str = SMCenter;
                    for(int i=8;i<16;i++)
                    {
                        if (str.Length > 2)
                        {
                            Data[i] = System.Convert.ToByte(str[1]);
                            Data[i] |= (byte)(System.Convert.ToByte(str[0])<<4);
                            str = str.Remove(0,2);
                        }
                        else if(str.Length>1)
                        {
                            Data[i] = (byte)(System.Convert.ToByte(str[0])<<4);
                            Data[i] |= 0x0F;
                            str = str.Remove(0,1);
                        }
                        else
                        {
                            Data[i] = 0xFF;
                        }
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                Phone = "";
                SMCenter = "";
                for (int i = 0; i < 8; i++)
                {
                    if((Data[i] & 0xF0)==0xF0)
                        break;
                    Phone += Data[i] >> 4;
                    if((Data[i] & 0x0F)==0x0F)
                        break;
                    Phone += Data[i] & 0x0F;
                }
                for (int i = 8; i < 16; i++)
                {
                    if ((Data[i] & 0xF0) == 0xF0)
                        break;
                    SMCenter += Data[i] >> 4;
                    if ((Data[i] & 0x0F) == 0x0F)
                        break;
                    SMCenter += Data[i] & 0x0F;
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 16; }
            }
        }
        /// <summary>
        /// F5：终端消息认证参数设置：p0
        /// </summary>
        public class AFN04_F005 : ParamBase
        {
            /// <summary>
            /// 密码编号
            /// </summary>
            public int PwdId { get; set; }
            /// <summary>
            /// 密码算法
            /// </summary>
            public int PwdKey { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[3];
                    Data[0] = (byte)PwdId;
                    Data[1] = (byte)(PwdKey % 256);
                    Data[2] = (byte)(PwdKey / 256);
                }
                return true;
            }
            public override bool Decode()
            {
                PwdId = Data[0];
                PwdKey = Data[1] + Data[2] * 256;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 3; }
            }
        }
        /// <summary>
        /// F6：终端组地址设置：p0
        /// </summary>
        public class AFN04_F006 : ParamBase
        {
            private List<int> addrList = new List<int>();
            public List<int> AddrList
            {
                get { return addrList; }
            }
            public void AddAddr(int addr)
            {
                addrList.Add(addr);
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    foreach (int addr in addrList)
                    {
                        if (index >= 16)
                            break;
                        if (addr < 1 || addr > 65534)
                        {
                            OperResult = OperResultOption.InvalidValue;
                            OperResultDesc = "组地址应在[1,65534]之间。";
                            return false;
                        }
                        Data[index++] = (byte)(addr % 256);
                        Data[index++] = (byte)(addr / 256);
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                for (int i = 0; i < 8; i++)
                {
                    int addr = Data[i] + Data[i + 1] * 256;
                    if (addr > 0)
                    {
                        addrList.Add(addr);
                    }
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 16; }
            }
        }
        /// <summary>
        /// F7：终端抄表日设置：p0
        /// </summary>
        public class AFN04_F007 : ParamBase
        {
            /// <summary>
            /// 终端抄表日期。D0~D30对应每月1日~31日，置“1”为有效，置“0”为无效
            /// </summary>
            public int ReadDay { get; set; }
            /// <summary>
            /// 抄表时间。时-分
            /// </summary>
            public DateTime ReadTime { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];
                    System.BitConverter.GetBytes(ReadDay).CopyTo(Data, 0);
                    if (!Dec2Bcd_Format19(ReadTime, Data, 4))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "抄表时间无效";
                        return false;
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                ReadDay = System.BitConverter.ToInt32(Data, 0);
                DateTime dt;
                if (!Bcd2Dec_Format19(Data, 4, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                ReadTime = dt;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 6; }
            }
        }
        /// <summary>
        /// F8：终端事件记录配置设置：p0
        /// </summary>
        public class AFN04_F008 : ParamBase
        {
            /// <summary>
            /// 事件有效性标志
            /// </summary>
            private bool[] validFlag = new bool[64];
            /// <summary>
            /// 事件重要性标志
            /// </summary>
            private bool[] impFlag = new bool[64];
            public bool[] ValidFlag { get { return validFlag; } }
            public bool[] ImpFlag { get { return impFlag; } }

            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];

                    for (int i = 0; i < 64; i++)
                    {
                        Data[i / 8] |= (byte)(validFlag[i] ? (1 << (i % 8)) : 0);
                        Data[8 + i / 8] |= (byte)(impFlag[i] ? (1 << (i % 8)) : 0);
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                for (int i = 0; i < 64; i++)
                {
                    validFlag[i] = ((Data[i / 8] >> (i % 8)) & 0x01) > 0;
                    impFlag[i] = ((Data[8 + i / 8] >> (i % 8)) & 0x01) > 0;
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 16; }
            }
        }
        /// <summary>
        /// F9：终端配置数量表：p0
        /// </summary>
        public class AFN04_F009 : ParamBase
        {
            /// <summary>
            /// 电表/交采数量
            /// </summary>
            public int MeterCount { get; set; }
            /// <summary>
            /// 脉冲数量
            /// </summary>
            public int PulseCount { get; set; }
            /// <summary>
            /// 交采数量
            /// </summary>
            public int ACCount { get; set; }
            /// <summary>
            /// 总加组数量
            /// </summary>
            public int TotalCount { get; set; }

            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];
                    Data[0] = (byte)MeterCount;
                    Data[1] = (byte)PulseCount;
                    Data[2] = (byte)ACCount;
                    Data[3] = (byte)TotalCount;
                }
                return true;
            }
            public override bool Decode()
            {
                MeterCount = Data[0];
                PulseCount = Data[1];
                ACCount = Data[2];
                TotalCount = Data[3];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F10：终端电能表/交流采样装置配置参数：p0
        /// </summary>
        public class AFN04_F010 : ParamBase
        {
            public class MeterItem
            {
                /// <summary>
                /// 装置序号（抄表分路）
                /// </summary>
                public int Road { get; set; }
                /// <summary>
                /// 测量点编号
                /// </summary>
                public int MpId { get; set; }
                /// <summary>
                /// 端口号
                /// </summary>
                public int Port { get; set; }
                /// <summary>
                /// 规约编号
                /// </summary>
                public int ProtoId { get; set; }
                /// <summary>
                /// 电表地址
                /// </summary>
                public string MeterAddr { get; set; }
                /// <summary>
                /// 电表密码
                /// </summary>
                public string MeterPwd { get; set; }
                /// <summary>
                /// 费率数
                /// </summary>
                public int RateCount { get; set; }
                /// <summary>
                /// 整数位数
                /// </summary>
                public int IntegerCount { get; set; }
                /// <summary>
                /// 小数位数
                /// </summary>
                public int FractionCount { get; set; }
            }
            private List<MeterItem> meterList = new List<MeterItem>();
            public List<MeterItem> MeterList { get { return meterList; } }
            public void AddMeter(MeterItem meter)
            {
                meterList.Add(meter);
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[1 + DataFixLen * meterList.Count];
                    Data[index++] = (byte)meterList.Count;
                    foreach (MeterItem meter in meterList)
                    {
                        Data[index++] = (byte)meter.Road;
                        Data[index++] = (byte)meter.MpId;
                        Data[index++] = (byte)meter.Port;
                        Data[index++] = (byte)meter.ProtoId;
                        Dec2Bcd_Format12(System.Convert.ToInt64(meter.MeterAddr), Data, index);
                        index += (int)FormatLenOption.Format12;
                        Dec2Bcd(System.Convert.ToInt64(meter.MeterPwd), Data, index, 6);
                        index += 6;
                        Data[index] = (byte)(((meter.FractionCount <= 0 ? 1 : meter.FractionCount) % 5) - 1);
                        if (meter.IntegerCount < 4 || meter.FractionCount > 7)
                        {
                            OperResult = OperResultOption.InvalidValue;
                            OperResultDesc = "整数位数取值范围[4,7]。";
                            return false;
                        }
                        Data[index] |= (byte)((meter.IntegerCount - 4) << 2);
                        if (meter.RateCount > 14)
                        {
                            OperResult = OperResultOption.InvalidValue;
                            OperResultDesc = "费率个数不能大于14";
                        }
                        Data[index] |= (byte)(meter.RateCount << 4);
                        index++;
                    }
                }
                else
                {
                    RespLen = 250;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                int count = Data[index++];
                for (int i = 0; i < count; i++)
                {
                    MeterItem meter = new MeterItem();
                    meter.Road = Data[index++];
                    meter.MpId = Data[index++];
                    meter.Port = Data[index++];
                    meter.ProtoId = Data[index++];
                    long tmp = 0;
                    Bcd2Dec_Format12(Data, index, out tmp);
                    index += (int)FormatLenOption.Format12;
                    meter.MeterAddr = tmp.ToString();
                    tmp = 0;
                    for (int j = 0; j < 6; j++)
                    {
                        tmp += Data[index+5-j]*256;
                    }
                    index += 6;
                    meter.MeterPwd = tmp.ToString();
                    meter.FractionCount = (Data[index] & 0x03) + 1;
                    meter.IntegerCount = ((Data[index] >> 2) & 0x03) + 4;
                    meter.RateCount = (Data[index] >> 4) & 0x0F;
                    meterList.Add(meter);
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 17; }
            }
        }
        /// <summary>
        /// F11：终端脉冲配置参数：p0
        /// </summary>
        public class AFN04_F011 : ParamBase
        {
            public class PulseItem
            {
                /// <summary>
                /// 脉冲端口号
                /// </summary>
                public int Port { get; set; }
                /// <summary>
                /// 测量点号
                /// </summary>
                public int MpId { get; set; }
                /// <summary>
                /// 脉冲属性
                /// </summary>
                public byte Attr { get; set; }
                /// <summary>
                /// 电表常数K
                /// </summary>
                public int K { get; set; }
            }
            private List<PulseItem> pulseList = new List<PulseItem>();
            /// <summary>
            /// 脉冲配置表
            /// </summary>
            public List<PulseItem> PulseList { get { return pulseList; } }
            /// <summary>
            /// 向脉冲配置表中增加1路脉冲
            /// </summary>
            /// <param name="pulse"></param>
            public void AddPulse(PulseItem pulse)
            {
                pulseList.Add(pulse);
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[1 + pulseList.Count * DataFixLen];
                    Data[index++] = (byte)pulseList.Count;
                    foreach (PulseItem pulse in pulseList)
                    {
                        Data[index++] = (byte)pulse.Port;
                        Data[index++] = (byte)pulse.MpId;
                        Data[index++] = (byte)pulse.Attr;
                        Data[index++] = (byte)(pulse.K % 256);
                        Data[index++] = (byte)(pulse.K / 256);
                    }
                }
                else
                {
                    RespLen = 75;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                int count = Data[index++];
                for (int i = 0; i < count; i++)
                {
                    PulseItem pulse = new PulseItem();
                    pulse.Port = Data[index++];
                    pulse.MpId = Data[index++];
                    pulse.Attr = Data[index++];
                    pulse.K = Data[index] + Data[index + 1] * 256;
                    index += 2;
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 5; }
            }
        }
        /// <summary>
        /// F13：终端电压/电流模拟量配置参数：p0
        /// </summary>
        public class AFN04_F013 : ParamBase
        {
            public class ACItem
            {
                /// <summary>
                /// 模拟量端口号
                /// </summary>
                public int Port { get; set; }
                /// <summary>
                /// 测量点号
                /// </summary>
                public int MpId { get; set; }
                /// <summary>
                /// 模拟量属性
                /// </summary>
                public int Attr { get; set; }

            }
            /// <summary>
            /// 模拟量配置表
            /// </summary>
            private List<ACItem> acList = new List<ACItem>();
            /// <summary>
            /// 模拟量配置表
            /// </summary>
            public List<ACItem> AcList { get { return acList; } }
            /// <summary>
            /// 向模拟量配置表增加一个模拟量
            /// </summary>
            /// <param name="ac"></param>
            public void AddAc(ACItem ac)
            {
                acList.Add(ac);
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[1 + acList.Count * DataFixLen];
                    Data[index++] = (byte)acList.Count;
                    foreach (ACItem ac in acList)
                    {
                        Data[index++] = (byte)ac.Port;
                        Data[index++] = (byte)ac.MpId;
                        Data[index++] = (byte)ac.Attr;
                    }
                }
                else
                {
                    RespLen = 100;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                int count = Data[index++];
                for (int i = 0; i < count; i++)
                {
                    ACItem ac = new ACItem();
                    ac.Port = Data[index++];
                    ac.MpId = Data[index++];
                    ac.Attr = Data[index++];
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 3; }
            }
        }
        /// <summary>
        /// F14：终端总加组配置参数：p0
        /// </summary>
        public class AFN04_F014 : ParamBase
        {
            public class MpItem
            {
                /// <summary>
                /// 测量点号
                /// </summary>
                public int MpId { get; set; }
                /// <summary>
                /// 正反向标志 置“0”：正向；置“1”：反向
                /// </summary>
                public byte DirFlag { get; set; }
                /// <summary>
                /// 运算标志 置“0”：“加”运算；置“1”：“减”运算
                /// </summary>
                public byte CalcFlag { get; set; }
            }
            public class TotalItem
            {
                /// <summary>
                /// 总加组编号
                /// </summary>
                public int TotalId { get; set; }
                /// <summary>
                /// 测量点列表
                /// </summary>
                private List<MpItem> mpList = new List<MpItem>();
                public List<MpItem> MpList { get { return mpList; } }
                /// <summary>
                /// 增加一个测量点
                /// </summary>
                /// <param name="mp"></param>
                public void AddMp(MpItem mp)
                {
                    mpList.Add(mp);
                }
            }
            /// <summary>
            /// 总加组列表
            /// </summary>
            private List<TotalItem> totalList = new List<TotalItem>();
            public List<TotalItem> TotalList { get { return totalList; } }
            /// <summary>
            /// 增加一个总加组
            /// </summary>
            /// <param name="total"></param>
            public void AddTotal(TotalItem total)
            {
                totalList.Add(total);
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[1 + totalList.Count * DataFixLen];
                    Data[index++] = (byte)totalList.Count;
                    foreach (TotalItem total in totalList)
                    {
                        Data[index++] = (byte)total.TotalId;
                        Data[index++] = (byte)total.MpList.Count;
                        foreach (MpItem mp in total.MpList)
                        {
                            Data[index] = (byte)(mp.MpId & 0x1F);
                            Data[index] |= (byte)(mp.DirFlag << 6);
                            Data[index] |= (byte)(mp.CalcFlag << 7);
                        }
                    }
                }
                else
                {
                    RespLen = 60;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                int tcount = Data[index++];
                for (int i = 0; i < tcount; i++)
                {
                    TotalItem total = new TotalItem();
                    total.TotalId = Data[index++];
                    int mcount = Data[index++];
                    for (int j = 0; j < mcount; j++)
                    {
                        MpItem mp = new MpItem();
                        mp.MpId = (byte)(Data[index] & 0x1F);
                        mp.DirFlag = (byte)((Data[index] >> 6) & 0x01);
                        mp.CalcFlag = (byte)((Data[index] >> 7) & 0x01);
                        index++;
                        total.AddMp(mp);
                    }
                    totalList.Add(total);
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F15：有功总电能量差动越限事件参数设置：p0
        /// </summary>
        public class AFN04_F015 : ParamBase
        {
            public class DiffMotion
            {
                /// <summary>
                /// 对比总加组Id
                /// </summary>
                public int ContrastId { get; set; }
                /// <summary>
                /// 参照总加组Id
                /// </summary>
                public int ReferId { get; set; }
                /// <summary>
                /// 时间跨度
                /// 0：60分钟电量；1：30分钟电量；2：15分钟电量
                /// </summary>
                public int TimeSpan { get; set; }
                /// <summary>
                /// 对比方法
                /// 0：相对，1：绝对
                /// </summary>
                public int ContrastWay { get; set; }
                /// <summary>
                /// 相对变差
                /// </summary>
                public int RelativeDiff { get; set; }
                /// <summary>
                /// 绝对偏差
                /// </summary>
                public long AbsolutDiff { get; set; }
            }
            /// <summary>
            /// 差动组列表
            /// </summary>
            private List<DiffMotion> diffList = new List<DiffMotion>();
            /// <summary>
            /// 差动组列表
            /// </summary>
            public List<DiffMotion> DiffList { get { return diffList; } }
            /// <summary>
            /// 增加一个差动组
            /// </summary>
            /// <param name="diff"></param>
            public void AddDiffMotion(DiffMotion diff)
            {
                diffList.Add(diff);
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[1 + diffList.Count * DataFixLen];
                    Data[index++] = (byte)diffList.Count;
                    foreach (DiffMotion diff in diffList)
                    {
                        Data[index++] = (byte)diff.ContrastId;
                        Data[index++] = (byte)diff.ReferId;
                        Data[index++] = (byte)(diff.TimeSpan | (diff.ContrastWay<<7));
                        Data[index++] = (byte)diff.RelativeDiff;
                        Dec2Bcd_Format03(diff.AbsolutDiff, Data, index);
                        index += (int)FormatLenOption.Format03;
                    }
                }
                else
                {
                    RespLen = 32;
                }
                return false;
            }
            public override bool Decode()
            {
                int index = 0;
                int count = Data[index++];
                for (int i = 0; i < count; i++)
                {
                    DiffMotion diff = new DiffMotion();
                    diff.ContrastId = Data[index++];
                    diff.ReferId = Data[index++];
                    diff.TimeSpan = Data[index] & 0x3;
                    diff.ContrastWay = Data[index] >> 7;
                    index++;
                    diff.RelativeDiff = Data[index++];
                    long? tmp;
                    if (Bcd2Dec_Format03(Data, index, out tmp))
                        diff.AbsolutDiff = (long)tmp;
                    diffList.Add(diff);
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 8; }
            }
        }
        /// <summary>
        /// F17：终端保安定值：p0
        /// </summary>
        public class AFN04_F017 : ParamBase
        {
            /// <summary>
            /// 保安定值
            /// </summary>
            public double SafeFixValue { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];
                    if (!Dec2Bcd_Format02(SafeFixValue, Data, 0))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "保安定值取值范围[0.001,9990000]。";
                        return false;
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                double? tmp;
                if (!Bcd2Dec_Format02(Data, 0, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                SafeFixValue = (double)tmp;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F18：终端功控时段：p0
        /// </summary>
        public class AFN04_F018 : ParamBase
        {
            /// <summary>
            /// 48点时段 0-不控制 1-控1 2-控2
            /// </summary>
            private int[] period = new int[48];
            public int this[int index]
            {
                get
                {
                    return period[index];
                }
                set
                {
                    period[index] = value;
                }
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[12];
                    for (int i = 0; i < 48; i++)
                    {
                        Data[i / 4] |= (byte)(period[i] << ((i % 4)*2)); 
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                for (int i = 0; i < 48; i++)
                {
                    period[i] = Data[i / 4] >> ((i % 4) * 2) & 0x03;
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 12; }
            }
        }
        /// <summary>
        /// F19：终端时段功控定值浮动系数：p0
        /// </summary>
        public class AFN04_F019 : ParamBase
        {
            /// <summary>
            /// 浮动系数
            /// </summary>
            public short DriftRatio { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[1];
                    if (!Dec2Bcd_Format04(DriftRatio, Data, 0))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "浮动系数取值范围[-79,79]";
                        return false;
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                short tmp;
                if (!Bcd2Dec_Format04(Data, 0, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                }
                DriftRatio = tmp;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 1; }
            }
        }
        /// <summary>
        /// F20：终端月电能量控定值浮动系数：p0
        /// </summary>
        public class AFN04_F020 : ParamBase
        {
            /// <summary>
            /// 浮动系数
            /// </summary>
            public short DriftRatio { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[1];
                    if (!Dec2Bcd_Format04(DriftRatio, Data, 0))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "浮动系数取值范围[-79,79]";
                        return false;
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                short tmp;
                if (!Bcd2Dec_Format04(Data, 0, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                }
                DriftRatio = tmp;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 1; }
            }
        }
        /// <summary>
        /// F21：终端电能量费率时段和费率数：p0
        /// </summary>
        public class AFN04_F021 : ParamBase
        {
            /// <summary>
            /// 电量时段
            /// </summary>
            private int[] period = new int[48];
            /// <summary>
            /// 费率数
            /// </summary>
            public int RateCount { get; set; }
            public int this[int index]
            {
                get { return period[index]; }
                set { period[index] = value; }
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];
                    for (int i = 0; i < 48; i++)
                    {
                        Data[i / 2] |= (byte)((period[i] & 0x0F) << ((i % 2)*4)); 
                    }
                    if (RateCount > 14)
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "费率个数不能多于14个。";
                        return false;
                    }
                    Data[24] = (byte)RateCount;
                }
                return true;
            }
            public override bool Decode()
            {
                for (int i = 0; i < 48; i++)
                {
                    period[i] = (Data[i / 2] >> ((i % 2) * 4)) & 0x0F;
                }
                RateCount = Data[24];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 25; }
            }
        }
        /// <summary>
        /// F22：终端电能量费率：p0
        /// </summary>
        public class AFN04_F022 : ParamBase
        {
            /// <summary>
            /// 费率。单位：厘
            /// </summary>
            private long?[] rate = new long?[14];
            public long? this[int index]
            {
                get { return rate[index]; }
                set { rate[index] = value; }
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    for (int i = 0; i < 14; i++)
                    {
                        if (!Dec2Bcd_Format03((long)rate[i], Data, index))
                        {
                            OperResult = OperResultOption.InvalidValue;
                            OperResultDesc = "费率值无效。";
                            return false;
                        }
                        index += (int)FormatLenOption.Format03;
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                int index=0;
                for (int i = 0; i < 14; i++)
                {
                    if (!Bcd2Dec_Format03(Data, index, out rate[i]))
                    {
                        OperResult = OperResultOption.InvalidData;
                        OperResultDesc = "回码数据无效";
                        return false;
                    }
                    index += (int)FormatLenOption.Format03;
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 56; }
            }
        }
        /// <summary>
        /// F23：终端催费告警参数：p0
        /// </summary>
        public class AFN04_F023 : ParamBase
        {
            /// <summary>
            /// 告警时段 true-告警 false-不告警
            /// 0~23点
            /// </summary>
            private bool[] period = new bool[24];
            public bool this[int index]
            {
                get { return period[index]; }
                set { period[index] = value; }
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];
                    for (int i = 0; i < 24; i++)
                    {
                        Data[i / 8] |= (byte)((period[i] ? 1:0) << (i % 8));
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                for (int i = 0; i < 24; i++)
                {
                    period[i] = ((Data[i / 8] >> (i % 8)) & 0x01) > 0;
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 3; }
            }
        }
        /// <summary>
        /// F24：终端抄表间隔设置：p0
        /// </summary>
        public class AFN04_F024 : ParamBase
        {
            /// <summary>
            /// 抄表间隔。单位：min
            /// </summary>
            public int Interval { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];
                    if(Interval<1 || Interval>15)
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "抄表间隔取值范围[1,15]";
                        return false;
                    }
                    Data[0] = (byte)Interval;
                }
                return true;
            }
            public override bool Decode()
            {
                Interval = Data[0];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 1; }
            }
        }
        /// <summary>
        /// F25：测量点基本参数：测量点号
        /// </summary>
        public class AFN04_F025 : ParamBase
        {
            /// <summary>
            /// 电压变比
            /// </summary>
            public int PT { get; set; }
            /// <summary>
            /// 电流变比
            /// </summary>
            public int CT { get; set; }
            /// <summary>
            /// 额定电压
            /// </summary>
            public double RatedVol { get; set; }
            /// <summary>
            /// 最大电流
            /// </summary>
            public double MaxCurr { get; set; }
            /// <summary>
            /// 接线方式 0-备用 1-三相三线 2-三相四线 3-单相
            /// </summary>
            public int WiringStyle { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    Data[index++] = (byte)(PT % 256);
                    Data[index++] = (byte)(PT / 256);
                    Data[index++] = (byte)(CT % 256);
                    Data[index++] = (byte)(CT / 256);
                    if (!Dec2Bcd_Format07(RatedVol, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "额定电压取值范围[0.1,999.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format07;
                    if (!Dec2Bcd_Format22(MaxCurr, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "额定电压取值范围[0.1,9.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format22;
                    Data[index++] = (byte)(WiringStyle & 0x03);
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                PT = Data[index] + Data[index + 1] * 256;
                index += 2;
                CT = Data[index] + Data[index + 1] * 256;
                index += 2;
                double? tmp;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                RatedVol = (double)tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format22(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                MaxCurr = (double) tmp;
                index += (int)FormatLenOption.Format22;
                WiringStyle = Data[index++];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 8; }
            }
        }
        /// <summary>
        /// F26：测量点限值参数：测量点号
        /// </summary>
        public class AFN04_F026 : ParamBase
        {
            /// <summary>
            /// 电压上限。单位：V
            /// </summary>
            public double VolUp { get; set; }
            /// <summary>
            /// 电压下限。单位：V
            /// </summary>
            public double VolLow { get; set; }
            /// <summary>
            /// 断相电压。单位：V
            /// </summary>
            public double VolBreak { get; set; }
            /// <summary>
            /// 电压上上限。单位：V
            /// </summary>
            public double VolUpper { get; set; }
            /// <summary>
            /// 电压下下限。单位：V
            /// </summary>
            public double VolLower { get; set; }
            /// <summary>
            /// 电流上上限。单位：A
            /// </summary>
            public double CurrUpper { get; set; }
            /// <summary>
            /// 电流上限。单位：A
            /// </summary>
            public double CurrUp { get; set; }
            /// <summary>
            /// 零相序电流上限。单位：A
            /// </summary>
            public double Curr0Up { get; set; }
            /// <summary>
            /// 视在功率上上限。单位：kVA
            /// </summary>
            public double AppPowerUpper { get; set; }
            /// <summary>
            /// 视在功率上限。单位：kVA
            /// </summary>
            public double AppPowerUp { get; set; }
            /// <summary>
            /// 电压不平衡限值。单位：%
            /// </summary>
            public double VolUnblance { get; set; }
            /// <summary>
            /// 电流不平衡限值。单位：%
            /// </summary>
            public double CurrUnblace { get; set; }
            /// <summary>
            /// 失压连续时间。单位：min
            /// </summary>
            public double LossVolTime { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    if (!Dec2Bcd_Format07(VolUp, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "电压上限取值范围[0.1,999.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format07;
                    if (!Dec2Bcd_Format07(VolLow, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "电压下限取值范围[0.1,999.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format07;
                    if (!Dec2Bcd_Format07(VolBreak, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "电压断相门限取值范围[0.1,999.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format07;
                    if (!Dec2Bcd_Format07(VolUpper, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "电压上上限取值范围[0.1,999.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format07;
                    if (!Dec2Bcd_Format07(VolLower, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "电压下下限取值范围[0.1,999.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format07;
                    if (!Dec2Bcd_Format06(CurrUpper, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "电流上上限取值范围[-79.99,79.99]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format06;
                    if (!Dec2Bcd_Format06(CurrUp, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "电流上限取值范围[-79.99,79.99]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format06;
                    if (!Dec2Bcd_Format06(Curr0Up, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "零相电流上限取值范围[-79.97,79.99]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format06;
                    if (!Dec2Bcd_Format23(AppPowerUpper, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "视在功率上上限取值范围[0.0001,99.9999]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format23;
                    if (!Dec2Bcd_Format23(AppPowerUp, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "视在功率上限取值范围[0.0001,99.9999]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format23;
                    if (!Dec2Bcd_Format05(VolUnblance, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "三相电压不平衡限值取值范围[-799.9,799.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format05;
                    if (!Dec2Bcd_Format05(CurrUnblace, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "三相电流不平衡限值取值范围[-799.9,799.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format05;
                    Data[index++] = (byte)LossVolTime;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                double? tmp;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                VolUp = (double)tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                VolLow = (double)tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                VolBreak = (double)tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                VolUpper = (double)tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                VolLower = (double)tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                CurrUpper = (double)tmp;
                index += (int)FormatLenOption.Format06;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                CurrUp = (double)tmp;
                index += (int)FormatLenOption.Format06;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                Curr0Up = (double)tmp;
                index += (int)FormatLenOption.Format06;
                double? value;
                if (!Bcd2Dec_Format23(Data, index, out value))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                AppPowerUpper = (double)value;
                index += (int)FormatLenOption.Format23;
                if (!Bcd2Dec_Format23(Data, index, out value))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                AppPowerUp = (double)value;
                index += (int)FormatLenOption.Format23;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                VolUnblance = (double)tmp;
                index += (int)FormatLenOption.Format05;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                CurrUnblace = (double)tmp;
                index += (int)FormatLenOption.Format05;
                LossVolTime = Data[index++];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 27; }
            }
        }
        /// <summary>
        /// F27：测量点数据冻结参数：测量点号
        /// </summary>
        public class AFN04_F027 : ParamBase
        {
            /// <summary>
            /// 冻结参数
            /// </summary>
            public class FreezeItem
            {
                /// <summary>
                /// 数据标识Fn
                /// </summary>
                public int Fn { get; set; }
                /// <summary>
                /// 冻结密度
                /// </summary>
                public int Denisty { get; set; }
            }
            /// <summary>
            /// 冻结参数列表
            /// </summary>
            private List<FreezeItem> freezeList = new List<FreezeItem>();
            public List<FreezeItem> FreezeList { get { return freezeList; } }
            /// <summary>
            /// 增加一个冻结参数
            /// </summary>
            /// <param name="freeze"></param>
            public void AddFreeze(FreezeItem freeze)
            {
                freezeList.Add(freeze);
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[1 + DataFixLen * freezeList.Count];
                    Data[index] = (byte)freezeList.Count;
                    foreach (FreezeItem freeze in freezeList)
                    {
                        Data[index++] = (byte)freeze.Fn;
                        Data[index++] = (byte)freeze.Denisty; 
                    }
                }
                else
                {
                    RespLen = 60;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                int count = Data[index++];
                for (int i = 0; i < count; i++)
                {
                    FreezeItem freeze = new FreezeItem();
                    freeze.Fn = Data[index++];
                    freeze.Denisty = Data[index++];
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F28：测量点功率因数分段限值：测量点号
        /// </summary>
        public class AFN04_F028 : ParamBase
        {
            /// <summary>
            /// 分段限值1
            /// </summary>
            public double LimitValue1 { get; set; }
            /// <summary>
            /// 分段限值2
            /// </summary>
            public double LimitValue2 { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    if (!Dec2Bcd_Format05(LimitValue1, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "功率因数区段1取值范围[-799.9,799.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format05;
                    if (!Dec2Bcd_Format05(LimitValue2, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "功率因数区段2取值范围[-799.9,799.9]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format05;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                double? tmp;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                LimitValue1 = (double)tmp;
                index += (int)FormatLenOption.Format05;

                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                LimitValue2 = (double)tmp;
                index += (int)FormatLenOption.Format05;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 4 ; }
            }
        }
        /// <summary>
        /// F33：总加组数据冻结参数：总加组号
        /// </summary>
        public class AFN04_F033 : ParamBase
        {
            /// <summary>
            /// 有功功率密度
            /// </summary>
            public int ActPowerDenisty { get; set; }
            /// <summary>
            /// 无功功率密度
            /// </summary>
            public int ReactPowerDenisty { get; set; }
            /// <summary>
            /// 有功电量密度
            /// </summary>
            public int ActEnergyDenisty { get; set; }
            /// <summary>
            /// 无功电量密度
            /// </summary>
            public int ReactEnergyDenisty { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];
                    Data[0] = (byte)ActPowerDenisty;
                    Data[1] = (byte)ReactPowerDenisty;
                    Data[2] = (byte)ActEnergyDenisty;
                    Data[3] = (byte)ReactEnergyDenisty;
                }
                return true;
            }
            public override bool Decode()
            {
                ActPowerDenisty = Data[0];
                ReactPowerDenisty = Data[1];
                ActEnergyDenisty = Data[2];
                ReactEnergyDenisty = Data[3];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F41：时段功控定值：总加组号
        /// </summary>
        public class AFN04_F041 : ParamBase
        {
            public class PeriodFixItem
            {
                /// <summary>
                /// 定值方案号
                /// </summary>
                public int SchemeId { get; set; }
                /// <summary>
                /// 功率定值.单位:kW
                /// </summary>
                private double[] fixValue = new double[8];
                public double this[int index]
                {
                    get { return fixValue[index];}
                    set {fixValue[index] = value;}
                }
            }
            /// <summary>
            /// 时段定值方案列表
            /// </summary>
            private List<PeriodFixItem> fixList = new List<PeriodFixItem>();
            public List<PeriodFixItem> FixList { get; set; }
            /// <summary>
            /// 增加一个定值方案
            /// </summary>
            /// <param name="fix"></param>
            public void AddPeriodFix(PeriodFixItem fix)
            {
                fixList.Add(fix);
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    if (fixList.Count > 3)
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "功率定值方案不能超过3个";
                        return false;
                    }
                    int index = 0;
                    Data = new byte[1 + fixList.Count * 17];
                    index++;
                    foreach (PeriodFixItem fix in fixList)
                    {
                        if (fix.SchemeId < 1 || fix.SchemeId > 3)
                        {
                            OperResult = OperResultOption.InvalidValue;
                            OperResultDesc = "功率定值方案号取值范围[1,3]";
                            return false;
                        }
                        Data[0] |= (byte)(1 << (fix.SchemeId-1));
                        Data[index++] = 0xFF;
                        for (int j = 0; j < 8; j++)
                        {
                            if (!Dec2Bcd_Format02(fix[j], Data, index))
                            {
                                OperResult = OperResultOption.InvalidValue;
                                OperResultDesc = "时段功率定值取值范围[-9990000,9990000]";
                                return false;
                            }
                            index += (int)FormatLenOption.Format02;
                        }
                    }
                }
                else
                {
                    RespLen = 52;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                index++;
                for (int i = 0; i < 3; i++)
                {
                    if (((Data[0] >> i) & 0x01) > 0)
                    {
                        PeriodFixItem fix = new PeriodFixItem();
                        fix.SchemeId = i + 1;
                        int tmp = index++;
                        for (int j = 0; j < 8; j++)
                        {
                            if (((Data[tmp] >> j) & 0x01) > 0)
                            {
                                double? val;
                                if (!Bcd2Dec_Format02(Data, index, out val))
                                {
                                    OperResult = OperResultOption.InvalidData;
                                    OperResultDesc = "回码数据无效";
                                    return false;
                                }
                                fix[j] = (double)val;
                                index += (int)FormatLenOption.Format02;
                            }
                        }
                        fixList.Add(fix);
                    }
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F42：厂休功控参数：总加组号
        /// </summary>
        public class AFN04_F042 : ParamBase
        {
            /// <summary>
            /// 厂休定值。单位:kW
            /// </summary>
            public double RestFixValue { get; set; }
            /// <summary>
            /// 起始时间-时
            /// </summary>
            public int StartHour { get; set; }
            /// <summary>
            /// 起始时间-分
            /// </summary>
            public int StartMinute { get; set; }
            /// <summary>
            /// 持续时间。单位:0.5h
            /// </summary>
            public int Duration { get; set; }
            /// <summary>
            /// 每周限电日：D1~D7表示星期一~星期日，D0=0
            /// </summary>
            public byte RestDay { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    if (!Dec2Bcd_Format02(RestFixValue, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "厂休日定值取值范围[-9990000,9990000]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format02;
                    Data[index++] = Dec2Bcd((byte)StartMinute);
                    Data[index++] = Dec2Bcd((byte)StartHour);
                    Data[index++] = (byte)Duration;
                    Data[index++] = RestDay;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                double? val;
                if (!Bcd2Dec_Format02(Data, index, out val))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                RestFixValue = (double)val;
                index += (int)FormatLenOption.Format02;
                StartMinute = Bcd2Dec(Data[index++]);
                StartHour = Bcd2Dec(Data[index++]);
                Duration = Data[index++];
                RestDay = Data[index++];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 6; }
            }
        }
        /// <summary>
        /// F43：功率控制的功率计算滑差时间：总加组号
        /// </summary>
        public class AFN04_F043 : ParamBase
        {
            /// <summary>
            /// 滑差时间。单位：min
            /// </summary>
            public int SlideDif { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[1];
                    if (SlideDif < 1 || SlideDif > 60)
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "功率计算滑差时间取值范围[1,60]";
                        return false;
                    }
                    Data[0] = (byte)SlideDif;
                }
                return true;
            }
            public override bool Decode()
            {
                SlideDif = Data[0];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 1; }
            }
        }
        /// <summary>
        /// F44：营业报停控参数：总加组号
        /// </summary>
        public class AFN04_F044 : ParamBase
        {
            /// <summary>
            /// 开始时间
            /// </summary>
            public DateTime StartTime { get; set; }
            /// <summary>
            /// 结束时间
            /// </summary>
            public DateTime EndTime { get; set; }
            /// <summary>
            /// 报停定值
            /// </summary>
            public double StopFixValue { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    if (!Dec2Bcd_Format20(StartTime, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "报停起始时间无效";
                        return false;
                    }
                    index += (int)FormatLenOption.Format20;
                    if (!Dec2Bcd_Format20(EndTime, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "报停结束时间无效";
                        return false;
                    }
                    index += (int)FormatLenOption.Format20;
                    if (!Dec2Bcd_Format02(StopFixValue, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "报停控定值取值范围[-999000,9990000]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format02;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                StartTime = dt;
                index += (int)FormatLenOption.Format20;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                EndTime = dt;
                index += (int)FormatLenOption.Format20;
                double? tmp;
                if (!Bcd2Dec_Format02(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                StopFixValue = (double)tmp;
                index += (int)FormatLenOption.Format02;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 8; }
            }
        }
        /// <summary>
        /// F45：功控轮次设定：总加组号
        /// </summary>
        public class AFN04_F045 : ParamBase
        {
            /// <summary>
            /// 轮次投入标志 true-投入 false-解除
            /// </summary>
            private bool[] turnFlag = new bool[8];
            /// <summary>
            /// 轮次投入标志。索引0~7表示轮次1~8
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool this[int index]
            {
                get { return turnFlag[index]; }
                set { turnFlag[index] = value; }
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[1];
                    for (int i = 0; i < 8; i++)
                    {
                        Data[0] |= (byte)(turnFlag[i] ? (1 << i) : 0);
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                for (int i = 0; i < 8; i++)
                {
                    if(((Data[0]>>i) & 0x01)>0)
                    {
                        turnFlag[i] = true;
                    }
                    else
                    {
                        turnFlag[i] = false;
                    }
                }
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 1; }
            }
        }
        /// <summary>
        /// F46：月电量控定值：总加组号
        /// </summary>
        public class AFN04_F046 : ParamBase
        {
            /// <summary>
            /// 电量定值
            /// </summary>
            public long? EnergyFixValue { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];
                    if (!Dec2Bcd_Format03((long)EnergyFixValue, Data, 0))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "月电量定值取值范围[-9999999000,9999999000]";
                        return false;
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                long? tmp;
                if (!Bcd2Dec_Format03(Data, 0, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                EnergyFixValue = tmp;
                return true;
            }
            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F47：购电量（费）控参数：总加组号
        /// </summary>
        public class AFN04_F047 : ParamBase
        {
            /// <summary>
            /// 购电单号
            /// </summary>
            public string BuyId { get; set; }
            /// <summary>
            /// 是否刷新 true-刷新 false-追加
            /// </summary>
            public bool IsRefresh { get; set; }
            /// <summary>
            /// 购电定值
            /// </summary>
            public long? BuyValue { get; set; }
            /// <summary>
            /// 告警定值
            /// </summary>
            public long? AlarmValue { get; set; }
            /// <summary>
            /// 跳闸定值
            /// </summary>
            public long? JumpValue { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    for (int i = 0; i < BuyId.Length; i++)
                    {
                        if (BuyId[i] < '0' || BuyId[i] > '9')
                        {
                            OperResult = OperResultOption.InvalidValue;
                            OperResultDesc = "购电单号非数值类型";
                            return false;
                        }
                    }
                    ulong buyId = System.Convert.ToUInt64(BuyId);
                    for (int i = 0; i < 4; i++)
                    {
                        Data[index++] = (byte)(buyId % 256);
                        buyId /= 256;
                    }
                    if (!Dec2Bcd_Format03((long)BuyValue, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "购电定值取值范围[-9999999000,9999999000]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format03;
                    if (!Dec2Bcd_Format03((long)AlarmValue, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "报警定值取值范围[-9999999000,9999999000]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format03;
                    if (!Dec2Bcd_Format03((long)JumpValue, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "跳闸定值取值范围[-9999999000,9999999000]";
                        return false;
                    }
                    index += (int)FormatLenOption.Format03;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                ulong buyId = 0;
                for (int i = 0; i < 4; i++)
                {
                    buyId = buyId * 256 + Data[3 - i];
                }
                index += 4;
                BuyId = System.Convert.ToString(buyId);
                long? tmp;
                if (!Bcd2Dec_Format03(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                BuyValue = tmp;
                index += (int)FormatLenOption.Format03;
                if (!Bcd2Dec_Format03(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                AlarmValue = tmp;
                index += (int)FormatLenOption.Format03;
                if (!Bcd2Dec_Format03(Data, index, out tmp))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                JumpValue = tmp;
                index += (int)FormatLenOption.Format03;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 17; }
            }
        }
        /// <summary>
        /// F48：电控轮次设定：总加组号
        /// </summary>
        public class AFN04_F048 : AFN04_F045
        {

        }
        /// <summary>
        /// F49：功控告警时间：控制轮次
        /// </summary>
        public class AFN04_F049 : ParamBase
        {
            /// <summary>
            /// 告警时间。单位：min
            /// </summary>
            public int AlartTime { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[1];
                    Data[0] = (byte)AlartTime;
                }
                return true;
            }
            public override bool Decode()
            {
                AlartTime = Data[0];
                return true;
            }
            public override int DataFixLen
            {
                get { return 1; }
            }
        }
        /// <summary>
        /// F57：终端声音告警允许∕禁止设置：p0
        /// </summary>
        public class AFN04_F050 : ParamBase
        {
            /// <summary>
            /// 告警允许时段 true-允许告警 false-禁止告警
            /// 索引0~23表示0~23点
            /// </summary>
            private bool[] AlertEnable = new bool[24];
            /// <summary>
            /// 告警允许时段 true-允许告警 false-禁止告警
            /// 索引0~23表示0~23点
            /// </summary>
            public bool this[int index]
            {
                get { return this[index]; }
                set { this[index] = value; }
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[DataFixLen];
                    for (int i = 0; i < 24; i++)
                    {
                        if (AlertEnable[i])
                        {
                            Data[i / 8] = (byte)(1 << (i % 8));
                        }
                    }
                }
                return true;
            }
            public override bool Decode()
            {
                for (int i = 0; i < 24; i++)
                {
                    AlertEnable[i] = ((Data[i / 8] >> (i % 8)) & 0x01) > 0;
                }
                return true;
            }
            public override int DataFixLen
            {
                get { return 3; }
            }
        }
        /// <summary>
        /// F58：终端自动保电参数：p0
        /// </summary>
        public class AFN04_F058 : ParamBase
        {
            /// <summary>
            /// 无通信保电时间。单位：h
            /// 允许终端连续未收到符合本行政区划代码的有效主站报文的时间。
            /// 若终端连续与主站无通信的时间超过本参数规定的值，则解除原有控制状态并自动进入保电状态。
            /// 本参数为0，则表示无自动保电功能。
            /// </summary>
            public int NoCommTime { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[1];
                    Data[0] = (byte)NoCommTime;
                }
                return true;
            }
            public override bool Decode()
            {
                NoCommTime = Data[0];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 1; }
            }
        }
        /// <summary>
        /// F59：电能表异常判别阈值设定：p0
        /// </summary>
        public class AFN04_F059 : ParamBase
        {
            /// <summary>
            /// 超差阈值。单位：kWh
            /// </summary>
            public double OverThreshold { get; set; }
            /// <summary>
            /// 飞走阈值
            /// </summary>
            public double SpeedThreshold { get; set; }
            /// <summary>
            /// 停走阈值。单位：15min
            /// </summary>
            public int StopThreshold { get; set; }
            /// <summary>
            /// 校时阈值。单位：min
            /// </summary>
            public int TimingThreshold { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[DataFixLen];
                    Data[index++] = (byte)OverThreshold;
                    Data[index++] = (byte)SpeedThreshold;
                    Data[index++] = (byte)StopThreshold;
                    Data[index++] = (byte)TimingThreshold;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                OverThreshold = Data[index++];
                SpeedThreshold = Data[index++];
                StopThreshold = Data[index++];
                TimingThreshold = Data[index++];
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F60：谐波限值：p0
        /// </summary>
        public class AFN04_F060 : ParamBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                return false;
            }
            public override bool Decode()
            {
                return false;
            }
            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F61：直流模拟量接入参数：p0
        /// </summary>
        public class AFN04_F061 : ParamBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                return false;
            }
            public override bool Decode()
            {
                return false;
            }
            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F65：定时发送1类数据任务设置：任务号
        /// </summary>
        public class AFN04_F065 : ParamBase
        {
            public class PnFn
            {
                public int Pn { get; set; }
                public int Fn { get; set; }
            }
            /// <summary>
            /// 周期单位。0-分 1-时 2-日 3-月
            /// </summary>
            public int CycleUnit { get; set; }
            /// <summary>
            /// 周期
            /// </summary>
            public int CycleValue { get; set; }
            /// <summary>
            /// 发送基准时间
            /// </summary>
            public DateTime ReferTime { get; set; }
            /// <summary>
            /// 曲线抽取倍率
            /// </summary>
            public int CurveQuotity { get; set; }
            /// <summary>
            /// 数据单元标识列表
            /// </summary>
            private List<PnFn> dataUnitIdList = new List<PnFn>();
            /// <summary>
            /// 数据单元标识列表
            /// </summary>
            public List<PnFn> DataUnitIdList { get { return dataUnitIdList; } }
            /// <summary>
            /// 增加一个数据单元
            /// </summary>
            /// <param name="dataUnitId"></param>
            public void AddDataUnitId(PnFn dataUnitId)
            {
                dataUnitIdList.Add(dataUnitId);
            }

            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    int index = 0;
                    Data = new byte[9 + dataUnitIdList.Count * 4];
                    Data[index] = (byte)(CycleUnit<<6);
                    Data[index] |= (byte)CycleValue;
                    index++;
                    if (!Dec2Bcd_Format01(ReferTime, Data, index))
                    {
                        OperResult = OperResultOption.InvalidValue;
                        OperResultDesc = "发送基准时间无效";
                        return false;
                    }
                    index += (int)FormatLenOption.Format01;
                    Data[index++] = (byte)CurveQuotity;
                    Data[index++] = (byte)dataUnitIdList.Count;
                    foreach (PnFn pf in dataUnitIdList)
                    {
                        //DA
                        if (pf.Pn == 0)
                        {
                            Data[index++] = 0;
                            Data[index++] = 0;
                        }
                        else
                        {
                            Data[index++] = (byte)(1 << ((pf.Pn - 1) % 8));
                            Data[index++] = (byte)(1 << ((pf.Pn - 1) / 8));
                        }
                        //DT
                        Data[index++] = (byte)(1 << ((pf.Fn - 1) % 8));
                        Data[index++] = (byte)((pf.Fn - 1) / 8);
                    }
                }
                else
                {
                    RespLen = 250;
                }
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                CycleUnit = Data[index] >> 6;
                CycleValue = Data[index] & 0x3F;
                index++;
                DateTime dt;
                if (!Bcd2Dec_Format01(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                ReferTime = dt;
                index += (int)FormatLenOption.Format01;
                CurveQuotity = Data[index++];
                int count = Data[index++];
                for (int i = 0; i < count; i++)
                {
                    PnFn pf = new PnFn();
                    //DA
                    if (Data[index+1] == 0)
                    {
                        pf.Pn = 0;
                    }
                    else
                    {
                        for (int k = 0; k < 8; k++)
                        {
                            if(((Data[index]>>k)&0x01)>0)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    if((((Data[index+1]>>j)&0x01)>0))
                                    {
                                        pf.Pn = j*8+k+1;
                                    }
                                }
                            }
                        }
                    }
                    index += 2;
                    //DT
                    for(int j=0;j<8;j++)
                    {
                        if (((Data[index] >> j) & 0x01) > 0)
                        {
                            pf.Fn = Data[index + 1] * 8 + j + 1;
                        }
                    }
                    dataUnitIdList.Add(pf);
                }
                return true;
            }
            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F66：定时发送2类数据任务设置：任务号
        /// </summary>
        public class AFN04_F066 : AFN04_F065
        {

        }
        /// <summary>
        /// F67：定时发送1类数据任务启动/停止设置：任务号
        /// </summary>
        public class AFN04_F067 : ParamBase
        {
            /// <summary>
            /// 任务是否启动 true-启动 false-停止
            /// </summary>
            public bool IsLaunch { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (isSet)
                {
                    Data = new byte[1];
                    Data[0] = (byte)(IsLaunch ? 0x55 : 0xAA);
                }
                return true;
            }
            public override bool Decode()
            {
                IsLaunch = (Data[0] == 0x55);
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 1; }
            }
        }
        /// <summary>
        /// F68：定时发送2类数据任务启动/停止设置：任务号
        /// </summary>
        public class AFN04_F068 : AFN04_F067
        {

        }
        /// <summary>
        /// F73：电容器参数：测量点号
        /// </summary>
        public class AFN04_F073 : ParamBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                return false;
            }
            public override bool Decode()
            {
                return false;
            }
            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F74：电容器投切运行参数：测量点号
        /// </summary>
        public class AFN04_F074 : ParamBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                return false;
            }
            public override bool Decode()
            {
                return false;
            }
            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F75：电容器保护参数：测量点号
        /// </summary>
        public class AFN04_F075 : ParamBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                return false;
            }
            public override bool Decode()
            {
                return false;
            }
            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F76：电容器投切控制方式：测量点号
        /// </summary>
        public class AFN04_F076 : ParamBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                return false;
            }
            public override bool Decode()
            {
                return false;
            }
            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F81：直流模拟量变比：直流模拟量点号
        /// </summary>
        public class AFN04_F081 : ParamBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                return false;
            }
            public override bool Decode()
            {
                return false;
            }
            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F82：直流模拟量限值：直流模拟量点号
        /// </summary>
        public class AFN04_F082 : ParamBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                return false;
            }
            public override bool Decode()
            {
                return false;
            }
            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F83：直流模拟量冻结参数：直流模拟量点号
        /// </summary>
        public class AFN04_F083 : ParamBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                return false;
            }
            public override bool Decode()
            {
                return false;
            }
            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        #endregion//参数数据项定义
        #region 控制数据项定义
        public abstract class CtrlBase : DataUnitBase
        {
            public CtrlBase()
            {
                IsCombined = false;
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                Data = new byte[0];
                return true;
            }
            public override bool Decode()
            {
                return false;
            }
            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        public class AFN05_F001 : CtrlBase
        {
            /// <summary>
            /// 告警延时
            /// 数值范围0~15，单位：min，为“0”时，表示立即跳闸；不为0时，按设置的告警时间进行告警
            /// </summary>
            public int AlertDelay { get; set; }
            /// <summary>
            /// 限电时间
            /// 数值范围0~15，单位：0.5h（半个小时），为0时，表示紧急限电，即长时间限电，不自动解除限电状态；不为0时，按设置的限电时间进行限电
            /// </summary>
            public int CtrlTime { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                IsCombined = true;
                Data = new byte[1];
                Data[0] = (byte)((AlertDelay & 0x0F) << 4);
                Data[0] |= (byte)(CtrlTime & 0x0F); 
                return true;
            }
        }
        /// <summary>
        /// F2：允许合闸
        /// </summary>
        public class AFN05_F002 : CtrlBase
        {

            public override bool Encode(bool isSet, int MaxRespLen)
            {
                IsCombined = true;
                return true;
            }
        }
        /// <summary>
        /// F9：时段功控投入
        /// </summary>
        public class AFN05_F009 : CtrlBase
        {
            /// <summary>
            /// 投入时段
            /// D0~D7按顺序对位表示第1~第8时段，置“1”：有效，置“0”：无效
            /// </summary>
            public byte Period { get; set; }
            /// <summary>
            /// 定值方案号
            /// </summary>
            public int SchemeId { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                Data = new byte[2];
                Data[0] = Period;
                Data[1] = (byte)SchemeId;
                return true;
            }
        }
        /// <summary>
        /// F10：厂休控投入
        /// </summary>
        public class AFN05_F010 : CtrlBase
        {

        }        
        /// <summary>
        /// F11：报停控投入
        /// </summary>
        public class AFN05_F011 : CtrlBase
        {

        }
        /// <summary>
        /// F12：当前功率下浮控投入
        /// </summary>
        public class AFN05_F012 : CtrlBase
        {
            /// <summary>
            /// 滑差时间。单位：min
            /// </summary>
            public int SlideDif { get; set; }
            /// <summary>
            /// 浮动系数
            /// </summary>
            public short FluCoe { get; set; }
            /// <summary>
            /// 冻结延时。单位：min
            /// </summary>
            public int FreDelay { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                Data = new byte[3];
                if (SlideDif < 1 || SlideDif > 60)
                {
                    OperResult = OperResultOption.InvalidValue;
                    OperResultDesc = "滑差时间取值范围应在[1,60]之间。";
                    return false;
                }
                Data[0] = (byte)SlideDif;
                if (FluCoe > 79 || FluCoe < -79)
                {
                    OperResult = OperResultOption.InvalidValue;
                    OperResultDesc = "浮动系数取值范围应在[-79,79]之间。";
                    return false;
                }
                Dec2Bcd_Format04(FluCoe, Data, 1);
                if (FreDelay < 5 || FreDelay > 60)
                {
                    OperResult = OperResultOption.InvalidValue;
                    OperResultDesc = "冻结延时取值范围应在[5,60]之间。";
                    return false;
                }
                Data[2] = (byte)FreDelay;
                return true;
            }
        }
        /// <summary>
        /// F15：月电控投入
        /// </summary>
        public class AFN05_F015 : CtrlBase
        {

        }
        /// <summary>
        /// F16：购电控投入
        /// </summary>
        public class AFN05_F016 : CtrlBase
        {

        }
        /// <summary>
        /// F17：时段功控解除
        /// </summary>
        public class AFN05_F017 : CtrlBase
        {

        }
        /// <summary>
        /// F18：厂休功控解除
        /// </summary>
        public class AFN05_F018 : CtrlBase
        {

        }
        /// <summary>
        /// F19：营业报停功控解除
        /// </summary>
        public class AFN05_F019 : CtrlBase
        {

        }
        /// <summary>
        /// F20：当前功率下浮控解除
        /// </summary>
        public class AFN05_F020 : CtrlBase
        {

        }
        /// <summary>
        /// F23：月电控解除
        /// </summary>
        public class AFN05_F023 : CtrlBase
        {

        }
        /// <summary>
        /// F24：购电控解除
        /// </summary>
        public class AFN05_F024 : CtrlBase
        {

        }
        /// <summary>
        /// F25：终端保电投入
        /// </summary>
        public class AFN05_F025 : CtrlBase
        {
            /// <summary>
            /// 保电持续时间
            /// 数值范围0~48，单位：0.5h（半个小时），为0时，表示无限期保电；不为0时，按设置的保电持续时间进行保电
            /// </summary>
            public int Duration { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                if (Duration < 0 || Duration > 48)
                {
                    OperResult = OperResultOption.InvalidValue;
                    OperResultDesc = "保电持续时间取值范围0~24小时";
                    return false;
                }
                Data[0] = (byte)Duration;
                return true;
            }
        }
        /// <summary>
        /// F26：催费告警投入
        /// </summary>
        public class AFN05_F026 : CtrlBase
        {

        }
        /// <summary>
        /// F27：允许终端与主站通话
        /// </summary>
        public class AFN05_F027 : CtrlBase
        {

        }
        /// <summary>
        /// F28：终端剔除投入
        /// </summary>
        public class AFN05_F028 : CtrlBase
        {

        }
        /// <summary>
        /// F29：允许终端主动上报
        /// </summary>
        public class AFN05_F029 : CtrlBase
        {

        }
        /// <summary>
        /// F31：对时命令
        /// </summary>
        public class AFN05_F031 : CtrlBase
        {
            /// <summary>
            /// 终端时钟
            /// </summary>
            public DateTime Clock { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                Data = new byte[(int)FormatLenOption.Format01];
                Dec2Bcd_Format01(Clock, Data, 0);
                return true;
            }
        }
        /// <summary>
        /// F32：中文信息
        /// </summary>
        public class AFN05_F032 : CtrlBase
        {
            /// <summary>
            /// 信息种类
            /// </summary>
            public int Kind { get; set; }
            /// <summary>
            /// 信息编号
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// 信息内容
            /// </summary>
            public string Content { get; set; }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                int len = 2 + System.Text.Encoding.ASCII.GetByteCount(Content);
                Data = new byte[len];
                Data[0] = (byte)(((Kind & 0x0F)<<4) | (Id & 0x0F));
                Data[1] = (byte)Id;
                System.Text.Encoding.ASCII.GetBytes(Content, 0, Content.Length, Data, 2);
                return true;
            }
        }
        /// <summary>
        /// F33：终端保电解除
        /// </summary>
        public class AFN05_F033 : CtrlBase
        {

        }
        /// <summary>
        /// F34：催费告警解除
        /// </summary>
        public class AFN05_F034 : CtrlBase
        {

        }
        /// <summary>
        /// F35：禁止终端与主站通话
        /// </summary>
        public class AFN05_F035 : CtrlBase
        {

        }
        /// <summary>
        /// F36：终端剔除解除
        /// </summary>
        public class AFN05_F036 : CtrlBase
        {

        }
        /// <summary>
        /// F37：禁止终端主动上报
        /// </summary>
        public class AFN05_F037 : CtrlBase
        {

        }
        #endregion  //控制数据项定义
        #region 数据类型定义
        /// <summary>
        /// 电能量定义
        /// </summary>
        public class EfficacyEnergyValue
        {
            /// /// <summary>
            /// 有无功属性
            /// </summary>
            public EfficacyOptions Attr { get; set; }
            /// <summary>
            /// 电能值。索引0为总
            /// </summary>
            public double?[] Value { get; set; }
        }
        /// <summary>
        /// 电能示值定义
        /// </summary>
        public class EfficacyShowValue : EfficacyEnergyValue
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
        }
        /// <summary>
        /// 带时间的值
        /// </summary>
        public class TimeValue
        {
            /// <summary>
            /// 值
            /// </summary>
            public double? Value { get; set; }
            /// <summary>
            /// 出现时间
            /// </summary>
            public DateTime Time { get; set; }
        }
        /// <summary>
        /// 最大需量
        /// </summary>
        public class EfficacyDemand
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 有无功属性
            /// </summary>
            public EfficacyOptions Attr { get; set; }
            /// <summary>
            /// 需量。索引0为总
            /// </summary>
            public TimeValue[] Value { get; set; }
        }
        public class HourValue
        {
            /// <summary>
            /// 小时时标
            /// </summary>
            public int Hour { get; set; }
            /// <summary>
            /// 有无功属性
            /// </summary>
            public EfficacyOptions Attr { get; set; }
            /// <summary>
            /// 相序
            /// </summary>
            public PhaseOptions Phase { get; set; }
            /// <summary>
            /// 小时值
            /// </summary>
            public double?[] Value = new double?[4];
        }
        #endregion //数据类型定义
        #region 1类数据项定义
        /// <summary>
        /// 1类数据基类定义
        /// </summary>
        public abstract class Cls1DataBase : DataUnitBase
        {
            public Cls1DataBase()
            {
                IsCombined = true;
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                Data = new byte[0];
                return true;
            }
        }
        /// <summary>
        /// F1：终端版本信息
        /// </summary>
        public class AFN0C_F001 : Cls1DataBase
        {
            /// <summary>
            /// 厂商代号
            /// </summary>
            public string FactoryId { get; set; }
            /// <summary>
            /// 设备编号
            /// </summary>
            public string RtuId { get; set; }
            /// <summary>
            /// 终端软件版本号
            /// </summary>
            public string SoftVer { get; set; }
            /// <summary>
            /// 终端软件发布日期：日月年
            /// </summary>
            public DateTime SoftTime { get; set; }
            /// <summary>
            /// 终端配置容量信息码
            /// </summary>
            public String RtuInfo { get; set; }
            public override bool Decode()
            {
                int index = 0;
                FactoryId = System.Text.Encoding.ASCII.GetString(Data, index, 4);
                index += 4;
                RtuId = System.Text.Encoding.ASCII.GetString(Data, index, 8);
                index += 8;
                SoftVer = System.Text.Encoding.ASCII.GetString(Data, index, 4);
                index += 4;
                DateTime dt;
                if (Bcd2Dec_Format20(Data, index, out dt))
                {
                    SoftTime = dt;
                }
                index += (int)FormatLenOption.Format20;
                RtuInfo = System.Text.Encoding.ASCII.GetString(Data, index, 11);
                index += 11;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 30; }
            }
        }
        /// <summary>
        /// F2：终端日历时钟
        /// </summary>
        public class AFN0C_F002 : Cls1DataBase
        {
            /// <summary>
            /// 时钟
            /// </summary>
            public DateTime Clock { get; set; }

            public override bool Decode()
            {
                DateTime dt;
                if (Bcd2Dec_Format01(Data, 0, out dt))
                {
                    Clock = dt;
                    OperResult = OperResultOption.Success;
                }
                else
                {
                    OperResult = OperResultOption.InvalidData;
                }
                return true;
            }

            public override int DataFixLen
            {
                get { return 6; }
            }
        }
        /// <summary>
        /// F3：终端参数状态
        /// </summary>
        public class AFN0C_F003 : Cls1DataBase
        {
            private bool[] paramStatus = new bool[248];
            /// <summary>
            /// 参数状态 true：终端已有该项参数，false：终端无该项数据。
            /// 索引范围0~247。索引0表示F1，依次类推
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool this[int index]
            {
                get
                {
                    if (index < 0 || index > 247)
                    {
                        return false;
                    }
                    else
                    {
                        return paramStatus[index];
                    }
                }
                set
                {
                    if (index < 0 || index > 247)
                        return;
                    else
                    {
                        paramStatus[index] = value;
                    }
                }
            }
            public override bool Decode()
            {
                for (int i = 0; i < 248; i++)
                {
                    paramStatus[i] = (((Data[i / 8] >> (i % 8)) & 0x01) > 0);
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 31; }
            }
        }
        /// <summary>
        /// F4：终端通信状态
        /// </summary>
        public class AFN0C_F004 : Cls1DataBase
        {
            /// <summary>
            /// 允许/禁止通话
            /// </summary>
            public bool IsAllowTalk { get; set; }
            /// <summary>
            /// 允许/禁止主动上报
            /// </summary>
            public bool IsAllowReport { get; set; }
            public override bool Decode()
            {
                IsAllowReport = (Data[0] & 0x01) > 0;
                IsAllowTalk = ((Data[0] >> 1) & 0x01) > 0;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 1; }
            }
        }
        /// <summary>
        /// F5：终端控制设置状态
        /// </summary>
        public class AFN0C_F005 : Cls1DataBase
        {
            /// <summary>
            /// 总加组控制设置状态
            /// </summary>
            public class CtrlSetStatus
            {
                /// <summary>
                /// 总加组编号
                /// </summary>
                public int TotalId { get; set; }
                /// <summary>
                /// 功控定值方案号
                /// </summary>
                public int SchemeId { get; set; }
                /// <summary>
                /// 功控时段有效标志位
                /// 索引0~7表示1~8时段控投入的有效时段，true：有效，false：无效。
                /// </summary>
                public bool[] Period { get; set; }
                /// <summary>
                /// 时段控状态 true-投入 false-解除
                /// </summary>
                public bool IsPeriodCtrl { get; set; }
                /// <summary>
                /// 厂休控状态 true-投入 false-解除
                /// </summary>
                public bool IsRestCtrl { get; set; }
                /// <summary>
                /// 报停控状态 true-投入 false-解除
                /// </summary>
                public bool IsStopCtrl { get; set; }
                /// <summary>
                /// 下浮控状态 true-投入 false-解除
                /// </summary>
                public bool IsFluCtrl { get; set; }
                /// <summary>
                /// 月电控状态
                /// </summary>
                public bool IsMonthCtrl { get; set; }
                /// <summary>
                /// 购电控状态
                /// </summary>
                public bool IsBuyCtrl { get; set; }
                /// <summary>
                /// 功控轮次状态
                /// 索引0~7表示1~8轮次开关的功控受控状态；true：受控，false：不受控
                /// </summary>
                public bool[] PowerTurn { get; set; }
                /// <summary>
                /// 电控轮次状态
                /// 索引0~7表示1~8轮次开关的功控受控状态；true：受控，false：不受控
                /// </summary>
                public bool[] ElectTurn { get; set; }
            }
            /// <summary>
            /// 保电状态
            /// </summary>
            public bool IsProtect { get; set; }
            /// <summary>
            /// 剔除状态
            /// </summary>
            public bool IsReject { get; set; }
            /// <summary>
            /// 催费告警状态
            /// </summary>
            public bool IsChargeAlarm { get; set; }
            private List<CtrlSetStatus> statusList = new List<CtrlSetStatus>();
            /// <summary>
            /// 总加组控制设置状态
            /// </summary>
            public List<CtrlSetStatus> TotalStatusList { get { return statusList; } }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 2 + 4 * DataFixLen;
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                IsProtect = (Data[index] & 0x01) > 0;
                IsReject = ((Data[index] >> 1) & 0x01) > 0;
                IsChargeAlarm = ((Data[index] >> 2) & 0x01) > 0;
                index++;
                byte tmp = Data[index++];
                for (int i = 0; i < 8; i++)
                {
                    if (((tmp >> i) & 0x01) > 0)
                    {
                        CtrlSetStatus stat = new CtrlSetStatus();
                        stat.TotalId = i + 1;
                        stat.SchemeId = Data[index++];
                        stat.Period = new bool[8];
                        for(int j=0;j<8;j++)
                        {
                            stat.Period[j] = ((Data[index] >> j) & 0x01) > 0;
                        }
                        index++;
                        stat.IsPeriodCtrl = (Data[index] & 0x01) > 0;
                        stat.IsRestCtrl = ((Data[index] >> 1) & 0x01) > 0;
                        stat.IsStopCtrl = ((Data[index] >> 2) & 0x01) > 0;
                        stat.IsFluCtrl = ((Data[index] >> 3) & 0x01) > 0;
                        index++;
                        stat.IsMonthCtrl = ((Data[index] >> 0) & 0x01) > 0;
                        stat.IsBuyCtrl = ((Data[index] >> 1) & 0x01) > 0;
                        index++;
                        stat.PowerTurn = new bool[8];
                        for (int j = 0; j < 8; j++)
                        {
                            stat.PowerTurn[j] = ((Data[index] >> j) & 0x01) > 0;
                        }
                        index++;
                        stat.ElectTurn = new bool[8];
                        for (int j = 0; j < 8; j++)
                        {
                            stat.ElectTurn[j] = ((Data[index] >> j) & 0x01) > 0;
                        }
                        index++;
                        statusList.Add(stat);
                    }
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 6; }
            }
        }
        /// <summary>
        /// F6：终端当前控制状态
        /// </summary>
        public class AFN0C_F006 : Cls1DataBase
        {
            /// <summary>
            /// 总加组控制状态
            /// </summary>
            public class CtrlStatus
            {
                /// <summary>
                /// 总加组编号
                /// </summary>
                public int TotalId { get; set; }
                /// <summary>
                /// 当前功控定值
                /// </summary>
                public double PowerFixValue { get; set; }
                /// <summary>
                /// 当前功率下浮控浮动系数
                /// </summary>
                public short FluCoe { get; set; }
                /// <summary>
                /// 功控跳闸输出状态
                /// 索引0~7表示终端1~8轮次功控跳闸输出状态，true：处于功控跳闸状态，false：未处于功控跳闸状态。
                /// </summary>
                public bool[] PowerJumpStatus { get; set; }
                /// <summary>
                /// 月电控跳闸输出状态
                /// 索引0~7表示终端1~8轮次月电控跳闸输出状态，true：处于月电控跳闸状态，false：未处于月电控跳闸状态。
                /// </summary>
                public bool[] MonthJumpStatus { get; set; }
                /// <summary>
                /// 购电控跳闸输出状态
                /// 索引0~7表示终端1~8轮次购电控跳闸输出状态，true：处于购电控跳闸状态，false：未处于购电控跳闸状态。
                /// </summary>
                public bool[] BuyJumpStatus { get; set; }
                /// <summary>
                /// 时段控越限告警
                /// </summary>
                public bool IsPeriodAlarm { get; set; }
                /// <summary>
                /// 厂休控越限告警
                /// </summary>
                public bool IsRestAlarm { get; set; }
                /// <summary>
                /// 报停控越限告警
                /// </summary>
                public bool IsStopAlarm { get; set; }
                /// <summary>
                /// 下浮控越限告警
                /// </summary>
                public bool IsFluAlarm { get; set; }
                /// <summary>
                /// 月电控越限告警
                /// </summary>
                public bool IsMonthAlarm { get; set; }
                /// <summary>
                /// 购电控越限告警
                /// </summary>
                public bool IsBuyAlarm { get; set; }
            }

            public bool[] RemoteJumpStatus { get; set; }
            public bool[] ChargeAlarmStatus { get; set; }
            private List<CtrlStatus> statusList = new List<CtrlStatus>();
            /// <summary>
            /// 总加组控制状态
            /// </summary>
            public List<CtrlStatus> TotalCtrlStatusList { get { return statusList; } }
            public override bool Decode()
            {
                int index = 0;
                RemoteJumpStatus = new bool[8];
                for (int j = 0; j < 8; j++)
                {
                    RemoteJumpStatus[j] = ((Data[index] >> j) & 0x01) > 0;
                }
                index++;
                ChargeAlarmStatus = new bool[8];
                for (int j = 0; j < 8; j++)
                {
                    ChargeAlarmStatus[j] = ((Data[index] >> j) & 0x01) > 0;
                }
                index++;
                byte tmp = Data[index++];
                for (int i = 0; i < 8; i++)
                {
                    if (((tmp >> i) & 0x01) > 0)
                    {
                        CtrlStatus stat = new CtrlStatus();
                        stat.TotalId = i + 1;
                        double? value;
                        if (Bcd2Dec_Format02(Data, index, out value))
                        {
                            stat.PowerFixValue = (double)value;
                        }
                        index += (int)FormatLenOption.Format02;
                        short coe;
                        if (Bcd2Dec_Format04(Data, index, out coe))
                        {
                            stat.FluCoe = coe;
                        }
                        index += (int)FormatLenOption.Format04;
                        stat.PowerJumpStatus = new bool[8];
                        for (int j = 0; j < 8; j++)
                        {
                            stat.PowerJumpStatus[j] = ((Data[index] >> j) & 0x01) > 0;
                        }
                        index++;
                        stat.MonthJumpStatus = new bool[8];
                        for (int j = 0; j < 8; j++)
                        {
                            stat.MonthJumpStatus[j] = ((Data[index] >> j) & 0x01) > 0;
                        }
                        index++;
                        stat.BuyJumpStatus = new bool[8];
                        for (int j = 0; j < 8; j++)
                        {
                            stat.BuyJumpStatus[j] = ((Data[index] >> j) & 0x01) > 0;
                        }
                        index++;
                        stat.IsPeriodAlarm = (Data[index] & 0x01) > 0;
                        stat.IsRestAlarm = ((Data[index] >> 1) & 0x01) > 0;
                        stat.IsStopAlarm = ((Data[index] >> 2) & 0x01) > 0;
                        stat.IsFluAlarm = ((Data[index] >> 2) & 0x01) > 0;
                        index++;
                        stat.IsMonthAlarm = (Data[index] & 0x01) > 0;
                        stat.IsBuyAlarm = ((Data[index] >> 1) & 0x01) > 0;
                        index++;
                        statusList.Add(stat);
                    }
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 8; }
            }
        }
        /// <summary>
        /// F7：终端事件计数器当前值
        /// </summary>
        public class AFN0C_F007 : Cls1DataBase
        {
            /// <summary>
            /// 重要事件计数器ERC1
            /// </summary>
            public int ImpCounter { get; set; }
            /// <summary>
            /// 一般事件计数器ERC2
            /// </summary>
            public int CommCounter { get; set; }
            public override bool Decode()
            {
                ImpCounter = Data[0];
                CommCounter = Data[1];
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F8：终端事件标志状态
        /// </summary>
        public class AFN0C_F008 : Cls1DataBase
        {
            private bool[] eventFlag = new bool[64];
            /// <summary>
            /// 索引0~63对应ERC1~ERC64，true：有事件；false：无事件
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool this[int index]
            {
                get { return eventFlag[index]; }
                set { eventFlag[index] = value; }
            }
            public override bool Decode()
            {
                for (int i = 0; i < 64; i++)
                {
                    eventFlag[i] = ((Data[i / 8] >> (i % 8)) & 0x01) > 0;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 8; }
            }
        }
        /// <summary>
        /// F9：终端状态量及变位标志
        /// </summary>
        public class AFN0C_F009 : Cls1DataBase
        {
            /// <summary>
            /// 变位标志
            /// </summary>
            public bool[] ChangeFlag = new bool[8];
            /// <summary>
            /// 变位状态
            /// </summary>
            public bool[] ChangeStatus = new bool[8];
            public override bool Decode()
            {
                for (int i = 0; i < 8; i++)
                {
                    if (((Data[0] >> i) & 0x01) > 0)
                        ChangeFlag[i] = true;
                    if (((Data[1] >> i) & 0x01) > 0)
                        ChangeStatus[i] = true;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F17：当前总加有功功率
        /// </summary>
        public class AFN0C_F017 : Cls1DataBase
        {
            public EfficacyPhaseValue Power = new EfficacyPhaseValue(EfficacyOptions.ForwardActivePower);
            public override bool Decode()
            {
                double? value;
                if (Bcd2Dec_Format02(Data, 0, out value))
                {
                    Power.Total = (decimal)value;
                }
                else
                {
                    Power.Total = null;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F18：当前总加无功功率
        /// </summary>
        public class AFN0C_F018 : Cls1DataBase
        {
            public EfficacyPhaseValue Power = new EfficacyPhaseValue(EfficacyOptions.ForwardReactivePower);
            public override bool Decode()
            {
                double? value;
                if (Bcd2Dec_Format02(Data, 0, out value))
                {
                    Power.Total = (decimal)value;
                }
                else
                {
                    Power.Total = null;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F19：当日总加有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F019 : Cls1DataBase
        {
            public EfficacyEnergyValue EnergyValue = new EfficacyEnergyValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                bool bSucc = base.Encode(isSet, MaxRespLen);
                RespLen = 1 + 4 * DataFixLen;
                return bSucc;
            }
            public override bool Decode()
            {
                int index = 0;
                EnergyValue.Attr = EfficacyOptions.ForwardActivePower;
                int count = Data[index++];
                for (int i = 0; i < count + 1; i++)
                {
                    long? value;
                    Bcd2Dec_Format03(Data, index, out value);
                    EnergyValue.Value[i] = (double?)value;
                    index += (int)FormatLenOption.Format03;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F20：当日总加无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F020 : AFN0C_F019
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ForwardReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F21：当月总加有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F021 : AFN0C_F019
        {

        }
        /// <summary>
        /// F22：当月总加无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F022 : AFN0C_F020
        {

        }
        /// <summary>
        /// F23：终端当前剩余电量（费）
        /// </summary>
        public class AFN0C_F023 : Cls1DataBase
        {
            /// <summary>
            /// 终端当前剩余电量（费）
            /// </summary>
            public long? RemainValue { get; set; }
            public override bool Decode()
            {
                long? value;
                Bcd2Dec_Format03(Data, 0, out value);
                RemainValue = value;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F24：当前功率下浮控控后总加有功功率冻结值
        /// </summary>
        public class AFN0C_F024 : Cls1DataBase
        {
            /// <summary>
            /// 控后冻结功率
            /// </summary>
            public EfficacyPhaseValue CtrlFrozenPower = new EfficacyPhaseValue(EfficacyOptions.ForwardActivePower);
            public override bool Decode()
            {
                double? value;
                if (Bcd2Dec_Format02(Data, 0, out value))
                {
                    CtrlFrozenPower.Total = (decimal)value;
                    OperResult = OperResultOption.Success;
                }
                else
                {
                    CtrlFrozenPower.Total = null;
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                }
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F25：当前三相及总有/无功功率、功率因数，三相电压、电流、零序电流
        /// </summary>
        public class AFN0C_F025 : Cls1DataBase
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 有功功率
            /// </summary>
            public EfficacyPhaseValue ActivePower = new EfficacyPhaseValue(EfficacyOptions.ForwardActivePower);
            /// <summary>
            /// 无功功率
            /// </summary>
            public EfficacyPhaseValue ReactivePower = new EfficacyPhaseValue(EfficacyOptions.ForwardReactivePower);
            /// <summary>
            /// 功率因数
            /// </summary>
            public EfficacyPhaseValue ActiveFactor = new EfficacyPhaseValue(EfficacyOptions.ForwardActivePower);
            /// <summary>
            /// 电压
            /// </summary>
            public PhaseValue Voltage = new PhaseValue();
            /// <summary>
            /// 电流
            /// </summary>
            public PhaseZeroValue Current = new PhaseZeroValue();
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                {
                    RtuReadTime = dt;
                }
                index += (int)FormatLenOption.Format15;
                double? tmp;
                if (Bcd2Dec_Format09(Data, index, out tmp))
                {
                    ActivePower.Total = (decimal)tmp;
                }
                else
                {
                    ActivePower.Total = null;
                }
                index += (int)FormatLenOption.Format09;
                if (Bcd2Dec_Format09(Data, index, out tmp))
                {
                    ActivePower.A = (decimal)tmp;
                }
                else
                {
                    ActivePower.A = null;
                }
                index += (int)FormatLenOption.Format09;
                if (Bcd2Dec_Format09(Data, index, out tmp))
                {
                    ActivePower.B = (decimal)tmp;
                }
                else
                {
                    ActivePower.B = null;
                }
                index += (int)FormatLenOption.Format09;
                if (Bcd2Dec_Format09(Data, index, out tmp))
                {
                    ActivePower.C = (decimal)tmp;
                }
                else
                {
                    ActivePower.C = null;
                }
                index += (int)FormatLenOption.Format09;
                if (Bcd2Dec_Format09(Data, index, out tmp))
                {
                    ReactivePower.Total = (decimal)tmp;
                }
                else
                {
                    ReactivePower.Total = null;
                }
                index += (int)FormatLenOption.Format09;
                if (Bcd2Dec_Format09(Data, index, out tmp))
                {
                    ReactivePower.A = (decimal)tmp;
                }
                else
                {
                    ReactivePower.A = null;
                }
                index += (int)FormatLenOption.Format09;
                if (Bcd2Dec_Format09(Data, index, out tmp))
                {
                    ReactivePower.B = (decimal)tmp;
                }
                else
                {
                    ReactivePower.B = null;
                }
                index += (int)FormatLenOption.Format09;
                if (Bcd2Dec_Format09(Data, index, out tmp))
                {
                    ReactivePower.C = (decimal)tmp;
                }
                else
                {
                    ReactivePower.C = null;
                }
                index += (int)FormatLenOption.Format09;
                if (Bcd2Dec_Format05(Data, index, out tmp))
                {
                    ActiveFactor.Total = (decimal)tmp;
                }
                else
                {
                    ActiveFactor.Total = null;
                }
                index += (int)FormatLenOption.Format05;
                if (Bcd2Dec_Format05(Data, index, out tmp))
                {
                    ActiveFactor.A = (decimal)tmp;
                }
                else
                {
                    ActiveFactor.A = null;
                }
                index += (int)FormatLenOption.Format05;
                if (Bcd2Dec_Format05(Data, index, out tmp))
                {
                    ActiveFactor.B = (decimal)tmp;
                }
                else
                {
                    ActiveFactor.B = null;
                }
                index += (int)FormatLenOption.Format05;
                if (Bcd2Dec_Format05(Data, index, out tmp))
                {
                    ActiveFactor.C = (decimal)tmp;
                }
                else
                {
                    ActiveFactor.C = null;
                }
                index += (int)FormatLenOption.Format05;
                if (Bcd2Dec_Format07(Data, index, out tmp))
                {
                    Voltage.A = (decimal)tmp;
                }
                else
                {
                    Voltage.A = null;
                }
                index += (int)FormatLenOption.Format07;
                if (Bcd2Dec_Format07(Data, index, out tmp))
                {
                    Voltage.B = (decimal)tmp;
                }
                else
                {
                    Voltage.B = null;
                }
                index += (int)FormatLenOption.Format07;
                if (Bcd2Dec_Format07(Data, index, out tmp))
                {
                    Voltage.C = (decimal)tmp;
                }
                else
                {
                    Voltage.C = null;
                }
                index += (int)FormatLenOption.Format07;
                if (Bcd2Dec_Format06(Data, index, out tmp))
                {
                    Current.A = (decimal)tmp;
                }
                else
                {
                    Current.A = null;
                }
                index += (int)FormatLenOption.Format06;
                if (Bcd2Dec_Format06(Data, index, out tmp))
                {
                    Current.B = (decimal)tmp;
                }
                else
                {
                    Current.B = null;
                }
                index += (int)FormatLenOption.Format06;
                if (Bcd2Dec_Format06(Data, index, out tmp))
                {
                    Current.C = (decimal)tmp;
                }
                else
                {
                    Current.C = null;
                }
                index += (int)FormatLenOption.Format06;
                if (Bcd2Dec_Format06(Data, index, out tmp))
                {
                    Current.Zero = (decimal)tmp;
                }
                else
                {
                    Current.Zero = null;
                }
                index += (int)FormatLenOption.Format06;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 51; }
            }
        }
        /// <summary>
        /// F26：A、B、C三相断相统计数据及最近一次断相记录
        /// </summary>
        public class AFN0C_F026 : Cls1DataBase
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 断相次数
            /// </summary>
            public PhaseTotalValue BreakTimes = new PhaseTotalValue();
            /// <summary>
            /// 断相累计时间
            /// </summary>
            public PhaseTotalValue BreakSumTime = new PhaseTotalValue();
            /// <summary>
            /// 最近一次断相起始时刻
            /// </summary>
            public DateTime LastBreakBegin { get; set; }
            /// <summary>
            /// 最近一次断相结束时刻
            /// </summary>
            public DateTime LastBreakEnd { get; set; }
            /// <summary>
            /// A相最近一次断相起始时刻
            /// </summary>
            public DateTime ALastBreakBegin { get; set; }
            /// <summary>
            /// A相最近一次断相结束时刻
            /// </summary>
            public DateTime ALastBreakEnd { get; set; }
            /// <summary>
            /// B相最近一次断相起始时刻
            /// </summary>
            public DateTime BLastBreakBegin { get; set; }
            /// <summary>
            /// B相最近一次断相结束时刻
            /// </summary>
            public DateTime BLastBreakEnd { get; set; }
            /// <summary>
            /// C相最近一次断相起始时刻
            /// </summary>
            public DateTime CLastBreakBegin { get; set; }
            /// <summary>
            /// C相最近一次断相结束时刻
            /// </summary>
            public DateTime CLastBreakEnd { get; set; }

            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                {
                    RtuReadTime = dt;
                }
                index += (int)FormatLenOption.Format15;
                int tmp;
                if (Bcd2Dec_Format08(Data, index, out tmp))
                {
                    BreakTimes.Total = (decimal)tmp;
                }
                else
                {
                    BreakTimes.Total = null;
                }
                index += (int)FormatLenOption.Format08;
                if (Bcd2Dec_Format08(Data, index, out tmp))
                {
                    BreakTimes.A = (decimal)tmp;
                }
                else
                {
                    BreakTimes.A = null;
                }
                index += (int)FormatLenOption.Format08;
                if (Bcd2Dec_Format08(Data, index, out tmp))
                {
                    BreakTimes.B = (decimal)tmp;
                }
                else
                {
                    BreakTimes.B = null;
                }
                index += (int)FormatLenOption.Format08;
                if (Bcd2Dec_Format08(Data, index, out tmp))
                {
                    BreakTimes.C = (decimal)tmp;
                }
                else
                {
                    BreakTimes.C = null;
                }
                index += (int)FormatLenOption.Format08;
                int value;
                if (Bcd2Dec_Format10(Data, index, out value))
                {
                    BreakSumTime.Total = (decimal)value;
                }
                else
                {
                    BreakSumTime.Total = null;
                }
                index += (int)FormatLenOption.Format10;
                if (Bcd2Dec_Format10(Data, index, out value))
                {
                    BreakSumTime.A = (decimal)value;
                }
                else
                {
                    BreakSumTime.A = null;
                }
                index += (int)FormatLenOption.Format10;
                if (Bcd2Dec_Format10(Data, index, out value))
                {
                    BreakSumTime.B = (decimal)value;
                }
                else
                {
                    BreakSumTime.B = null;
                }
                index += (int)FormatLenOption.Format10;
                if (Bcd2Dec_Format10(Data, index, out value))
                {
                    BreakSumTime.C = (decimal)value;
                }
                else
                {
                    BreakSumTime.C = null;
                }
                index += (int)FormatLenOption.Format10;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    LastBreakBegin = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    LastBreakEnd = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    ALastBreakBegin = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    ALastBreakEnd = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    BLastBreakBegin = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    BLastBreakEnd = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    CLastBreakBegin = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    CLastBreakEnd = dt;
                }
                index += (int)FormatLenOption.Format17;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 57; }
            }
        }
        /// <summary>
        /// F27：电能表日历时钟及电能表状态信息
        /// </summary>
        public class AFN0C_F027 : Cls1DataBase
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 电表时钟
            /// </summary>
            public DateTime MeterTime { get; set; }
            /// <summary>
            /// 电表运行状态字
            /// </summary>
            public byte RunStatus { get; set; }
            /// <summary>
            /// 电网状态字
            /// </summary>
            public byte NetStatus { get; set; }
            /// <summary>
            /// 最近一次编程时间
            /// </summary>
            public DateTime LastProgramTime { get; set; }
            /// <summary>
            /// 最近一次需量清零时间
            /// </summary>
            public DateTime LastClearTime { get; set; }
            /// <summary>
            /// 编程次数
            /// </summary>
            public int ProgramTimes { get; set; }
            /// <summary>
            /// 需量清零次数
            /// </summary>
            public int ClearTimes { get; set; }
            /// <summary>
            /// 电池工作时间
            /// </summary>
            public int BatteryTime { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                {
                    RtuReadTime = dt;
                }
                index = (int)FormatLenOption.Format15;
                if (Bcd2Dec_Format01(Data, index, out dt))
                {
                    MeterTime = dt;
                }
                index = (int)FormatLenOption.Format01;
                RunStatus = Data[index++];
                NetStatus = Data[index++];
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    LastProgramTime = dt;
                }
                index = (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    LastClearTime = dt;
                }
                index = (int)FormatLenOption.Format17;
                int tmp;
                if (Bcd2Dec_Format08(Data, index, out tmp))
                {
                    ProgramTimes = tmp;
                }
                index = (int)FormatLenOption.Format08;
                if (Bcd2Dec_Format08(Data, index, out tmp))
                {
                    ClearTimes = tmp;
                }
                index = (int)FormatLenOption.Format08;
                int value;
                if (Bcd2Dec_Format10(Data, index, out value))
                {
                    BatteryTime = value;
                }
                index = (int)FormatLenOption.Format10;
                OperResult = OperResultOption.Success;
                return true;
            }
            public override int DataFixLen
            {
                get { return 28; }
            }
        }
        /// <summary>
        /// F33：当前正向有/无功电能示值、一/四象限无功电能示值（总、费率1~M）
        /// </summary>
        public class AFN0C_F033 : Cls1DataBase
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 电能值List
            /// </summary>
            private List<EfficacyShowValue> valueList = new List<EfficacyShowValue>();
            public List<EfficacyShowValue> ValueList { get { return valueList; } }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 8 + 1 + (4 + 1) * (5 + 4 + 4 + 4);
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                //默认数据无效
                OperResult = OperResultOption.InvalidData;
                DateTime dt;
                int index = 0;
                if (Bcd2Dec_Format15(Data, index, out dt))
                {
                    RtuReadTime = dt;
                }
                index += (int)FormatLenOption.Format15;
                int count = Data[index++];
                //正向有功
                EfficacyShowValue ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ForwardActivePower;
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format14(Data, index, out ev.Value[i]);
                    index += (int)FormatLenOption.Format14;
                }
                valueList.Add(ev);
                //正向无功
                ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ForwardReactivePower;
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format11(Data, index, out ev.Value[i]);
                    index += (int)FormatLenOption.Format11;
                }
                valueList.Add(ev);
                //一象限无功
                ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ReactivePower1;
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format11(Data, index, out ev.Value[i]);
                    index += (int)FormatLenOption.Format11;
                }
                valueList.Add(ev);
                //四象限无功
                ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ReactivePower4;
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format11(Data, index, out ev.Value[i]);
                    index += (int)FormatLenOption.Format11;
                }
                valueList.Add(ev);
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 17; }
            }
        }
        /// <summary>
        /// F34：当前反向有/无功电能示值、二/三象限无功电能示值（总、费率1~M）
        /// </summary>
        public class AFN0C_F034 : AFN0C_F033
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                foreach (EfficacyShowValue value in ValueList)
                {
                    switch (value.Attr)
                    {
                        case EfficacyOptions.ForwardActivePower: value.Attr = EfficacyOptions.ReverseActivePower; break;
                        case EfficacyOptions.ForwardReactivePower: value.Attr = EfficacyOptions.ReverseReactivePower; break;
                        case EfficacyOptions.ReactivePower1: value.Attr = EfficacyOptions.ReactivePower2; break;
                        case EfficacyOptions.ReactivePower4: value.Attr = EfficacyOptions.ReactivePower3; break;
                    }
                }
                return bSucc;
            }
        }
        /// <summary>
        /// F35：当月正向有/无功最大需量及发生时间（总、费率1~M）
        /// </summary>
        public class AFN0C_F035 : Cls1DataBase
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 最大需量List
            /// </summary>
            private List<EfficacyDemand> valueList = new List<EfficacyDemand>();
            public List<EfficacyDemand> ValueList { get { return valueList; } }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 5 + 1 + (4 + 1) * (3 + 4 + 3 + 4);
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                {
                    RtuReadTime = dt;
                }
                index += (int)FormatLenOption.Format15;
                int count = Data[index++];
                //正向有功
                EfficacyDemand ed = new EfficacyDemand();
                ed.Value = new TimeValue[count + 1];
                ed.RtuReadTime = RtuReadTime;
                ed.Attr = EfficacyOptions.ForwardActivePower;
                double? value;
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format23(Data, index, out value);
                    ed.Value[i].Value = value;
                    index += (int)FormatLenOption.Format23;
                }
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format17(Data, index, out dt);
                    ed.Value[i].Time = dt;
                    index += (int)FormatLenOption.Format17;
                }
                valueList.Add(ed);
                //正向无功
                ed = new EfficacyDemand();
                ed.Value = new TimeValue[count + 1];
                ed.RtuReadTime = RtuReadTime;
                ed.Attr = EfficacyOptions.ForwardReactivePower;
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format23(Data, index, out value);
                    ed.Value[i].Value = value;
                    index += (int)FormatLenOption.Format23;
                }
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format17(Data, index, out dt);
                    ed.Value[i].Time = dt;
                    index += (int)FormatLenOption.Format17;
                }
                valueList.Add(ed);
                OperResult = OperResultOption.Success;
                return true; ;
            }

            public override int DataFixLen
            {
                get { return 14; }
            }
        }
        /// <summary>
        /// F36：当月反向有/无功最大需量及发生时间（总、费率1~M）
        /// </summary>
        public class AFN0C_F036 : AFN0C_F035
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                foreach (EfficacyDemand value in ValueList)
                {
                    switch (value.Attr)
                    {
                        case EfficacyOptions.ForwardActivePower: value.Attr = EfficacyOptions.ReverseActivePower; break;
                        case EfficacyOptions.ForwardReactivePower: value.Attr = EfficacyOptions.ReverseReactivePower; break;
                        case EfficacyOptions.ReactivePower1: value.Attr = EfficacyOptions.ReactivePower2; break;
                        case EfficacyOptions.ReactivePower4: value.Attr = EfficacyOptions.ReactivePower3; break;
                    }
                }
                return bSucc;
            }
        }
        /// <summary>
        /// F37：上月正向有/无功电能示值、一/四象限无功电能示值（总、费率1~M）
        /// </summary>
        public class AFN0C_F037 : AFN0C_F033
        {

        }
        /// <summary>
        /// F38：上月反向有/无功电能示值、二/三象限无功电能示值（总、费率1~M）
        /// </summary>
        public class AFN0C_F038 : AFN0C_F034
        {

        }
        /// <summary>
        /// F39：上月正向有/无功最大需量及发生时间（总、费率1~M）
        /// </summary>
        public class AFN0C_F039 : AFN0C_F035
        {

        }
        /// <summary>
        /// F40：上月反向有/无功最大需量及发生时间（总、费率1~M）
        /// </summary>
        public class AFN0C_F040 : AFN0C_F036
        {

        }
        /// <summary>
        /// F41：当日正向有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F041 : AFN0C_F019
        {

        }
        /// <summary>
        /// F42：当日正向无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F042 : AFN0C_F020
        {

        }
        /// <summary>
        /// F43：当日反向有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F043 : AFN0C_F019
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ReverseActivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F44：当日反向无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F044 : AFN0C_F019
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ReverseReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F45：当月正向有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F045 : AFN0C_F041
        {

        }
        /// <summary>
        /// F46：当月正向无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F046 : AFN0C_F042
        {

        }
        /// <summary>
        /// F47：当月反向有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F047 : AFN0C_F043
        {

        }
        /// <summary>
        /// F48：当月反向无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0C_F048 : AFN0C_F044
        {

        }
        /// <summary>
        /// F49：当前电压、电流相位角
        /// </summary>
        public class AFN0C_F049 : Cls1DataBase
        {
            /// <summary>
            /// 电压相位角
            /// </summary>
            public PhaseValue Voltage = new PhaseValue();
            /// <summary>
            /// 电流相位角
            /// </summary>
            public PhaseValue Current = new PhaseValue();
            public override bool Decode()
            {
                int index = 0;
                double? tmp;
                if (Bcd2Dec_Format05(Data, index, out tmp))
                {
                    Voltage.A = (decimal)tmp;
                }
                else
                {
                    Voltage.A = null;
                }
                index += (int)FormatLenOption.Format05;
                if (Bcd2Dec_Format05(Data, index, out tmp))
                {
                    Voltage.B = (decimal)tmp;
                }
                else
                {
                    Voltage.B = null;
                }
                index += (int)FormatLenOption.Format05;
                if (Bcd2Dec_Format05(Data, index, out tmp))
                {
                    Voltage.C = (decimal)tmp;
                }
                else
                {
                    Voltage.C = null;
                }
                index += (int)FormatLenOption.Format05;
                if (Bcd2Dec_Format05(Data, index, out tmp))
                {
                    Current.A = (decimal)tmp;
                }
                else
                {
                    Current.A = null;
                }
                index += (int)FormatLenOption.Format05;
                if (Bcd2Dec_Format05(Data, index, out tmp))
                {
                    Current.B = (decimal)tmp;
                }
                else
                {
                    Current.B = null;
                }
                index += (int)FormatLenOption.Format05;
                if (Bcd2Dec_Format05(Data, index, out tmp))
                {
                    Current.C = (decimal)tmp;
                }
                else
                {
                    Current.C = null;
                }
                index += (int)FormatLenOption.Format05;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 12; }
            }
        }
        /// <summary>
        /// F57：当前A、B、C三相电压、电流2~N次谐波有效值
        /// </summary>
        public class AFN0C_F057 : Cls1DataBase
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F58：当前A、B、C三相电压、电流2~N次谐波含有率
        /// </summary>
        public class AFN0C_F058 : Cls1DataBase
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F65：当前电容器投切状态
        /// </summary>
        public class AFN0C_F065 : Cls1DataBase
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F66：当前电容器累计补偿投入时间和次数
        /// </summary>
        public class AFN0C_F066 : Cls1DataBase
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F67：当日、当月电容器累计补偿的无功电能量
        /// </summary>
        public class AFN0C_F067 : Cls1DataBase
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F73：直流模拟量实时数据
        /// </summary>
        public class AFN0C_F073 : Cls1DataBase
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F81：小时冻结总加有功功率
        /// </summary>
        public class AFN0C_F081 : Cls1DataBase
        {
            /// <summary>
            /// 小时数据
            /// </summary>
            public HourValue Value = new HourValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 4 * DataFixLen;
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                Value.Attr = EfficacyOptions.ForwardActivePower;
                Value.Phase = PhaseOptions.Total;
                Value.Hour = (Data[index] & 0x3F);
                int den = ((Data[index] >> 6) & 0x03);
                int count = 0;
                switch (den)
                {
                    case 1: count = 4; break;
                    case 2: count = 2; break;
                    case 3: count = 1; break;
                    default: count = 0; break;
                }
                for (int i = 0; i < count; i++)
                {
                    double? tmp;
                    if (Bcd2Dec_Format02(Data, index, out tmp))
                        Value.Value[i * den] = tmp;
                    index += (int)FormatLenOption.Format02;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F82：小时冻结总加无功功率
        /// </summary>
        public class AFN0C_F082 : AFN0C_F081
        {

            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Attr = EfficacyOptions.ForwardReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F83：小时冻结总加有功总电能量
        /// </summary>
        public class AFN0C_F083 : Cls1DataBase
        {
            /// <summary>
            /// 小时数据
            /// </summary>
            public HourValue Value = new HourValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 4 * DataFixLen;
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                Value.Attr = EfficacyOptions.ForwardActivePower;
                Value.Phase = PhaseOptions.Total;
                Value.Hour = (Data[index] & 0x3F);
                int den = ((Data[index] >> 6) & 0x03);
                int count = 0;
                switch (den)
                {
                    case 1: count = 4; break;
                    case 2: count = 2; break;
                    case 3: count = 1; break;
                    default: count = 0; break;
                }
                for (int i = 0; i < count; i++)
                {
                    long? tmp;
                    if (Bcd2Dec_Format03(Data, index, out tmp))
                        Value.Value[i * den] = tmp;
                    index += (int)FormatLenOption.Format03;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F84：小时冻结总加无功总电能量
        /// </summary>
        public class AFN0C_F084 : AFN0C_F083
        {

            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Attr = EfficacyOptions.ForwardReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F89：小时冻结有功功率
        /// </summary>
        public class AFN0C_F089 : Cls1DataBase
        {
            /// <summary>
            /// 小时数据
            /// </summary>
            public HourValue Value = new HourValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 4 * DataFixLen;
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                Value.Attr = EfficacyOptions.ForwardActivePower;
                Value.Phase = PhaseOptions.Total;
                Value.Hour = (Data[index] & 0x3F);
                int den = ((Data[index] >> 6) & 0x03);
                int count = 0;
                switch (den)
                {
                    case 1: count = 4; break;
                    case 2: count = 2; break;
                    case 3: count = 1; break;
                    default: count = 0; break;
                }
                for (int i = 0; i < count; i++)
                {
                    double? tmp;
                    if (Bcd2Dec_Format09(Data, index, out tmp))
                        Value.Value[i * den] = tmp;
                    index += (int)FormatLenOption.Format09;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 3; }
            }
        }
        /// <summary>
        /// F90：小时冻结A相有功功率
        /// </summary>
        public class AFN0C_F090 : AFN0C_F089
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.A;
                return bSucc;
            }
        }
        /// <summary>
        /// F91：小时冻结B相有功功率
        /// </summary>
        public class AFN0C_F091 : AFN0C_F089
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.B;
                return bSucc;
            }
        }
        /// <summary>
        /// F92：小时冻结C相有功功率
        /// </summary>
        public class AFN0C_F092 : AFN0C_F089
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.C;
                return bSucc;
            }
        }
        /// <summary>
        /// F93：小时冻结无功功率
        /// </summary>
        public class AFN0C_F093 : AFN0C_F089
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Attr = EfficacyOptions.ForwardReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F94：小时冻结A相无功功率
        /// </summary>
        public class AFN0C_F094 : AFN0C_F093
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.A;
                return bSucc;
            }
        }
        /// <summary>
        /// F95：小时冻结B相无功功率
        /// </summary>
        public class AFN0C_F095 : AFN0C_F093
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.B;
                return bSucc;
            }
        }
        /// <summary>
        /// F96：小时冻结C相无功功率
        /// </summary>
        public class AFN0C_F096 : AFN0C_F093
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.C;
                return bSucc;
            }
        }
        /// <summary>
        /// F97：小时冻结A相电压
        /// </summary>
        public class AFN0C_F097 : Cls1DataBase
        {
            /// <summary>
            /// 小时数据
            /// </summary>
            public HourValue Value = new HourValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 4 * DataFixLen;
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                Value.Attr = EfficacyOptions.ForwardActivePower;
                Value.Phase = PhaseOptions.A;
                Value.Hour = (Data[index] & 0x3F);
                int den = ((Data[index] >> 6) & 0x03);
                int count = 0;
                switch (den)
                {
                    case 1: count = 4; break;
                    case 2: count = 2; break;
                    case 3: count = 1; break;
                    default: count = 0; break;
                }
                for (int i = 0; i < count; i++)
                {
                    double? tmp;
                    if (Bcd2Dec_Format07(Data, index, out tmp))
                        Value.Value[i * den] = tmp;
                    index += (int)FormatLenOption.Format07;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F98：小时冻结B相电压
        /// </summary>
        public class AFN0C_F098 : AFN0C_F097
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.B;
                return bSucc;
            }
        }
        /// <summary>
        /// F99：小时冻结C相电压
        /// </summary>
        public class AFN0C_F099 : AFN0C_F097
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.C;
                return bSucc;
            }
        }
        /// <summary>
        /// F100：小时冻结A相电流
        /// </summary>
        public class AFN0C_F100 : Cls1DataBase
        {
            /// <summary>
            /// 小时数据
            /// </summary>
            public HourValue Value = new HourValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 4 * DataFixLen;
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                Value.Attr = EfficacyOptions.ForwardActivePower;
                Value.Phase = PhaseOptions.A;
                Value.Hour = (Data[index] & 0x3F);
                int den = ((Data[index] >> 6) & 0x03);
                int count = 0;
                switch (den)
                {
                    case 1: count = 4; break;
                    case 2: count = 2; break;
                    case 3: count = 1; break;
                    default: count = 0; break;
                }
                for (int i = 0; i < count; i++)
                {
                    double? tmp;
                    if (Bcd2Dec_Format06(Data, index, out tmp))
                        Value.Value[i * den] = tmp;
                    index += (int)FormatLenOption.Format06;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F101：小时冻结B相电流
        /// </summary>
        public class AFN0C_F101 : AFN0C_F100
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.B;
                return bSucc;
            }
        }
        /// <summary>
        /// F102：小时冻结C相电流
        /// </summary>
        public class AFN0C_F102 : AFN0C_F100
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.C;
                return bSucc;
            }
        }
        /// <summary>
        /// F103：小时冻结零序电流
        /// </summary>
        public class AFN0C_F103 : AFN0C_F100
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.Zero;
                return bSucc;
            }
        }
        /// <summary>
        /// F105：小时冻结正向有功总电能量
        /// </summary>
        public class AFN0C_F105 : AFN0C_F083
        {

        }
        /// <summary>
        /// F106：小时冻结正向无功总电能量
        /// </summary>
        public class AFN0C_F106 : AFN0C_F084
        {

        }
        /// <summary>
        /// F107：小时冻结反向有功总电能量
        /// </summary>
        public class AFN0C_F107 : AFN0C_F083
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Attr = EfficacyOptions.ReverseActivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F108：小时冻结反向无功总电能量
        /// </summary>
        public class AFN0C_F108 : AFN0C_F083
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Attr = EfficacyOptions.ReverseReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F109：小时冻结正向有功总电能示值
        /// </summary>
        public class AFN0C_F109 : Cls1DataBase
        {
            /// <summary>
            /// 小时数据
            /// </summary>
            public HourValue Value = new HourValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 4 * DataFixLen;
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                Value.Attr = EfficacyOptions.ForwardActivePower;
                Value.Phase = PhaseOptions.Total;
                Value.Hour = (Data[index] & 0x3F);
                int den = ((Data[index] >> 6) & 0x03);
                int count = 0;
                switch (den)
                {
                    case 1: count = 4; break;
                    case 2: count = 2; break;
                    case 3: count = 1; break;
                    default: count = 0; break;
                }
                for (int i = 0; i < count; i++)
                {
                    double? tmp;
                    if (Bcd2Dec_Format11(Data, index, out tmp))
                        Value.Value[i * den] = tmp;
                    index += (int)FormatLenOption.Format11;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F110：小时冻结正向无功总电能示值
        /// </summary>
        public class AFN0C_F110 : AFN0C_F109
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Attr = EfficacyOptions.ForwardReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F111：小时冻结反向有功总电能示值
        /// </summary>
        public class AFN0C_F111 : AFN0C_F109
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Attr = EfficacyOptions.ReverseActivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F112：小时冻结反向无功总电能示值
        /// </summary>
        public class AFN0C_F112 : AFN0C_F109
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Attr = EfficacyOptions.ReverseReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F113：小时冻结总功率因数
        /// </summary>
        public class AFN0C_F113 : Cls1DataBase
        {
            /// <summary>
            /// 小时数据
            /// </summary>
            public HourValue Value = new HourValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 4 * DataFixLen;
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                Value.Attr = EfficacyOptions.ForwardActivePower;
                Value.Phase = PhaseOptions.Total;
                Value.Hour = (Data[index] & 0x3F);
                int den = ((Data[index] >> 6) & 0x03);
                int count = 0;
                switch (den)
                {
                    case 1: count = 4; break;
                    case 2: count = 2; break;
                    case 3: count = 1; break;
                    default: count = 0; break;
                }
                for (int i = 0; i < count; i++)
                {
                    double? tmp;
                    if (Bcd2Dec_Format05(Data, index, out tmp))
                        Value.Value[i * den] = tmp;
                    index += (int)FormatLenOption.Format05;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F114：小时冻结A相功率因数
        /// </summary>
        public class AFN0C_F114 : AFN0C_F113
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.A;
                return bSucc;
            }
        }
        /// <summary>
        /// F115：小时冻结B相功率因数
        /// </summary>
        public class AFN0C_F115 : AFN0C_F113
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.B;
                return bSucc;
            }
        }
        /// <summary>
        /// F116：小时冻结C相功率因数
        /// </summary>
        public class AFN0C_F116 : AFN0C_F113
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                Value.Phase = PhaseOptions.C;
                return bSucc;
            }
        }
        /// <summary>
        /// F121：小时冻结直流模拟量
        /// </summary>

        #endregion
        #region 2类数据项定义
        /// <summary>
        /// 2类数据基类定义
        /// </summary>
        public abstract class Cls2DataBase : DataUnitBase
        {
            /// <summary>
            /// 数据时间
            /// </summary>
            public DateTime DataTime { get; set; }

            public Cls2DataBase()
            {
                IsCombined = true;
            }
        }
        public abstract class Cls2DataTd_c : Cls2DataBase
        {
            /// <summary>
            /// 密度 1-15分钟 2-30分钟 3-60分钟
            /// </summary>
            public DenistyOption Denisty { get; protected set; }
            /// <summary>
            /// 数据点数。0-密度默认一天的点数，>0-需要的点数
            /// </summary>
            public int DataCount { get; protected set; }
            /// <summary>
            /// 返回的曲线值
            /// </summary>
            protected CurveValue curve = null;
            public CurveValue Curve { get { return curve; } }
            /// <summary>
            /// 召测一天的完整曲线
            /// </summary>
            /// <param name="dt">曲线日期，指明年月日即可</param>
            /// <param name="den">密度</param>
            public void SetFullCurveParam(DateTime startTime, DenistyOption den)
            {
                DataTime = new DateTime(startTime.Year, startTime.Month, startTime.Day);
                Denisty = den;
            }
            /// <summary>
            /// 召测曲线部分点
            /// </summary>
            /// <param name="startTime">起始时间，需要指明年月日时分</param>
            /// <param name="endTime">结束时间，需要指明年月日时分</param>
            /// <param name="den">密度</param>
            public void SetPartCurveParam(DateTime startTime, DateTime endTime, DenistyOption den)
            {

            }
            /// <summary>
            /// 召测曲线部分点
            /// </summary>
            /// <param name="startTime">起始时间，需要指明年月日时分</param>
            /// <param name="count">点数</param>
            /// <param name="den">密度</param>
            public void SetPartCurveParam(DateTime startTime, int count, DenistyOption den)
            {
                DataTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 0);
                DataCount = count;
                Denisty = den;
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                //组规约数据
                Data = new byte[7];
                if (!Dec2Bcd_Format15(DataTime, Data, 0))
                    return false;
                int minDiff = Denisty.GetIntervalMin();
                if (DataCount == 0)
                {
                    DataCount = Denisty.GetDayPoint();
                }
                RespLen = 7 + DataCount * DataFixLen;
                if (RespLen > MaxRespLen - 26)
                {//需要组多帧
                    double tmp = (DataCount*DataFixLen)*1.0/(MaxRespLen-26-7);
                    int nFrame = (tmp-(int)tmp)>0 ? (int)tmp+1 : (int)tmp;
                    DataCount = (DataCount / nFrame);
                    for(int i=1;i<nFrame;i++)
                    {
                        DateTime dt = DataTime.AddMinutes(minDiff * DataCount*i);
                        Cls2DataTd_c du = (Cls2DataTd_c)this.MemberwiseClone();
                        du.DataTime = dt;
                        du.Denisty = Denisty;
                        du.DataCount = DataCount;
                        du.Pn = Pn;
                        du.Encode(isSet, MaxRespLen);
                        if (SubDataUnitList == null)
                        {
                            SubDataUnitList = new List<DataUnitBase>();
                        }
                        SubDataUnitList.Add(du);
                    }
                    RespLen = 7 + DataCount * DataFixLen;
                    SubRelation = 1;
                }
                Data[5] = (byte)Denisty;
                Data[6] = (byte)DataCount;
                return true;
            }
            public override bool Merge(DataUnitBase unit)
            {
                Cls2DataTd_c unitCurve = (Cls2DataTd_c)unit;
                if (curve != null && unitCurve.curve != null)
                {
                    curve += unitCurve.Curve;
                    DataCount += unitCurve.DataCount;
                }
                if (curve == null && unitCurve.curve != null)
                {
                    this.DataTime = unitCurve.DataTime;
                    this.Denisty = unitCurve.Denisty;
                    this.DataCount = unitCurve.DataCount;
                    this.curve = unitCurve.curve;
                    this.Data = unitCurve.Data;
                }
                return base.Merge(unit);
            }
        }
        public abstract class Cls2DataTd_d : Cls2DataBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                Data = new byte[3];
                if (!Dec2Bcd_Format20(DataTime, Data, 0))
                    return false;
                return true;
            }
        }
        public abstract class Cls2DataTd_m : Cls2DataBase
        {
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                Data = new byte[2];
                if (!Dec2Bcd_Format21(DataTime, Data, 0))
                    return false;
                return true;
            }
        }
        /// <summary>
        /// F1：日冻结正向有/无功电能示值、一/四象限无功电能示值（总、费率1~M）
        /// </summary>
        public class AFN0D_F001 : Cls2DataTd_d
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 电能值List
            /// </summary>
            private List<EfficacyShowValue> valueList = new List<EfficacyShowValue>();
            public List<EfficacyShowValue> ValueList { get { return valueList; } }
            /// <summary>
            /// 向电能值List增加一个电能值
            /// </summary>
            /// <param name="value"></param>
            public void AddEnergyValue(EfficacyShowValue value)
            {
                valueList.Add(value);
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 8 + 1 + (4 + 1) * (5 + 4 + 4 + 4);
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                //默认数据无效
                OperResult = OperResultOption.InvalidData;
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    return false;
                }
                DataTime = dt;
                index += 3;
                if (!Bcd2Dec_Format15(Data, index, out dt))
                {
                    return false;
                }
                RtuReadTime = dt;
                index += (int)FormatLenOption.Format15;
                int count = Data[index++];
                //正向有功
                EfficacyShowValue ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ForwardActivePower;
                for (int i = 0; i < (count + 1); i++)
                {
                    if (!Bcd2Dec_Format14(Data, index, out ev.Value[i]))
                    {
                        ev.Value[i] = null;
                    }
                    index += (int)FormatLenOption.Format14;
                }
                valueList.Add(ev);
                //正向无功
                ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ForwardReactivePower;
                for (int i = 0; i < (count + 1); i++)
                {
                    if (!Bcd2Dec_Format11(Data, index, out ev.Value[i]))
                    {
                        ev.Value[i] = null;
                    }
                    index += (int)FormatLenOption.Format11;
                }
                valueList.Add(ev);
                //一象限无功
                ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ReactivePower1;
                for (int i = 0; i < (count + 1); i++)
                {
                    if (!Bcd2Dec_Format11(Data, index, out ev.Value[i]))
                    {
                        ev.Value[i] = null;
                    }
                    index += (int)FormatLenOption.Format11;
                }
                valueList.Add(ev);
                //四象限无功
                ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ReactivePower4;
                for (int i = 0; i < (count + 1); i++)
                {
                    if (!Bcd2Dec_Format11(Data, index, out ev.Value[i]))
                    {
                        ev.Value[i] = null;
                    }
                    index += (int)FormatLenOption.Format11;
                }
                valueList.Add(ev);

                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 17; }
            }
        }
        /// <summary>
        /// F2：日冻结反向有/无功电能示值、二/三象限无功电能示值（总、费率1~M）
        /// </summary>
        public class AFN0D_F002 : AFN0D_F001
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                foreach (EfficacyShowValue value in ValueList)
                {
                    switch (value.Attr)
                    {
                        case EfficacyOptions.ForwardActivePower: value.Attr = EfficacyOptions.ReverseActivePower; break;
                        case EfficacyOptions.ForwardReactivePower: value.Attr = EfficacyOptions.ReverseReactivePower; break;
                        case EfficacyOptions.ReactivePower1: value.Attr = EfficacyOptions.ReactivePower2; break;
                        case EfficacyOptions.ReactivePower4: value.Attr = EfficacyOptions.ReactivePower3; break;
                    }
                }
                return bSucc;
            }
        }
        /// <summary>
        /// F3：日冻结正向有/无功最大需量及发生时间（总、费率1~M）
        /// </summary>
        public class AFN0D_F003 : Cls2DataTd_d
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 最大需量List
            /// </summary>
            private List<EfficacyDemand> valueList = new List<EfficacyDemand>();
            public List<EfficacyDemand> ValueList { get { return valueList; } }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 5 + 1 + (4 + 1) * (3 + 4 + 3 + 4);
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                if (Bcd2Dec_Format15(Data, index, out dt))
                {
                    RtuReadTime = dt;
                }
                index += (int)FormatLenOption.Format15;
                int count = Data[index++];
                //正向有功
                EfficacyDemand ed = new EfficacyDemand();
                ed.Value = new TimeValue[count + 1];
                ed.RtuReadTime = RtuReadTime;
                ed.Attr = EfficacyOptions.ForwardActivePower;
                double? value;
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format23(Data, index, out value);
                    ed.Value[i].Value = value;
                    index += (int)FormatLenOption.Format23;
                }
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format17(Data, index, out dt);
                    ed.Value[i].Time = dt;
                    index += (int)FormatLenOption.Format17;
                }
                valueList.Add(ed);
                //正向无功
                ed = new EfficacyDemand();
                ed.Value = new TimeValue[count + 1];
                ed.RtuReadTime = RtuReadTime;
                ed.Attr = EfficacyOptions.ForwardReactivePower;
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format23(Data, index, out value);
                    ed.Value[i].Value = value;
                    index += (int)FormatLenOption.Format23;
                }
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format17(Data, index, out dt);
                    ed.Value[i].Time = dt;
                    index += (int)FormatLenOption.Format17;
                }
                valueList.Add(ed);
                OperResult = OperResultOption.Success;
                return true; ;
            }

            public override int DataFixLen
            {
                get { return 14; }
            }
        }
        /// <summary>
        /// F4：日冻结反向有/无功最大需量及发生时间（总、费率1~M）
        /// </summary>
        public class AFN0D_F004 : AFN0D_F003
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                foreach (EfficacyDemand value in ValueList)
                {
                    switch (value.Attr)
                    {
                        case EfficacyOptions.ForwardActivePower: value.Attr = EfficacyOptions.ReverseActivePower; break;
                        case EfficacyOptions.ForwardReactivePower: value.Attr = EfficacyOptions.ReverseReactivePower; break;
                        case EfficacyOptions.ReactivePower1: value.Attr = EfficacyOptions.ReactivePower2; break;
                        case EfficacyOptions.ReactivePower4: value.Attr = EfficacyOptions.ReactivePower3; break;
                    }
                }
                return bSucc;
            }
        }
        /// <summary>
        /// F5：正向有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F005 : Cls2DataTd_d
        {
            public EfficacyEnergyValue EnergyValue = new EfficacyEnergyValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                bool bSucc = base.Encode(isSet, MaxRespLen);
                RespLen = 1 + 4 * DataFixLen;
                return bSucc;
            }
            public override bool Decode()
            {
                int index = 0;
                EnergyValue.Attr = EfficacyOptions.ForwardActivePower;
                DateTime dt;
                if(!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                int count = Data[index++];
                for (int i = 0; i < count + 1; i++)
                {
                    double? value;
                    Bcd2Dec_Format13(Data, index, out value);
                    EnergyValue.Value[i] = (double?)value;
                    index += (int)FormatLenOption.Format13;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F6：正向无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F006 : AFN0D_F005
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ForwardReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F7：反向有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F007 : AFN0D_F005
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ReverseActivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F8：反向无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F008 : AFN0D_F005
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ReverseReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F9：抄表日正向有/无功电能示值、一/四象限无功电能示值（总、费率1~M）
        /// </summary>
        public class AFN0D_F009 : AFN0D_F001
        {

        }
        /// <summary>
        /// F10：抄表日反向有/无功电能示值、二/三象限无功电能示值（总、费率1~M）
        /// </summary>
        public class AFN0D_F010 : AFN0D_F002
        {

        }
        /// <summary>
        /// F11：抄表日正向有/无功最大需量及发生时间（总、费率1~M）
        /// </summary>
        public class AFN0D_F011 : AFN0D_F003
        {

        }
        /// <summary>
        /// F12：抄表日反向有/无功最大需量及发生时间（总、费率1~M）
        /// </summary>
        public class AFN0D_F012 : AFN0D_F004
        {

        }
        /// <summary>
        /// F17：月正向有/无功电能示值、一/四象限无功电能示值（总、费率1~M）
        /// </summary>
        public class AFN0D_F017 : Cls2DataTd_m
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 电能值List
            /// </summary>
            private List<EfficacyShowValue> valueList = new List<EfficacyShowValue>();
            public List<EfficacyShowValue> ValueList { get { return valueList; } }
            /// <summary>
            /// 向电能值List增加一个电能值
            /// </summary>
            /// <param name="value"></param>
            public void AddEnergyValue(EfficacyShowValue value)
            {
                valueList.Add(value);
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 8 + 1 + (4 + 1) * (5 + 4 + 4 + 4);
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                if (Bcd2Dec_Format15(Data, index, out dt))
                {
                    RtuReadTime = dt;
                }
                index += (int)FormatLenOption.Format15;
                int count = Data[index++];
                //正向有功
                EfficacyShowValue ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ForwardActivePower;
                for (int i = 0; i < (count + 1); i++)
                {
                    if (!Bcd2Dec_Format14(Data, index, out ev.Value[i]))
                    {
                        ev.Value[i] = null;
                    }
                    index += (int)FormatLenOption.Format14;
                }
                valueList.Add(ev);
                //正向无功
                ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ForwardReactivePower;
                for (int i = 0; i < (count + 1); i++)
                {
                    if (!Bcd2Dec_Format11(Data, index, out ev.Value[i]))
                    {
                        ev.Value[i] = null;
                    }
                    index += (int)FormatLenOption.Format11;
                }
                valueList.Add(ev);
                //一象限无功
                ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ReactivePower1;
                for (int i = 0; i < (count + 1); i++)
                {
                    if (!Bcd2Dec_Format11(Data, index, out ev.Value[i]))
                    {
                        ev.Value[i] = null;
                    }
                    index += (int)FormatLenOption.Format11;
                }
                valueList.Add(ev);
                //四象限无功
                ev = new EfficacyShowValue();
                ev.Value = new double?[count + 1];
                ev.RtuReadTime = dt;
                ev.Attr = EfficacyOptions.ReactivePower4;
                for (int i = 0; i < (count + 1); i++)
                {
                    if (!Bcd2Dec_Format11(Data, index, out ev.Value[i]))
                    {
                        ev.Value[i] = null;
                    }
                    index += (int)FormatLenOption.Format11;
                }
                valueList.Add(ev);

                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 17; }
            }
        }
        /// <summary>
        /// F18：月反向有/无功电能示值、二/三象限无功电能示值（总、费率1~M）
        /// </summary>
        public class AFN0D_F018 : AFN0D_F017
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                foreach (EfficacyShowValue value in ValueList)
                {
                    switch (value.Attr)
                    {
                        case EfficacyOptions.ForwardActivePower: value.Attr = EfficacyOptions.ReverseActivePower; break;
                        case EfficacyOptions.ForwardReactivePower: value.Attr = EfficacyOptions.ReverseReactivePower; break;
                        case EfficacyOptions.ReactivePower1: value.Attr = EfficacyOptions.ReactivePower2; break;
                        case EfficacyOptions.ReactivePower4: value.Attr = EfficacyOptions.ReactivePower3; break;
                    }
                }
                return bSucc;
            }
        }
        /// <summary>
        /// F19：月正向有/无功最大需量及发生时间（总、费率1~M）
        /// </summary>
        public class AFN0D_F019 : Cls2DataTd_m
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 最大需量List
            /// </summary>
            private List<EfficacyDemand> valueList = new List<EfficacyDemand>();
            public List<EfficacyDemand> ValueList { get { return valueList; } }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                RespLen = 5 + 1 + (4 + 1) * (3 + 4 + 3 + 4);
                return base.Encode(isSet, MaxRespLen);
            }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                if (Bcd2Dec_Format15(Data, index, out dt))
                {
                    RtuReadTime = dt;
                }
                index += (int)FormatLenOption.Format15;
                int count = Data[index++];
                //正向有功
                EfficacyDemand ed = new EfficacyDemand();
                ed.Value = new TimeValue[count + 1];
                ed.RtuReadTime = RtuReadTime;
                ed.Attr = EfficacyOptions.ForwardActivePower;
                double? value;
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format23(Data, index, out value);
                    ed.Value[i].Value = value;
                    index += (int)FormatLenOption.Format23;
                }
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format17(Data, index, out dt);
                    ed.Value[i].Time = dt;
                    index += (int)FormatLenOption.Format17;
                }
                valueList.Add(ed);
                //正向无功
                ed = new EfficacyDemand();
                ed.Value = new TimeValue[count + 1];
                ed.RtuReadTime = RtuReadTime;
                ed.Attr = EfficacyOptions.ForwardReactivePower;
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format23(Data, index, out value);
                    ed.Value[i].Value = value;
                    index += (int)FormatLenOption.Format23;
                }
                for (int i = 0; i < (count + 1); i++)
                {
                    Bcd2Dec_Format17(Data, index, out dt);
                    ed.Value[i].Time = dt;
                    index += (int)FormatLenOption.Format17;
                }
                valueList.Add(ed);
                OperResult = OperResultOption.Success;
                return true; ;
            }

            public override int DataFixLen
            {
                get { return 14; }
            }
        }
        /// <summary>
        /// F20：月反向有/无功最大需量及发生时间（总、费率1~M）
        /// </summary>
        public class AFN0D_F020 : AFN0D_F019
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                foreach (EfficacyDemand value in ValueList)
                {
                    switch (value.Attr)
                    {
                        case EfficacyOptions.ForwardActivePower: value.Attr = EfficacyOptions.ReverseActivePower; break;
                        case EfficacyOptions.ForwardReactivePower: value.Attr = EfficacyOptions.ReverseReactivePower; break;
                        case EfficacyOptions.ReactivePower1: value.Attr = EfficacyOptions.ReactivePower2; break;
                        case EfficacyOptions.ReactivePower4: value.Attr = EfficacyOptions.ReactivePower3; break;
                    }
                }
                return bSucc;
            }
        }
        /// <summary>
        /// F21：月正向有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F021 : Cls2DataTd_m
        {
            public EfficacyEnergyValue EnergyValue = new EfficacyEnergyValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                bool bSucc = base.Encode(isSet, MaxRespLen);
                RespLen = 1 + 4 * DataFixLen;
                return bSucc;
            }
            public override bool Decode()
            {
                int index = 0;
                EnergyValue.Attr = EfficacyOptions.ForwardActivePower;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                int count = Data[index++];
                for (int i = 0; i < count + 1; i++)
                {
                    double? value;
                    Bcd2Dec_Format13(Data, index, out value);
                    EnergyValue.Value[i] = (double?)value;
                    index += (int)FormatLenOption.Format13;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F22：月正向无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F022 : AFN0D_F021
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ForwardReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F23：月反向有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F023 : AFN0D_F021
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ReverseActivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F24：月反向无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F024 : AFN0D_F021
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ReverseReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F25：日总及分相最大有功功率及发生时间、有功功率为零时间
        /// </summary>
        public class AFN0D_F025 : Cls2DataTd_d
        {
            /// <summary>
            /// 最大功率及时间
            /// </summary>
            public TimeValue MaxPower { get; set; }
            /// <summary>
            /// A相最大功率及时间
            /// </summary>
            public TimeValue AMaxPower { get; set; }
            /// <summary>
            /// B相最大功率及时间
            /// </summary>
            public TimeValue BMaxPower { get; set; }
            /// <summary>
            /// C相最大功率及时间
            /// </summary>
            public TimeValue CMaxPower { get; set; }
            /// <summary>
            /// 功率为0时间
            /// </summary>
            public PhaseTotalValue ZeroPowerTime { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                double? value;
                Bcd2Dec_Format23(Data, index, out value);
                MaxPower.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                MaxPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                AMaxPower.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                AMaxPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                BMaxPower.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                BMaxPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                CMaxPower.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                CMaxPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                ZeroPowerTime.Total = Byte2Long(Data, index, 2);
                index += 2;
                ZeroPowerTime.A = Byte2Long(Data, index, 2);
                index += 2;
                ZeroPowerTime.B = Byte2Long(Data, index, 2);
                index += 2;
                ZeroPowerTime.C = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 35; }
            }
        }
        /// <summary>
        /// F26：日总及分相最大需量及发生时间
        /// </summary>
        public class AFN0D_F026 : Cls2DataTd_d
        {
            /// <summary>
            /// 总最大需量及时间
            /// </summary>
            public TimeValue MaxDemand { get; set; }
            /// <summary>
            /// A相最大需量及时间
            /// </summary>
            public TimeValue AMaxDemand { get; set; }
            /// <summary>
            /// B相最大需量及时间
            /// </summary>
            public TimeValue BMaxDemand { get; set; }
            /// <summary>
            /// C相最大需量及时间
            /// </summary>
            public TimeValue CMaxDemand { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                double? value;
                Bcd2Dec_Format23(Data, index, out value);
                MaxDemand.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                MaxDemand.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                AMaxDemand.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                AMaxDemand.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                BMaxDemand.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                BMaxDemand.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                CMaxDemand.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                CMaxDemand.Time = dt;
                index += (int)FormatLenOption.Format18;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 27; }
            }
        }
        /// <summary>
        /// F27：日电压统计数据
        /// </summary>
        public class AFN0D_F027 : Cls2DataTd_d
        {
            /// <summary>
            /// 电压越上上限时间
            /// </summary>
            public PhaseValue VolUpperTime { get; set; }
            /// <summary>
            /// 电压越下下限时间
            /// </summary>
            public PhaseValue VolLowerTime { get; set; }
            /// <summary>
            /// 电压越上限时间
            /// </summary>
            public PhaseValue VolUpTime { get; set; }
            /// <summary>
            /// 电压越下限时间
            /// </summary>
            public PhaseValue VolLowTime { get; set; }
            /// <summary>
            /// 电压合格时间
            /// </summary>
            public PhaseValue VolQualifiedTime { get; set; }
            /// <summary>
            /// A相最大电压
            /// </summary>
            public TimeValue AMaxVol { get; set; }
            /// <summary>
            /// A相最小电压
            /// </summary>
            public TimeValue AMinVol { get; set; }
            /// <summary>
            /// B相最大电压
            /// </summary>
            public TimeValue BMaxVol { get; set; }
            /// <summary>
            /// B相最小电压
            /// </summary>
            public TimeValue BMinVol { get; set; }
            /// <summary>
            /// C相最大电压
            /// </summary>
            public TimeValue CMaxVol { get; set; }
            /// <summary>
            /// C相最小电压
            /// </summary>
            public TimeValue CMinVol { get; set; }
            /// <summary>
            /// 平均电压
            /// </summary>
            public PhaseValue AvgVol { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                double? value;
                VolUpperTime.A = Byte2Long(Data, index, 2);
                index += 2;
                VolLowerTime.A = Byte2Long(Data, index, 2);
                index += 2;
                VolUpTime.A = Byte2Long(Data, index, 2);
                index += 2;
                VolLowTime.A = Byte2Long(Data, index, 2);
                index += 2;
                VolQualifiedTime.A = Byte2Long(Data, index, 2);
                index += 2;
                VolUpperTime.B = Byte2Long(Data, index, 2);
                index += 2;
                VolLowerTime.B = Byte2Long(Data, index, 2); ;
                index += 2;
                VolUpTime.B = Byte2Long(Data, index, 2); ;
                index += 2;
                VolLowTime.B = Byte2Long(Data, index, 2);
                index += 2;
                VolQualifiedTime.B = Byte2Long(Data, index, 2);
                index += 2; 
                VolUpperTime.C = Byte2Long(Data, index, 2);
                index += 2;
                VolLowerTime.C = Byte2Long(Data, index, 2);
                index += 2;
                VolUpTime.C = Byte2Long(Data, index, 2);
                index += 2;
                VolLowTime.C = Byte2Long(Data, index, 2);
                index += 2;
                VolQualifiedTime.C = Byte2Long(Data, index, 2);
                index += 2;
                Bcd2Dec_Format07(Data, index, out value);
                AMaxVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                AMaxVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                AMinVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                AMinVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                BMaxVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                BMaxVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                BMinVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                BMinVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                CMaxVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                CMaxVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                CMinVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                CMinVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                AvgVol.A = (decimal)value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format07(Data, index, out value);
                AvgVol.B = (decimal)value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format07(Data, index, out value);
                AvgVol.C = (decimal)value;
                index += (int)FormatLenOption.Format07;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 69; }
            }
        }
        /// <summary>
        /// F28：日不平衡度越限累计时间
        /// </summary>
        public class AFN0D_F028 : Cls2DataTd_d
        {
            /// <summary>
            /// 电流不平衡累计时间
            /// </summary>
            public long? CurrUnblanceTime { get; set; }
            /// <summary>
            /// 电压不平衡累计时间
            /// </summary>
            public long? VolUnblanceTime { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20; CurrUnblanceTime = Byte2Long(Data, index, 2);
                index += 2;
                VolUnblanceTime = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 7; }
            }
        }
        /// <summary>
        /// F29：日电流越限统计
        /// </summary>
        public class AFN0D_F029 : Cls2DataTd_d
        {
            /// <summary>
            /// 电流越上上限时间
            /// </summary>
            public PhaseValue CurrUpperTime { get; set; }
            /// <summary>
            /// 电流越上限时间
            /// </summary>
            public PhaseZeroValue CurrUpTime { get; set; }
            /// <summary>
            /// A相电流最大值
            /// </summary>
            public TimeValue AMaxCurr { get; set; }
            /// <summary>
            /// B相电流最大值
            /// </summary>
            public TimeValue BMaxCurr { get; set; }
            /// <summary>
            /// C相电流最大值
            /// </summary>
            public TimeValue CMaxCurr { get; set; }
            /// <summary>
            /// 零相电流最大值
            /// </summary>
            public TimeValue ZeroMaxCurr { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                CurrUpperTime.A = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpTime.A = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpperTime.B = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpTime.B = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpperTime.C = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpTime.C = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpTime.Zero = Byte2Long(Data, index, 2);
                index += 2;
                double? value;
                Bcd2Dec_Format06(Data, index, out value);
                AMaxCurr.Value = value;
                index += (int)FormatLenOption.Format06;
                Bcd2Dec_Format18(Data, index, out dt);
                AMaxCurr.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format06(Data, index, out value);
                BMaxCurr.Value = value;
                index += (int)FormatLenOption.Format06;
                Bcd2Dec_Format18(Data, index, out dt);
                BMaxCurr.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format06(Data, index, out value);
                CMaxCurr.Value = value;
                index += (int)FormatLenOption.Format06;
                Bcd2Dec_Format18(Data, index, out dt);
                CMaxCurr.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format06(Data, index, out value);
                ZeroMaxCurr.Value = value;
                index += (int)FormatLenOption.Format06;
                Bcd2Dec_Format18(Data, index, out dt);
                ZeroMaxCurr.Time = dt;
                index += (int)FormatLenOption.Format18;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 37; }
            }
        }
        /// <summary>
        /// F30：日视在功率越限累计时间
        /// </summary>
        public class AFN0D_F030 : Cls2DataTd_d
        {
            /// <summary>
            /// 视在功率越上上限时间
            /// </summary>
            public long? AppUpperTime { get; set; }
            /// <summary>
            /// 视在功率越上限时间
            /// </summary>
            public long? AppUpTime { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20; 
                AppUpperTime = Byte2Long(Data, index, 2);
                index += 2;
                AppUpTime = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 7; }
            }
        }
        /// <summary>
        /// F31：日电能表断相数据
        /// </summary>
        public class AFN0D_F031 : Cls2DataTd_d
        {
            /// <summary>
            /// 终端抄表时间
            /// </summary>
            public DateTime RtuReadTime { get; set; }
            /// <summary>
            /// 断相次数
            /// </summary>
            public PhaseTotalValue BreakTimes = new PhaseTotalValue();
            /// <summary>
            /// 断相累计时间
            /// </summary>
            public PhaseTotalValue BreakSumTime = new PhaseTotalValue();
            /// <summary>
            /// 最近一次断相起始时刻
            /// </summary>
            public DateTime LastBreakBegin { get; set; }
            /// <summary>
            /// 最近一次断相结束时刻
            /// </summary>
            public DateTime LastBreakEnd { get; set; }
            /// <summary>
            /// A相最近一次断相起始时刻
            /// </summary>
            public DateTime ALastBreakBegin { get; set; }
            /// <summary>
            /// A相最近一次断相结束时刻
            /// </summary>
            public DateTime ALastBreakEnd { get; set; }
            /// <summary>
            /// B相最近一次断相起始时刻
            /// </summary>
            public DateTime BLastBreakBegin { get; set; }
            /// <summary>
            /// B相最近一次断相结束时刻
            /// </summary>
            public DateTime BLastBreakEnd { get; set; }
            /// <summary>
            /// C相最近一次断相起始时刻
            /// </summary>
            public DateTime CLastBreakBegin { get; set; }
            /// <summary>
            /// C相最近一次断相结束时刻
            /// </summary>
            public DateTime CLastBreakEnd { get; set; }

            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20; 
                if (Bcd2Dec_Format15(Data, index, out dt))
                {
                    RtuReadTime = dt;
                }
                index += (int)FormatLenOption.Format15;
                int tmp;
                if (Bcd2Dec_Format08(Data, index, out tmp))
                {
                    BreakTimes.Total = (decimal)tmp;
                }
                else
                {
                    BreakTimes.Total = null;
                }
                index += (int)FormatLenOption.Format08;
                if (Bcd2Dec_Format08(Data, index, out tmp))
                {
                    BreakTimes.A = (decimal)tmp;
                }
                else
                {
                    BreakTimes.A = null;
                }
                index += (int)FormatLenOption.Format08;
                if (Bcd2Dec_Format08(Data, index, out tmp))
                {
                    BreakTimes.B = (decimal)tmp;
                }
                else
                {
                    BreakTimes.B = null;
                }
                index += (int)FormatLenOption.Format08;
                if (Bcd2Dec_Format08(Data, index, out tmp))
                {
                    BreakTimes.C = (decimal)tmp;
                }
                else
                {
                    BreakTimes.C = null;
                }
                index += (int)FormatLenOption.Format08;
                int value;
                if (Bcd2Dec_Format10(Data, index, out value))
                {
                    BreakSumTime.Total = (decimal)value;
                }
                else
                {
                    BreakSumTime.Total = null;
                }
                index += (int)FormatLenOption.Format10;
                if (Bcd2Dec_Format10(Data, index, out value))
                {
                    BreakSumTime.A = (decimal)value;
                }
                else
                {
                    BreakSumTime.A = null;
                }
                index += (int)FormatLenOption.Format10;
                if (Bcd2Dec_Format10(Data, index, out value))
                {
                    BreakSumTime.B = (decimal)value;
                }
                else
                {
                    BreakSumTime.B = null;
                }
                index += (int)FormatLenOption.Format10;
                if (Bcd2Dec_Format10(Data, index, out value))
                {
                    BreakSumTime.C = (decimal)value;
                }
                else
                {
                    BreakSumTime.C = null;
                }
                index += (int)FormatLenOption.Format10;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    LastBreakBegin = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    LastBreakEnd = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    ALastBreakBegin = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    ALastBreakEnd = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    BLastBreakBegin = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    BLastBreakEnd = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    CLastBreakBegin = dt;
                }
                index += (int)FormatLenOption.Format17;
                if (Bcd2Dec_Format17(Data, index, out dt))
                {
                    CLastBreakEnd = dt;
                }
                index += (int)FormatLenOption.Format17;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 60; }
            }
        }
        /// <summary>
        /// F33：月总及分相最大有功功率及发生时间、有功功率为零时间
        /// </summary>
        public class AFN0D_F033 : Cls2DataTd_c
        {
            /// <summary>
            /// 最大功率及时间
            /// </summary>
            public TimeValue MaxPower { get; set; }
            /// <summary>
            /// A相最大功率及时间
            /// </summary>
            public TimeValue AMaxPower { get; set; }
            /// <summary>
            /// B相最大功率及时间
            /// </summary>
            public TimeValue BMaxPower { get; set; }
            /// <summary>
            /// C相最大功率及时间
            /// </summary>
            public TimeValue CMaxPower { get; set; }
            /// <summary>
            /// 功率为0时间
            /// </summary>
            public PhaseTotalValue ZeroPowerTime { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                double? value;
                Bcd2Dec_Format23(Data, index, out value);
                MaxPower.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                MaxPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                AMaxPower.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                AMaxPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                BMaxPower.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                BMaxPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                CMaxPower.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                CMaxPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                ZeroPowerTime.Total = Byte2Long(Data, index, 2);
                index += 2;
                ZeroPowerTime.A = Byte2Long(Data, index, 2);
                index += 2;
                ZeroPowerTime.B = Byte2Long(Data, index, 2);
                index += 2;
                ZeroPowerTime.C = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 34; }
            }
        }
        /// <summary>
        /// F34：月总及分相有功最大需量及发生时间
        /// </summary>
        public class AFN0D_F034 : Cls2DataTd_m
        {
            /// <summary>
            /// 总最大需量及时间
            /// </summary>
            public TimeValue MaxDemand { get; set; }
            /// <summary>
            /// A相最大需量及时间
            /// </summary>
            public TimeValue AMaxDemand { get; set; }
            /// <summary>
            /// B相最大需量及时间
            /// </summary>
            public TimeValue BMaxDemand { get; set; }
            /// <summary>
            /// C相最大需量及时间
            /// </summary>
            public TimeValue CMaxDemand { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                double? value;
                Bcd2Dec_Format23(Data, index, out value);
                MaxDemand.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                MaxDemand.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                AMaxDemand.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                AMaxDemand.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                BMaxDemand.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                BMaxDemand.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format23(Data, index, out value);
                CMaxDemand.Value = value;
                index += (int)FormatLenOption.Format23;
                Bcd2Dec_Format18(Data, index, out dt);
                CMaxDemand.Time = dt;
                index += (int)FormatLenOption.Format18;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 26; }
            }
        }
        /// <summary>
        /// F35：月电压统计数据
        /// </summary>
        public class AFN0D_F035 : Cls2DataTd_m
        {
            /// <summary>
            /// 电压越上上限时间
            /// </summary>
            public PhaseValue VolUpperTime { get; set; }
            /// <summary>
            /// 电压越下下限时间
            /// </summary>
            public PhaseValue VolLowerTime { get; set; }
            /// <summary>
            /// 电压越上限时间
            /// </summary>
            public PhaseValue VolUpTime { get; set; }
            /// <summary>
            /// 电压越下限时间
            /// </summary>
            public PhaseValue VolLowTime { get; set; }
            /// <summary>
            /// 电压合格时间
            /// </summary>
            public PhaseValue VolQualifiedTime { get; set; }
            /// <summary>
            /// A相最大电压
            /// </summary>
            public TimeValue AMaxVol { get; set; }
            /// <summary>
            /// A相最小电压
            /// </summary>
            public TimeValue AMinVol { get; set; }
            /// <summary>
            /// B相最大电压
            /// </summary>
            public TimeValue BMaxVol { get; set; }
            /// <summary>
            /// B相最小电压
            /// </summary>
            public TimeValue BMinVol { get; set; }
            /// <summary>
            /// C相最大电压
            /// </summary>
            public TimeValue CMaxVol { get; set; }
            /// <summary>
            /// C相最小电压
            /// </summary>
            public TimeValue CMinVol { get; set; }
            /// <summary>
            /// 平均电压
            /// </summary>
            public PhaseValue AvgVol { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                double? value;
                VolUpperTime.A = Byte2Long(Data, index, 2);
                index += 2;
                VolLowerTime.A = Byte2Long(Data, index, 2);
                index += 2;
                VolUpTime.A = Byte2Long(Data, index, 2);
                index += 2;
                VolLowTime.A = Byte2Long(Data, index, 2);
                index += 2;
                VolQualifiedTime.A = Byte2Long(Data, index, 2);
                index += 2;
                VolUpperTime.B = Byte2Long(Data, index, 2);
                index += 2;
                VolLowerTime.B = Byte2Long(Data, index, 2); ;
                index += 2;
                VolUpTime.B = Byte2Long(Data, index, 2); ;
                index += 2;
                VolLowTime.B = Byte2Long(Data, index, 2);
                index += 2;
                VolQualifiedTime.B = Byte2Long(Data, index, 2);
                index += 2;
                VolUpperTime.C = Byte2Long(Data, index, 2);
                index += 2;
                VolLowerTime.C = Byte2Long(Data, index, 2);
                index += 2;
                VolUpTime.C = Byte2Long(Data, index, 2);
                index += 2;
                VolLowTime.C = Byte2Long(Data, index, 2);
                index += 2;
                VolQualifiedTime.C = Byte2Long(Data, index, 2);
                index += 2;
                Bcd2Dec_Format07(Data, index, out value);
                AMaxVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                AMaxVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                AMinVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                AMinVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                BMaxVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                BMaxVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                BMinVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                BMinVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                CMaxVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                CMaxVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                CMinVol.Value = value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format18(Data, index, out dt);
                CMinVol.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format07(Data, index, out value);
                AvgVol.A = (decimal)value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format07(Data, index, out value);
                AvgVol.B = (decimal)value;
                index += (int)FormatLenOption.Format07;
                Bcd2Dec_Format07(Data, index, out value);
                AvgVol.C = (decimal)value;
                index += (int)FormatLenOption.Format07;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 68; }
            }
        }
        /// <summary>
        /// F36：月不平衡度越限累计时间
        /// </summary>
        public class AFN0D_F036 : Cls2DataTd_m
        {
            /// <summary>
            /// 电流不平衡累计时间
            /// </summary>
            public long? CurrUnblanceTime { get; set; }
            /// <summary>
            /// 电压不平衡累计时间
            /// </summary>
            public long? VolUnblanceTime { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                CurrUnblanceTime = Byte2Long(Data, index, 2);
                index += 2;
                VolUnblanceTime = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 6; }
            }
        }
        /// <summary>
        /// F37：月电流越限统计
        /// </summary>
        public class AFN0D_F037 : Cls2DataTd_m
        {
            /// <summary>
            /// 电流越上上限时间
            /// </summary>
            public PhaseValue CurrUpperTime { get; set; }
            /// <summary>
            /// 电流越上限时间
            /// </summary>
            public PhaseZeroValue CurrUpTime { get; set; }
            /// <summary>
            /// A相电流最大值
            /// </summary>
            public TimeValue AMaxCurr { get; set; }
            /// <summary>
            /// B相电流最大值
            /// </summary>
            public TimeValue BMaxCurr { get; set; }
            /// <summary>
            /// C相电流最大值
            /// </summary>
            public TimeValue CMaxCurr { get; set; }
            /// <summary>
            /// 零相电流最大值
            /// </summary>
            public TimeValue ZeroMaxCurr { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                CurrUpperTime.A = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpTime.A = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpperTime.B = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpTime.B = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpperTime.C = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpTime.C = Byte2Long(Data, index, 2);
                index += 2;
                CurrUpTime.Zero = Byte2Long(Data, index, 2);
                index += 2;
                double? value;
                Bcd2Dec_Format06(Data, index, out value);
                AMaxCurr.Value = value;
                index += (int)FormatLenOption.Format06;
                Bcd2Dec_Format18(Data, index, out dt);
                AMaxCurr.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format06(Data, index, out value);
                BMaxCurr.Value = value;
                index += (int)FormatLenOption.Format06;
                Bcd2Dec_Format18(Data, index, out dt);
                BMaxCurr.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format06(Data, index, out value);
                CMaxCurr.Value = value;
                index += (int)FormatLenOption.Format06;
                Bcd2Dec_Format18(Data, index, out dt);
                CMaxCurr.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format06(Data, index, out value);
                ZeroMaxCurr.Value = value;
                index += (int)FormatLenOption.Format06;
                Bcd2Dec_Format18(Data, index, out dt);
                ZeroMaxCurr.Time = dt;
                index += (int)FormatLenOption.Format18;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 36; }
            }
        }
        /// <summary>
        /// F38：月视在功率越限累计时间
        /// </summary>
        public class AFN0D_F038 : Cls2DataTd_m
        {
            /// <summary>
            /// 视在功率越上上限时间
            /// </summary>
            public long? AppUpperTime { get; set; }
            /// <summary>
            /// 视在功率越上限时间
            /// </summary>
            public long? AppUpTime { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                AppUpperTime = Byte2Long(Data, index, 2);
                index += 2;
                AppUpTime = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 6; }
            }
        }
        /// <summary>
        /// F41：电容器投入累计时间和次数
        /// </summary>
        public class AFN0D_F041 : Cls2DataTd_d
        {
            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F42：日、月电容器累计补偿的无功电能量
        /// </summary>
        public class AFN0D_F042 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F43：日功率因数区段累计时间
        /// </summary>
        public class AFN0D_F043 : Cls2DataTd_d
        {
            /// <summary>
            /// 区段1累计时间
            /// </summary>
            public long? Zone1Time { get; set; }
            /// <summary>
            /// 区段2累计时间
            /// </summary>
            public long? Zone2Time { get; set; }
            /// <summary>
            /// 区段3累计时间
            /// </summary>
            public long? Zone3Time { get; set; }

            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                Zone1Time = Byte2Long(Data, index, 2);
                index += 2;
                Zone2Time = Byte2Long(Data, index, 2);
                index += 2;
                Zone2Time = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 9; }
            }
        }
        /// <summary>
        /// F44：月功率因数区段累计时间
        /// </summary>
        public class AFN0D_F044 : Cls2DataTd_m
        {
            /// <summary>
            /// 区段1累计时间
            /// </summary>
            public long? Zone1Time { get; set; }
            /// <summary>
            /// 区段2累计时间
            /// </summary>
            public long? Zone2Time { get; set; }
            /// <summary>
            /// 区段3累计时间
            /// </summary>
            public long? Zone3Time { get; set; }

            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                Zone1Time = Byte2Long(Data, index, 2);
                index += 2;
                Zone2Time = Byte2Long(Data, index, 2);
                index += 2;
                Zone2Time = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 8; }
            }
        }
        /// <summary>
        /// F49：终端日供电时间、日复位累计次数
        /// </summary>
        public class AFN0D_F049 : Cls2DataTd_d
        {
            /// <summary>
            /// 供电时间。单位：min
            /// </summary>
            public long? SuportTime { get; set; }
            /// <summary>
            /// 复位次数。单位：次
            /// </summary>
            public long? ResetTimes { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                SuportTime = Byte2Long(Data, index, 2);
                index += 2;
                ResetTimes = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 7; }
            }
        }
        /// <summary>
        /// F50：终端日控制统计数据
        /// </summary>
        public class AFN0D_F050 : Cls2DataTd_d
        {
            /// <summary>
            /// 月电控跳闸次数
            /// </summary>
            public long? MonthJumpTimes { get; set; }
            /// <summary>
            /// 购电控跳闸次数
            /// </summary>
            public long? BuyJumpTimes { get; set; }
            /// <summary>
            /// 功控跳闸次数
            /// </summary>
            public long? PowerJumpTimes { get; set; }
            /// <summary>
            /// 遥控跳闸次数
            /// </summary>
            public long? RemoteJumpTimes { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                MonthJumpTimes = Data[index++];
                BuyJumpTimes = Data[index++];
                PowerJumpTimes = Data[index++];
                RemoteJumpTimes = Data[index++];
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 7; }
            }
        }
        /// <summary>
        /// F51：终端月供电时间、月复位累计次数
        /// </summary>
        public class AFN0D_F051 : Cls2DataTd_m
        {
            /// <summary>
            /// 供电时间。单位：min
            /// </summary>
            public long? SuportTime { get; set; }
            /// <summary>
            /// 复位次数。单位：次
            /// </summary>
            public long? ResetTimes { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                SuportTime = Byte2Long(Data, index, 2);
                index += 2;
                ResetTimes = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 6; }
            }
        }
        /// <summary>
        /// F52：终端月控制统计数据
        /// </summary>
        public class AFN0D_F052 : Cls2DataTd_m
        {
            /// <summary>
            /// 月电控跳闸次数
            /// </summary>
            public long? MonthJumpTimes { get; set; }
            /// <summary>
            /// 购电控跳闸次数
            /// </summary>
            public long? BuyJumpTimes { get; set; }
            /// <summary>
            /// 功控跳闸次数
            /// </summary>
            public long? PowerJumpTimes { get; set; }
            /// <summary>
            /// 遥控跳闸次数
            /// </summary>
            public long? RemoteJumpTimes { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                MonthJumpTimes = Data[index++];
                BuyJumpTimes = Data[index++];
                PowerJumpTimes = Data[index++];
                RemoteJumpTimes = Data[index++];
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 6; }
            }
        }
        /// <summary>
        /// F57：总加组日最大、最小有功功率及其发生时间，有功功率为零日累计时间
        /// </summary>
        public class AFN0D_F057 : Cls2DataTd_d
        {
            /// <summary>
            /// 最大功率
            /// </summary>
            public TimeValue MaxPower { get; set; }
            /// <summary>
            /// 最小功率
            /// </summary>
            public TimeValue MinPower { get; set; }
            /// <summary>
            /// 功率为零时间
            /// </summary>
            public long? ZeroPowerTime { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                double? value;
                Bcd2Dec_Format02(Data, index, out value);
                MaxPower.Value = value;
                index += (int)FormatLenOption.Format02;
                Bcd2Dec_Format18(Data, index, out dt);
                MaxPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format02(Data, index, out value);
                MinPower.Value = value;
                index += (int)FormatLenOption.Format02;
                Bcd2Dec_Format18(Data, index, out dt);
                MinPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                ZeroPowerTime = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 15; }
            }
        }
        /// <summary>
        /// F58：总加组日累计有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F058 : Cls2DataTd_d
        {
            public EfficacyEnergyValue EnergyValue = new EfficacyEnergyValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                bool bSucc = base.Encode(isSet, MaxRespLen);
                RespLen = 3 + 1 + 4 * DataFixLen;
                return bSucc;
            }
            public override bool Decode()
            {
                int index = 0;
                EnergyValue.Attr = EfficacyOptions.ForwardActivePower;
                DateTime dt;
                if (!Bcd2Dec_Format20(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format20;
                int count = Data[index++];
                for (int i = 0; i < count + 1; i++)
                {
                    long? value;
                    Bcd2Dec_Format03(Data, index, out value);
                    EnergyValue.Value[i] = (double?)value;
                    index += (int)FormatLenOption.Format03;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F59：总加组日累计无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F059 : AFN0D_F058
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ForwardReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F60：总加组月最大、最小有功功率及其发生时间，有功功率为零月累计时间
        /// </summary>
        public class AFN0D_F060 : Cls2DataTd_m
        {
            /// <summary>
            /// 最大功率
            /// </summary>
            public TimeValue MaxPower { get; set; }
            /// <summary>
            /// 最小功率
            /// </summary>
            public TimeValue MinPower { get; set; }
            /// <summary>
            /// 功率为零时间
            /// </summary>
            public long? ZeroPowerTime { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                double? value;
                Bcd2Dec_Format02(Data, index, out value);
                MaxPower.Value = value;
                index += (int)FormatLenOption.Format02;
                Bcd2Dec_Format18(Data, index, out dt);
                MaxPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                Bcd2Dec_Format02(Data, index, out value);
                MinPower.Value = value;
                index += (int)FormatLenOption.Format02;
                Bcd2Dec_Format18(Data, index, out dt);
                MinPower.Time = dt;
                index += (int)FormatLenOption.Format18;
                ZeroPowerTime = Byte2Long(Data, index, 2);
                index += 2;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 14; }
            }
        }
        /// <summary>
        /// F61：总加组月累计有功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F061 : Cls2DataTd_m
        {
            public EfficacyEnergyValue EnergyValue = new EfficacyEnergyValue();
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                bool bSucc = base.Encode(isSet, MaxRespLen);
                RespLen = 3 + 1 + 4 * DataFixLen;
                return bSucc;
            }
            public override bool Decode()
            {
                int index = 0;
                EnergyValue.Attr = EfficacyOptions.ForwardActivePower;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                int count = Data[index++];
                for (int i = 0; i < count + 1; i++)
                {
                    long? value;
                    Bcd2Dec_Format03(Data, index, out value);
                    EnergyValue.Value[i] = (double?)value;
                    index += (int)FormatLenOption.Format03;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F62：总加组月累计无功电能量（总、费率1~M）
        /// </summary>
        public class AFN0D_F062 : AFN0D_F061
        {
            public override bool Decode()
            {
                bool bSucc = base.Decode();
                EnergyValue.Attr = EfficacyOptions.ForwardReactivePower;
                return bSucc;
            }
        }
        /// <summary>
        /// F65：总加组超功率定值的月累计时间、月累计电能量
        /// </summary>
        public class AFN0D_F065 : Cls2DataTd_m
        {
            /// <summary>
            /// 超定值时间
            /// </summary>
            public long? SurpassFixTime { get; set; }
            /// <summary>
            /// 超定值电量
            /// </summary>
            public double? SurpassFixEnergy { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format21(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format21;
                SurpassFixTime = Byte2Long(Data, index, 2);
                index += 2;
                long? value;
                Bcd2Dec_Format03(Data, index, out value);
                SurpassFixEnergy = (double?)value;
                index += (int)FormatLenOption.Format03;
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 8; }
            }
        }
        /// <summary>
        /// F66：总加组超月电能量定值的月累计时间、累计电能量
        /// </summary>
        public class AFN0D_F066 : AFN0D_F065
        {

        }
        /// <summary>
        /// F73：总加组有功功率曲线
        /// </summary>
        public class AFN0D_F073 : Cls2DataTd_c
        {
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if(!Bcd2Dec_Format15(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format15;
                Denisty = (DenistyOption)Data[index++];
                DataCount = Data[index++];
                int minDiff = Denisty.GetIntervalMin();       //分钟间隔
                CurvePointOptions point = Denisty.GetCurvePointOption();    //数据点数
                if (curve == null)
                {
                    curve = new CurveValue(point);
                }
                
                for (int i = 0; i < DataCount; i++)
                {
                    double? value;
                    Bcd2Dec_Format02(Data, index, out value);
                    index += (int)FormatLenOption.Format02;
                    DateTime time = DataTime.AddMinutes(i * minDiff);
                    int timeNum = time.Hour * 10000 + time.Minute * 100;
                    curve.SetValue(timeNum, (decimal?)value);
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F74：总加组无功功率曲线
        /// </summary>
        public class AFN0D_F074 : AFN0D_F073
        {

        }
        /// <summary>
        /// F75：总加组有功电能量曲线
        /// </summary>
        public class AFN0D_F075 : Cls2DataTd_c
        {
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if(!Bcd2Dec_Format15(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format15;
                Denisty = (DenistyOption)Data[index++];
                DataCount = Data[index++];
                int minDiff = Denisty.GetIntervalMin();       //分钟间隔
                CurvePointOptions point = Denisty.GetCurvePointOption();    //数据点数

                if (curve == null)
                {
                    curve = new CurveValue(point);
                }
                
                for (int i = 0; i < DataCount; i++)
                {
                    long? value;
                    Bcd2Dec_Format03(Data, index, out value);
                    index += (int)FormatLenOption.Format03;
                    DateTime time = DataTime.AddMinutes(i * minDiff);
                    int timeNum = time.Hour * 10000 + time.Minute * 100;
                    curve.SetValue(timeNum, (decimal?)value);
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F76：总加组无功电能量曲线
        /// </summary>
        public class AFN0D_F076 : AFN0D_F075
        {

        }
        /// <summary>
        /// F81：有功功率曲线
        /// </summary>
        public class AFN0D_F081 : Cls2DataTd_c
        {
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format15(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format15;
                Denisty = (DenistyOption)Data[index++];
                DataCount = Data[index++];
                int minDiff = Denisty.GetIntervalMin();       //分钟间隔
                CurvePointOptions point = Denisty.GetCurvePointOption();    //数据点数

                if (curve == null)
                {
                    curve = new CurveValue(point);
                }

                for (int i = 0; i < DataCount; i++)
                {
                    double? value;
                    Bcd2Dec_Format09(Data, index, out value);
                    index += (int)FormatLenOption.Format09;
                    DateTime time = DataTime.AddMinutes(i * minDiff);
                    int timeNum = time.Hour * 10000 + time.Minute * 100;
                    curve.SetValue(timeNum, (decimal?)value);
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 3; }
            }
        }
        /// <summary>
        /// F82：A相有功功率曲线
        /// </summary>
        public class AFN0D_F082 : AFN0D_F081
        {

        }
        /// <summary>
        /// F83：B相有功功率曲线
        /// </summary>
        public class AFN0D_F083 : AFN0D_F081
        {

        }
        /// <summary>
        /// F84：C相有功功率曲线
        /// </summary>
        public class AFN0D_F084 : AFN0D_F081
        {

        }
        /// <summary>
        /// F85：无功功率曲线
        /// </summary>
        public class AFN0D_F085 : AFN0D_F081
        {

        }
        /// <summary>
        /// F86：A相无功功率曲线
        /// </summary>
        public class AFN0D_F086 : AFN0D_F081
        {

        }
        /// <summary>
        /// F87：B相无功功率曲线
        /// </summary>
        public class AFN0D_F087 : AFN0D_F081
        {

        }
        /// <summary>
        /// F88：C相无功功率曲线
        /// </summary>
        public class AFN0D_F088 : AFN0D_F081
        {

        }
        /// <summary>
        /// F89：A相电压曲线
        /// </summary>
        public class AFN0D_F089 : Cls2DataTd_c
        {
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format15(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format15;
                Denisty = (DenistyOption)Data[index++];
                DataCount = Data[index++];
                int minDiff = Denisty.GetIntervalMin();       //分钟间隔
                CurvePointOptions point = Denisty.GetCurvePointOption();    //数据点数

                if (curve == null)
                {
                    curve = new CurveValue(point);
                }

                for (int i = 0; i < DataCount; i++)
                {
                    double? value;
                    Bcd2Dec_Format07(Data, index, out value);
                    index += (int)FormatLenOption.Format07;
                    DateTime time = DataTime.AddMinutes(i * minDiff);
                    int timeNum = time.Hour * 10000 + time.Minute * 100;
                    curve.SetValue(timeNum, (decimal?)value);
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F90：B相电压曲线
        /// </summary>
        public class AFN0D_F090 : AFN0D_F089
        {

        }
        /// <summary>
        /// F91：C相电压曲线
        /// </summary>
        public class AFN0D_F091 : AFN0D_F089
        {

        }
        /// <summary>
        /// F92：A相电流曲线
        /// </summary>
        public class AFN0D_F092 : Cls2DataTd_c
        {
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format15(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format15;
                Denisty = (DenistyOption)Data[index++];
                DataCount = Data[index++];
                int minDiff = Denisty.GetIntervalMin();       //分钟间隔
                CurvePointOptions point = Denisty.GetCurvePointOption();    //数据点数

                if (curve == null)
                {
                    curve = new CurveValue(point);
                }

                for (int i = 0; i < DataCount; i++)
                {
                    double? value;
                    Bcd2Dec_Format06(Data, index, out value);
                    index += (int)FormatLenOption.Format06;
                    DateTime time = DataTime.AddMinutes(i * minDiff);
                    int timeNum = time.Hour * 10000 + time.Minute * 100;
                    curve.SetValue(timeNum, (decimal?)value);
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F93：B相电流曲线
        /// </summary>
        public class AFN0D_F093 : AFN0D_F092
        {

        }
        /// <summary>
        /// F94：C相电流曲线
        /// </summary>
        public class AFN0D_F094 : AFN0D_F092
        {

        }
        /// <summary>
        /// F95：零序电流曲线
        /// </summary>
        public class AFN0D_F095 : AFN0D_F092
        {

        }
        /// <summary>
        /// F97：正向有功总电能量曲线
        /// </summary>
        public class AFN0D_F097 : Cls2DataTd_c
        {
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format15(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format15;
                Denisty = (DenistyOption)Data[index++];
                DataCount = Data[index++];
                int minDiff = Denisty.GetIntervalMin();       //分钟间隔
                CurvePointOptions point = Denisty.GetCurvePointOption();    //数据点数

                if (curve == null)
                {
                    curve = new CurveValue(point);
                }

                for (int i = 0; i < DataCount; i++)
                {
                    double? value;
                    Bcd2Dec_Format13(Data, index, out value);
                    index += (int)FormatLenOption.Format13;
                    DateTime time = DataTime.AddMinutes(i * minDiff);
                    int timeNum = time.Hour * 10000 + time.Minute * 100;
                    curve.SetValue(timeNum, (decimal?)value);
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F98：正向无功总电能量曲线
        /// </summary>
        public class AFN0D_F098 : AFN0D_F097
        {

        }
        /// <summary>
        /// F99：反向有功总电能量曲线
        /// </summary>
        public class AFN0D_F099 : AFN0D_F097
        {

        }
        /// <summary>
        /// F100：反向无功总电能量曲线
        /// </summary>
        public class AFN0D_F100 : AFN0D_F097
        {

        }
        /// <summary>
        /// F101：正向有功总电能示值曲线
        /// </summary>
        public class AFN0D_F101 : Cls2DataTd_c
        {
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format15(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format15;
                Denisty = (DenistyOption)Data[index++];
                DataCount = Data[index++];
                int minDiff = Denisty.GetIntervalMin();       //分钟间隔
                CurvePointOptions point = Denisty.GetCurvePointOption();    //数据点数

                if (curve == null)
                {
                    curve = new CurveValue(point);
                }

                for (int i = 0; i < DataCount; i++)
                {
                    double? value;
                    Bcd2Dec_Format11(Data, index, out value);
                    index += (int)FormatLenOption.Format11;
                    DateTime time = DataTime.AddMinutes(i * minDiff);
                    int timeNum = time.Hour * 10000 + time.Minute * 100;
                    curve.SetValue(timeNum, (decimal?)value);
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 4; }
            }
        }
        /// <summary>
        /// F102：正向无功总电能示值曲线
        /// </summary>
        public class AFN0D_F102 : AFN0D_F101
        {

        }
        /// <summary>
        /// F103：反向有功总电能示值曲线
        /// </summary>
        public class AFN0D_F103 : AFN0D_F101
        {

        }
        /// <summary>
        /// F104：反向无功总电能示值曲线
        /// </summary>
        public class AFN0D_F104 : AFN0D_F101
        {

        }
        /// <summary>
        /// F105：总功率因数曲线
        /// </summary>
        public class AFN0D_F105 : Cls2DataTd_c
        {
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (!Bcd2Dec_Format15(Data, index, out dt))
                {
                    OperResult = OperResultOption.InvalidData;
                    OperResultDesc = "回码数据无效";
                    return false;
                }
                DataTime = dt;
                index += (int)FormatLenOption.Format15;
                Denisty = (DenistyOption)Data[index++];
                DataCount = Data[index++];
                int minDiff = Denisty.GetIntervalMin();       //分钟间隔
                CurvePointOptions point = Denisty.GetCurvePointOption();    //数据点数

                if (curve == null)
                {
                    curve = new CurveValue(point);
                }

                for (int i = 0; i < DataCount; i++)
                {
                    double? value;
                    Bcd2Dec_Format05(Data, index, out value);
                    index += (int)FormatLenOption.Format05;
                    DateTime time = DataTime.AddMinutes(i * minDiff);
                    int timeNum = time.Hour * 10000 + time.Minute * 100;
                    curve.SetValue(timeNum, (decimal?)value);
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override int DataFixLen
            {
                get { return 2; }
            }
        }
        /// <summary>
        /// F106：A相功率因数曲线
        /// </summary>
        public class AFN0D_F106 : AFN0D_F105
        {

        }
        /// <summary>
        /// F107：B相功率因数曲线
        /// </summary>
        public class AFN0D_F107 : AFN0D_F105
        {

        }
        /// <summary>
        /// F108：C相功率因数曲线
        /// </summary>
        public class AFN0D_F108 : AFN0D_F105
        {

        }
        /// <summary>
        /// F113：A相2~19次谐波电流日最大值及发生时间
        /// </summary>
        public class AFN0D_F113 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F114：B相2~19次谐波电流日最大值及发生时间
        /// </summary>
        public class AFN0D_F114 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F115：C相2~19次谐波电流日最大值及发生时间
        /// </summary>
        public class AFN0D_F115 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F116：A相2~19次谐波电压含有率及总畸变率日最大值及发生时间
        /// </summary>
        public class AFN0D_F116 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F117：B相2~19次谐波电压含有率及总畸变率日最大值及发生时间
        /// </summary>
        public class AFN0D_F117 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F118：C相2~19次谐波电压含有率及总畸变率日最大值及发生时间
        /// </summary>
        public class AFN0D_F118 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F121：A相谐波越限日统计数据
        /// </summary>
        public class AFN0D_F121 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F122：B相谐波越限日统计数据
        /// </summary>
        public class AFN0D_F122 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F123：C相谐波越限日统计数据
        /// </summary>
        public class AFN0D_F123 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F129：直流模拟量越限日累计时间、最大/最小值及发生时间
        /// </summary>
        public class AFN0D_F129 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F130：直流模拟量越限月累计时间、最大/最小值及发生时间
        /// </summary>
        public class AFN0D_F130 : Cls2DataTd_d
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }
        /// <summary>
        /// F138：直流模拟量数据曲线
        /// </summary>
        public class AFN0D_F138 : Cls2DataTd_c
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }

            public override int DataFixLen
            {
                get { throw new NotImplementedException(); }
            }
        }

        #endregion
        #region 3类数据项定义
        #region 事件定义
        /*
         * 事件定义放在3类数据类外定义说明：
         * 如果事件在3类数据Cls3DataBase类中定义，其他规约就不方便重新定义事件了
         */
        /// <summary>
        /// 事件基类
        /// 事件子类命名规则为ERCXX，XX为事件编号。如ERC01
        /// </summary>
        public abstract class ERCBase
        {
            /// <summary>
            /// 事件编号
            /// 由程序根据类名自动获取
            /// </summary>
            public int Id 
            { 
                get
                {
                    string str = this.GetType().ToString();
                    return Convert.ToInt32(str.Substring(str.LastIndexOf('C') + 1));
                }
            }
            /// <summary>
            /// 事件指针。该事件在终端的存储位置
            /// </summary>
            public int Pointer { get; set; }
            /// <summary>
            /// 事件发生时间
            /// </summary>
            public DateTime OccurTime {get;set;}
            /// <summary>
            /// 事件解码，需要解码的数据在Data中
            /// </summary>
            /// <returns></returns>
            public abstract bool Decode();
            /// <summary>
            /// 事件规约格式数据
            /// </summary>
            public byte[] Data { get; set; }
        }
        /// <summary>
        /// ERC1：数据初始化和版本变更记录
        /// </summary>
        public class ERC01 : ERCBase
        {
            /// <summary>
            /// 初始化
            /// </summary>
            public bool IsReset { get; set; }
            /// <summary>
            /// 版本变更
            /// </summary>
            public bool IsVerChanged { get; set; }
            /// <summary>
            /// 变更前版本
            /// </summary>
            public string VerBefore { get; set; }
            /// <summary>
            /// 变更后版本
            /// </summary>
            public string VerAfter { get; set; }
            public override bool  Decode()
            {
 	            int index = 0;
                DateTime dt;
                if(Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                if ((Data[index] & 0x01) > 0)
                {
                    IsReset = true;
                }
                if ((Data[index] & 0x02) > 0)
                {
                    IsVerChanged = true;
                }
                index++;
                VerBefore = System.Text.Encoding.ASCII.GetString(Data, index, 4);
                index += 4;
                VerAfter = System.Text.Encoding.ASCII.GetString(Data, index, 4);
                index += 4;
                return true;
            }
        }
        /// <summary>
        /// ERC2：参数丢失记录
        /// </summary>
        public class ERC02 : ERCBase
        {
            /// <summary>
            /// 终端参数丢失
            /// </summary>
            public bool IsRtuParamLoss { get; set; }
            /// <summary>
            /// 测量点参数丢失
            /// </summary>
            public bool IsMpParamLoss { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                if ((Data[index] & 0x01) > 0)
                {
                    IsRtuParamLoss = true;
                }
                if ((Data[index] & 0x02) > 0)
                {
                    IsMpParamLoss = true;
                }
                index++;
                return true;
            }
        }
        /// <summary>
        /// ERC3：参数变更记录
        /// </summary>
        public class ERC03 : ERCBase
        {
            public int MSA { get; set; }
            public override bool Decode()
            {
                return false;
            }
        }
        /// <summary>
        /// ERC4：状态量变位记录
        /// </summary>
        public class ERC04 : ERCBase
        {
            /// <summary>
            /// 变位标志
            /// </summary>
            public bool[] ChangeFlag = new bool[8];
            /// <summary>
            /// 变位状态
            /// </summary>
            public bool[] ChangeStatus = new bool[8];
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                for (int i = 0; i < 8; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                        ChangeFlag[i] = true;
                    if (((Data[index+1] >> i) & 0x01) > 0)
                        ChangeStatus[i] = true;
                }
                return true;
            }
        }
        /// <summary>
        /// ERC5：遥控跳闸记录
        /// </summary>
        public class ERC05 : ERCBase
        {
            private List<int> turnIdList = new List<int>();
            /// <summary>
            /// 跳闸轮次
            /// </summary>
            public List<int> TurnIdList { get { return turnIdList; } }
            /// <summary>
            /// 跳闸时功率
            /// </summary>
            public double? JumpingPower { get; set; }
            /// <summary>
            /// 跳闸后2分钟功率
            /// </summary>
            public double? Jumped2MinPower { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                for (int i = 0; i < 8; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                        turnIdList.Add(i + 1);
                }
                index++;
                double? tmp;
                if (!Bcd2Dec_Format02(Data, index, out tmp))
                {
                    return false;
                }
                JumpingPower = tmp;
                index += (int)FormatLenOption.Format02;
                if (!Bcd2Dec_Format02(Data, index, out tmp))
                {
                    return false;
                }
                Jumped2MinPower = tmp;
                index += (int)FormatLenOption.Format02;
                return true;
            }
        }
        /// <summary>
        /// ERC6：功控跳闸记录
        /// </summary>
        public class ERC06 : ERCBase
        {
            /// <summary>
            /// 总加组号
            /// </summary>
            public int TotalId { get; set; }
            private List<int> turnIdList = new List<int>();
            /// <summary>
            /// 跳闸轮次
            /// </summary>
            public List<int> TurnIdList { get { return turnIdList; } }
            /// <summary>
            /// 时段控
            /// </summary>
            public bool IsPeriodCtrl { get; set; }
            /// <summary>
            /// 厂休控
            /// </summary>
            public bool IsRestCtrl { get; set; }
            /// <summary>
            /// 报停控
            /// </summary>
            public bool IsStopCtrl { get; set; }
            /// <summary>
            /// 下浮控
            /// </summary>
            public bool IsFluCtrl { get; set; }
            /// <summary>
            /// 跳闸时功率
            /// </summary>
            public double? JumpingPower { get; set; }
            /// <summary>
            /// 跳闸后2分钟功率
            /// </summary>
            public double? Jumped2MinPower { get; set; }
            /// <summary>
            /// 功率定值
            /// </summary>
            public double? PowerFixValue { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                TotalId = (Data[index++] & 0x3F);
                for (int i = 0; i < 8; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                        turnIdList.Add(i + 1);
                }
                index++;
                for (int i = 0; i < 8; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                    {
                        switch(i)
                        {
                            case 0: IsPeriodCtrl = true;break;
                            case 1: IsRestCtrl = true;  break;
                            case 2: IsStopCtrl = true; break;
                            case 3: IsFluCtrl = true; break;
                        }
                    }
                }
                index++;
                double? tmp;
                if (!Bcd2Dec_Format02(Data, index, out tmp))
                {
                    return false;
                }
                JumpingPower = tmp;
                index += (int)FormatLenOption.Format02;
                if (!Bcd2Dec_Format02(Data, index, out tmp))
                {
                    return false;
                }
                Jumped2MinPower = tmp;
                index += (int)FormatLenOption.Format02;
                if (!Bcd2Dec_Format02(Data, index, out tmp))
                {
                    return false;
                }
                PowerFixValue = tmp;
                index += (int)FormatLenOption.Format02;
                return true;
            }
        }
        /// <summary>
        /// ERC7：电控跳闸记录
        /// 电控类别	跳闸时的电能量	跳闸时电能量定值
        ///  月电控	    月电能量	    月电控定值
        ///  购电控	    剩余电能量/费	购电控跳闸门限
        /// </summary>
        public class ERC07 : ERCBase
        {
            /// <summary>
            /// 总加组号
            /// </summary>
            public int TotalId { get; set; }
            private List<int> turnIdList = new List<int>();
            /// <summary>
            /// 跳闸轮次
            /// </summary>
            public List<int> TurnIdList { get { return turnIdList; } }
            /// <summary>
            /// 月电控
            /// </summary>
            public bool IsMonCtrl { get; set; }
            /// <summary>
            /// 购电控
            /// </summary>
            public bool IsBuyCtrl { get; set; }
            /// <summary>
            /// 跳闸时电量
            /// </summary>
            public long? JumpingEnergy { get; set; }
            /// <summary>
            /// 跳闸时电量定值
            /// </summary>
            public long? EnergyFixValue { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                TotalId = (Data[index++] & 0x3F);
                for (int i = 0; i < 8; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                        turnIdList.Add(i + 1);
                }
                index++;
                for (int i = 0; i < 8; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                    {
                        switch (i)
                        {
                            case 0: IsMonCtrl = true; break;
                            case 1: IsBuyCtrl = true; break;
                        }
                    }
                }
                index++;
                long? tmp;
                if (!Bcd2Dec_Format03(Data, index, out tmp))
                {
                    return false;
                }
                JumpingEnergy = tmp;
                index += (int)FormatLenOption.Format03;
                if (!Bcd2Dec_Format03(Data, index, out tmp))
                {
                    return false;
                }
                EnergyFixValue = tmp;
                index += (int)FormatLenOption.Format03;
                return true;
            }
        }
        /// <summary>
        /// ERC8：电能表参数变更
        /// </summary>
        public class ERC08 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 电能表费率时段变化
            /// </summary>
            public bool IsPeriodChanged { get; set; }
            /// <summary>
            /// 电能表编程时间更改
            /// </summary>
            public bool IsProgramTimeChanged { get; set; }
            /// <summary>
            /// 电能表抄表日更改
            /// </summary>
            public bool IsReadDayChanged { get; set; }
            /// <summary>
            /// 电能表脉冲常数更改
            /// </summary>
            public bool IsKChanged { get; set; }
            /// <summary>
            /// 电能表的互感器倍率更改
            /// </summary>
            public bool IsCtPtChanged { get; set; }
            /// <summary>
            /// 电能表最大需量清零
            /// </summary>
            public bool IsDemandClear { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index++] & 0x3F);
                for (int i = 0; i < 8; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                    {
                        switch (i)
                        {
                            case 0: IsPeriodChanged = true; break;
                            case 1: IsProgramTimeChanged = true; break;
                            case 2: IsReadDayChanged = true; break;
                            case 3: IsKChanged = true; break;
                            case 4: IsCtPtChanged = true; break;
                            case 5: IsDemandClear = true; break;
                        }
                    }
                }
                index++;
                return true;
            }
        }
        /// <summary>
        /// ERC9：电流回路异常
        /// </summary>
        public class ERC09 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 起止标志 true-开始 false-结束
            /// </summary>
            public bool IsStart { get; set; }
            /// <summary>
            /// 异常相序。多个相序组合
            /// </summary>
            public int Phase { get; set; }
            /// <summary>
            /// 异常类型。1-短路 2-开路 3-反向
            /// </summary>
            public int Type { get; set; }
            /// <summary>
            /// A相电压
            /// </summary>
            public double? Ua { get; set; }
            /// <summary>
            /// B相电压
            /// </summary>
            public double? Ub { get; set; }
            /// <summary>
            /// C相电压
            /// </summary>
            public double? Uc { get; set; }
            /// <summary>
            /// A相电流
            /// </summary>
            public double? Ia { get; set; }
            /// <summary>
            /// B相电流
            /// </summary>
            public double? Ib { get; set; }
            /// <summary>
            /// C相电流
            /// </summary>
            public double? Ic { get; set; }
            /// <summary>
            /// 正向有功电能示值
            /// </summary>
            public double? ShowValue { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index] & 0x3F);
                IsStart = ((Data[index] >> 7) & 0x01) > 0;
                index++;
                for (int i = 0; i < 3; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                    {
                        Phase |= (1 >> (i + 1));
                    }
                }
                Type = ((Data[index] >> 6) & 0x3);
                index++;
                double? tmp;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Ua = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Ub = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Uc = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ia = tmp;
                index += (int)FormatLenOption.Format06;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ib = tmp;
                index += (int)FormatLenOption.Format06;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ic = tmp;
                index += (int)FormatLenOption.Format06;
                double? value;
                if (!Bcd2Dec_Format14(Data, index, out value))
                {
                    return false;
                }
                ShowValue = (double)value;
                index += (int)FormatLenOption.Format14;
                return true;
            }
        }
        /// <summary>
        /// ERC10：电压回路异常
        /// </summary>
        public class ERC10 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 起止标志 true-开始 false-结束
            /// </summary>
            public bool IsStart { get; set; }
            /// <summary>
            /// 异常相序。多个相序组合
            /// </summary>
            public int Phase { get; set; }
            /// <summary>
            /// 异常类型。1-断相 2-失压
            /// </summary>
            public int Type { get; set; }
            /// <summary>
            /// A相电压
            /// </summary>
            public double? Ua { get; set; }
            /// <summary>
            /// B相电压
            /// </summary>
            public double? Ub { get; set; }
            /// <summary>
            /// C相电压
            /// </summary>
            public double? Uc { get; set; }
            /// <summary>
            /// A相电流
            /// </summary>
            public double? Ia { get; set; }
            /// <summary>
            /// B相电流
            /// </summary>
            public double? Ib { get; set; }
            /// <summary>
            /// C相电流
            /// </summary>
            public double? Ic { get; set; }
            /// <summary>
            /// 正向有功电能示值
            /// </summary>
            public double? ShowValue { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index] & 0x3F);
                IsStart = ((Data[index] >> 7) & 0x01) > 0;
                index++;
                for (int i = 0; i < 3; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                    {
                        Phase |= (1 >> (i + 1));
                    }
                }
                Type = ((Data[index] >> 6) & 0x3);
                index++;
                double? tmp;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Ua = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Ub = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Uc = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ia = tmp;
                index += (int)FormatLenOption.Format06;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ib = tmp;
                index += (int)FormatLenOption.Format06;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ic = tmp;
                index += (int)FormatLenOption.Format06;
                double? value;
                if (!Bcd2Dec_Format14(Data, index, out value))
                {
                    return false;
                }
                ShowValue = (double)value;
                index += (int)FormatLenOption.Format14;
                return true;
            }
        }
        /// <summary>
        /// ERC11：相序异常
        /// </summary>
        public class ERC11 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 起止标志 true-开始 false-结束
            /// </summary>
            public bool IsStart { get; set; }
            /// <summary>
            /// A相电压相位角
            /// </summary>
            public double? Ua { get; set; }
            /// <summary>
            /// B相电压相位角
            /// </summary>
            public double? Ub { get; set; }
            /// <summary>
            /// C相电压相位角
            /// </summary>
            public double? Uc { get; set; }
            /// <summary>
            /// A相电流相位角
            /// </summary>
            public double? Ia { get; set; }
            /// <summary>
            /// B相电流相位角
            /// </summary>
            public double? Ib { get; set; }
            /// <summary>
            /// C相电流相位角
            /// </summary>
            public double? Ic { get; set; }
            /// <summary>
            /// 正向有功电能示值
            /// </summary>
            public double? ShowValue { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index] & 0x3F);
                IsStart = ((Data[index] >> 7) & 0x01) > 0;
                index++;
                double? tmp;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    return false;
                }
                Ua = tmp;
                index += (int)FormatLenOption.Format05;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    return false;
                }
                Ub = tmp;
                index += (int)FormatLenOption.Format05;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    return false;
                }
                Uc = tmp;
                index += (int)FormatLenOption.Format05;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    return false;
                }
                Ia = tmp;
                index += (int)FormatLenOption.Format05;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    return false;
                }
                Ib = tmp;
                index += (int)FormatLenOption.Format05;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    return false;
                }
                Ic = tmp;
                index += (int)FormatLenOption.Format05;
                double? value;
                if (!Bcd2Dec_Format14(Data, index, out value))
                {
                    return false;
                }
                ShowValue = (double)value;
                index += (int)FormatLenOption.Format14;
                return true;
            }
        }
        /// <summary>
        /// ERC12：电能表时间超差
        /// </summary>
        public class ERC12 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 起止标志 true-开始 false-结束
            /// </summary>
            public bool IsStart { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index] & 0x3F);
                IsStart = ((Data[index] >> 7) & 0x01) > 0;
                index++;
                return true;
            }
        }
        /// <summary>
        /// ERC13：电表故障信息
        /// </summary>
        public class ERC13 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 电能表编程次数或最大需量清零次数发生变化
            /// </summary>
            public bool IsProgramOrDemandClear { get; set; }
            /// <summary>
            /// 电能表断相次数变化
            /// </summary>
            public bool IsBreakTimes { get; set; }
            /// <summary>
            /// 电能表失压次数变化
            /// </summary>
            public bool IsLossTimes { get; set; }
            /// <summary>
            /// 电能表停电次数变化
            /// </summary>
            public bool IsNopowerTimes { get; set; }
            /// <summary>
            /// 电能表电池欠压
            /// </summary>
            public bool IsBatteryLack { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index++] & 0x3F);
                for (int i = 0; i < 8; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                    {
                        switch (i)
                        {
                            case 0: IsProgramOrDemandClear = true; break;
                            case 1: IsBreakTimes = true; break;
                            case 2: IsLossTimes = true; break;
                            case 3: IsNopowerTimes = true; break;
                            case 4: IsBatteryLack = true; break;
                        }
                    }
                }
                index++;
                return true;
            }
        }
        /// <summary>
        /// ERC14：终端停/上电事件
        /// </summary>
        public class ERC14 : ERCBase
        {
            /// <summary>
            /// 停电结束时间
            /// </summary>
            public DateTime EndTime { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    EndTime = dt;
                index += (int)FormatLenOption.Format15;
                return true;
            }
        }
        /// <summary>
        /// ERC15：谐波越限告警
        /// </summary>
        public class ERC15 : ERCBase
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// ERC16：直流模拟量越限记录
        /// </summary>
        public class ERC16 : ERCBase
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// ERC17：电压/电流不平衡度越限记录
        /// </summary>
        public class ERC17 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 起止标志 true-开始 false-结束
            /// </summary>
            public bool IsStart { get; set; }
            /// <summary>
            /// 电压不平衡
            /// </summary>
            public bool IsVoltage { get; set; }
            /// <summary>
            /// 电流不平衡
            /// </summary>
            public bool IsCurrent { get; set; }
            /// <summary>
            /// 电压不平衡度。单位：%
            /// </summary>
            public double? VolUnblance { get; set; }
            /// <summary>
            /// 电流不平衡度。单位：%
            /// </summary>
            public double? CurUnblance { get; set; }
            /// <summary>
            /// A相电压
            /// </summary>
            public double? Ua { get; set; }
            /// <summary>
            /// B相电压
            /// </summary>
            public double? Ub { get; set; }
            /// <summary>
            /// C相电压
            /// </summary>
            public double? Uc { get; set; }
            /// <summary>
            /// A相电流
            /// </summary>
            public double? Ia { get; set; }
            /// <summary>
            /// B相电流
            /// </summary>
            public double? Ib { get; set; }
            /// <summary>
            /// C相电流
            /// </summary>
            public double? Ic { get; set; }

            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index] & 0x3F);
                IsStart = ((Data[index] >> 7) & 0x01) > 0;
                index++;
                if ((Data[index] & 0x01) > 0)
                {
                    IsVoltage = true;
                }
                if (((Data[index] >> 1) & 0x01) > 0)
                {
                    IsCurrent = true; ;
                }
                index++;
                double? tmp;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    return false;
                }
                VolUnblance = tmp;
                index += (int)FormatLenOption.Format05;
                if (!Bcd2Dec_Format05(Data, index, out tmp))
                {
                    return false;
                }
                CurUnblance = tmp;
                index += (int)FormatLenOption.Format05;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Ua = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Ub = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Uc = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ia = tmp;
                index += (int)FormatLenOption.Format06;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ib = tmp;
                index += (int)FormatLenOption.Format06;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ic = tmp;
                index += (int)FormatLenOption.Format06;
                return true;
            }
        }
        /// <summary>
        /// ERC18：电容器投切自锁记录
        /// </summary>
        public class ERC18 : ERCBase
        {

            public override bool Decode()
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// ERC19：购电参数设置记录
        /// </summary>
        public class ERC19 : ERCBase
        {
            /// <summary>
            /// 购电单号
            /// </summary>
            public long BuyId { get; set; }
            /// <summary>
            /// 追加刷新标志
            /// </summary>
            public bool IsRefresh { get; set; }
            /// <summary>
            /// 购电定值
            /// </summary>
            public long? BuyValue { get; set; }
            /// <summary>
            /// 告警定值
            /// </summary>
            public long? AlermValue { get; set; }
            /// <summary>
            /// 跳闸定值
            /// </summary>
            public long? JumpValue { get; set; }
            /// <summary>
            /// 购电前剩余电量
            /// </summary>
            public long? BeforeRemain { get; set; }
            /// <summary>
            /// 购电后剩余电量
            /// </summary>
            public long? AfterRemain { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                for (int i = 0; i < 4; i++)
                {
                    BuyId = BuyId * 256 + Data[index + 3 - i];
                }
                index += 4;
                IsRefresh = Data[index++] == 0xAA;
                long? tmp;
                if (!Bcd2Dec_Format03(Data, index, out tmp))
                {
                    return false;
                }
                BuyValue = tmp;
                index += (int)FormatLenOption.Format03;
                if (!Bcd2Dec_Format03(Data, index, out tmp))
                {
                    return false;
                }
                AlermValue = tmp;
                index += (int)FormatLenOption.Format03;
                if (!Bcd2Dec_Format03(Data, index, out tmp))
                {
                    return false;
                }
                JumpValue = tmp;
                index += (int)FormatLenOption.Format03;
                if (!Bcd2Dec_Format03(Data, index, out tmp))
                {
                    return false;
                }
                BeforeRemain = tmp;
                index += (int)FormatLenOption.Format03;
                if (!Bcd2Dec_Format03(Data, index, out tmp))
                {
                    return false;
                }
                AfterRemain = tmp;
                index += (int)FormatLenOption.Format03;
                return true;
            }
        }
        /// <summary>
        /// ERC20：密码错误记录
        /// </summary>
        public class ERC20 : ERCBase
        {
            /// <summary>
            /// 错误密码
            /// </summary>
            public int ErrKey { get; set; }
            /// <summary>
            /// 启动站地址
            /// </summary>
            public int MSA { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                ErrKey = Data[index] + Data[index + 1] * 256;
                index += 2;
                MSA = Data[index++];
                return true;
            }
        }
        /// <summary>
        /// ERC21：终端故障记录
        /// </summary>
        public class ERC21 : ERCBase
        {
            /// <summary>
            /// 故障编码 
            /// 1-终端主板内存故障 2-时钟故障 3-主板通信故障 4-485抄表故障 5-显示板故障 
            /// </summary>
            public int FaultNo { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                FaultNo = Data[index];
                return true;
            }
        }
        /// <summary>
        /// ERC22：有功总电能量差动越限事件记录
        /// </summary>
        public class ERC22 : ERCBase
        {
            /// <summary>
            /// 差动组号
            /// </summary>
            public int DiffId { get; set; }
            /// <summary>
            /// 起止标志
            /// </summary>
            public bool IsStart { get; set; }
            /// <summary>
            /// 对比总加组电量
            /// </summary>
            public long? ContrastEnergy { get; set; }
            /// <summary>
            /// 参照总加组电量
            /// </summary>
            public long? ReferEnergy { get; set; }
            /// <summary>
            /// 相对变差
            /// </summary>
            public int RelativeDiff { get; set; }
            /// <summary>
            /// 绝对偏差
            /// </summary>
            public long? AbsolutDiff { get; set; }
            /// <summary>
            /// 对比总加组测量点正向有功电能示值
            /// </summary>
            private List<double> contrastShowValue = new List<double>();
            public List<double> ContrastShowValue { get { return contrastShowValue; } }
            /// <summary>
            /// 参照总加组测量点正向有功电能示值
            /// </summary>
            private List<double> referShowValue = new List<double>();
            public List<double> ReferShowValue { get { return referShowValue; } }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                DiffId = (Data[index] & 0x3F);
                IsStart = ((Data[index] >> 7) & 0x01) > 0;
                index++;
                long? tmp;
                if (Bcd2Dec_Format03(Data, index, out tmp))
                {
                    ContrastEnergy = tmp;
                }
                index += (int)FormatLenOption.Format03;
                if (Bcd2Dec_Format03(Data, index, out tmp))
                {
                    ReferEnergy = tmp;
                }
                index += (int)FormatLenOption.Format03;
                RelativeDiff = Data[index++];
                if (Bcd2Dec_Format03(Data, index, out tmp))
                {
                    AbsolutDiff = tmp;
                }
                index += (int)FormatLenOption.Format03;
                int count = Data[index++];
                for (int i = 0; i < count; i++)
                {
                    double? value;
                    if (Bcd2Dec_Format14(Data, index, out value))
                    {
                        contrastShowValue.Add((double)value);
                    }
                    index += (int)FormatLenOption.Format14;
                }
                count = Data[index++];
                for (int i = 0; i < count; i++)
                {
                    double? value;
                    if (Bcd2Dec_Format14(Data, index, out value))
                    {
                        referShowValue.Add((double)value);
                    }
                    index += (int)FormatLenOption.Format14;
                }
                return true;
            }
        }
        /// <summary>
        /// ERC24：电压越限记录
        /// </summary>
        public class ERC24 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 起止标志 true-开始 false-结束
            /// </summary>
            public bool IsStart { get; set; }
            /// <summary>
            /// 异常相序。多个相序组合
            /// </summary>
            public int Phase { get; set; }
            /// <summary>
            /// 越限标志 1-越上上限 2-越下下限
            /// </summary>
            public int Flag { get; set; }
            /// <summary>
            /// A相电压
            /// </summary>
            public double? Ua { get; set; }
            /// <summary>
            /// B相电压
            /// </summary>
            public double? Ub { get; set; }
            /// <summary>
            /// C相电压
            /// </summary>
            public double? Uc { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index] & 0x3F);
                IsStart = ((Data[index] >> 7) & 0x01) > 0;
                index++;
                for (int i = 0; i < 3; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                    {
                        Phase |= (1 >> (i + 1));
                    }
                }
                Flag = ((Data[index] >> 6) & 0x3);
                index++;
                double? tmp;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Ua = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Ub = tmp;
                index += (int)FormatLenOption.Format07;
                if (!Bcd2Dec_Format07(Data, index, out tmp))
                {
                    return false;
                }
                Uc = tmp;
                index += (int)FormatLenOption.Format07;
                return true;
            }
        }
        /// <summary>
        /// ERC25：电流越限记录
        /// </summary>
        public class ERC25 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 起止标志 true-开始 false-结束
            /// </summary>
            public bool IsStart { get; set; }
            /// <summary>
            /// 异常相序。多个相序组合
            /// </summary>
            public int Phase { get; set; }
            /// <summary>
            /// 越限标志 1-越上上限 2-越上限
            /// </summary>
            public int Flag { get; set; }
            /// <summary>
            /// A相电流
            /// </summary>
            public double? Ia { get; set; }
            /// <summary>
            /// B相电流
            /// </summary>
            public double? Ib { get; set; }
            /// <summary>
            /// C相电流
            /// </summary>
            public double? Ic { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index] & 0x3F);
                IsStart = ((Data[index] >> 7) & 0x01) > 0;
                index++;
                for (int i = 0; i < 3; i++)
                {
                    if (((Data[index] >> i) & 0x01) > 0)
                    {
                        Phase |= (1 >> (i + 1));
                    }
                }
                Flag = ((Data[index] >> 6) & 0x3);
                index++;
                double? tmp;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ia = tmp;
                index += (int)FormatLenOption.Format06;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ib = tmp;
                index += (int)FormatLenOption.Format06;
                if (!Bcd2Dec_Format06(Data, index, out tmp))
                {
                    return false;
                }
                Ic = tmp;
                index += (int)FormatLenOption.Format06;
                return true;
            }
        }
        /// <summary>
        /// ERC26：视在功率越限记录
        /// </summary>
        public class ERC26 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 起止标志 true-开始 false-结束
            /// </summary>
            public bool IsStart { get; set; }
            /// <summary>
            /// 越限标志 1-越上上限 2-越上限
            /// </summary>
            public int Flag { get; set; }
            /// <summary>
            /// 视在功率。单位：kVA
            /// </summary>
            public double? AppPower { get; set; }
            /// <summary>
            /// 视在功率限值。单位：kVA
            /// </summary>
            public double? AppPowerLimitValue { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index] & 0x3F);
                IsStart = ((Data[index] >> 7) & 0x01) > 0;
                index++;
                Flag = ((Data[index] >> 6) & 0x3);
                index++;
                double? tmp;
                if (Bcd2Dec_Format23(Data, index, out tmp))
                {
                    AppPower = tmp;
                }
                index += (int)FormatLenOption.Format23;
                if (Bcd2Dec_Format23(Data, index, out tmp))
                {
                    AppPowerLimitValue = tmp;
                }
                index += (int)FormatLenOption.Format23;
                return false;
            }
        }
        /// <summary>
        /// ERC27：电能表示度下降记录
        /// </summary>
        public class ERC27 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 异常前示值
            /// </summary>
            public double BeforeShowValue { get; set; }
            /// <summary>
            /// 异常后示值
            /// </summary>
            public double AfterShowValue { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index++] & 0x3F);
                double? tmp;
                if (Bcd2Dec_Format14(Data, index, out tmp))
                {
                    BeforeShowValue = (double)tmp;
                }
                index += (int)FormatLenOption.Format14;
                if (Bcd2Dec_Format14(Data, index, out tmp))
                {
                    AfterShowValue = (double)tmp;
                }
                index += (int)FormatLenOption.Format14;
                return true;
            }
        }
        /// <summary>
        /// ERC28：电能量超差记录
        /// </summary>
        public class ERC28 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 异常前示值
            /// </summary>
            public double BeforeShowValue { get; set; }
            /// <summary>
            /// 异常后示值
            /// </summary>
            public double AfterShowValue { get; set; }
            /// <summary>
            /// 阈值
            /// </summary>
            public int Threshold { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index++] & 0x3F);
                double? tmp;
                if (Bcd2Dec_Format14(Data, index, out tmp))
                {
                    BeforeShowValue = (double)tmp;
                }
                index += (int)FormatLenOption.Format14;
                if (Bcd2Dec_Format14(Data, index, out tmp))
                {
                    AfterShowValue = (double)tmp;
                }
                index += (int)FormatLenOption.Format14;
                Threshold = Data[index++];
                return true;
            }
        }
        /// <summary>
        /// ERC29：电能表飞走记录
        /// </summary>
        public class ERC29 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 异常前示值
            /// </summary>
            public double BeforeShowValue { get; set; }
            /// <summary>
            /// 异常后示值
            /// </summary>
            public double AfterShowValue { get; set; }
            /// <summary>
            /// 阈值
            /// </summary>
            public int Threshold { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index++] & 0x3F);
                double? tmp;
                if (Bcd2Dec_Format14(Data, index, out tmp))
                {
                    BeforeShowValue = (double)tmp;
                }
                index += (int)FormatLenOption.Format14;
                if (Bcd2Dec_Format14(Data, index, out tmp))
                {
                    AfterShowValue = (double)tmp;
                }
                index += (int)FormatLenOption.Format14;
                Threshold = Data[index++];
                return true;
            }
        }
        /// <summary>
        /// ERC30：电能表停走记录
        /// </summary>
        public class ERC30 : ERCBase
        {
            /// <summary>
            /// 测量点号
            /// </summary>
            public int MpId { get; set; }
            /// <summary>
            /// 停走时示值
            /// </summary>
            public double StopShowValue { get; set; }
            /// <summary>
            /// 阈值
            /// </summary>
            public int Threshold { get; set; }
            public override bool Decode()
            {
                int index = 0;
                DateTime dt;
                if (Bcd2Dec_Format15(Data, index, out dt))
                    OccurTime = dt;
                index += (int)FormatLenOption.Format15;
                MpId = (Data[index++] & 0x3F);
                double? tmp;
                if (Bcd2Dec_Format14(Data, index, out tmp))
                {
                    StopShowValue = (double)tmp;
                }
                index += (int)FormatLenOption.Format14;
                Threshold = Data[index++];
                return true;
            }
        }
        #endregion //事件定义
        public abstract class Cls3DataBase : DataUnitBase
        {
            /// <summary>
            /// 重要事件计数器ERC1
            /// </summary>
            public int ImpCounter { get; set; }
            /// <summary>
            /// 一般事件计数器ERC2
            /// </summary>
            public int CommCounter { get; set; }
            /// <summary>
            /// 起始指针。召测时需要指定
            /// </summary>
            public int StartPointer { get; set; }
            /// <summary>
            /// 结束指针。召测时需要指定
            /// </summary>
            public int EndPointer { get; set; }
            /// <summary>
            /// 事件列表。
            /// </summary>
            private List<ERCBase> ercList = new List<ERCBase>();
            public List<ERCBase> ErcList
            {
                get{ return ercList; }
            }
            public void AddErc(ERCBase erc)
            {
                ercList.Add(erc);
            }
            /// <summary>
            /// 构造函数
            /// </summary>
            public Cls3DataBase()
            {
                IsCombined = false;
            }
            /// <summary>
            /// 根据事件ID获取事件
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public ERCBase CreateERC(int id)
            {
                string name = this.GetType().ToString(); ;
                Type ERCType = Type.GetType(name.Substring(0, name.LastIndexOf('+')) + "+ERC" + id.ToString("D2"));
                if (ERCType != null)
                {
                    return (ERCBase)Activator.CreateInstance(ERCType);
                }
                return null;
            }
            public override bool Encode(bool isSet, int MaxRespLen)
            {
                Data = new byte[2];
                int EventNumPerFrame = (MaxRespLen-26)/49; //单帧事件个数 49为ERC15的长度（最长的一个事件）
                int pointDiff = 0;
                if(StartPointer<EndPointer)
                {
                    pointDiff = EndPointer - StartPointer;
                }
                else if(StartPointer>EndPointer)
                {
                    pointDiff = 256 + EndPointer - StartPointer;
                }
                if (pointDiff > EventNumPerFrame)
                {
                    double tmp = pointDiff * 1.0 / EventNumPerFrame;
                    int nFrame = (tmp - (int)tmp) > 0 ? ((int)tmp + 1) : (int)tmp;
                    for (int i = 1; i < nFrame; i++)
                    {
                        Cls3DataBase unit = (Cls3DataBase)this.MemberwiseClone();
                        unit.StartPointer = (StartPointer + i * EventNumPerFrame) % 256;
                        int end = (StartPointer + (i + 1) * EventNumPerFrame) % 256;
                        unit.EndPointer = end > EndPointer ? EndPointer : end;
                        unit.Encode(isSet, MaxRespLen);
                        if (SubDataUnitList == null)
                        {
                            SubDataUnitList = new List<DataUnitBase>();
                        }
                        SubDataUnitList.Add(unit);
                    }
                    EndPointer = (StartPointer + EventNumPerFrame) % 256;
                    SubRelation = 1;
                }
                Data[0] = (byte)StartPointer;
                Data[1] = (byte)EndPointer;
                return true;
            }
            public override bool Decode()
            {
                int index = 0;
                ImpCounter = Data[index++];
                CommCounter = Data[index++];
                StartPointer = Data[index++];
                EndPointer = Data[index++];
                while (index < Data.Length)
                {
                    int ercId = Data[index++];
                    int ercLen = Data[index++];
                    if (index + ercLen > Data.Length)
                    {
                        OperResult = OperResultOption.InvalidData;
                        OperResultDesc = "解码失败，返回帧长度错误。";
                        return false;
                    }
                    ERCBase erc = CreateERC(ercId);
                    if (erc != null)
                    {
                        byte[] tmp = new byte[ercLen];
                        System.Array.ConstrainedCopy(Data, index, tmp, 0, ercLen);
                        erc.Data = tmp;
                        //事件解码
                        erc.Decode();
                        ercList.Add(erc);
                    }
                    index += ercLen;
                }
                OperResult = OperResultOption.Success;
                return true;
            }

            public override bool Merge(DataUnitBase unit)
            {
                ercList.AddRange(((Cls3DataBase)unit).ercList);
                return base.Merge(unit);
            }
            public override int DataFixLen
            {
                get { return 220; }
            }
        }
        /// <summary>
        /// F1：请求重要事件
        /// </summary>
        public class AFN0E_F001 : Cls3DataBase
        {

        }
        /// <summary>
        /// F2：请求一般事件
        /// </summary>
        public class AFN0E_F002 : Cls3DataBase
        {

        }
        #endregion//3类数据项定义
        #endregion//规约数据项定义

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
            fixLen = dataUnit.DataFixLen;
            switch (AFN)
            {
                case AFNOption.AFN_PARAM_QUERY:
                    {
                        switch (dataUnit.Fn)
                        {
                            case 2: //终端中继
                                {
                                    fixLen = 1 + (data[start] & 0x7F) * 2;
                                }
                                break;
                            case 10: //终端电能表/交流采样装置配置参数
                            case 11: //终端脉冲配置参数
                            case 13: //终端电压/电流模拟量配置参数
                            case 15: //有功总电能量差动越限事件参数设置
                            case 27: //测量点数据冻结参数
                                {
                                    fixLen = 1 + data[start] * dataUnit.DataFixLen;
                                }
                                break;
                            case 14: //终端总加组配置参数         
                                {
                                    int dataLen = 0;
                                    int tcount = data[start + dataLen++];
                                    for (int i = 0; i < tcount; i++)
                                    {
                                        dataLen++;  //总加组
                                        int mcount = data[start + dataLen++];   //测量点个数
                                        dataLen += mcount;  //测量点
                                    }
                                    fixLen = dataLen;
                                }
                                break;
                            case 41: //时段功控定值
                                {
                                    int dataLen = 0;
                                    dataLen++;
                                    for (int i = 0; i < 3; i++)
                                    {
                                        if (((data[start] >> i) & 0x01) > 0)
                                        {
                                            int tmp = dataLen++;
                                            for (int j = 0; j < 8; j++)
                                            {
                                                if (((data[start + tmp] >> j) & 0x01) > 0)
                                                {
                                                    dataLen += 2;
                                                }
                                            }

                                        }
                                    }
                                    fixLen = dataLen;
                                }
                                break;
                            case 65://定时发送1类数据任务设置
                            case 66://定时发送2类数据任务设置
                                {
                                    fixLen = 9 + data[start + 8] * 4;
                                }
                                break;
                        }
                    }
                    break;
                case AFNOption.AFN_CLS_ONE_QUERY:
                    {
                        switch (dataUnit.Fn)
                        {
                            case 5: //F5：终端控制设置状态
                                {
                                    fixLen = 2;
                                    for (int i = 0; i < 8; i++)
                                    {
                                        if (((data[start + 1] >> i) & 0x01) > 0)
                                        {
                                            fixLen += dataUnit.DataFixLen;
                                        }
                                    }
                                }
                                break;
                            case 6: //F6：终端当前控制状态
                                {
                                    fixLen = 3;
                                    for (int i = 0; i < 8; i++)
                                    {
                                        if (((data[start + 2] >> i) & 0x01) > 0)
                                        {
                                            fixLen += dataUnit.DataFixLen;
                                        }
                                    }
                                }
                                break;
                            case 19: //当日总加有功电能量（总、费率1~M）
                            case 20: //当日总加无功电能量（总、费率1~M）
                            case 21: //当月总加有功电能量（总、费率1~M）
                            case 22: //当月总加无功电能量（总、费率1~M）
                            case 41: //当日正向有功电能量（总、费率1~M）
                            case 42: //当日正向无功电能量（总、费率1~M）
                            case 43: //当日反向有功电能量（总、费率1~M）
                            case 44: //当日反向无功电能量（总、费率1~M）
                            case 45: //当月正向有功电能量（总、费率1~M）
                            case 46: //当月正向无功电能量（总、费率1~M）
                            case 47: //当月反向有功电能量（总、费率1~M）
                            case 48: //当月反向无功电能量（总、费率1~M）

                                {
                                    fixLen = data[start] * dataUnit.DataFixLen + 1;
                                }
                                break;
                            case 33: //当前正向有/无功电能示值、一/四象限无功电能示值（总、费率1~M）
                            case 34: //当前反向有/无功电能示值、二/三象限无功电能示值（总、费率1~M）
                            case 37: //上月正向有/无功电能示值、一/四象限无功电能示值（总、费率1~M）
                            case 38: //上月反向有/无功电能示值、二/三象限无功电能示值（总、费率1~M）
                                {
                                    int num = data[start + 5];
                                    fixLen = 5 + 1 + (num + 1) * (5 + 4 + 4 + 4);
                                }
                                break;
                            case 35: //当月正向有/无功最大需量及发生时间（总、费率1~M）
                            case 36: //当月反向有/无功最大需量及发生时间（总、费率1~M）
                            case 39: //上月正向有/无功最大需量及发生时间（总、费率1~M）
                            case 40: //上月反向有/无功最大需量及发生时间（总、费率1~M）
                                {
                                    int num = data[start + 5];
                                    fixLen = 5 + 1 + (num + 1) * (3 + 4 + 3 + 4);
                                }
                                break;
                        }
                    }
                    break;
                case AFNOption.AFN_CLS_TWO_QUERY:
                    {
                        switch (dataUnit.Fn)
                        {
                            case 1: //日冻结正向有/无功电能示值、一/四象限无功电能示值（总、费率1~M）
                            case 2: //日冻结反向有/无功电能示值、二/三象限无功电能示值（总、费率1~M）
                            case 9: //抄表日正向有/无功电能示值、一/四象限无功电能示值（总、费率1~M）
                            case 10: //抄表日反向有/无功电能示值、二/三象限无功电能示值（总、费率1~M）
                                {
                                    int num = data[start + 8];
                                    fixLen = 8 + 1 + (num + 1) * (5 + 4 + 4 + 4);
                                }
                                break;
                            case 3: //日冻结正向有/无功最大需量及发生时间（总、费率1~M）
                            case 4: //日冻结反向有/无功最大需量及发生时间（总、费率1~M）
                            case 11: //抄表日正向有/无功最大需量及发生时间（总、费率1~M）
                            case 12: //抄表日反向有/无功最大需量及发生时间（总、费率1~M）
                                {
                                    int num = data[start + 8];
                                    fixLen = 8 + 1 + (num + 1) * (3 + 4 + 3 + 4);
                                }
                                break;
                            case 5: //正向有功电能量（总、费率1~M）
                            case 6: //正向无功电能量（总、费率1~M）
                            case 7: //反向有功电能量（总、费率1~M）
                            case 8: //反向无功电能量（总、费率1~M）
                                {
                                    fixLen = 3 + 1 + (data[start + 3] + 1) * dataUnit.DataFixLen;
                                }
                                break;
                            case 17: //月正向有/无功电能示值、一/四象限无功电能示值（总、费率1~M）
                            case 18: //月反向有/无功电能示值、二/三象限无功电能示值（总、费率1~M）
                                {
                                    int num = data[start + 7];
                                    fixLen = 7 + 1 + (num + 1) * (5 + 4 + 4 + 4);
                                }
                                break;
                            case 19: //正向有/无功最大需量及发生时间（总、费率1~M）
                            case 20: //反向有/无功最大需量及发生时间（总、费率1~M）
                                {
                                    int num = data[start + 7];
                                    fixLen = 7 + 1 + (num + 1) * (3 + 4 + 3 + 4);
                                }
                                break;
                            case 21: //月正向有功电能量（总、费率1~M）
                            case 22: //月正向无功电能量（总、费率1~M）
                            case 23: //月反向有功电能量（总、费率1~M）
                            case 24: //月反向无功电能量（总、费率1~M）
                                {
                                    fixLen = 2 + 1 + (data[start + 2] + 1) * dataUnit.DataFixLen;
                                }
                                break;
                            case 58: //日冻结总加组日累计有功电能量（总、费率1~M）
                            case 59: //日冻结总加组日累计无功电能量（总、费率1~M）
                                {
                                    fixLen = 3 + 1 + (data[start + 3] + 1) * dataUnit.DataFixLen;
                                }
                                break;
                            case 61: //月冻结总加组月有功电能量（总、费率1~M）
                            case 62: //月冻结总加组月无功电能量（总、费率1~M）
                                {
                                    fixLen = 2 + 1 + (data[start + 2] + 1) * dataUnit.DataFixLen;
                                }
                                break;
                            case 73: //总加组有功功率曲线
                            case 74: //总加组无功功率曲线
                            case 75: //总加组有功电能量曲线
                            case 76: //总加组无功电能量曲线
                            case 81: //有功功率曲线
                            case 82: //A相有功功率曲线
                            case 83: //B相有功功率曲线
                            case 84: //C相有功功率曲线
                            case 85: //无功功率曲线
                            case 86: //A相无功功率曲线
                            case 87: //B相无功功率曲线
                            case 88: //C相无功功率曲线
                            case 89: //A相电压曲线
                            case 90: //B相电压曲线
                            case 91: //C相电压曲线
                            case 92: //A相电流曲线
                            case 93: //B相电流曲线
                            case 94: //C相电流曲线
                            case 95: //零序电流曲线
                            case 97: //正向有功总电能量曲线
                            case 98: //正向无功总电能量曲线
                            case 99: //反向有功总电能量曲线
                            case 100: //反向无功总电能量曲线
                            case 101: //正向有功总电能示值曲线
                            case 102: //正向无功总电能示值曲线
                            case 103: //反向有功总电能示值曲线
                            case 104: //反向无功总电能示值曲线
                            case 105: //总功率因数曲线
                            case 106: //A相功率因数曲线
                            case 107: //B相功率因数曲线
                            case 108: //C相功率因数曲线
                            case 138: //直流模拟量数据曲线
                                {
                                    fixLen = 7 + data[start + 6] * dataUnit.DataFixLen;
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
