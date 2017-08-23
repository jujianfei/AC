using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.DLT645
{
    /// <summary>
    /// 使用 DL/T 645-1997 电力行业标准通信规约的设备驱动。
    /// </summary>
    public class DLT645_1997Drive : DLT645Drive
    {
        /// <summary>
        /// 读取电表指定数据项的数据。
        /// </summary>
        /// <param name="dataItem">数据项编码。例如：9010、901F、C010、C011、C032等</param>
        /// <returns>读取的该数据项的数据。（已减33H）</returns>
        public byte[] ReadData(string dataItem)
        {
            return this.ReadData(Function.ToBytes(dataItem));
        }

        /// <summary>
        /// 读取电表指定数据项的数据。
        /// </summary>
        /// <param name="dataItem">数据项编码，2字节。例如：901F可以表示为 byte[]{0x90,0x1F}</param>
        /// <returns>读取的该数据项的数据。（已减33H）</returns>
        public byte[] ReadData(byte[] dataItem)
        {
            byte[] bytData = this.SendData(DLT645_1997ControlCodeOptions.ReadData, Function.BytesReversal(dataItem), 0);
            if (bytData.Length >= 2)
            {
                byte[] bytTemp = new byte[bytData.Length - 2];

                for (int intIndex = 2; intIndex < bytData.Length; intIndex++)
                {
                    bytTemp[intIndex - 2] = bytData[intIndex];
                }
                return bytTemp;
            }
            else
            {
                throw new Exception("电表返回的数据域长度不足");
            }
        }

        /// <summary>
        /// 向电表发送数据。所发送的数据为规约文档所描述帧结构中的“数据域”部分，且无需加0x33。
        /// </summary>
        /// <param name="controlCode"></param>
        /// <param name="sendData">需要发送的数据。该数据为规约文档所描述帧结构中的“数据域”部分，且无需加0x33。</param>
        /// <param name="delay"></param>
        /// <returns>返回的数据。该数据为规约文档所描述帧结构中的“数据域”部分，且已经过减0x33处理。</returns>
        public byte[] SendData(DLT645_1997ControlCodeOptions controlCode, byte[] sendData, int delay)
        {
            int intIndex = 0;
            byte[] bytData = new byte[sendData.Length + 12 + 4];

            //前导字节
            bytData[intIndex++] = 0xFE;
            bytData[intIndex++] = 0xFE;
            bytData[intIndex++] = 0xFE;
            bytData[intIndex++] = 0xFE;

            int intCSStartIndex = intIndex;
            byte bytCS = 0;

            //帧起始符
            bytData[intIndex++] = 0x68;

            //地址域
            bool bolIsBroadcastAddress = (this.Address == 0);
            switch (controlCode)
            {
                case DLT645_1997ControlCodeOptions.ProofreadTime:
                case DLT645_1997ControlCodeOptions.WriteAddress:
                    bolIsBroadcastAddress = true;
                    break;

                default:
                    if (bolIsBroadcastAddress == false)
                    {
                        long intAddrMod = 1;
                        for (int iAddr = 0; iAddr < 6; iAddr++)
                        {
                            if ((base.Address / intAddrMod) > 0)
                            {
                                byte bytAddr = (byte)((base.Address / intAddrMod) % 100);
                                bytData[intIndex++] = Function.ByteToBCD(bytAddr);
                            }
                            else
                            {
                                bytData[intIndex++] = 0x00;
                            }
                            intAddrMod *= 100;
                        }
                    }
                    break;
            }

            if (bolIsBroadcastAddress)
            {
                bytData[intIndex++] = 0x99;
                bytData[intIndex++] = 0x99;
                bytData[intIndex++] = 0x99;
                bytData[intIndex++] = 0x99;
                bytData[intIndex++] = 0x99;
                bytData[intIndex++] = 0x99;
            }

            //帧起始符
            bytData[intIndex++] = 0x68;

            //控制码
            bytData[intIndex++] = (byte)controlCode;

            //数据长度
            bytData[intIndex++] = (byte)sendData.Length;

            for (int i = 0; i < sendData.Length; i++)
            {
                bytData[intIndex++] = (byte)(sendData[i] + 0x33);
            }

            //校验和
            for (int i = intCSStartIndex; i < intIndex; i++)
            {
                bytCS += bytData[i];
            }
            bytData[intIndex++] = bytCS;

            //结束字符
            bytData[intIndex++] = 0x16;

            base.OnSendingData(this, bytData);

            if (delay >= 0)
            {
                bytData = base.Parent.SendData(this, bytData, delay);

                int intReceiveStartIndex = -1;
                for (int i = 0; i < bytData.Length; i++)
                {
                    if (bytData[i] == (byte)0x68)
                    {
                        intReceiveStartIndex = i; // 查找0x68
                        break;
                    }
                }

                if (intReceiveStartIndex == -1)
                {
                    throw new Exception("返回的数据(" + Function.OutBytes(bytData) + ")无帧起始符0x68");
                }

                int intReceiveDataLength = bytData[intReceiveStartIndex + 9]; // 返回的数据长度

                int intReceiveCheckSum = 0;
                for (int i = intReceiveStartIndex + 0; i <= intReceiveStartIndex + 9 + intReceiveDataLength; i++)
                {
                    intReceiveCheckSum += bytData[i];
                }

                if ((byte)intReceiveCheckSum != bytData[intReceiveStartIndex + 9 + intReceiveDataLength + 1])
                {
                    throw new Exception("返回的数据(" + Function.OutBytes(bytData) + ")校验错误");
                }

                byte bytReceiveControl = bytData[intReceiveStartIndex + 8]; // 控制码

                switch (controlCode)
                {
                    case DLT645_1997ControlCodeOptions.ReadData:
                        if (bytReceiveControl == (byte)0xC1)
                        {
                            throw new Exception("电表应答异常：" + this.GetErrorContext((byte)(bytData[intReceiveStartIndex + 10] - 0x33)));
                        }
                        break;
                }

                byte[] bytReceiveData = new byte[intReceiveDataLength];

                for (int intDataIndex = 0; intDataIndex < intReceiveDataLength; intDataIndex++)
                {
                    bytReceiveData[intDataIndex] = (byte)(bytData[intReceiveStartIndex + 10 + intDataIndex] - 0x33);
                }

                return bytReceiveData;
            }
            else
            {
                return null;
            }
        }

        private string GetErrorContext(byte err)
        {
            string strError = "";

            if ((err & 0x01) == 0x01)
            {
                strError += "、非法数据";
            }

            if ((err & 0x02) == 0x02)
            {
                strError += "、数据标识错误";
            }

            if ((err & 0x04) == 0x04)
            {
                strError += "、密码错";
            }

            if ((err & 0x10) == 0x10)
            {
                strError += "、年时区数超";
            }
            if ((err & 0x20) == 0x20)
            {
                strError += "、日时段数超";
            }

            if ((err & 0x40) == 0x40)
            {
                strError += "、费率数超";
            }

            if (strError.Length > 0)
            {
                strError = strError.Substring(1);
            }
            return "错误码" + err.ToString("X2") + "，" + strError;
        }

    }
}
