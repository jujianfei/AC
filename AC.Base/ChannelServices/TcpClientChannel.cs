using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AC.Base.ChannelServices
{
    /// <summary>
    /// socket client 通道
    /// </summary>
    public class TcpClientChannel :  IChannelService
    {
        private Socket sockethandler; // tcp socket
        private string IPaddr = string.Empty;//key IP地址
        private int Port = 0;
        private int _DataReceivedCount = 0;//接收到数据统计
        private int _DataSendCount = 0;//发送数据统计
        private DateTime _LoginTime = DateTime.MinValue; //终端连接上的时间
        private DateTime _LastReceivedTime = DateTime.Now; //最后接收到数据的时间
        private int bytesRead; //异步接收数据的长度
        private int bytesSend; //异步发送数据的长度
        private const int BUFFERSIZE = 1024; //最大接收字节长度
        private byte[] buffer = new byte[BUFFERSIZE];//接收缓存区域
        public byte[] _temp;


        #region << 实现disponsabe接口 >>
        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                this.Dispose(true);
                GC.SuppressFinalize(this);  // Finalize 不会第二次执行
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)  // 对象正在被显示释放, 不是执行 Finalize()
            {
                //关闭socket链接
                if (sockethandler != null)
                {
                    sockethandler.Shutdown(SocketShutdown.Both);
                    sockethandler.Close();
                }
            }
        }
        #endregion

        /// <summary>
        /// 启动通道。该方法由通道服务程序或前置机程序作为宿主进行调用，调用该方法后通道应首先开启后台服务以便主控软件可以与该通道连接，然后开启设备端服务允许设备接入。
        /// </summary>
        public void Start()
        {
            if (this.State != ChannelServiceStateOptions.Running)
            {
                this.OnStateChanging(ChannelServiceStateOptions.Running);

                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(IPaddr), Port);
                //创建Socket
                sockethandler = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                //设置Socket参数
                sockethandler.ReceiveBufferSize = 8192;       //8k
                sockethandler.ReceiveTimeout = 100;       //ms
                sockethandler.SendBufferSize = 8192;	       //8k	
                sockethandler.SendTimeout = 100;          //ms
                //连接Socket
                sockethandler.Connect(ipe);

                sockethandler.BeginReceive(buffer, 0, BUFFERSIZE, SocketFlags.None, new AsyncCallback(ReadCallback), this);
                sockethandler.IOControl(-1744830460, new byte[] { 1, 0, 0, 0, 0x30, 0x75, 0, 0, 0xD0, 0x07, 0, 0 }, null);
                _LoginTime = DateTime.Now;
                this.State = ChannelServiceStateOptions.Running;
            }
        }

        #region << 接收数据 >>
        public void ReadCallback(IAsyncResult ar)
        {
            try
            {
                if (!_disposed)
                {
                    bytesRead = sockethandler.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        Interlocked.Increment(ref _DataReceivedCount);
                        _temp = new byte[bytesRead];
                        Array.Copy(buffer, 0, _temp, 0, bytesRead); //数据
                        _LastReceivedTime = DateTime.Now;
                        //this.OnDataRecived(id, _temp);
                        sockethandler.BeginReceive(buffer, 0, BUFFERSIZE, SocketFlags.None, new AsyncCallback(ReadCallback), this);
                    }
                    else
                    {
                        this.Dispose();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region << 发送数据 >>
        public void SendCallback(IAsyncResult ar)
        {
            bytesSend = sockethandler.EndSend(ar);
            Interlocked.Increment(ref _DataSendCount);
        }

        public void SenD(byte[] b)
        {
            sockethandler.Send(b);//同步发送
        }
        #endregion


        /// <summary>
        /// 停止通道。该方法由通道服务程序或前置机程序作为宿主进行调用，调用该方法后通道应首先关闭设备端服务断开所有设备的连接，然后通知所有已连接的后台自身即将关闭的消息并关闭后台服务。对于通道安全运行的多种形式，如主备通道或多主通道，则由具体实现完成该功能。
        /// </summary>
        public void Stop()
        {
            if (this.State == ChannelServiceStateOptions.Running)
            {
                this.OnStateChanging(ChannelServiceStateOptions.Stopped);
                this.Dispose();
                this.State = ChannelServiceStateOptions.Stopped;
            }
        }

        private ChannelServiceStateOptions m_State = ChannelServiceStateOptions.Ready;
        /// <summary>
        /// 通道服务当前运行状态。
        /// </summary>
        public ChannelServiceStateOptions State
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

        private void OnStateChanging(ChannelServiceStateOptions state)
        {
            if (this.StateChanging != null)
            {
                this.StateChanging(state);
            }
        }

        /// <summary>
        /// 通道服务运行状态发生改变时产生的事件。
        /// </summary>
        public event ChannelServiceStateChangingEventHandler StateChanging;

        private void OnStateChanged()
        {
            this.OnMessage(ChannelServiceMessageTypeOptions.BackImportance, "通道运行状态变为" + this.State.GetDescription());

            if (this.StateChanged != null)
            {
                this.StateChanged();
            }
        }

        /// <summary>
        /// 通道服务运行状态发生改变后的事件。
        /// </summary>
        public event ChannelServiceStateChangedEventHandler StateChanged;

        /// <summary>
        /// 通道服务产生的消息事件。
        /// </summary>
        public event ChannelServiceMessageEventHandler EventMessage;

        /// <summary>
        /// 引发通道服务产生消息事件
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="message">消息内容。</param>
        protected void OnMessage(ChannelServiceMessageTypeOptions messageType, string message)
        {
            if (this.EventMessage != null)
            {
                this.EventMessage(messageType, message);
            }
        }
    }
}
