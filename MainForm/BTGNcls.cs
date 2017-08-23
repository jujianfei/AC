using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml;
using System.Net;

namespace MainForm
{
    public enum DeviceTpye
    {
        JZQ1 = 0,
        JZQ2 = 1,
        CJQ1 = 2,
        CJQ2 = 3,
        ZB1 = 4,
        ZB3 = 5,
        DB = 6,
    }
    public enum LinkType
    {
        LT232 = 0,
        LT485 = 1,
        LTLan = 2
    }
    //表台
    public class BTGNCScls : AC.Base.ISystemConfig
    {
        public bool[] btSelIS = new bool[8];
        public BTGNcls[] btgns = new BTGNcls[8];

        public static DeviceTpye GetDeviceTpye(string valuestr)
        {
            if (valuestr == "JZQ1")
            {
                return DeviceTpye.JZQ1;
            }
            else if (valuestr == "JZQ2")
            {
                return DeviceTpye.JZQ2;
            }
            else if (valuestr == "CJQ1")
            {
                return DeviceTpye.CJQ1;
            }
            else if (valuestr == "CJQ2")
            {
                return DeviceTpye.CJQ2;
            }
            else if (valuestr == "ZB1")
            {
                return DeviceTpye.ZB1;
            }
            else if (valuestr == "ZB3")
            {
                return DeviceTpye.ZB3;
            }
            else
            {
                return DeviceTpye.JZQ1;
            }
        }
        public static LinkType GetLinkType(string valuestr)
        {
            if (valuestr == "LT232")
            {
                return LinkType.LT232;
            }
            else if (valuestr == "LT485")
            {
                return LinkType.LT485;
            }
            else if (valuestr == "LTLan")
            {
                return LinkType.LTLan;
            }
            else
            {
                return LinkType.LT232;
            }
        }

        public BTGNCScls()
        {
            for (int i = 0; i < btgns.Length; i++)
            {
                btSelIS[i] = false;
                btgns[i] = new BTGNcls();
                btgns[i].djaddress = (byte)(i + 1);
                btgns[i].bzaddress = btgns[i].djaddress;
            }
        }

