using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 三湘源涵普Test;
using System.Xml;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace MainForm
{
    //源
    public class DSPOutputValueCls : AC.Base.ISystemConfig
    {
        class WCBSJTest
        {
            /// <summary>
            /// 表位号
            /// </summary>
            public int bwindex = 0;
            /// <summary>
            /// 时间间隔
            /// </summary>
            public int lIntCirclesSJ = 60;
            /// <summary>
            /// 理论脉冲数90
            /// </summary>
            public int lLngPulesSJ = 0;
            /// <summary>
            /// 最大测试次数
            /// </summary>
            public int MaxNumSJ = 10;

            public Thread wcbsjtestthr = null;

            public SortedDictionary<int, MeterClass.Answer> SJValuesList = new SortedDictionary<int, MeterClass.Answer>();

            //public DSPOutputValueCls dspcls = null;

            private Stopwatch waittime = new Stopwatch();

            #region << 循环执行事件 >>GP
            /// <summary>
            /// 循环执行事件
            /// </summary>
            public event XHHandler XHH;
            /// <summary>
            /// 循环执行事件
            /// </summary>
            public delegate int XHHandler(int _index);
            /// <summary>
            /// 循环执行事件
            /// </summary>
            internal void OnXHH(int _index)
            {
                try
                {
                    if (XHH != null)
                        XHH(_index);
                }
                catch (Exception ex)
                {

                }
            }
            #endregion
            public void WCBTestThrStart()
            {
                wcbsjtestthr = new Thread(WCBTestThr);
                wcbsjtestthr.Start();
            }
            public void WCBTestThr()
            {
                try
                {
                    waittime.Restart();
                    long _waittime = lIntCirclesSJ;
                    long _runtime = _waittime;
                    int timenum = 0;
                    while (timenum < MaxNumSJ)
                    {
                        if (waittime.Elapsed.TotalSeconds > (_runtime + 3))
                        {
                            //dspcls.WCB_SJRead(bwindex);
                            OnXHH(bwindex);
                            timenum++;
                            _runtime = _runtime + _waittime;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    waittime.Stop();
                }
            }
        }

        #region ISystemConfig实现
        public void SetConfig(System.Xml.XmlNode serverConfig)
        {
            string errstr = "";
            string data = "";
            try
            {
                for (int i = 0; i < serverConfig.ChildNodes.Count; i++)
                {
                    data = serverConfig.ChildNodes[i].InnerText;
                    #region server
                    if (serverConfig.ChildNodes[i].Name.Equals("ComNo"))
                    {
                        errstr = "ComNo";
                        //ComNo = Convert.ToInt32(serverConfig.ChildNodes[i].InnerText);
                    }
                    if (serverConfig.ChildNodes[i].Name.Equals("ComBaud"))
                    {
                        errstr = "ComBaud";
                        ComBaud = Convert.ToInt32(serverConfig.ChildNodes[i].InnerText);
                    }
                    if (serverConfig.ChildNodes[i].Name.Equals("ComParity"))
                    {
                        errstr = "ComParity";
                        ComParity = Convert.ToInt32(serverConfig.ChildNodes[i].InnerText);
                    }
                    if (serverConfig.ChildNodes[i].Name.Equals("ComStopBits"))
                    {
                        errstr = "ComStopBits";
                        ComStopBits = Convert.ToInt32(serverConfig.ChildNodes[i].InnerText);
                    }
                    if (serverConfig.ChildNodes[i].Name.Equals("DataBit"))
                    {
                        errstr = "DataBit";
                        DataBit = Convert.ToByte(serverConfig.ChildNodes[i].InnerText);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("DSPOutputValueCls", "SetConfig", errstr, data, ex.ToString());
            }
        }
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xn = xmlDoc.CreateElement("DSPOutputValueCls");

            #region Com
            XmlNode XMLComNo = xmlDoc.CreateElement("ComNo");
            XMLComNo.InnerText = this.ComNo.ToString();
            xn.AppendChild(XMLComNo);

            XmlNode XMLComBaud = xmlDoc.CreateElement("ComBaud");
            XMLComBaud.InnerText = this.ComBaud.ToString();
            xn.AppendChild(XMLComBaud);

            XmlNode XMLComParity = xmlDoc.CreateElement("ComParity");
            XMLComParity.InnerText = this.ComParity.ToString();
            xn.AppendChild(XMLComParity);

            XmlNode XMLComStopBits = xmlDoc.CreateElement("ComStopBits");
            XMLComStopBits.InnerText = this.ComStopBits.ToString();
            xn.AppendChild(XMLComStopBits);

            XmlNode XMLDataBit = xmlDoc.CreateElement("DataBit");
            XMLDataBit.InnerText = this.DataBit.ToString();
            xn.AppendChild(XMLDataBit);
            #endregion

            return xn;
        }
        #endregion

        #region Com
        public int ComNo = 1;
        private int ComBaud = 2400;
        private int ComParity = 1;
        private int ComStopBits = 1;
        private byte DataBit = 8;
        #endregion

        #region MeterClass.DSPOutput 函数参数
        public int pLngAdjust = 1;      //  是否对输出作自动调整，1-调整，0-不调整。
        public int pLngSourceType = 6;  /*  信号源类型
                                         *  1、2为C形输出的信号源，
                                         *	3为D输出的信号源，
                                         *	4为DSP信号源(此处无效)
                                         *	5为D型输出的信号源配HC3100H标准表
                                         *	6为Dsp信号源输出配HC3100H标准表（此处无效）
                                         */

        //public int ComNo = ComNo;
        public int pIntID = 3;          /*   0:针对标准表为HC3100、HC3101
                                         *       调用函数为OperationHC_Fun，电流小数位数为4位。
                                         *   1:针对标准表为HC3100H、HC3101H（100A）
                                         *       调用函数为OperationHC_MODBUS_Fun，电流小数位数为5位。
                                         *   2:针对标准表为HC3100A、HC3101A（200A）
                                         *       调用函数为OperationHC_MODBUS_Fun，电流小数位数为5位。
                                         *   3:针对标准表为HC3100A、HC3101A（100A）
                                         *      调用函数为OperationHC_HC310X_Fun，电流小数位数为5位。
                                         *   4:针对标准表为HC3100H、HC3101H（200A）
                                         *       调用函数为OperationHC_MODBUS_200_Fun，电流小数位数为5位。
                                         *   5:针对标准表为HC3100H、HC3101H（100A）
                                         *       调用函数为OperationHC_MODBUS_Fun，电流小数位数为4位。
                                         *   6:针对标准表为HC3100H、HC3101H（200A）
                                         *      调用函数为OperationHC_MODBUS_200_Fun，电流小数位数为4位。
                                         *   7:针对老台湾标准表HC3100
                                         *      调用函数为OperationHC_HC3100_TW_Fun，电流小数位数为5位。
                                         */

        public int PhaseWire = 5;       /*  相线 
                                         *	1表示3P3W（三相三线有功）
                                         *  2表示3p3w(60Var)（三相三线60度无功）
                                         *  3表示3p3w(90Var)（三相三线90度无功）
                                         *  4表示3p3w(正弦Var)（三相三线正弦无功）
                                         *  5表示3P4W（三相四线有功）
                                         *  6表示3p4w(90Var)（三相四线90度无功）
                                         *  7表示3p4w(正弦Var)（三相四线正弦无功）
                                    	 *  8表示单相有功
                            	         *  9表示单相无功
                                         */

        public Single Frequency = 50;   //额定频率
        public Single Ua = 220;         //UA
        public Single Ub = 220;         //UB
        public Single Uc = 220;         //UC
        public Single Ia = 5;           //IA
        public Single Ib = 5;           //IA
        public Single Ic = 5;           //IA

        public Single DUAB = 120;       //为电压Uab和电压Uac直接的夹角。正相序时，夹角为120和240，逆相序时为240和120（以上为平衡输出时的夹角）
        public Single DUAC = 240;

        public Single DUIa = 0;          //DUIa、DUIb、DUIc 为A、B、C三相电压和电流之间的夹角。以上各个夹角的数值范围为（0—359.99）之间。为Single单精度类型
        public Single DUIb = 0;
        public Single DUIc = 0;

        public int WaveType = 1;        /*   输出的波形类别 
                                         *   1、正弦波;
                                         *   2、次群波（奇次谐波）;
                                         *   3、谐波;
                                         *   4、可控硅波（偶次谐波）;一般的台体上，此参数都选择1（正弦波），其他波形都必须是台体硬件支持才能实现，否则无效。
                                         */
        public MeterClass.DelayTime DeleyTime = new MeterClass.DelayTime();  /*  控源延时，此参数为一个结构体（定义在附注中）。
                                                                                 *  该结构体中含有两个LongInt类型参数：
                                                                                 *  pLngSeatdyTime 为台体升源稳定延时
                                                                                 *  pLngAdjustTime 为台体信号源调整延时
                                                                                 */
        public MeterClass.Standard strData = new MeterClass.Standard();
        public float pSngRealConst = 0;
        public int IntCommandID = 3;    /* ----- 控制命令字
                                      * 1 ：联机（保留字）
                                      * 2 ：脱机（保留字）
                                      * 3 ：读取标准表显示值
                                      * 4 ：标准表有功输出及电流输出档位
                                    *	 lIntID = 10 10A档位
                                    *	 lIntID = 5  5A档位
                                    *	 lIntID = 1  1A档位
                                    *	 lIntID = 0  自动档位
                                      * 5 ：标准表无功输出及电流输出档位
                                    *  lIntID = 10 10A档位
                                    *  lIntID = 5  5A档位
                                    *  lIntID = 1  1A档位
                                    *  lIntID = 0  自动档位
                                      * 6 ：设置标准表相线（phaseWire）
                                    *  lIntID = 1 3p4w(三相四线有功/正弦无功)
                                    *	 lIntID = 2 3p3w(三相三线有功/正弦无功)
                                    *	 lIntID = 3 3p4w(三相四线90度无功)
                                    *  lIntID = 4 3p3w(三相三线60度无功)
                                    *  lIntID = 5 3p3w(三相三线90度无功)
                                      *              7 ：保留字
                                      *              8 ：保留字
                                      *              9 ：设置标准显示电流档位
                                    *  lIntID = 100  100A档位
                                    *  lIntID = 10   10A档位
                                    *  lIntID = 1    1A档位
                                      * 10 ：保留字
                                      * 11 ：设置电压档位
                                    *  lIntID = 1   自动挡
                                    *  lIntID = 2   600V档位
                                    *  lIntID = 3   320V档位
                                    *  lIntID= 4   160V档位
                                    *  lIntID = 5   80V档位
                                    */
        public int lIntID = 0;
        #endregion

        #region MeterClass.SetDSPHarmonic 函数参数
        public byte[] XBabcIS = new byte[] { 1, 1, 1, 1, 1, 1 };

        public float WaveDegreeA = 0;       //谐波初相角1，0-359.99
        public float WaveDegreeB = 0;       //谐波初相角2，0-359.99
        public float WaveDegreeC = 0;       //谐波初相角3，0-359.99
        public int WaveTimesA = 0;       //谐波次数1，0-21
        public int WaveTimesB = 0;       //谐波次数2，0-21
        public int WaveTimesC = 0;        //谐波次数3，0-21
        public int VoltageWaveRateA = 0;  //电压谐波含量1，0-40
        public int VoltageWaveRateB = 0;  //电压谐波含量2，0-40
        public int VoltageWaveRateC = 0; //电压谐波含量3，0-40
        public int CurrentWaveRateA = 0; //电流谐波含量1，0-40
        public int CurrentWaveRateB = 0;  //电流谐波含量1，0-40
        public int CurrentWaveRateC = 0;  //电流谐波含量1，0-40
        #endregion

        #region MeterClass.OperationHC_HC310X_Fun 函数参数
        public float pIntStatus = 0; 
        public float SngVoltage = 0;
        public float pSngCurrent = 0;
        #endregion 

        #region MeterClass.ErrorCircles_gFun 函数参数
        private int lIntCircles = 1;       //圈数
        private int lSngCount = 6400;      //点表脉冲数
        private float lmaxu = 0;
        private float lmaxi = 0;
        private int lLngPules = 0;          //理论脉冲数
        private Single lSngMaxError = 1;

        private int WCMCTDNO = -1;          //使用脉冲通道
        private int WCBState = -1;          //误差表状态 1 计时, 2 交采
        private int lIntCirclesSJ = 60;     //时间间隔
        private int lLngPulesSJ = 0;        //理论脉冲数90
        private int MaxNumSJ = 10;
        private ConcurrentDictionary<int, WCBSJTest> WCBSJTests = new ConcurrentDictionary<int, WCBSJTest>();
       
        
        private MeterClass.Answer strWCBData = new MeterClass.Answer();
        private MeterClass.Pules strWCBPules = new MeterClass.Pules();
        #endregion

        private float xjua = 0;
        private float xjub = 120;
        private float xjuc = 240;
        private float xjia = 0;
        private float xjib = 120;
        private float xjic = 240;

        public float XJUa
        {
            get { return xjua;}
            set {
                if (value >= 360)
                    xjua = value % 360;
                else if (value < 0)
                    xjua = value % 360 + 360;
                else
                    xjua = value;
            }
        }
        public float XJUb
        {
            get { return xjub; }
            set
            {
                if (value >= 360)
                    xjub = value % 360;
                else if (value < 0)
                    xjub = value % 360 + 360;
                else
                    xjub = value;
            }
        }
        public float XJUc
        {
            get { return xjuc; }
            set
            {
                if (value >= 360)
                    xjuc = value % 360;
                else if (value < 0)
                    xjuc = value % 360 + 360;
                else
                    xjuc = value;
            }
        }
        public float XJIa
        {
            get { return xjia; }
            set
            {
                if (value >= 360)
                    xjia = value % 360;
                else if (value < 0)
                    xjia = value % 360 + 360;
                else
                    xjia = value;
            }
        }
        public float XJIb
        {
            get { return xjib; }
            set
            {
                if (value >= 360)
                    xjib = value % 360;
                else if (value < 0)
                    xjib = value % 360 + 360;
                else
                    xjib = value;
            }
        }
        public float XJIc
        {
            get { return xjic; }
            set
            {
                if (value >= 360)
                    xjic = value % 360;
                else if (value < 0)
                    xjic = value % 360 + 360;
                else
                    xjic = value;
            }
        }

        public void setPhaseWire(string PhaseWireType)
        {
            switch (PhaseWireType)
            {
                case "PT4":
                    PhaseWire = 5;
                    break;
                case "QT4":
                    PhaseWire = 7;
                    break;
                case "P32":
                    PhaseWire = 1;
                    break;
                case "Q32":
                    PhaseWire = 4;
                    break;
                case "Q60":
                    PhaseWire = 5;
                    break;
                case "Q90":
                    PhaseWire = 5;
                    break;
                case "Q33":
                    PhaseWire = 1;
                    break;
                case "P":
                    PhaseWire = 5;
                    break;
            }
        }
        public void setWaveType(string WaveTypeType)
        {
            switch (WaveTypeType)
            {
                case "正弦波":
                    WaveType = 1;
                    break;
                case "谐波":
                    WaveType = 3;
                    break;
            }
        }
        public void setDUABDUAC(float Adyjj, float Bdyjj, float Cdyjj)
        {
            XJUa = Adyjj;
            XJUb = Bdyjj;
            XJUc = Cdyjj;

            DUAB = Bdyjj - Adyjj;
            if (DUAB < 0)
            {
                DUAB = 360 + DUAB % 360;
            }
            else if (DUAB >= 360)
            {
                DUAB = DUAB % 360;
            }
            DUAC = Cdyjj - Adyjj;
            if (DUAC < 0)
            {
                DUAC = 360 + DUAC % 360;
            }
            else if (DUAC >= 360)
            {
                DUAC = DUAC % 360;
            }
        }
        public void setDUIaDUIbDUIc(float Adyjj, float Bdyjj, float Cdyjj, float Adljj, float Bdljj, float Cdljj)
        {
            XJUa = Adyjj;
            XJUb = Bdyjj;
            XJUc = Cdyjj;

            XJIa = Adljj;
            XJIb = Bdljj;
            XJIc = Cdljj;

            DUIa = Adljj - Adyjj;
            if (DUIa < 0)
            {
                DUIa = 360 + DUIa % 360;
            }
            else if (DUIa >= 360)
            {
                DUIa = DUIa % 360;
            }

            DUIb = Bdljj - Bdyjj;
            if (DUIb < 0)
            {
                DUIb = 360 + DUIb % 360;
            }
            else if (DUIb > 360)
            {
                DUIb = DUIb % 360;
            }

            DUIc = Cdljj - Cdyjj;
            if (DUIc < 0)
            {
                DUIc = 360 + DUIc % 360;
            }
            else if (DUIc > 360)
            {
                DUIc = DUIc % 360;
            }

        }

        public int DSPOutput()
        {
            int r = MeterClass.DSPOutput(pLngAdjust, ComNo, pIntID, PhaseWire, Frequency, Ua, Ub, Uc, Ia, Ib, Ic, DUAB, DUAC, DUIa, DUIb, DUIc, WaveType, ref DeleyTime);
            return r;
        }
        public int DSPDown()
        {
            //int r = MeterClass.SingleClear(ComNo);
            Ua = 0;
            Ub = 0;
            Uc = 0;
            Ia = 0;
            Ib = 0;
            Ic = 0;
            int r = MeterClass.DSPOutput(pLngAdjust, ComNo, pIntID, PhaseWire, Frequency, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, WaveType, ref DeleyTime);
            return r;
        }
        public int DSPStop()
        {
            int r = MeterClass.DSPOutput(pLngAdjust, ComNo, pIntID, PhaseWire, Frequency, Ua, Ub, Uc, 0, 0, 0, DUAB, DUAC, 0, 0, 0, WaveType, ref DeleyTime);
            return r;
        }

        public int SetDSPHarmonic()
        {
            bool f = false;
            float tempWaveDegreeA = 0;       //谐波初相角1，0-359.99
            float tempWaveDegreeB = 0;       //谐波初相角2，0-359.99
            float tempWaveDegreeC = 0;       //谐波初相角3，0-359.99
            int tempWaveTimesA = 0;       //谐波次数1，0-21
            int tempWaveTimesB = 0;       //谐波次数2，0-21
            int tempWaveTimesC = 0;        //谐波次数3，0-21
            int tempVoltageWaveRateA = 0;  //电压谐波含量1，0-40
            int tempVoltageWaveRateB = 0;  //电压谐波含量2，0-40
            int tempVoltageWaveRateC = 0; //电压谐波含量3，0-40
            int tempCurrentWaveRateA = 0; //电流谐波含量1，0-40
            int tempCurrentWaveRateB = 0;  //电流谐波含量1，0-40
            int tempCurrentWaveRateC = 0;  //电流谐波含量1，0-40
            if (XBabcIS[0] == 1)
            {
                tempWaveDegreeA = WaveDegreeA;
                tempWaveTimesA = WaveTimesA;
                tempVoltageWaveRateA = VoltageWaveRateA;
                f = true;
            }
            if (XBabcIS[3] == 1)
            {
                tempWaveDegreeA = WaveDegreeA;
                tempWaveTimesA = WaveTimesA;
                tempCurrentWaveRateA = CurrentWaveRateA;
                f = true;
            }
            if (XBabcIS[1] == 1)
            {
                tempWaveDegreeB = WaveDegreeB;
                tempWaveTimesB = WaveTimesB;
                tempVoltageWaveRateB = VoltageWaveRateB;
                f = true;
            }
            if (XBabcIS[4] == 1)
            {
                tempWaveDegreeB = WaveDegreeB;
                tempWaveTimesB = WaveTimesB;
                tempCurrentWaveRateB = CurrentWaveRateB;
                f = true;
            }
            if (XBabcIS[2] == 1)
            {
                tempWaveDegreeC = WaveDegreeC;
                tempWaveTimesC = WaveTimesC;
                tempVoltageWaveRateC = VoltageWaveRateC;
                f = true;
            }
            if (XBabcIS[5] == 1)
            {
                tempWaveDegreeC = WaveDegreeC;
                tempWaveTimesC = WaveTimesC;
                tempCurrentWaveRateC = CurrentWaveRateC;
                f = true;
            }
            if (!f)
            {
                return -1;
            }
            int r = MeterClass.SetDSPHarmonic(ComNo, pIntID, tempWaveDegreeA, tempWaveDegreeB, tempWaveDegreeC, tempWaveTimesA, tempWaveTimesB, tempWaveTimesC, tempVoltageWaveRateA, tempVoltageWaveRateB, tempVoltageWaveRateC, tempCurrentWaveRateA, tempCurrentWaveRateB, tempCurrentWaveRateC);
            return r;
        }

        //电压相位角和电流相位角？？？？
        public int OperationHC_Fun()
        {
            int r = MeterClass.OperationHC_HC310X_Fun(pLngSourceType, ComNo, 55, IntCommandID, 0, 0, 0f, 0f, 180000f, ref pSngRealConst, ref strData);
            if (r == -1)
                getis = true;
            else
                getis = false;

            if(getis)
            {
                if (strData.RealWA < 0)
                    strData.RealPFA = -strData.RealPFA;
                if (strData.RealWB < 0)
                    strData.RealPFB = -strData.RealPFB;
                if (strData.RealWC < 0)
                    strData.RealPFC = -strData.RealPFC;
            }
            return r;
        }
        public int SetOperationHC_Fun()
        {
            //正向有功
            if (pSngCurrent == 1.2f)
                pSngCurrent = 1f;
            int r = MeterClass.OperationHC_HC310X_Fun(pLngSourceType, ComNo, 55, 4, PhaseWire, 0, SngVoltage, pSngCurrent, 180000f, ref pSngRealConst, ref strData);
            return r;
        }

        private bool getis = false;
        public string getOperationHC_Fun()
        {
            if (!getis)
                return "召测失败！";
 
            float tempua, tempub, tempuc, tempia, tempib, tempic;
            if ((strData.RealVA > 1) && (strData.RealVA > 1) && (strData.RealVA > 1))
            {
                tempua = XJUa;
                tempub = XJUb;
                tempuc = XJUc;
            }
            else
            {
                tempua = 0;
                tempub = 0;
                tempuc = 0;
            }
            if ((strData.RealAA > 1) && (strData.RealAA > 1) && (strData.RealAA > 1))
            {
                tempia = XJIa;
                tempib = XJIb;
                tempic = XJIc;
            }
            else
            {
                tempia = 0;
                tempib = 0;
                tempic = 0;
            }

            string datastr = string.Format("[A相]电压:{0}V; 电压角:{1}度; 电流:{2}A; 电流角:{3}度\r\n", strData.RealVA.ToString("0.000").PadLeft(7,' '), tempua.ToString("0.000").PadLeft(7,' '), strData.RealAA.ToString("0.000").PadLeft(6,' '), tempia.ToString("0.000").PadLeft(7,' '));
            datastr = datastr + string.Format("[B相]电压:{0}V; 电压角:{1}度; 电流:{2}A; 电流角:{3}度\r\n", strData.RealVB.ToString("0.000").PadLeft(7,' '), tempub.ToString("0.000").PadLeft(7,' '), strData.RealAB.ToString("0.000").PadLeft(6,' '), tempib.ToString("0.000").PadLeft(7,' '));
            datastr = datastr + string.Format("[C相]电压:{0}V; 电压角:{1}度; 电流:{2}A; 电流角:{3}度\r\n", strData.RealVC.ToString("0.000").PadLeft(7, ' '), tempuc.ToString("0.000").PadLeft(7, ' '), strData.RealAC.ToString("0.000").PadLeft(6, ' '), tempic.ToString("0.000").PadLeft(7, ' '));
            datastr = datastr + string.Format(" UAB夹角:{0}度; UAC夹角:{1};\r\n", DUAB.ToString("0.000").PadLeft(7,' '), DUAC.ToString("0.000"));
            datastr = datastr + string.Format(" UIA夹角:{0}度; UIB夹角:{1}; UIC夹角:{2}\r\n", DUIa.ToString("0.000").PadLeft(7,' '), DUIb.ToString("0.000").PadLeft(7,' '), DUIc.ToString("0.000"));
            datastr = datastr + string.Format(" 频率:{0}Hz\r\n", strData.RealFrequency);
            datastr = datastr + string.Format("[A相]有功功率:{0}KWH; [B相]有功功率:{1}KWH; [C相]有功功率:{2}KWH\r\n", (strData.RealWA / 1000).ToString("0.000"), (strData.RealWB / 1000).ToString("0.000"), (strData.RealWC / 1000).ToString("0.000"));
            datastr = datastr + string.Format("[A相]功率因数:{0}; [B相]功率因数:{1}; [C相]功率因数:{2}\r\n", strData.RealPFA.ToString("0.000"), strData.RealPFB.ToString("0.000"), strData.RealPFC.ToString("0.000"));
            return datastr;
        }

        public int WCB_Start(int _index)
        {
            int r = MeterClass.OperationHC_HC310X_Fun(pLngSourceType, ComNo, 55, 5, PhaseWire, 0, lmaxu, lmaxi, 450000f, ref pSngRealConst, ref strData);
            if (WCMCTDNO != 0)
            {
                r = MeterClass.Ini_SecPules(ComNo, 0);
                if (r == 0)
                    WCMCTDNO = 0;
            }
            if (WCBState != 4)
            {
                MeterClass.Answer lUdtAnswer = new MeterClass.Answer();
                r = MeterClass.ErrorOrder_gFun(ComNo, (4 + 12), -1, 0, ref lUdtAnswer);
                if (r == 0)
                    WCBState = 4;
            }
            //if (r == -1)
            {
                lLngPules = (int)(pSngRealConst / lSngCount * lIntCircles);
                r = MeterClass.ErrorCircles_gFun(ComNo, _index, lIntCircles, lLngPules, lSngMaxError, ref strWCBData, ref strWCBPules);
            }
            return r;
        }
        public int WCB_Read(int _index)
        {
            int r = MeterClass.ErrorOrder_gFun(ComNo, 2, _index, 0, ref strWCBData);
            return r;
        }

        public int WCB_SJStart(int _index)
        {
            int r = 0;
            MeterClass.Answer pUdtAnswer = new MeterClass.Answer();
            MeterClass.Pules pUdtPules = new MeterClass.Pules();            
            if (WCMCTDNO != 1)
            {
                r = MeterClass.Ini_SecPules(ComNo, 1);
                if (r == 0)
                    WCMCTDNO = 1;
            }
           
            if (WCBState != 5)
            {
                MeterClass.Answer lUdtAnswer = new MeterClass.Answer();
                r = MeterClass.ErrorOrder_gFun(ComNo, (5 + 12), -1, 0, ref lUdtAnswer);
                if (r == 0)
                    WCBState = 5;
            }
 
            lLngPulesSJ = (int)(5000000 / 50 * lIntCirclesSJ);
            //if (r == 0)
            {
                r = MeterClass.ErrorCircles_gFun(ComNo, _index, lIntCirclesSJ, lLngPulesSJ, lSngMaxError, ref pUdtAnswer, ref pUdtPules);
            }

            if (r == -1)
            {
                WCBSJTest tempWCBSJTest = null;
                WCBSJTests.TryGetValue(_index, out tempWCBSJTest);
                if (tempWCBSJTest == null)
                {
                    r = 0;
                }
                else
                {
                    tempWCBSJTest.WCBTestThrStart();
                }
            }
            return r;
        }
        public int WCB_SJRead(int _index)
        {
            MeterClass.Answer pUdtAnswer = new MeterClass.Answer();
            lLngPulesSJ = (int)(5000000 / 50 * lIntCirclesSJ);
            int r = MeterClass.Error_SecPules(ComNo, _index, lLngPulesSJ, ref pUdtAnswer);

            WCBSJTest tempWCBSJTest = null;
            WCBSJTests.TryGetValue(_index, out tempWCBSJTest);
            if (tempWCBSJTest == null)
            {
                tempWCBSJTest = new WCBSJTest();
                WCBSJTests.TryAdd(_index, tempWCBSJTest);
            }

            if (r == -1)
            {
                tempWCBSJTest.SJValuesList.Add((tempWCBSJTest.SJValuesList.Count + 1), pUdtAnswer);
            }
            return r;
        }
        public int WCB_CLD_Clear(int _index)
        {
            int r = 0;

            WCBSJTest tempWCBSJTest = null;
            if (WCBSJTests.ContainsKey(_index))
            {
                WCBSJTests.TryRemove(_index, out tempWCBSJTest);
                if (tempWCBSJTest != null)
                {
                    tempWCBSJTest.SJValuesList.Clear();
                    try
                    {
                        tempWCBSJTest.wcbsjtestthr.Abort();
                    }
                    catch(Exception ex)
                    {

                    }
                    tempWCBSJTest.wcbsjtestthr = null;
                }
            }

            MeterClass.Answer pUdtAnswer = new MeterClass.Answer();
            MeterClass.Pules pUdtPules = new MeterClass.Pules();
            r = MeterClass.ErrorCircles_gFun(ComNo, _index, 0, 0, 0, ref pUdtAnswer, ref pUdtPules);
            //if (r == -1)
            //    r = MeterClass.Ini_SecPules(ComNo, 0);
            return r;
        }

        private string getPhaseWire(int kind)
        {
            string r = "";
            switch (kind)
            {
                case 0:
                    r = "PT4";
                    break;
                case 1:
                    r = "QT4";
                    break;
                case 2:
                    r = "P32";
                    break;
                case 3:
                    r = "Q32";
                    break;
                case 4:
                    r = "Q60";
                    break;
                case 5:
                    r = "Q90";
                    break;
                case 6:
                    r = "Q33";
                    break;
                case 7:
                    r = "P";
                    break;
            }
            return r;
        }

        //解析转发报文
        public bool DJupCInputToValue(string[] datas)
        {
            setPhaseWire(getPhaseWire(Convert.ToInt32(datas[0])));
            byte[] Uabc = new byte[3] { 1, 1, 1 };
            byte[] Iabc = new byte[3] { 1, 1, 1 };
            
            byte tempbyte = Convert.ToByte(datas[1]);
            if ((tempbyte & Convert.ToByte(Math.Pow(2,0))) == 0)
                Uabc[0] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,1))) == 0)
                Uabc[1] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,2))) == 0)
                Uabc[2] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,3))) == 0)
                Iabc[0] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,4))) == 0)
                Iabc[1] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,5))) == 0)
                Iabc[2] = 0;

            SngVoltage = Convert.ToSingle(datas[2]);
            if (Uabc[0] == 1)
                Ua = SngVoltage;
            else
                Ua = 0;
            if (Uabc[1] == 1)
                Ub = SngVoltage;
            else
                Ub = 0;
            if (Uabc[2] == 1)
                Uc = SngVoltage;
            else
                Uc = 0;

            pSngCurrent = Convert.ToSingle(datas[3]);
            if (Iabc[0] == 1)
                Ia = pSngCurrent;
            else
                Ia = 0;
            if (Uabc[1] == 1)
                Ib = pSngCurrent;
            else
                Ib = 0;
            if (Uabc[2] == 1)
                Ic = pSngCurrent;
            else
                Ic = 0;

            float tempfloat = Convert.ToSingle(datas[4]);
            DUIa = tempfloat;
            DUIb = tempfloat;
            DUIc = tempfloat;

            tempfloat = Convert.ToSingle(datas[5]);
            Frequency = tempfloat;

            tempbyte = Convert.ToByte(datas[6]);
            if ((tempbyte & Convert.ToByte(Math.Pow(2,0))) == 0)
                XBabcIS[0] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,1))) == 0)
                XBabcIS[1] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,2))) == 0)
                XBabcIS[2] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,3))) == 0)
                XBabcIS[3] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,4))) == 0)
                XBabcIS[4] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,5))) == 0)
                XBabcIS[5] = 0;

            if (tempbyte > 0)
                WaveType = 3;
            else
                WaveType = 1;

            this.setDUABDUAC(0, 120, 240);

            this.XJIa = (this.XJUa + this.DUIa);
            this.XJIb = (this.XJUb + this.DUIb);
            this.XJIc = (this.XJUc + this.DUIc);
            return true;
        }
        public bool GJupCInputToValue(string[] datas)
        {
            setPhaseWire(getPhaseWire(Convert.ToInt32(datas[0])));
            byte tempbyte = Convert.ToByte(datas[1]);
            byte[] Uabc = new byte[3] { 1, 1, 1 };
            byte[] Iabc = new byte[3] { 1, 1, 1 };
            
            if ((tempbyte & Convert.ToByte(Math.Pow(2,0))) == 0)
                Uabc[0] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,1))) == 0)
                Uabc[1] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,2))) == 0)
                Uabc[2] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,3))) == 0)
                Iabc[0] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,4))) == 0)
                Iabc[1] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,5))) == 0)
                Iabc[2] = 0;

            float tempfloat = Convert.ToSingle(datas[2]);
            Frequency = tempfloat;

            tempfloat = Convert.ToSingle(datas[3]);
            SngVoltage = tempfloat;
            if (Uabc[0] == 1)
            {
                Ua = tempfloat;
            }
            else
                Ua = 0;
            tempfloat = Convert.ToSingle(datas[5]);
            if (Uabc[1] == 1)
            {
                Ub = tempfloat;
                if (SngVoltage < Ub)
                    SngVoltage = Ub;
            }
            else
                Ub = 0;
            tempfloat = Convert.ToSingle(datas[7]);
            if (Uabc[2] == 1)
            {
                Uc = tempfloat;
                if (SngVoltage < Uc)
                    SngVoltage = Uc;
            }
            else
                Uc = 0;

            setDUABDUAC(Convert.ToSingle(datas[4]), Convert.ToSingle(datas[6]), Convert.ToSingle(datas[8]));

            tempfloat = Convert.ToSingle(datas[9]);
            pSngCurrent = tempfloat;
            if (Iabc[0] == 1)
            {
                Ia = tempfloat;
            }
            else
                Ia = 0;
            tempfloat = Convert.ToSingle(datas[11]);
            if (Uabc[1] == 1)
            {
                Ib = tempfloat;
                if (pSngCurrent < Ib)
                    pSngCurrent = Ib;
            }
            else
                Ib = 0;
            tempfloat = Convert.ToSingle(datas[13]);
            if (Uabc[2] == 1)
            {
                Ic = tempfloat;
                if (pSngCurrent < Ic)
                    pSngCurrent = Ic;
            }
            else
                Ic = 0;

            setDUIaDUIbDUIc(Convert.ToSingle(datas[4]), Convert.ToSingle(datas[6]), Convert.ToSingle(datas[8]), Convert.ToSingle(datas[10]), Convert.ToSingle(datas[12]), Convert.ToSingle(datas[14]));

            tempbyte = Convert.ToByte(datas[15]);
            if ((tempbyte & Convert.ToByte(Math.Pow(2,0))) == 0)
                XBabcIS[0] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,1))) == 0)
                XBabcIS[1] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,2))) == 0)
                XBabcIS[2] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,3))) == 0)
                XBabcIS[3] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,4))) == 0)
                XBabcIS[4] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2,5))) == 0)
                XBabcIS[5] = 0;
            if (tempbyte > 0)
                WaveType = 3;
            else
                WaveType = 1;
            return true;
        }
        public bool GJupCInputToValue99(string[] datas)
        {
            setPhaseWire(getPhaseWire(Convert.ToInt32(datas[1])));
            byte tempbyte = Convert.ToByte(datas[2]);
            byte[] Uabc = new byte[3] { 1, 1, 1 };
            byte[] Iabc = new byte[3] { 1, 1, 1 };

            if ((tempbyte & Convert.ToByte(Math.Pow(2, 0))) == 0)
                Uabc[0] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 1))) == 0)
                Uabc[1] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 2))) == 0)
                Uabc[2] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 3))) == 0)
                Iabc[0] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 4))) == 0)
                Iabc[1] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 5))) == 0)
                Iabc[2] = 0;

            float tempfloat = Convert.ToSingle(datas[3]);
            Frequency = tempfloat;

            tempfloat = Convert.ToSingle(datas[4]);
            SngVoltage = tempfloat;
            if (Uabc[0] == 1)
            {
                Ua = tempfloat;
            }
            else
                Ua = 0;
            tempfloat = Convert.ToSingle(datas[6]);
            if (Uabc[1] == 1)
            {
                Ub = tempfloat;
                if (SngVoltage < Ub)
                    SngVoltage = Ub;
            }
            else
                Ub = 0;
            tempfloat = Convert.ToSingle(datas[8]);
            if (Uabc[2] == 1)
            {
                Uc = tempfloat;
                if (SngVoltage < Uc)
                    SngVoltage = Uc;
            }
            else
                Uc = 0;

            setDUABDUAC(Convert.ToSingle(datas[5]), Convert.ToSingle(datas[7]), Convert.ToSingle(datas[9]));

            tempfloat = Convert.ToSingle(datas[10]);
            pSngCurrent = tempfloat;
            if (Iabc[0] == 1)
            {
                Ia = tempfloat;
            }
            else
                Ia = 0;
            tempfloat = Convert.ToSingle(datas[12]);
            if (Uabc[1] == 1)
            {
                Ib = tempfloat;
                if (pSngCurrent < Ib)
                    pSngCurrent = Ib;
            }
            else
                Ib = 0;
            tempfloat = Convert.ToSingle(datas[14]);
            if (Uabc[2] == 1)
            {
                Ic = tempfloat;
                if (pSngCurrent < Ic)
                    pSngCurrent = Ic;
            }
            else
                Ic = 0;

            setDUIaDUIbDUIc(Convert.ToSingle(datas[5]), Convert.ToSingle(datas[7]), Convert.ToSingle(datas[9]), Convert.ToSingle(datas[11]), Convert.ToSingle(datas[13]), Convert.ToSingle(datas[15]));

            tempbyte = Convert.ToByte(datas[16]);
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 0))) == 0)
                XBabcIS[0] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 1))) == 0)
                XBabcIS[1] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 2))) == 0)
                XBabcIS[2] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 3))) == 0)
                XBabcIS[3] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 4))) == 0)
                XBabcIS[4] = 0;
            if ((tempbyte & Convert.ToByte(Math.Pow(2, 5))) == 0)
                XBabcIS[5] = 0;
            if (tempbyte > 0)
                WaveType = 3;
            else
                WaveType = 1;
            return true;
        }
        public bool XBsetCInputToValue(string[] datas)
        {
            float tempbyte = Convert.ToByte(datas[0]);
            string[] xbdatas = datas[1].Split('-');
            if (xbdatas.Length < 3)
                return false;

            if (tempbyte == 0)
            {
                WaveDegreeA = Convert.ToSingle(xbdatas[2]);       //谐波初相角1，0-359.99
                WaveTimesA = Convert.ToInt32(xbdatas[0]);       //谐波次数1，0-21
                VoltageWaveRateA = Convert.ToInt32(xbdatas[1]) * 100;  //电压谐波含量1，0-40
            }
            if (tempbyte == 3)
            {
                WaveDegreeA = Convert.ToSingle(xbdatas[2]);       //谐波初相角1，0-359.99
                WaveTimesA = Convert.ToInt32(xbdatas[0]);       //谐波次数1，0-21
                CurrentWaveRateA = Convert.ToInt32(xbdatas[1]) * 100; //电流谐波含量1，0-40
            }
            if (tempbyte == 1)
            {
                WaveDegreeB = Convert.ToSingle(xbdatas[2]);       //谐波初相角1，0-359.99
                WaveTimesB = Convert.ToInt32(xbdatas[0]);       //谐波次数1，0-21
                VoltageWaveRateB = Convert.ToInt32(xbdatas[1]);  //电压谐波含量1，0-40
            }
            if (tempbyte == 4)
            {
                WaveDegreeB = Convert.ToSingle(xbdatas[2]);       //谐波初相角1，0-359.99
                WaveTimesB = Convert.ToInt32(xbdatas[0]);       //谐波次数1，0-21
                CurrentWaveRateB = Convert.ToInt32(xbdatas[1]); //电流谐波含量1，0-40
            }
            if (tempbyte == 2)
            {
                WaveDegreeC = Convert.ToSingle(xbdatas[2]);       //谐波初相角1，0-359.99
                WaveTimesC = Convert.ToInt32(xbdatas[0]);       //谐波次数1，0-21
                VoltageWaveRateC = Convert.ToInt32(xbdatas[1]);  //电压谐波含量1，0-40
            }
            if (tempbyte == 5)
            {
                WaveDegreeC = Convert.ToSingle(xbdatas[2]);       //谐波初相角1，0-359.99
                WaveTimesC = Convert.ToInt32(xbdatas[0]);       //谐波次数1，0-21
                CurrentWaveRateC = Convert.ToInt32(xbdatas[1]); //电流谐波含量1，0-40
            }
            return true;
        }

        //生成回复报文
        public string XBCValueToOutput(string cmdret, int bwindex)
        {
            if (cmdret == "990301")
            {
                float tempua, tempub, tempuc, tempia, tempib, tempic;
                if ((strData.RealVA > 1) && (strData.RealVA > 1) && (strData.RealVA > 1))
                {
                    tempua = XJUa;
                    tempub = XJUb;
                    tempuc = XJUc;
                }
                else
                {
                    tempua = 0;
                    tempub = 0;
                    tempuc = 0;
                }
                if ((strData.RealAA > 0) && (strData.RealAA > 0) && (strData.RealAA > 0))
                {
                    tempia = XJIa;
                    tempib = XJIb;
                    tempic = XJIc;
                }
                else
                {
                    tempia = 0;
                    tempib = 0;
                    tempic = 0;
                }
                string datastr = "cmd=" + cmdret + ",ret=0," + "data=" + (bwindex + 1) + ";";
                datastr = datastr + strData.RealVA + ";" + tempua + ";" + strData.RealVB + ";" + tempub + ";" + strData.RealVC + ";" + tempuc;
                datastr = datastr + ";" + strData.RealAA + ";" + tempia + ";" + strData.RealAB + ";" + tempib + ";" + strData.RealAC + ";" + tempic;
                datastr = datastr + ";" + strData.RealPFA + ";" + strData.RealPFB + ";" + strData.RealPFC + ";" + strData.RealWA + ";" + strData.RealWB + ";" + strData.RealWC;
                datastr = datastr + ";" + strData.RealP + ";" + strData.RealQ + ";" + strData.RealPF + ";" + strData.RealFrequency;
                return datastr;
            }
            else
            {
                float tempua, tempub, tempuc, tempia, tempib, tempic;
                if ((strData.RealVA > 1) && (strData.RealVA > 1) && (strData.RealVA > 1))
                {
                    tempua = XJUa;
                    tempub = XJUb;
                    tempuc = XJUc;
                }
                else
                {
                    tempua = 0;
                    tempub = 0;
                    tempuc = 0;
                }
                if ((strData.RealAA > 1) && (strData.RealAA > 1) && (strData.RealAA > 1))
                {
                    tempia = XJIa;
                    tempib = XJIb;
                    tempic = XJIc;
                }
                else
                {
                    tempia = 0;
                    tempib = 0;
                    tempic = 0;
                }
                string datastr = "cmd=" + cmdret + ",ret=0," + "data=";
                datastr = datastr + strData.RealVA + ";" + tempua + ";" + strData.RealVB + ";" + tempub + ";" + strData.RealVC + ";" + tempuc;
                datastr = datastr + ";" + strData.RealAA + ";" + tempia + ";" + strData.RealAB + ";" + tempib + ";" + strData.RealAC + ";" + tempic;
                datastr = datastr + ";" + strData.RealP + ";" + strData.RealQ + ";" + strData.RealPF + ";" + strData.RealFrequency;
                return datastr;
            }
        }

        //设误差表
        public bool WCBsetInputToValue(string[] datas)
        {
            lIntCircles = Convert.ToInt32(datas[1]);
            lSngCount = Convert.ToInt32(datas[2]);
            lmaxu = Convert.ToSingle(datas[3]);
            lmaxi = Convert.ToSingle(datas[4]);
            return true;
        }
        public string WCBValueToOutput(string cmdret, int _index)
        {
            string datastr = string.Format("cmd={0},ret=0,data={1};{2}", cmdret, _index, strWCBData.SngAnswer);
            return datastr;
        }

        public bool WCBSJsetInputToValue(String[] datas, int _index)
        {
            lIntCirclesSJ = Convert.ToInt32(datas[1]);    //时间间隔
            MaxNumSJ = Convert.ToInt32(datas[2]);

            WCBSJTest tempWCBSJTest = null;
            if (WCBSJTests.ContainsKey(_index))
            {
                WCBSJTests.TryRemove(_index, out tempWCBSJTest);
                if (tempWCBSJTest != null)
                {
                    if (tempWCBSJTest.wcbsjtestthr != null)
                    {
                        try
                        {
                            tempWCBSJTest.wcbsjtestthr.Abort();
                            Thread.Sleep(500);
                            tempWCBSJTest.wcbsjtestthr = null;
                            tempWCBSJTest.XHH -= WCB_SJRead;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            tempWCBSJTest = new WCBSJTest();
            WCBSJTests.TryAdd(_index, tempWCBSJTest);

            tempWCBSJTest.lIntCirclesSJ = lIntCirclesSJ;
            tempWCBSJTest.MaxNumSJ = MaxNumSJ;
            tempWCBSJTest.bwindex = _index;
            tempWCBSJTest.XHH += WCB_SJRead;
            return true;
        }
        public string WCBSJValueToOutput(string cmdret, int _index)
        {
            WCBSJTest tempWCBSJTest = null;
            bool f = false;
            float rvalue = 0;
            if (WCBSJTests.ContainsKey(_index))
            {
                WCBSJTests.TryGetValue(_index, out tempWCBSJTest);
            }
            if (tempWCBSJTest != null)
            {
                if (tempWCBSJTest.wcbsjtestthr != null)
                {
                    try
                    {
                        tempWCBSJTest.wcbsjtestthr.Abort();
                    }
                    catch(Exception ex)
                    {

                    }
                    tempWCBSJTest.wcbsjtestthr = null;
                    tempWCBSJTest.XHH -= WCB_SJRead;

                    float forntkey = 0;
                    int maxindex = 0;
                    float maxvalue = 0;
                    foreach (KeyValuePair<int, MeterClass.Answer> keyvalue in tempWCBSJTest.SJValuesList)
                    {
                        if (keyvalue.Value.SngTimes > 0)
                        {
                            if (forntkey != keyvalue.Value.SngTimes)
                            {
                                maxvalue = maxvalue + keyvalue.Value.SngAnswer;
                                maxindex++;
                                forntkey = keyvalue.Value.SngTimes;
                            }
                        }
                    }
                    rvalue = maxvalue / tempWCBSJTest.MaxNumSJ * 24 * 60 * 60 * 1000;
                    if (maxindex == tempWCBSJTest.MaxNumSJ)
                        f = true;
                }
            }
            string datastr = "";
            if (f)
                datastr = string.Format("cmd={0},ret=0,data={1}", cmdret, rvalue);
            else
                datastr = string.Format("cmd={0},ret=1,data=null", cmdret);
            return datastr;
        }

    }
}
