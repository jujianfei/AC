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

namespace MainForm
{
    /// <summary>
    /// 上行监听 1,未发现通信端口，2，通信超时
    /// </summary>
    public class Channel
    {
        public Channel()
        {
            
        }

        private const int BUFFERSIZE = 1024; //最大接收字节长度
        private byte[] buffer = new byte[BUFFERSIZE];
        public int delay = 30;//超时时间 单位：100ms
        public ConcurrentDictionary<string, SocketAsyncEventArgs> m_Onlines;  //在线终端
        //private ReaderWriterLock rwLock = new ReaderWriterLock();

        #region << 需要在任务类中处理 >>
        private ExecALLThread execallthread;
        //向下MOXA口
        public ConcurrentDictionary<EndPoint, MOXAEndPoint> m_Clients;
        //监听到的UDP
        public List<EndPoint> udpUPeplist = new List<EndPoint>(); 
        #endregion

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
        public void OnMessage(ChannelMessageTypeOptions MessageType, string Message)
        {
            if (this.EventMessage != null)
            {
                this.EventMessage(MessageType, Message);
            }
        }
        #endregion

        #region << udp 通道参数 >>
        /// 获取或设置后台服务监听的端口。如果该值小于等于 0 则表示不启动后台服务。
        public int udpPort = 20000;
        private EndPoint udpIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 60000);
        private Socket udpReceivedSocket;//udp 接收socket
        private SocketAsyncEventArgs udpReceivedSocketAsync;//接收数据使用的异步套接字
        private ConcurrentStack<SocketAsyncEventArgs> udpSendSocketAsyncStack;//发送数据使用的异步套接字栈
        //private ConcurrentDictionary<string, string> qzj_Result;
        #endregion

        #region << tcp 通道对象 >>
        private int tcpPort = 0;
        private string tcpIp = "127.0.0.1";
        protected Socket m_SocketTcpServer;
        internal ConcurrentDictionary<int, byte[]> m_Result;
        #endregion

