using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.IO.Ports;

namespace AC.Base.Drives.GDW130_376
{
    /*
     * 国网GDW130、GDW376.1通信规约抽象基类，主要完成规约框架的处理，实现组帧、解帧以及阻塞和非阻塞的通信控制逻辑
     * 继承子类主要实现具体F项的组、解规约以及自己的特殊处理
     * 对外主要提供的功能主要有：1、阻塞式通信 2、非阻塞式通信 3、主动上报解帧处理
     * 一、阻塞式通信
     *    1、接口函数SetDataUnit()、GetDataUnit()
     *    2、这两个函数调用后直接返回数据项的处理结果
     * 二、非阻塞通信
     *    1、接口函数MakeTask()、FrameRecv()、TaskResultReport事件
     *    2、MaskTask()主要是下达通信任务，FrameRecv()是由通道处理完终端一帧通信后调用（不管终端有没有回码都要调用）
     *       TaskResultReport事件由调用MaskTask()的对象进行监听，当终端通信任务处理完毕后会触发该事件。
     * 三、主动上报
     *    1、接口函数Uncompose()
     *    2、可重入函数，输入终端响应的规约帧，返回解帧成功的数据项列表
     * 注意：
     *    1、日数据一个数据项只召测1天的数据
     *    2、通道返回的帧就是该驱动对应的终端的返回帧
     *    3、使用前预先给行政区码、终端地址、地址类型、最大响应帧长度、最大数据单元个数等赋值
     * 作者：高书明
     * 日期：2011-11-7
     */
    public abstract class GDWDriveBase : DriveBase, IIntAddressDriveImplement, ICallClockDriveImplement
    {
        #region 事件定义
        /// <summary>
        /// 任务处理结果上报-非阻塞处理
        /// </summary>
        /// <param name="TaskId"></param>
        /// <param name="dataUnitList"></param>
        public delegate void TaskResultReportHandler(long TaskId, List<DataUnitBase> dataUnitList);
        public event TaskResultReportHandler TaskResultReport;
        #endregion//事件定义

        #region 公共枚举、函数定义
        /// <summary>
        /// AFN定义
        /// </summary>
        protected enum AFNOption
        {
            AFN_YESNO = 0x00,	            //确认/否认
            AFN_RESET = 0x01,	            //复位
            AFN_LINKTEST = 0x02,	        //链路测试
            AFN_RELAY_COMMAND = 0x03,	    //中继站命令
            AFN_PARAM_SET = 0x04,	        //设置参数
            AFN_CONTROL = 0x05,	            //控制命令
            AFN_AUTHENT = 0x06,             //身份认证
            AFN_NEXT_AUTOREPORT = 0x08,     //请求被级联终端主动上报
            AFN_CONFIG = 0x09,	            //终端配置
            AFN_PARAM_QUERY = 0x0A,	        //查询参数
            AFN_TASKDATA_QUERY = 0x0B,	    //任务数据查询
            AFN_CLS_ONE_QUERY = 0x0C,	    //请求1类数据（实时数据）
            AFN_CLS_TWO_QUERY = 0x0D,	    //请求2类数据（历史数据）
            AFN_CLS_THREE_QUERY = 0x0E,	    //请求3类数据（事件数据）
            AFN_FILE_TRANSFER = 0x0F,	    //文件传输
            AFN_DATA_TRANSMIT = 0x10,	    //数据转发
        }
        /// <summary>
        /// 功能码定义
        /// </summary>
        protected enum FuncOption
        {
            //启动站功能码定义
            MASTER_FUNC_RESET = 1,		    //复位
            MASTER_FUNC_USERDATA = 4,		//用户数据
            MASTER_FUNC_LINKTEST = 9,		//链路测试
            MASTER_FUNC_LEVEL1 = 10,		//请求1级数据
            MASTER_FUNC_LEVEL2 = 11,		//请求2级数据

