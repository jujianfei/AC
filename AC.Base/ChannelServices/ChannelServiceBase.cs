using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Net;

namespace AC.Base.ChannelServices
{
    /// <summary>
    /// 运行在前置计算机上的通道服务的基类。
    /// </summary>
    public abstract class ChannelServiceBase : IChannelService, IBackConnectChannelService
    {
        // 通道。与设备进行通讯，并启动服务为后台提供数据。提供主备切换的功能。
        // 主通道与各备用通道进行网络心跳检测，如果备用通道检测不到主通道，并且备用通道上行畅通，则备用通道转为主通道。
        // 通道上行作为服务端接受交换中心的连接或与交换中心运行在同一进程中被交换中心调用。
        // 继承 Channel 的通道完成各自的下行数据传输逻辑，如 TCP、UDP 网络服务通道；串口通道等。
        // 实现通道的同时，应该实现 ChannelInfo，用于设置与该通道相关的配置参数。

        //如果通道宿主在主台程序中，则事件无需通过网络，可以直接访问该对象传递
        //初始化通道后可以设置是否启动上行 TCP|UDP 服务
        //上行可以通过 TCP|UDP 接入（远程通道），也可以通过订阅事件接入（远程通道）
        //远程通道服务可以有多个IP地址，用于多交换机网络情况。？主备通道的相互检测，如何判断主通道失效
        //上行可以是交换中心也可以是客户端软件通过 TCP|UDP 接入
        //？通道缓存数据是否由基类完成。通常界面首次打开需要显示所有传感器当前值，然后产生数据更新事件后才更新为新值
        //主备通道，基类只能进行系统网络故障检测，通道下行状态由通道自身检测，通道检测出下行发生故障，可以通知备用通道启用
        //在配置设备所使用的通道界面上，主备通道应该仅显示一条信息。
        //在同一组通道的计算机上安装通道服务，配置每个通道的运行模式，并设置其它通道的连接方式。第一个运行的通道自然作为主通道启动。当在主通道上配置备用通道连接信息后，主通道可以尝试与备用通道连接，并发送通道组信息；或者也可在备用通道上配置主通道信息，备用通道尝试与主通道连接并发送自身信息。同一通道组的通道信息应全部保持一致

        //？使用负载均衡设备的通道如何处理；作为服务端并使用混合模式登录的终端如何处理
        //对于主动上报的数据，通道至少应能识别出是哪个设备上报的数据
        //通道是否要识别规约。如果不识别则无法得知上报报文的终端地址，如果要识别则有些通道如串口通道及其它公共通道不应包含识别逻辑
        //主动上报的数据，是应该传播给各客户端，还是应该存库
        //能否主动上报数据，首先取决于设备是否具备上报功能，然后取决于通道（数据传输网络的类型）是否有能力实现多主机网络。例如串口通道、TTL通道只能满足应答式查询
        //基类提供实时数据到达后分发数据的方法，继承的类不负责具体分发工作，由基类将需要分发的数据传输给ChannelInfo，或者产生数据事件由监听事件的类处理
        //如果一组主备通道不在同一个网络中，或相互之间无法直接联系，通道切换后如何交换状态信息，是否需要一个中心信息交换平台
        //记录通道数据流量，用于统计
        //可记录后台连接情况
        //可记录设备连接情况
        //可限制后台连接IP
        //后台连接密码、后台管理密码

        //后台与当前通道的连接
        //同组中的其它主备通道连接
        //用于监视通道运行情况的监控客户端连接
        //通道组内的通道可能不在同一网段内，2个通道相对的IP地址可能也不一样
        //后台与通道可能不在同一网段内，通道的IP地址对于备用通道及后台均不一样


        //*********************************
        // 主台软件与通道的连接。用于主台软件所操作的设备与通道物理连接的设备进行数据交换。

        //该通道的后台连接 IP 地址
        //该通道后台命令连接的端口号
        //该通道数据分发方式：UDP广播|使用后台连接
        //是否启用数据分发

        //提供一个获取实时数据的方法，传入实时数据类型，返回该对象的实例，程序绑定实时数据对象上新数据到达的事件。
        //在全局类中设置一个全局通道地图对象，发送到通道前检查该对象，用以确定是否通过数据中心转发还是直接发送到通道还是与通道在同一宿主中直接调用通道的发送方法。
        //与通道建立一个主连接，用于通道间消息传递；向设备发送数据使用连接池建立新连接进行通讯

        //ChannelConnect 需要该组通道所有成员的后台监听地址
        //Channel 需要除自身以外的其它成员的后台监听地址，用于无法与主通道连接后与其它成员协商升为主机。
        //通道状态改变事件、主备通道切换事件
        //ChannelConnect 始终与主通道连接。具有自主连接主通道，并在主通道意外故障后正确连接至由备升主通道的功能。
        //ChannelMemberInfo 可以连接至指定通道并监视该通道运行情况，无论该通道是否为主通道。