        #region ISystemConfig实现
        public void SetConfig(System.Xml.XmlNode serverConfig)
        {
            for (int i = 0; i < serverConfig.ChildNodes.Count; i++)
            {
                if (serverConfig.ChildNodes[i].Name.Equals("BTGN" + (i + 1).ToString()))
                {
                    for (int j = 0; j < serverConfig.ChildNodes[i].ChildNodes.Count; j++)
                    {
                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("DJaddress"))
                        {
                            this.btgns[i].djaddress = Convert.ToByte(serverConfig.ChildNodes[i].ChildNodes[j].InnerText);
                        }
                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("BZaddress"))
                        {
                            this.btgns[i].bzaddress = Convert.ToByte(serverConfig.ChildNodes[i].ChildNodes[j].InnerText);
                        }
                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("DeviceTpye"))
                        {
                            try
                            {
                                this.btgns[i].txdevicetype = GetDeviceTpye(serverConfig.ChildNodes[i].ChildNodes[j].InnerText);
                            }
                            catch (Exception ex)
                            {
                                TXTWrite.WriteERRTxt("BTGNCScls", "SetConfig", "DeviceTpye", serverConfig.ChildNodes[i].ChildNodes[j].InnerText, ex.ToString());
                            }
                        }
                        //if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("Num485"))
                        //{
                        //    this.btgns[i].Num485 = Convert.ToInt32(serverConfig.ChildNodes[i].ChildNodes[j].InnerText);
                        //}
                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("Num232"))
                        {
                            this.btgns[i].Num232 = Convert.ToInt32(serverConfig.ChildNodes[i].ChildNodes[j].InnerText);
                        }
                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("waittime1001"))
                        {
                            this.btgns[i].waittime1001 = Convert.ToInt32(serverConfig.ChildNodes[i].ChildNodes[j].InnerText);
                        }
                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("waittime1002"))
                        {
                            this.btgns[i].waittime1002 = Convert.ToInt32(serverConfig.ChildNodes[i].ChildNodes[j].InnerText);
                        }

                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("deviceseld1"))
                        {
                            if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.DSeljzType.Z3.ToString())
                                this.btgns[i].tempdeviceseld1 = MainForm.BTGNcls.DSeljzType.Z3;
                            else if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.DSeljzType.J1.ToString())
                                this.btgns[i].tempdeviceseld1 = MainForm.BTGNcls.DSeljzType.J1;
                            else
                                this.btgns[i].tempdeviceseld1 = MainForm.BTGNcls.DSeljzType.J1;
                        }
                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("deviceseld2"))
                        {
                            if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.DSelzcType.Z1.ToString())
                                this.btgns[i].tempdeviceseld2 = MainForm.BTGNcls.DSelzcType.Z1;
                            else if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.DSelzcType.C2.ToString())
                                this.btgns[i].tempdeviceseld2 = MainForm.BTGNcls.DSelzcType.C2;
                            else
                                this.btgns[i].tempdeviceseld2 = MainForm.BTGNcls.DSelzcType.Z1;
                        }
                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("deviceseld3"))
                        {
                            if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.DSeljcType.J2.ToString())
                                this.btgns[i].tempdeviceseld3 = MainForm.BTGNcls.DSeljcType.J2;
                            else if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.DSeljcType.C1.ToString())
                                this.btgns[i].tempdeviceseld3 = MainForm.BTGNcls.DSeljcType.C1;
                            else
                                this.btgns[i].tempdeviceseld3 = MainForm.BTGNcls.DSeljcType.C1;
                        }
                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("sel4851"))
                        {
                            if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.Sel485jzType.JZ485_1.ToString())
                                this.btgns[i].tempsel4851 = MainForm.BTGNcls.Sel485jzType.JZ485_1;
                            else if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.Sel485jzType.JZ485_2.ToString())
                                this.btgns[i].tempsel4851 = MainForm.BTGNcls.Sel485jzType.JZ485_2;
                            else
                                this.btgns[i].tempsel4851 = MainForm.BTGNcls.Sel485jzType.JZ485_1;
                        }
                        if (serverConfig.ChildNodes[i].ChildNodes[j].Name.Equals("sel4853"))
                        {
                            if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.Sel485jcType.JC485_1.ToString())
                                this.btgns[i].tempsel4853 = MainForm.BTGNcls.Sel485jcType.JC485_1;
                            else if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.Sel485jcType.JC485_2.ToString())
                                this.btgns[i].tempsel4853 = MainForm.BTGNcls.Sel485jcType.JC485_2;
                            else if (serverConfig.ChildNodes[i].ChildNodes[j].InnerText == MainForm.BTGNcls.Sel485jcType.JC485_3.ToString())
                                this.btgns[i].tempsel4853 = MainForm.BTGNcls.Sel485jcType.JC485_3;
                            else
                                this.btgns[i].tempsel4853 = MainForm.BTGNcls.Sel485jcType.JC485_1;
                        }
                    }

                }
            }
        }
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xns = xmlDoc.CreateElement("BTGNCScls");
            for (int i = 0; i < btgns.Length; i++)
            {
                System.Xml.XmlNode xn = xmlDoc.CreateElement("BTGN" + (i + 1).ToString());

                XmlNode XMLdjaddress = xmlDoc.CreateElement("DJaddress");
                XMLdjaddress.InnerText = this.btgns[i].djaddress.ToString();
                xn.AppendChild(XMLdjaddress);

                XmlNode XMLbzaddress = xmlDoc.CreateElement("BZaddress");
                XMLbzaddress.InnerText = this.btgns[i].bzaddress.ToString();
                xn.AppendChild(XMLbzaddress);

                XmlNode XMLdevicetype = xmlDoc.CreateElement("DeviceTpye");
                XMLdevicetype.InnerText = this.btgns[i].txdevicetype.ToString();
                xn.AppendChild(XMLdevicetype);

                //XmlNode XMLNum485 = xmlDoc.CreateElement("Num485");
                //XMLNum485.InnerText = this.btgns[i].Num485.ToString();
                //xn.AppendChild(XMLNum485);

                XmlNode XMLNum232 = xmlDoc.CreateElement("Num232");
                XMLNum232.InnerText = this.btgns[i].Num232.ToString();
                xn.AppendChild(XMLNum232);

                XmlNode XMLwaittime1001 = xmlDoc.CreateElement("waittime1001");
                XMLwaittime1001.InnerText = this.btgns[i].waittime1001.ToString();
                xn.AppendChild(XMLwaittime1001);

                XmlNode XMLwaittime1002 = xmlDoc.CreateElement("waittime1002");
                XMLwaittime1002.InnerText = this.btgns[i].waittime1002.ToString();
                xn.AppendChild(XMLwaittime1002);

                XmlNode XMLdeviceseld1 = xmlDoc.CreateElement("deviceseld1");
                XMLdeviceseld1.InnerText = this.btgns[i].deviceseld1.ToString();
                xn.AppendChild(XMLdeviceseld1);

                XmlNode XMLdeviceseld2 = xmlDoc.CreateElement("deviceseld2");
                XMLdeviceseld2.InnerText = this.btgns[i].deviceseld2.ToString();
                xn.AppendChild(XMLdeviceseld2);

                XmlNode XMLdeviceseld3 = xmlDoc.CreateElement("deviceseld3");
                XMLdeviceseld3.InnerText = this.btgns[i].deviceseld3.ToString();
                xn.AppendChild(XMLdeviceseld3);

                XmlNode XMLsel4851 = xmlDoc.CreateElement("sel4851");
                XMLsel4851.InnerText = this.btgns[i].sel4851.ToString();
                xn.AppendChild(XMLsel4851);

                XmlNode XMLsel4853 = xmlDoc.CreateElement("sel4853");
                XMLsel4853.InnerText = this.btgns[i].sel4853.ToString();
                xn.AppendChild(XMLsel4853);

                xns.AppendChild(xn);
            }

            return xns;
        }
        #endregion

        public bool initSelDevice485(Channel _ser)
        {
            bool f = false;
            for (int i = 0; i < btgns.Length; i++)
            {
                TCPClientChannelargsUserToken lnk = _ser.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, i);
                f = btgns[i].InitSelD485Set(lnk);
                if (!f)
                    break;
            }
            return f;
        }
    }
    //表位
    public class BTGNcls
    {
        public byte djaddress = 0;  //机电位地址
        public byte bzaddress = 0;  //刘晶晶板子通信地址
        public Channel server = null;
        public byte msnum = 0;
        private void OnMessage(ChannelMessageTypeOptions messageType, string message)
        {
            if (server != null)
                server.OnMessage(messageType, message);
        }

        /// <summary>
        /// 当前通信设备  485抄表时回哪个口
        /// </summary>
        public DeviceTpye txdevicetype = DeviceTpye.JZQ1;

        public bool setDeviceLinktype(string[] _datastr)
        {
            string _com = _datastr[1];

            string _dt1 = _datastr[2];
            string _4851 = _datastr[3];

            string _dt2 = _datastr[4];
            string _4852 = _datastr[5];

            string _dt3 = _datastr[6];
            string _4853 = _datastr[7];

            if (_com == "1")
                this.Num232 = 1;
            else
                this.Num232 = 2;

            if (_dt1 == "" + Convert.ToInt32(DeviceTpye.JZQ1))
                tempdeviceseld1 = DSeljzType.J1;
            else
                tempdeviceseld1 = DSeljzType.Z3;

            if (_4851 == "1")
                tempsel4851 = Sel485jzType.JZ485_1;
            else if (_4851 == "2")
                tempsel4851 = Sel485jzType.JZ485_2;

            if (_dt2 == "" + Convert.ToInt32(DeviceTpye.ZB1))
                tempdeviceseld2 = DSelzcType.Z1;
            else
                tempdeviceseld2 = DSelzcType.C2;

            if (_dt3 == "" + Convert.ToInt32(DeviceTpye.CJQ1))
            {
                deviceseld3 = DSeljcType.C1;
                tempdeviceseld3 = DSeljcType.C1;
            }
            else if (_dt3 == "" + Convert.ToInt32(DeviceTpye.JZQ2))
            {
                deviceseld3 = DSeljcType.J2;
                tempdeviceseld3 = DSeljcType.J2;
            }
            else
            {
                deviceseld3 = DSeljcType.DB;
                tempdeviceseld3 = DSeljcType.DB;
            }
            if (_4853 == "1")
                tempsel4853 = Sel485jcType.JC485_1;
            else if (_4851 == "2")
                tempsel4853 = Sel485jcType.JC485_2;
            else if (_4851 == "3")
                tempsel4853 = Sel485jcType.JC485_3;

            return true;
        }

        public int Num232 = 1;

        public int waittime1001 = 300;
        public int waittime1002 = 150;

        public static int DataIndex = 6;

        #region 机电控制
        public enum JDKZType
        {
            Unknow = 0,
            XY = 1,
            SL = 2
        }
        public static string getJDKZType(JDKZType jdkztype)
        {
            if (jdkztype == JDKZType.XY)
            {
                return "下压";
            }
            else if (jdkztype == JDKZType.SL)
            {
                return "上拉";
            }
            else
            {
                return "";
            }
        }

        public JDKZType jdkz1state = JDKZType.Unknow;
        public JDKZType jdkz2state = JDKZType.Unknow;
        static public byte[] cmdjdkz1 = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdjdkz1(byte jdkzstate)
        {
            byte[] r = new byte[cmdjdkz1.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdjdkz1[i];
            }
            r[4] = djaddress;
            r[6] = jdkzstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_jdkz1Send(TCPClientChannelargsUserToken socketc, ExecData ed, JDKZType jdkztype)
        {
            try
            {

                if (jdkztype == JDKZType.XY)
                {
                    ed.senddatas = getcmdjdkz1(2);
                }
                else if (jdkztype == JDKZType.SL)
                {
                    ed.senddatas = getcmdjdkz1(1);
                }
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    jdkz1state = jdkztype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，电机1升降命令失败！" + ex.Message));
            }
            return false;
        }

        static public byte[] cmdjdkz2 = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdjdkz2(byte jdkzstate)
        {
            byte[] r = new byte[cmdjdkz2.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdjdkz2[i];
            }
            r[4] = djaddress;
            r[6] = jdkzstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_jdkz2Send(TCPClientChannelargsUserToken socketc, ExecData ed, JDKZType jdkztype)
        {
            try
            {

                if (jdkztype == JDKZType.XY)
                {
                    ed.senddatas = getcmdjdkz2(2);
                }
                else if (jdkztype == JDKZType.SL)
                {
                    ed.senddatas = getcmdjdkz2(1);
                }
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    jdkz2state = jdkztype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，电机2升降命令失败！" + ex.Message));
            }
            return false;
        }
        #endregion

        #region 电流是否过终端，电压是否过载波隔离器
        public enum TDDDLDYType
        {
            Unknow = 0,
            TD = 1,
            DD = 2
        }
        public TDDDLDYType tdddlstate = TDDDLDYType.Unknow;
        public TDDDLDYType tdddystate = TDDDLDYType.Unknow;
        static public byte[] cmdtdddl = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdtdddl(byte dlstate)
        {
            byte[] r = new byte[cmdtdddl.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdtdddl[i];
            }
            r[4] = bzaddress;
            r[6] = dlstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_tdddlSend(TCPClientChannelargsUserToken socketc, ExecData ed, TDDDLDYType dltype)
        {
            try
            {

                if (dltype == TDDDLDYType.TD)
                {
                    ed.senddatas = getcmdtdddl(1);
                }
                else if (dltype == TDDDLDYType.DD)
                {
                    ed.senddatas = getcmdtdddl(0);
                }
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    tdddlstate = dltype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，终端电流通断电命令失败！" + ex.Message));
            }
            return false;
        }

        static public byte[] cmdtdddy = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdtdddy(byte dystate)
        {
            byte[] r = new byte[cmdtdddy.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdtdddy[i];
            }
            r[4] = bzaddress;
            r[6] = dystate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_tdddySend(TCPClientChannelargsUserToken socketc, ExecData ed, TDDDLDYType dytype)
        {
            try
            {

                if (dytype == TDDDLDYType.TD)
                {
                    ed.senddatas = getcmdtdddy(1);
                }
                else if (dytype == TDDDLDYType.DD)
                {
                    ed.senddatas = getcmdtdddy(0);
                }
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    tdddystate = dytype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，终端载波隔离器通断命令失败！" + ex.Message));
            }
            return false;
        }

        public bool SendTDDDLup(TCPClientChannelargsUserToken socketc, ExecData ed, string[] cmddatas)
        {
            bool f = false;
            //if ((txdevicetype == DeviceTpye.JZQ1) || (txdevicetype == DeviceTpye.ZB3))
            {
                f = cmd_tdddlSend(socketc, ed, TDDDLDYType.TD);
            }
            return f;
        }
        public bool SendTDDDLdown(TCPClientChannelargsUserToken socketc, ExecData ed, string[] cmddatas)
        {
            bool f = false;
            //if ((txdevicetype == DeviceTpye.JZQ1) || (txdevicetype == DeviceTpye.ZB3))
            {
                f = cmd_tdddlSend(socketc, ed, TDDDLDYType.DD);
            }
            return f;
        }

        public bool SendTDDDYup(TCPClientChannelargsUserToken socketc, ExecData ed, string[] cmddatas)
        {
            bool f = false;
            //if ((txdevicetype == DeviceTpye.JZQ1) || (txdevicetype == DeviceTpye.ZB3))
            {
                f = cmd_tdddySend(socketc, ed, TDDDLDYType.TD);
            }
            return f;
        }
        public bool SendTDDDYdown(TCPClientChannelargsUserToken socketc, ExecData ed, string[] cmddatas)
        {
            bool f = false;
            //if ((txdevicetype == DeviceTpye.JZQ1) || (txdevicetype == DeviceTpye.ZB3))
            {
                f = cmd_tdddySend(socketc, ed, TDDDLDYType.DD);
            }
            return f;
        }
        #endregion

        #region 电流是否过电表
        public TDDDLDYType tddbdlstate = TDDDLDYType.Unknow;
        static public byte[] cmdtddbdl = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdtddbdl(byte dlstate)
        {
            byte[] r = new byte[cmdtddbdl.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdtddbdl[i];
            }
            r[4] = bzaddress;
            r[6] = dlstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_tddbdlSend(TCPClientChannelargsUserToken socketc, ExecData ed, TDDDLDYType dltype)
        {
            try
            {

                if (dltype == TDDDLDYType.TD)
                {
                    ed.senddatas = getcmdtddbdl(1);
                }
                else if (dltype == TDDDLDYType.DD)
                {
                    ed.senddatas = getcmdtddbdl(0);
                }
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    tddbdlstate = dltype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，电表电流通断电命令失败！" + ex.Message));
            }
            return false;
        }

        #endregion

        #region 通断电
        public enum TDDType
        {
            Unknow = 0,
            TD = 1,
            DD = 2
        }
        public TDDType tddjzstate = TDDType.Unknow;
        public TDDType tddc1state = TDDType.Unknow;
        public TDDType tddc2state = TDDType.Unknow;
        static public byte[] cmdtdd = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdtdd(byte tddstate, byte com)
        {
            byte[] r = new byte[cmdtdd.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdtdd[i];
            }
            r[4] = bzaddress;
            r[6] = com;
            r[7] = tddstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_tddSend(TCPClientChannelargsUserToken socketc, ExecData ed, TDDType tddtype, int com)
        {
            try
            {

                if (tddtype == TDDType.TD)
                {
                    ed.senddatas = getcmdtdd(1, (byte)com);
                }
                else if (tddtype == TDDType.DD)
                {
                    ed.senddatas = getcmdtdd(0, (byte)com);
                }
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    if (com == 1)
                    {
                        tddjzstate = tddtype;
                    }
                    else if (com == 2)
                    {
                        tddc1state = tddtype;
                    }
                    else if (com == 3)
                    {
                        tddc2state = tddtype;
                    }
                    else
                    {
                        tddjzstate = tddtype;
                        tddc1state = tddtype;
                        tddc2state = tddtype;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，电压通断电命令失败！" + ex.Message));
            }
            return false;
        }

        public bool TDDInputToValue(string[] cmddatas, out TDDType tddtype)
        {
            if (Convert.ToInt32(cmddatas[1]) == 1)
                tddtype = TDDType.TD;
            else
                tddtype = TDDType.DD;

            return true;
        }
        public bool SendTDDSet(TCPClientChannelargsUserToken socketc, ExecData ed, string[] cmddatas)
        {
            bool f = false;
            TDDType _tddtype = TDDType.Unknow;
            f = TDDInputToValue(cmddatas, out _tddtype);

            if (f)
                f = cmd_tddSend(socketc, ed, _tddtype, 0xFF);

            return f;
        }
        #endregion

        #region 摇信
        public enum YXDeviceType
        {
            J = 0,
            Z = 1,
            C = 2,
            Unkonw = 3
        }
        //8路，摇信和门节点开起，闭合情况。0是开，1是合
        public enum YXType
        {
            Unknow = 0,
            XH = 1,
            DK = 2
        }
        public YXType[] yxzjstarts = new YXType[] { YXType.Unknow, YXType.Unknow, YXType.Unknow, YXType.Unknow, YXType.Unknow };
        public YXType yxcstart = YXType.Unknow;
        //设置集中器/专变摇信和门节点
        static public byte[] cmdyxjzSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdyxjzSet(byte yxstate)
        {
            byte[] r = new byte[cmdyxjzSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdyxjzSet[i];
            }
            r[4] = bzaddress;
            r[6] = yxstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_yxjzSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, YXType[] yxtypes)
        {
            try
            {

                int yxstate = 0;
                for (int i = 0; i < yxtypes.Length; i++)
                {
                    if (yxtypes[i] == YXType.XH)
                    {
                        yxstate = yxstate + Convert.ToByte(Math.Pow(2, i));
                    }
                }
                ed.senddatas = getcmdyxjzSet((byte)(yxstate & 0xFF));
                server.SendToMoxa(socketc, ed, 15);
                if (ed.backdatas != null)
                {
                    for (int i = 0; i < yxtypes.Length; i++)
                    {
                        yxzjstarts[i] = yxtypes[i];
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，集1专3摇信状态设置命令失败！" + ex.Message));
            }
            return false;
        }
        //设置采集器摇信
        static public byte[] cmdyxcSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdyxcSet(byte yxstate)
        {
            byte[] r = new byte[cmdyxjzSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdyxjzSet[i];
            }
            r[4] = bzaddress;
            r[6] = yxstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_yxcSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, YXType yxtype)
        {
            try
            {

                byte yxstate = 0;
                if (yxtype == YXType.XH)
                {
                    yxstate = 1;
                }
                ed.senddatas = getcmdyxcSet(yxstate);
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    yxcstart = yxtype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，采1摇信状态设置命令失败！" + ex.Message));
            }
            return false;
        }
        //读取集中器/专变摇信和门节点
        static public byte[] cmdyxjzGet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdyxjzGet()
        {
            byte[] r = new byte[cmdyxjzGet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdyxjzGet[i];
            }
            r[4] = bzaddress;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_yxjzGetSend(TCPClientChannelargsUserToken socketc, ExecData ed)
        {
            bool f = false;
            try
            {

                ed.senddatas = getcmdyxjzGet();
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    f = true;
                    if (ed.backdatas.Length >= ed.senddatas.Length)
                    {
                        for (int i = 0; i < DataIndex; i++)
                        {
                            if (ed.backdatas[i] != ed.senddatas[i])
                            {
                                f = false;
                                break;
                            }
                        }
                        if (f)
                        {
                            byte xyjzstart = ed.backdatas[DataIndex];
                            for (int i = 0; i < yxzjstarts.Length; i++)
                            {
                                if ((xyjzstart & Convert.ToByte(Math.Pow(2, i))) > 0)
                                {
                                    yxzjstarts[i] = YXType.DK;
                                }
                                else
                                {
                                    yxzjstarts[i] = YXType.XH;
                                }
                            }
                        }
                    }
                    else
                    {
                        f = false;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!f)
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，集1专3摇信状态招测命令失败！" + ex.Message));
                }
                else
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，解析集1专3摇信状态招测结果失败！" + ex.Message));
                }
            }
            return f;
        }
        //读取采集器摇信
        static public byte[] cmdyxcGet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdyxcGet()
        {
            byte[] r = new byte[cmdyxcGet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdyxcGet[i];
            }
            r[4] = bzaddress;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_yxcGetSend(TCPClientChannelargsUserToken socketc, ExecData ed)
        {
            bool f = false;
            try
            {

                ed.senddatas = getcmdyxcGet();
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    f = true;
                    if (ed.backdatas.Length >= ed.senddatas.Length)
                    {
                        for (int i = 0; i < DataIndex; i++)
                        {
                            if (ed.backdatas[i] != ed.senddatas[i])
                            {
                                f = false;
                                break;
                            }
                        }
                        if (f)
                        {
                            byte xyjzstart = ed.backdatas[DataIndex];
                            if ((xyjzstart & Convert.ToByte(Math.Pow(2, 0))) > 0)
                            {
                                yxcstart = YXType.XH;
                            }
                            else
                            {
                                yxcstart = YXType.DK;
                            }
                        }
                    }
                    else
                    {
                        f = false;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!f)
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，采1摇信状态招测命令失败！" + ex.Message));
                }
                else
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，解析采1摇信状态招测结果失败！" + ex.Message));
                }
            }
            return f;
        }

        //解析设置报文
        public bool XYCInputToValue(string[] cmddatas, ref YXType[] zjstarts, ref YXType cstart)
        {
            byte data = Convert.ToByte(cmddatas[1], 16);
            //if ((txdevicetype == DeviceTpye.JZQ1) || (txdevicetype == DeviceTpye.ZB3))
            //{
            for (int i = 0; i < 4; i++)
            {
                if ((data & Convert.ToByte(Math.Pow(2, i))) > 0)
                {
                    zjstarts[i] = YXType.DK;
                }
                else
                {
                    zjstarts[i] = YXType.XH;
                }
            }
            zjstarts[4] = YXType.DK;
            //}
            //else
            //{
            //    if ((data & Convert.ToByte(Math.Pow(2,0))) > 0)
            //    {
            //        cstart = YXType.XH;
            //    }
            //    else
            //    {
            //        cstart = YXType.DK;
            //    }
            //}
            return true;
        }
        //生成回复报文
        public string XYCValueToOutput(string cmdret)
        {
            string datastr = "cmd=" + cmdret + ",ret=0," + "data=";
            int xydata = 0;
            //if ((txdevicetype == DeviceTpye.JZQ1) || (txdevicetype == DeviceTpye.ZB3))
            //{
            for (int i = 0; i < 4; i++)
            {
                if (yxzjstarts[i] == YXType.XH)
                {
                    xydata = xydata + Convert.ToByte(Math.Pow(2, i));
                }
            }
            //}
            //else
            //{
            //    if (yxcstart == YXType.XH)
            //    {
            //        xydata = xydata + Convert.ToByte(Math.Pow(2,0));
            //    }
            //}
            datastr = datastr + ((byte)xydata).ToString("X2");
            return datastr;
        }

        public bool SendXYCSet(TCPClientChannelargsUserToken socketc, ExecData ed, string[] cmddatas)
        {
            YXType[] zjstarts = new YXType[yxzjstarts.Length];
            YXType cstart = YXType.Unknow;
            XYCInputToValue(cmddatas, ref zjstarts, ref cstart);
            bool f = false;
            //if (kind == 0)
            //{
            f = cmd_yxjzSetSend(socketc, ed, zjstarts);
            //}
            //else
            //{
            //    f = cmd_yxcSetSend(socketc, ed, cstart);
            //}

            return f;
        }
        public bool SendXYCGet(TCPClientChannelargsUserToken socketc, ExecData ed, string[] cmddatas)
        {
            YXType[] zjstarts = new YXType[yxzjstarts.Length];
            bool f = false;
            //if (kind == 0)
            //{
            f = cmd_yxjzGetSend(socketc, ed);
            //}
            //else
            //{
            //    f = cmd_yxcGetSend(socketc, ed);
            //}
            return f;
        }
        #endregion

        #region 表位设备和接口选择
        public enum SelD485CMDType
        {
            DSELJZ = 0,
            DSELZC = 1,
            SEL485JZ = 2,
            SEL485JC = 3
        }

        //位置上测设备是什么 集中器1还是转变3
        public enum DSeljzType
        {
            J1 = 0,
            Z3 = 1,
            UnKnow = 2
        }
        public DSeljzType deviceseld1 = DSeljzType.UnKnow;
        public DSeljzType tempdeviceseld1 = DSeljzType.J1;
        //485给那个设备用的 转变1还是采集器2
        public enum DSelzcType
        {
            Z1 = 0,
            C2 = 1,
            UnKnow = 2
        }
        public DSelzcType deviceseld2 = DSelzcType.UnKnow;
        public DSelzcType tempdeviceseld2 = DSelzcType.Z1;
        public enum DSeljcType
        {
            J2 = 0,
            C1 = 1,
            DB = 2,
            UnKnow = 3
        }
        public DSeljcType deviceseld3 = DSeljcType.UnKnow;
        public DSeljcType tempdeviceseld3 = DSeljcType.C1;
        //集中器1/转变3 位置上用的是那个485口
        public enum Sel485jzType
        {
            JZ485_1 = 0,
            JZ485_2 = 1,
            UnKnow = 2,
        }
        public Sel485jzType sel4851 = Sel485jzType.UnKnow;
        public Sel485jzType tempsel4851 = Sel485jzType.JZ485_1;
        //集中器2/采集器1 位置上用的是那个485口
        public enum Sel485jcType
        {
            JC485_1 = 0,
            JC485_2 = 1,
            JC485_3 = 2,
            UnKnow = 3
        }
        public Sel485jcType sel4853 = Sel485jcType.UnKnow;
        public Sel485jcType tempsel4853 = Sel485jcType.JC485_1;

        public bool InitSelD485Set(TCPClientChannelargsUserToken socketc)
        {
            bool f = false;
            ExecData ed = new ExecData(socketc);
            //if (deviceseld1 != DSeljzType.UnKnow)
            //    f = cmd_DSeljzSetSend(socketc, ed, deviceseld1);
            //else
            f = cmd_DSeljzSetSend(socketc, ed, tempdeviceseld1);
            if (f)
            {
                ed = new ExecData(socketc);
                //if (sel4851 != Sel485jzType.UnKnow)
                //    f = cmd_Sel485jzSetSend(socketc, ed, sel4851);
                //else
                f = cmd_Sel485jzSetSend(socketc, ed, tempsel4851);
            }
            if (f)
            {
                ed = new ExecData(socketc);
                //if (deviceseld2 != DSelzcType.UnKnow)
                //    f = cmd_DSelzcSetSend(socketc, ed, deviceseld2);
                //else
                f = cmd_DSelzcSetSend(socketc, ed, tempdeviceseld2);
            }
            if (f)
            {
                ed = new ExecData(socketc);
                //if (sel4853 != Sel485jcType.UnKnow)
                //{
                //    f = cmd_Sel485jcSetSend(socketc, ed, sel4853);
                //}
                //else
                //{
                deviceseld3 = tempdeviceseld3;
                f = cmd_Sel485jcSetSend(socketc, ed, tempsel4853);
                //}
            }
            return f;
        }
        public static string getDSeljzType(DSeljzType dseljztype)
        {
            if (dseljztype == DSeljzType.J1)
            {
                return "集中器1型";
            }
            else if (dseljztype == DSeljzType.Z3)
            {
                return "专变3型";
            }
            else
            {
                return "";
            }
        }
        public static string getDSelzcType(DSelzcType dselzctype)
        {
            if (dselzctype == DSelzcType.Z1)
            {
                return "专变1型";
            }
            else if (dselzctype == DSelzcType.C2)
            {
                return "采集器2型";
            }
            else
            {
                return "";
            }
        }
        public static string getDSeljcType(DSeljcType dseljctype)
        {
            if (dseljctype == DSeljcType.J2)
            {
                return "集中器2型";
            }
            else if (dseljctype == DSeljcType.C1)
            {
                return "采集器1型";
            }
            else if (dseljctype == DSeljcType.DB)
            {
                return "电表";
            }
            else
            {
                return "";
            }
        }
        public static string getSel485jzType(Sel485jzType sel485jztype)
        {
            if (sel485jztype == Sel485jzType.JZ485_1)
            {
                return "485Ⅰ";
            }
            else if (sel485jztype == Sel485jzType.JZ485_2)
            {
                return "485Ⅱ";
            }
            else
            {
                return "";
            }
        }
        public static string getSel485jcType(Sel485jcType sel485jctype)
        {
            if (sel485jctype == Sel485jcType.JC485_1)
            {
                return "485Ⅰ";
            }
            else if (sel485jctype == Sel485jcType.JC485_2)
            {
                return "485Ⅱ";
            }
            else if (sel485jctype == Sel485jcType.JC485_3)
            {
                return "485Ⅲ";
            }
            else
            {
                return "";
            }
        }

        public static byte[] cmdDSeljzSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getDSeljzSet(byte dseljzstate)
        {
            byte[] r = new byte[cmdDSeljzSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdDSeljzSet[i];
            }
            r[4] = bzaddress;
            r[6] = dseljzstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        object lock1 = new object();
        public bool cmd_DSeljzSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, DSeljzType dseljztype)
        {
            try
            {

                byte dselstate = 0;
                if (dseljztype == DSeljzType.J1)
                {
                    dselstate = 0;
                }
                else
                {
                    dselstate = 1;
                }
                ed.senddatas = getDSeljzSet(dselstate);
                server.SendToMoxa(socketc, ed, 15);
                if (ed.backdatas != null)
                {
                    deviceseld1 = dseljztype;
                    tempdeviceseld1 = dseljztype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, "地址：" + bzaddress + "表位，集1专3设备选择命令失败！" + ex.Message);
            }
            return false;
        }

        public static byte[] cmdDSelzcSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getDSelzcSet(byte dselzcstate)
        {
            byte[] r = new byte[cmdDSelzcSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdDSelzcSet[i];
            }
            r[4] = bzaddress;
            r[6] = dselzcstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_DSelzcSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, DSelzcType dselzctype)
        {
            try
            {

                byte dselstate = 0;
                if (dselzctype == DSelzcType.Z1)
                {
                    dselstate = 1;
                }
                else
                {
                    dselstate = 0;
                }
                ed.senddatas = getDSelzcSet(dselstate);
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    deviceseld2 = dselzctype;
                    tempdeviceseld2 = dselzctype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, "地址：" + bzaddress + "表位，专1采2设备选择命令失败！" + ex.Message);
            }
            return false;
        }

        public static byte[] cmdDSeljcSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getDSeljcSet(byte dsejccstate)
        {
            byte[] r = new byte[cmdDSeljcSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdDSeljcSet[i];
            }
            r[4] = bzaddress;
            r[6] = dsejccstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_DSeljcSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, DSeljcType dseljctype)
        {
            try
            {

                byte dselstate = 0;
                if (dseljctype == DSeljcType.DB)
                {
                    dselstate = 2;
                }
                else if (dseljctype == DSeljcType.C1)
                {
                    dselstate = 1;
                }
                else
                {
                    dselstate = 0;
                }
                ed.senddatas = getDSeljcSet(dselstate);
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    deviceseld3 = dseljctype;
                    tempdeviceseld3 = dseljctype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, "地址：" + bzaddress + "表位，专1采2设备选择命令失败！" + ex.Message);
            }
            return false;
        }

        public static byte[] cmdSel485jzSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getSel485jzSet(byte sel485jzstate)
        {
            byte[] r = new byte[cmdSel485jzSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdSel485jzSet[i];
            }
            r[4] = bzaddress;
            r[6] = sel485jzstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_Sel485jzSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, Sel485jzType sel485jztype)
        {
            try
            {

                byte sel485state = 0;
                if (sel485jztype == Sel485jzType.JZ485_1)
                {
                    sel485state = 0;
                }
                else
                {
                    sel485state = 1;
                }
                ed.senddatas = getSel485jzSet(sel485state);
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    sel4851 = sel485jztype;
                    tempsel4851 = sel485jztype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, "地址：" + bzaddress + "表位，集1专3-485接口选择命令失败！" + ex.Message);
            }
            return false;
        }

        public static byte[] cmdSel485jcSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getSel485jcSet(byte sel485jcstate)
        {
            byte[] r = new byte[cmdSel485jcSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdSel485jcSet[i];
            }
            r[4] = bzaddress;
            r[6] = sel485jcstate;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_Sel485jcSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, Sel485jcType sel485jctype)
        {
            try
            {
                byte sel485state = 0;
                if (sel485jctype == Sel485jcType.JC485_1)
                {
                    sel485state = 0;
                }
                else if (sel485jctype == Sel485jcType.JC485_2)
                {
                    sel485state = 1;
                }
                else
                {
                    sel485state = 2;
                }
                ed.senddatas = getSel485jcSet(sel485state);
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    sel4853 = sel485jctype;
                    tempsel4853 = sel485jctype;
                    return true;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("地址：" + bzaddress + "表位，集2采1-485接口选择控制失败！" + ex.Message, "设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        public bool SelD485InputToValue(string[] cmddatas, out int exitis, out int sel485is)
        {
            exitis = Convert.ToInt32(cmddatas[1]);
            sel485is = Convert.ToInt32(cmddatas[2]);
            return true;
        }
        public bool SendSelD485Set(TCPClientChannelargsUserToken socketc, ExecData ed1, ExecData ed2, ExecData ed3, ExecData ed4, ExecData ed5, string[] cmddatas)
        {
            bool f = false;
            int _exitis = 0;
            int _sel485is = 0;
            f = SelD485InputToValue(cmddatas, out _exitis, out  _sel485is);

            f = cmd_DSeljzSetSend(socketc, ed1, tempdeviceseld1);
            if (f)
            {
                f = cmd_Sel485jzSetSend(socketc, ed2, tempsel4851);
            }

            if (f)
            {
                f = cmd_DSelzcSetSend(socketc, ed3, tempdeviceseld2);
            }

            if (f)
            {
                f = cmd_DSeljcSetSend(socketc, ed4, tempdeviceseld3);
            }
            if (f)
            {
                f = cmd_Sel485jcSetSend(socketc, ed5, tempsel4853);
            }
            return f;
        }
        #endregion

        #region 脉冲（只针对集1/专1/专3位）
        public enum MCCMDType
        {
            DX = 0,
            SJ = 1,
            GS = 2,
            SE = 3
        }
        public enum MCDevicType
        {
            MC1 = 0,
            MC2 = 1,
            ALL = 2
        }
        public enum MCSETpye
        {
            UnKnow = 0,
            Start = 1,
            End = 2
        }

        public byte mcdx1 = 0;
        public long mcsj1 = 0;
        public long mcgs1 = 0;
        public MCSETpye mcse1 = MCSETpye.UnKnow;
        public byte mcdx2 = 0;
        public long mcsj2 = 0;
        public long mcgs2 = 0;
        public MCSETpye mcse2 = MCSETpye.UnKnow;

        public static byte[] cmdMCdxSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getMCdxSet(byte mctpye, byte mcdx)
        {
            byte[] r = new byte[cmdMCdxSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdMCdxSet[i];
            }
            r[4] = bzaddress;
            r[6] = mctpye;
            r[7] = mcdx;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_MCdxSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, MCDevicType MCdevictype, byte mcdx)
        {
            try
            {
                byte mctpyestate = 0;
                if (MCdevictype == MCDevicType.MC1)
                {
                    mctpyestate = 1;
                }
                else
                {
                    mctpyestate = 2;
                }
                ed.senddatas = getMCdxSet(mctpyestate, mcdx);
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    if (MCdevictype == MCDevicType.MC1)
                    {
                        this.mcdx1 = mcdx;
                    }
                    else
                    {
                        this.mcdx2 = mcdx;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, "地址：" + bzaddress + "表位，脉冲占空比设置命令失败！" + ex.Message);
            }
            return false;
        }

        public static byte[] cmdMCsjSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getMCsjSet(byte mctpye, long mcsj)
        {
            byte[] r = new byte[cmdMCsjSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdMCsjSet[i];
            }
            r[4] = bzaddress;
            byte[] tempr = BitConverter.GetBytes(mcsj);
            r[6] = tempr[3];
            r[7] = tempr[2];
            r[8] = tempr[1];
            r[9] = tempr[0];
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_MCsjSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, MCDevicType MCDevicType, long mcsj)
        {
            try
            {

                byte mctpyestate = 0;
                if (MCDevicType == MCDevicType.MC1)
                {
                    mctpyestate = 1;
                }
                else
                {
                    mctpyestate = 2;
                }
                ed.senddatas = getMCsjSet(mctpyestate, mcsj);
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    if (MCDevicType == MCDevicType.MC1)
                    {
                        this.mcsj1 = mcsj;
                    }
                    else
                    {
                        this.mcsj2 = mcsj;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, "地址：" + bzaddress + "表位，脉冲时间和个数设置命令失败！" + ex.Message);
            }
            return false;
        }

        public static byte[] cmdMCgsSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getMCgsSet(byte mctpye, long mcgs)
        {
            byte[] r = new byte[cmdMCgsSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdMCgsSet[i];
            }
            r[4] = bzaddress;
            r[6] = mctpye;
            //r[7] = mcsj1;
            byte[] tempr = BitConverter.GetBytes(mcgs);
            r[8] = tempr[3];
            r[9] = tempr[2];
            r[10] = tempr[1];
            r[11] = tempr[0];
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_MCgsSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, MCDevicType MCdevictype, long mcgs)
        {
            try
            {
                byte mctpyestate = 0;
                if (MCdevictype == MCDevicType.MC1)
                {
                    mctpyestate = 1;
                }
                else
                {
                    mctpyestate = 2;
                }
                ed.senddatas = getMCgsSet(mctpyestate, mcgs);
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    if (MCdevictype == MCDevicType.MC1)
                    {
                        this.mcgs1 = mcgs;
                    }
                    else
                    {
                        this.mcgs2 = mcgs;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, "地址：" + bzaddress + "表位，脉冲占空比设置命令失败！" + ex.Message);
            }
            return false;
        }


        public static byte[] cmdMCStartEndSet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getMCstartendSet(byte mctpye, byte mcstartendstate)
        {
            byte[] r = new byte[cmdMCStartEndSet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = cmdMCStartEndSet[i];
            }
            r[4] = bzaddress;
            r[6] = mctpye;
            r[7] = mcstartendstate;

            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_MCstartSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, MCDevicType MCDevicType)
        {
            try
            {

                byte mctpyestate = 0;
                if (MCDevicType == MCDevicType.MC1)
                {
                    mctpyestate = 1;
                }
                else
                {
                    mctpyestate = 2;
                }
                ed.senddatas = getMCstartendSet(mctpyestate, 1);
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    if (MCDevicType == MCDevicType.MC1)
                    {
                        mcse1 = MCSETpye.Start;
                    }
                    else
                    {
                        mcse2 = MCSETpye.Start;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, "地址：" + bzaddress + "表位，脉冲启动命令失败！" + ex.Message);
            }
            return false;
        }
        public bool cmd_MCendSetSend(TCPClientChannelargsUserToken socketc, ExecData ed, MCDevicType MCDevicType)
        {
            try
            {

                byte mctpyestate = 0;
                if (MCDevicType == MCDevicType.MC1)
                {
                    mctpyestate = 1;
                }
                else
                {
                    mctpyestate = 2;
                }
                ed.senddatas = getMCstartendSet(mctpyestate, 0);
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    if (MCDevicType == MCDevicType.MC1)
                    {
                        mcse1 = MCSETpye.End;
                    }
                    else
                    {
                        mcse2 = MCSETpye.End;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, "地址：" + bzaddress + "表位，脉冲停止命令失败！" + ex.Message);
            }
            return false;
        }

        //解析设置报文
        public bool MCCSetInputToValue(string[] cmddatas, ref MCDevicType mctpye, ref byte mcdx, ref int mcsj, ref int mcgs)
        {
            byte mcnum = Convert.ToByte(cmddatas[1]);
            if (mcnum == 5)
            {
                mctpye = MCDevicType.MC1;
                mcgs = Convert.ToInt32(cmddatas[2]);
                mcsj = Convert.ToInt32(cmddatas[3]);
                if (cmddatas.Length > 4)
                    mcdx = Convert.ToByte(cmddatas[4]);
                else
                    mcdx = 50;
            }
            else if (mcnum == 7)
            {
                mctpye = MCDevicType.MC2;
                mcgs = Convert.ToInt32(cmddatas[2]);
                mcsj = Convert.ToInt32(cmddatas[3]);
                if (cmddatas.Length > 4)
                    mcdx = Convert.ToByte(cmddatas[4]);
                else
                    mcdx = 50;
            }
            else
            {
                return false;
            }
            return true;
        }
        public bool MCCSEInputToValue(string[] cmddatas, ref MCDevicType mctpye)
        {
            byte mcnum = Convert.ToByte(cmddatas[1]);
            if (mcnum == 5)
            {
                mctpye = MCDevicType.MC1;
            }
            else if (mcnum == 7)
            {
                mctpye = MCDevicType.MC2;
            }
            else if (mcnum == 0)
            {
                mctpye = MCDevicType.ALL;
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool SendMCCSet(TCPClientChannelargsUserToken socketc, ExecData ed1, ExecData ed2, ExecData ed3, string[] cmddatas)
        {
            MCDevicType mctpye = MCDevicType.MC1;
            byte mcdx = 0;
            int mcsj = 0;
            int mcgs = 0;
            bool f = MCCSetInputToValue(cmddatas, ref mctpye, ref mcdx, ref mcsj, ref mcgs);
            if (!f)
            {
                return f;
            }
            //if ((txdevicetype == DeviceTpye.ZB1) || (txdevicetype == DeviceTpye.ZB3))
            {
                f = cmd_MCdxSetSend(socketc, ed1, mctpye, mcdx);
                if (f)
                    f = cmd_MCsjSetSend(socketc, ed2, mctpye, mcsj);
                if (f)
                    f = cmd_MCgsSetSend(socketc, ed3, mctpye, mcgs);
            }
            //else
            //{
            //    f = cmd_MCdxSetSend(socketc, mctpye, mcdx, out sendbytes1, out receivebytes1);
            //    f = cmd_MCsjSetSend(socketc, mctpye, mcsj1, mcsj2, out sendbytes2, out receivebytes2);
            //}
            return f;
        }
        public bool SendMCCStart(TCPClientChannelargsUserToken socketc, ExecData ed1, ExecData ed2, string[] cmddatas)
        {
            MCDevicType mctpye = MCDevicType.MC1;
            //MCCSEInputToValue(cmddatas, ref mctpye);
            bool f = MCCSEInputToValue(cmddatas, ref mctpye);
            if (!f)
            {
                return f;
            }
            //if ((txdevicetype == DeviceTpye.ZB1) || (txdevicetype == DeviceTpye.ZB3))
            {
                if (mctpye == MCDevicType.ALL)
                {
                    f = cmd_MCstartSetSend(socketc, ed1, MCDevicType.MC1);
                    f = cmd_MCstartSetSend(socketc, ed2, MCDevicType.MC2);
                }
                else
                {
                    f = cmd_MCstartSetSend(socketc, ed1, mctpye);
                }
            }
            return f;
        }
        public bool SendMCCEnd(TCPClientChannelargsUserToken socketc, ExecData ed1, ExecData ed2, string[] cmddatas)
        {
            MCDevicType mctpye = MCDevicType.ALL;
            //MCCSEInputToValue(cmddatas, ref mctpye);
            bool f = MCCSEInputToValue(cmddatas, ref mctpye);
            if (!f)
            {
                return f;
            }
            //if ((txdevicetype == DeviceTpye.ZB1) || (txdevicetype == DeviceTpye.ZB3))
            {
                if (mctpye == MCDevicType.ALL)
                {
                    f = cmd_MCendSetSend(socketc, ed1, MCDevicType.MC1);
                    f = cmd_MCendSetSend(socketc, ed2, MCDevicType.MC2);
                }
                else
                {
                    f = cmd_MCendSetSend(socketc, ed1, mctpye);
                }
            }
            return f;
        }

        #endregion

        #region 遥控，告警(只针对集1/专1/专3位)
        public enum YKGJDeviceTpye
        {
            T1 = 0,
            T2 = 1,
            GJ = 2
        }

        public enum YKBHType
        {
            UnKnow = 0,
            BH = 1,
            WBH = 2
        }
        public enum YKType
        {
            UnKnow = 0,
            XH = 1,
            DK = 2
        }
        public enum GJBHType
        {
            UnKnow = 0,
            BH = 1,
            WBH = 2
        }
        public enum GJType
        {
            UnKnow = 0,
            XH = 1,
            DK = 2
        }

        public static string getYKBHType(YKBHType ykbhtype)
        {
            if (ykbhtype == YKBHType.BH)
            {
                return "变化";
            }
            else if (ykbhtype == YKBHType.BH)
            {
                return "无变化";
            }
            else
            {
                return "";
            }
        }
        public static string getGJBHType(GJBHType gjbhtype)
        {
            if (gjbhtype == GJBHType.BH)
            {
                return "变化";
            }
            else if (gjbhtype == GJBHType.BH)
            {
                return "无变化";
            }
            else
            {
                return "";
            }
        }
        public static string getYKType(YKType yktype)
        {
            if (yktype == YKType.XH)
            {
                return "吸合";
            }
            else if (yktype == YKType.DK)
            {
                return "断开";
            }
            else
            {
                return "";
            }
        }
        public static string getGJType(GJType gjtype)
        {
            if (gjtype == GJType.XH)
            {
                return "吸合";
            }
            else if (gjtype == GJType.DK)
            {
                return "断开";
            }
            else
            {
                return "";
            }
        }

        public YKBHType[] ykbhs1 = new YKBHType[2] { YKBHType.UnKnow, YKBHType.UnKnow };
        public YKType[] ykstates1 = new YKType[2] { YKType.UnKnow, YKType.UnKnow };
        public YKBHType[] ykbhs2 = new YKBHType[2] { YKBHType.UnKnow, YKBHType.UnKnow };
        public YKType[] ykstates2 = new YKType[2] { YKType.UnKnow, YKType.UnKnow };

        public GJBHType gjbh = GJBHType.UnKnow;
        public GJType gjstate = GJType.UnKnow;

        //读取采集器摇信
        static public byte[] ykgjGet = new byte[] { 0xFE, 0xFE, 0xFE, 0x05, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x0D, 0x0A };
        private byte[] getcmdykgjGet()
        {
            byte[] r = new byte[ykgjGet.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = ykgjGet[i];
            }
            r[4] = bzaddress;
            int sum = 0;
            for (int i = 4; i < 12; i++)
            {
                sum += r[i];
            }
            r[12] = Convert.ToByte(sum & 0xFF);
            return r;
        }
        public bool cmd_ykgjGetSend(TCPClientChannelargsUserToken socketc, ExecData ed)
        {
            bool f = false;
            try
            {
                ed.senddatas = getcmdykgjGet();
                server.SendToMoxa(socketc, ed, 15);

                if (ed.backdatas != null)
                {
                    f = true;
                    if (ed.backdatas.Length >= ed.senddatas.Length)
                    {
                        for (int i = 0; i < DataIndex; i++)
                        {
                            if (ed.backdatas[i] != ed.senddatas[i])
                            {
                                f = false;
                                break;
                            }
                        }
                        if (f)
                        {
                            byte ykstart = ed.backdatas[DataIndex];
                            byte gjstart = ed.backdatas[DataIndex + 1];
                            #region T1常开，常闭变换状态
                            if ((ykstart & Convert.ToByte(Math.Pow(2, 0))) > 0)
                            {
                                ykbhs1[0] = MainForm.BTGNcls.YKBHType.BH;
                            }
                            else
                            {
                                ykbhs1[0] = MainForm.BTGNcls.YKBHType.WBH;
                            }
                            if ((ykstart & Convert.ToByte(Math.Pow(2, 1))) > 0)
                            {
                                ykbhs1[1] = MainForm.BTGNcls.YKBHType.BH;
                            }
                            else
                            {
                                ykbhs1[1] = MainForm.BTGNcls.YKBHType.WBH;
                            }

                            #endregion
                            #region T2常开，常闭变换状态
                            if ((ykstart & Convert.ToByte(Math.Pow(2, 2))) > 0)
                            {
                                ykbhs2[0] = MainForm.BTGNcls.YKBHType.BH;
                            }
                            else
                            {
                                ykbhs2[0] = MainForm.BTGNcls.YKBHType.WBH;
                            }
                            if ((ykstart & Convert.ToByte(Math.Pow(2, 3))) > 0)
                            {
                                ykbhs2[1] = MainForm.BTGNcls.YKBHType.BH;
                            }
                            else
                            {
                                ykbhs2[1] = MainForm.BTGNcls.YKBHType.WBH;
                            }

                            #endregion
                            #region T1常开，常闭开始关闭
                            if ((ykstart & Convert.ToByte(Math.Pow(2, 4))) > 0)
                            {
                                ykstates1[0] = MainForm.BTGNcls.YKType.XH;
                            }
                            else
                            {
                                ykstates1[0] = MainForm.BTGNcls.YKType.DK;
                            }
                            if ((ykstart & Convert.ToByte(Math.Pow(2, 5))) > 0)
                            {
                                ykstates1[1] = MainForm.BTGNcls.YKType.XH;
                            }
                            else
                            {
                                ykstates1[1] = MainForm.BTGNcls.YKType.DK;
                            }

                            #endregion
                            #region T2常开，常闭开始关闭
                            if ((ykstart & Convert.ToByte(Math.Pow(2, 6))) > 0)
                            {
                                ykstates2[0] = MainForm.BTGNcls.YKType.XH;
                            }
                            else
                            {
                                ykstates2[0] = MainForm.BTGNcls.YKType.DK;
                            }
                            if ((ykstart & Convert.ToByte(Math.Pow(2, 7))) > 0)
                            {
                                ykstates2[1] = MainForm.BTGNcls.YKType.XH;
                            }
                            else
                            {
                                ykstates2[1] = MainForm.BTGNcls.YKType.DK;
                            }

                            #endregion
                            #region 告警端子
                            if ((gjstart & Convert.ToByte(Math.Pow(2, 0))) > 0)
                            {
                                gjbh = MainForm.BTGNcls.GJBHType.BH;
                            }
                            else
                            {
                                gjbh = MainForm.BTGNcls.GJBHType.WBH;
                            }

                            if ((gjstart & Convert.ToByte(Math.Pow(2, 4))) > 0)
                            {
                                gjstate = MainForm.BTGNcls.GJType.XH;
                            }
                            else
                            {
                                gjstate = MainForm.BTGNcls.GJType.DK;
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        f = false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                if (!f)
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，摇控状态招测命令失败！" + ex.Message));
                }
                else
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，解析摇控状态招测结果失败！" + ex.Message));
                }
            }
            return false;
        }

        //获得返回值
        public string YKCValueToOutput(string cmdret)
        {
            string datastr = "cmd=" + cmdret + ",ret=0," + "data=";
            byte ykscstate = 0;
            int ykdata = 0;
            if (gjstate == GJType.XH)
                ykdata = 1;
            for (int i = 0; i < 2; i++)
            {
                if (ykstates1[i] == YKType.XH)
                {
                    ykdata = ykdata + 2 ^ (i + 1);
                }
                if (ykstates2[i] == YKType.XH)
                {
                    ykdata = ykdata + 2 ^ (2 + i + 1);
                }
            }
            datastr = datastr + ykscstate.ToString("X2") + ((byte)ykdata).ToString("X2");
            return datastr;
        }
        public bool SendYKCSet(TCPClientChannelargsUserToken socketc, ExecData ed, string[] cmddatas)
        {
            bool f = false;
            //if ((txdevicetype == DeviceTpye.JZQ1) || (txdevicetype == DeviceTpye.ZB3))
            //{
            f = cmd_ykgjGetSend(socketc, ed);
            //}
            return f;
        }
        #endregion

        #region 给各表位终端发送规约数据
        public TCPClientChannelargsUserToken ZFgetSocketClient(int index)
        {
            TCPClientChannelargsUserToken lnk = null;
            if (Num232 == 1)
            {
                lnk = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.Link2321, index);
            }
            else
            {
                lnk = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.Link2322, index);
            }
            return lnk;
        }
        public bool cmd_ZFZZOrderToDeviceSend(TCPClientChannelargsUserToken usertokn, ExecData ed)
        {
            try
            {
                return server.SendToMoxa(usertokn, ed, waittime1001);
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.Warning, ("地址：" + bzaddress + "表位，转发主台向设备报文失败！" + ex.Message));
            }
            return false;
        }
        public bool cmd_ZFZZOrder232485ToDeviceSend(TCPClientChannelargsUserToken usertokn, ExecData ed)
        {
            try
            {
                return server.SendToMoxa232485(usertokn, ed, waittime1001);
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.Warning, ("地址：" + bzaddress + "表位，转发主台向设备报文失败！" + ex.Message));
            }
            return false;
        }

        //解析转发报文
        public bool ZFCInputToValue(string[] cmddatas, out byte[] bwbytes)
        {
            byte lntype = Convert.ToByte(cmddatas[1]);
            bwbytes = AC.Base.Function.HexStringToByteArray(cmddatas[2]);
            return true;
        }
        public bool SendZF(int index, ExecData ed, string[] cmddatas)
        {
            bool f = false;
            TCPClientChannelargsUserToken lnk = null;
            byte[] sendbytes = null;
            f = ZFCInputToValue(cmddatas, out sendbytes);
            if (f)
            {
                lnk = ZFgetSocketClient(index);
                if (Num232 == 1)
                {
                    if (lnk != null)
                    {
                        ed.senddatas = sendbytes;
                        f = cmd_ZFZZOrder232485ToDeviceSend(lnk, ed);
                    }
                }
                else
                {
                    if (lnk != null)
                    {
                        ed.senddatas = sendbytes;
                        f = cmd_ZFZZOrder232485ToDeviceSend(lnk, ed);
                    }
                }
            }
            return f;
        }
        #endregion

        #region 模拟抄表
        public TCPClientChannelargsUserToken[] MNCBgetSocketClient(int index, bool sendtoonlylinkis)
        {
            TCPClientChannelargsUserToken[] lnk = null;
            if (sendtoonlylinkis)
            {
                lnk = new TCPClientChannelargsUserToken[1];
                lnk[0] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkJ1Z3, index);
            }
            else
            {
                lnk = new TCPClientChannelargsUserToken[3];
                lnk[0] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkJ1Z3, index);
                lnk[1] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkZ1C2, index);
                lnk[2] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkJ2C1, index);
            }
            return lnk;
        }

        public bool cmd_mncbZZOrderToDeviceSend(TCPClientChannelargsUserToken usertokn, byte[] senddatas)
        {
            try
            {
                return server.CBSendToMoxa(usertokn, senddatas);
            }
            catch (Exception ex)
            {
                //OnMessage(ChannelMessageTypeOptions.MOXAError, ("地址：" + bzaddress + "表位，转发主台返回模拟表数据失败！" + ex.Message));
            }
            return false;
        }
        public bool MNCBCInputToValue(string[] cmddatas, out byte[] bwbytes)
        {
            bwbytes = AC.Base.Function.HexStringToByteArray(cmddatas[1]);
            return true;
        }
        public bool SendMNCB(int index, string[] cmddatas, out byte[] sendbytes)
        {
            bool f = false;
            bool sendf = true;
            sendbytes = null;
            TCPClientChannelargsUserToken[] lnk = null;

            f = MNCBCInputToValue(cmddatas, out sendbytes);
            if (f)
            {
                if (sendf)
                {
                    lnk = MNCBgetSocketClient(index, sendf);
                    if (lnk != null)
                    {
                        f = cmd_mncbZZOrderToDeviceSend(lnk[0], sendbytes);
                    }
                }
                else
                {
                    lnk = MNCBgetSocketClient(index, sendf);
                    if (lnk != null)
                    {
                        f = cmd_mncbZZOrderToDeviceSend(lnk[0], sendbytes);
                        f = cmd_mncbZZOrderToDeviceSend(lnk[1], sendbytes);
                        f = cmd_mncbZZOrderToDeviceSend(lnk[2], sendbytes);
                    }
                }
            }
            return f;
        }
        /// <summary>
        /// 通过纪录的抄表端口回复数据
        /// </summary>
        /// <param name="lnk"></param>
        /// <param name="cmddatas"></param>
        /// <param name="sendbytes"></param>
        /// <returns></returns>
        public bool SendMNCB(TCPClientChannelargsUserToken lnk, string[] cmddatas, out byte[] sendbytes)
        {
            bool f = false;
            sendbytes = null;

            f = MNCBCInputToValue(cmddatas, out sendbytes);
            if (f)
            {
                f = cmd_mncbZZOrderToDeviceSend(lnk, sendbytes);
            }
            return f;
        }
        #endregion

        #region 通道开启
        public TCPClientChannelargsUserToken[] KGTDgetSocketClient(int num, bool sendtoonlylinkis)
        {
            TCPClientChannelargsUserToken[] lnk = null;
            if ((num > 0) && (num < 9))
            {
                #region 232
                if (sendtoonlylinkis)
                {
                    lnk = new TCPClientChannelargsUserToken[1];
                    if (Num232 == 1)
                    {
                        lnk[0] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.Link2321, num - 1);
                    }
                    else if (Num232 == 2)
                    {
                        lnk[0] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.Link2322, num - 1);
                    }
                }
                else
                {
                    lnk = new TCPClientChannelargsUserToken[2];
                    lnk[0] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.Link2321, num - 1);
                    lnk[1] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.Link2322, num - 1);
                }
                #endregion
            }
            else if ((num > 16) && (num < 25))
            {
                num = num - 16;
                #region 485
                lnk = new TCPClientChannelargsUserToken[3];
                lnk[0] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkJ1Z3, num - 1);
                lnk[1] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkZ1C2, num - 1);
                lnk[2] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkJ2C1, num - 1);
                #endregion
            }
            return lnk;
        }
        public bool KGTDInputToValue(string[] cmddatas, out int num)
        {
            num = Convert.ToByte(cmddatas[0]);
            return true;
        }
        public bool KGTDtoK(string[] cmddatas)
        {
            bool f = false;
            bool sendf = false;
            int num = 0;
            TCPClientChannelargsUserToken[] lnk = null;

            f = KGTDInputToValue(cmddatas, out num);

            if (sendf)
            {
                lnk = KGTDgetSocketClient(num, sendf);
                if (lnk != null)
                {
                    server.TCPClientStart(lnk[0]);
                    return true;
                }
            }
            else
            {
                lnk = KGTDgetSocketClient(num, sendf);
                if (lnk != null)
                {
                    for (int i = 0; i < lnk.Length; i++)
                    {
                        server.TCPClientStart(lnk[i]);
                    }
                    return true;
                }
            }

            return false;
        }
        public bool KGTDtoG(string[] cmddatas)
        {
            bool f = false;
            bool sendf = false;
            int num = 0;
            TCPClientChannelargsUserToken[] lnk = null;

            f = KGTDInputToValue(cmddatas, out num);

            if (sendf)
            {
                lnk = KGTDgetSocketClient(num, sendf);
                if (lnk != null)
                {
                    server.TCPClientStop(lnk[0]);
                    return true;
                }
            }
            else
            {
                lnk = KGTDgetSocketClient(num, sendf);
                if (lnk != null)
                {
                    for (int i = 0; i < lnk.Length; i++)
                    {
                        server.TCPClientStop(lnk[i]);
                    }
                    return true;
                }
            }

            return false;
        }

        public bool GetTD485(int index, out byte[] sendbytes, out string receivestr)
        {
            bool sendf = true;
            int _state = 0;
            receivestr = String.Empty;
            sendbytes = null;
            TCPClientChannelargsUserToken[] lnk = null;

            lnk = KGTDgetSocketClient(index + 15, sendf);
            if (lnk != null)
            {
                if (server.GetTCPClientState(lnk[0]) != null)
                {
                    _state = 1;
                }
                else
                {
                    _state = 0;
                }
                receivestr = _state.ToString("00");
                return true;
            }
            return false;
        }
        public bool SetTD485(int index, int _state, out byte[] sendbytes, out string receivestr)
        {
            bool sendf = false;
            int state = _state;
            receivestr = _state.ToString("00");
            sendbytes = null;
            TCPClientChannelargsUserToken[] lnk = null;
            if (sendf)
            {
                lnk = KGTDgetSocketClient(index + 17, sendf);
                if (lnk != null)
                {
                    if (_state == 1)
                        server.TCPClientStart(lnk[0]);
                    else
                        server.TCPClientStop(lnk[0]);
                    return true;
                }
            }
            else
            {
                lnk = KGTDgetSocketClient(index + 17, sendf);
                if (lnk != null)
                {
                    for (int i = 0; i < lnk.Length; i++)
                    {
                        if (_state == 1)
                            server.TCPClientStart(lnk[i]);
                        else
                            server.TCPClientStop(lnk[i]);
                    }
                    return true;
                }
            }
            return false;
        }
        public void OpenCB485(int index)
        {
            TCPClientChannelargsUserToken[] lnk = null;
            if ((index > 16) && (index < 25))
            {
                index = index - 16;
                lnk = new TCPClientChannelargsUserToken[3];
                lnk[0] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkJ1Z3, index - 1);
            }
        }

        public TCPClientChannelargsUserToken[] KGTDnewgetSocketClient(int _bwindex, int _channeltype, int _gytype, int _PROT_MeterBaudRate, int _PROT_STOP, int _PROT_BIT, int _PROT_P)
        {
            TCPClientChannelargsUserToken[] lnk = null;
            if (_channeltype == 0)
            {
                lnk = new TCPClientChannelargsUserToken[1];
                if (Num232 == 1)
                {
                    lnk[0] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.Link2321, _bwindex);
                    lnk[0].gy485type = _gytype;
                    lnk[0]._PROT_MeterBaudRate = _PROT_MeterBaudRate;
                    lnk[0]._PROT_BIT = _PROT_BIT;
                    lnk[0]._PROT_STOP = _PROT_STOP;
                    lnk[0]._PROT_P = _PROT_P;
                }
                else if (Num232 == 2)
                {
                    lnk[0] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.Link2322, _bwindex);
                    lnk[0].gy485type = _gytype;
                    lnk[0]._PROT_MeterBaudRate = _PROT_MeterBaudRate;
                    lnk[0]._PROT_BIT = _PROT_BIT;
                    lnk[0]._PROT_STOP = _PROT_STOP;
                    lnk[0]._PROT_P = _PROT_P;
                }
            }
            else if (_channeltype == 1 || _channeltype == 2)
            {
                lnk = new TCPClientChannelargsUserToken[3];
                lnk[0] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkJ1Z3, _bwindex);
                lnk[0].gy485type = _gytype;
                lnk[0]._PROT_MeterBaudRate = _PROT_MeterBaudRate;
                lnk[0]._PROT_BIT = _PROT_BIT;
                lnk[0]._PROT_STOP = _PROT_STOP;
                lnk[0]._PROT_P = _PROT_P;
                lnk[1] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkJ2C1, _bwindex);
                lnk[1].gy485type = _gytype;
                lnk[1]._PROT_MeterBaudRate = _PROT_MeterBaudRate;
                lnk[1]._PROT_BIT = _PROT_BIT;
                lnk[1]._PROT_STOP = _PROT_STOP;
                lnk[1]._PROT_P = _PROT_P;
                lnk[2] = server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkZ1C2, _bwindex);
                lnk[2].gy485type = _gytype;
                lnk[2]._PROT_MeterBaudRate = _PROT_MeterBaudRate;
                lnk[2]._PROT_BIT = _PROT_BIT;
                lnk[2]._PROT_STOP = _PROT_STOP;
                lnk[2]._PROT_P = _PROT_P;
            }
            return lnk;
        }
        public bool KGTDnewInputToValue(string[] cmddatas, out int _channeltype, out int _gytype, out int _PROT_MeterBaudRate, out int _PROT_STOP, out int _PROT_BIT, out int _PROT_P)
        {
            _channeltype = Convert.ToInt32(cmddatas[0]);
            _gytype = Convert.ToInt32(cmddatas[1]);
            //_bwindex = Convert.ToByte(cmddatas[2]);
            string[] value = cmddatas[3].Split('-');
            _PROT_MeterBaudRate = Convert.ToInt32(value[0]);
            if (value[1] == "e" || value[1] == "E")
            {
                _PROT_P = 2;
            }
            else if (value[1] == "o" || value[1] == "O")
            {
                _PROT_P = 1;
            }
            else
            {
                _PROT_P = 0;
            }
            _PROT_BIT = Convert.ToInt32(value[2]);
            _PROT_STOP = Convert.ToInt32(value[3]);
            return true;
        }
        public bool KGTDnewtoK(string[] cmddatas, int _bwindex)
        {
            bool f = false;
            int _channeltype, _gytype, _PROT_MeterBaudRate, _PROT_STOP, _PROT_BIT, _PROT_P;
            TCPClientChannelargsUserToken[] lnk = null;
            f = KGTDnewInputToValue(cmddatas, out _channeltype, out _gytype, out _PROT_MeterBaudRate, out _PROT_STOP, out _PROT_BIT, out _PROT_P);

            lnk = KGTDnewgetSocketClient(_bwindex, _channeltype, _gytype, _PROT_MeterBaudRate, _PROT_STOP, _PROT_BIT, _PROT_P);

            if (lnk != null)
            {
                for (int i = 0; i < lnk.Length; i++)
                {
                    server.TCPClientStart(lnk[i]);
                    int mode = 0;
                    if (lnk[i]._PROT_BIT == 5)
                    {
                        mode = mode | PortControl.BIT_5;
                    }
                    else if (lnk[i]._PROT_BIT == 6)
                    {
                        mode = mode | PortControl.BIT_6;
                    }
                    else if (lnk[i]._PROT_BIT == 7)
                    {
                        mode = mode | PortControl.BIT_7;
                    }
                    else if (lnk[i]._PROT_BIT == 8)
                    {
                        mode = mode | PortControl.BIT_8;
                    }
                    if (lnk[i]._PROT_STOP == 1)
                    {
                        mode = mode | PortControl.STOP_1;
                    }
                    else if (lnk[i]._PROT_STOP == 2)
                    {
                        mode = mode | PortControl.STOP_2;
                    }
                    if (lnk[i]._PROT_P == 0)
                    {
                        mode = mode | PortControl.P_NONE;
                    }
                    else if (lnk[i]._PROT_P == 1)
                    {
                        mode = mode | PortControl.P_ODD;
                    }
                    else if (lnk[i]._PROT_P == 2)
                    {
                        mode = mode | PortControl.P_EVEN;
                    }
                    PortControl.setPortBaudandMode(lnk[i].IP, lnk[i].mgrPort, lnk[i]._PROT_MeterBaudRate, mode);
                }
                return true;
            }
            return false;
        }
        #endregion
    }
}