            //从动站功能码定义
            SLAVE_FUNC_YESNO = 0,		    //确认/否认
            SLAVE_FUNC_USERDATA = 8,		//用户数据
            SLAVE_FUNC_NO = 9,		        //否认
            SLAVE_FUNC_LINKSTATUS = 11,		//链路状态
        }
        /// <summary>
        /// 地址类型
        /// </summary>
        public enum AddrTypeOption
        {
            UNICAST = 0,                    //单终端
            GROUPCAST = 1,                  //组播
            BROADCAST = 2,                  //广播
        }
        /// <summary>
        /// 上行帧类型
        /// </summary>
        public enum UpFrameTypeOption
        {
            UNKONWN = 0,                    //未知
            ACK = 2,                        //确认
            DENY = 3,                       //否认
            DATA = 4,                       //数据
            REPORT_NEED = 5,                //主动上报_需要确认
            REPORT_NO = 6,                  //主动上报_不需要确认
        }
        /// <summary>
        /// 数据项处理结果
        /// </summary>
        public enum OperResultOption
        {
            None = 0,                       //未处理
            Success = 1,                    //成功
            NoAnswer = 2,                   //无回码
            Deny = 3,                       //否认
            InvalidData = 4,                //回码数据无效
            InvalidValue = 5,               //设置值无效
            Other = 6,                      //其他
        }
        #region 数据格式转换
        /// <summary>
        /// 是否BCD码
        /// </summary>
        /// <param name="data">待判断的数据</param>
        /// <returns>是BCD码返回true</returns>
        protected static bool IsBcd(byte data)
        {
            if ((data & 0x0F) > 10)
                return false;
            if (((data & 0xF0) >> 4) > 10)
                return false;
            return true;
        }        /// <summary>
        /// 单字节将BCD码转换为十进制数字
        /// </summary>
        /// <param name="data">需要转换的BCD码</param>
        /// <returns></returns>
        protected static byte Bcd2Dec(byte data)
        {
            byte value = (byte)((data & 0x0F) + ((data & 0xF0) >> 4) * 10);
            return value;
        }
        protected static byte Dec2Bcd(byte data)
        {
            byte tmp = 0;
            tmp = (byte)(((data / 10) << 4) + (data % 10));
            return tmp;
        }
        /// <summary>
        /// 将十进制整数转换为指定字节长度的BCD码
        /// </summary>
        /// <param name="value">需要转换的十进制整数</param>
        /// <param name="data">转换后的BCD码字节数组（外部已经初始化）</param>
        /// <param name="start">data存放BCD码的起始位置（输入参数）</param>
        /// <param name="len">需要转换成的字节数组长度（输入参数）</param>
        /// <returns></returns>
        protected static bool Dec2Bcd(long value, byte[] data, int start, int len/*in*/)
        {
            int i = 0;
            while (value > 0 && i < len)
            {
                for (int j = 0; j < 2; j++)
                {
                    long tmp = value % 10;
                    data[start + i] |= (byte)((tmp) << (j * 4));
                    value /= 10;
                }
                i++;
            }
            return true;
        }
        /// <summary>
        /// 将字节转换为整型数值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long? Byte2Long(byte[] data, int start, int length)
        {
            long? value = 0;
            bool bValid = false;
            for (int i = 0; i < length; i++)
            {
                if (data[start + i] != 0xEE)
                {
                    bValid = true;
                    break;
                }
            }
            if (!bValid)
            {
                return null;
            }
            for (int i = 0; i < length; i++)
            {
                value = value * 256 + data[start + length - 1];
            }
            return value;
        }
        /// <summary>
        /// 数据格式长度
        /// </summary>
        protected enum FormatLenOption
        {
            Format01 = 6,
            Format02 = 2,
            Format03 = 4,
            Format04 = 1,
            Format05 = 2,
            Format06 = 2,
            Format07 = 2,
            Format08 = 2,
            Format09 = 3,
            Format10 = 3,
            Format11 = 4,
            Format12 = 6,
            Format13 = 4,
            Format14 = 5,
            Format15 = 5,
            Format16 = 4,
            Format17 = 4,
            Format18 = 3,
            Format19 = 2,
            Format20 = 3,
            Format21 = 2,
            Format22 = 1,
            Format23 = 3,
        }
        /// <summary>
        /// 数据格式01解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format01(byte[] data, int start, out DateTime value)
        {
            value = new DateTime();
            for (int i = 0; i < (int)FormatLenOption.Format01; i++)
            {
                if (i != 4)
                {
                    if (!IsBcd(data[start + i]))
                        return false;
                }
                else
                {
                    if (!IsBcd((byte)(data[start + i] & 0x0F)))
                    {
                        return false;
                    }
                }
            }
            string str;
            str = (Bcd2Dec(data[start + 5]) + 2000) + "-" + Bcd2Dec((byte)(data[start + 4] & 0x1F)) + "-" + Bcd2Dec(data[start + 3]);
            str += " " + Bcd2Dec(data[start + 2]) + ":" + Bcd2Dec(data[start + 1]) + ":" + Bcd2Dec(data[start]);
            value = Convert.ToDateTime(str);
            return true;
        }
        /// <summary>
        /// 数据格式01编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format01(DateTime value, byte[] data, int start)
        {
            int index = start;
            data[index++] = Dec2Bcd((byte)value.Second);
            data[index++] = Dec2Bcd((byte)value.Minute);
            data[index++] = Dec2Bcd((byte)value.Hour);
            data[index++] = Dec2Bcd((byte)value.Day);
            data[index++] = (byte)(Dec2Bcd((byte)value.Month) | (((int)value.DayOfWeek) << 5));
            data[index++] = Dec2Bcd((byte)(value.Year % 100));
            return true;
        }
        /// <summary>
        /// 数据格式02解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format02(byte[] data, int start, out double? value)
        {
            value = 0;
            if (!IsBcd(data[start]) || !IsBcd((byte)(data[start + 1] & 0x0F)))
            {
                value = null;
                return false;
            }
            value = Bcd2Dec(data[start]) + Bcd2Dec((byte)(data[start + 1] & 0x0F)) * 100;
            switch (data[start + 1] >> 5)
            {
                case 0: value *= 10000; break;
                case 1: value *= 1000; break;
                case 2: value *= 100; break;
                case 3: value *= 10; break;
                case 4: value *= 1; break;
                case 5: value *= 0.1; break;
                case 6: value *= 0.01; break;
                case 7: value *= 0.001; break;
            }
            if ((data[start + 1] & 0x10) > 0)
            {
                value *= -1;
            }
            return true;
        }
        /// <summary>
        /// 数据格式02编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format02(double value, byte[] data, int start)
        {
            if (value < -9990000 || value > 9990000)
                return false;
            int index = start;
            if (value < 0)
            {
                value *= -1;
                data[start + 1] = 0x10;
            }
            if (value < 0.001)
                return false;
            int multiple = 0;
            while (value > 999)
            {
                value /= 10;
                multiple++;
            }
            if (multiple > 4)
                return false;
            while (value < 100)
            {
                value *= 10;
                multiple--;
            }
            while (multiple < -3)
            {
                value /= 10;
                multiple++;
            }
            data[start] = Dec2Bcd((byte)(value % 100));
            data[start + 1] |= Dec2Bcd((byte)(value / 100));
            return true;
        }
        /// <summary>
        /// 数据格式03解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format03(byte[] data, int start, out long? value)
        {
            value = 0;
            for (int i = 0; i < (int)FormatLenOption.Format03; i++)
            {
                if (!IsBcd(data[start + i]))
                {
                    value = null;
                    return false;
                }
            }
            value = Bcd2Dec(data[start]) + Bcd2Dec(data[start + 1]) * 100 + Bcd2Dec(data[start + 2]) * 10000 + Bcd2Dec((byte)(data[start + 3] & 0x0F)) * 1000000;
            if ((data[start + 3] & 0x4) > 0)
            {
                value *= 1000;
            }
            if ((data[start + 1] & 0x10) > 0)
            {
                value *= -1;
            }
            return true;
        }
        /// <summary>
        /// 数据格式03编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format03(long value, byte[] data, int start)
        {
            int index = start;
            if (value < 0)
            {
                value *= -1;
                data[start + 4] = 0x10;
            }
            if (value > 9999999)
            {
                value /= 1000;
                data[start + 4] |= 0x40;
            }
            for (int i = 0; i < 4; i++)
            {
                long tmp = value % 100;
                value /= 100;
                data[start + i] = Dec2Bcd((byte)tmp);
            }
            return true;
        }
        /// <summary>
        /// 数据格式04解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format04(byte[] data, int start, out short value)
        {
            value = 0;
            if (!IsBcd((byte)(data[start] & 0x7F)))
                return false;
            value = Bcd2Dec((byte)(data[start] & 0x7F));
            if ((data[start] & 0x80) > 0)
            {
                value *= -1;
            }
            return true;
        }
        /// <summary>
        /// 数据格式04编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format04(short value, byte[] data, int start)
        {

            return true;
        }
        /// <summary>
        /// 数据格式05解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format05(byte[] data, int start, out double? value)
        {
            value = 0;
            if (!IsBcd(data[start]) || !IsBcd((byte)(data[start + 1] & 0x7F)))
            {
                value = null;
                return false;
            }
            value = Bcd2Dec(data[start]) / 10.0 + Bcd2Dec((byte)(data[start + 1] & 0x7F)) * 10;
            if ((data[start + 1] & 0x80) > 0)
            {
                value *= -1;
            }
            return true;
        }
        /// <summary>
        /// 数据格式05编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format05(double value, byte[] data, int start)
        {
            if (value < -799.9 || value > 799.9)
                return false;
            if (value < 0)
            {
                data[start + 1] = 0x80;
                value *= -1;
            }
            int tmp = (int)(value * 10);
            data[start] = Dec2Bcd((byte)(tmp % 100));
            data[start + 1] = Dec2Bcd((byte)(tmp / 100));
            return true;
        }
        /// <summary>
        /// 数据格式06解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format06(byte[] data, int start, out double? value)
        {
            value = 0;
            if (!IsBcd(data[start]) || !IsBcd((byte)(data[start + 1] & 0x7F)))
            {
                value = null;
                return false;
            }
            value = Bcd2Dec(data[start]) / 100.0 + Bcd2Dec((byte)(data[start + 1] & 0x7F));
            if ((data[start + 1] & 0x80) > 0)
            {
                value *= -1;
            }
            return true;
        }
        /// <summary>
        /// 数据格式06编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format06(double value, byte[] data, int start)
        {
            if (value < -79.99 || value > 79.99)
                return false;
            if (value < 0)
            {
                value *= -1;
                data[start + 1] = 0x80;
            }
            int tmp = (int)(value * 100);
            data[start] = Dec2Bcd((byte)(tmp % 100));
            data[start + 1] |= Dec2Bcd((byte)((tmp / 100) % 100));
            return true;
        }
        /// <summary>
        /// 数据格式07解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format07(byte[] data, int start, out double? value)
        {
            value = 0;
            if (!IsBcd(data[start]) || !IsBcd(data[start + 1]))
            {
                value = null;
                return false;
            }
            value = Bcd2Dec(data[start]) / 10.0 + Bcd2Dec(data[start + 1]) * 10;
            return true;
        }
        /// <summary>
        /// 数据格式07编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format07(double value, byte[] data, int start)
        {
            if (value < 0.1 || value > 999.9)
                return false;
            int index = start;
            int tmp = (int)(value * 10);
            data[start] = Dec2Bcd((byte)(tmp % 100));
            data[start + 1] = Dec2Bcd((byte)((tmp / 100) % 100));
            return true;
        }
        /// <summary>
        /// 数据格式08解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format08(byte[] data, int start, out int value)
        {
            value = 0;
            if (!IsBcd(data[start]) || !IsBcd(data[start + 1]))
                return false;
            value = Bcd2Dec(data[start]) + Bcd2Dec(data[start + 1]) * 100;
            return true;
        }
        /// <summary>
        /// 数据格式09解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format09(byte[] data, int start, out double? value)
        {
            value = 0;
            if (!IsBcd(data[start]) || !IsBcd(data[start + 1]) || !IsBcd((byte)(data[start + 2] & 0x7F)))
                return false;
            value = Bcd2Dec(data[start]) / 10000.0 + Bcd2Dec(data[start + 1]) / 100.0 + Bcd2Dec((byte)(data[start + 2] & 0x7F));
            if ((data[start + 2] & 0x80) > 0)
            {
                value *= -1;
            }
            return true;
        }
        /// <summary>
        /// 数据格式10解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format10(byte[] data, int start, out int value)
        {
            value = 0;
            if (!IsBcd(data[start]) || !IsBcd(data[start + 1]) || !IsBcd(data[start + 2]))
                return false;
            value = Bcd2Dec(data[start]) + Bcd2Dec(data[start + 1]) * 100 + Bcd2Dec(data[start + 2]) * 10000;
            return true;
        }
        /// <summary>
        /// 数据格式11解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format11(byte[] data, int start, out double? value)
        {
            value = 0;
            for (int i = 0; i < (int)FormatLenOption.Format11; i++)
            {
                if (!IsBcd(data[start + i]))
                {
                    value = null;
                    return false;
                }
                value = value * 100 + Bcd2Dec(data[start + 3 - i]);
            }
            value /= 100;
            return true;
        }
        /// <summary>
        /// 数据格式12解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format12(byte[] data, int start, out long value)
        {
            value = 0;
            for (int i = 0; i < (int)FormatLenOption.Format12; i++)
            {
                if (!IsBcd(data[start + i]))
                    return false;
                value = value * 100 + Bcd2Dec(data[start + 5 - i]);
            }
            return true;
        }
        /// <summary>
        /// 数据格式12编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format12(long value, byte[] data, int start)
        {
            int i = 0;
            while (value > 0 && i < (int)FormatLenOption.Format12)
            {
                byte tmp = (byte)(value % 100);
                value /= 100;
                data[start + i] = Dec2Bcd(tmp);
            }
            return true;
        }
        /// <summary>
        /// 数据格式13解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format13(byte[] data, int start, out double? value)
        {
            value = 0;
            for (int i = 0; i < (int)FormatLenOption.Format13; i++)
            {
                if (!IsBcd(data[start + i]))
                {
                    value = null;
                    return false;
                }
                value = value * 100 + Bcd2Dec(data[start + 3 - i]);
            }
            value /= 10000;
            return true;
        }
        /// <summary>
        /// 数据格式14解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format14(byte[] data, int start, out double? value)
        {
            value = 0;
            for (int i = 0; i < (int)FormatLenOption.Format14; i++)
            {
                if (!IsBcd(data[start + i]))
                {
                    value = null;
                    return false;
                }
                value = value * 100 + Bcd2Dec(data[start + 4 - i]);
            }
            value /= 10000;
            return true;
        }
        /// <summary>
        /// 数据格式15解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format15(byte[] data, int start, out DateTime value)
        {
            value = new DateTime();
            for (int i = 0; i < (int)FormatLenOption.Format15; i++)
            {
                if (!IsBcd(data[start + i]))
                    return false;
            }
            string str = (Bcd2Dec(data[start + 4]) + 2000) + "-" + Bcd2Dec(data[start + 3]) + "-" + Bcd2Dec(data[start + 2]);
            str += " " + Bcd2Dec(data[start + 1]) + ":" + Bcd2Dec(data[start]) + ":0";
            value = Convert.ToDateTime(str);
            return true;
        }
        /// <summary>
        /// 数据格式15编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format15(DateTime value, byte[] data, int start)
        {
            int index = start;
            data[index++] = Dec2Bcd((byte)value.Minute);
            data[index++] = Dec2Bcd((byte)value.Hour);
            data[index++] = Dec2Bcd((byte)value.Day);
            data[index++] = Dec2Bcd((byte)value.Month);
            data[index++] = Dec2Bcd((byte)(value.Year % 100));
            return true;
        }
        /// <summary>
        /// 数据格式16解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format16(byte[] data, int start, out DateTime value)
        {
            value = DateTime.Now;
            for (int i = 0; i < (int)FormatLenOption.Format16; i++)
            {
                if (!IsBcd(data[start + i]))
                    return false;
            }
            if (Bcd2Dec(data[start + 3]) > value.Day)
            {
                value.AddMonths(-1);
            }
            string str = value.Year + "-" + value.Month + "-" + Bcd2Dec(data[start + 3]);
            str += " " + Bcd2Dec(data[start + 2]) + ":" + Bcd2Dec(data[start+1]) + ":" + Bcd2Dec(data[start]);
            value = Convert.ToDateTime(str);
            return true;
        }
        /// <summary>
        /// 数据格式17解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format17(byte[] data, int start, out DateTime value)
        {
            value = DateTime.Now;
            for (int i = 0; i < (int)FormatLenOption.Format17; i++)
            {
                if (!IsBcd(data[start + i]))
                    return false;
            }
            if (Bcd2Dec(data[start + 3]) > value.Month)
            {
                value.AddYears(-1);
            }
            string str = value.Year + "-" + Bcd2Dec(data[start + 3]) + "-" + Bcd2Dec(data[start + 2]);
            str += " " + Bcd2Dec(data[start + 1]) + ":" + Bcd2Dec(data[start]) + ":0";
            value = Convert.ToDateTime(str);
            return true;
        }
        /// <summary>
        /// 数据格式18解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format18(byte[] data, int start, out DateTime value)
        {
            value = DateTime.Now;
            for (int i = 0; i < (int)FormatLenOption.Format18; i++)
            {
                if (!IsBcd(data[start + i]))
                {
                    return false;
                }
            }
            if (Bcd2Dec(data[start + 2]) > value.Day)
            {
                value.AddMonths(-1);
            }
            string str = value.Year + "-" + value.Month + "-" + Bcd2Dec(data[start + 2]);
            str += " " + Bcd2Dec(data[start + 1]) + ":" + Bcd2Dec(data[start]) + ":0";
            value = Convert.ToDateTime(str);
            return true;
        }
        /// <summary>
        /// 数据格式19解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format19(byte[] data, int start, out DateTime value)
        {
            value = DateTime.Now;
            for (int i = 0; i < (int)FormatLenOption.Format19; i++)
            {
                if (!IsBcd(data[start + i]))
                    return false;
            }
            string str = value.Year + "-" + value.Month + "-" + value.Day;
            str += " " + Bcd2Dec(data[start + 1]) + ":" + Bcd2Dec(data[start]) + ":0";
            value = Convert.ToDateTime(str);
            return true;
        }
        /// <summary>
        /// 数据格式19编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format19(DateTime value, byte[] data, int start)
        {
            int index = start;
            data[index++] = Dec2Bcd((byte)value.Hour);
            data[index++] = Dec2Bcd((byte)value.Minute);
            return true;
        }
        /// <summary>
        /// 数据格式20解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format20(byte[] data, int start, out DateTime value)
        {
            value = new DateTime();
            for (int i = 0; i < (int)FormatLenOption.Format20; i++)
            {
                if (!IsBcd(data[i]))
                    return false;
            }
            string str = (Bcd2Dec(data[2]) + 2000) + "-" + Bcd2Dec(data[1]) + "-" + Bcd2Dec(data[0]);
            value = System.Convert.ToDateTime(str);
            return true;
        }
        /// <summary>
        /// 数据格式20编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format20(DateTime value, byte[] data, int start)
        {
            int index = start;
            data[index++] = Dec2Bcd((byte)value.Day);
            data[index++] = Dec2Bcd((byte)value.Month);
            data[index++] = Dec2Bcd((byte)(value.Year % 100));
            return true;
        }
        /// <summary>
        /// 数据格式21解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format21(byte[] data, int start, out DateTime value)
        {
            value = new DateTime();
            for (int i = 0; i < (int)FormatLenOption.Format21; i++)
            {
                if (!IsBcd(data[i]))
                    return false;
            }
            string str = (data[2] + 2000) + "-" + data[1] + "-1";
            value = System.Convert.ToDateTime(str);
            return true;
        }
        /// <summary>
        /// 数据格式21编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format21(DateTime value, byte[] data, int start)
        {
            int index = start;
            data[index++] = Dec2Bcd((byte)value.Month);
            data[index++] = Dec2Bcd((byte)(value.Year % 100));
            return true;
        }
        /// <summary>
        /// 数据格式22解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format22(byte[] data, int start, out double? value)
        {
            if (!IsBcd(data[start]))
            {
                value = null;
                return false;
            }
            value = Bcd2Dec(data[start]) / 10.0;
            return true;
        }
        /// <summary>
        /// 数据格式22编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format22(double value, byte[] data, int start)
        {
            if (value < 0.1 || value > 9.9)
                return false;
            int index = start;
            int tmp = (int)(value * 10);
            data[start] = Dec2Bcd((byte)(tmp % 100));
            return true;
        }
        /// <summary>
        /// 数据格式23解码
        /// </summary>
        /// <param name="data">待解码数据</param>
        /// <param name="start">数据起始位置</param>
        /// <param name="value">输出解码结果</param>
        /// <returns>解码成功返回true</returns>
        protected static bool Bcd2Dec_Format23(byte[] data, int start, out double? value)
        {
            value = 0;
            for (int i = 0; i < (int)FormatLenOption.Format23; i++)
            {
                if (!IsBcd(data[start + i]))
                {
                    value = null;
                    return false;
                }
                value = value * 100 + Bcd2Dec(data[start + i]);
            }
            value /= 10000;
            return true;
        }
        /// <summary>
        /// 数据格式23编码
        /// </summary>
        /// <param name="value">待编码数据</param>
        /// <param name="data">编码后的数据</param>
        /// <param name="start">数据起始位置</param>
        /// <returns>编码成功返回true</returns>
        protected static bool Dec2Bcd_Format23(double value, byte[] data, int start)
        {
            if (value < 0.0001 || value > 99.9999)
                return false;
            int tmp = (int)(value * 10000);
            for (int i = 0; i < (int)FormatLenOption.Format23; i++)
            {
                data[start + i] = Dec2Bcd((byte)(tmp % 100));
                tmp /= 100;
            }
            return true;
        }