        //通道通讯报文：
        //0xAA
        //类型 0x01：后台给通道发送的命令；0x02：通道给后台发送的命令；0x10：后台给通道发送的数据；0x20：通道给后台发送的数据
        //随机数 2字节。如果是应答报文，该随机数可以确保接收的报文是设备回应的报文。
        //设备地址长度 1字节
        //设备地址
        //数据区长度 2字节
        //数据区
        //0x55

        private Socket m_BackSocketServer;
        private ConcurrentStack<SocketAsyncEventArgs> m_stkBackSocketAsync;       //可用的异步套接字操作栈
        private ConcurrentStack<byte[]> m_stkBackBuffer;                          //可用的数据缓存栈

        #region << 参数 >>

        private int m_BackListenPort = 4001;
        /// <summary>
        /// 获取或设置后台服务监听的端口。如果该值小于等于 0 则表示不启动后台服务。
        /// </summary>
        public int BackListenPort
        {
            get { return this.m_BackListenPort; }
            set { this.m_BackListenPort = value; }
        }

        private int m_BackBufferSize = 8;
        /// <summary>
        /// 后台服务 TCP 缓冲区的大小。
        /// 默认值为 1024。
        /// 更改参数后需要 Stop 然后 Run 该通道。
        /// </summary>
        public int BackBufferSize
        {
            get { return this.m_BackBufferSize; }
            set { this.m_BackBufferSize = value; }
        }

        private string m_BackListenIp;
        /// <summary>
        /// 获取或设置后台服务监听绑定的 IP 地址。
        /// 如果通道所在计算机有多个 IP 地址，该属性可以指定通道只在某一个地址上进行监听。如果该属性为 null 则表示监听所有地址上的数据。
        /// 更改参数后需要 Stop 然后 Run 该通道。
        /// </summary>
        public string BackListenIp
        {
            get { return this.m_BackListenIp; }
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
                this.m_BackListenIp = value;
            }
        }

        private int m_BackMaxConnectNumber = 1000;
        /// <summary>
        /// 后台服务的最大连接数量，超出该数量的后续连接将被拒绝。
        /// 该值有效范围为 1 - 10000。默认值为 1000。
        /// 更改参数后需要重启该通道。
        /// </summary>
        public int BackMaxConnectNumber
        {
            get { return this.m_BackMaxConnectNumber; }
            set
            {
                if (value < 1)
                {
                    new Exception("后台服务最大连接数量不得小于 1");
                }
                else if (value > 10000)
                {
                    new Exception("后台服务最大连接数量不得大于 10000");
                }
                else
                {
                    this.m_BackMaxConnectNumber = value;
                }
            }
        }

        private int m_BackListenBacklog = 100;
        /// <summary>
        /// 后台服务挂起连接队列的最大长度。如果服务器处于忙碌状态未能及时响应后台的连接，则连接将被放入等待队列中，如果等待队列已满则后续连接将被拒绝。如果在同一时刻有大量的连接需要接入，则应将此值设置为较大值。
        /// 默认值为 100。
        /// 更改参数后需要重启该通道。
        /// </summary>
        public int BackListenBacklog
        {
            get { return this.m_BackListenBacklog; }
            set { this.m_BackListenBacklog = value; }
        }

        /// <summary>
        /// 设置或获取后台连接密码。
        /// </summary>
        public string BackConnectPassword { get; set; }

        /// <summary>
        /// 设置或获取后台管理密码。远程停止启动通道服务等操作需要使用管理密码，管理密码的权限高于连接密码。
        /// </summary>
        public string BackManagePassword { get; set; }

        #endregion

        #region << IChannelService 成员 >>

        /// <summary>
        /// 从保存此通道服务数据的 XML 文档节点初始化当前通道服务。
        /// </summary>
        /// <param name="channelServiceConfig">该对象节点的数据</param>
        public virtual void SetChannelServiceConfig(System.Xml.XmlNode channelServiceConfig)
        {
            foreach (System.Xml.XmlNode xnItem in channelServiceConfig.ChildNodes)
            {
                switch (xnItem.Name)
                {
                    case "BackListenPort":
                        this.BackListenPort = Function.ToInt(xnItem.InnerText);
                        break;

                    case "BackBufferSize":
                        this.BackBufferSize = Function.ToInt(xnItem.InnerText);
                        break;

                    case "BackListenIp":
                        this.BackListenIp = xnItem.InnerText;
                        break;

                    case "BackMaxConnectNumber":
                        this.BackMaxConnectNumber = Function.ToInt(xnItem.InnerText);
                        break;

                    case "BackListenBacklog":
                        this.BackListenBacklog = Function.ToInt(xnItem.InnerText);
                        break;

                    case "BackConnectPassword":
                        this.BackConnectPassword = xnItem.InnerText;
                        break;

                    case "BackManagePassword":
                        this.BackManagePassword = xnItem.InnerText;
                        break;

                }
            }
        }

