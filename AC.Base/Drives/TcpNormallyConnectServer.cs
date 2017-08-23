using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading;
using System.Net;

namespace AC.Base.Drives
{
    /// <summary>
    /// TCP 长连接服务器。该类使用 TCP 协议长连接方式作为 Socket 监听服务器，具有客户端连接后缓存该连接并检查是否在限定时间内登录、向指定连接发送数据、接收并缓存接收到的数据的功能。
    /// </summary>
    public abstract class TcpNormallyConnectServer<ADDRESS> : DriveBase
    {

        #region 变量
        /// <summary>
        /// Socket 监听服务
        /// </summary>
        private Socket m_SocketServer;

        /// <summary>
        /// 用于周期检查连接状态的线程
        /// </summary>
        private Thread m_thrTimerCheck;

        /// <summary>
        /// 已连接但是还未登录的异步套接字操作集合，该集合内保存了状态为已连接(Connected)的异步套接字操作
        /// </summary>
        private ConcurrentBag<SocketAsyncEventArgs> m_Connecteds;

        /// <summary>
        /// 待处理的异步套接字登录队列，该集合内保存了状态为已连接(Connected)的异步套接字操作，并且在弹出队列后状态将变更为已登录在线中(Online)。
        /// </summary>
        private BlockingCollection<SocketAsyncEventArgs> m_LogonTask;

        /// <summary>
        /// 待处理的异步套接字登录线程
        /// </summary>
        private Thread m_thrLogonTask;

        /// <summary>
        /// 已成功登录并且在线的异步套接字操作集合，该集合内保存了状态为已登录在线中(Online)的异步套接字操作。
        /// </summary>
        private ConcurrentDictionary<ADDRESS, SocketAsyncEventArgs> m_Onlines;

        /// <summary>
        /// 待处理的接收数据队列集合
        /// </summary>
        private BlockingCollection<DataTask> m_ReceiveDataTasks;

        /// <summary>
        /// 待处理的接收数据队列线程
        /// </summary>
        private Thread m_thrReceiveDataTask;

        private bool m_IsStart;

        #endregion

