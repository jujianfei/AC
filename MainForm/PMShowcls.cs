using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MainForm
{
    public class PMShowScls
    {
        public PMShowcls[] pmshows = new PMShowcls[8];

        public PMShowScls()
        {
            for (int i = 0; i < pmshows.Length; i++)
            {
                pmshows[i] = new PMShowcls();
                pmshows[i].address = (byte)(i + 1);
            }
        }
    }

    public class PMShowcls
    {
        public byte address = 0;  //位地址
        public Channel server = null;
        private void OnMessage(ChannelMessageTypeOptions messageType, string message)
        {
            if (server != null)
                server.OnMessage(messageType, message);
        }

        static public byte[] cmdbg = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdbg(byte bgstate)
        {
            byte[] r = new byte[cmdbg.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdbg[i];
            }
            r[4] = address;
            r[6] = bgstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_bgkSend(TCPClientChannelargsUserToken socketc, out byte[] sendbytes, out byte[] receivebytes)
        {
            sendbytes = null;
            receivebytes = null;
            try
            {
                sendbytes = getcmdbg(1);
                server.ONLYSendToMoxa(socketc, sendbytes);
                return true;
            }
            catch (Exception ex)
            {
                //OnMessage(MessageTypeOptions.SystemException, ("地址：" + address + "表位，屏幕被光开失败！" + ex.Message));
            }
            return false;
        }
        public bool cmd_bggSend(TCPClientChannelargsUserToken socketc, out byte[] sendbytes, out byte[] receivebytes)
        {
            sendbytes = null;
            receivebytes = null;
            try
            {
                sendbytes = getcmdbg(2);
                server.ONLYSendToMoxa(socketc, sendbytes);
                return true;
            }
            catch (Exception ex)
            {
                //OnMessage(MessageTypeOptions.SystemException, ("地址：" + address + "表位，屏幕被光关失败！" + ex.Message));
            }
            return false;
        }

        static public byte[] cmdclear = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdclear()
        {
            byte[] r = new byte[cmdclear.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdclear[i];
            }
            r[4] = address;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_clearkSend(TCPClientChannelargsUserToken socketc, out byte[] sendbytes, out byte[] receivebytes)
        {
            sendbytes = null;
            receivebytes = null;
            try
            {
                sendbytes = getcmdclear();
                server.ONLYSendToMoxa(socketc, sendbytes);
                return true;
            }
            catch (Exception ex)
            {
                //OnMessage(MessageTypeOptions.SystemException, ("地址：" + address + "表位，屏幕清屏失败！" + ex.Message));
            }
            return false;
        }

        static public byte[] cmdshow1 = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdshow1(byte[] showdatas)
        {
            byte[] r = new byte[cmdshow1.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdshow1[i];
            }
            r[4] = address;

            r[6] = showdatas[0];
            r[7] = showdatas[1];
            r[8] = showdatas[2];
            r[9] = showdatas[3];
            r[10] = showdatas[4];
            r[11] = showdatas[5];

            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        static public byte[] cmdshow2 = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdshow2(byte[] showdatas)
        {
            byte[] r = new byte[cmdshow2.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdshow2[i];
            }
            r[4] = address;

            r[6] = showdatas[0];
            r[7] = showdatas[1];
            r[8] = showdatas[2];
            r[9] = showdatas[3];
            r[10] = showdatas[4];
            r[11] = showdatas[5];

            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_showSend(TCPClientChannelargsUserToken socketc, string strdata, out byte[] sendbytes1, out byte[] receivebytes1, out byte[] sendbytes2, out byte[] receivebytes2)
        {
            sendbytes1 = null;
            receivebytes1 = null;
            sendbytes2 = null;
            receivebytes2 = null;
            try
            {
                 
                Encoding gb2312 = Encoding.GetEncoding("gb2312");
                byte[] datas;
                byte[] datas1 = gb2312.GetBytes("　　　");
                byte[] datas2 = gb2312.GetBytes("　　　");

                datas = gb2312.GetBytes(strdata);

                for (int i = 0; i < datas.Length; i++)
                {
                    if (i < datas1.Length)
                    {
                        datas1[i] = datas[i];
                    }
                    else if (i < (datas1.Length + datas2.Length))
                    {
                        datas2[i - datas1.Length] = datas[i];
                    }
                    else
                    {
                        break;
                    }
                }
                sendbytes1 = getcmdshow1(datas1);
                server.ONLYSendToMoxa(socketc, sendbytes1);
                Thread.Sleep(500);
                sendbytes2 = getcmdshow2(datas2);
                server.ONLYSendToMoxa(socketc, sendbytes2);
                return true;
            }
            catch (Exception ex)
            {
                //OnMessage(MessageTypeOptions.SystemException, ("地址：" + address + "表位，屏幕输入设置失败！" + ex.Message));
            }
            return false;
        }
    }
}