        #region << 套接字操作 Start & Stop >>
        //打开通道服务后台监听
        public void Start(ApplicationClass m_Application)
        {
            if (this.State != ChannelStateOptions.Running)
            {
                try
                {
                    this.State = ChannelStateOptions.Running;
                    this.m_Clients = new ConcurrentDictionary<EndPoint, MOXAEndPoint>();

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
                    if (udpPort > 0)
                    {
                        //this.qzj_Result = new ConcurrentDictionary<string, string>();
                        this.udpReceivedSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        this.udpReceivedSocket.Bind(new IPEndPoint(System.Net.IPAddress.Any, udpPort));
                        this.udpReceivedSocketAsync = new SocketAsyncEventArgs();
                        this.udpReceivedSocketAsync.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        this.udpReceivedSocketAsync.Completed += new EventHandler<SocketAsyncEventArgs>(udpReceivedSocketAsync_Completed);
                        this.udpReceivedSocketAsync.SetBuffer(this.buffer, 0, BUFFERSIZE);
                        this.udpSendSocketAsyncStack = new ConcurrentStack<SocketAsyncEventArgs>();
                        this.udpReceiveFrom();
                        this.OnMessage(ChannelMessageTypeOptions.Importance, string.Format("Udp service 已经启动 , 监听[{0}:{1}]", this.tcpIp, this.udpPort));
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    if (m_SocketTcpServer != null)
                        m_SocketTcpServer.Close();

                    if (udpReceivedSocket != null)
                        udpReceivedSocket.Close();

                    this.OnMessage(ChannelMessageTypeOptions.Error, "通道服务启动失败，" + ex.Message);
                    this.State = ChannelStateOptions.Stopped;
                }
                finally
                {
                    execallthread = new ExecALLThread(this);
                    execallthread.InitValue(m_Application);
                }
            }
        }

        /// <summary>
        /// 停止通道。该方法由通道服务程序或前置机程序作为宿主进行调用，调用该方法后通道应首先关闭设备端服务断开所有设备的连接，然后通知所有已连接的后台自身即将关闭的消息并关闭后台服务。对于通道安全运行的多种形式，如主备通道或多主通道，则由具体实现完成该功能。
        /// </summary>
        public void Stop()
        {
            this.State = ChannelStateOptions.Stopped;
            if (this.m_Onlines != null)
            {
                foreach (KeyValuePair<string, SocketAsyncEventArgs> kvp in this.m_Onlines)
                    this.SocketAsyncFinish(kvp.Value);
            }

            if (this.m_Onlines != null)
            {
                foreach (KeyValuePair<string, SocketAsyncEventArgs> kvp in this.m_Onlines)
                    this.SocketAsyncFinish(kvp.Value);
            }

            if (this.m_SocketTcpServer != null)
                this.m_SocketTcpServer.Close();
            if (this.udpReceivedSocket != null)
                this.udpReceivedSocket.Close();

            this.OnMessage(ChannelMessageTypeOptions.Importance, "台体服务通道已被关闭，所有链接都被释放。");
        }


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
                    this.OnMessage(ChannelMessageTypeOptions.Error, args.SocketError.ToString());
                    this.SocketAsyncFinish(args);
                }
            }
            catch (Exception e)
            {
                this.OnMessage(ChannelMessageTypeOptions.Error, e.Message);
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
                    this.OnMessage(ChannelMessageTypeOptions.Message1,  ByteArrayToHex(_temp));

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
                                    return;
                                }
                                else
                                {
                                    #region << 登陆/心跳 发回确认帧 >>
                                    if (_temp[16] == 1)
                                    {
                                        string Addr = _temp[8].ToString("X2") + _temp[7].ToString("X2") + (_temp[9] + _temp[10] * 0x100).ToString().PadLeft(5, '0');
                                        this.OnMessage(ChannelMessageTypeOptions.Message1, "[登录]" + ByteArrayToHex(_temp) + "[" + Addr + "]");
                                        args.UserToken = Addr;

                                        if (!this.m_Onlines.TryAdd(Addr, args))
                                        {
                                            SocketAsyncEventArgs _args;
                                            this.m_Onlines.TryRemove(Addr, out _args);
                                            this.SocketAsyncFinish(_args);
                                            this.m_Onlines.TryAdd(Addr, args);
                                        }
                                    }
                                    else
                                        this.OnMessage(ChannelMessageTypeOptions.Message1, "[心跳]" + ByteArrayToHex(_temp));

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

                                    this.OnMessage(ChannelMessageTypeOptions.Message2, "[确认]" + ByteArrayToHex(_b));
                                    args.AcceptSocket.Send(_b);
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
                            this.OnMessage(ChannelMessageTypeOptions.Message1, "[登录]" + ByteArrayToHex(_temp) + "[" + _temp[13] + "]");
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

            MOXAEndPoint moxaendpoint = null;
            this.m_Clients.TryRemove(args.AcceptSocket.RemoteEndPoint, out moxaendpoint);

            if (moxaendpoint != null)
            {
                if (moxaendpoint.link != null)
                {
                    if (moxaendpoint.link.AcceptSocket != null)
                        args.AcceptSocket.Close();
                    args.Dispose();
                }
            }
            //this.m_Onlines.TryRemove(args.UserToken.ToString(), out args);
            //断线操作
        }
        #endregion

        #region << udp 通道回调 >>

        private void args_Completed(object sender, SocketAsyncEventArgs args)
        {
            this.udpSendSocketAsyncStack.Push(args);
        }

        private void udpReceivedSocketAsync_Completed(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                if (args.BytesTransferred > 0)
                {
                    byte[] data = new byte[args.BytesTransferred];
                    Array.Copy(args.Buffer, args.Offset, data, 0, data.Length);

                    string s1 = Encoding.Default.GetString(data, 0, data.Length);
                    string[] ss = s1.Split(',')[0].Split('=');
                    string cmd = "";
                    if (ss.Length > 1)
                        cmd = ss[1];

                    //if (!qzj_Result.TryAdd(cmd, s1))
                    //{
                    //    string s2= null;
                    //    qzj_Result.TryRemove(cmd, out s2);
                    //    qzj_Result.TryAdd(cmd, s1);
                    //}
                    this.OnMessage(ChannelMessageTypeOptions.Importance, cmd);
                    this.OnMessage(ChannelMessageTypeOptions.Importance, ((IPEndPoint)args.RemoteEndPoint).Port.ToString());
                    this.OnMessage(ChannelMessageTypeOptions.Importance, Encoding.Default.GetString(data, 0, data.Length));
                }
            }
            else
            {
                this.OnMessage(ChannelMessageTypeOptions.Importance, "收到的系统消息数据异步套接字结果是 " + args.SocketError.ToString() + "，数据长度为 " + args.BytesTransferred.ToString() + " 字节。");
            }
            this.udpReceiveFrom();
        }

        private void udpReceiveFrom()
        {
            if (this.udpReceivedSocket.ReceiveFromAsync(udpReceivedSocketAsync) == false)
            {
                throw new Exception("开始接收系统消息时，I/O 操作同步完成。");
            }
        }

        #endregion

        #region << tcp client 接收数据 >>
        private void SocClientAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs args)
        {
            if (args.LastOperation == SocketAsyncOperation.Receive)
                this.SocClientBackBeginReceive(args); 
        }

        private void SocClientBackBeginReceive(SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                try
                {
                    byte[] _temp = new byte[args.BytesTransferred];
                    Array.Copy(args.Buffer, args.Offset, _temp, 0, _temp.Length);
                    MOXAEndPoint ut = args.UserToken as MOXAEndPoint;
                    if (ut.PortType == TTDeviceType.Link2321)//这里判断
                    {
                        
                    }
                    this.OnMessage(ChannelMessageTypeOptions.Message1, ByteArrayToHex(_temp));
                }
                catch
                { }
                finally
                {
                    if (args.AcceptSocket.ReceiveAsync(args) == false)
                        this.SocClientBackBeginReceive(args);
                }
            }
            else
                this.SocClientSocketAsyncFinish(args);
        }

        /// <summary>
        /// 当前socket连接终止
        /// </summary>
        /// <param name="args"></param>
        protected void SocClientSocketAsyncFinish(SocketAsyncEventArgs args)
        {
            args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocketAsyncEventArgs_Completed);

            MOXAEndPoint moxaendpoint = null;
            this.m_Clients.TryRemove(args.AcceptSocket.RemoteEndPoint, out moxaendpoint);

            if (moxaendpoint != null)
            {
                if (moxaendpoint.link != null)
                {
                    if (moxaendpoint.link.AcceptSocket != null)
                        args.AcceptSocket.Close();
                    args.Dispose();
                }
            }
            //this.m_Onlines.TryRemove(args.UserToken.ToString(), out args);
            //断线操作
        }
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

        #endregion

        #region << 字符串转换 >>
        /// <summary>
        /// 16进制字符串转换byte[]
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexToByteArray(string s)
        {
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary>
        /// byte[]转换16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToHex(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().Trim().ToUpper();
        }
        #endregion

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
                        return ByteArrayToHex(_d);
                }
                return "2";//超时没回
            }
            return "1";//没发现终端
        }

        /// <summary>
        /// udp 发送主台设置命令
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string Send(string s)
        {
            SocketAsyncEventArgs args = null;
            string cmd = s.Split(',')[0].Split('=')[1];
            this.udpSendSocketAsyncStack.TryPop(out args);
            if (args == null)
                args = new SocketAsyncEventArgs();
            args.Completed+=new EventHandler<SocketAsyncEventArgs>(args_Completed);

            byte[] d = Encoding.ASCII.GetBytes(s);
            args.RemoteEndPoint = udpIp;
            args.SetBuffer(d, 0, d.Length);
            this.udpReceivedSocket.SendToAsync(args);

            string sss = null;
            //this.qzj_Result.TryRemove(cmd, out sss);

            int i = 0;
            while (i < delay)
            {
                Thread.Sleep(100);
                string ss = null;
                //this.qzj_Result.TryGetValue(cmd, out ss);
                if (ss != null)
                    return ss;
                i++;
            }
            return "2";//超时没回
        }

        public string Send(EndPoint ep , string s)
        {
            MOXAEndPoint moxaep = null;
            SocketAsyncEventArgs args = null;
            argsUserToken token = null;
            this.m_Clients.TryGetValue(ep, out moxaep);
            args = moxaep.link;
            if (args == null)
            {
                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                args.RemoteEndPoint = ep;
                try
                {
                    soc.ConnectAsync(args);
                }
                catch
                {
                    args.Completed -= new EventHandler<SocketAsyncEventArgs>(SocClientAsyncEventArgs_Completed);
                    args = null;
                    soc = null;
                    return "1";
                }
                token = new argsUserToken();
                token.ChannelType = ChannelTypeOptions.Rtu;
                args.UserToken = token;//绑定用户数据

                Thread.Sleep(50);
                byte[] bytBuffer = new byte[1024];
                args.SetBuffer(bytBuffer, 0, bytBuffer.Length);
                Thread.Sleep(50);
                args.AcceptSocket = soc;
                //this.m_Clients.TryAdd(ep, args);

                if (soc.ReceiveAsync(args) == false)
                {
                    this.BackBeginReceive(args);
                }
            }

            if (token == null)
                token = args.UserToken as argsUserToken;
  
            token.reply = null;
            args.AcceptSocket.Send(Encoding.ASCII.GetBytes(s));

            int i = 0;
            while (i < delay)
            {
                Thread.Sleep(100);
                if(token.reply != null)
                    return ByteArrayToHex(token.reply);
                i++;
            }

            return "2";
        }

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
                if (ed == null)
                {
                    foreach (EndPoint udpupep in udpUPeplist)
                    {
                        //this.OnMessage(MessageTypeOptions.ZHUZANBAOWEN, string.Format("发送数据报文[{0}]", buffer));
                        this.udpReceivedSocket.SendTo(Encoding.ASCII.GetBytes(buffer), udpupep);
                    }
                }
                else
                {
                    //this.OnMessage(MessageTypeOptions.ZHUZANBAOWEN, string.Format("发送数据报文[{0}]", buffer));
                    this.udpReceivedSocket.SendTo(Encoding.ASCII.GetBytes(buffer), ed.upep);
                }
            }
            catch (Exception ex)
            {

            }
        }
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
        /// 接收报文
        /// </summary>
        Message1,
        /// <summary>
        /// 发送报文
        /// </summary>
        Message2,
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
                case ChannelMessageTypeOptions.Message1:
                    return "接收报文";
                case ChannelMessageTypeOptions.Message2:
                    return "发送报文";
                default:
                    throw new NotImplementedException("尚未实现该枚举。");
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
    public enum ChannelTypeOptions
    {
        /// <summary>
        /// 集中器、终端
        /// </summary>
        Rtu,
        /// <summary>
        /// 新联自制作板
        /// </summary>
        XLCard
    }
    #endregion

    #region << argsUserToken 用户数据 >>
    /// <summary>
    /// socket 用户数据类
    /// </summary>
    public class argsUserToken
    {
        public byte[] reply = null;

        public ChannelTypeOptions ChannelType = ChannelTypeOptions.XLCard;
        /// <summary>
        /// 获取最后收发数据的时间。
        /// </summary>
        public DateTime LastReceivedTime = DateTime.MinValue;
        /// <summary>
        /// 接收数据缓存，用来拼帧操作
        /// </summary>
        public byte[] cache = new byte[2048];
    }
    #endregion

    public enum TTDeviceType
    {
        Unknow = 0,
        LinkDJ = 1,
        LinkBZ = 2,
        LinkPM = 3,
        LinkJ1Z3 = 4,
        LinkZ1C2 = 5,
        LinkJ2C1 = 6,
        Link2321 = 7,
        Link2322 = 8
    }
    class MOXAEndPoint
    {
        /// <summary>
        /// IP地址
        /// </summary>
        private string IP { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        private int Port { get; set; }

        /// <summary>
        /// 管理端口
        /// </summary>
        private int mgrPort { get; set; }

        /// <summary>
        /// 端口名称
        /// </summary>
        private string PortName { get; set; }

        /// <summary>
        /// 端口ID名称
        /// </summary>
        private string PortIDName { get; set; }

        /// <summary>
        /// 端口类型
        /// </summary>
        public TTDeviceType PortType { get; set; }

        public EndPoint ep;
        public SocketAsyncEventArgs link;
        public MOXAEndPoint(string _ip, int _port, int _mgrport, string _name, int _id, TTDeviceType _type)
        {
            IP= _ip;
            Port = _port;
            mgrPort = _mgrport;
            PortName = _name;
            PortIDName = "[" + _ip + ":" + _id + "]";
            PortType = _type;
            try
            {
                ep = (EndPoint)new IPEndPoint(IPAddress.Parse(IP), Port);
            }
            catch (Exception ex)
            {
                ep = null;
            }
            link = null;
        }
    }
    public class ExecData
    {
        public string data = "";
        public EndPoint upep = null;
        List<byte> TempBuffer = new List<byte>();
        public ExecData(string _data, EndPoint _ep)
        {
            data = _data;
            upep = _ep;
        }

    }
    class ExecALLThread
    {
        public ExecALLThread(Channel _server)
        {
            server = _server;
            //if (server != null)
            //    EventMessage += server.OnMessage;
        }

        //~ExecALLThread()
        //{
        //    if (server != null)
        //        EventMessage -= server.OnMessage;
        //}

        public Channel server;
        #region << 通道服务产生消息的事件 >>
        /// <summary>
        /// 通道服务产生的消息事件。
        /// </summary>
        //public event MessageEventHandler EventMessage;

        /// <summary>
        /// 通道服务产生的消息事件所调用的委托。
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="message">消息内容。</param>
        //public delegate void MessageEventHandler(ChannelMessageTypeOptions messageType, string message);

        /// <summary>
        /// 引发通道服务产生消息事件
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="message">消息内容。</param>
        //protected void OnMessage(ChannelMessageTypeOptions messageType, string message)
        //{
        //    if (this.EventMessage != null)
        //    {
        //        this.EventMessage(messageType, message);
        //    }
        //}
        #endregion
        private void OnMessage(ChannelMessageTypeOptions messageType, string message)
        {
            if (server != null)
                server.OnMessage(messageType, message);
        }

        #region << 初始化系统通道 >>
        public DSPOutputValueCls NowDSPOutputValue = null;
        public BTGNCScls NowBTGNCScls = null;
        public PMShowScls NowPMShowScls = null;

        #region 通道地址
        private byte[] LinkDJ = new byte[] { 0, 1 };
        private byte[] LinkPM = new byte[] { 0, 2 };
        private byte[] LinkBZ = new byte[] { 0, 3 };

        private byte[] LinkBW1J1Z3 = new byte[] { 0, 4 };
        private byte[] LinkBW2J1Z3 = new byte[] { 0, 9 };
        private byte[] LinkBW3J1Z3 = new byte[] { 0, 14 };
        private byte[] LinkBW4J1Z3 = new byte[] { 1, 3 };
        private byte[] LinkBW5J1Z3 = new byte[] { 1, 8 };
        private byte[] LinkBW6J1Z3 = new byte[] { 1, 13 };
        private byte[] LinkBW7J1Z3 = new byte[] { 2, 2 };
        private byte[] LinkBW8J1Z3 = new byte[] { 2, 7 };

        private byte[] LinkBW1Z1C2 = new byte[] { 0, 5 };
        private byte[] LinkBW2Z1C2 = new byte[] { 0, 10 };
        private byte[] LinkBW3Z1C2 = new byte[] { 0, 15 };
        private byte[] LinkBW4Z1C2 = new byte[] { 1, 4 };
        private byte[] LinkBW5Z1C2 = new byte[] { 1, 9 };
        private byte[] LinkBW6Z1C2 = new byte[] { 1, 14 };
        private byte[] LinkBW7Z1C2 = new byte[] { 2, 3 };
        private byte[] LinkBW8Z1C2 = new byte[] { 2, 8 };

        private byte[] LinkBW1J2C1 = new byte[] { 0, 6 };
        private byte[] LinkBW2J2C1 = new byte[] { 0, 11 };
        private byte[] LinkBW3J2C1 = new byte[] { 0, 16 };
        private byte[] LinkBW4J2C1 = new byte[] { 1, 5 };
        private byte[] LinkBW5J2C1 = new byte[] { 1, 10 };
        private byte[] LinkBW6J2C1 = new byte[] { 1, 15 };
        private byte[] LinkBW7J2C1 = new byte[] { 2, 4 };
        private byte[] LinkBW8J2C1 = new byte[] { 2, 9 };

        private byte[] LinkBW12321 = new byte[] { 0, 7 };
        private byte[] LinkBW22321 = new byte[] { 0, 12 };
        private byte[] LinkBW32321 = new byte[] { 1, 1 };
        private byte[] LinkBW42321 = new byte[] { 1, 6 };
        private byte[] LinkBW52321 = new byte[] { 1, 11 };
        private byte[] LinkBW62321 = new byte[] { 1, 16 };
        private byte[] LinkBW72321 = new byte[] { 2, 5 };
        private byte[] LinkBW82321 = new byte[] { 2, 10 };

        private byte[] LinkBW12322 = new byte[] { 0, 8 };
        private byte[] LinkBW22322 = new byte[] { 0, 13 };
        private byte[] LinkBW32322 = new byte[] { 1, 2 };
        private byte[] LinkBW42322 = new byte[] { 1, 7 };
        private byte[] LinkBW52322 = new byte[] { 1, 12 };
        private byte[] LinkBW62322 = new byte[] { 2, 1 };
        private byte[] LinkBW72322 = new byte[] { 2, 6 };
        private byte[] LinkBW82322 = new byte[] { 2, 11 };

        private string[] moxaip = new string[] { "192.168.127.101", "192.168.127.102", "192.168.127.103" };
        #endregion
        public MOXAEndPoint getSocketClient(TTDeviceType ttdy, int index)
        {
            MOXAEndPoint rsc = null;
            if (server == null)
                return rsc;

            EndPoint _ep;
            if (ttdy == TTDeviceType.LinkDJ)
            {
                _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkDJ[0]]), 4000 + LinkDJ[1]);
                server.m_Clients.TryGetValue(_ep, out rsc);
            }
            else if (ttdy == TTDeviceType.LinkBZ)
            {
                _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBZ[0]]), 4000 + LinkBZ[1]);
                server.m_Clients.TryGetValue(_ep, out rsc);
            }
            else if (ttdy == TTDeviceType.LinkPM)
            {
                _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkPM[0]]), 4000 + LinkPM[1]);
                server.m_Clients.TryGetValue(_ep, out rsc);
            }
            else if (ttdy == TTDeviceType.Link2321)
            {
                #region Link2321
                if (index == 0)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW12321[0]]), 4000 + LinkBW12321[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc); 
                }
                else if (index == 1)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW22321[0]]), 4000 + LinkBW22321[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);        
                }
                else if (index == 2)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW32321[0]]), 4000 + LinkBW32321[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc); 
                }
                else if (index == 3)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW42321[0]]), 4000 + LinkBW42321[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc); 
                }
                else if (index == 4)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW52321[0]]), 4000 + LinkBW52321[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc); 
                }
                else if (index == 5)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW62321[0]]), 4000 + LinkBW62321[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc); 
                }
                else if (index == 6)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW72321[0]]), 4000 + LinkBW72321[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc); 
                }
                else if (index == 7)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW82321[0]]), 4000 + LinkBW82321[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc); 
                }
                #endregion
            }
            else if (ttdy == TTDeviceType.Link2322)
            {
                #region Link2322
                if (index == 0)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW12322[0]]), 4000 + LinkBW12322[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 1)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW22322[0]]), 4000 + LinkBW22322[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 2)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW32322[0]]), 4000 + LinkBW32322[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 3)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW42322[0]]), 4000 + LinkBW42322[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 4)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW52322[0]]), 4000 + LinkBW52322[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 5)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW62322[0]]), 4000 + LinkBW62322[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 6)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW72322[0]]), 4000 + LinkBW72322[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 7)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW82322[0]]), 4000 + LinkBW82322[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                #endregion
            }
            else if (ttdy == TTDeviceType.LinkJ1Z3)
            {
                #region LinkJ1Z3
                if (index == 0)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW1J1Z3[0]]), 4000 + LinkBW1J1Z3[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 1)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW2J1Z3[0]]), 4000 + LinkBW2J1Z3[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 2)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW3J1Z3[0]]), 4000 + LinkBW3J1Z3[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 3)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW4J1Z3[0]]), 4000 + LinkBW4J1Z3[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 4)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW5J1Z3[0]]), 4000 + LinkBW5J1Z3[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 5)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW6J1Z3[0]]), 4000 + LinkBW6J1Z3[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 6)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW7J1Z3[0]]), 4000 + LinkBW7J1Z3[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 7)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW8J1Z3[0]]), 4000 + LinkBW8J1Z3[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                #endregion
            }
            else if (ttdy == TTDeviceType.LinkZ1C2)
            {
                #region LinkZ1C2
                if (index == 0)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW1Z1C2[0]]), 4000 + LinkBW1Z1C2[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 1)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW2Z1C2[0]]), 4000 + LinkBW2Z1C2[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 2)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW3Z1C2[0]]), 4000 + LinkBW3Z1C2[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 3)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW4Z1C2[0]]), 4000 + LinkBW4Z1C2[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 4)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW5Z1C2[0]]), 4000 + LinkBW5Z1C2[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 5)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW6Z1C2[0]]), 4000 + LinkBW6Z1C2[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 6)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW7Z1C2[0]]), 4000 + LinkBW7Z1C2[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 7)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW8Z1C2[0]]), 4000 + LinkBW8Z1C2[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                #endregion
            }
            else if (ttdy == TTDeviceType.LinkJ2C1)
            {
                #region LinkJ2C1
                if (index == 0)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW1J2C1[0]]), 4000 + LinkBW1J2C1[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 1)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW2J2C1[0]]), 4000 + LinkBW2J2C1[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 2)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW3J2C1[0]]), 4000 + LinkBW3J2C1[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 3)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW4J2C1[0]]), 4000 + LinkBW4J2C1[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 4)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW5J2C1[0]]), 4000 + LinkBW5J2C1[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 5)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW6J2C1[0]]), 4000 + LinkBW6J2C1[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 6)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW7J2C1[0]]), 4000 + LinkBW7J2C1[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                else if (index == 7)
                {
                    _ep = (EndPoint)new IPEndPoint(IPAddress.Parse(moxaip[LinkBW8J2C1[0]]), 4000 + LinkBW8J2C1[1]);
                    server.m_Clients.TryGetValue(_ep, out rsc);
                }
                #endregion
            }
            return rsc;
        }

        public void InitValue(ApplicationClass m_application)
        {
            server.m_Clients = new ConcurrentDictionary<EndPoint, MOXAEndPoint>();
            MOXAEndPoint moxaep;
            #region 电机，板子，屏幕
            moxaep = new MOXAEndPoint(moxaip[LinkDJ[0]], 4000 + LinkDJ[1], 965 + LinkDJ[1], "台体电机通信口", LinkDJ[1], TTDeviceType.LinkDJ);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBZ[0]], 4000 + LinkBZ[1], 965 + LinkBZ[1], "台体控制通信口", LinkBZ[1], TTDeviceType.LinkBZ);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkPM[0]], 4000 + LinkPM[1], 965 + LinkPM[1], "台体屏幕通信口", LinkPM[1],TTDeviceType.LinkPM);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);
            #endregion
            #region 集中器1/专变3 485
            moxaep = new MOXAEndPoint(moxaip[LinkBW1J1Z3[0]], 4000 + LinkBW1J1Z3[1], 965 + LinkBW1J1Z3[1], "表位1[集1专3]485口", LinkBW1J1Z3[1], TTDeviceType.LinkJ1Z3);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW2J1Z3[0]], 4000 + LinkBW2J1Z3[1], 965 + LinkBW2J1Z3[1], "表位2[集1专3]485口", LinkBW2J1Z3[1], TTDeviceType.LinkJ1Z3);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW3J1Z3[0]], 4000 + LinkBW3J1Z3[1], 965 + LinkBW3J1Z3[1], "表位3[集1专3]485口", LinkBW3J1Z3[1], TTDeviceType.LinkJ1Z3);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW4J1Z3[0]], 4000 + LinkBW4J1Z3[1], 965 + LinkBW4J1Z3[1], "表位4[集1专3]485口", LinkBW4J1Z3[1], TTDeviceType.LinkJ1Z3);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW5J1Z3[0]], 4000 + LinkBW5J1Z3[1], 965 + LinkBW5J1Z3[1], "表位5[集1专3]485口", LinkBW5J1Z3[1], TTDeviceType.LinkJ1Z3);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW6J1Z3[0]], 4000 + LinkBW6J1Z3[1], 965 + LinkBW6J1Z3[1], "表位6[集1专3]485口", LinkBW6J1Z3[1], TTDeviceType.LinkJ1Z3);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW7J1Z3[0]], 4000 + LinkBW7J1Z3[1], 965 + LinkBW7J1Z3[1], "表位7[集1专3]485口", LinkBW7J1Z3[1], TTDeviceType.LinkJ1Z3);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW8J1Z3[0]], 4000 + LinkBW8J1Z3[1], 965 + LinkBW8J1Z3[1], "表位8[集1专3]485口", LinkBW8J1Z3[1], TTDeviceType.LinkJ1Z3);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);
            #endregion
            #region 专变1/采集器2 485
            moxaep = new MOXAEndPoint(moxaip[LinkBW1Z1C2[0]], 4000 + LinkBW1Z1C2[1], 965 + LinkBW1Z1C2[1], "表位1[专1采2]485口", LinkBW1Z1C2[1], TTDeviceType.LinkZ1C2);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW2Z1C2[0]], 4000 + LinkBW2Z1C2[1], 965 + LinkBW2Z1C2[1], "表位2[专1采2]485口", LinkBW2Z1C2[1], TTDeviceType.LinkZ1C2);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW3Z1C2[0]], 4000 + LinkBW3Z1C2[1], 965 + LinkBW3Z1C2[1], "表位3[专1采2]485口", LinkBW3Z1C2[1], TTDeviceType.LinkZ1C2);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW4Z1C2[0]], 4000 + LinkBW4Z1C2[1], 965 + LinkBW4Z1C2[1], "表位4[专1采2]485口", LinkBW4Z1C2[1], TTDeviceType.LinkZ1C2);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW5Z1C2[0]], 4000 + LinkBW5Z1C2[1], 965 + LinkBW5Z1C2[1], "表位5[专1采2]485口", LinkBW5Z1C2[1], TTDeviceType.LinkZ1C2);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW6Z1C2[0]], 4000 + LinkBW6Z1C2[1], 965 + LinkBW6Z1C2[1], "表位6[专1采2]485口", LinkBW6Z1C2[1], TTDeviceType.LinkZ1C2);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW7Z1C2[0]], 4000 + LinkBW7Z1C2[1], 965 + LinkBW7Z1C2[1], "表位7[专1采2]485口", LinkBW7Z1C2[1], TTDeviceType.LinkZ1C2);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW8Z1C2[0]], 4000 + LinkBW8Z1C2[1], 965 + LinkBW8Z1C2[1], "表位8[专1采2]485口", LinkBW8Z1C2[1], TTDeviceType.LinkZ1C2);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);
            #endregion
            #region 集中器2/采集器1 485
            moxaep = new MOXAEndPoint(moxaip[LinkBW1J2C1[0]], 4000 + LinkBW1J2C1[1], 965 + LinkBW1J2C1[1], "表位1[集2采1]485口", LinkBW1J2C1[1], TTDeviceType.LinkJ2C1);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW2J2C1[0]], 4000 + LinkBW2J2C1[1], 965 + LinkBW2J2C1[1], "表位2[集2采1]485口", LinkBW2J2C1[1], TTDeviceType.LinkJ2C1);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW3J2C1[0]], 4000 + LinkBW3J2C1[1], 965 + LinkBW3J2C1[1], "表位3[集2采1]485口", LinkBW3J2C1[1], TTDeviceType.LinkJ2C1);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW4J2C1[0]], 4000 + LinkBW4J2C1[1], 965 + LinkBW4J2C1[1], "表位4[集2采1]485口", LinkBW4J2C1[1], TTDeviceType.LinkJ2C1);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW5J2C1[0]], 4000 + LinkBW5J2C1[1], 965 + LinkBW5J2C1[1], "表位5[集2采1]485口", LinkBW5J2C1[1], TTDeviceType.LinkJ2C1);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW6J2C1[0]], 4000 + LinkBW6J2C1[1], 965 + LinkBW6J2C1[1], "表位6[集2采1]485口", LinkBW6J2C1[1], TTDeviceType.LinkJ2C1);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW7J2C1[0]], 4000 + LinkBW7J2C1[1], 965 + LinkBW7J2C1[1], "表位7[集2采1]485口", LinkBW7J2C1[1], TTDeviceType.LinkJ2C1);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW8J2C1[0]], 4000 + LinkBW8J2C1[1], 965 + LinkBW8J2C1[1], "表位8[集2采1]485口", LinkBW8J2C1[1], TTDeviceType.LinkJ2C1);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);
            #endregion
            #region 2321
            moxaep = new MOXAEndPoint(moxaip[LinkBW12321[0]], 4000 + LinkBW12321[1], 965 + LinkBW12321[1], "表位1[左侧]485口", LinkBW12321[1], TTDeviceType.Link2321);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW22321[0]], 4000 + LinkBW22321[1], 965 + LinkBW22321[1], "表位2[左侧]485口", LinkBW22321[1], TTDeviceType.Link2321);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW32321[0]], 4000 + LinkBW32321[1], 965 + LinkBW32321[1], "表位3[左侧]485口", LinkBW32321[1], TTDeviceType.Link2321);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW42321[0]], 4000 + LinkBW42321[1], 965 + LinkBW42321[1], "表位4[左侧]485口", LinkBW42321[1], TTDeviceType.Link2321);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW52321[0]], 4000 + LinkBW52321[1], 965 + LinkBW52321[1], "表位5[左侧]485口", LinkBW52321[1], TTDeviceType.Link2321);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW62321[0]], 4000 + LinkBW62321[1], 965 + LinkBW62321[1], "表位6[左侧]485口", LinkBW62321[1], TTDeviceType.Link2321);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW72321[0]], 4000 + LinkBW72321[1], 965 + LinkBW72321[1], "表位7[左侧]485口", LinkBW72321[1], TTDeviceType.Link2321);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW82321[0]], 4000 + LinkBW82321[1], 965 + LinkBW82321[1], "表位8[左侧]485口", LinkBW82321[1], TTDeviceType.Link2321);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);
            #endregion
            #region 2322
            moxaep = new MOXAEndPoint(moxaip[LinkBW12322[0]], 4000 + LinkBW12322[1], 965 + LinkBW12322[1], "表位1[右侧]485口", LinkBW12322[1], TTDeviceType.Link2322);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW22322[0]], 4000 + LinkBW22322[1], 965 + LinkBW22322[1], "表位2[右侧]485口", LinkBW22322[1], TTDeviceType.Link2322);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW32322[0]], 4000 + LinkBW32322[1], 965 + LinkBW32322[1], "表位3[右侧]485口", LinkBW32322[1], TTDeviceType.Link2322);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW42322[0]], 4000 + LinkBW42322[1], 965 + LinkBW42322[1], "表位4[右侧]485口", LinkBW42322[1], TTDeviceType.Link2322);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW52322[0]], 4000 + LinkBW52322[1], 965 + LinkBW52322[1], "表位5[右侧]485口", LinkBW52322[1], TTDeviceType.Link2322);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW62322[0]], 4000 + LinkBW62322[1], 965 + LinkBW62322[1], "表位6[右侧]485口", LinkBW62322[1], TTDeviceType.Link2322);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW72322[0]], 4000 + LinkBW72322[1], 965 + LinkBW72322[1], "表位7[右侧]485口", LinkBW72322[1], TTDeviceType.Link2322);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);

            moxaep = new MOXAEndPoint(moxaip[LinkBW82322[0]], 4000 + LinkBW82322[1], 965 + LinkBW82322[1], "表位8[右侧]485口", LinkBW82322[1], TTDeviceType.Link2322);
            server.m_Clients.TryAdd(moxaep.ep, moxaep);
            #endregion

            NowBTGNCScls = m_application.GetSystemConfig(typeof(BTGNCScls)) as BTGNCScls;
            //m_application.SetSystemConfig(NowBTGNCScls);
            for (int i = 0; i < NowBTGNCScls.btgns.Length; i++)
            {
                NowBTGNCScls.btgns[i].server = server;
            }

            NowPMShowScls = new PMShowScls();
            for (int i = 0; i < NowPMShowScls.pmshows.Length; i++)
            {
                NowPMShowScls.pmshows[i].server = server;
            }

            NowDSPOutputValue = m_application.GetSystemConfig(typeof(DSPOutputValueCls)) as DSPOutputValueCls;
        }
        #endregion

        #region  <<任务处理>>
        System.Timers.Timer exectime;

        private void timer_tick(object sender, EventArgs e)
        {
            runexecs();
        }

        //0 非待测设备通信（电源，台体控制），1-8 对应表位待测设备通信
        Dictionary<long, ExecData>[] cmdslist = new Dictionary<long, ExecData>[9];
        ExecThread[] runexecslist = new ExecThread[9];
        private void InitCMDsList()
        {
            for (int i = 0; i < 9; i++)
            {
                Dictionary<long, ExecData> tempcmds = new Dictionary<long, ExecData>();
                cmdslist[i] = tempcmds;

                ExecThread _runexec = null;
                runexecslist[i] = _runexec;
            }
        }
        private void ClearCMDsList()
        {
            for (int i = 0; i < 9; i++)
            {
                cmdslist[i].Clear();
                if (runexecslist[i] != null)
                    runexecslist[i].stopThread();
            }
        }
        private bool addexec(string cmd, EndPoint upudp)
        {
            if (server != null)
            {
                if (server.udpUPeplist.BinarySearch(upudp) < 0)
                {
                    server.udpUPeplist.Add(upudp);
                }
            }

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
            bool f = false;
            int bwindex = 0;
            Dictionary<long, ExecData> _tempcmds = null;
            ExecData _execdata;
            try
            {
                switch (key)
                {
                    case "1001":
                        #region <<  1001 给各表位终端发送规约数据>>
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]);
                        if ((bwindex > 0) && (bwindex < 9))
                        {
                            _tempcmds = cmdslist[bwindex];
                            _execdata = new ExecData(cmd, upudp);
                            _tempcmds.Add(DateTime.Now.Ticks, _execdata);
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
                            _tempcmds = cmdslist[bwindex];
                            _execdata = new ExecData(cmd, upudp);
                            _tempcmds.Add(DateTime.Now.Ticks, _execdata);
                            f = true;
                        }
                        #endregion
                        break;
                    default:
                        strdatas = data.Split(';');
                        _tempcmds = cmdslist[0];
                        _execdata = new ExecData(cmd, upudp);
                        _tempcmds.Add(DateTime.Now.Ticks, _execdata);
                        f = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("ExecALLThread", "addexec", "", "", ex.ToString());
            }
            return f;
        }
        private void runexecs()
        {
            bool f = false;
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    Dictionary<long, ExecData> _cmds = cmdslist[i];
                    ExecThread _execrun = runexecslist[i];
                    if (_cmds.Count > 0)
                    {
                        if (_execrun == null)
                        {
                            foreach (KeyValuePair<long, ExecData> kv in _cmds)
                            {
                                _execrun = new ExecThread(server, i, kv.Key, kv.Value);
                                runexecslist[i] = _execrun;
                                break;
                            }
                            _cmds.Remove(_execrun.time);
                        }
                        else
                        {
                            if (!_execrun.thread.IsAlive)
                            {
                                foreach (KeyValuePair<long, ExecData> kv in _cmds)
                                {
                                    runexecslist[i].stopThread();
                                    _execrun = new ExecThread(server, i, kv.Key, kv.Value);
                                    runexecslist[i] = _execrun;
                                    break;
                                }
                                _cmds.Remove(_execrun.time);
                            }
                        }
                        if (_cmds.Count > 0)
                        {
                            f = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("ExecALLThread", "runexecs", "", "", ex.ToString());
            }
            finally
            {
                if (!f)
                {
                    exectime.Stop();
                    exectime.Enabled = false;
                }
                else
                {
                    exectime.Start();
                    exectime.Enabled = true;
                }
            }
        }
        #endregion

    }
    class ExecThread
    {
        public ExecThread(ExecALLThread _execallthread, int _index, long _time, ExecData _ed)
        {
            execallthread = _execallthread;
            index = _index;
            time = _time;
            upep = _ed.upep;
            cmd = _ed.data;
            thread = new Thread(exec);
            thread.Start();
        }

        public int index;
        public EndPoint upep;
        public long time;
        public string cmd;
        string result = string.Empty;
        public Thread thread;
        ExecALLThread execallthread;

        public void stopThread()
        {
            if (thread != null)
            {
                thread.Abort();
            }
        }
        private void exec()
        {
            try
            {
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
                byte[] sendbytes, receivebytes = null;
                byte[] sendbytes2, receivebytes2 = null;
                string receivestr = String.Empty;
                SocketClient lnk = null;
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
                        f = execallthread.NowBTGNCScls.btgns[bwindex].KGTDtoK(strdatas, out sendbytes, out receivestr);
                        result = "cmd=0102,ret=0,data=null";
                        //result = "cmd=0102,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0103 关闭台体多路服务器串口 >>  ???
                    case "0103":
                        strdatas = data.Split(';');
                        f = execallthread.NowBTGNCScls.btgns[bwindex].KGTDtoG(strdatas, out sendbytes, out receivestr);
                        result = "cmd=0103,ret=0,data=null";
                        //result = "cmd=0103,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0201 台体升源（常用） >>
                    case "0201":
                        strdatas = data.Split(';');
                        execallthread.NowDSPOutputValue.DJupCInputToValue(strdatas);
                        r = execallthread.NowDSPOutputValue.SetOperationHC_Fun();
                        Thread.Sleep(100);
                        if (r == -1)
                        {
                            r = execallthread.NowDSPOutputValue.DSPOutput();
                        }
                        Thread.Sleep(100);
                        if (r == -1)
                        {
                            r = execallthread.NowDSPOutputValue.OperationHC_Fun();
                        }
                        Thread.Sleep(100);
                        if (r == -1)
                        {
                            result = "cmd=0201,ret=0,data=null";
                        }
                        else
                        {
                            result = "cmd=0201,ret=1,data=null";
                        }
                        break;
                    #endregion

                    #region << 0202 功率源升源（高级） >>
                    case "0202":
                        strdatas = data.Split(';');
                        execallthread.NowDSPOutputValue.DJupCInputToValue(strdatas);
                        r = execallthread.NowDSPOutputValue.SetOperationHC_Fun();
                        Thread.Sleep(100);
                        if (r == -1)
                        {
                            r = execallthread.NowDSPOutputValue.DSPOutput();
                        }
                        Thread.Sleep(100);
                        if (r == -1)
                        {
                            r = execallthread.NowDSPOutputValue.OperationHC_Fun();
                        }
                        Thread.Sleep(100);
                        if (r == -1)
                        {
                            result = "cmd=0202,ret=0,data=null";
                        }
                        else
                        {
                            result = "cmd=0202,ret=1,data=null";
                        }
                        break;
                    #endregion

                    #region << 0203 功率源关源 >>
                    case "0203":
                        r = execallthread.NowDSPOutputValue.DSPDown();
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
                        f = execallthread.NowDSPOutputValue.XBsetCInputToValue(strdatas);
                        if (f)
                            r = execallthread.NowDSPOutputValue.SetDSPHarmonic();
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
                    case "0301":
                        r = execallthread.NowDSPOutputValue.OperationHC_Fun();
                        //if (r == -1)
                        //{
                        result = execallthread.NowDSPOutputValue.XBCValueToOutput("0301");
                        //}
                        //else
                        //{
                        //    result = "cmd=0301,ret=1,data=null";
                        //}
                        break;
                    #endregion

                    #region << 0302 设置标准表>>     XXX

                        break;
                        #endregion

                    #region << 0401 初始化台体表位>>   ???
                    case "0401":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        _tempdata = Convert.ToInt32(strdatas[2]);
                        lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].SendSelD485Set(lnk, strdatas, out sendbytes, out receivebytes);
                            Thread.Sleep(100);
                            if (f)
                            {
                                f = execallthread.NowBTGNCScls.btgns[bwindex].SetTD485(bwindex, _tempdata, out sendbytes, out receivestr);
                            }
                        }
                        if (f)
                            result = "cmd=0401,ret=0,data=null";
                        else
                            result = "cmd=0401,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0402 得到台体遥信状态>>
                    case "0402":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].SendXYCGet(lnk, strdatas, out sendbytes, out receivebytes);
                            Thread.Sleep(100);
                        }
                        if (f)
                            result = execallthread.NowBTGNCScls.btgns[bwindex].XYCValueToOutput(key);
                        else
                            result = "cmd=0402,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0403 设置台体遥信状态>>
                    case "0403":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].SendXYCSet(lnk, strdatas, out sendbytes, out receivebytes);
                            Thread.Sleep(100);
                        }
                        if (f)
                            result = "cmd=0403,ret=0,data=null";
                        else
                            result = "cmd=0403,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0404 得到台体遥控状态>> 不支持，但当作成功
                    case "0404":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].SendYKCSet(lnk, strdatas, out sendbytes, out receivebytes);
                            Thread.Sleep(100);
                        }
                        if (f)
                            result = execallthread.NowBTGNCScls.btgns[bwindex].YKCValueToOutput(key);
                        else
                            result = "cmd=0404,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0405 设置台体遥控状态>>  XXX
                    case "0405":
                        result = "cmd=0405,ret=0,data=null";
                        break;
                    #endregion

                    #region << 0406 台体脉冲设置>>
                    case "0406":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        }
                        f = execallthread.NowBTGNCScls.btgns[bwindex].SendMCCSet(lnk, strdatas, out sendbytes, out receivebytes, out sendbytes2, out receivebytes2);
                        Thread.Sleep(100);
                        if (f)
                            result = "cmd=0406,ret=0,data=null";
                        else
                            result = "cmd=0406,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0407 台体脉冲输出>>
                    case "0407":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        }
                        f = execallthread.NowBTGNCScls.btgns[bwindex].SendMCCStart(lnk, strdatas, out sendbytes, out receivebytes, out sendbytes2, out receivebytes2);
                        Thread.Sleep(100);
                        if (f)
                            result = "cmd=0407,ret=0,data=null";
                        else
                            result = "cmd=0407,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0408 台体脉冲输出停止>>
                    case "0408":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        f = execallthread.NowBTGNCScls.btgns[bwindex].SendMCCEnd(lnk, strdatas, out sendbytes, out receivebytes, out sendbytes2, out receivebytes2);
                        Thread.Sleep(100);
                        if (f)
                            result = "cmd=0408,ret=0,data=null";
                        else
                            result = "cmd=0408,ret=1,data=null";
                        break;
                    #endregion

                    #region << 0409 电流回路复位>>
                    case "0409":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].SendTDDDLdown(lnk, strdatas, out sendbytes, out receivebytes);
                            Thread.Sleep(500);
                            if (f)
                                f = execallthread.NowBTGNCScls.btgns[bwindex].SendTDDDLup(lnk, strdatas, out sendbytes, out receivebytes);
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
                        lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].SendTDDDLdown(lnk, strdatas, out sendbytes, out receivebytes);
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
                        lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].SendTDDDLup(lnk, strdatas, out sendbytes, out receivebytes);
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
                        lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].GetTD485(bwindex, out sendbytes, out receivestr);
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
                        lnk = execallthread.getSocketClient(TTDeviceType.LinkBZ, bwindex);
                        if ((bwindex >= 0) && (bwindex < 8) && (lnk != null))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].SetTD485(bwindex, _tempdata, out sendbytes, out receivestr);
                        }
                        if (f)
                            result = "cmd=040D,ret=0,data=" + receivestr;
                        else
                            result = "cmd=040D,ret=1,data=null";
                        break;
                    #endregion

                    #region <<  1001 给各表位终端发送规约数据>>
                    case "1001":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].SendZF(bwindex, strdatas, out sendbytes, out receivestr);
                        }

                        if (f)
                            result = "cmd=1001,ret=0,data=" + (bwindex + 1) + ";" + receivestr;
                        else
                            result = "cmd=1001,ret=1,data=null";
                        break;
                    #endregion

                    #region <<  1002 各表位终端抄读模拟电表数据>>
                    case "1002":
                        strdatas = data.Split(';');
                        bwindex = Convert.ToInt32(strdatas[0]) - 1;
                        if ((bwindex >= 0) && (bwindex < 8))
                        {
                            f = execallthread.NowBTGNCScls.btgns[bwindex].SendMNCB(bwindex, strdatas, out sendbytes, out receivestr);
                        }

                        //if (f)
                        //    result = "cmd=0102,ret=0,data=" + bwindex + ";" + receivestr;
                        //else
                        //    result = "cmd=0102,ret=1,data=null";
                        break;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                TXTWrite.WriteERRTxt("ExecThread", "exec", "", "", ex.ToString());
            }
            finally
            {
                if (result != string.Empty)
                {
                    execallthread.UdpSend(result, ed);
                }
                threadrunningis = false;
            }
        }
    }
}