        /// <summary>
        /// 启动 Socket 服务器
        /// </summary>
        public override void Start()
        {
            try
            {
                this.m_Connecteds = new ConcurrentBag<SocketAsyncEventArgs>();

                this.m_LogonTask = new BlockingCollection<SocketAsyncEventArgs>();
                this.m_thrLogonTask = new Thread(new ThreadStart(this.LogonTaskTake));
                this.m_thrLogonTask.IsBackground = true;
                this.m_thrLogonTask.Start();

                this.m_Onlines = new ConcurrentDictionary<ADDRESS, SocketAsyncEventArgs>();

                this.m_SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.m_SocketServer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                this.m_SocketServer.SendBufferSize = this.BufferSize;
                this.m_SocketServer.ReceiveBufferSize = this.BufferSize;

                this.m_ReceiveDataTasks = new BlockingCollection<DataTask>();
                this.m_thrReceiveDataTask = new Thread(new ThreadStart(this.ReceiveDataTaskTake));
                this.m_thrReceiveDataTask.IsBackground = true;
                this.m_thrReceiveDataTask.Start();

                if (this.ListenIp == null || this.ListenIp.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length != 4)
                {
                    this.m_SocketServer.Bind(new IPEndPoint(System.Net.IPAddress.Any, this.ListenPort));
                }
                else
                {
                    string[] strAddress = this.ListenIp.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    byte[] bytAddress = { Byte.Parse(strAddress[0]), Byte.Parse(strAddress[1]), Byte.Parse(strAddress[2]), Byte.Parse(strAddress[3]) };
                    this.m_SocketServer.Bind(new IPEndPoint(new IPAddress(bytAddress), this.ListenPort));
                }
                this.m_SocketServer.Listen(this.ListenBacklog);

                Accept();

                this.m_thrTimerCheck = new System.Threading.Thread(new System.Threading.ThreadStart(this.TimerCheck));
                this.m_thrTimerCheck.IsBackground = true;
                this.m_thrTimerCheck.Start();

                this.m_IsStart = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void TimerCheck()
        {
            //int intIndex = 0;
            //foreach (KeyValuePair<ADDRESS, SocketAsyncEventArgs> kvp in this.m_Onlines)
            //{
            //    intIndex++;
            //    kvp.Value.AcceptSocket.Send(new byte[] { (byte)intIndex });
            //}

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private volatile int SocketClientIndex;
        private volatile int AcceptSN;

        private void Accept()
        {
            SocketAsyncEventArgs args = null;
            try
            {
                args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(SocketAsyncEventArgs_Completed);

                this.SocketClientIndex++;
                SocketClient _SocketClient = new SocketClient(this, args);
                _SocketClient.SocketClientIndex = this.SocketClientIndex;
                args.UserToken = _SocketClient;

                this.AcceptSN++;
                _SocketClient.SN = this.AcceptSN;
                _SocketClient.CloseCount = 0;

                if (args != null)
                {
                    try
                    {
                        if (this.m_SocketServer.AcceptAsync(args) == false)
                        {
                            this.AcceptCallback(args);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        this.DoException(args, ex);

                        this.Accept();
                    }
                    catch (Exception ex)
                    {
                        this.DoException(args, ex);

                        this.SocketAsyncFinish(args);
                        this.Accept();
                    }
                }
            }
            catch (Exception ex)
            {
                this.DoException(args, ex);
            }
        }

        private void SocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs args)
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
                {
                    this.SocketAsyncFinish(args);
                }
            }
            catch (Exception ex)
            {
                this.DoException(args, ex);
            }
        }

        //有新的设备 Socket 连接，将该对象设置为已连接状态
        private void AcceptCallback(SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    byte[] bytBuffer = new byte[this.BufferSize];
                    args.SetBuffer(bytBuffer, 0, bytBuffer.Length);

                    SocketClient _SocketClient = args.UserToken as SocketClient;
                    _SocketClient.ChangeState(SocketClientStateOptions.Connected);

                    try
                    {
                        if (args.AcceptSocket.ReceiveAsync(args) == false)
                        {
                            this.ReceiveCallback(args);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.DoException(args, ex);

                        this.SocketAsyncFinish(args);
                    }
                }
                else
                {
                    this.DoException(args, new Exception("在 BeginAccept 方法中 SocketAsyncEventArgs 的 SocketError 属性为 " + args.SocketError.ToString() + "。"));

                    this.SocketAsyncFinish(args);
                }
            }
            catch (Exception ex)
            {
                this.DoException(args, ex);
            }
            finally
            {
                this.Accept();
            }
        }

        //接收客户端发来的数据
        private void ReceiveCallback(SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    if (args.BytesTransferred > 0)
                    {
                        byte[] bytReceiveData = new byte[args.BytesTransferred];
                        Array.Copy(args.Buffer, args.Offset, bytReceiveData, 0, bytReceiveData.Length);

                        //使用队列处理接收到的数据 bytReceiveData
                        DataTask _DataTask;
                        _DataTask.Args = args;
                        _DataTask.Data = bytReceiveData;
                        this.m_ReceiveDataTasks.TryAdd(_DataTask);

                        SocketClient _SocketClient = args.UserToken as SocketClient;
                        _SocketClient.UpLastTime();

                        try
                        {
                            if (args.AcceptSocket.ReceiveAsync(args) == false)
                            {
                                this.ReceiveCallback(args);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.DoException(args, ex);

                            this.SocketAsyncFinish(args);
                        }
                    }
                    else
                    {
                        this.SocketAsyncFinish(args);
                    }
                }
                else
                {
                    this.DoException(args, new Exception("在 BeginReceive 方法中 SocketAsyncEventArgs 的 SocketError 属性为 " + args.SocketError.ToString() + "。"));
                }
            }
            catch (Exception ex)
            {
                this.DoException(args, ex);
            }
        }

        //处理数据接收队列
        private void ReceiveDataTaskTake()
        {
            while (true)
            {
                DataTask _DataTask = this.m_ReceiveDataTasks.Take();
                try
                {
                    SocketClient _SocketClient = _DataTask.Args.UserToken as SocketClient;
                    this.ReceiveData(_DataTask.Args, _SocketClient.State == SocketClientStateOptions.Online, _DataTask.Data);
                }
                catch (Exception ex)
                {
                    this.DoException(_DataTask.Args, ex);
                }

            }
        }