        /// <summary>
        /// 获取当前通道服务的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public virtual System.Xml.XmlNode GetChannelServiceConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

            System.Xml.XmlNode xnBackListenPort = xmlDoc.CreateElement("BackListenPort");
            xnBackListenPort.InnerText = this.BackListenPort.ToString();
            xnConfig.AppendChild(xnBackListenPort);

            System.Xml.XmlNode xnBackBufferSize = xmlDoc.CreateElement("BackBufferSize");
            xnBackBufferSize.InnerText = this.BackBufferSize.ToString();
            xnConfig.AppendChild(xnBackBufferSize);

            if (this.BackListenIp != null && this.BackListenIp.Length > 0)
            {
                System.Xml.XmlNode xnBackListenIp = xmlDoc.CreateElement("BackListenIp");
                xnBackListenIp.InnerText = this.BackListenIp;
                xnConfig.AppendChild(xnBackListenIp);
            }

            System.Xml.XmlNode xnBackMaxConnectNumber = xmlDoc.CreateElement("BackMaxConnectNumber");
            xnBackMaxConnectNumber.InnerText = this.BackMaxConnectNumber.ToString();
            xnConfig.AppendChild(xnBackMaxConnectNumber);

            System.Xml.XmlNode xnBackListenBacklog = xmlDoc.CreateElement("BackListenBacklog");
            xnBackListenBacklog.InnerText = this.BackListenBacklog.ToString();
            xnConfig.AppendChild(xnBackListenBacklog);

            if (this.BackConnectPassword != null && this.BackConnectPassword.Length > 0)
            {
                System.Xml.XmlNode xnBackConnectPassword = xmlDoc.CreateElement("BackConnectPassword");
                xnBackConnectPassword.InnerText = this.BackConnectPassword;
                xnConfig.AppendChild(xnBackConnectPassword);
            }

            if (this.BackManagePassword != null && this.BackManagePassword.Length > 0)
            {
                System.Xml.XmlNode xnBackManagePassword = xmlDoc.CreateElement("BackManagePassword");
                xnBackManagePassword.InnerText = this.BackManagePassword;
                xnConfig.AppendChild(xnBackManagePassword);
            }