        #endregion
        /// <summary>
        /// IP地址转换为字节数组
        /// </summary>
        /// <param name="IP">字符串IP</param>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        protected static bool IP2Bytes(string IP, byte[] data, int start)
        {
            string[] ips = IP.Split('.');
            for (int i = 0; i < ips.Length; i++)
            {
                data[start + i] = System.Convert.ToByte(ips[i]);
            }
            return true;
        }
        /// <summary>
        /// 计算校验和（按字节模和）
        /// </summary>
        /// <param name="data">待计算的数据</param>
        /// <param name="start">开始位置</param>
        /// <param name="len">需要计算的长度</param>
        /// <returns>计算后的校验和</returns>
        protected static byte CalcCS(byte[] data, int start, int len)
        {
            int cs = 0;
            for (int i = start; i < start + len; i++)
            {
                cs += data[i];
            }
            return (byte)cs;
        }
        #endregion

        #region 规约数据项定义
        /*
         *	所有数据项定义名称都以AFNXX_FYYY格式
         *	其中XX为AFN编号，不足两位用0补足，如AFN04
         *	YYY为F项编号，不足三位用0不足，如F002
         */
        /// <summary>
        /// 数据单元抽象类
        /// </summary>
        public abstract class DataUnitBase
        {
            public new bool Equals(object obj)
            {
                DataUnitBase unit = (DataUnitBase)obj;
                if(Pn==unit.Pn && Fn==unit.Fn && AFN==unit.AFN)
                    return true;
                return false;
            }
            /// <summary>
            /// 数据单元编码，编码后数据在Data中
            /// </summary>
            /// <returns></returns>
            public abstract bool Encode(bool isSet, int MaxRespLen = 255);
            /// <summary>
            /// 数据单元解码，需要解码的数据在Data中
            /// 该函数必须对OperResult进行赋值
            /// </summary>
            /// <returns></returns>
            public abstract bool Decode();
            /// <summary>
            /// Pn值。后台在调用召测之前要设置
            /// </summary>
            public int Pn { get; set; }
            /// <summary>
            /// Fn值
            /// 由程序根据类名自动获取
            /// </summary>
            public int Fn
            {
                get
                {
                    string str = this.GetType().ToString();
                    return Convert.ToInt32(str.Substring(str.LastIndexOf('F') + 1));
                }
            }
            /// <summary>
            /// AFN
            /// 由程序根据类名自动获取
            /// </summary>
            public int AFN
            {
                get
                {
                    string name = this.GetType().ToString();
                    int afn = Convert.ToInt32(name.Substring(name.LastIndexOf("N") + 1, 2), 16);
                    return afn;
                }
            }
            /// <summary>
            /// 对应的规约格式数据。
            /// 组帧内容和待解帧的内容都在此存放
            /// </summary>
            public byte[] Data { get; set; }
            private int respLen;
            /// <summary>
            /// 响应帧数据单元的可能长度
            /// 规约中是定长的可以直接用DataFixLen代替
            /// 变长的需要在Encode中设置
            /// </summary>
            public int RespLen
            {
                get
                {
                    if (respLen == 0)
                        return DataFixLen;
                    return respLen;
                }
                set
                {
                    respLen = value;
                }
            }
            /// <summary>
            /// 是否可与其他数据项组合成一帧处理
            /// </summary>
            public bool IsCombined { get; set; }
            /// <summary>
            /// 该数据单元规约帧固定长度。
            /// 该值对于每个Fn来说是常量，因此外部不能设置。
            /// 如果规约长度是由不确定的n个固定x字节组成（n不固定，x固定），则DataFixLen = x
            /// </summary>
            public abstract int DataFixLen { get; }
            #region 数据项分帧处理
            /// <summary>
            /// 主数据项与子数据项之间的关系。只需要主数据项设置
            /// 0-与，表示子数据项全部成功主数据项才成功 1-或，表示一个子数据项成功则主数据项就可以认为成功
            /// </summary>
            public int SubRelation { get; set; }
            /// <summary>
            /// 子数据项列表
            /// 组帧时使用，主要是当该数据项一帧不能返回所有数据时，将数据项分成多个，从第二个开始放到该列表中
            /// 例如：对于电台终端召测96点电量曲线，必须要召测两帧，假设两帧为：1、0点到12点 2、12点到24点。那么12点到24点
            ///       的召测就需要额外生成一个子数据项放到SubDataUnitList中。该功能在Encode()中完成
            /// </summary>
            public List<DataUnitBase> SubDataUnitList = null;
            /// <summary>
            /// 将解帧后子数据项合并到主数据项。
            /// </summary>
            /// <param name="unit">子数据项</param>
            /// <returns></returns>
            public virtual bool Merge(DataUnitBase unit)
            {
                if (SubRelation == 0)
                {
                    if (OperResult == OperResultOption.Success)
                    {
                        if (unit.OperResult != OperResultOption.Success)
                        {
                            OperResult = unit.OperResult;
                            OperResultDesc = unit.OperResultDesc;
                        }
                    }
                }
                else if(SubRelation == 1)
                {
                    if (OperResult != OperResultOption.Success)
                    {
                        if (unit.OperResult == OperResultOption.Success)
                        {
                            OperResult = unit.OperResult;
                            OperResultDesc = "";
                        }
                    }
                }
                return true;
            }
            #endregion
            #region 数据项处理结果相关信息
            /// <summary>
            /// 数据项处理结果。
            /// Decode()中需要填写解码处理结果
            /// </summary>
            public OperResultOption OperResult { get; set; }
            /// <summary>
            /// 数据项处理结果描述，主要存放失败原因
            /// </summary>
            public string OperResultDesc { get; set; }
            #endregion
        }
        /// <summary>
        /// 根据AFN和Fn创建数据单元对象
        /// </summary>
        /// <param name="AFN"></param>
        /// <param name="Fn"></param>
        /// <returns></returns>
        private DataUnitBase CreateDataUnit(AFNOption AFN, int Fn)
        {
            //特殊处理：参数召测生成的F项是参数设置的F项
            Type DataUnitType = Type.GetType(this.GetType() + "+AFN" + ((int)(AFN == AFNOption.AFN_PARAM_QUERY ? AFNOption.AFN_PARAM_SET : AFN)).ToString("X2") + "_F" + Fn.ToString("D3"));
            if (DataUnitType != null)
            {
                return (DataUnitBase)Activator.CreateInstance(DataUnitType);
            }
            return null;
        }
        #endregion//规约数据项定义