        /// <summary>
        /// 接收到新的数据，对数据进行分包检查，并处理完整的一包数据。
        /// </summary>
        /// <param name="args">异步套接字</param>
        /// <param name="isLogged">该连接是否已登录</param>
        /// <param name="receiveData">新接收到的数据。</param>
        public abstract void ReceiveData(SocketAsyncEventArgs args, bool isLogged, byte[] receiveData);

        /// <summary>
        /// 接收到的数据不是完整的数据，需要等待下一包数据合并为一帧数据。
        /// </summary>
        /// <param name="args">异步套接字</param>
        /// <param name="receiveData">此次接收的数据。</param>
        public void WaitNextData(SocketAsyncEventArgs args, byte[] receiveData)
        {
        }

        /// <summary>
        /// 自上次接收到分包数据后，又接收到新的数据。
        /// </summary>
        /// <param name="args">异步套接字</param>
        /// <param name="isLogged">该连接是否已登录</param>
        /// <param name="receiveData">此次接收的数据。</param>
        /// <param name="lastData">之前所接收到的所有分包数据。</param>
        public abstract void ReceiveData(SocketAsyncEventArgs args, bool isLogged, byte[] receiveData, byte[] lastData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="sendData"></param>
        public void SendData(SocketAsyncEventArgs args, byte[] sendData)
        {
            try
            {
                if (args.AcceptSocket != null && args.AcceptSocket.Connected)
                {
                    args.AcceptSocket.Send(sendData);
                }
                //System.Diagnostics.Debug.WriteLine("[" + Thread.CurrentThread.ManagedThreadId + "]发送 " + Function.OutBytes(sendData));
            }
            catch (Exception ex)
            {
                this.DoException(args, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="sendData"></param>
        public void SendData(ADDRESS address, byte[] sendData)
        {
            // 该方法应将发送数据加入队列，并且考虑发送优先级

            SocketAsyncEventArgs args;
            if (this.m_Onlines.TryGetValue(address, out args))
            {
                try
                {
                    if (args.AcceptSocket.Connected)
                    {
                        args.AcceptSocket.Send(sendData);
                    }
                    else
                    {
                        this.DoException(args, new Exception("向通讯地址为 " + address + " 的设备发送数据时 Socket 连接已关闭。"));
                    }
                }
                catch (Exception ex)
                {
                    this.DoException(args, new Exception("向通讯地址为 " + address + " 的设备发送数据时产生异常。", ex));
                }
            }
            else
            {
                this.DoException(args, new Exception("通讯地址为 " + address + " 的设备不在线。"));
            }
        }

        /// <summary>
        /// 指定的异步套接字准许登录
        /// </summary>
        /// <param name="args">异步套接字</param>
        /// <param name="address">通讯地址</param>
        protected void Logon(SocketAsyncEventArgs args, ADDRESS address)
        {
            try
            {
                SocketClient _SocketClient = args.UserToken as SocketClient;
                _SocketClient.Address = address;
                this.m_LogonTask.TryAdd(args);
            }
            catch (Exception ex)
            {
                this.DoException(args, ex);
            }
        }

        //处理登录队列
        private void LogonTaskTake()
        {
            while (true)
            {
                SocketAsyncEventArgs args = this.m_LogonTask.Take();
                try
                {
                    SocketClient _SocketClient = args.UserToken as SocketClient;
                    _SocketClient.ChangeState(SocketClientStateOptions.Online);
                }
                catch (Exception ex)
                {
                    this.DoException(args, ex);
                }
            }
        }

        /// <summary>
        /// 指定的异步套接字请求注销。该操作将断开服务器与该异步套接字之间的连接。
        /// </summary>
        /// <param name="address">通讯地址</param>
        protected void Logout(ADDRESS address)
        {
        }

        /// <summary>
        /// 分包数据已接收完成。
        /// </summary>
        /// <param name="args">异步套接字</param>
        public void WaitFinish(SocketAsyncEventArgs args)
        {
        }

        /// <summary>
        /// 关闭设备 Socket，并恢复该对象为可再次使用状态
        /// </summary>
        /// <param name="args"></param>
        private void SocketAsyncFinish(SocketAsyncEventArgs args)
        {
            try
            {
                SocketClient _SocketClient = args.UserToken as SocketClient;
                _SocketClient.ChangeState(SocketClientStateOptions.Free);
            }
            catch (Exception ex)
            {
                this.DoException(args, ex);
            }
        }

        /// <summary>
        /// 停止 Socket 服务器。
        /// </summary>
        public override void Stop()
        {
            while (this.m_thrTimerCheck.ThreadState != System.Threading.ThreadState.Aborted && this.m_thrTimerCheck.ThreadState != ThreadState.Stopped)
            {
                this.m_thrTimerCheck.Abort();
                this.m_thrTimerCheck.Join(100);
            }
            this.m_thrTimerCheck = null;

            //while (this.m_cdLogged.Count > 0)
            //{
            //    try
            //    {
            //        this.SocketAsyncFinish(this.m_cdLogged[0], true);
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine("[" + Thread.CurrentThread.ManagedThreadId.ToString("X2") + "]" + ex);
            //    }
            //}

            //SocketAsyncEventArgs args = null;
            //this.m_cbConnected.TryTake(out args);
            //while (args != null)
            //{
            //    try
            //    {
            //        this.SocketAsyncFinish(args, true);

            //        this.m_cbConnected.TryTake(out args);
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine("[" + Thread.CurrentThread.ManagedThreadId.ToString("X2") + "]" + ex);
            //    }
            //}

            this.m_IsStart = false;
        }

        private void DoException(SocketAsyncEventArgs args, Exception ex)
        {
            SocketClient _SocketClient = args.UserToken as SocketClient;
            System.Diagnostics.Debug.WriteLine("[" + Thread.CurrentThread.ManagedThreadId.ToString("X2") + "] " + (args.AcceptSocket != null && args.AcceptSocket.Connected ? args.AcceptSocket.RemoteEndPoint.ToString() : "无地址") + "\tSN:" + _SocketClient.SN + "\tCID:" + _SocketClient.SocketClientIndex + "\tCloseCount:" + _SocketClient.CloseCount + "\tAddress:" + _SocketClient.Address + "\t" + _SocketClient.GetCollectionName() + "\r\n" + ex);
        }

        /// <summary>
        /// 处理异常。
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void DoException(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("[" + Thread.CurrentThread.ManagedThreadId.ToString("X2") + "] " + ex);
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.m_IsStart)
            {
                string str = "";
                str += "已建套接字:  " + this.SocketClientIndex + "\r\n";
                str += "连接队列:    " + this.m_Connecteds.Count + "\r\n";
                str += "登录队列:    " + this.m_LogonTask.Count + "\r\n";
                str += "在线集合:    " + this.m_Onlines.Count + "\r\n";
                str += "数据处理队列:" + this.m_ReceiveDataTasks.Count + "\r\n";

                //foreach (KeyValuePair<ADDRESS, SocketAsyncEventArgs> kvp in this.m_Onlines)
                //{
                //    str += kvp.Key + "\r\n";
                //}

                return str;
            }
            else
            {
                return "服务尚未启动";
            }
        }

        private class SocketClient
        {
            private TcpNormallyConnectServer<ADDRESS> m_Server;
            private SocketAsyncEventArgs m_Args;

            public SocketClient(TcpNormallyConnectServer<ADDRESS> server, SocketAsyncEventArgs args)
            {
                this.m_Server = server;
                this.m_Args = args;
            }

            public int SocketClientIndex;

            public int SN;

            public int CloseCount;

            /// <summary>
            /// 地址。
            /// </summary>
            public ADDRESS Address = default(ADDRESS);

            /// <summary>
            /// 获取首次连接接入服务器的时间
            /// </summary>
            public DateTime ConnectTime { get; private set; }

            /// <summary>
            /// 获取最后收发数据的时间。
            /// </summary>
            public DateTime LastTime { get; private set; }

            /// <summary>
            /// 更新最后收发数据的时间。
            /// </summary>
            public void UpLastTime()
            {
                this.LastTime = DateTime.Now;
            }

            /// <summary>
            /// 获取当前状态。
            /// </summary>
            public SocketClientStateOptions State { get; private set; }

            private void Close()
            {
                this.CloseCount++;

                if (this.CloseCount > 1)
                {
                    this.m_Server.DoException(this.m_Args, new Exception("关闭了 " + this.CloseCount + " 次"));
                }

                this.m_Args.Completed -= new EventHandler<SocketAsyncEventArgs>(this.m_Server.SocketAsyncEventArgs_Completed);

                this.Address = default(ADDRESS);

                if (this.m_Args.AcceptSocket != null)
                {
                    if (this.m_Args.AcceptSocket.Connected)
                    {
                        try
                        {
                            this.m_Args.AcceptSocket.Shutdown(SocketShutdown.Both);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    this.m_Args.AcceptSocket.Close();
                    //Thread.Sleep(10);                    //（必须！）等待 AcceptSocket 释放，如果出现“现在已经正在使用此 SocketAsyncEventArgs 实例进行异步套接字操作”的错误，则还需增大延时。

                    this.m_Args.AcceptSocket = null;
                }

                if (this.m_Args.Buffer != null)
                {
                    this.m_Args.SetBuffer(null, 0, 0);
                }

                this.State = SocketClientStateOptions.Free;
            }

            /// <summary>
            /// 改变状态。
            /// </summary>
            /// <param name="state"></param>
            public void ChangeState(SocketClientStateOptions state)
            {
                lock (this)
                {
                    PrintState(state, false);

                    if (state != this.State)
                    {
                        switch (this.State)
                        {
                            case SocketClientStateOptions.Free:
                                if (state == SocketClientStateOptions.Connected)
                                {
                                    if (this.m_Server.m_Connecteds.Contains(this.m_Args) == false)
                                    {
                                        this.m_Server.m_Connecteds.Add(this.m_Args);
                                        this.State = state;
                                    }
                                    else
                                    {
                                        this.m_Server.DoException(this.m_Args, new Exception("状态从 " + this.State + " 变更为 " + state + " 时，连接队列中已存在当前异步套接字操作"));
                                    }
                                }
                                else
                                {
                                    if (this.CloseCount != 0)
                                    {
                                        PrintState(state, true);
                                    }
                                }
                                break;

                            case SocketClientStateOptions.Connected:
                                SocketAsyncEventArgs argsOut;
                                if (this.m_Server.m_Connecteds.TryTake(out argsOut) == false)
                                {
                                    this.m_Server.DoException(this.m_Args, new Exception("从异步套接字连接集合中移除连接时发生错误：未发现该套接字的连接信息。"));
                                }

                                if (state == SocketClientStateOptions.Online)
                                {
                                    while (this.m_Server.m_Onlines.TryAdd(this.Address, this.m_Args) == false)
                                    {
                                        //如果加入登录后的集合中失败，则表示已有相同地址的连接已经登录
                                        if (this.m_Server.m_Onlines.TryGetValue(this.Address, out argsOut))
                                        {
                                            SocketClient _SocketClientOut = argsOut.UserToken as SocketClient;
                                            _SocketClientOut.ChangeState(SocketClientStateOptions.Free);
                                        }
                                    }
                                    this.State = state;
                                }
                                else if (state == SocketClientStateOptions.Free)
                                {
                                    this.Close();
                                }
                                break;

                            case SocketClientStateOptions.Online:
                                if (state == SocketClientStateOptions.Free)
                                {
                                    SocketAsyncEventArgs argsDis;
                                    if (this.m_Server.m_Onlines.TryRemove(this.Address, out argsDis) == false)
                                    {
                                        this.m_Server.DoException(this.m_Args, new Exception("从异步套接字在线集合中移除连接时发生错误：未发现该套接字的在线信息。(该异常通常为强制关闭连接后出现，不影响后续操作)"));
                                    }

                                    this.Close();
                                }
                                else
                                {
                                    PrintState(state, true);
                                }
                                break;
                        }
                    }
                    else
                    {
                        PrintState(state, true);
                    }
                }
            }

            public void PrintState(SocketClientStateOptions state, bool isError)
            {
                string str = "[" + Thread.CurrentThread.ManagedThreadId.ToString("X2") + "] " + (isError ? "！" : "  ") + this.State + ">" + state + "\tSN:" + this.SN + "\tCID:" + this.SocketClientIndex + "\tCloseCount:" + this.CloseCount + "\tAddress:" + this.Address + "\t" + this.GetCollectionName() + "\tLength:" + this.m_Args.BytesTransferred + "\tOperation:" + this.m_Args.LastOperation + "\tError:" + this.m_Args.SocketError;
                if (this.m_Args.AcceptSocket != null)
                {
                    str += "\tConnected:" + this.m_Args.AcceptSocket.Connected;
                    if (this.m_Args.AcceptSocket.RemoteEndPoint != null)
                    {
                        str += " IP:" + this.m_Args.AcceptSocket.RemoteEndPoint;
                    }
                }
                //System.Diagnostics.Debug.WriteLine(str);
            }

            public string GetCollectionName()
            {
                string str = "";

                if (this.m_Server.m_Connecteds.Contains(this.m_Args))
                {
                    str += "Connecteds";
                }
                if (this.m_Server.m_Onlines.ContainsKey(this.Address))
                {
                    try
                    {
                        if (this.m_Server.m_Onlines[this.Address].Equals(this.m_Args))
                        {
                            str += "Onlines ";
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (str.Length == 0)
                {
                    str = "\t";
                }
                return str;
            }
        }

        private enum SocketClientStateOptions
        {
            /// <summary>
            /// 未使用。
            /// </summary>
            Free,

            /// <summary>
            /// 已连接。
            /// </summary>
            Connected,

            /// <summary>
            /// 已登录在线中。
            /// </summary>
            Online,
        }

        //数据任务队列
        private struct DataTask
        {
            public SocketAsyncEventArgs Args;
            public byte[] Data;
        }

        #region 参数

        private int m_ListenPort = 0;
        /// <summary>
        /// 获取或设置服务监听的端口。如果该值不能小于等于 0。
        /// 更改参数后需要重启服务器。
        /// </summary>
        public int ListenPort
        {
            get { return this.m_ListenPort; }
            set { this.m_ListenPort = value; }
        }

        private int m_BufferSize = 1024;
        /// <summary>
        /// 发送数据时 TCP 缓冲区的大小。默认值为 1024。
        /// 更改参数后需要重启服务器。
        /// </summary>
        public int BufferSize
        {
            get { return this.m_BufferSize; }
            set { this.m_BufferSize = value; }
        }

        private string m_ListenIp;
        /// <summary>
        /// 获取或设置服务监听绑定的 IP 地址。如果所在计算机有多个 IP 地址，该属性可以指定只在某一个地址上进行监听。如果该属性为 null 则表示监听所有地址上的数据。
        /// 更改参数后需要重启服务器。
        /// </summary>
        public string ListenIp
        {
            get { return this.m_ListenIp; }
            set
            {
                if (value != null)
                {
                    string[] strIp = value.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strIp.Length != 4)
                    {
                        new Exception("IP 地址格式不正确。");
                    }
                    else
                    {
                        for (int intIndex = 0; intIndex < strIp.Length; intIndex++)
                        {
                            byte bytIp;
                            if (Byte.TryParse(strIp[intIndex], out bytIp) == false)
                            {
                                new Exception("IP 地址格式不正确。");
                            }
                        }
                    }
                }
                this.m_ListenIp = value;
            }
        }

        private int m_MaximumConnection = 30000;
        /// <summary>
        /// 最大连接数量，超出该数量的后续连接将被拒绝。该值有效范围为 1 - 100000。默认值为 30000。
        /// </summary>
        public int MaximumConnection
        {
            get { return this.m_MaximumConnection; }
            set
            {
                if (value < 1)
                {
                    new Exception("最大连接数量不得小于 1");
                }
                else if (value > 100000)
                {
                    new Exception("最大连接数量不得大于 100000");
                }
                else
                {
                    this.m_MaximumConnection = value;
                }
            }
        }

        private int m_ListenBacklog = 200;
        /// <summary>
        /// 挂起连接队列的最大长度。如果服务器处于忙碌状态未能及时响应连接，则连接将被放入等待队列中，如果等待队列已满则后续连接将被拒绝。如果在同一时刻有大量的连接需要接入，则应将此值设置为较大值。默认值为 1000。
        /// 更改参数后需要重启服务器。
        /// </summary>
        public int ListenBacklog
        {
            get { return this.m_ListenBacklog; }
            set { this.m_ListenBacklog = value; }
        }

        private int m_LogonTimeout = 5000;
        /// <summary>
        /// 连接后并登录的时间。新的连接接入后必须在该时间内发送登录报文确认连接身份，如果超出该时间未能成功登录，则服务器将断开该连接。默认值为 5000 毫秒，该值为 -1 时表示不进行此项检测。
        /// </summary>
        public int LogonTimeout
        {
            get { return this.m_LogonTimeout; }
            set { this.m_LogonTimeout = value; }
        }

        private int m_CommunicationTimeout = 900000;
        /// <summary>
        /// 登录后的连接，在指定的时间内如果未收发任何数据，则认为该连接已失效将关闭该连接。默认值为 900000 毫秒(15分钟)，该值为 -1 时表示不进行此项检测。
        /// </summary>
        public int CommunicationTimeout
        {
            get { return this.m_CommunicationTimeout; }
            set { this.m_CommunicationTimeout = value; }
        }


        //分包数据时间间隔
        private int m_DataSplitTimeout = 1000;
        /// <summary>
        /// 当数据包发生分包现象时，2个数据包之间最大允许的时间间隔，超过此时间间隔的分包数据将被丢弃。默认值为 1000 毫秒。
        /// </summary>
        public int DataSplitTimeout
        {
            get { return this.m_DataSplitTimeout; }
            set { this.m_DataSplitTimeout = value; }
        }

        /// <summary>
        /// 从保存此服务数据的 XML 文档节点初始化当前服务。
        /// </summary>
        /// <param name="channelServiceConfig">该对象节点的数据</param>
        public void SetConfig(System.Xml.XmlNode channelServiceConfig)
        {
            foreach (System.Xml.XmlNode xnItem in channelServiceConfig.ChildNodes)
            {
                switch (xnItem.Name)
                {
                    case "ListenPort":
                        this.ListenPort = Function.ToInt(xnItem.InnerText);
                        break;

                    case "BufferSize":
                        this.BufferSize = Function.ToInt(xnItem.InnerText);
                        break;

                    case "ListenIp":
                        this.ListenIp = xnItem.InnerText;
                        break;

                    case "MaximumConnection":
                        this.MaximumConnection = Function.ToInt(xnItem.InnerText);
                        break;

                    case "ListenBacklog":
                        this.ListenBacklog = Function.ToInt(xnItem.InnerText);
                        break;
                }
            }
        }

        /// <summary>
        /// 获取当前配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

            System.Xml.XmlNode xnListenPort = xmlDoc.CreateElement("ListenPort");
            xnListenPort.InnerText = this.ListenPort.ToString();
            xnConfig.AppendChild(xnListenPort);

            System.Xml.XmlNode xnBufferSize = xmlDoc.CreateElement("BufferSize");
            xnBufferSize.InnerText = this.BufferSize.ToString();
            xnConfig.AppendChild(xnBufferSize);

            if (this.ListenIp != null && this.ListenIp.Length > 0)
            {
                System.Xml.XmlNode xnListenIp = xmlDoc.CreateElement("ListenIp");
                xnListenIp.InnerText = this.ListenIp;
                xnConfig.AppendChild(xnListenIp);
            }

            System.Xml.XmlNode xnMaximumConnection = xmlDoc.CreateElement("MaximumConnection");
            xnMaximumConnection.InnerText = this.MaximumConnection.ToString();
            xnConfig.AppendChild(xnMaximumConnection);

            System.Xml.XmlNode xnListenBacklog = xmlDoc.CreateElement("ListenBacklog");
            xnListenBacklog.InnerText = this.ListenBacklog.ToString();
            xnConfig.AppendChild(xnListenBacklog);

            return xnConfig;
        }

        #endregion

    }
}
