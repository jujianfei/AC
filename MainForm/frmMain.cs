using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using 三湘源涵普Test;
using System.IO.Ports;
using System.Threading;

namespace MainForm
{
    public partial class frmMain : Form
    {
        #region 创建托盘图标对象 
        Icon ico;
        #endregion

        private MainApplicationClass m_Application;
        Channel _server = new Channel();

        internal frmMain(MainApplicationClass application)
        {
            this.m_Application = application;
            this.m_Application.m_MainForm = this;
            this.m_Application.m_CurrentAccount = m_Application.SecuritySolution.AccountValidate("admin", "admin");

            //this.m_Application.SetSystemConfig(this.m_ControlGroupSystemConfig);

            InitializeComponent();

            ContrulValueLoad();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                ico = new Icon("Krypton.ico");
                //设置鼠标放在托盘图标上面的文字 
                this.notifyIcon1.Text = "前置机";

                IPHostEntry localhost = Dns.GetHostEntry(Dns.GetHostName());
                for (int i = 0; i < localhost.AddressList.Length; i++)
                {
                    if (localhost.AddressList[i].ToString().Split('.').Length == 4)
                        txtIP.Text = localhost.AddressList[1].ToString();
                }
                if (txtIP.Text == "")
                    txtIP.Text = "读取IP失败！";

                cbjx.SelectedIndex = 0;

                Assembly thisAssembly = Assembly.GetExecutingAssembly();
                this.Text = string.Format("{0} {1}", this.Text, thisAssembly.GetName().Version.ToString());

                _server = m_Application.GetSystemConfig(typeof(Channel)) as Channel;
                //m_Application.SetSystemConfig(_server);
                _server.EventMessage += server_EventMessage;
                _server.SaveTCLClientOpens += HSever_SaveTCLClientOpens;
                _server.EventEndWaitForm += new Channel.EndWaitFormEventHandler(OnCloseWaitForm);
                _server.ExecALLThreadInit(m_Application);
                _server.exceallthread.NowDSPOutputValue = new DSPOutputValueCls();
                _server.exceallthread.NowDSPOutputValue.ComNo = _server.DSPCOM;

                this.splitContainerALL.FixedPanel = FixedPanel.Panel1;
                this.splitContainerALL.IsSplitterFixed = true;
                this.numericUpDownCOM.Value = _server.DSPCOM;
                pagegj.RightMargin = pagegj.Size.Width - 25;
                //pagegj.Refresh();
                pagetcpclient.RightMargin = pagetcpclient.Size.Width - 25;
                //pagetcpclient.Refresh();

                LoadBTGNCS();
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("frmMain", "frmMain_Load", "", "", ex.ToString());
                MessageBox.Show("初始化数据失败！" + ex.Message, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Thread tempthread = new Thread(StartToLink);
            tempthread.IsBackground = true;
            tempthread.Start();
        }
        private void StartToLink()
        {
            this.Invoke((EventHandler)delegate
            {
                try
                {
                    if (!_server.exceallthread.NowBTGNCScls.initSelDevice485(_server))
                    {
                        MessageBox.Show("表位设备初始化失败！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _server.udpPort = (int)numPort.Value;
                    Thread tempthread = new Thread(this._server.Start);
                    tempthread.IsBackground = true;
                    tempthread.Start();
                   
                    this.EndWaitFormEvent -= this.EndLinkEvent;
                    this.EndWaitFormEvent += this.EndLinkEvent;
                    this.splitContainerALL.Enabled = false;
                    this.OpenWaitForm();
                }
                catch (Exception ex)
                {

                }
            });
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }
        
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否最小化到物理托盘？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                e.Cancel = true;

                this.WindowState = FormWindowState.Minimized;

                //注意notifyIcon1是控件的名字而不是对象的名字 
                notifyIcon1.Icon = ico;
                //隐藏任务栏区图标 
                this.ShowInTaskbar = false;

                //图标显示在托盘区 
                notifyIcon1.Visible = true;
            }
            else
            {
                if (MessageBox.Show("退出程序将使一切通过本程序转发数据的实验通信失败，您确定要退出吗？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    _server.Stop();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        #region 还原窗体
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //判断是否已经最小化于托盘 
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示 
                WindowState = FormWindowState.Normal;

                //激活窗体并给予它焦点 
                this.Activate();

                //托盘区图标隐藏 
                notifyIcon1.Visible = false;

                //任务栏区显示图标 
                this.ShowInTaskbar = true;
            }
        }
        #endregion
    
        #region << 通道服务产生消息的事件 >>

        private SortedDictionary<long, messageshow> messagesList = new SortedDictionary<long, messageshow>();
        private System.Timers.Timer exectime;
        private bool runningis = false;
        private void timer_tick(object sender, EventArgs e)
        {
            runningis = true;
            runmessageshow();
        }
        private void runmessageshow()
        {
            try
            {
                long timekey = 0;
                foreach (KeyValuePair<long, messageshow> keyvalue in messagesList)
                {
                    timekey = keyvalue.Key;
                    pagegj.BeginInvoke(new HMessageShow(this.HMessageShow_Invoke), new object[] { keyvalue.Key, keyvalue.Value });
                    messagesList.Remove(timekey);
                    break;
                }
                
                bool f = false;
                if (messagesList.Count > 0)
                {
                    f = true;
                }
                if (!f)
                {
                    runningis = false;
                    exectime.Stop();
                }
                else
                {
                    exectime.Start();
                }
            }
            catch(Exception ex)
            {
                runningis = false;
                exectime.Stop();
            }
        }
        void server_EventMessage(ChannelMessageTypeOptions messageType, string message)
        {
            //try
            //{
            //    messageshow tempmessageshow = new messageshow();
            //    tempmessageshow.message = message;
            //    tempmessageshow.messageType = messageType;
            //    long timekey = DateTime.Now.Ticks;
                
            //    while (messagesList.ContainsKey(timekey))
            //    {
            //        timekey++;
            //    }
            //    messagesList.Add(DateTime.Now.Ticks, tempmessageshow);
            //    if (exectime == null)
            //    {
            //        exectime = new System.Timers.Timer();
            //        exectime.Elapsed += new System.Timers.ElapsedEventHandler(timer_tick);
            //        exectime.Interval = 50;
            //        exectime.AutoReset = false;
            //        exectime.Start();
            //    }
            //    else
            //    {
            //        if (runningis == false)
            //        {
            //            exectime.Start();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
            pagegj.BeginInvoke(new HMessageShow(this.HMessageShow_Invoke), new object[] { messageType, message });
        }

        private delegate void HMessageShow(ChannelMessageTypeOptions messageType, string message);
        private void HMessageShow_Invoke(ChannelMessageTypeOptions messageType, string message)
        {
            try
            {
                #region 如果是抄表的，没选就不显示
                if (!(cbcbshowis1.Checked))
                {
                    if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB1) || (messageType == ChannelMessageTypeOptions.UDPsendCB1) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB1) || (messageType == ChannelMessageTypeOptions.MOXAsendCB1))
                    {
                        return;
                    }
                }
                if (!(cbcbshowis2.Checked))
                {
                    if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB2) || (messageType == ChannelMessageTypeOptions.UDPsendCB2) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB2) || (messageType == ChannelMessageTypeOptions.MOXAsendCB2))
                    {
                        return;
                    }
                }
                if (!(cbcbshowis3.Checked))
                {
                    if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB3) || (messageType == ChannelMessageTypeOptions.UDPsendCB3) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB3) || (messageType == ChannelMessageTypeOptions.MOXAsendCB3))
                    {
                        return;
                    }
                }
                if (!(cbcbshowis4.Checked))
                {
                    if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB4) || (messageType == ChannelMessageTypeOptions.UDPsendCB4) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB4) || (messageType == ChannelMessageTypeOptions.MOXAsendCB4))
                    {
                        return;
                    }
                }
                if (!(cbcbshowis5.Checked))
                {
                    if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB5) || (messageType == ChannelMessageTypeOptions.UDPsendCB5) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB5) || (messageType == ChannelMessageTypeOptions.MOXAsendCB5))
                    {
                        return;
                    }
                }
                if (!(cbcbshowis6.Checked))
                {
                    if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB6) || (messageType == ChannelMessageTypeOptions.UDPsendCB6) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB6) || (messageType == ChannelMessageTypeOptions.MOXAsendCB6))
                    {
                        return;
                    }
                }
                if (!(cbcbshowis7.Checked))
                {
                    if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB7) || (messageType == ChannelMessageTypeOptions.UDPsendCB7) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB7) || (messageType == ChannelMessageTypeOptions.MOXAsendCB7))
                    {
                        return;
                    }
                }
                if (!(cbcbshowis8.Checked))
                {
                    if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB8) || (messageType == ChannelMessageTypeOptions.UDPsendCB8) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB8) || (messageType == ChannelMessageTypeOptions.MOXAsendCB8))
                    {
                        return;
                    }
                }
                #endregion 

                if ((messageType == ChannelMessageTypeOptions.MOXAreceive) || (messageType == ChannelMessageTypeOptions.MOXAsend) || (messageType == ChannelMessageTypeOptions.MOXAError) || (messageType == ChannelMessageTypeOptions.MOXAInfo) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB1) || (messageType == ChannelMessageTypeOptions.MOXAsendCB1) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB2) || (messageType == ChannelMessageTypeOptions.MOXAsendCB2) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB3) || (messageType == ChannelMessageTypeOptions.MOXAsendCB3) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB4) || (messageType == ChannelMessageTypeOptions.MOXAsendCB4) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB5) || (messageType == ChannelMessageTypeOptions.MOXAsendCB5) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB6) || (messageType == ChannelMessageTypeOptions.MOXAsendCB6) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB7) || (messageType == ChannelMessageTypeOptions.MOXAsendCB7) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB8) || (messageType == ChannelMessageTypeOptions.MOXAsendCB8))
                {
                    if (pagetcpclient.TextLength >= 33333)
                        pagetcpclient.Clear();

                    pagetcpclient.SelectionStart = pagetcpclient.TextLength;

                    pagetcpclient.SelectionColor = Color.DarkGray;
                    pagetcpclient.AppendText(DateTime.Now.ToString("MM-dd HH:mm:ss") + " ");
                    pagetcpclient.AppendText(messageType.GetDescription() + " ");
                    if ((messageType == ChannelMessageTypeOptions.Error) || (messageType == ChannelMessageTypeOptions.UDPError) || (messageType == ChannelMessageTypeOptions.MOXAError))
                        pagetcpclient.SelectionColor = Color.Red;
                    else
                        pagetcpclient.SelectionColor = Color.Blue;

                    pagetcpclient.AppendText(message + "\r\n");

                    pagetcpclient.SelectionStart = pagetcpclient.TextLength;
                    //pagetcpclient.Focus();
                }
                else
                {
                    if (pagegj.TextLength >= 33333)
                        pagegj.Clear();

                    pagegj.SelectionStart = pagegj.TextLength;

                    pagegj.SelectionColor = Color.DarkGray;
                    pagegj.AppendText(DateTime.Now.ToString("MM-dd HH:mm:ss") + " ");
                    pagegj.AppendText(messageType.GetDescription() + " ");
                    if ((messageType == ChannelMessageTypeOptions.Error) || (messageType == ChannelMessageTypeOptions.UDPError) || (messageType == ChannelMessageTypeOptions.MOXAError))
                        pagegj.SelectionColor = Color.Red;
                    else
                        pagegj.SelectionColor = Color.Blue;

                    pagegj.AppendText(message + "\r\n");

                    pagegj.SelectionStart = pagegj.TextLength;
                    //pagegj.Focus();
                }

                if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB1) || (messageType == ChannelMessageTypeOptions.UDPsendCB1) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB1) || (messageType == ChannelMessageTypeOptions.MOXAsendCB1))
                {
                    TXTWrite.WriteCBInfo(messageType.GetDescription() + " " + message, 1);
                }
                else if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB2) || (messageType == ChannelMessageTypeOptions.UDPsendCB2)|| (messageType == ChannelMessageTypeOptions.MOXAreceiveCB2) || (messageType == ChannelMessageTypeOptions.MOXAsendCB2))
                {
                    TXTWrite.WriteCBInfo(messageType.GetDescription() + " " + message, 2);
                }
                else if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB3) || (messageType == ChannelMessageTypeOptions.UDPsendCB3)|| (messageType == ChannelMessageTypeOptions.MOXAreceiveCB3) || (messageType == ChannelMessageTypeOptions.MOXAsendCB3))
                {
                    TXTWrite.WriteCBInfo(messageType.GetDescription() + " " + message, 3);
                }
                else if((messageType == ChannelMessageTypeOptions.UDPreceiveCB4) || (messageType == ChannelMessageTypeOptions.UDPsendCB4)|| (messageType == ChannelMessageTypeOptions.MOXAreceiveCB4) || (messageType == ChannelMessageTypeOptions.MOXAsendCB4))
                {
                    TXTWrite.WriteCBInfo(messageType.GetDescription() + " " + message, 4);
                }
                else if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB5) || (messageType == ChannelMessageTypeOptions.UDPsendCB5)|| (messageType == ChannelMessageTypeOptions.MOXAreceiveCB5) || (messageType == ChannelMessageTypeOptions.MOXAsendCB5))
                {
                    TXTWrite.WriteCBInfo(messageType.GetDescription() + " " + message, 5);
                }
                else if((messageType == ChannelMessageTypeOptions.UDPreceiveCB6) || (messageType == ChannelMessageTypeOptions.UDPsendCB6)|| (messageType == ChannelMessageTypeOptions.MOXAreceiveCB6) || (messageType == ChannelMessageTypeOptions.MOXAsendCB6))
                {
                    TXTWrite.WriteCBInfo(messageType.GetDescription() + " " + message, 6);
                }
                else if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB7) || (messageType == ChannelMessageTypeOptions.UDPsendCB7) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB7) || (messageType == ChannelMessageTypeOptions.MOXAsendCB7))
                {
                    TXTWrite.WriteCBInfo(messageType.GetDescription() + " " + message, 7);
                }
                else if ((messageType == ChannelMessageTypeOptions.UDPreceiveCB8) || (messageType == ChannelMessageTypeOptions.UDPsendCB8) || (messageType == ChannelMessageTypeOptions.MOXAreceiveCB8) || (messageType == ChannelMessageTypeOptions.MOXAsendCB8))
                {
                    TXTWrite.WriteCBInfo(messageType.GetDescription() + " " + message, 8);
                }
                else
                {
                    TXTWrite.WriteInfo(messageType.GetDescription() + " " + message);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region << 记录通道TCPClient打开端口以便崩溃重启事件 >>
        private void HSever_SaveTCLClientOpens()
        {
            try
            {
                m_Application.SetSystemConfig(_server);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 等待页面

        frmWait waitfrm = null;

        /// <summary>
        /// 等待滚屏结束时的事件所调用的委托。
        /// </summary>
        public delegate void EndWaitFormEventHandler();

        /// <summary>
        /// 结束等待后需要执行的事件，加载需要执行的函数
        /// </summary>
        public event EndWaitFormEventHandler EndWaitFormEvent;
        private void OnEndWaitFormEvent()
        {
            try
            {
                if (this.EndWaitFormEvent != null)
                {
                    this.EndWaitFormEvent();
                }
            }
            catch { }
        }

        public object LockWaitFormobj = new object();
        /// <summary>
        /// 关闭等待界面事件，需要等待的事件调用
        /// </summary>
        public void OnCloseWaitForm()
        {
            this.BeginInvoke(new EndWaitFormEventHandler(this.CloseWaitForm));
        }
        public void CloseWaitForm()
        {
            try
            {
                Thread.Sleep(1500);
                lock (LockWaitFormobj)
                {
                    if (this.waitfrm != null)
                    {
                        this.waitfrm.SendToBack();
                        this.waitfrm.Visible = false; ;
                    }
                }
            }
            catch { }
            finally
            {
                OnEndWaitFormEvent();
            }
        }

        public void OpenWaitForm()
        {
            try
            {
                lock (LockWaitFormobj)
                {
                    if (this.waitfrm == null)
                    {
                        waitfrm = new frmWait();
                        waitfrm.TopLevel = false;
                        waitfrm.Location = new Point((this.Size.Width - waitfrm.Size.Width) / 2, (this.Size.Height - waitfrm.Size.Height) / 2);
                        this.Controls.Add(waitfrm);
                        waitfrm.BringToFront();
                        waitfrm.Show();
                    }
                    else
                    {
                        waitfrm.Location = new Point((this.Size.Width - waitfrm.Size.Width) / 2, (this.Size.Height - waitfrm.Size.Height) / 2);
                        waitfrm.BringToFront();
                        waitfrm.Visible = true;
                    }
                }
            }
            catch (Exception ex) { }
        }
        #endregion

        #region 控件
        public void EndLinkEvent()
        {
            this.EndWaitFormEvent -= this.EndLinkEvent;
            if (this._server.State == ChannelStateOptions.Running)
            {
                this.btnServOptions.Text = "关闭连接";
                this.btnServOptions.ForeColor = Color.Red;
                m_Application.SetSystemConfig(_server);
            }
            else
            {
                this.btnServOptions.Text = "打开连接";
                this.btnServOptions.ForeColor = Color.Black;
            }
            this.splitContainerALL.BringToFront();
            this.splitContainerALL.Enabled = true;
        }

        private void btnServOptions_Click(object sender, EventArgs e)
        {
            try
            {
                if (_server.State != ChannelStateOptions.Running)
                {
                    if (MessageBox.Show("监听端口 [" + numPort.Value.ToString() + "] ，确定要启动服务程序吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                    {

                        if (!_server.exceallthread.NowBTGNCScls.initSelDevice485(_server))
                        {
                            MessageBox.Show("表位设备初始化失败！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        _server.DSPCOM = Convert.ToInt32(this.numericUpDownCOM.Value);
                        _server.udpPort = (int)numPort.Value;
                        Thread tempthread = new Thread(this._server.Start);
                        tempthread.IsBackground = true;
                        tempthread.Start();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (MessageBox.Show("停止通道会造成主程序测试停止，确定要停止服务程序吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                    {
                        Thread tempthread = new Thread(this._server.Stop);
                        tempthread.IsBackground = true;
                        tempthread.Start();
                    }
                    else
                    {
                        return;
                    }
                }

                this.EndWaitFormEvent -= this.EndLinkEvent;
                this.EndWaitFormEvent += this.EndLinkEvent;
                this.splitContainerALL.Enabled = false;
                this.OpenWaitForm();
            }
            catch (Exception ex)
            {

            }
        }

        private System.Timers.Timer CSTIMER;
        private void CSTIMERtick(object sender, EventArgs e)
        {
            while (cbcbshowis.Checked)
            {
                for (int i = 1; i < 3; i++)
                {
                    server_EventMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("=======表位{0}=======", i));
                    string cmdop = string.Format("FE FE FE 05 0{0} 03 00 00 00 00 00 00 {1} AA 0D 0A", i, (3 + i).ToString("X2"));
                    string cmdcls = string.Format("FE FE FE 05 0{0} 03 01 00 00 00 00 00 {1} AA 0D 0A", i, (3 + i + 1).ToString("X2"));
                    ExecData ed = new ExecData(cmdop, new IPEndPoint(IPAddress.Parse("192.168.127.101"), 4003));
                    TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, (i - 1));
                    bool f = _server.exceallthread.NowBTGNCScls.btgns[i - 1].SendTDDDYup(lnk, ed, null);
                    if (!f)
                        server_EventMessage(ChannelMessageTypeOptions.MOXAError, string.Format("尼玛开出错！"));
                    //Thread.Sleep(150);
                    ed = new ExecData(cmdcls, new IPEndPoint(IPAddress.Parse("192.168.127.101"), 4003));
                    lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, (i - 1));
                    f = _server.exceallthread.NowBTGNCScls.btgns[i - 1].SendTDDDYdown(lnk, ed, null);
                    if (!f)
                        server_EventMessage(ChannelMessageTypeOptions.MOXAError, string.Format("尼玛关出错！"));
                    //Thread.Sleep(150);
                }
            }
        }
        private void btnRefreshTT_Click(object sender, EventArgs e)
        {
            //if (CSTIMER == null)
            //{
            //    CSTIMER = new System.Timers.Timer();
            //    CSTIMER.Elapsed += new System.Timers.ElapsedEventHandler(CSTIMERtick);
            //    CSTIMER.Interval = 50;
            //    CSTIMER.AutoReset = false;
            //}
            //CSTIMER.Start();
            //return;
            try
            {
                _server.exceallthread.timerwait();
                _server.refreshTCPClient();
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("frmMain", "btnRefreshTT_Click","","",ex.ToString());
                MessageBox.Show(ex.Message, "系统告警", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            pagegj.Clear();
        }
        private void tcpclientbtnclear_Click(object sender, EventArgs e)
        {
            pagetcpclient.Clear();
        }

        private void udpbtnstate_Click(object sender, EventArgs e)
        {
            _server.udpgetstate();
        }
        private void tcpclientbtnstate_Click(object sender, EventArgs e)
        {
             _server.tcpcliengettstate();
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (cbControlIS.Checked)
            {
                MessageBox.Show("已勾选 不通过电科院协议直接控制台体！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
        }
        #endregion

        #region 标准功率源
        private long mHanle = 0;                                //电源句柄
        
        #region 数据控件值初始化
        private void ContrulValueLoad()
        {
            cbjx.Text = "PT4";
            cbpl.Text = "" + 50;
            cbbx.Text = "正弦波";
            cbsc.Text = "调整";

            cbdy.Text = "" + 220;
            cbSAdy.Checked = true;
            cbSBdy.Checked = true;
            cbSCdy.Checked = true;
            cbdl.Text = "" + 2.5;
            cbSAdl.Checked = true;
            cbSBdl.Checked = true;
            cbSCdl.Checked = true;
            cbdydljj.Text = "" + 0;

            cbAdy.Text = "" + 220;
            cbBdy.Text = "" + 220;
            cbCdy.Text = "" + 220;

            cbAdyjj.Text = "" + 0;
            cbBdyjj.Text = "" + 120;
            cbCdyjj.Text = "" + 240;

            cbAdl.Text = "" + 2.5;
            cbBdl.Text = "" + 2.5;
            cbCdl.Text = "" + 2.5;

            cbAdljj.Text = "" + 0;
            cbBdljj.Text = "" + 120;
            cbCdljj.Text = "" + 240;

            cbaxbcs.Text = "" + 21;
            cbbxbcs.Text = "" + 0;
            cbcxbcs.Text = "" + 0;

            cbaxbxwcj.Text = "" + 60;
            cbbxbxwcj.Text = "" + 0;
            cbcxbxwcj.Text = "" + 0;

            cbaxbdyhl.Text = "" + 10;
            cbbxbdyhl.Text = "" + 0;
            cbcxbdyhl.Text = "" + 0;

            cbaxbdlhl.Text = "" + 10;
            cbbxbdlhl.Text = "" + 0;
            cbcxbdlhl.Text = "" + 0;
        }
        #endregion

        #region 按钮控件
        /*
         * 高级生源，三项分别设置输出，调用的dll函数，无需单独处理串口关闭
         */
        private void btngjsy_Click(object sender, EventArgs e)
        {
            int r = 0;
            try
            {
                string strrusult = "";
                if (!checkTextInISNumALL(ref strrusult))
                {
                    MessageBox.Show(strrusult, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DSPOutputValueCls tempDSPOutputValue = new DSPOutputValueCls();
                tempDSPOutputValue.ComNo = _server.exceallthread.NowDSPOutputValue.ComNo;
                tempDSPOutputValue.setPhaseWire(cbjx.Text);
                tempDSPOutputValue.setWaveType(cbbx.Text);
                tempDSPOutputValue.Frequency = Convert.ToInt32(cbpl.Text);

                tempDSPOutputValue.Ua = Convert.ToSingle(cbAdy.Text);
                tempDSPOutputValue.Ub = Convert.ToSingle(cbBdy.Text);
                tempDSPOutputValue.Uc = Convert.ToSingle(cbCdy.Text);
                tempDSPOutputValue.Ia = Convert.ToSingle(cbAdl.Text);
                tempDSPOutputValue.Ib = Convert.ToSingle(cbBdl.Text);
                tempDSPOutputValue.Ic = Convert.ToSingle(cbCdl.Text);

                tempDSPOutputValue.SngVoltage = tempDSPOutputValue.Ua;
                if (tempDSPOutputValue.SngVoltage < tempDSPOutputValue.Ub)
                    tempDSPOutputValue.SngVoltage = tempDSPOutputValue.Ub;
                if (tempDSPOutputValue.SngVoltage < tempDSPOutputValue.Uc)
                    tempDSPOutputValue.SngVoltage = tempDSPOutputValue.Uc;


                tempDSPOutputValue.pSngCurrent = tempDSPOutputValue.Ia;
                if (tempDSPOutputValue.pSngCurrent < tempDSPOutputValue.Ib)
                    tempDSPOutputValue.pSngCurrent = tempDSPOutputValue.Ib;
                if (tempDSPOutputValue.pSngCurrent < tempDSPOutputValue.Ic)
                    tempDSPOutputValue.pSngCurrent = tempDSPOutputValue.Ic;

                tempDSPOutputValue.setDUABDUAC(Convert.ToSingle(cbAdyjj.Text), Convert.ToSingle(cbBdyjj.Text), Convert.ToSingle(cbCdyjj.Text));
                tempDSPOutputValue.setDUIaDUIbDUIc(Convert.ToSingle(cbAdyjj.Text), Convert.ToSingle(cbBdyjj.Text), Convert.ToSingle(cbCdyjj.Text), Convert.ToSingle(cbAdljj.Text), Convert.ToSingle(cbBdljj.Text), Convert.ToSingle(cbCdljj.Text));

                if (tempDSPOutputValue.WaveType > 1)
                {
                    strrusult = "";
                    if (!checkSetDSPHarmonic(ref strrusult))
                    {
                        MessageBox.Show(strrusult, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    tempDSPOutputValue.XBabcIS = new byte[] { 1, 1, 1, 1, 1, 1 };

                    tempDSPOutputValue.WaveDegreeA = Convert.ToSingle(cbaxbxwcj.Text);       //谐波初相角1，0-359.99
                    tempDSPOutputValue.WaveDegreeB = Convert.ToSingle(cbbxbxwcj.Text);       //谐波初相角2，0-359.99
                    tempDSPOutputValue.WaveDegreeC = Convert.ToSingle(cbcxbxwcj.Text);       //谐波初相角3，0-359.99
                    tempDSPOutputValue.WaveTimesA = Convert.ToInt32(cbaxbcs.Text);       //谐波次数1，0-21
                    tempDSPOutputValue.WaveTimesB = Convert.ToInt32(cbbxbcs.Text);       //谐波次数2，0-21
                    tempDSPOutputValue.WaveTimesC = Convert.ToInt32(cbcxbcs.Text);        //谐波次数3，0-21
                    tempDSPOutputValue.VoltageWaveRateA = Convert.ToInt32(cbaxbdyhl.Text);  //电压谐波含量1，0-40
                    tempDSPOutputValue.VoltageWaveRateB = Convert.ToInt32(cbbxbdyhl.Text);  //电压谐波含量2，0-40
                    tempDSPOutputValue.VoltageWaveRateC = Convert.ToInt32(cbcxbdyhl.Text); //电压谐波含量3，0-40
                    tempDSPOutputValue.CurrentWaveRateA = Convert.ToInt32(cbaxbdlhl.Text); //电流谐波含量1，0-40
                    tempDSPOutputValue.CurrentWaveRateB = Convert.ToInt32(cbbxbdlhl.Text);  //电流谐波含量1，0-40
                    tempDSPOutputValue.CurrentWaveRateC = Convert.ToInt32(cbcxbdlhl.Text);  //电流谐波含量1，0-40

                    _server.exceallthread.timerwait();
                    r = tempDSPOutputValue.SetDSPHarmonic();
                }

                _server.exceallthread.timerwait();
                r = tempDSPOutputValue.DSPStop();
                r = tempDSPOutputValue.SetOperationHC_Fun();
                r = tempDSPOutputValue.DSPOutput();
                _server.exceallthread.NowDSPOutputValue = tempDSPOutputValue;
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {

            }
            if (r == -1)
            {
                server_EventMessage(ChannelMessageTypeOptions.Information, "升源成功！");
            }
            else
            {
                server_EventMessage(ChannelMessageTypeOptions.Information, "升源失败！");
            }
        }

        /*
         * 生源，三项统一设置输出，调用的dll函数，无需单独处理串口关闭
         */
        private void btnsy_Click(object sender, EventArgs e)
        {
            int r = 0;
            try
            {
                string strrusult = "";
                if (!checkTextInISNumALL(ref strrusult))
                {
                    MessageBox.Show(strrusult, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DSPOutputValueCls tempDSPOutputValue = new DSPOutputValueCls();
                tempDSPOutputValue.ComNo = _server.exceallthread.NowDSPOutputValue.ComNo;
                tempDSPOutputValue.setPhaseWire(cbjx.Text);
                tempDSPOutputValue.setWaveType(cbbx.Text);
                tempDSPOutputValue.Frequency = Convert.ToInt32(cbpl.Text);

                if (cbSAdy.Checked)
                    tempDSPOutputValue.Ua = Convert.ToSingle(cbdy.Text);
                else
                    tempDSPOutputValue.Ua = 0;
                if (cbSBdy.Checked)
                    tempDSPOutputValue.Ub = Convert.ToSingle(cbdy.Text);
                else
                    tempDSPOutputValue.Ub = 0;
                if (cbSCdy.Checked)
                    tempDSPOutputValue.Uc = Convert.ToSingle(cbdy.Text);
                else
                    tempDSPOutputValue.Uc = 0;

                if (cbSAdl.Checked)
                    tempDSPOutputValue.Ia = Convert.ToSingle(cbdl.Text);
                else
                    tempDSPOutputValue.Ia = 0;

                if (cbSBdl.Checked)
                    tempDSPOutputValue.Ib = Convert.ToSingle(cbdl.Text);
                else
                    tempDSPOutputValue.Ib = 0;

                if (cbSCdl.Checked)
                    tempDSPOutputValue.Ic = Convert.ToSingle(cbdl.Text);
                else
                    tempDSPOutputValue.Ic = 0;

                tempDSPOutputValue.SngVoltage = tempDSPOutputValue.Ua;
                if (tempDSPOutputValue.SngVoltage < tempDSPOutputValue.Ub)
                    tempDSPOutputValue.SngVoltage = tempDSPOutputValue.Ub;
                if (tempDSPOutputValue.SngVoltage < tempDSPOutputValue.Uc)
                    tempDSPOutputValue.SngVoltage = tempDSPOutputValue.Uc;

                tempDSPOutputValue.pSngCurrent = tempDSPOutputValue.Ia;
                if (tempDSPOutputValue.pSngCurrent < tempDSPOutputValue.Ib)
                    tempDSPOutputValue.pSngCurrent = tempDSPOutputValue.Ib;
                if (tempDSPOutputValue.pSngCurrent < tempDSPOutputValue.Ib)
                    tempDSPOutputValue.pSngCurrent = tempDSPOutputValue.Ib;

                tempDSPOutputValue.setDUABDUAC(0, 120, 240);
                tempDSPOutputValue.DUIa = Convert.ToSingle(cbdydljj.Text);
                tempDSPOutputValue.DUIb = Convert.ToSingle(cbdydljj.Text);
                tempDSPOutputValue.DUIc = Convert.ToSingle(cbdydljj.Text);

                tempDSPOutputValue.XJIa = (tempDSPOutputValue.XJUa + tempDSPOutputValue.DUIa);
                tempDSPOutputValue.XJIb = (tempDSPOutputValue.XJUb + tempDSPOutputValue.DUIb);
                tempDSPOutputValue.XJIc = (tempDSPOutputValue.XJUc + tempDSPOutputValue.DUIc);

                if (tempDSPOutputValue.WaveType > 1)
                {
                    strrusult = "";
                    if (!checkSetDSPHarmonic(ref strrusult))
                    {
                        MessageBox.Show(strrusult, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    tempDSPOutputValue.WaveDegreeA = Convert.ToSingle(cbaxbxwcj.Text);       //谐波初相角1，0-359.99
                    tempDSPOutputValue.WaveDegreeB = Convert.ToSingle(cbbxbxwcj.Text);       //谐波初相角2，0-359.99
                    tempDSPOutputValue.WaveDegreeC = Convert.ToSingle(cbcxbxwcj.Text);       //谐波初相角3，0-359.99
                    tempDSPOutputValue.WaveTimesA = Convert.ToInt32(cbaxbcs.Text);       //谐波次数1，0-21
                    tempDSPOutputValue.WaveTimesB = Convert.ToInt32(cbbxbcs.Text);       //谐波次数2，0-21
                    tempDSPOutputValue.WaveTimesC = Convert.ToInt32(cbcxbcs.Text);        //谐波次数3，0-21
                    tempDSPOutputValue.VoltageWaveRateA = Convert.ToInt32(cbaxbdyhl.Text);  //电压谐波含量1，0-40
                    tempDSPOutputValue.VoltageWaveRateB = Convert.ToInt32(cbbxbdyhl.Text);  //电压谐波含量2，0-40
                    tempDSPOutputValue.VoltageWaveRateC = Convert.ToInt32(cbcxbdyhl.Text); //电压谐波含量3，0-40
                    tempDSPOutputValue.CurrentWaveRateA = Convert.ToInt32(cbaxbdlhl.Text); //电流谐波含量1，0-40
                    tempDSPOutputValue.CurrentWaveRateB = Convert.ToInt32(cbbxbdlhl.Text);  //电流谐波含量1，0-40
                    tempDSPOutputValue.CurrentWaveRateC = Convert.ToInt32(cbcxbdlhl.Text);  //电流谐波含量1，0-40
                    
                    _server.exceallthread.timerwait();
                    r = tempDSPOutputValue.SetDSPHarmonic();
                }

                _server.exceallthread.timerwait();
                r = tempDSPOutputValue.SetOperationHC_Fun();
                r = tempDSPOutputValue.DSPOutput();
                _server.exceallthread.NowDSPOutputValue = tempDSPOutputValue;
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
            }
            if (r == -1)
            {
                server_EventMessage(ChannelMessageTypeOptions.Information, "升源成功！");
            }
            else
            {
                server_EventMessage(ChannelMessageTypeOptions.Information, "升源失败！");
            }
        }

        /*
         * 降源，关闭电源，调用的dll函数，无需单独处理串口关闭
         */
        private void btnjy_Click(object sender, EventArgs e)
        {
            int r = 0;
            try
            {
                if (_server.exceallthread.NowDSPOutputValue != null)
                {
                    _server.exceallthread.timerwait();
                    r = _server.exceallthread.NowDSPOutputValue.DSPDown();
                    _server.exceallthread.timerrun();
                }
                else
                {
                    _server.exceallthread.NowDSPOutputValue = new DSPOutputValueCls();
                    _server.exceallthread.NowDSPOutputValue.ComNo = _server.DSPCOM;
                    _server.exceallthread.timerwait();
                    r = _server.exceallthread.NowDSPOutputValue.DSPDown();
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
            }
            if (r == -1)
            {
                server_EventMessage(ChannelMessageTypeOptions.Information, "降源成功！");
            }
            else
            {
                server_EventMessage(ChannelMessageTypeOptions.Information, "降源失败！");
            }
        }
        private void btngjjy_Click(object sender, EventArgs e)
        {
            int r = 0;
            try
            {
                if (_server.exceallthread.NowDSPOutputValue != null)
                {
                    _server.exceallthread.timerwait();
                    r = _server.exceallthread.NowDSPOutputValue.DSPDown();
                    _server.exceallthread.timerrun();
                }
                else
                {
                    _server.exceallthread.NowDSPOutputValue = new DSPOutputValueCls();
                    _server.exceallthread.NowDSPOutputValue.ComNo = _server.DSPCOM;
                    _server.exceallthread.timerwait();
                    r = _server.exceallthread.NowDSPOutputValue.DSPDown();
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
            }
            if (r == -1)
            {
                server_EventMessage(ChannelMessageTypeOptions.Information, "降源成功！");
            }
            else
            {
                server_EventMessage(ChannelMessageTypeOptions.Information, "降源失败！");
            }
        }


        private void refreshdsp(DSPOutputValueCls tempDSPOutputValue)
        {
            try
            {
                richTextBox1.Text = tempDSPOutputValue.getOperationHC_Fun();
            }
            catch (Exception ex)
            {

            }
        }

        /*
         * 读取输出数值，调用的dll函数，无需单独处理串口关闭
         */
        private void btndb_Click(object sender, EventArgs e)
        {
            try
            {
                DSPOutputValueCls tempDSPOutputValue = _server.exceallthread.NowDSPOutputValue;
                _server.exceallthread.timerwait();
                tempDSPOutputValue.OperationHC_Fun();
                _server.exceallthread.timerrun();
                refreshdsp(tempDSPOutputValue);
            }
            catch (Exception ex)
            {
            }
        }
        private void btngjdb_Click(object sender, EventArgs e)
        {
            try
            {
                DSPOutputValueCls tempDSPOutputValue = _server.exceallthread.NowDSPOutputValue;
                _server.exceallthread.timerwait();
                tempDSPOutputValue.OperationHC_Fun();
                _server.exceallthread.timerrun();
                refreshdsp(tempDSPOutputValue);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 数据控件处理
        private float tempSaveValue = 0;
        private bool checkTextInISNum(string strnum)
        {
            double r = 0;
            if (!Double.TryParse(strnum, out r))
            {
                return false;
            }
            return true;
        }
        private Boolean checkTextInISNumALL(ref string strresult)
        {
            strresult = "";
            if (!checkTextInISNum(cbpl.Text))
            {
                strresult = strresult + "请填入" + cbpl.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbdy.Text))
            {
                strresult = strresult + "请填入" + cbdy.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbdl.Text))
            {
                strresult = strresult + "请填入" + cbdl.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbdydljj.Text))
            {
                strresult = strresult + "请填入" + cbdydljj.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbAdy.Text))
            {
                strresult = strresult + "请填入" + cbAdy.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbBdy.Text))
            {
                strresult = strresult + "请填入" + cbBdy.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbCdy.Text))
            {
                strresult = strresult + "请填入" + cbCdy.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbAdyjj.Text))
            {
                strresult = strresult + "请填入" + cbAdyjj.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbBdyjj.Text))
            {
                strresult = strresult + "请填入" + cbBdyjj.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbCdyjj.Text))
            {
                strresult = strresult + "请填入" + cbCdyjj.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbAdl.Text))
            {
                strresult = strresult + "请填入" + cbAdl.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbBdl.Text))
            {
                strresult = strresult + "请填入" + cbBdl.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbCdl.Text))
            {
                strresult = strresult + "请填入" + cbCdl.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbAdyjj.Text))
            {
                strresult = strresult + "请填入" + cbAdyjj.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbBdyjj.Text))
            {
                strresult = strresult + "请填入" + cbBdyjj.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbCdyjj.Text))
            {
                strresult = strresult + "请填入" + cbCdyjj.Tag.ToString() + "的值";
                return false;
            }

            return true;
        }
        private bool checkSetDSPHarmonic(ref string strresult)
        {
            strresult = "";
            if (!checkTextInISNum(cbaxbcs.Text))
            {
                strresult = strresult + "请填入" + cbaxbcs.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbaxbxwcj.Text))
            {
                strresult = strresult + "请填入" + cbaxbxwcj.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbaxbdyhl.Text))
            {
                strresult = strresult + "请填入" + cbaxbdyhl.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbaxbdlhl.Text))
            {
                strresult = strresult + "请填入" + cbaxbdlhl.Tag.ToString() + "的值";
                return false;
            }

            if (!checkTextInISNum(cbbxbcs.Text))
            {
                strresult = strresult + "请填入" + cbbxbcs.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbbxbxwcj.Text))
            {
                strresult = strresult + "请填入" + cbbxbxwcj.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbbxbdyhl.Text))
            {
                strresult = strresult + "请填入" + cbbxbdyhl.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbbxbdlhl.Text))
            {
                strresult = strresult + "请填入" + cbbxbdlhl.Tag.ToString() + "的值";
                return false;
            }

            if (!checkTextInISNum(cbcxbcs.Text))
            {
                strresult = strresult + "请填入" + cbcxbcs.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbcxbxwcj.Text))
            {
                strresult = strresult + "请填入" + cbcxbxwcj.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbcxbdyhl.Text))
            {
                strresult = strresult + "请填入" + cbcxbdyhl.Tag.ToString() + "的值";
                return false;
            }
            if (!checkTextInISNum(cbcxbdlhl.Text))
            {
                strresult = strresult + "请填入" + cbcxbdlhl.Tag.ToString() + "的值";
                return false;
            }
            return true;
        }

        private void cbpl_TextUpdate(object sender, EventArgs e)
        {
            if (((ComboBox)sender).Text == "")
            {
                ((ComboBox)sender).Text = tempSaveValue.ToString();
                MessageBox.Show(((ComboBox)sender).Tag.ToString() + "不可为空！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!checkTextInISNum(((ComboBox)sender).Text))
            {
                ((ComboBox)sender).Text = tempSaveValue.ToString();
                MessageBox.Show(((ComboBox)sender).Tag.ToString() + "请输入数字！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                tempSaveValue = Convert.ToSingle(((ComboBox)sender).Text);
            }
        }
        private void cbpl_Click(object sender, EventArgs e)
        {
            tempSaveValue = Convert.ToSingle(((ComboBox)sender).Text);
        }
        #endregion
        #endregion

        #region 表台功能测试
        #region 串口
        private enum PortType
        {
            ALL = 0,
            JD = 1,
            BZ = 2
        }

        private SerialPort LJJDJSport = new SerialPort();
        private string LJJDJPortName = "COM5";
        private int LJJDJPortBaudRate = 9600;
        private int LJJDJPortParity = Convert.ToInt32(Parity.None);
        private int LJJDJPortStopBits = Convert.ToInt32(StopBits.One);
        private int LJJDJPortDataBits = 8;

        private SerialPort LJJBZSport = new SerialPort();
        private string LJJBZPortName = "COM6";
        private int LJJBZPortBaudRate = 9600;
        private int LJJBZPortParity = Convert.ToInt32(Parity.None);
        private int LJJBZPortStopBits = Convert.ToInt32(StopBits.One);
        private int LJJBZPortDataBits = 8;
        #endregion
        #region 检测串口是否连接正常并打开
        private Boolean checkCom(PortType tp)
        {
            Boolean f1 = true;
            Boolean f2 = true;
            #region 打开串口
            if (LJJDJSport != null)
                LJJDJSport.Close();
            if (LJJBZSport != null)
                LJJBZSport.Close();

            if (tp != PortType.BZ)
            {
                try
                {
                    LJJDJSport.Parity = (Parity)LJJDJPortParity;
                    LJJDJSport.PortName = LJJDJPortName;
                    LJJDJSport.BaudRate = LJJDJPortBaudRate;
                    LJJDJSport.StopBits = (StopBits)LJJDJPortStopBits;
                    LJJDJSport.DataBits = LJJDJPortDataBits;

                    LJJDJSport.Open();
                }
                catch (Exception ex)
                {
                    f1 = false;
                    MessageBox.Show("串口" + LJJDJPortName + "出错！" + ex.Message, "串口连接", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (tp != PortType.JD)
            {
                try
                {
                    LJJBZSport.Parity = (Parity)LJJBZPortParity;
                    LJJBZSport.PortName = LJJBZPortName;
                    LJJBZSport.BaudRate = LJJBZPortBaudRate;
                    LJJBZSport.StopBits = (StopBits)LJJBZPortStopBits;
                    LJJBZSport.DataBits = LJJBZPortDataBits;

                    LJJBZSport.Open();
                }
                catch (Exception ex)
                {
                    f2 = false;
                    MessageBox.Show("串口" + LJJBZPortName + "出错！" + ex.Message, "串口连接", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            #endregion
            #region 失败关闭串口
            if (!f1)
            {
                if (LJJDJSport.IsOpen)
                    LJJDJSport.Close();
            }
            if (!f2)
            {
                if (LJJBZSport.IsOpen)
                    LJJBZSport.Close();
            }
            #endregion
            return (f1 && f2);
        }
        #endregion

        private void LoadBTGNCS()
        {
            chkball.Checked = false;

            rbSelD1.Checked = true;

            cbSELDjz.SelectedIndex = 0;
            cbSELDzc.SelectedIndex = 0;
            cbSELDjc.SelectedIndex = 0;
            cbSELcomjz.SelectedIndex = 0;
            cbSELcomzc.SelectedIndex = 0;
            cbSELcomjc.SelectedIndex = 0;
            cbSEL485jz.SelectedIndex = 0;
            cbSEL485zc.SelectedIndex = 0;
            cbSEL485jc.SelectedIndex = 0;

            {
                LJJDJSport.Close();
                LJJBZSport.Close();
            }
        }

        //屏幕显示
        private void pmshow(int index, string str)
        {
            byte[] sendbytes, receivebytes;
            byte[] sendbytes2, receivebytes2;
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkPM, index);
            bool f = true;
            try
            {
                _server.exceallthread.timerwait();
                f = _server.exceallthread.NowPMShowScls.pmshows[index].cmd_clearkSend(lnk, out sendbytes, out receivebytes);
                Thread.Sleep(500);
                f = _server.exceallthread.NowPMShowScls.pmshows[index].cmd_bgkSend(lnk, out sendbytes, out receivebytes);
                Thread.Sleep(500);
                f = _server.exceallthread.NowPMShowScls.pmshows[index].cmd_showSend(lnk, str, out sendbytes, out receivebytes, out sendbytes2, out receivebytes2);
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("frmMain", "pmshow","台体屏幕显示","",ex.ToString());           
            }
        }
        private string get0_9(int num)
        {
            string str = "";
            if (num == 0)
            {
                str = "０";
            }
            else if (num == 1)
            {
                str = "１";
            }
            else if (num == 2)
            {
                str = "２";
            }
            else if (num == 3)
            {
                str = "３";
            }
            else if (num == 4)
            {
                str = "４";
            }
            else if (num == 5)
            {
                str = "５";
            }
            else if (num == 6)
            {
                str = "６";
            }
            else if (num == 7)
            {
                str = "７";
            }
            else if (num == 8)
            {
                str = "８";
            }
            else if (num == 9)
            {
                str = "９";
            }
            return str;
        }

        #region 机电控制
        private object JDKZobj = new object();

        public void djkz1xy(int index)
        {
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkDJ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                _server.exceallthread.timerwait();
                f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_jdkz1Send(lnk, ed, MainForm.BTGNcls.JDKZType.XY);
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "电机控制", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    djkzrefresh(index, 1, MainForm.BTGNcls.JDKZType.XY, MainForm.BTGNcls.JDKZType.Unknow);
                }
            }
        }
        public void djkz1sl(int index)
        {
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkDJ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                _server.exceallthread.timerwait();
                f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_jdkz1Send(lnk, ed, MainForm.BTGNcls.JDKZType.SL);
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "电机控制", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    djkzrefresh(index, 1, MainForm.BTGNcls.JDKZType.SL, MainForm.BTGNcls.JDKZType.Unknow);
                }
            }
        }
        public void djkz2xy(int index)
        {
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkDJ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                _server.exceallthread.timerwait();
                f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_jdkz2Send(lnk, ed, MainForm.BTGNcls.JDKZType.XY);
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "电机控制", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    djkzrefresh(index, 2, MainForm.BTGNcls.JDKZType.Unknow, MainForm.BTGNcls.JDKZType.XY);
                }
            }
        }
        public void djkz2sl(int index)
        {
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkDJ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                _server.exceallthread.timerwait();
                f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_jdkz2Send(lnk, ed, MainForm.BTGNcls.JDKZType.SL);
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "电机控制", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    djkzrefresh(index, 2, MainForm.BTGNcls.JDKZType.Unknow, MainForm.BTGNcls.JDKZType.SL);
                }
            }
        }

        private void djkzrefresh(int index, int num, MainForm.BTGNcls.JDKZType jdkz1state, MainForm.BTGNcls.JDKZType jdkz2state)
        {
            try
            {
                lvJDKZ.Visible = false;
                if (num == 1)
                    lvJDKZ.Items[index].SubItems[1].Text = MainForm.BTGNcls.getJDKZType(jdkz1state);
                else
                    lvJDKZ.Items[index].SubItems[2].Text = MainForm.BTGNcls.getJDKZType(jdkz2state);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                lvJDKZ.Visible = true;
            }
        }

        private void btnDJKZ1xy_Click(object sender, EventArgs e)
        {
            lock (JDKZobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        djkz1xy(i);
                        Thread.Sleep(500);
                        //pmshow(i, "电机１下压");
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnDJKZ1sl_Click(object sender, EventArgs e)
        {
            lock (JDKZobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        djkz1sl(i);
                        Thread.Sleep(500);
                        //pmshow(i, "电机１上拉");
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnDJKZ2xy_Click(object sender, EventArgs e)
        {
            lock (JDKZobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        djkz2xy(i);
                        Thread.Sleep(500);
                        //pmshow(i, "电机２下压");
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnDJKZ2sl_Click(object sender, EventArgs e)
        {
            lock (JDKZobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        djkz2sl(i);
                        Thread.Sleep(500);
                        //pmshow(i, "电机２上拉");
                        Thread.Sleep(500);
                    }
                }
            }
        }
        #endregion

        #region [控件]表台
        private void chkball_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox tempcb = (CheckBox)sender;
            chkbs1.Checked = tempcb.Checked;
            chkbs2.Checked = tempcb.Checked;
            chkbs3.Checked = tempcb.Checked;
            chkbs4.Checked = tempcb.Checked;
            chkbs5.Checked = tempcb.Checked;
            chkbs6.Checked = tempcb.Checked;
            chkbs7.Checked = tempcb.Checked;
            chkbs8.Checked = tempcb.Checked;
        }
        private void chkbs1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox tempcb = (CheckBox)sender;
            _server.exceallthread.NowBTGNCScls.btSelIS[0] = tempcb.Checked;
        }
        private void chkbs2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox tempcb = (CheckBox)sender;
            _server.exceallthread.NowBTGNCScls.btSelIS[1] = tempcb.Checked;
        }
        private void chkbs3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox tempcb = (CheckBox)sender;
            _server.exceallthread.NowBTGNCScls.btSelIS[2] = tempcb.Checked;

        }
        private void chkbs4_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox tempcb = (CheckBox)sender;
            _server.exceallthread.NowBTGNCScls.btSelIS[3] = tempcb.Checked;
        }
        private void chkbs5_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox tempcb = (CheckBox)sender;
            _server.exceallthread.NowBTGNCScls.btSelIS[4] = tempcb.Checked;
        }
        private void chkbs6_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox tempcb = (CheckBox)sender;
            _server.exceallthread.NowBTGNCScls.btSelIS[5] = tempcb.Checked;

        }
        private void chkbs7_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox tempcb = (CheckBox)sender;
            _server.exceallthread.NowBTGNCScls.btSelIS[6] = tempcb.Checked;
        }
        private void chkbs8_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox tempcb = (CheckBox)sender;
            _server.exceallthread.NowBTGNCScls.btSelIS[7] = tempcb.Checked;
        }

        private void rbSelD1_CheckedChanged(object sender, EventArgs e)
        {
            bool f = ((RadioButton)sender).Checked;
            lbSELD1.Enabled = f;
            cbSELDjz.Enabled = f;
            lbSEL4851.Enabled = f;
            cbSEL485jz.Enabled = f;
            btnSELD485jz.Enabled = f;
            cbSELcomjz.Enabled = f;
            
        }
        private void rbSelD2_CheckedChanged(object sender, EventArgs e)
        {
            bool f = ((RadioButton)sender).Checked;
            lbSELD2.Enabled = f;
            cbSELDzc.Enabled = f;
            lbSEL4852.Enabled = f;
            cbSEL485zc.Enabled = f;
            btnSELD485zc.Enabled = f;
            cbSELcomzc.Enabled = f;
            
        }
        private void rbSelD3_CheckedChanged(object sender, EventArgs e)
        {
            bool f = ((RadioButton)sender).Checked;
            lbSELD3.Enabled = f;
            cbSELDjc.Enabled = f;
            lbSEL4853.Enabled = f;
            cbSEL485jc.Enabled = f;
            btnSELD485jc.Enabled = f;
            cbSELcomjc.Enabled = f;

        }
        private void cbSELDjz_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void cbSELDzc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void cbSELDjc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region 通断电
        private object TDDobj = new object();

        private bool tdddlupdown(int index, BTGNcls.TDDDLDYType dltype)
        {
            bool[] okis = new bool[6] { false, false, false, false, false ,false};
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tdddlSend(lnk, ed, dltype);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    okis[0] = true;
                    
                    if (dltype == BTGNcls.TDDDLDYType.TD)
                        resultstr[0] = "通过";
                    else if (dltype == BTGNcls.TDDDLDYType.DD)
                        resultstr[0] = "不通过";
                    tddrefresh(index, okis, resultstr);
                }

                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
            return f;
        }
        private bool tdddyupdown(int index, BTGNcls.TDDDLDYType dytype)
        {
            bool[] okis = new bool[6] { false, false, false, false, false, false };
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tdddySend(lnk, ed, dytype);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    okis[2] = true;
                    if (dytype == BTGNcls.TDDDLDYType.TD)
                        resultstr[2] = "通过";
                    else if (dytype == BTGNcls.TDDDLDYType.DD)
                        resultstr[2] = "不通过";
                    tddrefresh(index, okis, resultstr);
                }

                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
            return f;
        }

        private bool tddbdlupdown(int index, BTGNcls.TDDDLDYType dltype)
        {
            bool[] okis = new bool[6] { false, false, false, false, false, false };
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tddbdlSend(lnk, ed, dltype);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    okis[1] = true;
                    if (dltype == BTGNcls.TDDDLDYType.TD)
                        resultstr[1] = "通过";
                    else if (dltype == BTGNcls.TDDDLDYType.DD)
                        resultstr[1] = "不通过";
                    tddrefresh(index, okis, resultstr);
                }

                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
            return f;
        }

        private void tddallup(int index)
        {
            bool f = false;
            MainForm.BTGNcls.TDDDLDYType dytype = BTGNcls.TDDDLDYType.Unknow;
            bool[] okis = new bool[6] { false, false, false, false, false, false };
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            try
            {
                if (rbTDDdyt.Checked)
                {
                    dytype = BTGNcls.TDDDLDYType.TD;
                    resultstr[1] = "通过";
                }
                else
                {
                    dytype = BTGNcls.TDDDLDYType.DD;
                    resultstr[1] = "不通过";
                }

                f = tdddyupdown(index, dytype);
                if (f)
                {
                    okis[1] = true;
                    //if (checkCom(PortType.BZ))
                    {
                        _server.exceallthread.timerwait();
                        f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tddSend(lnk, ed, MainForm.BTGNcls.TDDType.TD, 0xff);
                        _server.exceallthread.timerrun();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    okis[3] = true;
                    okis[4] = true;
                    okis[5] = true;

                    resultstr[3] = "上电";
                    resultstr[4] = "上电";
                    resultstr[5] = "上电";
                    tddrefresh(index, okis, resultstr);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        private void tddalldwon(int index)
        {
            bool f = false;
            bool[] okis = new bool[6] { false, false, false, false, false, false };
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tddSend(lnk, ed, MainForm.BTGNcls.TDDType.DD, 0xff);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    okis[3] = true;
                    okis[4] = true;
                    okis[5] = true;

                    resultstr[3] = "断电";
                    resultstr[4] = "断电";
                    resultstr[5] = "断电";
                    tddrefresh(index, okis, resultstr);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }

        private void tddjzup(int index)
        {
            bool f = false;
            MainForm.BTGNcls.TDDDLDYType dytype = BTGNcls.TDDDLDYType.Unknow;
            bool[] okis = new bool[6] { false, false, false, false, false, false };
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            try
            {
                if (rbTDDdyt.Checked)
                {
                    dytype = BTGNcls.TDDDLDYType.TD;
                    resultstr[1] = "通过";
                }
                else
                {
                    dytype = BTGNcls.TDDDLDYType.DD;
                    resultstr[1] = "不通过";
                }

                f = tdddyupdown(index, dytype);
                Thread.Sleep(500);
                if (f)
                {
                    okis[1] = true;

                    //if (checkCom(PortType.BZ))
                    {
                        _server.exceallthread.timerwait();
                        f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tddSend(lnk, ed, MainForm.BTGNcls.TDDType.TD, 0x01);
                        _server.exceallthread.timerrun();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    okis[3] = true;
                    //okis[3] = true;
                    //okis[4] = true;

                    resultstr[3] = "上电";
                    //resultstr[3] = "上电";
                    //resultstr[4] = "上电";
                    tddrefresh(index, okis, resultstr);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        private void tddjzdwon(int index)
        {
           
            bool f = false;
            bool[] okis = new bool[6] { false, false, false, false, false, false };
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tddSend(lnk, ed, MainForm.BTGNcls.TDDType.DD, 0x01);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    okis[3] = true;
                    //okis[3] = true;
                    //okis[4] = true;

                    resultstr[3] = "断电";
                    //resultstr[3] = "断电";
                    //resultstr[4] = "断电";
                    tddrefresh(index, okis, resultstr);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }

        private void tddjcup(int index)
        {
           
            bool f = false;
            MainForm.BTGNcls.TDDDLDYType dytype = BTGNcls.TDDDLDYType.Unknow;
            bool[] okis = new bool[6] { false, false, false, false, false, false };
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            try
            {
                if (rbTDDdyt.Checked)
                {
                    dytype = BTGNcls.TDDDLDYType.TD;
                    resultstr[1] = "通过";
                }
                else
                {
                    dytype = BTGNcls.TDDDLDYType.DD;
                    resultstr[1] = "不通过";
                }

                f = tdddyupdown(index, dytype);
                if (f)
                {
                    okis[1] = true;

                    //if (checkCom(PortType.BZ))
                    {
                        _server.exceallthread.timerwait();
                        f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tddSend(lnk, ed, MainForm.BTGNcls.TDDType.TD, 0x02);
                        _server.exceallthread.timerrun();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    //okis[2] = true;
                    okis[4] = true;
                    //okis[4] = true;

                    //resultstr[2] = "上电";
                    resultstr[4] = "上电";
                    //resultstr[4] = "上电";
                    tddrefresh(index, okis, resultstr);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        private void tddjcdwon(int index)
        {
           
            bool f = false;
            bool[] okis = new bool[6] { false, false, false, false, false, false };
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tddSend(lnk, ed, MainForm.BTGNcls.TDDType.DD, 0x02);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message , "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    //okis[2] = true;
                    okis[4] = true;
                    //okis[4] = true;

                    //resultstr[2] = "断电";
                    resultstr[4] = "断电";
                    //resultstr[4] = "断电";
                    tddrefresh(index, okis, resultstr);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }

        private void tddcup(int index)
        {
           
            bool f = false;
            MainForm.BTGNcls.TDDDLDYType dytype = BTGNcls.TDDDLDYType.Unknow;
            bool[] okis = new bool[6] { false, false, false, false, false, false };
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            try
            {
                if (rbTDDdyt.Checked)
                {
                    dytype = BTGNcls.TDDDLDYType.TD;
                    resultstr[1] = "通过";
                }
                else
                {
                    dytype = BTGNcls.TDDDLDYType.DD;
                    resultstr[1] = "不通过";
                }

                f = tdddyupdown(index, dytype);
                if (f)
                {
                    okis[1] = true;

                    //if (checkCom(PortType.BZ))
                    {
                        _server.exceallthread.timerwait();
                        f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tddSend(lnk, ed, MainForm.BTGNcls.TDDType.TD, 0x03);
                        _server.exceallthread.timerrun();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message , "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    //okis[2] = true;
                    //okis[3] = true;
                    okis[5] = true;

                    //resultstr[2] = "上电";
                    //resultstr[3] = "上电";
                    resultstr[5] = "上电";
                    tddrefresh(index, okis, resultstr);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        private void tddcdwon(int index)
        {
           
            bool f = false;
            bool[] okis = new bool[6] { false, false, false, false, false, false };
            string[] resultstr = new string[6] { "", "", "", "", "", "" };
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_tddSend(lnk, ed, MainForm.BTGNcls.TDDType.DD, 0x03);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "设备通断电", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    //okis[2] = true;
                    //okis[3] = true;
                    okis[5] = true;

                    //resultstr[2] = "断电";
                    //resultstr[3] = "断电";
                    resultstr[5] = "断电";
                    tddrefresh(index, okis, resultstr);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }

        private void tddrefresh(int index, bool[] okis, string[] result)
        {
            try
            {
                lvTDD.Visible = false;
                for (int i = 0; i < okis.Length; i++)
                {
                    if (okis[i])
                    {
                        lvTDD.Items[index].SubItems[i + 1].Text = result[i];
                    }
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                lvTDD.Visible = true;
            }
        }

        private void btnTDDdlup_Click(object sender, EventArgs e)
        {
            if (!checkBoxZD.Checked && !checkBoxDB.Checked)
            {
                MessageBox.Show("请选择一种设备!", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MainForm.BTGNcls.TDDDLDYType dltype = BTGNcls.TDDDLDYType.TD;
            lock (TDDobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (checkBoxZD.Checked)
                    {
                        if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                        {
                            tdddlupdown(i, dltype);
                            Thread.Sleep(500);
                            //pmshow(i, "电流上电");
                            Thread.Sleep(500);
                        }
                    }
                    if (checkBoxDB.Checked)
                    {
                        if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                        {
                            tddbdlupdown(i, dltype);
                            Thread.Sleep(500);
                            //pmshow(i, "电流上电");
                            Thread.Sleep(500);
                        }
                    }
                }
            }
        }
        private void btnTDDdldown_Click(object sender, EventArgs e)
        {
            if (!checkBoxZD.Checked && !checkBoxDB.Checked)
            {
                MessageBox.Show("请选择一种设备!", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MainForm.BTGNcls.TDDDLDYType dltype = BTGNcls.TDDDLDYType.DD;
            lock (TDDobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (checkBoxZD.Checked)
                    {
                        if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                        {
                            tdddlupdown(i, dltype);
                            Thread.Sleep(500);
                            //pmshow(i, "电流断电");
                            Thread.Sleep(500);
                        }
                    }
                    if (checkBoxDB.Checked)
                    {
                        if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                        {
                            tddbdlupdown(i, dltype);
                            Thread.Sleep(500);
                            //pmshow(i, "电流上电");
                            Thread.Sleep(500);
                        }
                    }
                }
            }
        }

        private void btnTDDALLup_Click(object sender, EventArgs e)
        {
            lock (TDDobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        tddallup(i);
                        Thread.Sleep(500);
                        //pmshow(i, "所有电压上电");
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnTDDALLdown_Click(object sender, EventArgs e)
        {
            lock (TDDobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        tddalldwon(i);
                        Thread.Sleep(500);
                        //pmshow(i, "所有电压断电");
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void btnTDDJZup_Click(object sender, EventArgs e)
        {
            lock (TDDobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        tddjzup(i);
                        Thread.Sleep(500);
                        //pmshow(i, "集１专３上电");
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnTDDJZdown_Click(object sender, EventArgs e)
        {
            lock (TDDobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        tddjzdwon(i);
                        Thread.Sleep(500);
                        //pmshow(i, "集１专３断电");
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void btnTDDC1up_Click(object sender, EventArgs e)
        {
            lock (TDDobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        tddjcup(i);
                        Thread.Sleep(500);
                        //pmshow(i, "集２采１上电");
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnTDDC1down_Click(object sender, EventArgs e)
        {
            lock (TDDobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        tddjcdwon(i);
                        Thread.Sleep(500);
                        //pmshow(i, "集２采１断电");
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void btnTDDC2up_Click(object sender, EventArgs e)
        {
            lock (TDDobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        tddcup(i);
                        Thread.Sleep(500);
                        //pmshow(i, "专３采２上电");
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnTDDC2down_Click(object sender, EventArgs e)
        {
            lock (TDDobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        tddcdwon(i);
                        Thread.Sleep(500);
                        //pmshow(i, "专３采２断电");
                        Thread.Sleep(500);
                    }
                }
            }
        }
        #endregion

        #region 摇信
        private object YXobj = new object();

        private void yxjzset(int index)
        {
           
            MainForm.BTGNcls.YXType[] yxtypes = new MainForm.BTGNcls.YXType[_server.exceallthread.NowBTGNCScls.btgns[index].yxzjstarts.Length];
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                if (cbYXjzRoad1.Checked)
                {
                    yxtypes[0] = MainForm.BTGNcls.YXType.XH;
                }
                else
                {
                    yxtypes[0] = MainForm.BTGNcls.YXType.DK;
                }
                if (cbYXjzRoad2.Checked)
                {
                    yxtypes[1] = MainForm.BTGNcls.YXType.XH;
                }
                else
                {
                    yxtypes[1] = MainForm.BTGNcls.YXType.DK;
                }
                if (rbYXjzSelj.Checked)
                {
                    if (cbYXjzRoad3.Checked)
                    {
                        yxtypes[2] = MainForm.BTGNcls.YXType.XH;
                    }
                    else
                    {
                        yxtypes[2] = MainForm.BTGNcls.YXType.DK;
                    }
                    if (cbYXjzRoad4.Checked)
                    {
                        yxtypes[3] = MainForm.BTGNcls.YXType.XH;
                    }
                    else
                    {
                        yxtypes[3] = MainForm.BTGNcls.YXType.DK;
                    }
                }
                if (rbYXjzmOP.Checked)
                {
                    yxtypes[4] = MainForm.BTGNcls.YXType.XH;
                }
                else
                {
                    yxtypes[4] = MainForm.BTGNcls.YXType.DK;
                }

                _server.exceallthread.timerwait();
                f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_yxjzSetSend(lnk, ed, yxtypes);
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message , "摇信控制", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    if (rbYXjzSelj.Checked)
                    {
                        yxjzrefresh(index, MainForm.BTGNcls.YXDeviceType.J, yxtypes, MainForm.BTGNcls.YXType.Unknow);
                    }
                    else
                    {
                        yxjzrefresh(index, MainForm.BTGNcls.YXDeviceType.Z, yxtypes, MainForm.BTGNcls.YXType.Unknow);
                    }
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        private void yxcset(int index)
        {
           
            MainForm.BTGNcls.YXType yxtype = MainForm.BTGNcls.YXType.Unknow;
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                if (cbYXcRoad1.Checked)
                {
                    yxtype = MainForm.BTGNcls.YXType.XH;
                }
                else
                {
                    yxtype = MainForm.BTGNcls.YXType.DK;
                }

                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_yxcSetSend(lnk, ed, yxtype);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "摇信控制", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    yxjzrefresh(index, MainForm.BTGNcls.YXDeviceType.C, null, yxtype);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        /*
         * 刷行显示结果控件
         */
        private void yxjzrefresh(int index, MainForm.BTGNcls.YXDeviceType txdevicetype, MainForm.BTGNcls.YXType[] yxjztpyes, MainForm.BTGNcls.YXType yxctpye)
        {
            try
            {
                lvYX.Visible = false;
                if (txdevicetype == MainForm.BTGNcls.YXDeviceType.C)
                {
                    if (yxctpye == BTGNcls.YXType.XH)
                        lvYX.Items[2 * index + 1].SubItems[3].Text = "1合;";
                    else if (yxctpye == BTGNcls.YXType.DK)
                        lvYX.Items[2 * index + 1].SubItems[3].Text = "1断;";
                    else
                        lvYX.Items[2 * index + 1].SubItems[3].Text = "";
                }
                else if (txdevicetype == MainForm.BTGNcls.YXDeviceType.J)
                {
                    lvYX.Items[2 * index].SubItems[2].Text = "集中器";
                    if (yxjztpyes[0] == BTGNcls.YXType.XH)
                        lvYX.Items[2 * index].SubItems[3].Text = "1合;";
                    else if (yxjztpyes[0] == BTGNcls.YXType.DK)
                        lvYX.Items[2 * index].SubItems[3].Text = "1断;";
                    else 
                        lvYX.Items[2 * index].SubItems[3].Text = "";

                    if (yxjztpyes[1] == BTGNcls.YXType.XH)
                        lvYX.Items[2 * index].SubItems[3].Text = lvYX.Items[2 * index].SubItems[3].Text + "2合;";
                    else if (yxjztpyes[1] == BTGNcls.YXType.DK)
                        lvYX.Items[2 * index].SubItems[3].Text = lvYX.Items[2 * index].SubItems[3].Text + "2断;";
                    else
                        lvYX.Items[2 * index].SubItems[3].Text = "";

                    if (yxjztpyes[2] == BTGNcls.YXType.XH)
                        lvYX.Items[2 * index].SubItems[3].Text = lvYX.Items[2 * index].SubItems[3].Text + "3合;";
                    else if (yxjztpyes[2] == BTGNcls.YXType.DK)
                        lvYX.Items[2 * index].SubItems[3].Text = lvYX.Items[2 * index].SubItems[3].Text + "3断;";
                    else
                        lvYX.Items[2 * index].SubItems[3].Text = "";

                    if (yxjztpyes[3] == BTGNcls.YXType.XH)
                        lvYX.Items[2 * index].SubItems[3].Text = lvYX.Items[2 * index].SubItems[3].Text + "4合;";
                    else if (yxjztpyes[3] == BTGNcls.YXType.DK)
                        lvYX.Items[2 * index].SubItems[3].Text = lvYX.Items[2 * index].SubItems[3].Text + "4断;";
                    else
                        lvYX.Items[2 * index].SubItems[3].Text = lvYX.Items[2 * index].SubItems[3].Text + "";

                    if (yxjztpyes[4] == BTGNcls.YXType.XH)
                        lvYX.Items[2 * index].SubItems[4].Text = "合";
                    else if (yxjztpyes[4] == BTGNcls.YXType.DK)
                        lvYX.Items[2 * index].SubItems[4].Text = "断";
                    else
                        lvYX.Items[2 * index].SubItems[4].Text = "";
                }
                else if (txdevicetype == MainForm.BTGNcls.YXDeviceType.Z)
                {
                    lvYX.Items[2 * index].SubItems[2].Text = "专变";

                    if (yxjztpyes[0] == BTGNcls.YXType.XH)
                        lvYX.Items[2 * index].SubItems[3].Text = "1合;";
                    else if (yxjztpyes[0] == BTGNcls.YXType.DK)
                        lvYX.Items[2 * index].SubItems[3].Text = "1断;";
                    else
                        lvYX.Items[2 * index].SubItems[3].Text = "";

                    if (yxjztpyes[1] == BTGNcls.YXType.XH)
                        lvYX.Items[2 * index].SubItems[3].Text = lvYX.Items[2 * index].SubItems[3].Text + "2合;";
                    else if (yxjztpyes[1] == BTGNcls.YXType.DK)
                        lvYX.Items[2 * index].SubItems[3].Text = lvYX.Items[2 * index].SubItems[3].Text + "2断;";
                    else
                        lvYX.Items[2 * index].SubItems[3].Text = lvYX.Items[2 * index].SubItems[3].Text + "";

                    if (yxjztpyes[4] == BTGNcls.YXType.XH)
                        lvYX.Items[2 * index].SubItems[4].Text = "合";
                    else if (yxjztpyes[4] == BTGNcls.YXType.DK)
                        lvYX.Items[2 * index].SubItems[4].Text = "断";
                    else
                        lvYX.Items[2 * index].SubItems[4].Text = "";
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                lvYX.Visible = true;
            }
        }

        private void yxjzget(int index)
        {
            
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_yxjzGetSend(lnk, ed);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "摇信控制", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    if (rbYXjzSelj.Checked)
                    {
                        yxjzrefresh(index, MainForm.BTGNcls.YXDeviceType.J, _server.exceallthread.NowBTGNCScls.btgns[index].yxzjstarts, MainForm.BTGNcls.YXType.Unknow);
                    }
                    else
                    {
                        yxjzrefresh(index, MainForm.BTGNcls.YXDeviceType.Z, _server.exceallthread.NowBTGNCScls.btgns[index].yxzjstarts, MainForm.BTGNcls.YXType.Unknow);
                    }
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        private void yxcget(int index)
        {
            
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_yxcGetSend(lnk, ed);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "摇信控制", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    yxjzrefresh(index, MainForm.BTGNcls.YXDeviceType.C, null, _server.exceallthread.NowBTGNCScls.btgns[index].yxcstart);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }

        private void btnYXjzSet_Click(object sender, EventArgs e)
        {
            lock (YXobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        yxjzset(i);
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnYXjzGet_Click(object sender, EventArgs e)
        {
            lock (YXobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        yxjzget(i);
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void btnYXcSet_Click(object sender, EventArgs e)
        {
            lock (YXobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        yxcset(i);
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnYXcGet_Click(object sender, EventArgs e)
        {
            lock (YXobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        yxcget(i);
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void rbYXjzSelj_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbtemp = (RadioButton)sender;
            if (rbtemp.Checked)
            {
                cbYXjzRoad3.Enabled = true;
                cbYXjzRoad4.Enabled = true;
            }
        }
        private void rbYXjzSelz_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbtemp = (RadioButton)sender;
            if (rbtemp.Checked)
            {
                cbYXjzRoad3.Checked = false;
                cbYXjzRoad4.Checked = false;
                cbYXjzRoad3.Enabled = false;
                cbYXjzRoad4.Enabled = false;
            }
        }
        #endregion

        #region 脉冲
        private object MCobj = new object();

        private float tempMCSaveValue = 0;
        private void MCTEXT_Change(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).Text = tempMCSaveValue.ToString();
                MessageBox.Show(((TextBox)sender).Tag.ToString() + "不可为空！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!checkTextInISNum(((TextBox)sender).Text))
            {
                ((TextBox)sender).Text = tempMCSaveValue.ToString();
                MessageBox.Show(((TextBox)sender).Tag.ToString() + "请输入数字！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                tempMCSaveValue = Convert.ToSingle(((TextBox)sender).Text);
            }
        }

        private bool mcdxset(int index, BTGNcls.MCDevicType mctpye, byte mcdata)
        {
           
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_MCdxSetSend(lnk, ed, mctpye, mcdata);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "表位设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    MCrefresh(index, MainForm.BTGNcls.MCCMDType.DX, mctpye, mcdata, 0, 0, "");
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
            return f;
        }
        private bool mcdsjet(int index, BTGNcls.MCDevicType mctpye, long mcdata)
        {
           
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_MCsjSetSend(lnk, ed, mctpye, mcdata);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "表位设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    MCrefresh(index, MainForm.BTGNcls.MCCMDType.SJ, mctpye, 0, mcdata, 0, "");
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
            return f;
        }
        private bool mcdgset(int index, BTGNcls.MCDevicType mctpye, long mcdata)
        {

            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_MCgsSetSend(lnk, ed, mctpye, mcdata);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "表位设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    MCrefresh(index, MainForm.BTGNcls.MCCMDType.GS, mctpye, 0, 0, mcdata, "");
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
            return f;
        }

        private void mcstart(int index, BTGNcls.MCDevicType mctpye)
        {
           
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_MCstartSetSend(lnk, ed, mctpye);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "表位设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    MCrefresh(index, MainForm.BTGNcls.MCCMDType.SE, mctpye, 0, 0, 0, "Start");
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        private void mcend(int index, BTGNcls.MCDevicType mctpye)
        {
           
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_MCendSetSend(lnk,ed, mctpye);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "表位设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    MCrefresh(index, MainForm.BTGNcls.MCCMDType.SE, mctpye, 0, 0, 0, "关闭");
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }

        private void MCrefresh(int index, MainForm.BTGNcls.MCCMDType MCCMDType, MainForm.BTGNcls.MCDevicType MCDevicType, byte mcdatadx, long mcdatasj, long mcdatags, string strstate)
        {
            try
            {
                lvMC.Visible = false;
                if (MCDevicType == MainForm.BTGNcls.MCDevicType.MC1)
                {
                    if (MCCMDType == MainForm.BTGNcls.MCCMDType.DX)
                    {
                        lvMC.Items[index * 2].SubItems[2].Text = mcdatadx.ToString();
                    }
                    if (MCCMDType == MainForm.BTGNcls.MCCMDType.SJ)
                    {
                        lvMC.Items[index * 2].SubItems[3].Text = mcdatasj.ToString();
                    }
                    if (MCCMDType == MainForm.BTGNcls.MCCMDType.GS)
                    {
                        lvMC.Items[index * 2].SubItems[4].Text = mcdatags.ToString();
                    }
                    if (MCCMDType == MainForm.BTGNcls.MCCMDType.SE)
                    {
                        if (strstate == "Start")
                            lvMC.Items[index * 2].SubItems[5].Text = "启动";
                        else
                            lvMC.Items[index * 2].SubItems[5].Text = "";
                    }
                }
                else
                {
                    if (MCCMDType == MainForm.BTGNcls.MCCMDType.DX)
                    {
                        lvMC.Items[index * 2 + 1].SubItems[2].Text = mcdatadx.ToString();
                    }
                    if (MCCMDType == MainForm.BTGNcls.MCCMDType.SJ)
                    {
                        lvMC.Items[index * 2 + 1].SubItems[3].Text = mcdatasj.ToString();
                    }
                    if (MCCMDType == MainForm.BTGNcls.MCCMDType.GS)
                    {
                        lvMC.Items[index * 2 + 1].SubItems[4].Text = mcdatags.ToString();
                    }
                    if (strstate == "Start")
                        lvMC.Items[index * 2 + 1].SubItems[5].Text = "启动";
                    else
                        lvMC.Items[index * 2 + 1].SubItems[5].Text = "";
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                lvMC.Visible = true;
            }
        }

        private void btnMCset1_Click(object sender, EventArgs e)
        {
            lock (MCobj)
            {
                byte mcdata = Convert.ToByte(txtMCdx1.Text);
                long mcdata1 = Convert.ToInt64(txtMCsj1.Text);
                long mcdata2 = Convert.ToInt64(txtMCgs1.Text);
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        mcdxset(i, MainForm.BTGNcls.MCDevicType.MC1, mcdata);
                        mcdsjet(i, MainForm.BTGNcls.MCDevicType.MC1, mcdata1);
                        mcdgset(i, MainForm.BTGNcls.MCDevicType.MC1, mcdata2);
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnMCset2_Click(object sender, EventArgs e)
        {
            lock (MCobj)
            {
                byte mcdata = Convert.ToByte(txtMCdx2.Text);
                long mcdata1 = Convert.ToByte(txtMCsj2.Text);
                long mcdata2 = Convert.ToInt32(txtMCgs2.Text);
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        mcdxset(i, MainForm.BTGNcls.MCDevicType.MC2, mcdata);
                        mcdsjet(i, MainForm.BTGNcls.MCDevicType.MC2, mcdata1);
                        mcdgset(i, MainForm.BTGNcls.MCDevicType.MC2, mcdata2);
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void btnMCstart1_Click(object sender, EventArgs e)
        {
            lock (MCobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        mcstart(i, MainForm.BTGNcls.MCDevicType.MC1);
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnMCend1_Click(object sender, EventArgs e)
        {
            lock (MCobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        mcend(i, MainForm.BTGNcls.MCDevicType.MC1);
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void btnMCstart2_Click(object sender, EventArgs e)
        {
            lock (MCobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        mcstart(i, MainForm.BTGNcls.MCDevicType.MC2);
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private void btnMCend2_Click(object sender, EventArgs e)
        {
            lock (MCobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        mcend(i, MainForm.BTGNcls.MCDevicType.MC2);
                        Thread.Sleep(500);
                    }
                }
            }
        }
        #endregion

        #region 表位设备和接口选择
        private object SelD485obj = new object();

        private void dseljzset(int index, BTGNcls.DSeljzType dseljztype)
        {
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                _server.exceallthread.timerwait();
                f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_DSeljzSetSend(lnk, ed, dseljztype);
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "表位设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    selDrefresh(index, dseljztype, BTGNcls.DSelzcType.UnKnow, BTGNcls.DSeljcType.UnKnow);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        private void dselzcset(int index, BTGNcls.DSelzcType dselzctype)
        {
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                _server.exceallthread.timerwait();
                f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_DSelzcSetSend(lnk, ed, dselzctype);
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "表位设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    selDrefresh(index, BTGNcls.DSeljzType.UnKnow, dselzctype, BTGNcls.DSeljcType.UnKnow);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        private void dseljcset(int index, BTGNcls.DSeljcType dseljctype)
        {
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                _server.exceallthread.timerwait();
                f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_DSeljcSetSend(lnk, ed, dseljctype);
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "表位设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    selDrefresh(index, BTGNcls.DSeljzType.UnKnow, BTGNcls.DSelzcType.UnKnow, dseljctype);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }

        private void sel485jcset(int index, BTGNcls.DSeljcType dseljctype, BTGNcls.Sel485jcType sel485jctype)
        {
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_Sel485jcSetSend(lnk, ed, sel485jctype);
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "表位设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    selDrefresh(index, BTGNcls.DSeljzType.UnKnow, BTGNcls.DSelzcType.UnKnow, dseljctype);
                    sel485refresh(index, BTGNcls.Sel485jzType.UnKnow, BTGNcls.DSelzcType.UnKnow, sel485jctype);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }
        private void sel485jzset(int index, BTGNcls.Sel485jzType sel485jztype)
        {
           
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                _server.exceallthread.timerwait();
                f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_Sel485jzSetSend(lnk, ed, sel485jztype);
                _server.exceallthread.timerrun();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "表位设备和接口选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    sel485refresh(index, sel485jztype, BTGNcls.DSelzcType.UnKnow, BTGNcls.Sel485jcType.UnKnow);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }

        private void selDrefresh(int index, BTGNcls.DSeljzType dseljztype, BTGNcls.DSelzcType dselzctype, BTGNcls.DSeljcType dseljctype)
        {
            try
            {
                lvSelD485.Visible = false;
                lvSelD485.Items[index].SubItems[1].Text = "";
                lvSelD485.Items[index].SubItems[3].Text = "";
                lvSelD485.Items[index].SubItems[5].Text = "";

                if (dseljztype != BTGNcls.DSeljzType.UnKnow)
                {
                    lvSelD485.Items[index].SubItems[1].Text = BTGNcls.getDSeljzType(dseljztype);
                }

                if (dselzctype != BTGNcls.DSelzcType.UnKnow)
                {
                    lvSelD485.Items[index].SubItems[3].Text = BTGNcls.getDSelzcType(dselzctype);
                }
  
                if (dseljctype != BTGNcls.DSeljcType.UnKnow)
                {
                    lvSelD485.Items[index].SubItems[5].Text = BTGNcls.getDSeljcType(dseljctype);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                lvSelD485.Visible = true;
            }
        }
        private void sel485refresh(int index, BTGNcls.Sel485jzType sel485jztype, BTGNcls.DSelzcType dselzctype, BTGNcls.Sel485jcType sel485jctype)
        {
            try
            {
                lvSelD485.Visible = false;

                if (sel485jztype != BTGNcls.Sel485jzType.UnKnow)
                {
                    lvSelD485.Items[index].SubItems[2].Text = string.Format("COM{1},{0}",BTGNcls.getSel485jzType(sel485jztype),_server.exceallthread.NowBTGNCScls.btgns[index].Num232);
                }
                if (dselzctype != BTGNcls.DSelzcType.UnKnow)
                {
                    lvSelD485.Items[index].SubItems[4].Text = string.Format("COM{1},{0}", "485Ⅰ", _server.exceallthread.NowBTGNCScls.btgns[index].Num232);
                }
                if (sel485jctype != BTGNcls.Sel485jcType.UnKnow)
                {
                    lvSelD485.Items[index].SubItems[6].Text = string.Format("COM{1},{0}", BTGNcls.getSel485jcType(sel485jctype), _server.exceallthread.NowBTGNCScls.btgns[index].Num232); 
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                lvSelD485.Visible = true;
            }
        }

        #region
        private void btnDselj1_Click(object sender, EventArgs e)
        {
            lock (SelD485obj)
            {
                if (_server.exceallthread.NowBTGNCScls.btSelIS[0])
                {
                    dseljzset(0, MainForm.BTGNcls.DSeljzType.J1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[1])
                {
                    dseljzset(1, MainForm.BTGNcls.DSeljzType.J1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[2])
                {
                    dseljzset(2, MainForm.BTGNcls.DSeljzType.J1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[3])
                {
                    dseljzset(3, MainForm.BTGNcls.DSeljzType.J1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[4])
                {
                    dseljzset(4, MainForm.BTGNcls.DSeljzType.J1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[5])
                {
                    dseljzset(5, MainForm.BTGNcls.DSeljzType.J1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[6])
                {
                    dseljzset(6, MainForm.BTGNcls.DSeljzType.J1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[7])
                {
                    dseljzset(7, MainForm.BTGNcls.DSeljzType.J1);
                }
            }
        }
        private void btnDselz3_Click(object sender, EventArgs e)
        {
            lock (SelD485obj)
            {
                if (_server.exceallthread.NowBTGNCScls.btSelIS[0])
                {
                    dseljzset(0, MainForm.BTGNcls.DSeljzType.Z3);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[1])
                {
                    dseljzset(1, MainForm.BTGNcls.DSeljzType.Z3);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[2])
                {
                    dseljzset(2, MainForm.BTGNcls.DSeljzType.Z3);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[3])
                {
                    dseljzset(3, MainForm.BTGNcls.DSeljzType.Z3);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[4])
                {
                    dseljzset(4, MainForm.BTGNcls.DSeljzType.Z3);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[5])
                {
                    dseljzset(5, MainForm.BTGNcls.DSeljzType.Z3);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[6])
                {
                    dseljzset(6, MainForm.BTGNcls.DSeljzType.Z3);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[7])
                {
                    dseljzset(7, MainForm.BTGNcls.DSeljzType.Z3);
                }
            }
        }
        private void btnDselz1_Click(object sender, EventArgs e)
        {
            lock (SelD485obj)
            {
                if (_server.exceallthread.NowBTGNCScls.btSelIS[0])
                {
                    dselzcset(0, MainForm.BTGNcls.DSelzcType.Z1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[1])
                {
                    dselzcset(1, MainForm.BTGNcls.DSelzcType.Z1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[2])
                {
                    dselzcset(2, MainForm.BTGNcls.DSelzcType.Z1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[3])
                {
                    dselzcset(3, MainForm.BTGNcls.DSelzcType.Z1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[4])
                {
                    dselzcset(4, MainForm.BTGNcls.DSelzcType.Z1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[5])
                {
                    dselzcset(5, MainForm.BTGNcls.DSelzcType.Z1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[6])
                {
                    dselzcset(6, MainForm.BTGNcls.DSelzcType.Z1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[7])
                {
                    dselzcset(7, MainForm.BTGNcls.DSelzcType.Z1);
                }
            }
        }
        private void btnDselc2_Click(object sender, EventArgs e)
        {
            lock (SelD485obj)
            {
                if (_server.exceallthread.NowBTGNCScls.btSelIS[0])
                {
                    dselzcset(0, MainForm.BTGNcls.DSelzcType.C2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[1])
                {
                    dselzcset(1, MainForm.BTGNcls.DSelzcType.C2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[2])
                {
                    dselzcset(2, MainForm.BTGNcls.DSelzcType.C2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[3])
                {
                    dselzcset(3, MainForm.BTGNcls.DSelzcType.C2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[4])
                {
                    dselzcset(4, MainForm.BTGNcls.DSelzcType.C2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[5])
                {
                    dselzcset(5, MainForm.BTGNcls.DSelzcType.C2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[6])
                {
                    dselzcset(6, MainForm.BTGNcls.DSelzcType.C2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[7])
                {
                    dselzcset(7, MainForm.BTGNcls.DSelzcType.C2);
                }
            }
        }
        private void btnsel485jc1_Click(object sender, EventArgs e)
        {
            //bool f = false;
            //lock (SelD485obj)
            //{
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[0])
            //    {
            //        sel485jcset(0, MainForm.BTGNcls.Sel485jcType.JC485_1);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[1])
            //    {
            //        sel485jcset(1, MainForm.BTGNcls.Sel485jcType.JC485_1);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[2])
            //    {
            //        sel485jcset(2, MainForm.BTGNcls.Sel485jcType.JC485_1);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[3])
            //    {
            //        sel485jcset(3, MainForm.BTGNcls.Sel485jcType.JC485_1);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[4])
            //    {
            //        sel485jcset(4, MainForm.BTGNcls.Sel485jcType.JC485_1);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[5])
            //    {
            //        sel485jcset(5, MainForm.BTGNcls.Sel485jcType.JC485_1);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[6])
            //    {
            //        sel485jcset(6, MainForm.BTGNcls.Sel485jcType.JC485_1);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[7])
            //    {
            //        sel485jcset(7, MainForm.BTGNcls.Sel485jcType.JC485_1);
            //    }
            //}
        }
        private void btnsel485jc2_Click(object sender, EventArgs e)
        {
            //bool f = false;
            //lock (SelD485obj)
            //{
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[0])
            //    {
            //        sel485jcset(0, MainForm.BTGNcls.Sel485jcType.JC485_2);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[1])
            //    {
            //        sel485jcset(1, MainForm.BTGNcls.Sel485jcType.JC485_2);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[2])
            //    {
            //        sel485jcset(2, MainForm.BTGNcls.Sel485jcType.JC485_2);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[3])
            //    {
            //        sel485jcset(3, MainForm.BTGNcls.Sel485jcType.JC485_2);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[4])
            //    {
            //        sel485jcset(4, MainForm.BTGNcls.Sel485jcType.JC485_2);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[5])
            //    {
            //        sel485jcset(5, MainForm.BTGNcls.Sel485jcType.JC485_2);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[6])
            //    {
            //        sel485jcset(6, MainForm.BTGNcls.Sel485jcType.JC485_2);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[7])
            //    {
            //        sel485jcset(7, MainForm.BTGNcls.Sel485jcType.JC485_2);
            //    }
            //}
        }
        private void btnsel485jc3_Click(object sender, EventArgs e)
        {
            //bool f = false;
            //lock (SelD485obj)
            //{
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[0])
            //    {
            //        sel485jcset(0, MainForm.BTGNcls.Sel485jcType.JC485_3);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[1])
            //    {
            //        sel485jcset(1, MainForm.BTGNcls.Sel485jcType.JC485_3);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[2])
            //    {
            //        sel485jcset(2, MainForm.BTGNcls.Sel485jcType.JC485_3);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[3])
            //    {
            //        sel485jcset(3, MainForm.BTGNcls.Sel485jcType.JC485_3);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[4])
            //    {
            //        sel485jcset(4, MainForm.BTGNcls.Sel485jcType.JC485_3);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[5])
            //    {
            //        sel485jcset(5, MainForm.BTGNcls.Sel485jcType.JC485_3);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[6])
            //    {
            //        sel485jcset(6, MainForm.BTGNcls.Sel485jcType.JC485_3);
            //    }
            //    if (_server.exceallthread.NowBTGNCScls.btSelIS[7])
            //    {
            //        sel485jcset(7, MainForm.BTGNcls.Sel485jcType.JC485_3);
            //    }
            //}
        }
        private void btnsel485jz1_Click(object sender, EventArgs e)
        {
            lock (SelD485obj)
            {
                if (_server.exceallthread.NowBTGNCScls.btSelIS[0])
                {
                    sel485jzset(0, MainForm.BTGNcls.Sel485jzType.JZ485_1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[1])
                {
                    sel485jzset(1, MainForm.BTGNcls.Sel485jzType.JZ485_1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[2])
                {
                    sel485jzset(2, MainForm.BTGNcls.Sel485jzType.JZ485_1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[3])
                {
                    sel485jzset(3, MainForm.BTGNcls.Sel485jzType.JZ485_1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[4])
                {
                    sel485jzset(4, MainForm.BTGNcls.Sel485jzType.JZ485_1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[5])
                {
                    sel485jzset(5, MainForm.BTGNcls.Sel485jzType.JZ485_1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[6])
                {
                    sel485jzset(6, MainForm.BTGNcls.Sel485jzType.JZ485_1);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[7])
                {
                    sel485jzset(7, MainForm.BTGNcls.Sel485jzType.JZ485_1);
                }
            }
        }
        private void btnsel485jz2_Click(object sender, EventArgs e)
        {
            lock (SelD485obj)
            {
                if (_server.exceallthread.NowBTGNCScls.btSelIS[0])
                {
                    sel485jzset(0, MainForm.BTGNcls.Sel485jzType.JZ485_2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[1])
                {
                    sel485jzset(1, MainForm.BTGNcls.Sel485jzType.JZ485_2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[2])
                {
                    sel485jzset(2, MainForm.BTGNcls.Sel485jzType.JZ485_2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[3])
                {
                    sel485jzset(3, MainForm.BTGNcls.Sel485jzType.JZ485_2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[4])
                {
                    sel485jzset(4, MainForm.BTGNcls.Sel485jzType.JZ485_2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[5])
                {
                    sel485jzset(5, MainForm.BTGNcls.Sel485jzType.JZ485_2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[6])
                {
                    sel485jzset(6, MainForm.BTGNcls.Sel485jzType.JZ485_2);
                }
                if (_server.exceallthread.NowBTGNCScls.btSelIS[7])
                {
                    sel485jzset(7, MainForm.BTGNcls.Sel485jzType.JZ485_2);
                }
            }
        }
        #endregion

        //public void 

        private void btnSELD485jz_Click(object sender, EventArgs e)
        {
            BTGNcls.DSeljzType dseljztype = BTGNcls.DSeljzType.UnKnow;
            BTGNcls.Sel485jzType sel485jztype = BTGNcls.Sel485jzType.UnKnow;
            lock (SelD485obj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    string selectstr = cbSELDjz.Text;
                    string selectstr485 = cbSEL485jz.Text;
                    string selectstrcom = cbSELcomjz.Text;
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        if (selectstr == "集中器1型")
                        {
                            dseljztype = BTGNcls.DSeljzType.J1;
                        }
                        else
                        {
                            dseljztype = BTGNcls.DSeljzType.Z3;
                        }
                        if (selectstr485 == "485Ⅰ")
                        {
                            sel485jztype = BTGNcls.Sel485jzType.JZ485_1;
                        }
                        else//485Ⅱ
                        {
                            sel485jztype = BTGNcls.Sel485jzType.JZ485_2;
                        }
                        if (selectstrcom == "COM1")
                        {
                            _server.exceallthread.NowBTGNCScls.btgns[i].Num232 = 1;
                        }
                        else
                        {
                            _server.exceallthread.NowBTGNCScls.btgns[i].Num232 = 2;
                        }
                        dseljzset(i, dseljztype);
                        sel485jzset(i, sel485jztype);
                    }
                }
                m_Application.SetSystemConfig(this._server.exceallthread.NowBTGNCScls);
            }
        }
        private void btnSELD485zc_Click(object sender, EventArgs e)
        {

            BTGNcls.DSelzcType dselzctype = BTGNcls.DSelzcType.UnKnow;
            lock (SelD485obj)
            {
                string selectstr = cbSELDzc.Text;
                string selectstrcom = cbSELcomzc.Text;
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        if (selectstr == "专变1型")
                        {
                            dselzctype = BTGNcls.DSelzcType.Z1;
                        }
                        else
                        {
                            dselzctype = BTGNcls.DSelzcType.C2;
                        }
                        if (selectstrcom == "COM1")
                        {
                            _server.exceallthread.NowBTGNCScls.btgns[i].Num232 = 1;
                        }
                        else
                        {
                            _server.exceallthread.NowBTGNCScls.btgns[i].Num232 = 1;
                        }
                        dselzcset(i, dselzctype);
                    }
                }
                m_Application.SetSystemConfig(this._server.exceallthread.NowBTGNCScls);
            }
        }
        private void btnSELD485jc_Click(object sender, EventArgs e)
        {
            BTGNcls.DSeljcType dseljctype = BTGNcls.DSeljcType.UnKnow;
            BTGNcls.Sel485jcType sel485jctype = BTGNcls.Sel485jcType.UnKnow;
            lock (SelD485obj)
            {
                string selectstr = cbSELDjc.Text;
                string selectstr485 = cbSEL485jc.Text;
                string selectstrcom = cbSELcomjc.Text;
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        if (selectstr == "集中器2型")
                        {
                            dseljctype = BTGNcls.DSeljcType.J2;
                        }
                        else if (selectstr == "采集器1型")
                        {
                            dseljctype = BTGNcls.DSeljcType.C1;
                        }
                        else
                        {
                            dseljctype = BTGNcls.DSeljcType.DB;
                        }
                        if (selectstr485 == "485Ⅰ")
                        {
                            sel485jctype = BTGNcls.Sel485jcType.JC485_1;
                        }
                        else if (selectstr485 == "485Ⅱ")
                        {
                            sel485jctype = BTGNcls.Sel485jcType.JC485_2;
                        }
                        else
                        {
                            sel485jctype = BTGNcls.Sel485jcType.JC485_3;
                        }
                        if (selectstrcom == "COM1")
                        {
                            _server.exceallthread.NowBTGNCScls.btgns[i].Num232 = 1;
                        }
                        else
                        {
                            _server.exceallthread.NowBTGNCScls.btgns[i].Num232 = 2;
                        }
                        dseljcset(i, dseljctype);
                        sel485jcset(i, dseljctype, sel485jctype);
                    }
                }
                m_Application.SetSystemConfig(this._server.exceallthread.NowBTGNCScls);
            }
        }
        #endregion

        #region 遥控，告警
        private object YKGJobj = new object();
        private void ykgjget(int index)
        {
            
            TCPClientChannelargsUserToken lnk = _server.exceallthread.getSocketClient(TCPClientChannelTypeOptions.LinkBZ, index);
            ExecData ed = new ExecData(lnk);
            bool f = false;
            try
            {
                //if (checkCom(PortType.BZ))
                {
                    _server.exceallthread.timerwait();
                    f = _server.exceallthread.NowBTGNCScls.btgns[index].cmd_ykgjGetSend(lnk, ed);
                    _server.exceallthread.timerrun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "摇信控制", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (f)
                {
                    ykgjrefresh(index, _server.exceallthread.NowBTGNCScls.btgns[index].ykbhs1, _server.exceallthread.NowBTGNCScls.btgns[index].ykstates1, _server.exceallthread.NowBTGNCScls.btgns[index].ykbhs2, _server.exceallthread.NowBTGNCScls.btgns[index].ykstates2, _server.exceallthread.NowBTGNCScls.btgns[index].gjbh, _server.exceallthread.NowBTGNCScls.btgns[index].gjstate);
                }
                if (LJJDJSport != null)
                    LJJDJSport.Close();
                if (LJJBZSport != null)
                    LJJBZSport.Close();
            }
        }

        private void ykgjrefresh(int index, MainForm.BTGNcls.YKBHType[] ykbhs1, MainForm.BTGNcls.YKType[] ykstates1, MainForm.BTGNcls.YKBHType[] ykbhs2, MainForm.BTGNcls.YKType[] ykstates2, MainForm.BTGNcls.GJBHType gjbh, MainForm.BTGNcls.GJType gjstate)
        {
            try
            {
                lvYKGJ.Visible = false;
                lvYKGJ.Items[index * 3].SubItems[2].Text = MainForm.BTGNcls.getYKBHType(ykbhs1[0]);
                lvYKGJ.Items[index * 3].SubItems[3].Text = MainForm.BTGNcls.getYKType(ykstates1[0]);
                lvYKGJ.Items[index * 3].SubItems[5].Text = MainForm.BTGNcls.getYKBHType(ykbhs1[1]);
                lvYKGJ.Items[index * 3].SubItems[6].Text = MainForm.BTGNcls.getYKType(ykstates1[1]);
                lvYKGJ.Items[index * 3 + 1].SubItems[2].Text = MainForm.BTGNcls.getYKBHType(ykbhs2[0]);
                lvYKGJ.Items[index * 3 + 1].SubItems[3].Text = MainForm.BTGNcls.getYKType(ykstates2[0]);
                lvYKGJ.Items[index * 3 + 1].SubItems[5].Text = MainForm.BTGNcls.getYKBHType(ykbhs2[1]);
                lvYKGJ.Items[index * 3 + 1].SubItems[6].Text = MainForm.BTGNcls.getYKType(ykstates2[1]);

                lvYKGJ.Items[index * 3 + 2].SubItems[2].Text = MainForm.BTGNcls.getGJBHType(gjbh);
                lvYKGJ.Items[index * 3 + 2].SubItems[3].Text = MainForm.BTGNcls.getGJType(gjstate);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                lvYKGJ.Visible = true;
            }
        }

        private void btnYKGJ_Click(object sender, EventArgs e)
        {
            lock (YKGJobj)
            {
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btSelIS.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btSelIS[i])
                    {
                        ykgjget(i);
                        Thread.Sleep(500);
                    }
                }
            }
        }
        #endregion

#endregion
 
        private void pagegj_SizeChanged(object sender, EventArgs e)
        {
            pagegj.RightMargin = pagegj.Size.Width - 25;
        }

        private void pagetcpclient_SizeChanged(object sender, EventArgs e)
        {
            pagetcpclient.RightMargin = pagetcpclient.Size.Width - 25;
        }

        private void tabbt_Selected(object sender, TabControlEventArgs e)
        {
            TabControl tab = sender as TabControl;
            if (tab.SelectedTab.Text == "机电控制")
            {
                #region 机电控制
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btgns.Length; i++)
                {
                    djkzrefresh(i, 1, _server.exceallthread.NowBTGNCScls.btgns[i].jdkz1state, MainForm.BTGNcls.JDKZType.Unknow);
                    djkzrefresh(i, 2, MainForm.BTGNcls.JDKZType.Unknow, _server.exceallthread.NowBTGNCScls.btgns[i].jdkz2state);
                }
                #endregion
            }
            else if (tab.SelectedTab.Text == "设备接口")
            {
                #region 设备接口
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btgns.Length; i++)
                {
                    selDrefresh(i, _server.exceallthread.NowBTGNCScls.btgns[i].deviceseld1,_server.exceallthread.NowBTGNCScls.btgns[i].deviceseld2, _server.exceallthread.NowBTGNCScls.btgns[i].deviceseld3);
                    sel485refresh(i, _server.exceallthread.NowBTGNCScls.btgns[i].sel4851, _server.exceallthread.NowBTGNCScls.btgns[i].deviceseld2, _server.exceallthread.NowBTGNCScls.btgns[i].sel4853);
                }
                #endregion
            }
            else if (tab.SelectedTab.Text == "设备上电")
            {
                #region 设备上电
                bool[] okis;
                string[] resultstr;
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btgns.Length; i++)
                {
                    okis = new bool[6] { true, true, true, true, true , true};
                    resultstr = new string[6] { "", "", "", "", "" , ""};
                    if (_server.exceallthread.NowBTGNCScls.btgns[i].tdddlstate == MainForm.BTGNcls.TDDDLDYType.Unknow)
                    {
                        resultstr[0] = "";
                    }
                    else if (_server.exceallthread.NowBTGNCScls.btgns[i].tdddlstate == MainForm.BTGNcls.TDDDLDYType.DD)
                    {
                        resultstr[0] = "不通过";
                    }
                    else
                    {
                        resultstr[0] = "通过";
                    }
                    if (_server.exceallthread.NowBTGNCScls.btgns[i].tddbdlstate == MainForm.BTGNcls.TDDDLDYType.Unknow)
                    {
                        resultstr[1] = "";
                    }
                    else if (_server.exceallthread.NowBTGNCScls.btgns[i].tddbdlstate == MainForm.BTGNcls.TDDDLDYType.DD)
                    {
                        resultstr[1] = "不通过";
                    }
                    else
                    {
                        resultstr[1] = "通过";
                    }
                    if (_server.exceallthread.NowBTGNCScls.btgns[i].tdddystate == MainForm.BTGNcls.TDDDLDYType.Unknow)
                    {
                        resultstr[1] = "";
                    }
                    else if (_server.exceallthread.NowBTGNCScls.btgns[i].tdddystate == MainForm.BTGNcls.TDDDLDYType.DD)
                    {
                        resultstr[2] = "不通过";
                    }
                    else
                    {
                        resultstr[2] = "通过";
                    }

                    if (_server.exceallthread.NowBTGNCScls.btgns[i].tddjzstate == MainForm.BTGNcls.TDDType.Unknow)
                    {
                        resultstr[3] = "";
                    }
                    else if (_server.exceallthread.NowBTGNCScls.btgns[i].tddjzstate == MainForm.BTGNcls.TDDType.DD)
                    {
                        resultstr[3] = "断电";
                    }
                    else
                    {
                        resultstr[3] = "上电";
                    }

                    if (_server.exceallthread.NowBTGNCScls.btgns[i].tddc1state == MainForm.BTGNcls.TDDType.Unknow)
                    {
                        resultstr[4] = "";
                    }
                    else if (_server.exceallthread.NowBTGNCScls.btgns[i].tddc1state == MainForm.BTGNcls.TDDType.DD)
                    {
                        resultstr[4] = "断电";
                    }
                    else
                    {
                        resultstr[4] = "上电";
                    }

                    if (_server.exceallthread.NowBTGNCScls.btgns[i].tddc2state == MainForm.BTGNcls.TDDType.Unknow)
                    {
                        resultstr[5] = "";
                    }
                    else if (_server.exceallthread.NowBTGNCScls.btgns[i].tddc2state == MainForm.BTGNcls.TDDType.DD)
                    {
                        resultstr[5] = "断电";
                    }
                    else
                    {
                        resultstr[5] = "上电";
                    }
                    tddrefresh(i, okis, resultstr);
                }
                #endregion
            }
            else if (tab.SelectedTab.Text == " 遥    信 ")
            {
                #region 遥信
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btgns.Length; i++)
                {
                    if (_server.exceallthread.NowBTGNCScls.btgns[i].deviceseld1 == MainForm.BTGNcls.DSeljzType.J1)
                    {
                        yxjzrefresh(i, MainForm.BTGNcls.YXDeviceType.J, _server.exceallthread.NowBTGNCScls.btgns[i].yxzjstarts, MainForm.BTGNcls.YXType.Unknow);
                    
                    }
                    else if (_server.exceallthread.NowBTGNCScls.btgns[i].deviceseld1 == MainForm.BTGNcls.DSeljzType.Z3)
                    {
                        yxjzrefresh(i, MainForm.BTGNcls.YXDeviceType.Z, _server.exceallthread.NowBTGNCScls.btgns[i].yxzjstarts, MainForm.BTGNcls.YXType.Unknow);
                    }
                    yxjzrefresh(i, MainForm.BTGNcls.YXDeviceType.C, null, _server.exceallthread.NowBTGNCScls.btgns[i].yxcstart);
                }
                #endregion
            }
            else if (tab.SelectedTab.Text == "遥控告警")
            {
                #region 遥控告警
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btgns.Length; i++)
                {
                    ykgjrefresh(i, _server.exceallthread.NowBTGNCScls.btgns[i].ykbhs1, _server.exceallthread.NowBTGNCScls.btgns[i].ykstates1, _server.exceallthread.NowBTGNCScls.btgns[i].ykbhs2, _server.exceallthread.NowBTGNCScls.btgns[i].ykstates2, _server.exceallthread.NowBTGNCScls.btgns[i].gjbh, _server.exceallthread.NowBTGNCScls.btgns[i].gjstate);
                }
                #endregion
            }
            else if (tab.SelectedTab.Text == " 脉    冲 ")
            {
                #region 遥控告警
                for (int i = 0; i < _server.exceallthread.NowBTGNCScls.btgns.Length; i++)
                {
                    MCrefresh(i, MainForm.BTGNcls.MCCMDType.DX, MainForm.BTGNcls.MCDevicType.MC1, _server.exceallthread.NowBTGNCScls.btgns[i].mcdx1, _server.exceallthread.NowBTGNCScls.btgns[i].mcsj1, _server.exceallthread.NowBTGNCScls.btgns[i].mcgs1, _server.exceallthread.NowBTGNCScls.btgns[i].mcse1.ToString());
                    MCrefresh(i, MainForm.BTGNcls.MCCMDType.SJ, MainForm.BTGNcls.MCDevicType.MC1, _server.exceallthread.NowBTGNCScls.btgns[i].mcdx1, _server.exceallthread.NowBTGNCScls.btgns[i].mcsj1, _server.exceallthread.NowBTGNCScls.btgns[i].mcgs1, _server.exceallthread.NowBTGNCScls.btgns[i].mcse1.ToString());
                    MCrefresh(i, MainForm.BTGNcls.MCCMDType.GS, MainForm.BTGNcls.MCDevicType.MC1, _server.exceallthread.NowBTGNCScls.btgns[i].mcdx1, _server.exceallthread.NowBTGNCScls.btgns[i].mcsj1, _server.exceallthread.NowBTGNCScls.btgns[i].mcgs1, _server.exceallthread.NowBTGNCScls.btgns[i].mcse1.ToString());
                    MCrefresh(i, MainForm.BTGNcls.MCCMDType.SE, MainForm.BTGNcls.MCDevicType.MC1, _server.exceallthread.NowBTGNCScls.btgns[i].mcdx1, _server.exceallthread.NowBTGNCScls.btgns[i].mcsj1, _server.exceallthread.NowBTGNCScls.btgns[i].mcgs1, _server.exceallthread.NowBTGNCScls.btgns[i].mcse1.ToString());

                    MCrefresh(i, MainForm.BTGNcls.MCCMDType.DX, MainForm.BTGNcls.MCDevicType.MC2, _server.exceallthread.NowBTGNCScls.btgns[i].mcdx2, _server.exceallthread.NowBTGNCScls.btgns[i].mcsj2, _server.exceallthread.NowBTGNCScls.btgns[i].mcgs2, _server.exceallthread.NowBTGNCScls.btgns[i].mcse2.ToString());
                    MCrefresh(i, MainForm.BTGNcls.MCCMDType.SJ, MainForm.BTGNcls.MCDevicType.MC2, _server.exceallthread.NowBTGNCScls.btgns[i].mcdx2, _server.exceallthread.NowBTGNCScls.btgns[i].mcsj2, _server.exceallthread.NowBTGNCScls.btgns[i].mcgs2, _server.exceallthread.NowBTGNCScls.btgns[i].mcse2.ToString());
                    MCrefresh(i, MainForm.BTGNcls.MCCMDType.GS, MainForm.BTGNcls.MCDevicType.MC2, _server.exceallthread.NowBTGNCScls.btgns[i].mcdx2, _server.exceallthread.NowBTGNCScls.btgns[i].mcsj2, _server.exceallthread.NowBTGNCScls.btgns[i].mcgs2, _server.exceallthread.NowBTGNCScls.btgns[i].mcse2.ToString());
                    MCrefresh(i, MainForm.BTGNcls.MCCMDType.SE, MainForm.BTGNcls.MCDevicType.MC2, _server.exceallthread.NowBTGNCScls.btgns[i].mcdx2, _server.exceallthread.NowBTGNCScls.btgns[i].mcsj2, _server.exceallthread.NowBTGNCScls.btgns[i].mcgs2, _server.exceallthread.NowBTGNCScls.btgns[i].mcse2.ToString());
                }
                #endregion
            }

        }

        private void cbcbshowis_CheckedChanged(object sender, EventArgs e)
        {
            cbcbshowis1.Checked = cbcbshowis.Checked;
            cbcbshowis2.Checked = cbcbshowis.Checked;
            cbcbshowis3.Checked = cbcbshowis.Checked;
            cbcbshowis4.Checked = cbcbshowis.Checked;
            cbcbshowis5.Checked = cbcbshowis.Checked;
            cbcbshowis6.Checked = cbcbshowis.Checked;
            cbcbshowis7.Checked = cbcbshowis.Checked;
            cbcbshowis8.Checked = cbcbshowis.Checked;
        }

        private void checkBox0A0D_CheckedChanged(object sender, EventArgs e)
        {
            this._server.Add0A0Dis = checkBox0A0D.Checked;
        }
    }
    class messageshow
    {
        public ChannelMessageTypeOptions messageType;
        public string message;
    }
}