        #region 数据设置、召测处理
#region 阻塞处理
        /// <summary>
        /// 数据下发
        /// </summary>
        /// <param name="dataUnit">要下发的数据项</param>
        /// <returns></returns>
        public DataUnitBase SetDataUnit(DataUnitBase dataUnit)
        {
            List<DataUnitBase> list = new List<DataUnitBase>();
            list.Add(dataUnit);
            list = SetDataUnit(list);
            return list[0];
        }
        /// <summary>
        /// 数据下发
        /// </summary>
        /// <param name="dataUnitList">要下发的数据项列表</param>
        /// <returns></returns>
        public List<DataUnitBase> SetDataUnit(List<DataUnitBase> dataUnitList)
        {
            //预处理成功数据项列表
            List<DataUnitBase> preDealOKList = new List<DataUnitBase>();
            //预处理失败数据项列表
            List<DataUnitBase> preDealFailList = new List<DataUnitBase>();
            foreach (DataUnitBase dataUnit in dataUnitList)
            {
                AFNOption afn = (AFNOption)dataUnit.AFN;
                if (afn != AFNOption.AFN_CONTROL && afn != AFNOption.AFN_PARAM_SET)
                {
                    dataUnit.OperResult = OperResultOption.Other;
                    dataUnit.OperResultDesc = "该操作只支持参数和控制！";
                    preDealFailList.Add(dataUnit);
                    continue;
                }
                if (!dataUnit.Encode(true, MaxRespLen))
                {
                    preDealFailList.Add(dataUnit);
                    continue;
                }
                preDealOKList.Add(dataUnit);
            }

            List<DataUnitBase> dealList = BlockingProc(true, preDealOKList);
            if (dealList == null)
            {
                dealList = preDealFailList;
            }
            else
            {
                dealList.AddRange(preDealFailList);
            }
            return dealList;
        }
        /// <summary>
        /// 数据召测
        /// </summary>
        /// <param name="dataUnit">要召测的数据项</param>
        /// <returns></returns>
        public DataUnitBase GetDataUnit(DataUnitBase dataUnit)
        {
            List<DataUnitBase> list = new List<DataUnitBase>();
            list.Add(dataUnit);
            list = GetDataUnit(list);
            return list[0];
        }
        /// <summary>
        /// 数据召测
        /// </summary>
        /// <param name="dataUnitList">要召测的数据项列表</param>
        /// <returns></returns>
        public List<DataUnitBase> GetDataUnit(List<DataUnitBase> dataUnitList)
        {
            //预处理成功数据项列表
            List<DataUnitBase> preDealOKList = new List<DataUnitBase>();
            //预处理失败数据项列表
            List<DataUnitBase> preDealFailList = new List<DataUnitBase>();
            foreach (DataUnitBase dataUnit in dataUnitList)
            {
                AFNOption afn = (AFNOption)dataUnit.AFN;
                if (afn == AFNOption.AFN_CONTROL)
                {
                    dataUnit.OperResult = OperResultOption.Other;
                    dataUnit.OperResultDesc = "该操作不支持控制！";
                    preDealFailList.Add(dataUnit);
                    continue;
                }
                if (!dataUnit.Encode(false, MaxRespLen))
                {
                    preDealFailList.Add(dataUnit);
                    continue;
                }
                preDealOKList.Add(dataUnit);
            }
            List<DataUnitBase> dealList = BlockingProc(false, preDealOKList);
            if (dealList == null)
            {
                dealList = preDealFailList;
            }
            else
            {
                dealList.AddRange(preDealFailList);
            }
            
            return dealList;
        }
        /// <summary>
        /// 阻塞处理。所有数据项处理完毕后返回
        /// </summary>
        /// <param name="isSet">是否是设置下发</param>
        /// <param name="dataUnitList">要处理的数据项列表</param>
        /// <returns></returns>
        protected List<DataUnitBase> BlockingProc(bool isSet, List<DataUnitBase> dataUnitList)
        {
            DownUnitCol.Clear();
            foreach (DataUnitBase dataUnit in dataUnitList)
            {
                AFNOption afn = (AFNOption)dataUnit.AFN;
                if (!isSet)
                {
                    if (afn == AFNOption.AFN_PARAM_SET)
                    {
                        afn = AFNOption.AFN_PARAM_QUERY;
                    }
                }
                AddDownDataUnit(afn, dataUnit);
            }
            string errMsg = "";
            bool bSucc = ComposeAll(ref errMsg);
            if (!bSucc)
                return null;
            //发送接收
            byte[] rcvData = new byte[2048];
            int rcvLen = 0;
            foreach (Frame frame in FrameList)
            {
                if (frame.Retry >= 0)
                {
                    comm.Write(frame.DownData, 0, frame.DownData.Length);
                    rcvLen = 0;
                    frame.IsSent = true;
                    frame.IsTimeout = false;
                    frame.Retry--;
                    System.Threading.Thread.Sleep(500);
                    if (comm.BytesToRead > 0)
                    {
                        rcvLen = comm.Read(rcvData, 0, 2048);
                        byte[] tmp = new byte[rcvLen];
                        System.Array.Copy(rcvData, tmp, rcvLen);
                        frame.UpData = tmp;
                        List<DataUnitBase> list = Uncompose(tmp, frame);
                        if (list != null)
                        {
                            foreach(DataUnitBase unit in list)
                            {
                                AFNOption Afn = (AFNOption)unit.AFN;
                                if(Afn==AFNOption.AFN_PARAM_SET && !isSet)
                                {
                                    Afn = AFNOption.AFN_PARAM_QUERY;
                                }
                                AddUpDataUnit(Afn, unit);
                            }
                        }
                    }
                    else
                    {
                        frame.IsTimeout = true;
                    }
                }
            }
            List<DataUnitBase> dealUnitList = new List<DataUnitBase>();
            foreach (DataUnitBase dataUnit in dataUnitList)
            {
                AFNOption afn = (AFNOption)dataUnit.AFN;
                if (!isSet)
                {
                    if (afn == AFNOption.AFN_PARAM_SET)
                    {
                        afn = AFNOption.AFN_PARAM_QUERY;
                    }
                }
                DataUnitBase operUnit = FindUpDataUnit(afn, dataUnit);
                if (operUnit == null)
                {
                    operUnit = dataUnit;
                    operUnit.OperResult = OperResultOption.Other;
                    operUnit.OperResultDesc = "终端有回码，但未包含此数据项";
                }
                dealUnitList.Add(operUnit);
            }

            return dealUnitList;
        }
        #endregion//阻塞处理
        #region 非阻塞处理
        /// <summary>
        /// 下达任务-非阻塞方式
        /// </summary>
        /// <param name="isSet">是否是设置（控制投入、参数下发）</param>
        /// <param name="dataUnitList">数据项列表</param>
        /// <returns>下达成功返回true</returns>
        public bool MakeTask(long taskId, List<DataUnitBase> dataUnitList, bool isSet = false)
        {
            TaskId = taskId;
            IsSetTask = isSet;
            return UnblockPreProc(dataUnitList, isSet);
        }
        /// <summary>
        /// 帧接收处理-非阻塞方式
        /// 由通道对象收到数据或超时等异常时调用
        /// </summary>
        /// <param name="frameId">帧编号。</param>       
        /// <param name="flag">标志 0-成功回码 1-无回码 2-与前置机通信异常</param>
        /// <param name="data">收到的数据</param>
        public void FrameRecv(int frameId, int flag, byte[] data)
        {
            foreach (Frame frame in FrameList)
            {
                if (frame.Id == frameId)
                {
                    if (flag == 0)
                    {
                        frame.UpData = data;
                        List<DataUnitBase> list = Uncompose(data, frame);
                        if (list != null)
                        {
                            foreach (DataUnitBase unit in list)
                            {
                                AFNOption Afn = (AFNOption)unit.AFN;
                                if (Afn == AFNOption.AFN_PARAM_SET && !IsSetTask)
                                {
                                    Afn = AFNOption.AFN_PARAM_QUERY;
                                }
                                AddUpDataUnit(Afn, unit);
                            }
                        }
                    }
                    else
                    {
                        frame.IsAnswer = false;
                        frame.IsTimeout = true;
                    }
                    break;
                }
            }
            if (!UnblockSendFrame())
            {//处理完毕
                UnblockFinishProc();
            }
        }
        /// <summary>
        /// 非阻塞预处理
        /// </summary>
        /// <param name="dataUnitList"></param>
        /// <param name="isSet"></param>
        /// <returns></returns>
        private bool UnblockPreProc(List<DataUnitBase> dataUnitList, bool isSet)
        {
            preDealFailDataUnitList.Clear();
            dealingDataUnitList.Clear();
            DownUnitCol.Clear();
            foreach (DataUnitBase dataUnit in dataUnitList)
            {
                AFNOption afn = (AFNOption)dataUnit.AFN;
                if (isSet)
                {
                    if (afn != AFNOption.AFN_CONTROL && afn != AFNOption.AFN_PARAM_SET)
                    {
                        dataUnit.OperResult = OperResultOption.Other;
                        dataUnit.OperResultDesc = "该操作只支持参数和控制！";
                        preDealFailDataUnitList.Add(dataUnit);
                        continue;
                    }
                }
                else
                {
                    if (afn == AFNOption.AFN_CONTROL)
                    {
                        dataUnit.OperResult = OperResultOption.Other;
                        dataUnit.OperResultDesc = "该操作不支持控制！";
                        preDealFailDataUnitList.Add(dataUnit);
                        continue;
                    }
                    else if (afn == AFNOption.AFN_PARAM_SET)
                    {
                        afn = AFNOption.AFN_PARAM_QUERY;
                    }
                }
                if (!dataUnit.Encode(isSet, MaxRespLen))
                {
                    preDealFailDataUnitList.Add(dataUnit);
                    continue;
                }
                AddDownDataUnit(afn, dataUnit);
                dealingDataUnitList.Add(dataUnit);
            }
            string errMsg="";
            bool bSucc = ComposeAll(ref errMsg);
            if (!bSucc)
                return false;
            return UnblockSendFrame();
        }
        /// <summary>
        /// 非阻塞发送一帧
        /// </summary>
        /// <returns>true-成功发送 false-没有帧可以发了，表示非阻塞处理结束</returns>
        private bool UnblockSendFrame()
        {
            bool bEnd = true;
            foreach (Frame frame in FrameList)
            {
                if(frame.IsEnd)
                    continue;
                if(frame.IsAnswer)
                    continue;
                if (frame.Retry<0)
                {
                    frame.IsEnd = true;
                    continue;
                }
                else
                {
                    bEnd = false;
                    //发送一帧出去
                    comm.Write(frame.DownData, 0, frame.DownData.Length);
                    frame.IsSent = true;
                    frame.IsTimeout = false;
                    frame.Retry--;
                    break;
                }
            }
            return !bEnd;
        }
        /// <summary>
        /// 非阻塞结束处理
        /// </summary>
        /// <returns></returns>
        private bool UnblockFinishProc()
        {
            List<DataUnitBase> dealUnitList = new List<DataUnitBase>();
            foreach (DataUnitBase dataUnit in dealingDataUnitList)
            {
                AFNOption afn = (AFNOption)dataUnit.AFN;
                if (!IsSetTask)
                {
                    if (afn == AFNOption.AFN_PARAM_SET)
                    {
                        afn = AFNOption.AFN_PARAM_QUERY;
                    }
                }
                DataUnitBase operUnit = FindUpDataUnit(afn, dataUnit);
                if (operUnit == null)
                {
                    operUnit = dataUnit;
                    operUnit.OperResult = OperResultOption.Other;
                    operUnit.OperResultDesc = "终端有回码，但未包含此数据项";
                }
                dealUnitList.Add(operUnit);
            }
            dealUnitList.AddRange(preDealFailDataUnitList);
            this.TaskResultReport(TaskId, dealUnitList);
            return true;
        }
        /// <summary>
        /// 正在处理的数据项列表（参与组帧的）
        /// </summary>
        private List<DataUnitBase> dealingDataUnitList = new List<DataUnitBase>();
        /// <summary>
        /// 预处理失败的数据项列表（没有参与组帧）
        /// </summary>
        private List<DataUnitBase> preDealFailDataUnitList = new List<DataUnitBase>();
        /// <summary>
        /// 本次任务的编号
        /// </summary>
        private long TaskId { get; set; }
        /// <summary>
        /// 本次任务是否是设置命令（控制下发、参数设置）
        /// </summary>
        private bool IsSetTask { get; set; }
        #endregion //非阻塞处理      
        #endregion//数据项设置、召测处理

