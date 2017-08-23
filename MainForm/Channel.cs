using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections;
using AC.Base.ChannelServices;
using AC.Base;
using System.Xml;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace MainForm
{
    /// <summary>
    /// 上行监听 1,未发现通信端口，2，通信超时
    /// </summary>
    public class Channel : AC.Base.ISystemConfig
    {
        #region ISystemConfig实现
        public void SetConfig(System.Xml.XmlNode serverConfig)
        {
            for (int i = 0; i < serverConfig.ChildNodes.Count; i++)
            {
                #region server
                if (serverConfig.ChildNodes[i].Name.Equals("m_UdpPort"))
                {
                    udpPort = Function.ToInt(serverConfig.ChildNodes[i].InnerText);
                }
                if (serverConfig.ChildNodes[i].Name.Equals("DSPCOM"))
                {
                    DSPCOM = Function.ToInt(serverConfig.ChildNodes[i].InnerText);
                }
                if (serverConfig.ChildNodes[i].Name.Equals("m_TcpPort"))
                {
                    tcpPort = Function.ToInt(serverConfig.ChildNodes[i].InnerText);
                }
                if (serverConfig.ChildNodes[i].Name.Equals("m_UdpListenIp"))
                {
                    udpIp = serverConfig.ChildNodes[i].InnerText;
                }
                if (serverConfig.ChildNodes[i].Name.Equals("m_TcpListenIp"))
                {
                    tcpIp = serverConfig.ChildNodes[i].InnerText;
                }
                if (serverConfig.ChildNodes[i].Name.Equals("TcpClientEPs"))
                {
                    try
                    {
                        string[] _eps = serverConfig.ChildNodes[i].InnerText.Split(';');
                        string[] _ep;
                        EndPoint _ed;
                        for (int _i = 0; _i < _eps.Length; _i++)
                        {
                            if (_eps[i] != "")
                            {
                                _ep = _eps[_i].Split(':');
                                _ed = new IPEndPoint(System.Net.IPAddress.Parse(_ep[0]), Convert.ToInt32(_ep[1]));
                                opentcpclients.TryAdd(_ed, _ed);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                #endregion
            }
        }
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xn = xmlDoc.CreateElement("SocketServer");

            #region server
            XmlNode XMLm_UdpPort = xmlDoc.CreateElement("m_UdpPort");
            XMLm_UdpPort.InnerText = this.udpPort.ToString();
            xn.AppendChild(XMLm_UdpPort);

            XmlNode XMLm_DSPCOM = xmlDoc.CreateElement("DSPCOM");
            XMLm_DSPCOM.InnerText = this.DSPCOM.ToString();
            xn.AppendChild(XMLm_DSPCOM);

            XmlNode XMLm_TcpPort = xmlDoc.CreateElement("m_TcpPort");
            XMLm_TcpPort.InnerText = this.tcpPort.ToString();
            xn.AppendChild(XMLm_TcpPort);

            XmlNode XMLm_UdpListenIp = xmlDoc.CreateElement("m_UdpListenIp");
            XMLm_UdpListenIp.InnerText = this.udpIp.ToString();
            xn.AppendChild(XMLm_UdpListenIp);

            XmlNode XMLm_TcpListenIp = xmlDoc.CreateElement("m_TcpListenIp");
            XMLm_TcpListenIp.InnerText = this.tcpIp.ToString();
            xn.AppendChild(XMLm_TcpListenIp);

            try
            {
                if (opentcpclients.Count > 0)
                {
                    XmlNode XMLm_TcpClientEPs = xmlDoc.CreateElement("TcpClientEPs");
                    foreach (KeyValuePair<EndPoint, EndPoint> _eps in opentcpclients)
                    {
                        XMLm_TcpClientEPs.InnerText = XMLm_TcpClientEPs.InnerText + _eps.Value.ToString() + ";";
                    }
                    xn.AppendChild(XMLm_TcpClientEPs);
                }
            }
            catch (Exception ex)
            { }
            #endregion

            return xn;
        }
        #endregion

        private const int BUFFERSIZE = 2048; //最大接收字节长度
        private byte[] buffer = new byte[BUFFERSIZE];
        public int delay = 30;//超时时间 单位：100ms

        private object linklock = new object();
        private object tcpclientlock = new object();

        #region << 通道服务运行状态发生改变后的事件 >>
        /// <summary>
        /// 通道服务运行状态发生改变后的事件所调用的委托。
        /// </summary>
        public delegate void StateChangedEventHandler();

        /// <summary>
        /// 通道服务运行状态发生改变后的事件。
        /// </summary>
        public event StateChangedEventHandler StateChanged;

        private void OnStateChanged()
        {
            this.OnMessage(ChannelMessageTypeOptions.Importance, "台体服务程序运行状态变为" + this.State.GetDescription());
            if (this.StateChanged != null)
            {
                this.StateChanged();
            }
        }
        #endregion

        #region <<  通道服务运行状态改变时产生的事件 >>

        private ChannelStateOptions m_State = ChannelStateOptions.Stopped;
        /// <summary>
        /// 通道服务当前运行状态。
        /// </summary>
        public ChannelStateOptions State
        {
            get { return this.m_State; }
            set
            {
                if (this.m_State != value)
                {
                    this.m_State = value;
                    this.OnStateChanged();
                }
            }
        }

        /// <summary>
        /// 通道服务运行状态改变时产生的事件所调用的委托。
        /// </summary>
        /// <param name="state">即将改变的状态。</param>
        public delegate void ChangingEventHandler(ChannelStateOptions state);

        /// <summary>
        /// 通道服务运行状态发生改变时产生的事件。
        /// </summary>
        public event ChangingEventHandler StateChanging;


        private void OnStateChanging(ChannelStateOptions state)
        {
            if (this.StateChanging != null)
            {
                this.StateChanging(state);
            }
        }
        #endregion

        #region << 通道服务产生消息的事件 >>
        /// <summary>
        /// 通道服务产生的消息事件。
        /// </summary>
        public event MessageEventHandler EventMessage;

        /// <summary>
        /// 通道服务产生的消息事件所调用的委托。
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="message">消息内容。</param>
        public delegate void MessageEventHandler(ChannelMessageTypeOptions MessageType, string Message);

        /// <summary>
        /// 引发通道服务产生消息事件
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="message">消息内容。</param>
        internal void OnMessage(ChannelMessageTypeOptions MessageType, string Message)
        {
            if (this.EventMessage != null)
            {
                this.EventMessage(MessageType, Message);
            }
        }
        #endregion

        #region << 等待滚屏结束时的事件 >>
        /// <summary>
        /// 等待滚屏结束时的事件。
        /// </summary>
        public event EndWaitFormEventHandler EventEndWaitForm;

        /// <summary>
        /// 等待滚屏结束时的事件所调用的委托。
        /// </summary>
        public delegate void EndWaitFormEventHandler();

        /// <summary>
        /// 引发等待滚屏结束时的事件
        /// </summary>
        internal void OnEndWaitForm()
        {
            if (this.EventEndWaitForm != null)
            {
                this.EventEndWaitForm();
            }
        }
        #endregion

        #region << 记录通道TCPClient打开端口以便崩溃重启事件 >>GP
        /// <summary>
        /// 通道事件。
        /// </summary>
        public event SaveTCLClientOpensHandler SaveTCLClientOpens;
        /// <summary>
        /// 通道事件所调用的委托。
        /// </summary>
        public delegate void SaveTCLClientOpensHandler();
        /// <summary>
        /// 引发通道事件
        /// </summary>
        internal void OnSaveTCLClientOpens(SocketAsyncEventArgs args, bool openis)
        {
            try
            {
                EndPoint _ep = null;
                if (args != null)
                {
                    if (openis)
                        opentcpclients.TryAdd(args.RemoteEndPoint, args.RemoteEndPoint);
                    else
                        opentcpclients.TryRemove(args.RemoteEndPoint, out _ep);
                }
                else
                {
                    if (openis)
                    {

                    }
                    else
                    {
                        opentcpclients.Clear();
                    }
                }

                if (SaveTCLClientOpens != null)
                    SaveTCLClientOpens();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region << tcp 终端在线监测事件 >>GP
        /// <summary>
        /// tcp 终端在线监测事件
        /// </summary>
        public event CheckOnlinesCHandler CheckOnlines;
        /// <summary>
        /// tcp 终端在线监测事件
        /// </summary>
        public delegate void CheckOnlinesCHandler(ConcurrentDictionary<string, SocketAsyncEventArgs> _Onlines);
        /// <summary>
        /// 引发通道事件
        /// </summary>
        internal void OnCheckOnlines(ConcurrentDictionary<string, SocketAsyncEventArgs> _Onlines)
        {
            try
            {
                if (CheckOnlines != null)
                    CheckOnlines(_Onlines);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        public int DSPCOM = 1;

        #region << udp 通道参数 >>
        /// 获取或设置后台服务监听的端口。如果该值小于等于 0 则表示不启动后台服务。
        public int udpPort = 20000;
        private string udpIp = "127.0.0.1";
        private Socket udpReceivedSocket;//udp 接收socket
        private Socket udpSendSocket;//
        private SocketAsyncEventArgs udpReceivedSocketAsync;//接收数据使用的异步套接字
        //private SocketAsyncEventArgs udpSendSocketAsync;//发送数据使用的异步套接字

        public List<EndPoint> udpUPeplist = new List<EndPoint>();
        #endregion

        #region << tcp 通道对象 >>
        private int tcpPort = 0;
        private string tcpIp = "127.0.0.1";
        protected Socket m_SocketTcpServer;
        internal ConcurrentDictionary<int, byte[]> m_Result;

        public ConcurrentDictionary<string, SocketAsyncEventArgs> m_Onlines;  //在线终端
        #endregion

        #region << tcp client 通道参数 >>
        public ConcurrentDictionary<EndPoint, SocketAsyncEventArgs> m_Clients;
        public ExecALLThread exceallthread;
        /// <summary>
        /// 当前打开的TCPclient，中途崩溃后，开启重连
        /// </summary>
        public ConcurrentDictionary<EndPoint, EndPoint> opentcpclients = new ConcurrentDictionary<EndPoint, EndPoint>();
        public void ExecALLThreadInit(ApplicationClass m_Application)
        {
            try
            {
                m_Clients = new ConcurrentDictionary<EndPoint, SocketAsyncEventArgs>();
                if (exceallthread == null)
                {
                    exceallthread = m_Application.GetSystemConfig(typeof(ExecALLThread)) as ExecALLThread;
                    //m_application.SetSystemConfig(NowBTGNCScls);
                    exceallthread.server = this;

                    exceallthread.InitValue(m_Application);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region << 套接字操作 Start & Stop >>
        /// <summary>
        ///打开通道服务后台监听
        /// </summary>
        public void Start()
        {
            lock (linklock)
            {
                if (this.State != ChannelStateOptions.Running)
                {
                    try
                    {
                        this.State = ChannelStateOptions.Running;

                        #region << TCP 服务 >>
                        if (this.tcpPort > 0)
                        {
                            this.m_Onlines = new ConcurrentDictionary<string, SocketAsyncEventArgs>();
                            this.m_Result = new ConcurrentDictionary<int, byte[]>();
                            this.m_SocketTcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            this.m_SocketTcpServer.SendBufferSize = BUFFERSIZE;
                            this.m_SocketTcpServer.ReceiveBufferSize = BUFFERSIZE;
                            this.m_SocketTcpServer.Bind(new IPEndPoint(System.Net.IPAddress.Any, this.tcpPort));
                            this.m_SocketTcpServer.Listen(10);
                            Accept();//开始接受监听返回
                            this.OnMessage(ChannelMessageTypeOptions.Importance, string.Format("台体服务程序已经启动 , 监听[{0}:{1}]", this.tcpIp, this.tcpPort.ToString()));
                        }
                        #endregion

                        #region <<  udp 服务 >>
                        if (udpPort != 0)
                        {
                            this.udpReceivedSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                            this.udpReceivedSocket.Bind(new IPEndPoint(System.Net.IPAddress.Any, udpPort));

                            this.udpReceivedSocketAsync = new SocketAsyncEventArgs();
                            this.udpReceivedSocketAsync.RemoteEndPoint = new IPEndPoint(System.Net.IPAddress.Any, udpPort);// new IPEndPoint(IPAddress.Any, 0);
                            this.udpReceivedSocketAsync.Completed += new EventHandler<SocketAsyncEventArgs>(udpReceivedSocketAsync_Completed);
                            this.udpReceivedSocketAsync.SetBuffer(this.buffer, 0, BUFFERSIZE);

                            this.udpReceiveFrom();
                            this.OnMessage(ChannelMessageTypeOptions.Importance, string.Format("Udp service 已经启动 , 监听[{0}:{1}]", this.tcpIp, this.udpPort));
                        }
                        #endregion

                        this.exceallthread.OpenSaveTCLClientOpens();
                    }
                    catch (Exception ex)
                    {
                        this.State = ChannelStateOptions.Stopped;

                        if (m_SocketTcpServer != null)
                            m_SocketTcpServer.Close();

                        if (udpReceivedSocket != null)
                        {
                            this.udpReceivedSocketAsync.Completed -= new EventHandler<SocketAsyncEventArgs>(udpReceivedSocketAsync_Completed);

                            udpReceivedSocketAsync.Dispose();
                            udpReceivedSocket.Close();
                        }
                        this.OnMessage(ChannelMessageTypeOptions.UDPError, "通道服务启动失败，" + ex.Message);
                    }
                }
                this.OnEndWaitForm();
            }
        }
        /// <summary>
        /// 停止通道。该方法由通道服务程序或前置机程序作为宿主进行调用，调用该方法后通道应首先关闭设备端服务断开所有设备的连接，然后通知所有已连接的后台自身即将关闭的消息并关闭后台服务。对于通道安全运行的多种形式，如主备通道或多主通道，则由具体实现完成该功能。
        /// </summary>
        public void Stop()
        {
            lock (linklock)
            {
                try
                {
                    this.State = ChannelStateOptions.Stopped;
                    if (this.m_Onlines != null)
                    {
                        foreach (KeyValuePair<string, SocketAsyncEventArgs> kvp in this.m_Onlines)
                            this.SocketAsyncFinish(kvp.Value);
                    }
                    if (this.m_Clients != null)
                    {
                        foreach (KeyValuePair<EndPoint, SocketAsyncEventArgs> kvp in this.m_Clients)
                            this.SocClientAsyncFinish(kvp.Value);
                    }
                    exceallthread.ClearList();
                    //exceallthread.Clear485cbList();

                    if (this.m_SocketTcpServer != null)
                        this.m_SocketTcpServer.Close();
                    if (this.udpReceivedSocket != null)
                    {
                        this.udpReceivedSocketAsync.Completed -= new EventHandler<SocketAsyncEventArgs>(udpReceivedSocketAsync_Completed);
                        this.udpReceivedSocket.Close();
                        this.udpReceivedSocketAsync.Dispose();
                    }
                    OnSaveTCLClientOpens(null, false);
                    this.OnMessage(ChannelMessageTypeOptions.Importance, "台体服务通道已被关闭，所有链接都被释放。");
                }
                catch { }
                this.OnEndWaitForm();
            }
        }
        public void udpgetstate()
        {
            try
            {
                if (this.State == ChannelStateOptions.Running)
                {
                    this.OnMessage(ChannelMessageTypeOptions.UDPInfo, string.Format("UDP[{0}]打开", udpReceivedSocket.LocalEndPoint.ToString()));
                }
                else
                {
                    this.OnMessage(ChannelMessageTypeOptions.UDPInfo, string.Format("UDP通道未开启"));
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void tcpcliengettstate()
        {
            try
            {
                if (this.m_Clients != null)
                {
                    TCPClientChannelargsUserToken tcpclientut = null;
                    string info = "";
                    ConcurrentDictionary<TCPClientChannelTypeOptions, ConcurrentDictionary<string, string>> m_TcpClientTypes = new ConcurrentDictionary<TCPClientChannelTypeOptions, ConcurrentDictionary<string, string>>();
                    ConcurrentDictionary<string, string> _templist;
                    foreach (KeyValuePair<EndPoint, SocketAsyncEventArgs> kvp in this.m_Clients)
                    {
                        tcpclientut = kvp.Value.UserToken as TCPClientChannelargsUserToken;
                        if (kvp.Value.SocketError == SocketError.Success)
                        {
                            info = "打开";
                        }
                        else
                        {
                            info = "关闭[未释放]";
                        }
                        if (tcpclientut != null)
                        {
                            m_TcpClientTypes.TryGetValue(tcpclientut.PortType, out _templist);
                            if (_templist == null)
                            {
                                _templist = new ConcurrentDictionary<string, string>();
                                m_TcpClientTypes.TryAdd(tcpclientut.PortType, _templist);
                            }
                            _templist.TryAdd(kvp.Value.RemoteEndPoint.ToString(), tcpclientut.PortName + tcpclientut.PortIDName + "[" + info + "]");
                        }
                        else
                        {
                            m_TcpClientTypes.TryGetValue(TCPClientChannelTypeOptions.Unknow, out _templist);
                            if (_templist == null)
                            {
                                _templist = new ConcurrentDictionary<string, string>();
                                m_TcpClientTypes.TryAdd(TCPClientChannelTypeOptions.Unknow, _templist);
                            }

                            _templist.TryAdd(kvp.Value.RemoteEndPoint.ToString(), "未定义类型端口 [" + info + "]");
                        }
                    }
                    int _int = 0;
                    foreach (KeyValuePair<TCPClientChannelTypeOptions, ConcurrentDictionary<string, string>> kvp in m_TcpClientTypes)
                    {
                        this.OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("=========={1} 端口个数[{0}]==========", kvp.Value.Count, kvp.Key.ToString()));
                        foreach (KeyValuePair<string, string> _kvp in kvp.Value)
                        {
                            this.OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("{1}[{0}]", _kvp.Key, _kvp.Value));
                        }
                    }
                    if (m_Clients.Count == 0)
                    {
                        this.OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("所有MOXA通道未开启"));
                    }
                }
                else
                {
                    this.OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("所有MOXA通道未开启"));
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region << TCP 通道回调 >>
        /// <summary>
        /// 监听连接
        /// </summary>
        private void Accept()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(SocketAsyncEventArgs_Completed);

            try
            {
                this.m_SocketTcpServer.AcceptAsync(args);
            }
            catch (Exception ex)
            {
                this.OnMessage(ChannelMessageTypeOptions.Importance, ex.ToString());
                this.SocketAsyncFinish(args);
                this.Accept();
            }
        }

        internal void SocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.AcceptSocket != null && args.AcceptSocket.Connected)
                {
                    switch (args.LastOperation)
                    {
                        case SocketAsyncOperation.Accept:
                            this.AcceptCallback(args);
                            break;
                        case SocketAsyncOperation.Receive:
                            this.ReceiveCallback(args);
                            break;
                        default:
                            this.DoException(args, new Exception("在 SocketAsyncEventArgs_Completed 方法中 SocketAsyncEventArgs 的 LastOperation 属性为 " + args.LastOperation.ToString() + "，通常不应出现该操作。"));
                            break;
                    }
                }
                else
                    this.SocketAsyncFinish(args);
            }
            catch (Exception ex)
            {
                this.DoException(args, ex);
            }
        }

        /// <summary>
        /// 监听回调
        /// </summary>
        /// <param name="args"></param>
        protected virtual void AcceptCallback(SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    args.SetBuffer(buffer, 0, BUFFERSIZE);
                    //args.AcceptSocket.IOControl(-1744830460, new byte[] { 1, 0, 0, 0, 0x30, 0x75, 0, 0, 0xD0, 0x07, 0, 0 }, null); //keepalvice
                    this.OnMessage(ChannelMessageTypeOptions.Importance, string.Format("已接受来自 {0} 的连接请求", args.AcceptSocket.RemoteEndPoint));

                    if (!args.AcceptSocket.ReceiveAsync(args))
                        this.ReceiveCallback(args);
                }
                else
                {
                    this.OnMessage(ChannelMessageTypeOptions.UDPError, args.SocketError.ToString());
                    this.SocketAsyncFinish(args);
                }
            }
            catch (Exception e)
            {
                this.OnMessage(ChannelMessageTypeOptions.UDPError, e.Message);
            }
            finally
            {
                this.Accept();
            }
        }

        /// <summary>
        /// 数据接收回调，数据处理。
        /// </summary>
        /// <param name="args"></param>
        protected virtual void ReceiveCallback(SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                try
                {
                    byte[] _temp = new byte[args.BytesTransferred];
                    Array.Copy(args.Buffer, args.Offset, _temp, 0, _temp.Length);
                    this.OnMessage(ChannelMessageTypeOptions.Information, AC.Base.Function.ByteArrayToHexString(_temp));

                    if (_temp[0] == 0x68 && _temp[_temp.Length - 1] == 0x16)
                    {
                        #region << 终端主动上报帧，登录心跳 >>
                        if ((_temp[6] & 64) == 64)
                        {
                            if (_temp[12] == 2)//登陆、心跳
                            {
                                if (_temp[16] == 2)//退出
                                {
                                    this.SocketAsyncFinish(args);

                                    OnCheckOnlines(this.m_Onlines);
                                    return;
                                }
                                else
                                {
                                    #region << 登陆/心跳 发回确认帧 >>
                                    if (_temp[16] == 1)
                                    {
                                        string Addr = _temp[8].ToString("X2") + _temp[7].ToString("X2") + (_temp[9] + _temp[10] * 0x100).ToString().PadLeft(5, '0');
                                        this.OnMessage(ChannelMessageTypeOptions.Information, "[登录]" + AC.Base.Function.ByteArrayToHex(_temp) + "[" + Addr + "]");
                                        args.UserToken = Addr;

                                        if (!this.m_Onlines.TryAdd(Addr, args))
                                        {
                                            SocketAsyncEventArgs _args;
                                            this.m_Onlines.TryRemove(Addr, out _args);
                                            this.SocketAsyncFinish(_args);
                                            this.m_Onlines.TryAdd(Addr, args);

                                            OnCheckOnlines(this.m_Onlines);
                                        }
                                    }
                                    else
                                        this.OnMessage(ChannelMessageTypeOptions.Information, "[心跳]" + AC.Base.Function.ByteArrayToHex(_temp));

                                    byte[] _b = new byte[] { 0x68, 0x32, 0x00, 0x32, 0x00, 0x68, 0x1B, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x63, 0x00, 0x00, 0x01, 0x00, 0x00, 0x16 };
                                    _b[7] = _temp[7];
                                    _b[8] = _temp[8];
                                    _b[9] = _temp[9];
                                    _b[10] = _temp[10];
                                    _b[11] = _temp[11];
                                    _b[13] = (byte)(96 + (_temp[13] & 0xF));
                                    for (int i = 6; i < 18; i++)
                                        _b[18] += _b[i];
                                    _b[18] = (byte)(_b[18] % 256);

                                    this.OnMessage(ChannelMessageTypeOptions.Information, "[确认]" + AC.Base.Function.ByteArrayToHex(_b));
                                    args.AcceptSocket.Send(_b);

                                    OnCheckOnlines(this.m_Onlines);
                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            if (!this.m_Result.TryAdd(_temp[13], _temp))
                            {
                                byte[] b;
                                this.m_Result.TryRemove(_temp[13], out b);
                                Thread.Sleep(10);
                                this.m_Result.TryAdd(_temp[13], _temp);
                            }
                            this.OnMessage(ChannelMessageTypeOptions.Information, "[登录]" + AC.Base.Function.ByteArrayToHex(_temp) + "[" + _temp[13] + "]");
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    this.OnMessage(ChannelMessageTypeOptions.Importance, ex.ToString());
                }
                finally
                {
                    if (!args.AcceptSocket.ReceiveAsync(args))
                        ReceiveCallback(args);
                }
            }
            else
            {
                this.OnMessage(ChannelMessageTypeOptions.Importance, string.Format("来自 {0} 的连接断线", args.AcceptSocket.RemoteEndPoint));
                this.SocketAsyncFinish(args);
            }
        }

        /// <summary>
        /// 当前socket连接终止
        /// </summary>
        /// <param name="args"></param>
        protected void SocketAsyncFinish(SocketAsyncEventArgs args)
        {
            args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocketAsyncEventArgs_Completed);

            SocketAsyncEventArgs _args = null;
            this.m_Onlines.TryRemove(args.UserToken.ToString(), out args);

            if (args.AcceptSocket != null)
                args.AcceptSocket.Close();
            args.Dispose();
        }
        #endregion

        #region << udp 通道回调 >>
        private void udpReceivedSocketAsync_Completed(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.LastOperation == SocketAsyncOperation.ReceiveFrom)
                {
                    udpReceivedbegin(args);
                }
                else
                {
                    if (State == ChannelStateOptions.Running)
                    {
                        this.udpReceiveFrom();
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnMessage(ChannelMessageTypeOptions.UDPError, "UDP监听端口错误停止！");
            }
        }
        private void udpReceivedbegin(SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
                {
                    if (args.BytesTransferred > 0)
                    {
                        bool f = true;
                        for (int i = 0; i < udpUPeplist.Count; i++)
                        {
                            if (udpUPeplist[i].Equals(args.RemoteEndPoint))
                            {
                                f = false;
                                break;
                            }
                        }
                        if (f)
                            udpUPeplist.Add(args.RemoteEndPoint);

                        byte[] data = new byte[args.BytesTransferred];
                        Array.Copy(args.Buffer, args.Offset, data, 0, data.Length);

                        string cmd = Encoding.Default.GetString(data, 0, data.Length);

                        #region <<  1002 各表位终端抄读模拟电表数据>>
                        string[] cmdks = cmd.Split(',');
                        string key = String.Empty;
                        byte ret = 0;
                        string cmddata = String.Empty;
                        if (cmdks.Length < 3)
                        {
                            key = cmdks[0].Split('=')[1];
                            cmddata = cmdks[1].Split('=')[1];
                        }
                        else
                        {
                            key = cmdks[0].Split('=')[1];
                            ret = Convert.ToByte(cmdks[1].Split('=')[1]);
                            cmddata = cmdks[2].Split('=')[1];
                        }


                        ChannelMessageTypeOptions tempsendinfo = ChannelMessageTypeOptions.UDPreceive;
                        if (key == "1002")
                        {
                            string[] strdatas = cmddata.Split(';');
                            int _bwindex = Convert.ToInt32(strdatas[0]);

                            if (_bwindex == 1)
                                tempsendinfo = ChannelMessageTypeOptions.UDPreceiveCB1;
                            else if (_bwindex == 2)
                                tempsendinfo = ChannelMessageTypeOptions.UDPreceiveCB2;
                            else if (_bwindex == 3)
                                tempsendinfo = ChannelMessageTypeOptions.UDPreceiveCB3;
                            else if (_bwindex == 4)
                                tempsendinfo = ChannelMessageTypeOptions.UDPreceiveCB4;
                            else if (_bwindex == 5)
                                tempsendinfo = ChannelMessageTypeOptions.UDPreceiveCB5;
                            else if (_bwindex == 6)
                                tempsendinfo = ChannelMessageTypeOptions.UDPreceiveCB6;
                            else if (_bwindex == 7)
                                tempsendinfo = ChannelMessageTypeOptions.UDPreceiveCB7;
                            else if (_bwindex == 8)
                                tempsendinfo = ChannelMessageTypeOptions.UDPreceiveCB8;
                        }
                        #endregion

                        this.OnMessage(tempsendinfo, string.Format("接收数据报文[{0}]", cmd));
                        if (exceallthread != null)
                        {
                            exceallthread.addexec(cmd, args.RemoteEndPoint);
                            exceallthread.timerstart();
                        }
                        else
                        {
                            this.OnMessage(ChannelMessageTypeOptions.UDPError, "任务类未初始化！");
                        }
                    }
                }
                else
                {
                    this.OnMessage(ChannelMessageTypeOptions.UDPInfo, string.Format("[{0}]断链！", args.RemoteEndPoint));
                }
            }
            catch (Exception ex)
            {
                this.OnMessage(ChannelMessageTypeOptions.UDPError, ex.ToString());
                buffer = new byte[BUFFERSIZE];
            }
            finally
            {
                if (State == ChannelStateOptions.Running)
                {
                    this.udpReceiveFrom();
                }
            }
        }
        private void udpReceiveFrom()
        {
            try
            {
                this.udpReceivedSocketAsync.AcceptSocket = null;
                if (this.udpReceivedSocket.ReceiveFromAsync(udpReceivedSocketAsync) == false)
                {
                    udpReceivedbegin(udpReceivedSocketAsync);
                }
            }
            catch (Exception ex)
            {
                this.OnMessage(ChannelMessageTypeOptions.UDPError, ex.ToString());
            }
        }
        private void stopReceiveSocket(SocketAsyncEventArgs args)
        {
            if (args != null)
            {
                if (args.AcceptSocket != null)
                {
                    if (args.AcceptSocket.Connected)
                        args.AcceptSocket.Close();
                    args.AcceptSocket.Dispose();
                }
                args.UserToken = null;
                args.Dispose();
            }
        }
        private void udpSendSocketAsync_Completed(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.LastOperation == SocketAsyncOperation.SendTo)
                {
                    if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
                    {

                    }
                    else
                    {
                        this.OnMessage(ChannelMessageTypeOptions.UDPInfo, string.Format("[{0}]通信失败！", args.RemoteEndPoint));
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnMessage(ChannelMessageTypeOptions.UDPError, ex.ToString());
            }
            finally
            {
                args.Completed -= new EventHandler<SocketAsyncEventArgs>(udpSendSocketAsync_Completed);
                args.Dispose();
            }
        }

        public bool Add0A0Dis = false;
        /// <summary>
        /// 同步发送
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public void UdpSend(string buffer, ExecData ed)
        {
            try
            {
                #region <<  1002 各表位终端抄读模拟电表数据>>
                string[] cmdks = buffer.Split(',');
                string key = String.Empty;
                byte ret = 0;
                string data = String.Empty;
                if (cmdks.Length < 3)
                {
                    key = cmdks[0].Split('=')[1];
                    data = cmdks[1].Split('=')[1];
                }
                else
                {
                    key = cmdks[0].Split('=')[1];
                    ret = Convert.ToByte(cmdks[1].Split('=')[1]);
                    data = cmdks[2].Split('=')[1];
                }

                ChannelMessageTypeOptions tempsendinfo = ChannelMessageTypeOptions.UDPsend;
                if (key == "1002")
                {
                    string[] strdatas = data.Split(';');
                    int _bwindex = Convert.ToInt32(strdatas[0]);

                    if (_bwindex == 1)
                        tempsendinfo = ChannelMessageTypeOptions.UDPsendCB1;
                    else if (_bwindex == 2)
                        tempsendinfo = ChannelMessageTypeOptions.UDPsendCB2;
                    else if (_bwindex == 3)
                        tempsendinfo = ChannelMessageTypeOptions.UDPsendCB3;
                    else if (_bwindex == 4)
                        tempsendinfo = ChannelMessageTypeOptions.UDPsendCB4;
                    else if (_bwindex == 5)
                        tempsendinfo = ChannelMessageTypeOptions.UDPsendCB5;
                    else if (_bwindex == 6)
                        tempsendinfo = ChannelMessageTypeOptions.UDPsendCB6;
                    else if (_bwindex == 7)
                        tempsendinfo = ChannelMessageTypeOptions.UDPsendCB7;
                    else if (_bwindex == 8)
                        tempsendinfo = ChannelMessageTypeOptions.UDPsendCB8;
                }
                #endregion

                if (udpSendSocket == null)
                {
                    this.udpSendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                }
                if (ed == null)
                {
                    for (int i = 0; i < udpUPeplist.Count; i++)
                    {
                        this.OnMessage(tempsendinfo, string.Format("发送数据报文[{0}]", buffer));

                        SocketAsyncEventArgs udpSendSocketAsync = new SocketAsyncEventArgs();
                        if (Add0A0Dis)
                            buffer = buffer + "\n";

                        byte[] _sendbuffer = Encoding.ASCII.GetBytes(buffer);
                        udpSendSocketAsync.RemoteEndPoint = udpUPeplist[i];
                        udpSendSocketAsync.Completed += new EventHandler<SocketAsyncEventArgs>(udpSendSocketAsync_Completed);
                        udpSendSocketAsync.SetBuffer(_sendbuffer, 0, _sendbuffer.Length);
                        this.udpSendSocket.SendToAsync(udpSendSocketAsync);

                        //this.udpReceivedSocket.SendTo(Encoding.ASCII.GetBytes(buffer), udpUPeplist[i]);
                    }
                }
                else
                {
                    this.OnMessage(tempsendinfo, string.Format("发送数据报文[{0}]", buffer));

                    SocketAsyncEventArgs udpSendSocketAsync = new SocketAsyncEventArgs();
                    if (Add0A0Dis)
                        buffer = buffer + "\n";

                    byte[] _sendbuffer = Encoding.ASCII.GetBytes(buffer);
                    udpSendSocketAsync.RemoteEndPoint = ed.upep;
                    udpSendSocketAsync.Completed += new EventHandler<SocketAsyncEventArgs>(udpSendSocketAsync_Completed);
                    udpSendSocketAsync.SetBuffer(_sendbuffer, 0, _sendbuffer.Length);
                    this.udpSendSocket.SendToAsync(udpSendSocketAsync);

                    //this.udpReceivedSocket.SendTo(Encoding.ASCII.GetBytes(buffer), ed.upep);
                }
            }
            catch (Exception ex)
            {
                this.OnMessage(ChannelMessageTypeOptions.UDPError, string.Format("UDP发送数据报文[{0}]失败。{1}", buffer, ex.ToString()));
            }
        }
        #endregion

        #region<< tcp client 通道回调 >>
        #region << tcp client 接收数据 >>
        private void SocClientAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                //lock (tcpclientlock)
                {
                    if (args.LastOperation == SocketAsyncOperation.Receive)
                        this.SocClientBackBeginReceive(args);
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 记录是哪个TCPclient穿上来的抄表数据
        /// </summary>
        public ConcurrentDictionary<string, EndPoint> up485epslist = new ConcurrentDictionary<string, EndPoint>();

        private void SocClientBackBeginReceive(SocketAsyncEventArgs args)
        {
            TCPClientChannelargsUserToken usertokn = args.UserToken as TCPClientChannelargsUserToken;
            SocketAsyncEventArgs tempargs = null;
            try
            {
                //lock (tcpclientlock)
                {
                    if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
                    {

                        byte[] _temp = new byte[args.BytesTransferred];
                        Array.Copy(args.Buffer, args.Offset, _temp, 0, _temp.Length);

                        if (usertokn.ed == null)
                        {
                            usertokn.ed = new ExecData(usertokn);
                        }

                        if ((usertokn.PortType == TCPClientChannelTypeOptions.Link2321) || (usertokn.PortType == TCPClientChannelTypeOptions.Link2321))
                        {
                            this.OnMessage(ChannelMessageTypeOptions.MOXAreceive, AC.Base.Function.ByteArrayToHexString(_temp));
                            #region 232
                            usertokn.ADDTempBuffer232(_temp);
                            _temp = usertokn.get232Realbuffer(usertokn.ed.key, usertokn.ep.ToString());
                            while (_temp != null)
                            {
                                usertokn.ed.backdatas = _temp;
                                _temp = usertokn.get232Realbuffer(usertokn.ed.key, usertokn.ep.ToString());
                            }
                            #endregion
                        }
                        else if ((usertokn.PortType == TCPClientChannelTypeOptions.LinkDJ) || (usertokn.PortType == TCPClientChannelTypeOptions.LinkBZ))
                        {
                            this.OnMessage(ChannelMessageTypeOptions.MOXAreceive, AC.Base.Function.ByteArrayToHexString(_temp));
                            #region 板子和电机
                            if (_temp.Length == 15)
                            {
                                if ((_temp[_temp.Length - 5] == 0xAA) && (_temp[_temp.Length - 4] == 0x0D) && (_temp[_temp.Length - 3] == 0x0A))
                                {
                                    byte address = _temp[1];
                                    byte cmd = _temp[2];
                                    string key = address.ToString("X2") + cmd.ToString("X2");

                                    if (usertokn.ed.key.Equals(key))
                                        usertokn.ed.backdatas = _temp;
                                }
                                else
                                {
                                    //_serv.UdpSend(AC.Base.Function.ByteArrayToHex(_temp));
                                }
                            }
                            else if (_temp.Length == 14)
                            {
                                if ((_temp[_temp.Length - 4] == 0xAA) && (_temp[_temp.Length - 3] == 0x0D) && (_temp[_temp.Length - 2] == 0x0A))
                                {
                                    byte address = _temp[1];
                                    byte cmd = _temp[2];
                                    string key = address.ToString("X2") + cmd.ToString("X2");

                                    if (usertokn.ed.key.Equals(key))
                                        usertokn.ed.backdatas = _temp;
                                }
                                else
                                {
                                    //_serv.UdpSend(AC.Base.Function.ByteArrayToHex(_temp));
                                }
                            }
                            else
                            {
                                //_serv.UdpSend(AC.Base.Function.ByteArrayToHex(_temp));
                            }
                            #endregion
                        }
                        else if (usertokn.PortType == TCPClientChannelTypeOptions.LinkPM)
                        {
                            this.OnMessage(ChannelMessageTypeOptions.MOXAreceive, AC.Base.Function.ByteArrayToHexString(_temp));
                            #region 屏幕
                            if (_temp.Length == 15)
                            {
                                if ((_temp[_temp.Length - 5] == 0xAA) && (_temp[_temp.Length - 4] == 0x0D) && (_temp[_temp.Length - 3] == 0x0A))
                                {
                                    byte address = _temp[1];
                                    byte cmd = _temp[2];
                                    string key = address.ToString("X2") + cmd.ToString("X2");

                                    if (usertokn.ed.key.Equals(key))
                                        usertokn.ed.backdatas = _temp;
                                }
                                else
                                {
                                    //_serv.UdpSend(AC.Base.Function.ByteArrayToHex(_temp));
                                }
                            }
                            else
                            {
                                //_serv.UdpSend(AC.Base.Function.ByteArrayToHex(_temp));
                            }
                            #endregion
                        }
                        else
                        {
                            #region 485
                            if (!usertokn.cb485OKis)
                                return;

                            int _bwindex = (args.UserToken as TCPClientChannelargsUserToken).bwindex;
                            ChannelMessageTypeOptions tempsendinfo = ChannelMessageTypeOptions.MOXAsend;
                            if (_bwindex == 1)
                                tempsendinfo = ChannelMessageTypeOptions.MOXAreceiveCB1;
                            else if (_bwindex == 2)
                                tempsendinfo = ChannelMessageTypeOptions.MOXAreceiveCB2;
                            else if (_bwindex == 3)
                                tempsendinfo = ChannelMessageTypeOptions.MOXAreceiveCB3;
                            else if (_bwindex == 4)
                                tempsendinfo = ChannelMessageTypeOptions.MOXAreceiveCB4;
                            else if (_bwindex == 5)
                                tempsendinfo = ChannelMessageTypeOptions.MOXAreceiveCB5;
                            else if (_bwindex == 6)
                                tempsendinfo = ChannelMessageTypeOptions.MOXAreceiveCB6;
                            else if (_bwindex == 7)
                                tempsendinfo = ChannelMessageTypeOptions.MOXAreceiveCB7;
                            else if (_bwindex == 8)
                                tempsendinfo = ChannelMessageTypeOptions.MOXAreceiveCB8;
                            this.OnMessage(tempsendinfo, "[" + args.RemoteEndPoint.ToString() + "]" + AC.Base.Function.ByteArrayToHexString(_temp));
                            usertokn.ADDTempBuffer485(_temp);

                            byte[] _485temp = usertokn.get485Realbuffer();
                            _temp = _485temp;
                            while (_485temp != null)
                            {
                                _485temp = usertokn.get485Realbuffer();
                                if (_485temp == null)
                                    break;
                                _temp = _485temp;
                            }

                            if (_temp != null)
                            {
                                _485temp = _temp;
                                EndPoint _485ep = null;
                                if (_485temp.Length >= 25)
                                {

                                    if ((_485temp[0] == 0x68) && (_485temp[14] == 0x05) && (_485temp[BitConverter.ToInt16(_485temp, 1) + 1] == 0x16))
                                    {
                                        string baddress = _485temp[10].ToString("X2") + _485temp[9].ToString("X2") + _485temp[8].ToString("X2") + _485temp[7].ToString("X2") + _485temp[6].ToString("X2") + _485temp[5].ToString("X2");
                                        up485epslist.TryRemove((_bwindex.ToString() + baddress), out _485ep);
                                        up485epslist.TryAdd((_bwindex.ToString() + baddress), args.RemoteEndPoint);

                                        int _linkkind = 0;
                                        if (usertokn.PortType == TCPClientChannelTypeOptions.LinkJ1Z3)
                                            _linkkind = 0;
                                        else if (usertokn.PortType == TCPClientChannelTypeOptions.LinkZ1C2)
                                            _linkkind = 2;
                                        else if (usertokn.PortType == TCPClientChannelTypeOptions.LinkJ2C1)
                                            _linkkind = 1;
                                        UdpSend(string.Format("cmd=1002,data={0};{1};{2}", _bwindex, AC.Base.Function.ByteArrayToHex(_485temp), _linkkind), null);
                                    }
                                    else if ((_485temp[0] == 0x68) && (_485temp[14] == 0x10) && (_485temp[17] == 0x05) && (_485temp[BitConverter.ToInt16(_485temp, 1) + 1] == 0x16))//698虚拟表
                                    {
                                        string baddress = _485temp[10].ToString("X2") + _485temp[9].ToString("X2") + _485temp[8].ToString("X2") + _485temp[7].ToString("X2") + _485temp[6].ToString("X2") + _485temp[5].ToString("X2");
                                        up485epslist.TryRemove((_bwindex.ToString() + baddress), out _485ep);
                                        up485epslist.TryAdd((_bwindex.ToString() + baddress), args.RemoteEndPoint);

                                        int _linkkind = 0;
                                        if (usertokn.PortType == TCPClientChannelTypeOptions.LinkJ1Z3)
                                            _linkkind = 0;
                                        else if (usertokn.PortType == TCPClientChannelTypeOptions.LinkZ1C2)
                                            _linkkind = 2;
                                        else if (usertokn.PortType == TCPClientChannelTypeOptions.LinkJ2C1)
                                            _linkkind = 1;
                                        UdpSend(string.Format("cmd=1002,data={0};{1};{2}", _bwindex, AC.Base.Function.ByteArrayToHex(_485temp), _linkkind), null);
                                    }

                                }
                                else if (_485temp[0] == 0xFE && _485temp[4] == 0x68 && _485temp[11] == 0x68)
                                {
                                    string baddress = _485temp[10].ToString("X2") + _485temp[9].ToString("X2") + _485temp[8].ToString("X2") + _485temp[7].ToString("X2") + _485temp[6].ToString("X2") + _485temp[5].ToString("X2");
                                    up485epslist.TryRemove((_bwindex.ToString() + baddress), out _485ep);
                                    up485epslist.TryAdd((_bwindex.ToString() + baddress), args.RemoteEndPoint);

                                    int _linkkind = 0;
                                    if (usertokn.PortType == TCPClientChannelTypeOptions.LinkJ1Z3)
                                        _linkkind = 0;
                                    else if (usertokn.PortType == TCPClientChannelTypeOptions.LinkZ1C2)
                                        _linkkind = 2;
                                    else if (usertokn.PortType == TCPClientChannelTypeOptions.LinkJ2C1)
                                        _linkkind = 1;
                                    UdpSend(string.Format("cmd=1002,data={0};{1};{2}", _bwindex, AC.Base.Function.ByteArrayToHex(_485temp), _linkkind), null);
                                }

                                else
                                {
                                    byte _tempbyte = _485temp[4];
                                    int AFlen = (_tempbyte & 0x07) + 1;
                                    int AFtype = (_tempbyte & 0xC0);
                                    if (AFtype == 0 || AFtype == 1)
                                    {
                                        string baddress = "";
                                        for (int i = 0; i < 2 + AFlen; i++)
                                        {
                                            baddress = baddress + _485temp[4 + i].ToString("X2");
                                        }
                                        up485epslist.TryRemove((_bwindex.ToString() + baddress), out _485ep);
                                        up485epslist.TryAdd(("" + _bwindex + baddress), args.RemoteEndPoint);
                                        UdpSend(string.Format("cmd=1002,data={0};{1}", _bwindex, AC.Base.Function.ByteArrayToHex(_485temp)), null);
                                    }
                                    else
                                    {
                                        this.OnMessage(ChannelMessageTypeOptions.MOXAError, "AFtype = " + AFtype);
                                    }
                                }



                            }
                            #endregion
                        }
                    }
                    else
                    {
                        this.SocClientAsyncFinish(args);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnMessage(ChannelMessageTypeOptions.MOXAError, ex.ToString());
            }
            finally
            {
                if (args != null)
                {
                    if (args.AcceptSocket.ReceiveAsync(args) == false)
                    {
                        this.SocClientBackBeginReceive(args);
                    }
                }
            }
        }

        private void SocClientAsyncFinish(SocketAsyncEventArgs args)
        {
            TCPClientChannelargsUserToken usertokn = args.UserToken as TCPClientChannelargsUserToken;
            try
            {
                args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);

                SocketAsyncEventArgs _args = null;
                this.m_Clients.TryRemove(args.AcceptSocket.RemoteEndPoint, out _args);

                try
                {
                    if (args.AcceptSocket != null)
                    {
                        args.AcceptSocket.Close();
                        args.AcceptSocket = null;
                    }
                }
                catch (Exception ex)
                { }
                try
                {
                    if (args.ConnectSocket != null)
                    {
                        args.ConnectSocket.Close();
                    }
                }
                catch (Exception ex)
                { }
                args.Dispose();
                args = null;

                if (usertokn != null)
                {
                    usertokn.cb485dataslistClear();
                    usertokn.stop485cb();
                }

                if (usertokn != null)
                    OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("{0}[{1}]端口关闭成功！", usertokn.PortName, usertokn.PortIDName));
                else
                    OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("[{0}]端口关闭成功！", args.RemoteEndPoint.ToString()));
            }
            catch (Exception ex)
            {
                if (usertokn != null)
                    OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口关闭失败！{2}", usertokn.PortName, usertokn.PortIDName, ex.ToString()));
                else
                    OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("[{0}]端口关闭成功！", args.RemoteEndPoint.ToString()));
            }
        }

        private void SocClientAsyncRefresh(SocketAsyncEventArgs args)
        {
            TCPClientChannelargsUserToken usertokn = args.UserToken as TCPClientChannelargsUserToken;
            if (usertokn != null)
            {
                TCPClientStop(usertokn);
                TCPClientStart(usertokn);
            }
        }

        private string socclientgfetstate()
        {
            string str = "";

            return str;
        }
        #endregion
        #region << tcp client 发送数据 >>

        object lock1 = new object();
        public bool SendToMoxa(TCPClientChannelargsUserToken usertokn, ExecData execdata, int waittime)
        {
            SocketAsyncEventArgs args = null;
            SocketAsyncEventArgs tempargs = null;
            this.m_Clients.TryGetValue(usertokn.ep, out args);
            if (args == null)
            {
                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                args.RemoteEndPoint = usertokn.ep;
                try
                {
                    soc.ConnectAsync(args);
                }
                catch
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                    args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                    args.Dispose();
                    soc.Dispose();
                    return false;
                }

                args.UserToken = usertokn;//绑定用户数据

                Thread.Sleep(50);
                byte[] bytBuffer = new byte[BUFFERSIZE];
                lock (args)
                {
                    args.SetBuffer(bytBuffer, 0, bytBuffer.Length);
                }
                Thread.Sleep(50);

                args.AcceptSocket = soc;
                this.m_Clients.TryAdd(usertokn.ep, args);

                if (soc.ReceiveAsync(args) == false)
                {
                    //this.SocClientBackBeginReceive(args);
                    OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                    this.m_Clients.TryRemove(usertokn.ep, out tempargs);
                    args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                    args.Dispose();
                    soc.Dispose();
                    return false;
                }
                else
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("{0}[{1}]端口打开成功！", usertokn.PortName, usertokn.PortIDName));
                    OnSaveTCLClientOpens(args, true);
                }
            }

            if (args != null)
            {
                if ((usertokn.PortType == TCPClientChannelTypeOptions.Link2321) || (usertokn.PortType == TCPClientChannelTypeOptions.Link2322))
                {
                    string seq = "SEQ=" + (execdata.senddatas[13] & 15) + string.Format("[{0}]", usertokn.ep.ToString());//帧序列
                    execdata.key = seq;
                }
                else if ((usertokn.PortType == TCPClientChannelTypeOptions.LinkBZ) || (usertokn.PortType == TCPClientChannelTypeOptions.LinkDJ) || (usertokn.PortType == TCPClientChannelTypeOptions.LinkPM))
                {
                    byte address = execdata.senddatas[4];
                    byte cmd = execdata.senddatas[5];
                    string key = address.ToString("X2") + cmd.ToString("X2");
                    execdata.key = key;
                }
                else
                {
                    execdata.key = "key";
                }

                usertokn.ed = execdata;
                execdata.usertokn = usertokn;

                args.AcceptSocket.Send(execdata.senddatas);
                this.OnMessage(ChannelMessageTypeOptions.MOXAsend, AC.Base.Function.ByteArrayToHexString(execdata.senddatas));

                int i = 0;
                while (i < waittime)
                {
                    Thread.Sleep(100);
                    if (execdata.backdatas != null)
                        return true;
                    i++;
                }
            }
            return false;
        }

        public bool SendToMoxa232485(TCPClientChannelargsUserToken usertokn, ExecData execdata, int waittime)
        {
            SocketAsyncEventArgs args = null;
            SocketAsyncEventArgs tempargs = null;
            this.m_Clients.TryGetValue(usertokn.ep, out args);
            if (args == null)
            {
                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                args.RemoteEndPoint = usertokn.ep;
                try
                {
                    soc.ConnectAsync(args);
                }
                catch
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                    args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                    args.Dispose();
                    soc.Dispose();
                    return false;
                }

                args.UserToken = usertokn;//绑定用户数据

                Thread.Sleep(50);
                byte[] bytBuffer = new byte[BUFFERSIZE];
                args.SetBuffer(bytBuffer, 0, bytBuffer.Length);
                Thread.Sleep(50);

                args.AcceptSocket = soc;
                this.m_Clients.TryAdd(usertokn.ep, args);

                if (soc.ReceiveAsync(args) == false)
                {
                    //this.SocClientBackBeginReceive(args);
                    OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                    this.m_Clients.TryRemove(usertokn.ep, out tempargs);
                    args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                    args.Dispose();
                    soc.Dispose();
                    return false;
                }
                else
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("{0}[{1}]端口打开成功！", usertokn.PortName, usertokn.PortIDName));
                    OnSaveTCLClientOpens(args, true);
                }
            }

            if (args != null)
            {
                usertokn.ed = execdata;
                execdata.usertokn = usertokn;

                args.AcceptSocket.Send(execdata.senddatas);
                this.OnMessage(ChannelMessageTypeOptions.MOXAsend, AC.Base.Function.ByteArrayToHexString(execdata.senddatas));

                int i = 0;
                while (i < waittime)
                {
                    Thread.Sleep(100);
                    if (execdata.backdatas != null)
                        return true;
                    i++;
                }
            }
            return false;
        }

        public bool ONLYSendToMoxa(TCPClientChannelargsUserToken usertokn, byte[] senddatas)
        {
            SocketAsyncEventArgs args = null;
            SocketAsyncEventArgs tempargs = null;
            this.m_Clients.TryGetValue(usertokn.ep, out args);
            if (args == null)
            {
                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                args.RemoteEndPoint = usertokn.ep;
                try
                {
                    soc.ConnectAsync(args);
                }
                catch
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                    args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                    args.Dispose();
                    soc.Dispose();
                    return false;
                }

                args.UserToken = usertokn;//绑定用户数据

                Thread.Sleep(50);
                byte[] bytBuffer = new byte[BUFFERSIZE];
                args.SetBuffer(bytBuffer, 0, bytBuffer.Length);
                Thread.Sleep(50);

                args.AcceptSocket = soc;
                this.m_Clients.TryAdd(usertokn.ep, args);

                if (soc.ReceiveAsync(args) == false)
                {
                    //this.SocClientBackBeginReceive(args);
                    OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                    this.m_Clients.TryRemove(usertokn.ep, out tempargs);
                    args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                    args.Dispose();
                    soc.Dispose();
                    return false;
                }
                else
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("{0}[{1}]端口打开成功！", usertokn.PortName, usertokn.PortIDName));
                    OnSaveTCLClientOpens(args, true);
                }
            }

            if (args != null)
            {
                usertokn.ed = null;

                args.AcceptSocket.Send(senddatas);
                this.OnMessage(ChannelMessageTypeOptions.MOXAsend, AC.Base.Function.ByteArrayToHexString(senddatas));
            }
            return false;
        }

        public bool CBSendToMoxa(TCPClientChannelargsUserToken usertokn, byte[] senddatas)
        {
            SocketAsyncEventArgs args = null;
            SocketAsyncEventArgs tempargs = null;
            this.m_Clients.TryGetValue(usertokn.ep, out args);
            if (args == null)
            {
                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                args.RemoteEndPoint = usertokn.ep;
                try
                {
                    soc.ConnectAsync(args);
                }
                catch
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                    args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                    args.Dispose();
                    soc.Dispose();
                    return false;
                }

                args.UserToken = usertokn;//绑定用户数据

                Thread.Sleep(50);
                byte[] bytBuffer = new byte[BUFFERSIZE];
                args.SetBuffer(bytBuffer, 0, bytBuffer.Length);
                Thread.Sleep(50);

                args.AcceptSocket = soc;
                this.m_Clients.TryAdd(usertokn.ep, args);

                if (soc.ReceiveAsync(args) == false)
                {
                    //this.SocClientBackBeginReceive(args);
                    OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                    this.m_Clients.TryRemove(usertokn.ep, out tempargs);
                    args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                    args.Dispose();
                    soc.Dispose();
                    return false;
                }
                else
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("{0}[{1}]端口打开成功！", usertokn.PortName, usertokn.PortIDName));
                    OnSaveTCLClientOpens(args, true);
                }
            }

            if (args != null)
            {
                usertokn.ed = null;
                args.AcceptSocket.Send(senddatas);
                ChannelMessageTypeOptions tempsendinfo = ChannelMessageTypeOptions.MOXAsend;
                if (usertokn.bwindex == 1)
                    tempsendinfo = ChannelMessageTypeOptions.MOXAsendCB1;
                else if (usertokn.bwindex == 2)
                    tempsendinfo = ChannelMessageTypeOptions.MOXAsendCB2;
                else if (usertokn.bwindex == 3)
                    tempsendinfo = ChannelMessageTypeOptions.MOXAsendCB3;
                else if (usertokn.bwindex == 4)
                    tempsendinfo = ChannelMessageTypeOptions.MOXAsendCB4;
                else if (usertokn.bwindex == 5)
                    tempsendinfo = ChannelMessageTypeOptions.MOXAsendCB5;
                else if (usertokn.bwindex == 6)
                    tempsendinfo = ChannelMessageTypeOptions.MOXAsendCB6;
                else if (usertokn.bwindex == 7)
                    tempsendinfo = ChannelMessageTypeOptions.MOXAsendCB7;
                else if (usertokn.bwindex == 8)
                    tempsendinfo = ChannelMessageTypeOptions.MOXAsendCB8;

                this.OnMessage(tempsendinfo, "[" + args.RemoteEndPoint.ToString() + "]" + AC.Base.Function.ByteArrayToHexString(senddatas));
            }
            return false;
        }
        #endregion
        #region << tcp client 打开关闭连接 >>
        public bool TCPClientStart(TCPClientChannelargsUserToken usertokn)
        {
            lock (tcpclientlock)
            {
                SocketAsyncEventArgs args = null;
                SocketAsyncEventArgs tempargs = null;
                Socket soc = null;
                try
                {
                    usertokn.setcb485OK(true);

                    this.m_Clients.TryGetValue(usertokn.ep, out args);
                    if (args == null)
                    {
                        soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        args = new SocketAsyncEventArgs();
                        args.Completed += new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                        args.RemoteEndPoint = usertokn.ep;
                        try
                        {
                            soc.ConnectAsync(args);
                        }
                        catch
                        {
                            OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                            args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                            args.Dispose();
                            soc.Dispose();
                            return false;
                        }

                        args.UserToken = usertokn;//绑定用户数据

                        Thread.Sleep(50);
                        byte[] bytBuffer = new byte[BUFFERSIZE];
                        args.SetBuffer(bytBuffer, 0, bytBuffer.Length);
                        Thread.Sleep(50);

                        args.AcceptSocket = soc;
                        this.m_Clients.TryAdd(usertokn.ep, args);

                        if (soc.ReceiveAsync(args) == false)
                        {
                            //this.SocClientBackBeginReceive(args);
                            OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                            this.m_Clients.TryRemove(usertokn.ep, out tempargs);
                            args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                            args.Dispose();
                            soc.Close();
                            soc.Dispose();
                            return false;
                        }
                        else
                        {
                            OnMessage(ChannelMessageTypeOptions.MOXAInfo, string.Format("{0}[{1}]端口打开成功！", usertokn.PortName, usertokn.PortIDName));
                            OnSaveTCLClientOpens(args, true);
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, string.Format("{0}[{1}]端口打开失败！", usertokn.PortName, usertokn.PortIDName));
                    this.m_Clients.TryRemove(usertokn.ep, out tempargs);
                    args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                    args.Dispose();
                    if (soc != null)
                    {
                        soc.Close();
                        soc.Dispose();
                    }
                }
                return false;
            }
        }
        public bool TCPClientStop(TCPClientChannelargsUserToken usertokn)
        {
            lock (tcpclientlock)
            {
                SocketAsyncEventArgs args = null;
                this.m_Clients.TryRemove(usertokn.ep, out args);
                if (args != null)
                {
                    SocClientAsyncFinish(args);
                    OnSaveTCLClientOpens(args, false);
                }
                return true;
            }
        }

        public void refreshTCPClient()
        {
            lock (tcpclientlock)
            {
                if (this.m_Clients != null)
                {
                    foreach (KeyValuePair<EndPoint, SocketAsyncEventArgs> kvp in this.m_Clients)
                        this.SocClientAsyncRefresh(kvp.Value);
                }
            }
        }

        public SocketAsyncEventArgs GetTCPClientState(TCPClientChannelargsUserToken usertokn)
        {
            lock (tcpclientlock)
            {
                SocketAsyncEventArgs args = null;
                this.m_Clients.TryGetValue(usertokn.ep, out args);
                return args;
            }
        }

        #endregion
        #endregion

        #region << 异常处理 >>
        /// <summary>
        /// 通道异常。该方法由继承通道的类调用。
        /// 当通道因为某种原因（如网络断开、串口不存在）而不能继续正常工作时，由继承的类调用该方法，以便通知通道组进行主备通道切换。
        /// 继承的类调用该方法时应确保已经停止与设备端的连接。
        /// 调用该方法后无需再调用 Stop() 方法。
        internal void DoException(SocketAsyncEventArgs args, Exception ex)
        {
            //argsUserToken usertoken = args.UserToken as argsUserToken;
            //this.OnMessage(ChannelMessageTypeOptions.Error, "[" + Thread.CurrentThread.ManagedThreadId.ToString("X2") + "] " + (args.AcceptSocket != null && args.AcceptSocket.Connected ? args.AcceptSocket.RemoteEndPoint.ToString() : "无地址") + "\tCID:" + usertoken.SocketClientIndex + "\tCloseCount:" + usertoken.CloseCount + "\tAddress:" + usertoken.Address + "\t" + usertoken.GetCollectionName() + "\r\n" + ex);
        }
        /// <summary>
        /// 处理异常。
        /// </summary>
        /// <param name="ex"></param>
        internal void DoException(Exception ex)
        {
            this.OnMessage(ChannelMessageTypeOptions.Error, "[" + Thread.CurrentThread.ManagedThreadId.ToString("X2") + "] " + ex.Message);
        }
        #endregion

        #region TCP 发送
        /// <summary>
        /// tcp发送 376.1命令
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string Send(byte[] _temp)
        {
            SocketAsyncEventArgs args = null;
            string Addr = _temp[8].ToString("X2") + _temp[7].ToString("X2") + (_temp[9] + _temp[10] * 0x100).ToString().PadLeft(5, '0');
            int idx = _temp[13];
            int i = 0;
            if (this.m_Onlines.TryGetValue(Addr, out args))
            {
                args.AcceptSocket.Send(_temp);
                while (i < delay)
                {
                    Thread.Sleep(100);
                    byte[] _d = null;
                    this.m_Result.TryGetValue(idx, out _d);
                    if (_d != null)
                        return AC.Base.Function.ByteArrayToHex(_d);
                }

                this.m_Onlines.TryRemove(Addr, out args);
                OnCheckOnlines(this.m_Onlines);
                return "2";//超时没回
            }
            return "1";//没发现终端
        }
        #region << socket client SEND >>
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="ep">IP地址</param>
        /// <param name="HEX">HEX字符串,2位一个byte,补0,没有空格‘6801000000000068010252C3E916’</param>
        /// <returns></returns>
        public string Send(EndPoint ep, string HEX, bool bReply)
        {
            SocketAsyncEventArgs args = null;
            TCPClientChannelargsUserToken aut = null;
            this.m_Clients.TryGetValue(ep, out args);

            #region << 连接端口 >>
            if (args == null)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try { socket.Connect(ep); }
                catch { return "0"; }
                args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);

                args.AcceptSocket = socket;
                aut = new TCPClientChannelargsUserToken();
                aut.PortType = TCPClientChannelTypeOptions.Link2321;
                args.UserToken = aut;//绑定用户数据
                this.m_Clients.TryAdd(ep, args);

                byte[] bytBuffer = new byte[BUFFERSIZE];
                args.SetBuffer(bytBuffer, 0, bytBuffer.Length);
                if (args.AcceptSocket.ReceiveAsync(args) == false)
                    this.SocClientBackBeginReceive(args);
            }
            else
                aut = args.UserToken as TCPClientChannelargsUserToken;
            #endregion

            byte[] b;
            int j = 0;
            try { b = AC.Base.Function.HexToByteArray(HEX); j = b[13]; }
            catch (Exception) { b = Encoding.ASCII.GetBytes(HEX); }

            aut.reply.Clear();
            args.AcceptSocket.Send(b);

            if (!bReply)
                return "OK";
            else
            {
                int i = 0;
                while (i < delay)
                {
                    if (j == 0)
                    {
                        if (aut.reply.Count > 0)
                            return AC.Base.Function.ByteArrayToHex(aut.reply[0]);
                    }
                    else
                    {
                        foreach (byte[] Message in aut.reply)
                        {
                            if (Message[13] == j)
                                return AC.Base.Function.ByteArrayToHex(Message);
                        }
                    }
                    i++;
                    Thread.Sleep(100);
                }
            }
            return "OUT";
        }

        public string Send(EndPoint ep, string HEX)
        {
            return Send(ep, HEX, false);
        }
        #endregion
        #region << 多功能虚拟表 支持07 97 edited by IXCH >>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32.dll")]
        private extern static int GetPrivateProfileStringA(string segName, string keyName, string sDefault, byte[] buffer, int iLen, string fileName); // ANSI版本

        public Hashtable GetiMetValue()
        {
            Hashtable iMET = new Hashtable();
            byte[] _buffer = new byte[5120];//接收缓存区域
            StringBuilder Value = new StringBuilder(255);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "iMet.ini");
            int i = GetPrivateProfileStringA("M1", null, "", _buffer, _buffer.GetUpperBound(0), path);
            int iCnt = 0; int iPos = 0;
            for (iCnt = 0; iCnt <= i; iCnt++)
            {
                if (_buffer[iCnt] == 0x00)
                {
                    string Key = System.Text.ASCIIEncoding.Default.GetString(_buffer, iPos, iCnt - iPos).Trim().ToUpper();
                    iPos = iCnt + 1;
                    if (Key != "")
                    {
                        GetPrivateProfileString("M1", Key, "", Value, 255, path);
                        if (iMET.ContainsKey(Key))
                            iMET.Remove(Key);
                        iMET.Add(Key, Value.ToString().Trim());
                    }
                }
            }
            return iMET;
        }

        /// <summary>
        /// 初始化虚拟表
        /// </summary>
        /// <param name="ep">虚拟表Moxa ip地址，网络口表</param>
        /// <param name="addr">虚拟表地址，支持多个地址。地址“000000000001”</param>
        public bool OpeniMET(EndPoint ep, string[] addr)
        {
            SocketAsyncEventArgs args = null;
            this.m_Clients.TryGetValue(ep, out args);

            if (args != null)
            {
                if (args.AcceptSocket.Connected)
                    return true;
                else
                    this.SocketAsyncFinish(args);
            }

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try { socket.Connect(ep); }
            catch { return false; }

            args = new SocketAsyncEventArgs();
            args.AcceptSocket = socket;

            TCPClientChannelargsUserToken aut = new TCPClientChannelargsUserToken();
            aut.PortType = TCPClientChannelTypeOptions.iMET;
            aut.iMETAddr = addr;

            Hashtable iMET = GetiMetValue();
            aut.iMETValue = new Hashtable();
            aut.iMETValue.Add("AAAAAAAAAAAA", iMET);

            if (addr != null && addr.Length > 0)
            {
                for (int i = 0; i < addr.Length; i++)
                {
                    Hashtable hb = iMET;
                    aut.iMETValue.Add(addr[i], hb);
                }
            }
            args.UserToken = aut;
            args.Completed += new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
            this.m_Clients.TryAdd(ep, args);

            byte[] bytBuffer = new byte[BUFFERSIZE];
            args.SetBuffer(bytBuffer, 0, bytBuffer.Length);
            if (args.AcceptSocket.ReceiveAsync(args) == false)
                this.SocClientBackBeginReceive(args);

            return true;
        }

        /// <summary>
        /// 不判断表地址的虚拟表，可回复任何地址。
        /// </summary>
        /// <param name="ep">网络端口</param>
        public bool OpeniMET(EndPoint ep)
        {
            return OpeniMET(ep, null);
        }

        private byte[] Reply(byte[] Message, Hashtable METvalue)
        {
            if (METvalue == null)
                METvalue = GetiMetValue();

            byte[] rep = null;
            if ((Message[0] == 0x68 && Message[Message.Length - 1] == 0x16))
            {
                string key = string.Empty;
                byte cs;
                if (Message[9] == 4)//07规约
                {
                    key = ((byte)(Message[13] - 0x33)).ToString("X2") +
                        ((byte)(Message[12] - 0x33)).ToString("X2") +
                        ((byte)(Message[11] - 0x33)).ToString("X2") +
                        ((byte)(Message[10] - 0x33)).ToString("X2");

                    if (METvalue.ContainsKey(key))
                    {
                        string[] s = METvalue[key].ToString().Split(' ');
                        string[] _temp = s[1].Split(',');
                        int length = int.Parse(s[0]);
                        rep = new byte[16 + length];
                        rep[0] = 0x68;
                        rep[1] = Message[1];
                        rep[2] = Message[2];
                        rep[3] = Message[3];
                        rep[4] = Message[4];
                        rep[5] = Message[5];
                        rep[6] = Message[6];
                        rep[7] = 0x68;
                        rep[8] = 0x91;
                        rep[9] = (byte)(length + 4);
                        rep[10] = Message[10];
                        rep[11] = Message[11];
                        rep[12] = Message[12];
                        rep[13] = Message[13];

                        int _len = 0;
                        for (int k = 0; k < _temp.Length; k++)
                        {
                            int l = _temp[k].Length / 2; ;
                            for (int ii = 0; ii < l; ii++)
                                rep[13 + _len + l - ii] = (byte)((Convert.ToByte(_temp[k].Substring(ii * 2, 2), 16) + 0x33) % 0x100);
                            _len += l;
                        }

                        cs = 0;
                        for (int jj = 0; jj < 14 + length; jj++)
                            cs += rep[jj];
                        rep[14 + length] = (byte)(cs % 256);
                        rep[15 + length] = 0x16;
                    }
                    else
                    {
                        rep = new byte[13];
                        rep[0] = 0x68;
                        rep[1] = Message[1];
                        rep[2] = Message[2];
                        rep[3] = Message[3];
                        rep[4] = Message[4];
                        rep[5] = Message[5];
                        rep[6] = Message[6];

                        rep[7] = 0x68;
                        rep[8] = 0xD1;
                        rep[9] = 1;
                        rep[10] = 0x35;
                        cs = 0;
                        for (int jj = 0; jj < 11; jj++)
                            cs += rep[jj];
                        rep[11] = (byte)(cs % 256);
                        rep[12] = 0x16;
                    }
                }
                else if (Message[9] == 2)//97规约
                {
                    key = ((byte)(Message[11] - 0x33)).ToString("X2") + ((byte)(Message[10] - 0x33)).ToString("X2");
                    if (METvalue.ContainsKey(key))
                    {
                        string[] s = METvalue[key].ToString().Split(' ');
                        string[] _temp = s[1].Split(',');
                        byte length = byte.Parse(s[0]);
                        rep = new byte[14 + length];
                        rep[0] = 0x68;
                        rep[1] = Message[1];
                        rep[2] = Message[2];
                        rep[3] = Message[3];
                        rep[4] = Message[4];
                        rep[5] = Message[5];
                        rep[6] = Message[6];
                        rep[7] = 0x68;
                        rep[8] = 0x81;
                        rep[9] = length;
                        rep[10] = Message[10];
                        rep[11] = Message[11];

                        int _len = 0;
                        for (int k = 0; k < _temp.Length; k++)
                        {
                            int l = _temp[k].Length / 2; ;
                            for (int ii = 0; ii < l; ii++)
                                rep[11 + _len + l - ii] = (byte)(Convert.ToByte(_temp[k].Substring(ii * 2, 2), 16) + 0x33);
                            _len += l;
                        }
                        cs = 0;
                        for (int jj = 0; jj < 12 + length; jj++)
                        {
                            cs += rep[jj];
                        }
                        rep[12 + length] = (byte)(cs % 256);
                        rep[13 + length] = 0x16;
                    }
                    else
                    {
                        rep = new byte[13];
                        rep[0] = 0x68;
                        rep[1] = Message[1];
                        rep[2] = Message[2];
                        rep[3] = Message[3];
                        rep[4] = Message[4];
                        rep[5] = Message[5];
                        rep[6] = Message[6];
                        rep[7] = 0x68;
                        rep[8] = 0xc1;
                        rep[9] = 1;
                        rep[10] = 0x35;
                        cs = 0;
                        for (int jj = 0; jj < 11; jj++)
                            cs += rep[jj];
                        rep[11] = (byte)(cs % 256);
                        rep[12] = 0x16;
                    }
                }
            }
            return rep;
        }
        #endregion
        #endregion
    }

    #region << ChannelMessageTypeOptions  通道服务消息类型 >>
    /// <summary>
    /// 通道服务消息类型
    /// </summary>
    public enum ChannelMessageTypeOptions
    {
        /// <summary>
        /// 一般的通知消息。
        /// </summary>
        Information,

        /// <summary>
        /// 重要消息。
        /// </summary>
        Importance,

        /// <summary>
        /// 警告消息。
        /// </summary>
        Warning,

        /// <summary>
        /// 错误消息。
        /// </summary>
        Error,
        /// <summary>
        /// UDP接收报文
        /// </summary>
        UDPreceive,
        /// <summary>
        /// UDP发送报文
        /// </summary>
        UDPsend,

        /// <summary>
        /// UDP接收抄表报文 
        /// </summary>
        UDPreceiveCB1,
        UDPreceiveCB2,
        UDPreceiveCB3,
        UDPreceiveCB4,
        UDPreceiveCB5,
        UDPreceiveCB6,
        UDPreceiveCB7,
        UDPreceiveCB8,

        /// <summary>
        /// UDP转发抄表报文 
        /// </summary>
        UDPsendCB1,
        UDPsendCB2,
        UDPsendCB3,
        UDPsendCB4,
        UDPsendCB5,
        UDPsendCB6,
        UDPsendCB7,
        UDPsendCB8,

        /// <summary>
        /// UDP通信错误
        /// </summary>
        UDPError,
        /// <summary>
        /// UDP相关信息
        /// </summary>
        UDPInfo,

        // <summary>
        /// MOXA接收报文
        /// </summary>
        MOXAreceive,
        // <summary>
        /// MOXA抄表接收报文
        /// </summary>
        MOXAreceiveCB1,
        MOXAreceiveCB2,
        MOXAreceiveCB3,
        MOXAreceiveCB4,
        MOXAreceiveCB5,
        MOXAreceiveCB6,
        MOXAreceiveCB7,
        MOXAreceiveCB8,

        /// <summary>
        /// MOXA发送报文
        /// </summary>
        MOXAsend,
        /// <summary>
        /// MOXA抄表发送报文
        /// </summary>
        MOXAsendCB1,
        MOXAsendCB2,
        MOXAsendCB3,
        MOXAsendCB4,
        MOXAsendCB5,
        MOXAsendCB6,
        MOXAsendCB7,
        MOXAsendCB8,

        /// <summary>
        /// MOXA通信错误
        /// </summary>
        MOXAError,
        /// <summary>
        /// UDP相关信息
        /// </summary>
        MOXAInfo,

    }

    public static class ChannelMessageTypeOptionsExtensions
    {
        public static string GetDescription(this ChannelMessageTypeOptions messageType)
        {
            switch (messageType)
            {
                case ChannelMessageTypeOptions.Information:
                    return "通知消息";

                case ChannelMessageTypeOptions.Importance:
                    return "重要消息";

                case ChannelMessageTypeOptions.Warning:
                    return "警告消息";

                case ChannelMessageTypeOptions.Error:
                    return "错误消息";

                case ChannelMessageTypeOptions.UDPreceive:
                    return "UDP接收报文";

                case ChannelMessageTypeOptions.UDPsend:
                    return "UDP发送报文";

                case ChannelMessageTypeOptions.UDPreceiveCB1:
                    return "表位1UDP接收抄表报文";
                case ChannelMessageTypeOptions.UDPreceiveCB2:
                    return "表位2UDP接收抄表报文";
                case ChannelMessageTypeOptions.UDPreceiveCB3:
                    return "表位3UDP接收抄表报文";
                case ChannelMessageTypeOptions.UDPreceiveCB4:
                    return "表位4UDP接收抄表报文";
                case ChannelMessageTypeOptions.UDPreceiveCB5:
                    return "表位5UDP接收抄表报文";
                case ChannelMessageTypeOptions.UDPreceiveCB6:
                    return "表位6UDP接收抄表报文";
                case ChannelMessageTypeOptions.UDPreceiveCB7:
                    return "表位7UDP接收抄表报文";
                case ChannelMessageTypeOptions.UDPreceiveCB8:
                    return "表位8UDP接收抄表报文";

                case ChannelMessageTypeOptions.UDPsendCB1:
                    return "表位1UDP发送抄表报文";
                case ChannelMessageTypeOptions.UDPsendCB2:
                    return "表位2UDP发送抄表报文";
                case ChannelMessageTypeOptions.UDPsendCB3:
                    return "表位3UDP发送抄表报文";
                case ChannelMessageTypeOptions.UDPsendCB4:
                    return "表位4UDP发送抄表报文";
                case ChannelMessageTypeOptions.UDPsendCB5:
                    return "表位5UDP发送抄表报文";
                case ChannelMessageTypeOptions.UDPsendCB6:
                    return "表位6UDP发送抄表报文";
                case ChannelMessageTypeOptions.UDPsendCB7:
                    return "表位7UDP发送抄表报文";
                case ChannelMessageTypeOptions.UDPsendCB8:
                    return "表位8UDP发送抄表报文";

                case ChannelMessageTypeOptions.UDPError:
                    return "UDP通信错误";

                case ChannelMessageTypeOptions.UDPInfo:
                    return "UDP通道信息";

                case ChannelMessageTypeOptions.MOXAreceive:
                    return "MOXA接收报文";

                case ChannelMessageTypeOptions.MOXAsend:
                    return "MOXA发送报文";

                case ChannelMessageTypeOptions.MOXAreceiveCB1:
                    return "表位1MOXA接收抄表报文";
                case ChannelMessageTypeOptions.MOXAreceiveCB2:
                    return "表位2MOXA接收抄表报文";
                case ChannelMessageTypeOptions.MOXAreceiveCB3:
                    return "表位3MOXA接收抄表报文";
                case ChannelMessageTypeOptions.MOXAreceiveCB4:
                    return "表位4MOXA接收抄表报文";
                case ChannelMessageTypeOptions.MOXAreceiveCB5:
                    return "表位5MOXA接收抄表报文";
                case ChannelMessageTypeOptions.MOXAreceiveCB6:
                    return "表位6MOXA接收抄表报文";
                case ChannelMessageTypeOptions.MOXAreceiveCB7:
                    return "表位7MOXA接收抄表报文";
                case ChannelMessageTypeOptions.MOXAreceiveCB8:
                    return "表位8MOXA接收抄表报文";

                case ChannelMessageTypeOptions.MOXAsendCB1:
                    return "表位1MOXA发送抄表报文";
                case ChannelMessageTypeOptions.MOXAsendCB2:
                    return "表位2MOXA发送抄表报文";
                case ChannelMessageTypeOptions.MOXAsendCB3:
                    return "表位3MOXA发送抄表报文";
                case ChannelMessageTypeOptions.MOXAsendCB4:
                    return "表位4MOXA发送抄表报文";
                case ChannelMessageTypeOptions.MOXAsendCB5:
                    return "表位5MOXA发送抄表报文";
                case ChannelMessageTypeOptions.MOXAsendCB6:
                    return "表位6MOXA发送抄表报文";
                case ChannelMessageTypeOptions.MOXAsendCB7:
                    return "表位7MOXA发送抄表报文";
                case ChannelMessageTypeOptions.MOXAsendCB8:
                    return "表位8MOXA发送抄表报文";

                case ChannelMessageTypeOptions.MOXAError:
                    return "MOXAP通信错误";

                case ChannelMessageTypeOptions.MOXAInfo:
                    return "MOXAP通道信息";

                default:
                    return "未知项";
                //throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }

    #endregion

    #region <<  ChannelStateOptions 通道服务当前运行状态选项 >>
    /// <summary>
    /// 通道服务当前运行状态选项。
    /// </summary>
    public enum ChannelStateOptions
    {
        /// <summary>
        /// 空闲。
        /// </summary>
        Free,
        /// <summary>
        /// 正常运行。
        /// </summary>
        Running,
        /// <summary>
        /// 停止运行。
        /// </summary>
        Stopped,
        /// <summary>
        /// 故障停止。
        /// </summary>
        Exception,
    }

    /// <summary>
    /// 通道服务当前运行状态方法扩展。
    /// </summary>
    public static class ChannelStateOptionsExtensions
    {
        /// <summary>
        /// 获取通道服务运行状态的文字说明。
        /// </summary>
        /// <param name="channelServiceState">通道服务状态</param>
        /// <returns>通道服务运行状态的文字说明</returns>
        public static string GetDescription(this ChannelStateOptions State)
        {
            switch (State)
            {
                case ChannelStateOptions.Free:
                    return "空闲";
                case ChannelStateOptions.Running:
                    return "运行";
                case ChannelStateOptions.Stopped:
                    return "停止";
                case ChannelStateOptions.Exception:
                    return "故障";
                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
    #endregion

    #region << client 主动连接通道类型 >>
    public enum TCPClientChannelTypeOptions
    {
        Unknow = 0,
        LinkDJ = 1,
        LinkBZ = 2,
        LinkPM = 3,
        LinkJ1Z3 = 4,
        LinkZ1C2 = 5,
        LinkJ2C1 = 6,
        Link2321 = 7,
        Link2322 = 8,
        iMET = 9,
        iDEV = 10
    }
    #endregion

    #region << argsUserToken 用户数据 >>
    /// <summary>
    /// socket 用户数据类
    /// </summary>
    public class TCPClientChannelargsUserToken
    {
        Channel channel = null;

        /// <summary>
        /// 表位
        /// </summary>
        public int bwindex = 0;
        /// <summary>
        /// 虚拟表值
        /// </summary>
        public Hashtable iMETValue = null;
        /// <summary>
        /// 虚拟表地址
        /// </summary>
        public string[] iMETAddr = null;
        /// <summary>
        /// 回复缓存
        /// </summary>
        public List<byte[]> reply = new List<byte[]>();
        //public TCPClientChannelTypeOptions ChannelType = TCPClientChannelTypeOptions.iDEV;
        /// <summary>
        /// 获取最后收发数据的时间。
        /// </summary>
        public DateTime LastReceivedTime = DateTime.MinValue;
        /// <summary>
        /// 接收数据缓存，用来拼帧操作
        /// </summary>

        public byte[] cache = new byte[2048];

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 管理端口
        /// </summary>
        public int mgrPort { get; set; }

        /// <summary>
        /// 端口名称
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// 端口ID名称
        /// </summary>
        public string PortIDName { get; set; }

        /// <summary>
        /// 端口类型
        /// </summary>
        public TCPClientChannelTypeOptions PortType { get; set; }

        private List<byte> Temp232Buffer = new List<byte>();
        private List<byte> Temp485Buffer = new List<byte>();
        public void ADDTempBuffer232(byte[] temps)
        {
            for (int i = 0; i < temps.Length; i++)
            {
                Temp232Buffer.Add(temps[i]);
            }
        }
        /// <summary>
        /// 判断处理回复帧（如有主动上报需要另外处理）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="epstr"></param>
        /// <returns></returns>
        public byte[] get232Realbuffer(string key, string epstr)
        {
            byte[] _buffer = null;
            int _index = 0;
            int _len = 0;
            bool f = false;
            string seq = "";
            if (Temp232Buffer.Count > 8)
            {
                for (int i = 0; i < Temp232Buffer.Count; i++)
                {
                    _index = i;
                    if (Temp232Buffer[i] == 0x68)
                    {
                        if ((i + 5) < Temp232Buffer.Count)
                        {
                            if ((Temp232Buffer[i + 1] == Temp232Buffer[i + 3]) || (Temp232Buffer[i + 2] == Temp232Buffer[i + 4]) || (Temp232Buffer[i + 5] == 0x68))
                            {
                                _len = (Temp232Buffer[i + 2] * 0x100 + Temp232Buffer[i + 1] - 2) / 4;
                                if ((i + 5 + _len + 2) < Temp232Buffer.Count)
                                {
                                    if (Temp232Buffer[i + 5 + _len + 2] == 0x16)
                                    {
                                        _len = _len + 8;
                                        if (key == "")
                                        {
                                            if ((Temp232Buffer[_index + 6] & 64) == 64)//主动上报的数据
                                            {
                                                f = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            seq = "SEQ=" + (Temp232Buffer[_index + 13] & 15) + string.Format("[{0}]", epstr);
                                            if (key.Equals(seq))
                                            {
                                                f = true;
                                                break;
                                            }
                                        }
                                        i = i + 5 + _len + 2;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                if (f)
                {
                    _buffer = new byte[_len];
                    try
                    {
                        for (int i = _index; i < _len + _index; i++)
                        {
                            _buffer[i - _index] = Temp232Buffer[i];
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    Temp232Buffer.RemoveRange(0, _index + _len);
                }
                else
                {
                    if (_index > 0)
                    {
                        Temp232Buffer.RemoveRange(0, _index);
                    }
                }
            }
            return _buffer;
        }
        public void ADDTempBuffer485(byte[] temps)
        {
            int index = 0;
            Temp485Buffer = new List<byte>();
            for (int k = 0; k < temps.Length; k++)
            {
                if (temps[k] == 0x68)
                {
                    index = k;
                    break;
                }
            }
            for (int i = index; i < temps.Length; i++)
            {
                Temp485Buffer.Add(temps[i]);
            }
        }
        public byte[] get485Realbuffer()
        {
            if (Temp485Buffer.Count >= 16)//判断抄表报文是07规约还是698规约  sjt+ 2017-4-23
            {
                if ((Temp485Buffer[0] == 0x68) && (Temp485Buffer[7] == 0x68))
                    this.gy485type = 0;
                if (Temp485Buffer.Count >= 25)
                    if (((Temp485Buffer[0] == 0x68) && (Temp485Buffer[14] == 0x05) && (Temp485Buffer[(Temp485Buffer[1] + Temp485Buffer[2] * 0x100) + 1] == 0x16)) || ((Temp485Buffer[0] == 0x68) && (Temp485Buffer[14] == 0x10) && (Temp485Buffer[17] == 0x05) && (Temp485Buffer[(Temp485Buffer[1] + Temp485Buffer[2] * 0x100) + 1] == 0x16)))//698虚拟表
                        this.gy485type = 1;
            }
            byte[] _buffer = null;
            try
            {
                if (this.gy485type == 0)
                {
                    int eindex = 0;
                    int index = 4;
                    bool okis = false;
                    if (Temp485Buffer.Count >= 8)
                    {
                        for (int i = 0; i < Temp485Buffer.Count; i++)
                        {
                            if (!okis)
                            {
                                if ((Temp485Buffer[i] == 0x68) && (Temp485Buffer[i + 7] == 0x68))
                                {
                                    int len = Temp485Buffer[i + 9];
                                    if ((i + 12 + len - 1 < Temp485Buffer.Count) && (Temp485Buffer[i + 12 + len - 1] == 0x16))
                                    {
                                        eindex = i + 12 + len;
                                        _buffer = new byte[12 + len + 4];
                                        _buffer[0] = 0xFE;
                                        _buffer[1] = 0xFE;
                                        _buffer[2] = 0xFE;
                                        _buffer[3] = 0xFE;
                                        okis = true;
                                    }
                                }
                                else
                                {

                                }
                            }
                            if (okis)
                            {
                                _buffer[index++] = Temp485Buffer[i];
                                if (index == _buffer.Length)
                                    break;
                            }
                        }
                    }
                    if (okis)
                    {
                        Temp485Buffer.RemoveRange(0, eindex);
                    }
                    else
                    {
                        if (Temp485Buffer.Count > 1024)
                            Temp485Buffer.RemoveRange(0, (Temp485Buffer.Count - 1024));
                        _buffer = null;
                    }
                }
                else
                {
                    int eindex = 0;
                    int index = 0;
                    bool okis = false;
                    if (Temp485Buffer.Count > 4)
                    {
                        for (int i = 0; i < Temp485Buffer.Count; i++)
                        {
                            if (!okis)
                            {
                                if ((Temp485Buffer[i] == 0x68) && ((i + 2) < Temp485Buffer.Count))
                                {
                                    int len = Temp485Buffer[i + 1] + Temp485Buffer[i + 2] * 0x100;
                                    if (((i + len + 1) < Temp485Buffer.Count) && (Temp485Buffer[i + len + 1] == 0x16))
                                    {
                                        eindex = i + 2 + len;
                                        _buffer = new byte[2 + len];
                                        okis = true;
                                    }
                                }
                            }
                            if (okis)
                            {
                                _buffer[index++] = Temp485Buffer[i];
                                if (index == _buffer.Length)
                                    break;
                            }
                        }
                    }
                    if (okis)
                    {
                        Temp485Buffer.RemoveRange(0, eindex);
                    }
                    else
                    {
                        if (Temp485Buffer.Count > 2048)
                            Temp485Buffer.RemoveRange(0, (Temp485Buffer.Count - 2048));
                        _buffer = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _buffer = null;
            }
            return _buffer;
        }
        public int get485BufferDatas()
        {
            return this.Temp485Buffer.Count;
        }


        public ExecData ed;
        public EndPoint ep;
        public TCPClientChannelargsUserToken()
        {
            IP = "";
            Port = 0;
            mgrPort = 0;
            PortName = "";
            PortIDName = "";
            PortType = TCPClientChannelTypeOptions.Unknow;
            ed = null;
            ep = null;
        }
        public TCPClientChannelargsUserToken(string _ip, int _port, int _mgrport, string _name, int _id, TCPClientChannelTypeOptions _type, Channel _channel)
        {
            this.channel = _channel;
            this.IP = _ip;
            this.Port = _port;
            this.mgrPort = _mgrport;
            this.PortName = _name;
            this.PortIDName = "[" + _ip + ":" + _id.ToString("00") + "]";
            this.PortType = _type;
            this.ed = null;
            try
            {
                this.ep = (EndPoint)new IPEndPoint(IPAddress.Parse(this.IP), this.Port);
            }
            catch (Exception ex)
            {
                this.ep = null;
            }
        }

        public ConcurrentDictionary<long, byte[]> cb485dataslist = new ConcurrentDictionary<long, byte[]>();
        private object lock485cbobj = new object();
        public void cb485dataslistADD(long _tick, byte[] _datas)
        {
            if (!cb485dataslist.TryAdd(_tick, _datas))
            {
                Thread.Sleep(50);
                cb485dataslist.TryAdd(_tick, _datas);
            }
        }
        public void cb485dataslistClear()
        {
            cb485dataslist.Clear();
        }
        public byte[] getMincb485datas()
        {
            byte[] mindatas = null;
            long mintime = 0;
            foreach (KeyValuePair<long, byte[]> kv in cb485dataslist)
            {
                if (mintime == 0)
                {
                    mintime = kv.Key;
                }
                else if (mintime > kv.Key)
                {
                    mintime = kv.Key;
                }
            }
            cb485dataslist.TryRemove(mintime, out mindatas);
            //cb485dataslist.TryGetValue(mintime, out mindatas);
            return mindatas;
        }

        /// <summary>
        /// 抄表开始了多少时间
        /// </summary>
        private Stopwatch wait485cbtime = new Stopwatch();
        /// <summary>
        /// 抄表是否已经开始了，true　开始，false　没开始
        /// </summary>
        private bool wait485cbis = false;

        /// <summary>
        /// 抄表任务开始等回
        /// </summary>
        public void start485cb()
        {
            lock (lock485cbobj)
            {
                this.wait485cbis = true;
                this.wait485cbtime.Restart();
            }
        }
        /// <summary>
        /// 抄表任务完成
        /// </summary>
        public void stop485cb()
        {
            lock (lock485cbobj)
            {
                this.wait485cbis = false;
                this.wait485cbtime.Stop();
            }
        }
        /// <summary>
        /// 检测是不是已有一个抄表任务在等回
        /// </summary>
        /// <returns></returns>
        public bool check485cbokis()
        {
            lock (lock485cbobj)
            {
                if (wait485cbis)
                {
                    if (wait485cbtime.Elapsed.Seconds > 3)
                    {
                        wait485cbtime.Stop();
                        wait485cbis = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 这个端口的如果是485端口，则根据此判断是否处理抄表报文
        /// </summary>
        public bool cb485OKis = true;
        public void setcb485OK(bool _is)
        {
            this.cb485OKis = _is;
            if (!this.cb485OKis)
            {
                this.cb485dataslistClear();
            }
        }

        /// <summary>
        ///  0 645, 1 698
        /// </summary>
        public int gy485type = 0;

        public int _PROT_MeterBaudRate = 9600;
        public int _PROT_STOP = 1;
        public int _PROT_BIT = 8;
        /// <summary>
        /// 0 无，1 奇，2偶
        /// </summary>
        public int _PROT_P = 2;
    }
    #endregion

    #region << 任务 >>
    public class ExecData
    {
        public EndPoint upep = null;
        public TCPClientChannelargsUserToken usertokn;
        public string key = "";
        public string cmd = "";
        public byte[] backdatas;
        public byte[] senddatas;

        public string up485datas = "";
        public int index = 0;

        public ExecData(ExecData ed)
        {
            upep = ed.upep;
            usertokn = ed.usertokn;
            key = ed.key;
            cmd = ed.cmd;

            backdatas = null;
            senddatas = null;
        }
        public ExecData(string _cmd, EndPoint _upep)
        {
            usertokn = null;
            upep = _upep;
            key = "";
            cmd = _cmd;
            backdatas = null;
            senddatas = null;
        }
        public ExecData(TCPClientChannelargsUserToken _moxaep, string _cmd, EndPoint _upep)
        {
            usertokn = _moxaep;
            upep = _upep;
            key = "";
            cmd = _cmd;
            backdatas = null;
            senddatas = null;
        }
        public ExecData(TCPClientChannelargsUserToken _moxaep, byte[] _senddatas, EndPoint _upep)
        {
            usertokn = _moxaep;
            upep = _upep;
            key = "";
            cmd = "";
            backdatas = null;
            senddatas = _senddatas;
        }

        public ExecData(TCPClientChannelargsUserToken _moxaep)
        {
            usertokn = _moxaep;
            upep = null;
            key = "";
            cmd = "";
            backdatas = null;
            senddatas = null;
        }
    }
    public class Data485
    {
        public string cmd = "";
        public long timetick = 0;
    }
    public class ExecALLThread : AC.Base.ISystemConfig
    {
        #region ISystemConfig实现
        private void XmlToLinkLinkValue(XmlNode xmlnode, byte[] linklinkvalue, int num, string name)
        {
            try
            {
                string[] str = xmlnode.InnerText.Split(',');
                if (str.Length == num)
                {
                    for (int i = 0; i < num; i++)
                    {
                        linklinkvalue[i] = Convert.ToByte(str[i]);
                    }
                }
                else
                {
                    TXTWrite.WriteERRTxt("ExecALLThread", "XmlToLinkLinkValue", name, xmlnode.InnerText, "数据格式不对！");
                }
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("SocketServer", "setLinkLinkValue", name, xmlnode.InnerText, name + ex.ToString());
            }
        }
        private void XmlToLinkLinkValue(XmlNode xmlnode, string[] linklinkvalue, int num, string name)
        {
            try
            {
                string[] str = xmlnode.InnerText.Split(',');
                if (str.Length == num)
                {
                    for (int i = 0; i < num; i++)
                    {
                        linklinkvalue[i] = str[i];
                    }
                }
                else
                {
                    TXTWrite.WriteERRTxt("ExecALLThread", "XmlToLinkLinkValue", name, xmlnode.InnerText, "数据格式不对！");
                }
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("SocketServer", "setLinkLinkValue", name, xmlnode.InnerText, name + ex.ToString());
            }
        }

        public void SetConfig(System.Xml.XmlNode serverConfig)
        {
            for (int i = 0; i < serverConfig.ChildNodes.Count; i++)
            {
                #region TTZB
                if (serverConfig.ChildNodes[i].Name.Equals("LinkDJ"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkDJ, 2, "LinkDJ");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkPM"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkPM, 2, "LinkPM");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBZ"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBZ, 2, "LinkBZ");
                }
                #endregion

                #region 485J1Z3
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW1J1Z3"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW1J1Z3, 2, "LinkBW1J1Z3");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW2J1Z3"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW2J1Z3, 2, "LinkBW2J1Z3");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW3J1Z3"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW3J1Z3, 2, "LinkBW3J1Z3");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW4J1Z3"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW4J1Z3, 2, "LinkBW4J1Z3");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW5J1Z3"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW5J1Z3, 2, "LinkBW5J1Z3");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW6J1Z3"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW6J1Z3, 2, "LinkBW6J1Z3");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW7J1Z3"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW7J1Z3, 2, "LinkBW7J1Z3");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW8J1Z3"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW8J1Z3, 2, "LinkBW8J1Z3");
                }
                #endregion

                #region 485Z1C2
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW1Z1C2"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW1Z1C2, 2, "LinkBW1Z1C2");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW2Z1C2"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW2Z1C2, 2, "LinkBW2Z1C2");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW3Z1C2"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW3Z1C2, 2, "LinkBW3Z1C2");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW4Z1C2"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW4Z1C2, 2, "LinkBW4Z1C2");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW5Z1C2"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW5Z1C2, 2, "LinkBW5Z1C2");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW6Z1C2"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW6Z1C2, 2, "LinkBW6Z1C2");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW7Z1C2"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW7Z1C2, 2, "LinkBW7Z1C2");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW8Z1C2"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW8Z1C2, 2, "LinkBW8Z1C2");
                }
                #endregion

                #region 485J2C1
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW1J2C1"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW1J2C1, 2, "LinkBW1J2C1");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW2J2C1"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW2J2C1, 2, "LinkBW2J2C1");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW3J2C1"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW3J2C1, 2, "LinkBW3J2C1");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW4J2C1"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW4J2C1, 2, "LinkBW4J2C1");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW5J2C1"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW5J2C1, 2, "LinkBW5J2C1");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW6J2C1"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW6J2C1, 2, "LinkBW6J2C1");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW7J2C1"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW7J2C1, 2, "LinkBW7J2C1");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW8J2C1"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW8J2C1, 2, "LinkBW8J2C1");
                }
                #endregion

                #region 2321
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW12321"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW12321, 2, "LinkBW12321");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW22321"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW22321, 2, "LinkBW22321");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW32321"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW32321, 2, "LinkBW32321");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW42321"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW42321, 2, "LinkBW42321");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW52321"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW52321, 2, "LinkBW52321");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW62321"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW62321, 2, "LinkBW62321");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW72321"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW72321, 2, "LinkBW72321");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW82321"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW82321, 2, "LinkBW82321");
                }
                #endregion

                #region 2322
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW12322"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW12322, 2, "LinkBW12322");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW22322"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW22322, 2, "LinkBW22322");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW32322"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW32322, 2, "LinkBW32322");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW42322"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW42322, 2, "LinkBW42322");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW52322"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW52322, 2, "LinkBW52322");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW62322"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW62322, 2, "LinkBW62322");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW72322"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW72322, 2, "LinkBW72322");
                }
                if (serverConfig.ChildNodes[i].Name.Equals("LinkBW82322"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], LinkBW82322, 2, "LinkBW82322");
                }
                #endregion

                #region TTZB
                if (serverConfig.ChildNodes[i].Name.Equals("MOXAIP"))
                {
                    XmlToLinkLinkValue(serverConfig.ChildNodes[i], moxaip, 3, "MOXAIP");
                }
                #endregion

                if (serverConfig.ChildNodes[i].Name.Equals("DSPAgainTime"))
                {
                    this.dspagaintime = Function.ToInt(serverConfig.ChildNodes[i].InnerText);
                }
            }
        }
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xn = xmlDoc.CreateElement("ExecALLThread");

            #region TTZB
            XmlNode XMLLinkDJ = xmlDoc.CreateElement("LinkDJ");
            XMLLinkDJ.InnerText = this.LinkDJ[0] + "," + this.LinkDJ[1];
            xn.AppendChild(XMLLinkDJ);

            XmlNode XMLLinkPM = xmlDoc.CreateElement("LinkPM");
            XMLLinkPM.InnerText = this.LinkPM[0] + "," + this.LinkPM[1];
            xn.AppendChild(XMLLinkPM);

            XmlNode XMLLinkBZ = xmlDoc.CreateElement("LinkBZ");
            XMLLinkBZ.InnerText = this.LinkBZ[0] + "," + this.LinkBZ[1];
            xn.AppendChild(XMLLinkBZ);
            #endregion

            #region 485J1Z3
            XmlNode XMLLinkBW1J1Z3 = xmlDoc.CreateElement("LinkBW1J1Z3");
            XMLLinkBW1J1Z3.InnerText = this.LinkBW1J1Z3[0] + "," + this.LinkBW1J1Z3[1];
            xn.AppendChild(XMLLinkBW1J1Z3);

            XmlNode XMLLinkBW2J1Z3 = xmlDoc.CreateElement("LinkBW2J1Z3");
            XMLLinkBW2J1Z3.InnerText = this.LinkBW2J1Z3[0] + "," + this.LinkBW2J1Z3[1];
            xn.AppendChild(XMLLinkBW2J1Z3);

            XmlNode XMLLinkBW3J1Z3 = xmlDoc.CreateElement("LinkBW3J1Z3");
            XMLLinkBW3J1Z3.InnerText = this.LinkBW3J1Z3[0] + "," + this.LinkBW3J1Z3[1];
            xn.AppendChild(XMLLinkBW3J1Z3);

            XmlNode XMLLinkBW4J1Z3 = xmlDoc.CreateElement("LinkBW4J1Z3");
            XMLLinkBW4J1Z3.InnerText = this.LinkBW4J1Z3[0] + "," + this.LinkBW4J1Z3[1];
            xn.AppendChild(XMLLinkBW4J1Z3);

            XmlNode XMLLinkBW5J1Z3 = xmlDoc.CreateElement("LinkBW5J1Z3");
            XMLLinkBW5J1Z3.InnerText = this.LinkBW5J1Z3[0] + "," + this.LinkBW5J1Z3[1];
            xn.AppendChild(XMLLinkBW5J1Z3);

            XmlNode XMLLinkBW6J1Z3 = xmlDoc.CreateElement("LinkBW6J1Z3");
            XMLLinkBW6J1Z3.InnerText = this.LinkBW6J1Z3[0] + "," + this.LinkBW6J1Z3[1];
            xn.AppendChild(XMLLinkBW6J1Z3);

            XmlNode XMLLinkBW7J1Z3 = xmlDoc.CreateElement("LinkBW7J1Z3");
            XMLLinkBW7J1Z3.InnerText = this.LinkBW7J1Z3[0] + "," + this.LinkBW7J1Z3[1];
            xn.AppendChild(XMLLinkBW7J1Z3);

            XmlNode XMLLinkBW8J1Z3 = xmlDoc.CreateElement("LinkBW8J1Z3");
            XMLLinkBW8J1Z3.InnerText = this.LinkBW8J1Z3[0] + "," + this.LinkBW8J1Z3[1];
            xn.AppendChild(XMLLinkBW8J1Z3);
            #endregion

            #region 485Z1C2
            XmlNode XMLLinkBW1Z1C2 = xmlDoc.CreateElement("LinkBW1Z1C2");
            XMLLinkBW1Z1C2.InnerText = this.LinkBW1Z1C2[0] + "," + this.LinkBW1Z1C2[1];
            xn.AppendChild(XMLLinkBW1Z1C2);

            XmlNode XMLLinkBW2Z1C2 = xmlDoc.CreateElement("LinkBW2Z1C2");
            XMLLinkBW2Z1C2.InnerText = this.LinkBW2Z1C2[0] + "," + this.LinkBW2Z1C2[1];
            xn.AppendChild(XMLLinkBW2Z1C2);

            XmlNode XMLLinkBW3Z1C2 = xmlDoc.CreateElement("LinkBW3Z1C2");
            XMLLinkBW3Z1C2.InnerText = this.LinkBW3Z1C2[0] + "," + this.LinkBW3Z1C2[1];
            xn.AppendChild(XMLLinkBW3Z1C2);

            XmlNode XMLLinkBW4Z1C2 = xmlDoc.CreateElement("LinkBW4Z1C2");
            XMLLinkBW4Z1C2.InnerText = this.LinkBW4Z1C2[0] + "," + this.LinkBW4Z1C2[1];
            xn.AppendChild(XMLLinkBW4Z1C2);

            XmlNode XMLLinkBW5Z1C2 = xmlDoc.CreateElement("LinkBW5Z1C2");
            XMLLinkBW5Z1C2.InnerText = this.LinkBW5Z1C2[0] + "," + this.LinkBW5Z1C2[1];
            xn.AppendChild(XMLLinkBW5Z1C2);

            XmlNode XMLLinkBW6Z1C2 = xmlDoc.CreateElement("LinkBW6Z1C2");
            XMLLinkBW6Z1C2.InnerText = this.LinkBW6Z1C2[0] + "," + this.LinkBW6Z1C2[1];
            xn.AppendChild(XMLLinkBW6Z1C2);

            XmlNode XMLLinkBW7Z1C2 = xmlDoc.CreateElement("LinkBW7Z1C2");
            XMLLinkBW7Z1C2.InnerText = this.LinkBW7Z1C2[0] + "," + this.LinkBW7Z1C2[1];
            xn.AppendChild(XMLLinkBW7Z1C2);

            XmlNode XMLLinkBW8Z1C2 = xmlDoc.CreateElement("LinkBW8Z1C2");
            XMLLinkBW8Z1C2.InnerText = this.LinkBW8Z1C2[0] + "," + this.LinkBW8Z1C2[1];
            xn.AppendChild(XMLLinkBW8Z1C2);

            #endregion

            #region 485J2C1
            XmlNode XMLLinkBW1J2C1 = xmlDoc.CreateElement("LinkBW1J2C1");
            XMLLinkBW1J2C1.InnerText = this.LinkBW1J2C1[0] + "," + this.LinkBW1J2C1[1];
            xn.AppendChild(XMLLinkBW1J2C1);

            XmlNode XMLLinkBW2J2C1 = xmlDoc.CreateElement("LinkBW2J2C1");
            XMLLinkBW2J2C1.InnerText = this.LinkBW2J2C1[0] + "," + this.LinkBW2J2C1[1];
            xn.AppendChild(XMLLinkBW2J2C1);

            XmlNode XMLLinkBW3J2C1 = xmlDoc.CreateElement("LinkBW3J2C1");
            XMLLinkBW3J2C1.InnerText = this.LinkBW3J2C1[0] + "," + this.LinkBW3J2C1[1];
            xn.AppendChild(XMLLinkBW3J2C1);

            XmlNode XMLLinkBW4J2C1 = xmlDoc.CreateElement("LinkBW4J2C1");
            XMLLinkBW4J2C1.InnerText = this.LinkBW4J2C1[0] + "," + this.LinkBW4J2C1[1];
            xn.AppendChild(XMLLinkBW4J2C1);

            XmlNode XMLLinkBW5J2C1 = xmlDoc.CreateElement("LinkBW5J2C1");
            XMLLinkBW5J2C1.InnerText = this.LinkBW5J2C1[0] + "," + this.LinkBW5J2C1[1];
            xn.AppendChild(XMLLinkBW5J2C1);

            XmlNode XMLLinkBW6J2C1 = xmlDoc.CreateElement("LinkBW6J2C1");
            XMLLinkBW6J2C1.InnerText = this.LinkBW6J2C1[0] + "," + this.LinkBW6J2C1[1];
            xn.AppendChild(XMLLinkBW6J2C1);

            XmlNode XMLLinkBW7J2C1 = xmlDoc.CreateElement("LinkBW7J2C1");
            XMLLinkBW7J2C1.InnerText = this.LinkBW7J2C1[0] + "," + this.LinkBW7J2C1[1];
            xn.AppendChild(XMLLinkBW7J2C1);

            XmlNode XMLLinkBW8J2C1 = xmlDoc.CreateElement("LinkBW8J2C1");
            XMLLinkBW8J2C1.InnerText = this.LinkBW8J2C1[0] + "," + this.LinkBW8J2C1[1];
            xn.AppendChild(XMLLinkBW8J2C1);
            #endregion

            #region 4852321
            XmlNode XMLLinkBW12321 = xmlDoc.CreateElement("LinkBW12321");
            XMLLinkBW12321.InnerText = this.LinkBW12321[0] + "," + this.LinkBW12321[1];
            xn.AppendChild(XMLLinkBW12321);

            XmlNode XMLLinkBW22321 = xmlDoc.CreateElement("LinkBW22321");
            XMLLinkBW22321.InnerText = this.LinkBW22321[0] + "," + this.LinkBW22321[1];
            xn.AppendChild(XMLLinkBW22321);

            XmlNode XMLLinkBW32321 = xmlDoc.CreateElement("LinkBW32321");
            XMLLinkBW32321.InnerText = this.LinkBW32321[0] + "," + this.LinkBW32321[1];
            xn.AppendChild(XMLLinkBW32321);

            XmlNode XMLLinkBW42321 = xmlDoc.CreateElement("LinkBW42321");
            XMLLinkBW42321.InnerText = this.LinkBW42321[0] + "," + this.LinkBW42321[1];
            xn.AppendChild(XMLLinkBW42321);

            XmlNode XMLLinkBW52321 = xmlDoc.CreateElement("LinkBW52321");
            XMLLinkBW52321.InnerText = this.LinkBW52321[0] + "," + this.LinkBW52321[1];
            xn.AppendChild(XMLLinkBW52321);

            XmlNode XMLLinkBW62321 = xmlDoc.CreateElement("LinkBW62321");
            XMLLinkBW62321.InnerText = this.LinkBW62321[0] + "," + this.LinkBW62321[1];
            xn.AppendChild(XMLLinkBW62321);

            XmlNode XMLLinkBW72321 = xmlDoc.CreateElement("LinkBW72321");
            XMLLinkBW72321.InnerText = this.LinkBW72321[0] + "," + this.LinkBW72321[1];
            xn.AppendChild(XMLLinkBW72321);

            XmlNode XMLLinkBW82321 = xmlDoc.CreateElement("LinkBW82321");
            XMLLinkBW82321.InnerText = this.LinkBW82321[0] + "," + this.LinkBW82321[1];
            xn.AppendChild(XMLLinkBW82321);
            #endregion

            #region 4852322
            XmlNode XMLLinkBW12322 = xmlDoc.CreateElement("LinkBW12322");
            XMLLinkBW12322.InnerText = this.LinkBW12322[0] + "," + this.LinkBW12322[1];
            xn.AppendChild(XMLLinkBW12322);

            XmlNode XMLLinkBW22322 = xmlDoc.CreateElement("LinkBW22322");
            XMLLinkBW22322.InnerText = this.LinkBW22322[0] + "," + this.LinkBW22322[1];
            xn.AppendChild(XMLLinkBW22322);

            XmlNode XMLLinkBW32322 = xmlDoc.CreateElement("LinkBW32322");
            XMLLinkBW32322.InnerText = this.LinkBW32322[0] + "," + this.LinkBW32322[1];
            xn.AppendChild(XMLLinkBW32322);

            XmlNode XMLLinkBW42322 = xmlDoc.CreateElement("LinkBW42322");
            XMLLinkBW42322.InnerText = this.LinkBW42322[0] + "," + this.LinkBW42322[1];
            xn.AppendChild(XMLLinkBW42322);

            XmlNode XMLLinkBW52322 = xmlDoc.CreateElement("LinkBW52322");
            XMLLinkBW52322.InnerText = this.LinkBW52322[0] + "," + this.LinkBW52322[1];
            xn.AppendChild(XMLLinkBW52322);

            XmlNode XMLLinkBW62322 = xmlDoc.CreateElement("LinkBW62322");
            XMLLinkBW62322.InnerText = this.LinkBW62322[0] + "," + this.LinkBW62322[1];
            xn.AppendChild(XMLLinkBW62322);

            XmlNode XMLLinkBW72322 = xmlDoc.CreateElement("LinkBW72322");
            XMLLinkBW72322.InnerText = this.LinkBW72322[0] + "," + this.LinkBW72322[1];
            xn.AppendChild(XMLLinkBW72322);

            XmlNode XMLLinkBW82322 = xmlDoc.CreateElement("LinkBW82322");
            XMLLinkBW82322.InnerText = this.LinkBW82322[0] + "," + this.LinkBW82322[1];
            xn.AppendChild(XMLLinkBW82322);
            #endregion

            #region 3个MOXAIP
            XmlNode XMLmoxaip = xmlDoc.CreateElement("MOXAIP");
            XMLmoxaip.InnerText = this.moxaip[0] + "," + this.moxaip[1] + "," + this.moxaip[2];
            xn.AppendChild(XMLmoxaip);
            #endregion

            XmlNode XMLdspagaintime = xmlDoc.CreateElement("DSPAgainTime");
            XMLdspagaintime.InnerText = this.dspagaintime.ToString();
            xn.AppendChild(XMLdspagaintime);

            return xn;
        }
        #endregion

        public ExecALLThread()
        {

        }

        public Channel server;

        private void OnMessage(ChannelMessageTypeOptions messageType, string message)
        {
            if (server != null)
                server.OnMessage(messageType, message);
        }

        /// <summary>
        /// 打开崩溃前记录的通道TCPClient打开的端口
        /// </summary>
        public void OpenSaveTCLClientOpens()
        {
            try
            {
                TCPClientChannelargsUserToken _ut = null;
                foreach (KeyValuePair<EndPoint, EndPoint> keyvalue in server.opentcpclients)
                {
                    _ut = null;
                    moxaeplist.TryGetValue(keyvalue.Value, out _ut);
                    if (_ut != null)
                        server.TCPClientStart(_ut);
                }

                foreach (KeyValuePair<EndPoint, TCPClientChannelargsUserToken> keyvalue in moxaeplist)
                {
                    server.TCPClientStart(keyvalue.Value);
                }
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("ExecThread", "OpenSaveTCLClientOpens", "崩溃从新打开链接失败！", "", ex.ToString());
            }
        }

        #region << 初始化系统通道 >>
        public DSPOutputValueCls NowDSPOutputValue = null;
        public BTGNCScls NowBTGNCScls = null;
        public PMShowScls NowPMShowScls = null;
        public ConcurrentDictionary<EndPoint, TCPClientChannelargsUserToken> moxaeplist;

        #region 通道地址
        private byte[] LinkDJ = new byte[] { 0, 3 };
        private byte[] LinkPM = new byte[] { 0, 2 };
        private byte[] LinkBZ = new byte[] { 0, 1 };

        private byte[] LinkBW12321 = new byte[] { 0, 4 };
        private byte[] LinkBW22321 = new byte[] { 0, 9 };
        private byte[] LinkBW32321 = new byte[] { 0, 14 };
        private byte[] LinkBW42321 = new byte[] { 1, 3 };
        private byte[] LinkBW52321 = new byte[] { 1, 8 };
        private byte[] LinkBW62321 = new byte[] { 1, 13 };
        private byte[] LinkBW72321 = new byte[] { 2, 2 };
        private byte[] LinkBW82321 = new byte[] { 2, 7 };

        private byte[] LinkBW12322 = new byte[] { 0, 5 };
        private byte[] LinkBW22322 = new byte[] { 0, 10 };
        private byte[] LinkBW32322 = new byte[] { 0, 15 };
        private byte[] LinkBW42322 = new byte[] { 1, 4 };
        private byte[] LinkBW52322 = new byte[] { 1, 9 };
        private byte[] LinkBW62322 = new byte[] { 1, 14 };
        private byte[] LinkBW72322 = new byte[] { 2, 3 };
        private byte[] LinkBW82322 = new byte[] { 2, 8 };

        private byte[] LinkBW1J1Z3 = new byte[] { 0, 6 };
        private byte[] LinkBW2J1Z3 = new byte[] { 0, 11 };
        private byte[] LinkBW3J1Z3 = new byte[] { 0, 16 };
        private byte[] LinkBW4J1Z3 = new byte[] { 1, 5 };
        private byte[] LinkBW5J1Z3 = new byte[] { 1, 10 };
        private byte[] LinkBW6J1Z3 = new byte[] { 1, 15 };
        private byte[] LinkBW7J1Z3 = new byte[] { 2, 4 };
        private byte[] LinkBW8J1Z3 = new byte[] { 2, 9 };

        private byte[] LinkBW1Z1C2 = new byte[] { 0, 7 };
        private byte[] LinkBW2Z1C2 = new byte[] { 0, 12 };
        private byte[] LinkBW3Z1C2 = new byte[] { 1, 1 };
        private byte[] LinkBW4Z1C2 = new byte[] { 1, 6 };
        private byte[] LinkBW5Z1C2 = new byte[] { 1, 11 };
        private byte[] LinkBW6Z1C2 = new byte[] { 1, 16 };
        private byte[] LinkBW7Z1C2 = new byte[] { 2, 5 };
        private byte[] LinkBW8Z1C2 = new byte[] { 2, 10 };

        private byte[] LinkBW1J2C1 = new byte[] { 0, 8 };
        private byte[] LinkBW2J2C1 = new byte[] { 0, 13 };
        private byte[] LinkBW3J2C1 = new byte[] { 1, 2 };
        private byte[] LinkBW4J2C1 = new byte[] { 1, 7 };
        private byte[] LinkBW5J2C1 = new byte[] { 1, 12 };
        private byte[] LinkBW6J2C1 = new byte[] { 2, 1 };
        private byte[] LinkBW7J2C1 = new byte[] { 2, 6 };
        private byte[] LinkBW8J2C1 = new byte[] { 2, 11 };

        private string[] moxaip = new string[] { "192.168.127.101", "192.168.127.102", "192.168.127.103" };
        #endregion
        public TCPClientChannelargsUserToken getSocketClient(TCPClientChannelTypeOptions ttdy, int index)
        {
            TCPClientChannelargsUserToken rsc = null;
            if (server == null)
                return rsc;

            EndPoint _ep;
            if (ttdy == TCPClientChannelTypeOptions.LinkDJ)
            {
                _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkDJ[0]]), 4000 + LinkDJ[1]);
                moxaeplist.TryGetValue(_ep, out rsc);
            }
            else if (ttdy == TCPClientChannelTypeOptions.LinkBZ)
            {
                _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBZ[0]]), 4000 + LinkBZ[1]);
                moxaeplist.TryGetValue(_ep, out rsc);
            }
            else if (ttdy == TCPClientChannelTypeOptions.LinkPM)
            {
                _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkPM[0]]), 4000 + LinkPM[1]);
                moxaeplist.TryGetValue(_ep, out rsc);
            }
            else if (ttdy == TCPClientChannelTypeOptions.Link2321)
            {
                #region Link2321
                if (index == 0)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW12321[0]]), 4000 + LinkBW12321[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 1)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW22321[0]]), 4000 + LinkBW22321[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 2)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW32321[0]]), 4000 + LinkBW32321[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 3)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW42321[0]]), 4000 + LinkBW42321[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 4)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW52321[0]]), 4000 + LinkBW52321[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 5)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW62321[0]]), 4000 + LinkBW62321[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 6)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW72321[0]]), 4000 + LinkBW72321[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 7)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW82321[0]]), 4000 + LinkBW82321[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                #endregion
            }
            else if (ttdy == TCPClientChannelTypeOptions.Link2322)
            {
                #region Link2322
                if (index == 0)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW12322[0]]), 4000 + LinkBW12322[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 1)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW22322[0]]), 4000 + LinkBW22322[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 2)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW32322[0]]), 4000 + LinkBW32322[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 3)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW42322[0]]), 4000 + LinkBW42322[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 4)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW52322[0]]), 4000 + LinkBW52322[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 5)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW62322[0]]), 4000 + LinkBW62322[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 6)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW72322[0]]), 4000 + LinkBW72322[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 7)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW82322[0]]), 4000 + LinkBW82322[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                #endregion
            }
            else if (ttdy == TCPClientChannelTypeOptions.LinkJ1Z3)
            {
                #region LinkJ1Z3
                if (index == 0)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW1J1Z3[0]]), 4000 + LinkBW1J1Z3[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 1)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW2J1Z3[0]]), 4000 + LinkBW2J1Z3[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 2)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW3J1Z3[0]]), 4000 + LinkBW3J1Z3[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 3)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW4J1Z3[0]]), 4000 + LinkBW4J1Z3[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 4)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW5J1Z3[0]]), 4000 + LinkBW5J1Z3[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 5)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW6J1Z3[0]]), 4000 + LinkBW6J1Z3[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 6)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW7J1Z3[0]]), 4000 + LinkBW7J1Z3[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 7)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW8J1Z3[0]]), 4000 + LinkBW8J1Z3[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                #endregion
            }
            else if (ttdy == TCPClientChannelTypeOptions.LinkZ1C2)
            {
                #region LinkZ1C2
                if (index == 0)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW1Z1C2[0]]), 4000 + LinkBW1Z1C2[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 1)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW2Z1C2[0]]), 4000 + LinkBW2Z1C2[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 2)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW3Z1C2[0]]), 4000 + LinkBW3Z1C2[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 3)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW4Z1C2[0]]), 4000 + LinkBW4Z1C2[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 4)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW5Z1C2[0]]), 4000 + LinkBW5Z1C2[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 5)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW6Z1C2[0]]), 4000 + LinkBW6Z1C2[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 6)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW7Z1C2[0]]), 4000 + LinkBW7Z1C2[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 7)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW8Z1C2[0]]), 4000 + LinkBW8Z1C2[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                #endregion
            }
            else if (ttdy == TCPClientChannelTypeOptions.LinkJ2C1)
            {
                #region LinkJ2C1
                if (index == 0)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW1J2C1[0]]), 4000 + LinkBW1J2C1[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 1)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW2J2C1[0]]), 4000 + LinkBW2J2C1[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 2)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW3J2C1[0]]), 4000 + LinkBW3J2C1[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 3)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW4J2C1[0]]), 4000 + LinkBW4J2C1[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 4)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW5J2C1[0]]), 4000 + LinkBW5J2C1[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 5)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW6J2C1[0]]), 4000 + LinkBW6J2C1[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 6)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW7J2C1[0]]), 4000 + LinkBW7J2C1[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                else if (index == 7)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW8J2C1[0]]), 4000 + LinkBW8J2C1[1]);
                    moxaeplist.TryGetValue(_ep, out rsc);
                }
                #endregion
            }
            return rsc;
        }
        public void InitValue(ApplicationClass m_application)
        {
            moxaeplist = new ConcurrentDictionary<EndPoint, TCPClientChannelargsUserToken>();
            TCPClientChannelargsUserToken usertokn;

            #region 电机，板子，屏幕
            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkDJ[0]], 4000 + LinkDJ[1], 965 + LinkDJ[1], "台体电机通信口", LinkDJ[1], TCPClientChannelTypeOptions.LinkDJ, server);
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBZ[0]], 4000 + LinkBZ[1], 965 + LinkBZ[1], "台体控制通信口", LinkBZ[1], TCPClientChannelTypeOptions.LinkBZ, server);
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkPM[0]], 4000 + LinkPM[1], 965 + LinkPM[1], "台体屏幕通信口", LinkPM[1], TCPClientChannelTypeOptions.LinkPM, server);
            moxaeplist.TryAdd(usertokn.ep, usertokn);
            #endregion
            #region 集中器1/专变3 485
            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW1J1Z3[0]], 4000 + LinkBW1J1Z3[1], 965 + LinkBW1J1Z3[1], "表位1[集1专3]485口", LinkBW1J1Z3[1], TCPClientChannelTypeOptions.LinkJ1Z3, server);
            usertokn.bwindex = 1;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW2J1Z3[0]], 4000 + LinkBW2J1Z3[1], 965 + LinkBW2J1Z3[1], "表位2[集1专3]485口", LinkBW2J1Z3[1], TCPClientChannelTypeOptions.LinkJ1Z3, server);
            usertokn.bwindex = 2;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW3J1Z3[0]], 4000 + LinkBW3J1Z3[1], 965 + LinkBW3J1Z3[1], "表位3[集1专3]485口", LinkBW3J1Z3[1], TCPClientChannelTypeOptions.LinkJ1Z3, server);
            usertokn.bwindex = 3;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW4J1Z3[0]], 4000 + LinkBW4J1Z3[1], 965 + LinkBW4J1Z3[1], "表位4[集1专3]485口", LinkBW4J1Z3[1], TCPClientChannelTypeOptions.LinkJ1Z3, server);
            usertokn.bwindex = 4;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW5J1Z3[0]], 4000 + LinkBW5J1Z3[1], 965 + LinkBW5J1Z3[1], "表位5[集1专3]485口", LinkBW5J1Z3[1], TCPClientChannelTypeOptions.LinkJ1Z3, server);
            usertokn.bwindex = 5;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW6J1Z3[0]], 4000 + LinkBW6J1Z3[1], 965 + LinkBW6J1Z3[1], "表位6[集1专3]485口", LinkBW6J1Z3[1], TCPClientChannelTypeOptions.LinkJ1Z3, server);
            usertokn.bwindex = 6;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW7J1Z3[0]], 4000 + LinkBW7J1Z3[1], 965 + LinkBW7J1Z3[1], "表位7[集1专3]485口", LinkBW7J1Z3[1], TCPClientChannelTypeOptions.LinkJ1Z3, server);
            usertokn.bwindex = 7;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW8J1Z3[0]], 4000 + LinkBW8J1Z3[1], 965 + LinkBW8J1Z3[1], "表位8[集1专3]485口", LinkBW8J1Z3[1], TCPClientChannelTypeOptions.LinkJ1Z3, server);
            usertokn.bwindex = 8;
            moxaeplist.TryAdd(usertokn.ep, usertokn);
            #endregion
            #region 专变1/采集器2 485
            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW1Z1C2[0]], 4000 + LinkBW1Z1C2[1], 965 + LinkBW1Z1C2[1], "表位1[专1采2]485口", LinkBW1Z1C2[1], TCPClientChannelTypeOptions.LinkZ1C2, server);
            usertokn.bwindex = 1;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW2Z1C2[0]], 4000 + LinkBW2Z1C2[1], 965 + LinkBW2Z1C2[1], "表位2[专1采2]485口", LinkBW2Z1C2[1], TCPClientChannelTypeOptions.LinkZ1C2, server);
            usertokn.bwindex = 2;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW3Z1C2[0]], 4000 + LinkBW3Z1C2[1], 965 + LinkBW3Z1C2[1], "表位3[专1采2]485口", LinkBW3Z1C2[1], TCPClientChannelTypeOptions.LinkZ1C2, server);
            usertokn.bwindex = 3;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW4Z1C2[0]], 4000 + LinkBW4Z1C2[1], 965 + LinkBW4Z1C2[1], "表位4[专1采2]485口", LinkBW4Z1C2[1], TCPClientChannelTypeOptions.LinkZ1C2, server);
            usertokn.bwindex = 4;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW5Z1C2[0]], 4000 + LinkBW5Z1C2[1], 965 + LinkBW5Z1C2[1], "表位5[专1采2]485口", LinkBW5Z1C2[1], TCPClientChannelTypeOptions.LinkZ1C2, server);
            usertokn.bwindex = 5;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW6Z1C2[0]], 4000 + LinkBW6Z1C2[1], 965 + LinkBW6Z1C2[1], "表位6[专1采2]485口", LinkBW6Z1C2[1], TCPClientChannelTypeOptions.LinkZ1C2, server);
            usertokn.bwindex = 6;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW7Z1C2[0]], 4000 + LinkBW7Z1C2[1], 965 + LinkBW7Z1C2[1], "表位7[专1采2]485口", LinkBW7Z1C2[1], TCPClientChannelTypeOptions.LinkZ1C2, server);
            usertokn.bwindex = 7;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW8Z1C2[0]], 4000 + LinkBW8Z1C2[1], 965 + LinkBW8Z1C2[1], "表位8[专1采2]485口", LinkBW8Z1C2[1], TCPClientChannelTypeOptions.LinkZ1C2, server);
            usertokn.bwindex = 8;
            moxaeplist.TryAdd(usertokn.ep, usertokn);
            #endregion
            #region 集中器2/采集器1 485
            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW1J2C1[0]], 4000 + LinkBW1J2C1[1], 965 + LinkBW1J2C1[1], "表位1[集2采1]485口", LinkBW1J2C1[1], TCPClientChannelTypeOptions.LinkJ2C1, server);
            usertokn.bwindex = 1;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW2J2C1[0]], 4000 + LinkBW2J2C1[1], 965 + LinkBW2J2C1[1], "表位2[集2采1]485口", LinkBW2J2C1[1], TCPClientChannelTypeOptions.LinkJ2C1, server);
            usertokn.bwindex = 2;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW3J2C1[0]], 4000 + LinkBW3J2C1[1], 965 + LinkBW3J2C1[1], "表位3[集2采1]485口", LinkBW3J2C1[1], TCPClientChannelTypeOptions.LinkJ2C1, server);
            usertokn.bwindex = 3;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW4J2C1[0]], 4000 + LinkBW4J2C1[1], 965 + LinkBW4J2C1[1], "表位4[集2采1]485口", LinkBW4J2C1[1], TCPClientChannelTypeOptions.LinkJ2C1, server);
            usertokn.bwindex = 4;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW5J2C1[0]], 4000 + LinkBW5J2C1[1], 965 + LinkBW5J2C1[1], "表位5[集2采1]485口", LinkBW5J2C1[1], TCPClientChannelTypeOptions.LinkJ2C1, server);
            usertokn.bwindex = 5;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW6J2C1[0]], 4000 + LinkBW6J2C1[1], 965 + LinkBW6J2C1[1], "表位6[集2采1]485口", LinkBW6J2C1[1], TCPClientChannelTypeOptions.LinkJ2C1, server);
            usertokn.bwindex = 6;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW7J2C1[0]], 4000 + LinkBW7J2C1[1], 965 + LinkBW7J2C1[1], "表位7[集2采1]485口", LinkBW7J2C1[1], TCPClientChannelTypeOptions.LinkJ2C1, server);
            usertokn.bwindex = 7;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW8J2C1[0]], 4000 + LinkBW8J2C1[1], 965 + LinkBW8J2C1[1], "表位8[集2采1]485口", LinkBW8J2C1[1], TCPClientChannelTypeOptions.LinkJ2C1, server);
            usertokn.bwindex = 8;
            moxaeplist.TryAdd(usertokn.ep, usertokn);
            #endregion
            #region 2321
            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW12321[0]], 4000 + LinkBW12321[1], 965 + LinkBW12321[1], "表位1[左侧]485口", LinkBW12321[1], TCPClientChannelTypeOptions.Link2321, server);
            usertokn.bwindex = 1;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW22321[0]], 4000 + LinkBW22321[1], 965 + LinkBW22321[1], "表位2[左侧]485口", LinkBW22321[1], TCPClientChannelTypeOptions.Link2321, server);
            usertokn.bwindex = 2;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW32321[0]], 4000 + LinkBW32321[1], 965 + LinkBW32321[1], "表位3[左侧]485口", LinkBW32321[1], TCPClientChannelTypeOptions.Link2321, server);
            usertokn.bwindex = 3;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW42321[0]], 4000 + LinkBW42321[1], 965 + LinkBW42321[1], "表位4[左侧]485口", LinkBW42321[1], TCPClientChannelTypeOptions.Link2321, server);
            usertokn.bwindex = 4;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW52321[0]], 4000 + LinkBW52321[1], 965 + LinkBW52321[1], "表位5[左侧]485口", LinkBW52321[1], TCPClientChannelTypeOptions.Link2321, server);
            usertokn.bwindex = 5;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW62321[0]], 4000 + LinkBW62321[1], 965 + LinkBW62321[1], "表位6[左侧]485口", LinkBW62321[1], TCPClientChannelTypeOptions.Link2321, server);
            usertokn.bwindex = 6;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW72321[0]], 4000 + LinkBW72321[1], 965 + LinkBW72321[1], "表位7[左侧]485口", LinkBW72321[1], TCPClientChannelTypeOptions.Link2321, server);
            usertokn.bwindex = 7;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW82321[0]], 4000 + LinkBW82321[1], 965 + LinkBW82321[1], "表位8[左侧]485口", LinkBW82321[1], TCPClientChannelTypeOptions.Link2321, server);
            usertokn.bwindex = 8;
            moxaeplist.TryAdd(usertokn.ep, usertokn);
            #endregion
            #region 2322
            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW12322[0]], 4000 + LinkBW12322[1], 965 + LinkBW12322[1], "表位1[右侧]485口", LinkBW12322[1], TCPClientChannelTypeOptions.Link2322, server);
            usertokn.bwindex = 1;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW22322[0]], 4000 + LinkBW22322[1], 965 + LinkBW22322[1], "表位2[右侧]485口", LinkBW22322[1], TCPClientChannelTypeOptions.Link2322, server);
            usertokn.bwindex = 2;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW32322[0]], 4000 + LinkBW32322[1], 965 + LinkBW32322[1], "表位3[右侧]485口", LinkBW32322[1], TCPClientChannelTypeOptions.Link2322, server);
            usertokn.bwindex = 3;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW42322[0]], 4000 + LinkBW42322[1], 965 + LinkBW42322[1], "表位4[右侧]485口", LinkBW42322[1], TCPClientChannelTypeOptions.Link2322, server);
            usertokn.bwindex = 4;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW52322[0]], 4000 + LinkBW52322[1], 965 + LinkBW52322[1], "表位5[右侧]485口", LinkBW52322[1], TCPClientChannelTypeOptions.Link2322, server);
            usertokn.bwindex = 5;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW62322[0]], 4000 + LinkBW62322[1], 965 + LinkBW62322[1], "表位6[右侧]485口", LinkBW62322[1], TCPClientChannelTypeOptions.Link2322, server);
            usertokn.bwindex = 6;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW72322[0]], 4000 + LinkBW72322[1], 965 + LinkBW72322[1], "表位7[右侧]485口", LinkBW72322[1], TCPClientChannelTypeOptions.Link2322, server);
            usertokn.bwindex = 7;
            moxaeplist.TryAdd(usertokn.ep, usertokn);

            usertokn = new TCPClientChannelargsUserToken(moxaip[LinkBW82322[0]], 4000 + LinkBW82322[1], 965 + LinkBW82322[1], "表位8[右侧]485口", LinkBW82322[1], TCPClientChannelTypeOptions.Link2322, server);
            usertokn.bwindex = 8;
            moxaeplist.TryAdd(usertokn.ep, usertokn);
            #endregion

            NowBTGNCScls = m_application.GetSystemConfig(typeof(BTGNCScls)) as BTGNCScls;
            //m_application.SetSystemConfig(NowBTGNCScls);

            for (int i = 0; i < NowBTGNCScls.btgns.Length; i++)
            {
                NowBTGNCScls.btgns[i].server = server;
            }

            NowPMShowScls = new PMShowScls();
            TCPClientChannelargsUserToken lnk = null;
            byte[] sendbytes = null;
            byte[] receivebytes = null;
            for (int i = 0; i < NowPMShowScls.pmshows.Length; i++)
            {
                NowPMShowScls.pmshows[i].server = server;
                sendbytes = null;
                receivebytes = null;
                lnk = getSocketClient(TCPClientChannelTypeOptions.LinkPM, i);

                NowPMShowScls.pmshows[i].cmd_bgkSend(lnk, out sendbytes, out receivebytes);
                Thread.Sleep(500);
            }

            NowDSPOutputValue = m_application.GetSystemConfig(typeof(DSPOutputValueCls)) as DSPOutputValueCls;
        }
        #endregion

        #region  <<任务处理>>
        private System.Timers.Timer exectime;
        private bool runningis = false;
        private bool ttworkis = false;
        private int runningstate = 0;
        public void timerstart()
        {
            if (exectime == null)
            {
                exectime = new System.Timers.Timer();
                exectime.Elapsed += new System.Timers.ElapsedEventHandler(timer_tick);
                exectime.Interval = 50;
                exectime.AutoReset = false;
                exectime.Start();
            }
            else
            {
                if (runningis == false)
                {
                    exectime.Start();
                }

            }
        }

        public void timerwait()
        {
            ttworkis = true;
            while (runningstate == 1)
            {
                Thread.Sleep(100);
            }
        }
        public void timerrun()
        {
            ttworkis = false;
            if (runningis)
                exectime.Start();
        }

        private void timer_tick(object sender, EventArgs e)
        {
            runningis = true;
            runexecs();
        }

        private ConcurrentDictionary<long, ExecData> cmdslist = new ConcurrentDictionary<long, ExecData>();

        private void cmdslistADD(long _key, ExecData _value)
        {
            lock (cmdslist)
            {
                int index = 0;
                while (index < 3)
                {
                    if (cmdslist.TryAdd(_key, _value))
                        break;
                    else
                    {
                        _key++;
                        index++;
                    }
                }
            }
        }
        private void cmdslistDel(long _key, out ExecData _value)
        {
            lock (cmdslist)
            {
                cmdslist.TryRemove(_key, out _value);
            }
        }
        private void cmdslistClear()
        {
            lock (cmdslist)
            {
                cmdslist.Clear();
            }
        }
        public void ClearList()
        {
            cmdslistClear();
        }
        public bool addexec(string cmd, EndPoint upudp)
        {
            bool f = false;
            try
            {
                foreach (KeyValuePair<long, ExecData> kv in cmdslist)
                {
                    if (kv.Value.cmd.Equals(cmd))
                    {
                        f = true;
                        break;
                    }
                }
                if (f)
                    return false;

                byte[] sendbytes = null;
                string[] cmdks = cmd.Split(',');
                string key = String.Empty;
                byte ret = 0;
                string data = String.Empty;
                if (cmdks.Length < 3)
                {
                    key = cmdks[0].Split('=')[1];
                    data = cmdks[1].Split('=')[1];
                }
                else
                {
                    key = cmdks[0].Split('=')[1];
                    ret = Convert.ToByte(cmdks[1].Split('=')[1]);
                    data = cmdks[2].Split('=')[1];
                }
                string[] strdatas = new string[0];

                int bwindex = 0;
                ExecData _execdata;

                switch (key)
                {
                    case "1001":
                        #region <<  1001 给各表位终端发送规约数据>>
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]);
                        if ((bwindex > 0) && (bwindex < 9))
                        {

                            _execdata = new ExecData(cmd, upudp);
                            cmdslistADD(DateTime.Now.Ticks, _execdata);
                            f = true;
                        }
                        #endregion
                        break;
                    case "1002":
                        #region <<  1002 各表位终端抄读模拟电表数据>>
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]);
                        if ((bwindex > 0) && (bwindex < 9))
                        {
                            strdatas = data.Split(';');
                            bwindex = Convert.ToInt32(strdatas[0]) - 1;
                            if ((bwindex >= 0) && (bwindex < 8))
                            {
                                byte[] _485temp = HexStringToByteArray(strdatas[1]);
                                TCPClientChannelargsUserToken _485ut = null;
                                EndPoint _485ep = null;
                                if (Convert.ToByte(strdatas[1].Substring(0, 2), 16) == 0x68 && Convert.ToByte(strdatas[1].Substring(14, 2), 16) == 0x68)
                                {
                                    string _baddress = strdatas[1].Substring(12, 2) + strdatas[1].Substring(10, 2) + strdatas[1].Substring(8, 2) + strdatas[1].Substring(6, 2) + strdatas[1].Substring(4, 2) + strdatas[1].Substring(2, 2);
                                    server.up485epslist.TryGetValue(((bwindex + 1).ToString() + _baddress), out _485ep);
                                }
                                else if (((_485temp[0] == 0x68) && (_485temp[14] == 0x85) && (_485temp[BitConverter.ToInt16(_485temp, 1) + 1] == 0x16)) || ((_485temp[0] == 0x68) && (_485temp[14] == 0x90) && (_485temp[17] == 0x85) && (_485temp[BitConverter.ToInt16(_485temp, 1) + 1] == 0x16)))//698虚拟表
                                {
                                    string _baddress = _485temp[10].ToString("X2") + _485temp[9].ToString("X2") + _485temp[8].ToString("X2") + _485temp[7].ToString("X2") + _485temp[6].ToString("X2") + _485temp[5].ToString("X2");
                                    server.up485epslist.TryGetValue(((bwindex + 1).ToString() + _baddress), out _485ep);
                                }
                                else
                                {
                                    byte _tempbyte = Convert.ToByte(strdatas[1].Substring(8, 2), 16);
                                    int AFlen = (_tempbyte & 0x07) + 1;
                                    int AFtype = (_tempbyte & 0xC0);
                                    string _baddress = strdatas[1].Substring(8, (4 + AFlen * 2));
                                    server.up485epslist.TryGetValue(((bwindex + 1).ToString() + _baddress), out _485ep);
                                }

                                if (_485ep != null)
                                {
                                    moxaeplist.TryGetValue(_485ep, out _485ut);
                                    if (_485ut != null)
                                    {
                                        try
                                        {
                                            f = NowBTGNCScls.btgns[bwindex].SendMNCB(_485ut, strdatas, out sendbytes);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    else
                                        this.OnMessage(ChannelMessageTypeOptions.MOXAError, "找不到回复对象！" + cmd);
                                }
                            }
                        }
                        #endregion
                        break;
                    default:
                        _execdata = new ExecData(cmd, upudp);
                        cmdslistADD(DateTime.Now.Ticks, _execdata);
                        f = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                this.OnMessage(ChannelMessageTypeOptions.MOXAError, "错误命令！" + ex.Message);
            }
            return f;
        }
        /// <summary>
        /// 16进制字符串转换byte[]
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        private void runexecs()
        {
            Console.WriteLine("===开始");
            if (ttworkis)
                return;
            bool f = false;
            runningstate = 1;
            long _time = 0;
            try
            {
                ExecData ed = null;
                if (cmdslist.Count > 0)
                {
                    foreach (long _key in cmdslist.Keys)
                    {
                        if (_time == 0)
                            _time = _key;
                        else if (_time > _key)
                            _time = _key;
                    }
                    cmdslist.TryGetValue(_time, out ed);
                    if (ed != null)
                    {
                        //if (execthreadis(ed))
                        //{
                        //    TCPClientChannelargsUserToken ut = ed.usertokn;
                        //    if (ut != null)
                        //    {
                        //        if (ut.exectime == null)
                        //        {

                        //        }
                        //        ParameterizedThreadStart parts = new ParameterizedThreadStart(threadexec);
                        //        ut.edthread = new Thread(parts);
                        //        ut.edthread.Priority = ThreadPriority.Highest;
                        //        ut.ed = ed;
                        //        ut.edthread.Start(ut);
                        //        cmdslistDel(_time, out ed);
                        //    }
                        //}
                        //else
                        //{
                        exec(ed);
                        cmdslistDel(_time, out ed);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("ExecALLThread", "runexecs", "MOXA", "", ex.ToString());
            }
            finally
            {
                runningstate = 0;
                if (cmdslist.Count > 0)
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
            Console.WriteLine("结束===");
        }
        #endregion

        #region  <<485抄表　任务处理>>
        //private System.Timers.Timer exectime485cb;
        //private bool runningis485cb = false;
        //public void timer485cbstart()
        //{
        //    if (exectime485cb == null)
        //    {
        //        exectime485cb = new System.Timers.Timer();
        //        exectime485cb.Elapsed += new System.Timers.ElapsedEventHandler(timer485cb_tick);
        //        exectime485cb.Interval = 50;
        //        exectime485cb.AutoReset = false;
        //        exectime485cb.Start();
        //    }
        //    else
        //    {
        //        if (runningis485cb == false)
        //        {
        //            exectime485cb.Start();
        //        }
        //    }
        //}
        //private void timer485cb_tick(object sender, EventArgs e)
        //{
        //    runningis485cb = true;
        //    run485cbexecs();
        //}
        //public ConcurrentDictionary<int, TCPClientChannelargsUserToken> cmds485cblist = new ConcurrentDictionary<int,TCPClientChannelargsUserToken>();

        //public void Clear485cbList()
        //{
        //    foreach (KeyValuePair<int, TCPClientChannelargsUserToken> kv in cmds485cblist)
        //    {
        //        kv.Value.cb485dataslistClear();
        //        kv.Value.stop485cb();
        //    }
        //    cmds485cblist.Clear();
        //}

        //public void addexec485cb(int _bw, TCPClientChannelargsUserToken _ut)
        //{
        //    if (!cmds485cblist.ContainsKey(_bw))
        //        cmds485cblist.TryAdd(_bw, _ut);
        //}
        //private void run485cbexecs()
        //{
        //    try
        //    {
        //        TCPClientChannelargsUserToken tempUT = null;
        //        byte[] senddatas = null;
        //        foreach (KeyValuePair<int, TCPClientChannelargsUserToken> kv in cmds485cblist)
        //        {
        //            tempUT = kv.Value;
        //            if (tempUT.check485cbokis())
        //            {
        //                senddatas = tempUT.getMincb485datas();
        //                if (server == null)
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    if (server.State == ChannelStateOptions.Running)
        //                    {
        //                        server.UdpSend(string.Format("cmd=1002,data={0};{1}", kv.Key, AC.Base.Function.ByteArrayToHex(senddatas)), null);
        //                        tempUT.start485cb();
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TXTWrite.WriteERRTxt("ExecALLThread", "runexecs", "MOXA", "", ex.ToString());
        //    }
        //    finally
        //    {
        //        bool f = false;
        //        foreach (KeyValuePair<int, TCPClientChannelargsUserToken> kv in cmds485cblist)
        //        {
        //            if (kv.Value.cb485dataslist.Count > 0)
        //            {
        //                f = true;
        //                break;
        //            }
        //        }
        //        if (f)
        //        {
        //            exectime485cb.Start();
        //        }
        //        else
        //        {
        //            runningis485cb = false;
        //            exectime485cb.Stop();
        //        }
        //    }
        //}
        #endregion

        private DateTime dspoutputtime = DateTime.MinValue;
        private int dspagaintime = 15;

        private void threadDJSY(object data)
        {
            string[] strdatas = (data as string).Split(';');
            NowDSPOutputValue.DJupCInputToValue(strdatas);
            int r = -1;
            r = NowDSPOutputValue.SetOperationHC_Fun();
            Thread.Sleep(300);
            if (r == -1)
            {
                r = NowDSPOutputValue.DSPOutput();
            }
        }

        /// <summary>
        /// 因为高级回复升源超时，不得已使用线程
        /// </summary>
        private void threadGJSY(object data)
        {
            string[] strdatas = (data as string).Split(';');
            NowDSPOutputValue.GJupCInputToValue(strdatas);
            int r = -1;
            if ((NowDSPOutputValue.Ia == 0) || (NowDSPOutputValue.Ib == 0) || (NowDSPOutputValue.Ic == 0))
            {
                r = NowDSPOutputValue.DSPStop();
            }
            if (r == -1)
            {
                r = NowDSPOutputValue.SetOperationHC_Fun();
            }
            if (r == -1)
            {
                r = NowDSPOutputValue.DSPOutput();
            }
            if (r == -1)
            {
                r = NowDSPOutputValue.OperationHC_Fun();
            }
        }
        private void threadGJSY99(object data)
        {
            string[] strdatas = (data as string).Split(';');
            NowDSPOutputValue.GJupCInputToValue99(strdatas);
            int r = -1;
            if ((NowDSPOutputValue.Ia == 0) || (NowDSPOutputValue.Ib == 0) || (NowDSPOutputValue.Ic == 0))
            {
                r = NowDSPOutputValue.DSPStop();
            }
            if (r == -1)
            {
                r = NowDSPOutputValue.SetOperationHC_Fun();
            }
            if (r == -1)
            {
                r = NowDSPOutputValue.DSPOutput();
            }
            if (r == -1)
            {
                r = NowDSPOutputValue.OperationHC_Fun();
            }
        }

        private void threadReadY(object data)
        {
            int r = NowDSPOutputValue.OperationHC_Fun();
            Thread.Sleep(300);
        }

        private bool execthreadis(ExecData ed)
        {
            string result = string.Empty;
            try
            {
                string cmd = ed.cmd;
                string[] cmdks = cmd.Split(',');
                string key = String.Empty;
                byte ret = 0;
                string data = String.Empty;
                if (cmdks.Length < 3)
                {
                    key = cmdks[0].Split('=')[1];
                    data = cmdks[1].Split('=')[1];
                }
                else
                {
                    key = cmdks[0].Split('=')[1];
                    ret = Convert.ToByte(cmdks[1].Split('=')[1]);
                    data = cmdks[2].Split('=')[1];
                }
                string[] strdatas;
                TCPClientChannelargsUserToken lnk = null;
                int bwindex = 0;
                #region <<  1001 给各表位终端发送规约数据>>
                if (key == "1001")
                {
                    strdatas = data.Split(';');
                    bwindex = Convert.ToInt32(strdatas[0]) - 1;
                    if ((bwindex >= 0) && (bwindex < 8))
                    {
                        lnk = NowBTGNCScls.btgns[bwindex].ZFgetSocketClient(bwindex);
                        if (lnk != null)
                        {
                            ed.usertokn = lnk;
                            return true;
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("ExecThread", "execthreadis", "", "", ex.ToString());
            }
            return false;
        }
        private void exec(ExecData ed)
        {
            string result = string.Empty;
            try
            {
                string cmd = ed.cmd;
                ExecData ed2 = new ExecData(ed.usertokn);
                ExecData ed3 = new ExecData(ed.usertokn);
                ExecData ed4 = new ExecData(ed.usertokn);
                ExecData ed5 = new ExecData(ed.usertokn);
                ExecData ed6 = new ExecData(ed.usertokn);
                ExecData ed7 = new ExecData(ed.usertokn);
                string[] cmdks = cmd.Split(',');
                string key = String.Empty;
                byte ret = 0;
                string data = String.Empty;
                if (cmdks.Length < 3)
                {
                    key = cmdks[0].Split('=')[1];
                    data = cmdks[1].Split('=')[1];
                }
                else
                {
                    key = cmdks[0].Split('=')[1];
                    ret = Convert.ToByte(cmdks[1].Split('=')[1]);
                    data = cmdks[2].Split('=')[1];
                }
                string[] strdatas = new string[0];
                byte[] sendbytes = null;
                string receivestr = String.Empty;
                TCPClientChannelargsUserToken lnk = null;
                bool f = false;
                int r = 0;
                int bwindex = 0;
                int _tempdata = 0;
                switch (key)
                {
                    #region << 0101 连接台体多路服务器 , 本机不用连接多路服务器 >>  ???
                    case "0101":
                        result = "cmd=0101,ret=0,data=null";
                        break;
                    #endregion

                    #region << 0102 初始化台体多路服务器串口>>  ???
                    case "0102":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[2]) - 1;
                        f = NowBTGNCScls.btgns[bwindex].KGTDtoK(strdatas);
                        result = "cmd=0102,ret=0,data=null";
                        break;
                    #endregion

                    #region << 0103 关闭台体多路服务器串口 >>  ???
                    case "0103":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[2]) - 1;
                        f = NowBTGNCScls.btgns[bwindex].KGTDtoG(strdatas);
                        result = "cmd=0103,ret=0,data=null";
                        break;
                    #endregion

                    #region << 0104 初始化台体多路服务器串口>>  ???
                    case "0104":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[2]) - 1;
                        if (bwindex >= 0)
                            f = NowBTGNCScls.btgns[bwindex].KGTDnewtoK(strdatas, bwindex);
                        else if (bwindex == -1)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                f = NowBTGNCScls.btgns[bwindex].KGTDnewtoK(strdatas, i);
                            }
                        }
                        result = "cmd=0104,ret=0,data=null";
                        break;
                    #endregion

                    #region << 0201 台体升源（常用） >>
                    case "0201":
                        ParameterizedThreadStart _djsythread = new ParameterizedThreadStart(threadDJSY);
                        Thread _djthread = new Thread(_djsythread);
                        _djthread.IsBackground = true;
                        _djthread.Start(data);

                        Thread.Sleep(20000);
                        result = "cmd=0201,ret=0,data=null";
                        break;
                    #endregion

                    #region << 0202 功率源升源（高级） >>
                    case "0202":
                        ParameterizedThreadStart _gjsythread = new ParameterizedThreadStart(threadGJSY);
                        Thread _gjthread = new Thread(_gjsythread);
                        _gjthread.IsBackground = true;
                        _gjthread.Start(data);

                        Thread.Sleep(20000);
                        result = "cmd=0202,ret=0,data=null";
                        break;
                    case "990202":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]);
                        ParameterizedThreadStart _99gjsythread = new ParameterizedThreadStart(threadGJSY99);
                        Thread _99thread = new Thread(_99gjsythread);
                        _99thread.IsBackground = true;
                        _99thread.Start(data);

                        Thread.Sleep(20000);
                        result = string.Format("cmd={0},ret=0,data={1};null", key, bwindex);
                        break;
                    #endregion

                    #region << 0203 功率源关源 >>
                    case "0203":
                        r = NowDSPOutputValue.DSPDown();
                        Thread.Sleep(300);
                        if (r == -1)
                        {
                            result = "cmd=0203,ret=0,data=null";
                        }
                        else
                        {
                            result = "cmd=0203,ret=1,data=null";
                        }
                        break;
                    #endregion

                    #region << 0204 设置谐波参数>>
                    case "0204":
                        strdatas = data.Split(';');
                        f = NowDSPOutputValue.XBsetCInputToValue(strdatas);
                        if (f)
                        {
                            r = NowDSPOutputValue.SetDSPHarmonic();
                            Thread.Sleep(300);
                        }
                        if (r == -1)
                        {
                            result = "cmd=0204,ret=0,data=null";
                        }
                        else
                        {
                            result = "cmd=0204,ret=1,data=null";
                        }
                        break;
                    #endregion

                    #region << 0301 读标准表数据>>
                    case "990301":
                    case "0301":
                        if (key == "990301")
                        {
                            strdatas = data.Split(';');
                            bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        }
                        ParameterizedThreadStart _readythread = new ParameterizedThreadStart(threadReadY);
                        Thread _readthread = new Thread(_readythread);
                        _readthread.IsBackground = true;
                        _readthread.Start(data);

                        Thread.Sleep(15000);
                        result = NowDSPOutputValue.XBCValueToOutput(key, bwindex);
                        break;
                    #endregion

                    #region << 0302 设置标准表>>     XXX
                    case "0302":
                        break;
                    #endregion

                    #region << 990303 设置误差表按表位来>>
                    case "990303":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8))
                        {
                            f = NowDSPOutputValue.WCBsetInputToValue(strdatas);
                            if (f)
                            {
                                NowDSPOutputValue.WCB_CLD_Clear(bwindex + 1);
                                Thread.Sleep(500);
                                r = NowDSPOutputValue.WCB_Start(bwindex + 1);
                                if (r == -1)
                                {
                                    result = string.Format("cmd=990303,ret=0,data={0};null", (bwindex + 1));
                                }
                                else
                                {
                                    result = string.Format("cmd=990303,ret=1,data={0};null", (bwindex + 1));
                                }
                            }
                            else
                            {
                                result = string.Format("cmd=990303,ret=1,data={0};null", (bwindex + 1));
                            }
                        }
                        break;
                    #endregion

                    #region << 990304 读取误差表>>     XXX
                    case "990304":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8))
                        {
                            r = NowDSPOutputValue.WCB_Read(bwindex + 1);
                            Thread.Sleep(300);
                            if (r == -1)
                            {
                                result = NowDSPOutputValue.WCBValueToOutput("990304", (bwindex + 1));
                            }
                            else
                            {
                                result = string.Format("cmd=990304,ret=1,data={0};null", (bwindex + 1));
                            }
                        }
                        break;
                    #endregion

                    #region << 990305 载波开>>     XXX
                    case "990305":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        Thread.Sleep(300);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = NowBTGNCScls.btgns[bwindex].SendTDDDYup(lnk, ed, strdatas);
                            Thread.Sleep(500);
                            if (f)
                            {
                                result = string.Format("cmd={0},ret=0,data={1};null", key, (bwindex + 1));
                            }
                            else
                            {
                                result = string.Format("cmd={0},ret=1,data={1};null", key, (bwindex + 1));
                            }
                        }
                        break;
                    #endregion

                    #region << 990306 载波关>>     XXX
                    case "990306":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = NowBTGNCScls.btgns[bwindex].SendTDDDYdown(lnk, ed, strdatas);
                            Thread.Sleep(500);
                            if (f)
                            {
                                result = string.Format("cmd={0},ret=0,data={1};null", key, (bwindex + 1));
                            }
                            else
                            {
                                result = string.Format("cmd={0},ret=1,data={1};null", key, (bwindex + 1));
                            }
                        }
                        break;
                    #endregion

                    #region << 0401 初始化台体表位>>   ???
                    case "0401":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;

                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            lnk.gy485type = 0;
                            _tempdata = Convert.ToInt32(strdatas[1]);
                            if (_tempdata == 1)
                            {
                                f = NowBTGNCScls.btgns[bwindex].SendTDDDLup(lnk, ed, strdatas);
                            }
                            else
                            {
                                f = NowBTGNCScls.btgns[bwindex].SendTDDDLdown(lnk, ed, strdatas);
                            }
                            Thread.Sleep(300);
                            if (f)
                            {
                                f = NowBTGNCScls.btgns[bwindex].SendTDDSet(lnk, ed, strdatas);
                                Thread.Sleep(300);
                            }

                            _tempdata = Convert.ToInt32(strdatas[2]);
                            if (f)
                            {
                                f = NowBTGNCScls.btgns[bwindex].SendSelD485Set(lnk, ed, ed2, ed3, ed4, ed5, strdatas);
                                Thread.Sleep(300);
                            }
                            if (f)
                            {
                                f = NowBTGNCScls.btgns[bwindex].SetTD485(bwindex, _tempdata, out sendbytes, out receivestr);
                                Thread.Sleep(300);
                            }
                            if (f)
                            {
                                if (NowBTGNCScls.btgns[bwindex].deviceseld3 == BTGNcls.DSeljcType.DB)
                                {
                                    f = NowBTGNCScls.btgns[bwindex].cmd_tddbdlSend(lnk, ed, BTGNcls.TDDDLDYType.TD);
                                }
                                else
                                {
                                    f = NowBTGNCScls.btgns[bwindex].cmd_tddbdlSend(lnk, ed, BTGNcls.TDDDLDYType.DD);
                                }
                                Thread.Sleep(300);
                            }
                            if (f)
                            {
                                result = string.Format("cmd={0},ret=0,data=null", key);
                            }
                            else
                            {
                                result = string.Format("cmd={0},ret=1,data=null", key);
                            }
                        }
                        break;
                    case "990401":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;

                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            lnk.gy485type = 0;
                            _tempdata = Convert.ToInt32(strdatas[1]);
                            if (_tempdata == 0)
                            {
                                f = NowBTGNCScls.btgns[bwindex].SendTDDDLdown(lnk, ed, strdatas);
                            }
                            else
                            {
                                f = NowBTGNCScls.btgns[bwindex].SendTDDDLup(lnk, ed, strdatas);
                            }
                            Thread.Sleep(300);

                            if (f)
                            {
                                _tempdata = Convert.ToInt32(strdatas[3]);
                                if (_tempdata == 1)
                                    f = NowBTGNCScls.btgns[bwindex].SendTDDDYup(lnk, ed, strdatas);
                                else
                                    f = NowBTGNCScls.btgns[bwindex].SendTDDDYdown(lnk, ed, strdatas);
                                Thread.Sleep(300);
                            }

                            _tempdata = Convert.ToInt32(strdatas[2]);
                            if (f)
                            {
                                f = NowBTGNCScls.btgns[bwindex].SendSelD485Set(lnk, ed, ed2, ed3, ed4, ed5, strdatas);
                                Thread.Sleep(300);
                            }
                            if (f)
                            {
                                f = NowBTGNCScls.btgns[bwindex].SetTD485(bwindex, _tempdata, out sendbytes, out receivestr);
                                Thread.Sleep(300);
                            }
                            if (f)
                            {
                                if (NowBTGNCScls.btgns[bwindex].deviceseld3 == BTGNcls.DSeljcType.DB)
                                {
                                    f = NowBTGNCScls.btgns[bwindex].cmd_tddbdlSend(lnk, ed, BTGNcls.TDDDLDYType.TD);
                                }
                                else
                                {
                                    f = NowBTGNCScls.btgns[bwindex].cmd_tddbdlSend(lnk, ed, BTGNcls.TDDDLDYType.DD);
                                }
                                Thread.Sleep(300);
                            }
                            if (f)
                            {
                                result = string.Format("cmd={0},ret=0,data={1};null", key, (bwindex + 1));
                            }
                            else
                            {
                                result = string.Format("cmd={0},ret=1,data={1};null", key, (bwindex + 1));
                            }
                        }
                        break;
                    #endregion

                    #region << 0402 得到台体遥信状态>>
                    case "0402":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = NowBTGNCScls.btgns[bwindex].SendXYCGet(lnk, ed, strdatas);
                            Thread.Sleep(300);
                        }
                        if (f)
                        {
                            result = NowBTGNCScls.btgns[bwindex].XYCValueToOutput(key);
                            Thread.Sleep(300);
                        }
                        else
                        {
                            result = "cmd=0402,ret=1,data=null";
                        }
                        break;
                    #endregion

                    #region << 0403 设置台体遥信状态>>
                    case "0403":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = NowBTGNCScls.btgns[bwindex].SendXYCSet(lnk, ed, strdatas);
                            Thread.Sleep(300);
                        }
                        if (f)
                            result = "cmd=0403,ret=0,data=null";
                        else
                            result = "cmd=0403,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0404 得到台体遥控状态>>  不支持
                    case "0404":
                        //strdatas = data.Split(';');
                        //bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        //lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        //if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        //{
                        //    f = NowBTGNCScls.btgns[bwindex].SendYKCSet(lnk, ed, strdatas);
                        //    Thread.Sleep(300);
                        //}
                        //if (f)
                        //{
                        //    result = NowBTGNCScls.btgns[bwindex].YKCValueToOutput(key);
                        //    Thread.Sleep(300);
                        //}
                        //else
                        //{
                        //    result = "cmd=0404,ret=1,data=null";
                        //}
                        result = "cmd=0404,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0405 设置台体遥控状态>>  不支持
                    case "0405":
                        //result = "cmd=0405,ret=0,data=null";
                        result = "cmd=0405,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0406 台体脉冲设置>>
                    case "0406":
                    case "990406":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8))
                        {
                            lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                            f = NowBTGNCScls.btgns[bwindex].SendMCCSet(lnk, ed, ed2, ed3, strdatas);
                        }
                        Thread.Sleep(300);
                        if (f)
                        {
                            if (key == "0406")
                                result = string.Format("cmd={0},ret=0,data=null", key);
                            else
                                result = string.Format("cmd={0},ret=0,data={1};null", key, (bwindex + 1));
                        }
                        else
                        {
                            if (key == "0406")
                                result = string.Format("cmd={0},ret=1,data=null", key);
                            else
                                result = string.Format("cmd={0},ret=1,data={1};null", key, (bwindex + 1));
                        }
                        break;
                    #endregion

                    #region << 0407 台体脉冲输出>>
                    case "0407":
                    case "990407":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8))
                        {
                            lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                            f = NowBTGNCScls.btgns[bwindex].SendMCCStart(lnk, ed, ed2, strdatas);
                        }
                        Thread.Sleep(300);
                        if (f)
                        {
                            if (key == "0407")
                                result = string.Format("cmd={0},ret=0,data=null", key);
                            else
                                result = string.Format("cmd={0},ret=0,data={1};null", key, (bwindex + 1));
                        }
                        else
                        {
                            if (key == "0407")
                                result = string.Format("cmd={0},ret=1,data=null", key);
                            else
                                result = string.Format("cmd={0},ret=1,data={1};null", key, (bwindex + 1));
                        }
                        break;
                    #endregion

                    #region << 0408 台体脉冲输出停止>>
                    case "0408":
                    case "990408":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        f = NowBTGNCScls.btgns[bwindex].SendMCCEnd(lnk, ed, ed2, strdatas);
                        Thread.Sleep(300);
                        if (f)
                        {
                            if (key == "0408")
                                result = string.Format("cmd={0},ret=0,data=null", key);
                            else
                                result = string.Format("cmd={0},ret=0,data={1};null", key, (bwindex + 1));
                        }
                        else
                        {
                            if (key == "0408")
                                result = string.Format("cmd={0},ret=1,data=null", key);
                            else
                                result = string.Format("cmd={0},ret=1,data={1};null", key, (bwindex + 1));
                        }
                        break;
                    #endregion

                    #region << 0409 电流回路复位>>
                    case "0409":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = NowBTGNCScls.btgns[bwindex].SendTDDDLdown(lnk, ed, strdatas);
                            Thread.Sleep(500);
                            if (f)
                            {
                                f = NowBTGNCScls.btgns[bwindex].SendTDDDLup(lnk, ed, strdatas);
                                Thread.Sleep(500);
                            }
                        }
                        if (f)
                            result = "cmd=0409,ret=0,data=null";
                        else
                            result = "cmd=0409,ret=1,data=null";
                        break;
                    #endregion

                    #region << 040A 电流回路断开>>
                    case "040A":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = NowBTGNCScls.btgns[bwindex].SendTDDDLdown(lnk, ed, strdatas);
                            Thread.Sleep(500);
                        }
                        if (f)
                            result = "cmd=040A,ret=0,data=null";
                        else
                            result = "cmd=040A,ret=1,data=null";
                        break;
                    #endregion

                    #region << 040B 电流回路闭合>>
                    case "040B":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = NowBTGNCScls.btgns[bwindex].SendTDDDLup(lnk, ed, strdatas);
                            Thread.Sleep(500);
                        }
                        if (f)
                            result = "cmd=040B,ret=0,data=null";
                        else
                            result = "cmd=040B,ret=1,data=null";
                        break;
                    #endregion

                    #region << 040C 得到终端RS-485接入状态>>    ???
                    case "040C":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = NowBTGNCScls.btgns[bwindex].GetTD485(bwindex, out sendbytes, out receivestr);
                        }
                        if (f)
                            result = "cmd=040C,ret=0,data=null";
                        else
                            result = "cmd=040C,ret=1,data=null";
                        break;
                    #endregion

                    #region << 040D 设置终端RS-485接入状态>>    ???
                    case "040D":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        _tempdata = Convert.ToInt32(strdatas[1]);
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = NowBTGNCScls.btgns[bwindex].SetTD485(bwindex, _tempdata, out sendbytes, out receivestr);
                            Thread.Sleep(300);
                        }
                        if (f)
                            result = "cmd=040D,ret=0,data=" + receivestr;
                        else
                            result = "cmd=040D,ret=1,data=null";
                        break;
                    #endregion

                    #region<<GP 990901 修改当前表位通信的设备以确定通道选择>>
                    case "990901":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8))
                        {
                            f = NowBTGNCScls.btgns[bwindex].setDeviceLinktype(strdatas);
                        }
                        if (f)
                            result = "cmd=990901,ret=0,data=" + (bwindex + 1);
                        else
                            result = "cmd=990901,ret=1,data=" + (bwindex + 1);
                        break;
                    #endregion

                    #region<<GP 990902 控制各表位485端口开关, 不是真的关了，在这里只是做个标记，在收数据的时候做个判断>>
                    case "990902":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8))
                        {
                            lnk = getSocketClient(TCPClientChannelTypeOptions.LinkJ1Z3, bwindex);
                            if (lnk != null)
                            {
                                this.server.TCPClientStart(lnk);
                                if (strdatas[1] == "0")
                                {
                                    lnk.setcb485OK(false);
                                    //this.server.TCPClientStop(lnk);
                                }
                                else
                                {
                                    lnk.setcb485OK(true);
                                    //this.server.TCPClientStart(lnk);
                                }
                            }
                            lnk = getSocketClient(TCPClientChannelTypeOptions.LinkZ1C2, bwindex);
                            if (lnk != null)
                            {
                                this.server.TCPClientStart(lnk);
                                if (strdatas[2] == "0")
                                {
                                    lnk.setcb485OK(false);
                                    //this.server.TCPClientStop(lnk);
                                }
                                else
                                {
                                    lnk.setcb485OK(true);
                                    //this.server.TCPClientStart(lnk);
                                }
                            }
                            lnk = getSocketClient(TCPClientChannelTypeOptions.LinkJ2C1, bwindex);
                            if (lnk != null)
                            {
                                this.server.TCPClientStart(lnk);
                                if (strdatas[3] == "0")
                                {
                                    lnk.setcb485OK(false);
                                    //this.server.TCPClientStop(lnk);
                                }
                                else
                                {
                                    lnk.setcb485OK(true);
                                    //this.server.TCPClientStart(lnk);
                                }
                            }
                        }
                        //if (f)
                        result = string.Format("cmd=990902,ret=0,data={0}", (bwindex + 1));
                        //else
                        //    result = string.Format("cmd=990902,ret=1,data={0}",(bwindex + 1));
                        break;
                    #endregion

                    #region << 0412 停止计时误差测试,清空结果>>
                    case "0412":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        r = NowDSPOutputValue.WCB_CLD_Clear(bwindex + 1);

                        if (r == -1)
                            result = "cmd=0412,ret=0,data=null";
                        else
                            result = "cmd=0412,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0411 设置计时误差参数>>
                    case "0411":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        NowDSPOutputValue.WCBSJsetInputToValue(strdatas, (bwindex + 1));
                        r = NowDSPOutputValue.WCB_SJStart(bwindex + 1);

                        if (r == -1)
                        {
                            result = "cmd=0411,ret=0,data=null";
                        }
                        else
                            result = "cmd=0411,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0413 读取误差表>>     XXX
                    case "0413":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8))
                        {
                            result = NowDSPOutputValue.WCBSJValueToOutput("0413", (bwindex + 1));
                        }
                        break;
                    #endregion

                    #region <<  1001 给各表位终端发送规约数据>>
                    case "1001":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8))
                        {
                            f = NowBTGNCScls.btgns[bwindex].SendZF(bwindex, ed, strdatas);
                        }

                        if (f)
                            result = "cmd=1001,ret=0,data=" + (bwindex + 1) + ";" + AC.Base.Function.ByteArrayToHex(ed.backdatas);
                        else
                            result = "cmd=1001,ret=1,data=null";
                        break;
                    #endregion

                    #region <<  1002 各表位终端抄读模拟电表数据>>
                    #endregion

                    #region << 990400 电压电流回路断开>>
                    case "990400":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = getSocketClient(TCPClientChannelTypeOptions.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = NowBTGNCScls.btgns[bwindex].cmd_tdddlSend(lnk, ed, BTGNcls.TDDDLDYType.DD);
                            Thread.Sleep(500);
                            f = NowBTGNCScls.btgns[bwindex].cmd_tddbdlSend(lnk, ed, BTGNcls.TDDDLDYType.DD);
                            Thread.Sleep(500);

                            if (Convert.ToInt32(strdatas[1]) == 0)
                            {
                                f = NowBTGNCScls.btgns[bwindex].cmd_tddSend(lnk, ed, BTGNcls.TDDType.DD, 0xFF);
                            }
                            else
                            {
                                if (Convert.ToInt32(strdatas[2]) == 0)
                                    f = NowBTGNCScls.btgns[bwindex].cmd_tddSend(lnk, ed, BTGNcls.TDDType.TD, 0xFF);
                                else
                                {
                                    f = NowBTGNCScls.btgns[bwindex].cmd_tddSend(lnk, ed, BTGNcls.TDDType.TD, 2);
                                    Thread.Sleep(500);
                                    f = NowBTGNCScls.btgns[bwindex].cmd_tddSend(lnk, ed, BTGNcls.TDDType.TD, 3);
                                }
                            }
                            Thread.Sleep(500);
                        }
                        break;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                OnMessage(ChannelMessageTypeOptions.MOXAError, "报文格式不正确！" + ex.ToString());
                TXTWrite.WriteERRTxt("ExecThread", "exec", "", "", ex.ToString());
            }
            finally
            {
                try
                {
                    if ((result != string.Empty) && (server != null))
                    {
                        server.UdpSend(result, ed);
                    }
                }
                catch (Exception ex)
                {
                    OnMessage(ChannelMessageTypeOptions.MOXAError, "UDP返回数据发送失败！" + ex.ToString());
                    TXTWrite.WriteERRTxt("ExecThread", "exec", "", "", ex.ToString());
                }
            }
        }
    }
    #endregion
}