            return xnConfig;
        }

        /// <summary>
        /// 启动通道。该方法由通道服务程序或前置机程序作为宿主进行调用，调用该方法后通道应首先开启后台服务以便主控软件可以与该通道连接，然后开启设备端服务允许设备接入。
        /// </summary>
        public void Start()
        {
            if (this.State != ChannelServiceStateOptions.Running)
            {
                this.OnStateChanging(ChannelServiceStateOptions.Running);

                this.BackOpen();

                this.Open();

                this.State = ChannelServiceStateOptions.Running;
            }
        }

        /// <summary>
        /// 停止通道。该方法由通道服务程序或前置机程序作为宿主进行调用，调用该方法后通道应首先关闭设备端服务断开所有设备的连接，然后通知所有已连接的后台自身即将关闭的消息并关闭后台服务。对于通道安全运行的多种形式，如主备通道或多主通道，则由具体实现完成该功能。
        /// </summary>
        public void Stop()
        {
            if (this.State == ChannelServiceStateOptions.Running)
            {
                this.OnStateChanging(ChannelServiceStateOptions.Stopped);

                this.Close();

                this.BackClose();

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

        #endregion

        #region << 后台监听 >>

        //打开通道服务后台监听
        private void BackOpen()
        {
            if (this.BackListenPort > 0)
            {
                this.m_stkBackSocketAsync = new ConcurrentStack<SocketAsyncEventArgs>();
                this.m_stkBackBuffer = new ConcurrentStack<byte[]>();

                this.m_BackSocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.m_BackSocketServer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                this.m_BackSocketServer.SendBufferSize = this.BackBufferSize;
                this.m_BackSocketServer.ReceiveBufferSize = this.BackBufferSize;

                if (this.BackListenIp == null || this.BackListenIp.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length != 4)
                {
                    this.m_BackSocketServer.Bind(new IPEndPoint(System.Net.IPAddress.Any, this.BackListenPort));
                }
                else
                {
                    string[] strAddress = this.BackListenIp.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    byte[] bytAddress = { Byte.Parse(strAddress[0]), Byte.Parse(strAddress[1]), Byte.Parse(strAddress[2]), Byte.Parse(strAddress[3]) };
                    this.m_BackSocketServer.Bind(new IPEndPoint(new IPAddress(bytAddress), this.BackListenPort));
                }
                this.m_BackSocketServer.Listen(this.BackListenBacklog);

                this.OnMessage(ChannelServiceMessageTypeOptions.BackImportance, "后台监听地址及端口 " + this.m_BackSocketServer.LocalEndPoint.ToString());

                BackAccept();
            }
        }

        int intBackSocketId = 0;

        private void BackAccept()
        {
            intBackSocketId++;
            SocketAsyncEventArgs args = null;
            this.m_stkBackSocketAsync.TryPop(out args);
            if (args == null)
            {
                args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(BackSocketAsyncEventArgs_Completed);
            }

            if (args != null)
            {
                args.UserToken = intBackSocketId;
                if (this.m_BackSocketServer.AcceptAsync(args) == false)
                {
                    this.BackBeginAccept(args);
                }
            }
        }

        private void BackSocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs args)
        {
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    this.BackBeginAccept(args);
                    break;

                case SocketAsyncOperation.Receive:
                    this.BackBeginReceive(args);
                    break;

                default:
                    //什么情况下会执行到此处
                   // Console.WriteLine(args);
                    break;
            }
        }

        private void BackBeginAccept(SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    byte[] bytBuffer = null;
                    this.m_stkBackBuffer.TryPop(out bytBuffer);
                    if (bytBuffer == null)
                    {
                        bytBuffer = new byte[this.BackBufferSize];
                    }

                    if (bytBuffer != null)
                    {
                        args.SetBuffer(bytBuffer, 0, bytBuffer.Length);

                       // Console.WriteLine(args.AcceptSocket.RemoteEndPoint + "[" + args.UserToken + "]" + " 连接");
                        this.OnBackConnected(args.AcceptSocket.RemoteEndPoint);

                        //this.m_BackConnects.Add(args);

                        if (args.AcceptSocket.ReceiveAsync(args) == false)
                        {
                            this.BackBeginReceive(args);
                        }
                    }
                }
                else
                {
                   // Console.WriteLine(args.SocketError);

                    this.BackSocketAsyncFinish(args);
                }
            }
            catch (Exception e)
            {
               // Console.WriteLine(e);
            }
            finally
            {
                this.BackAccept();
            }
        }

        private void BackBeginReceive(SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                byte[] data = new byte[args.BytesTransferred];

                Array.Copy(args.Buffer, args.Offset, data, 0, data.Length);
               // Console.WriteLine(args.AcceptSocket.RemoteEndPoint + "[" + args.UserToken + "]" + " : " + BitConverter.ToString(data).Replace("-", " "));

                if (args.AcceptSocket.ReceiveAsync(args) == false)
                {
                    BackBeginReceive(args);
                }
            }
            else
            {
               // Console.WriteLine(args.AcceptSocket.RemoteEndPoint + "[" + args.UserToken + "]" + " 断线" + args.UserToken);
                this.OnBackDisconnected(args.AcceptSocket.RemoteEndPoint);

                this.BackSocketAsyncFinish(args);
            }
        }


        private void BackSocketAsyncFinish(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            if (args.Buffer != null)
            {
                this.m_stkBackBuffer.Push(args.Buffer);
                args.SetBuffer(null, 0, 0);
            }

            this.m_stkBackSocketAsync.Push(args);
        }

        //关闭通道服务后台监听
        private void BackClose()
        {
            this.m_stkBackSocketAsync.Clear();
            this.m_stkBackSocketAsync = null;

            this.m_stkBackBuffer.Clear();
            this.m_stkBackBuffer = null;
        }

        /// <summary>
        /// 通道异常。该方法由继承通道的类调用。
        /// 当通道因为某种原因（如网络断开、串口不存在）而不能继续正常工作时，由继承的类调用该方法，以便通知通道组进行主备通道切换。
        /// 继承的类调用该方法时应确保已经停止与设备端的连接。
        /// 调用该方法后无需再调用 Stop() 方法。
        /// </summary>
        protected void OnException()
        {
        }

        #endregion

        /// <summary>
        /// 打开设备侧端口并运行设备侧服务。
        /// </summary>
        protected abstract void Open();

        /// <summary>
        /// 关闭设备侧端口并停止设备侧服务。
        /// </summary>
        protected abstract void Close();

        #region << IBackConnectChannelService 成员 >>

        /// <summary>
        /// 新的后台连接后产生的事件。
        /// </summary>
        public event ChannelServiceConnectEventHandler BackConnected;

        private void OnBackConnected(System.Net.EndPoint endPoint)
        {
            if (this.BackConnected != null)
            {
                this.BackConnected(endPoint);
            }
        }

        /// <summary>
        /// 后台断开连接后产生的事件。
        /// </summary>
        public event ChannelServiceConnectEventHandler BackDisconnected;

        private void OnBackDisconnected(System.Net.EndPoint endPoint)
        {
            if (this.BackDisconnected != null)
            {
                this.BackDisconnected(endPoint);
            }
        }

        #endregion
    }
}