        #region 组帧处理
        /// <summary>
        /// 将Pn、Fn转换成Da、Dt
        /// </summary>
        /// <param name="Pn"></param>
        /// <param name="Fn"></param>
        /// <param name="data"></param>
        /// <param name="start"></param>
        protected abstract void DaDt(int Pn, int Fn, byte[] data, int start);
        /// <summary>
        /// 向参数设置数据单元集合增加一个数据单元
        /// </summary>
        /// <param name="dataUnit"></param>
        protected void AddParamSetUnit(DataUnitBase dataUnit)
        {
            AddDownDataUnit(AFNOption.AFN_PARAM_SET, dataUnit);
        }
        protected void AddParamQueryUnit(DataUnitBase dataUnit)
        {
            AddDownDataUnit(AFNOption.AFN_PARAM_QUERY, dataUnit);
        }
        /// <summary>
        /// 向数据单元集合增加一个指定AFN的数据单元
        /// </summary>
        /// <param name="AFN"></param>
        /// <param name="dataUnit"></param>
        private void AddDownDataUnit(AFNOption AFN, DataUnitBase dataUnit)
        {
            List<DataUnitBase> list = null;
            if (DownUnitCol.ContainsKey(AFN))
            {
                list = (List<DataUnitBase>)DownUnitCol[AFN];
            }
            else
            {
                list = new List<DataUnitBase>();
                DownUnitCol.Add(AFN, list);
            }
            list.Add(dataUnit);
        }

        /// <summary>
        /// 将数据单元组装成规约帧
        /// </summary>
        /// <returns>组帧成功返回true</returns>
        private bool ComposeAll(ref string errMsg)
        {
            //清空上次处理
            FrameList.Clear();
            UpUnitCol.Clear();
            if (DownUnitCol.Count == 0)
            {
                errMsg = "没有需要组帧的数据项！";
                return false;
            }
            else
            {
                foreach (DictionaryEntry de in DownUnitCol)
                {
                    Compose((AFNOption)de.Key, (List<DataUnitBase>)de.Value);
                }
            }
            return true;
        }
        /// <summary>
        /// 根据AFN和List组帧
        /// </summary>
        /// <param name="AFN"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool Compose(AFNOption AFN, List<DataUnitBase> unitList)
        {
            byte[] data = new byte[1024];
            int dataLen = 0;        //数据长度
            int dataUnitCount = 0;  //数据单元个数
            int respLen = 0;        //响应帧长度
            Frame frameObj = new Frame();
            //所有需要组帧数据项 unitList+子数据项
            List<DataUnitBase> allUnitList = new List<DataUnitBase>();
            allUnitList.AddRange(unitList);
            foreach (DataUnitBase dataUnit in unitList)
            {//获取子数据项
                if (dataUnit.SubDataUnitList != null)
                {
                    allUnitList.AddRange(dataUnit.SubDataUnitList);
                }
            }
            foreach (DataUnitBase dataUnit in allUnitList)
            {
                if (!dataUnit.IsCombined)
                {//只能单独组帧
                    int PFC = GetPFC();
                    byte[] tmp = new byte[4 + dataUnit.Data.Length];
                    DaDt(dataUnit.Pn, dataUnit.Fn, tmp, 0);
                    dataUnit.Data.CopyTo(tmp, 4);
                    byte[] frame = MakeFrame(AFN, tmp, PFC);
                    Frame frameObj1 = new Frame();
                    frameObj1.Id = FrameList.Count + 1;
                    frameObj1.DownData = frame;
                    frameObj1.RespLen = dataUnit.RespLen+26;
                    frameObj1.PFC = PFC;
                    frameObj1.AddDataUnit(dataUnit);
                    FrameList.Add(frameObj1);
                    continue;
                }
                if (((respLen + dataUnit.RespLen) >= MaxRespLen || (dataUnitCount + 1) >= MaxDataUnitCount) && dataLen > 0)
                {//超过最大响应帧长度或超过最大数据单元个数
                    int PFC = GetPFC();
                    byte[] tmp = new byte[dataLen];
                    System.Array.Copy(data, tmp, dataLen);
                    byte[] frame = MakeFrame(AFN, tmp, PFC);
                    frameObj.DownData = frame;
                    frameObj.RespLen = respLen+22;
                    frameObj.PFC = PFC;
                    frameObj.Id = FrameList.Count + 1;
                    FrameList.Add(frameObj);
                    frameObj = new Frame();
                    dataLen = 0;
                    dataUnitCount = 0;
                    respLen = 0;
                }
                DaDt(dataUnit.Pn, dataUnit.Fn, data, dataLen);
                dataLen += 4;
                respLen += 4;
                dataUnit.Data.CopyTo(data, dataLen);
                dataLen += dataUnit.Data.Length;
                frameObj.AddDataUnit(dataUnit);
                dataUnitCount++;
                respLen += dataUnit.RespLen;
            }
            if (dataLen > 0)
            {
                int PFC = GetPFC();
                byte[] tmp = new byte[dataLen];
                System.Array.Copy(data, tmp, dataLen);
                byte[] frame = MakeFrame(AFN, tmp, PFC);
                frameObj.DownData = frame;
                frameObj.RespLen = respLen+22;
                frameObj.PFC = PFC;
                frameObj.Id = FrameList.Count + 1;
                FrameList.Add(frameObj);
                dataLen = 0;
                dataUnitCount = 0;
                respLen = 0;
            }
            unitList.Clear();
            return true;
        }
        /// <summary>
        /// 将数据组成规约帧
        /// </summary>
        /// <param name="AFN"></param>
        /// <param name="data">需要组帧的数据</param>
        /// <param name="PFC"></param>
        /// <param name="bPw">是否带PW</param>
        /// <param name="bTp">是否带Tp</param>
        /// <returns>组装的规约帧内容</returns>
        private byte[] MakeFrame(AFNOption AFN, byte[] data, int PFC)
        {
            byte[] frame = new byte[1024];
            int index = 0;

            bool bPw = false;   //是否需要密码
            bool bTp = false;   //是否带Tp
            bool bCon = false;  //是否需要确认
            FuncOption func = FuncOption.MASTER_FUNC_USERDATA;      //功能码
            switch (AFN)
            {
                case AFNOption.AFN_YESNO:
                    {
                        func = FuncOption.SLAVE_FUNC_LINKSTATUS;
                        bCon = false;
                        bPw = false;
                        bTp = false;
                    }
                    break;
                case AFNOption.AFN_LINKTEST:
                    {

                    }
                    break;
                case AFNOption.AFN_RESET:
                    {
                        func = FuncOption.MASTER_FUNC_RESET;
                        bCon = true;
                        bPw = true;
                        bTp = false;
                    }
                    break;
                case AFNOption.AFN_PARAM_SET:
                case AFNOption.AFN_CONTROL:
                    {
                        func = FuncOption.MASTER_FUNC_USERDATA;
                        if (AddrType == AddrTypeOption.UNICAST)
                            bCon = true;
                        else
                            bCon = false;
                        bPw = true;
                        bTp = false;
                    }
                    break;
                default:
                    {
                        func = FuncOption.MASTER_FUNC_LEVEL2;
                        bCon = false;
                        bPw = false;
                        bTp = IsHavePFC;
                    }
                    break;
            }
            //帧头
            frame[index++] = 0x68;
            index += 4; //越过长度
            frame[index++] = 0x68;
            //控制码
            frame[index] = 0;       //DIR=0 FCB=0 FCV=0
            frame[index] |= 1 << 6; //PRM
            frame[index] |= (byte)(0x0F & (int)func);
            index++;
            //行政区码A1
            Dec2Bcd(AreaCode, frame, index, 2);
            index += 2;
            //终端地址A2
            frame[index] = (byte)(AddrCode % 256);
            frame[index + 1] = (byte)(AddrCode / 256);
            index += 2;
            //A3
            frame[index] = (byte)(GetMSA() << 1);
            frame[index] = (byte)(frame[index] | (AddrType == AddrTypeOption.UNICAST ? 0 : 1));
            index++;
            //AFN
            frame[index++] = (byte)AFN;
            //SEQ
            frame[index] = 0;
            if (bTp)
                frame[index] |= 1 << 7;
            frame[index] |= 3 << 5;
            if (bCon)
                frame[index] |= 1 << 4;
            frame[index] |= (byte)(PFC & 0x0F);
            index++;
            //数据
            data.CopyTo(frame, index);
            index += data.Length;
            //PW
            if (bPw)
            {
                index += MakePWD(frame, index);
            }
            if (bTp)
            {
                frame[index++] = (byte)PFC;
                DateTime dt = DateTime.Now;
                frame[index++] = Dec2Bcd((byte)dt.Second);
                frame[index++] = Dec2Bcd((byte)dt.Minute);
                frame[index++] = Dec2Bcd((byte)dt.Hour);
                frame[index++] = Dec2Bcd((byte)dt.Day);
                frame[index++] = 0;
            }
            //校验和
            int length = index - 6;
            frame[index] = CalcCS(frame, 6, length);
            index++;
            //帧尾
            frame[index++] = 0x16;
            //帧长度
            length = (length << 2) | 0x1;
            frame[1] = frame[3] = (byte)(length % 256);
            frame[2] = frame[4] = (byte)(length / 256);
            byte[] tmp = new byte[index];
            System.Array.Copy(frame, tmp, index);
            return tmp;
        }
        /// <summary>
        /// 生成密码
        /// </summary>
        /// <param name="data">规约帧</param>
        /// <param name="index">密码存放起始位置</param>
        /// <returns>返回密码的长度</returns>
        protected abstract int MakePWD(byte[] data, int index);
        /// <summary>
        /// 需要组帧的数据单元集合
        /// 关键字为AFN，值为所有该AFN下的数据单元List<DataUnitBase>
        /// </summary>
        private Hashtable DownUnitCol = new Hashtable();
        /// <summary>
        /// 规约帧信息。
        /// 包含组帧、发送、应答信息
        /// </summary>
        public class Frame
        {
            /// <summary>
            /// 帧编号
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// 预计响应帧长度
            /// </summary>
            public int RespLen { get; set; }
            /// <summary>
            /// 该帧PFC
            /// </summary>
            public int PFC { get; set; }
            /// <summary>
            /// 组装的下行规约帧内容
            /// </summary>
            public byte[] DownData { get; set; }
            /// <summary>
            /// 应答的上行规约帧内容
            /// </summary>
            public byte[] UpData { get; set; }
            /// <summary>
            /// 是否已经发送
            /// </summary>
            public bool IsSent { get; set; }
            /// <summary>
            /// 是否已经超时
            /// 如果超过等待时间没有回码则认为超时
            /// </summary>
            public bool IsTimeout { get; set; }
            /// <summary>
            /// 重发次数。
            /// 当Retry=-1时，停止发送。不需要重发则Retry=0
            /// </summary>
            public int Retry { get; set; }
            /// <summary>
            /// 是否有应答帧
            /// 返回帧SEQ相等则认为是应答帧
            /// </summary>
            public bool IsAnswer { get; set; }
            /// <summary>
            /// 是否处理结束
            /// </summary>
            public bool IsEnd { get; set; }
            /// <summary>
            /// 上行帧类型
            /// </summary>
            public UpFrameTypeOption UpFrameType { get; set; }
            /// <summary>
            /// 帧中包含的数据项
            /// </summary>
            private List<DataUnitBase> unitList = new List<DataUnitBase>();
            /// <summary>
            /// 增加一个数据项
            /// </summary>
            /// <param name="unit"></param>
            public void AddDataUnit(DataUnitBase unit)
            {
                if (!ContainsDataUnit(unit))
                    unitList.Add(unit);
            }
            /// <summary>
            /// 帧中是否包含指定的数据项
            /// </summary>
            /// <param name="unit"></param>
            /// <returns></returns>
            public bool ContainsDataUnit(DataUnitBase unit)
            {
                foreach (DataUnitBase du in unitList)
                {
                    if(du.Equals(unit))
                        return true;
                }
                return false;
            }
            /// <summary>
            /// 将规约帧以字符串方式输出
            /// </summary>
            /// <param name="blocksPerLine">每行块数（每块8字节）</param>
            /// <param name="seperator">分隔符</param>
            /// <returns></returns>
            public string toString(int blocksPerLine = 4, string seperator = "\r\n")
            {
                string str = "";
                if (DownData != null)
                {
                    str = "下行：\r\n";
                    for (int i = 0; i < DownData.Length; i++)
                    {
                        if (i > 0)
                        {
                            if (i % 8 > 0)
                                str += " ";	//非块结束，非行结束，空格相隔
                            else if (i % (8 * blocksPerLine) > 0)
                                str += "-";	//块结束，'-'相隔
                            else
                                str += seperator;	//行结束，seperator相隔
                        }
                        str += DownData[i].ToString("X2");
                    }
                    str += "\r\n";
                }
                if (UpData != null)
                {
                    str += "上行：\r\n";
                    for (int i = 0; i < UpData.Length; i++)
                    {
                        if (i > 0)
                        {
                            if (i % 8 > 0)
                                str += " ";	//非块结束，非行结束，空格相隔
                            else if (i % (8 * blocksPerLine) > 0)
                                str += "-";	//块结束，'-'相隔
                            else
                                str += seperator;	//行结束，seperator相隔
                        }
                        str += UpData[i].ToString("X2");
                    }
                }
                return str;
            }
        }
        /// <summary>
        /// 下行规约帧集合
        /// </summary>
        public List<Frame> FrameList = new List<Frame>();
        /// <summary>
        /// 终端地址
        /// </summary>
        public int AddrCode { get; set; }
        /// <summary>
        /// 行政区码
        /// </summary>
        public int AreaCode { get; set; }
        /// <summary>
        /// 地址类型
        /// </summary>
        public AddrTypeOption AddrType { get; set; }
        /// <summary>
        /// 最大响应帧长度
        /// </summary>
        public int MaxRespLen { get; set; }
        /// <summary>
        /// 每帧最大数据单元个数
        /// </summary>
        public int MaxDataUnitCount { get; set; }
        /// <summary>
        /// 是否支持PFC
        /// </summary>
        public bool IsHavePFC { get; set; }
        /// <summary>
        /// 终端当前正在使用的密码算法
        /// </summary>
        public int PwdId { get; set; }
        /// <summary>
        /// 终端当前正在使用的密钥
        /// </summary>
        public int PwdKey { get; set; }
        /// <summary>
        /// 获取终端的PFC
        /// </summary>
        /// <returns></returns>
        public int GetPFC()
        {
            return 5;
        }
        /// <summary>
        /// 获取主站地址
        /// </summary>
        /// <returns></returns>
        public byte GetMSA()
        {
            return 1;
        }
        #endregion

        #region 解帧处理
        /// <summary>
        /// 解帧处理。
        /// 对于阻塞和非阻塞处理，调用者保证收到的帧就是需要的帧
        /// 对于主动上报，frame=null
        /// </summary>
        /// <param name="data">设备返回的完整规约帧</param>
        /// <param name="data">阻塞和非阻塞处理时对应的下行帧，主动上报帧为null</param>
        /// <returns>如果是数据帧，解帧成功返回数据项的List，如果是确认、否认帧解帧成功，返回List为空，解帧失败返回NULL</returns>
        public List<DataUnitBase> Uncompose(byte[] data, Frame frame=null)
        {
            bool isReport = (data[11] == 0);//是否主动上报
            DecodeArea = Bcd2Dec(data[7]) + Bcd2Dec(data[8]) * 100;
            DecodeAddr = (int)Byte2Long(data, 9, 2);
            if (!isReport && (DecodeAddr != AddrCode || DecodeArea != DecodeArea))
            {//非本终端地址
                return null;
            }
            if (!isReport && frame!=null)
            {
                frame.IsAnswer = true;
                frame.IsEnd = true;
            }
            List<DataUnitBase> unitList = new List<DataUnitBase>();
            int length = (data[1] + data[2] * 256) >> 2;
            int dataLen = length - 8;
            if ((data[6] & 0x20) > 0)    //控制码
            {//有事件计数器
                dataLen -= 2;
                //在此进行事件计数器的处理

            }
            if ((data[13] & 0x80) > 0) //SEQ
            {//有Tp
                dataLen -= 6;
            }
            AFNOption AFN = (AFNOption)data[12];
            if (AFN == AFNOption.AFN_YESNO)
            {
                if (frame == null)//没有需要应答的帧
                    return null;
                if ((data[16] & 0x01) > 0)
                {//确认帧
                    frame.UpFrameType = UpFrameTypeOption.ACK;
                }
                else if ((data[16] & 0x02) > 0)
                {//否认帧
                    frame.UpFrameType = UpFrameTypeOption.DENY;
                }
                else
                {//部分确认、否认
                    return null;   //不支持部分确认、否认
                }
                return unitList;
            }
            //数据处理
            if (frame != null)
            {
                frame.UpFrameType = UpFrameTypeOption.DATA;
            }
            int offset = 14;
            int dealLen = 0;//已经处理的长度
            while (dealLen < dataLen)
            {
                int[] Pn;
                int[] Fn;
                GetPnFn(data, offset, out Pn, out Fn);
                offset += 4;
                dealLen += 4;
                if (Fn == null)
                    continue;
                for (int i = 0; i < Pn.Length; i++)
                {
                    for (int j = 0; j < Fn.Length; j++)
                    {
                        DataUnitBase unit = CreateDataUnit(AFN, Fn[j]);
                        if (unit != null)
                        {
                            int len = GetFnDataLen(AFN, unit, data, offset);
                            if (AFN == AFNOption.AFN_CLS_THREE_QUERY)
                            {//事件召测，数据长度就是F项长度
                                len = dataLen - 4;
                            }
                            if (dealLen + len > dataLen)
                            {//数据长度不对，无法继续解帧
                                return null;
                            }
                            byte[] tmp = new byte[len];
                            System.Array.ConstrainedCopy(data, offset, tmp, 0, len);
                            offset += len;
                            dealLen += len;
                            unit.Pn = Pn[i];
                            unit.Data = tmp;
                            //解码处理
                            unit.Decode();
                            unitList.Add(unit);
                        }
                        else
                        {//没有定义对该数据项的处理
                            return null;
                        }
                    }
                }
            }
            return unitList;
        }
        /// <summary>
        /// 根据数据单元标识DA、DT获取Pn、Fn
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="Pn"></param>
        /// <param name="Fn"></param>
        protected abstract void GetPnFn(byte[] data, int start, out int[] Pn, out int[] Fn);
        /// <summary>
        /// 获取指定F项对应的数据长度
        /// </summary>
        /// <param name="AFN"></param>
        /// <param name="Fn"></param>
        /// <returns>数据长度</returns>
        protected abstract int GetFnDataLen(AFNOption AFN, DataUnitBase dataUnit, byte[] data, int start);
        /// <summary>
        /// 上行解帧后的数据单元集合
        /// </summary>
        private Hashtable UpUnitCol = new Hashtable();
        /// <summary>
        /// 向上行解帧数据单元增加数据单元
        /// </summary>
        /// <param name="AFN"></param>
        /// <param name="dataUnit"></param>
        private void AddUpDataUnit(AFNOption AFN, DataUnitBase dataUnit)
        {
            List<DataUnitBase> list = null;
            if (UpUnitCol.ContainsKey(AFN))
            {
                list = (List<DataUnitBase>)UpUnitCol[AFN];
            }
            else
            {
                list = new List<DataUnitBase>();
                UpUnitCol.Add(AFN, list);
            }
            list.Add(dataUnit);
        }
        /// <summary>
        /// 根据AFN、Pn、Fn查找对应的数据单元
        /// </summary>
        /// <param name="AFN"></param>
        /// <param name="Pn"></param>
        /// <param name="Fn"></param>
        /// <returns>找到返回对应数据单元，找不到返回null</returns>
        protected DataUnitBase FindUpDataUnit(AFNOption AFN, DataUnitBase dataUnit)
        {
            bool bFind = false;
            List<DataUnitBase> resultList = new List<DataUnitBase>();
            DataUnitBase expUnit = CreateDataUnit(AFN, dataUnit.Fn);//异常时使用的数据单元
            if (expUnit == null)
                return null;
            foreach (Frame frame in FrameList)
            {
                if (frame.ContainsDataUnit(dataUnit))
                {
                    bFind = true;
                    if (!frame.IsAnswer)
                    {
                        expUnit.OperResult = OperResultOption.NoAnswer;
                        expUnit.OperResultDesc = "无回码";
                        resultList.Add(expUnit);
                    }
                    if (frame.UpFrameType == UpFrameTypeOption.ACK || frame.UpFrameType == UpFrameTypeOption.DENY)
                    {
                        expUnit.Pn = dataUnit.Pn;
                        expUnit.OperResult = frame.UpFrameType == UpFrameTypeOption.ACK ? OperResultOption.Success : OperResultOption.Deny;
                        if (frame.UpFrameType == UpFrameTypeOption.DENY)
                            expUnit.OperResultDesc = "否认";
                        resultList.Add(expUnit);
                    }
                }
            }
            if (!bFind)
            {//组帧中没有该Pn、Fn
                return null;
            }
            if (UpUnitCol.ContainsKey(AFN))
            {
                List<DataUnitBase> unitCol = (List<DataUnitBase>)UpUnitCol[AFN];
                foreach (DataUnitBase unit in unitCol)
                {
                    if (unit.Equals(dataUnit))
                    {
                        resultList.Add(unit);
                    }
                }
            }
            if (resultList.Count > 0)
            {
                DataUnitBase unit = resultList[0];
                unit.SubRelation = dataUnit.SubRelation;
                for (int i = 1; i < resultList.Count; i++)
                {
                    unit.Merge(resultList[i]);
                }
                return unit;
            }
            else
                return null;
        }
        /// <summary>
        /// 解帧得到的行政区码
        /// </summary>
        public int? DecodeArea { get; set; }
        /// <summary>
        /// 解帧得到的终端地址
        /// </summary>
        public int? DecodeAddr { get; set; }
        #endregion //解帧处理

        #region 通信对象
        public SerialPort comm = new SerialPort("COM7", 1200, Parity.Even, 8, StopBits.One);
        #endregion

        public DateTime CallClock()
        {
            throw new NotImplementedException();
        }

        public override byte[] SendData(IDrive childrenDrive, byte[] sendData, int delay)
        {
            throw new NotImplementedException();
        }

        public override byte[] ReceiveData(byte[] receiveData)
        {
            throw new NotImplementedException();
        }

        public int Address { get; set; }

        public byte[] GetAddress()
        {
            throw new NotImplementedException();
        }

        public void SetAddress(byte[] address)
        {
            throw new NotImplementedException();
        }
    }
}
