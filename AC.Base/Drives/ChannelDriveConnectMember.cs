using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace AC.Base.Drives
{
    /// <summary>
    /// 后台所连接的通道服务成员。
    /// </summary>
    public class ChannelDriveConnectMember
    {
        private System.Collections.Generic.List<DriveSocket> m_DriveSockets = new System.Collections.Generic.List<DriveSocket>();
        //private System.Collections.Concurrent.BlockingCollection<DriveSocket>

        internal ChannelDriveConnectMember(ChannelDriveBase_ex channelDrive, ChannelServiceMemberInfo memberInfo)
        {
            this.ChannelDrive = channelDrive;
            this.MemberInfo = memberInfo;
        }

        /// <summary>
        /// 提供访问通道服务的供后台使用的驱动基类。
        /// </summary>
        public ChannelDriveBase_ex ChannelDrive { get; private set; }

        /// <summary>
        /// 通道服务成员信息。
        /// </summary>
        public ChannelServiceMemberInfo MemberInfo { get; private set; }

        /// <summary>
        /// 向通道服务成员发送指定命令的数据。
        /// </summary>
        /// <param name="sendData">数据。</param>
        /// <param name="delay">额外的延时。</param>
        /// <returns></returns>
        public byte[] SendData(byte[] sendData, int delay)
        {
            //如果尚未与通道服务成员连接，则首先使用主通道登录。
            if (this.m_DriveSockets.Count == 0)
            {
                if (this.MemberInfo.Addresses.Length > 0)
                {
                    foreach (IPAddress ipAddress in this.MemberInfo.Addresses)
                    {
                        try
                        {
                            IPEndPoint ipe = new IPEndPoint(ipAddress, this.MemberInfo.Port);
                            DriveSocket _DriveSocket = new DriveSocket(this, ipe);
                            //byte[] bytReceiveData = _DriveSocket.SendData(ChannelToChannelServiceCommandOptions.Connection, Convert.FromBase64String(this.MemberInfo.Password), delay);
                            //处理登录报文，如果登录通过则将 Socket 放入集合
                            this.m_DriveSockets.Add(_DriveSocket);
                            break;
                        }
                        catch
                        {
                        }
                    }

                    if (this.m_DriveSockets.Count == 0)
                    {
                        throw new CommunicationException("无法与通道成员 " + this.MemberInfo.ToString() + " 建立连接。");
                    }
                }
                else
                {
                    throw new CommunicationException("未配置通道服务成员 IP 地址。");
                }
            }

            if (this.m_DriveSockets.Count > 0)
            {
                DriveSocket _DriveSocket = this.m_DriveSockets[0];
                //return _DriveSocket.SendData(command, sendData, delay);
            }
            return null;
        }

        private class DriveSocket : System.Net.Sockets.Socket
        {
            private ChannelDriveConnectMember ConnectMember;

            public DriveSocket(ChannelDriveConnectMember connectMember, IPEndPoint ipEndPoint)
                : base(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                this.ConnectMember = connectMember;

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = ipEndPoint;
                args.Completed += new EventHandler<SocketAsyncEventArgs>(SocketAsyncEventArgs_Completed);

                if (this.ConnectAsync(args) == false)
                {
                    this.BackBeginConnect(args);
                }
            }

            private void BackBeginConnect(SocketAsyncEventArgs args)
            {
                byte[] bytBuffer = new byte[1024];
                args.SetBuffer(bytBuffer, 0, bytBuffer.Length);

                if (this.ReceiveAsync(args) == false)
                {
                    this.BackBeginReceive(args);
                }
            }

            private void SocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs args)
            {
                switch (args.LastOperation)
                {
                    case SocketAsyncOperation.Connect:
                        this.BackBeginConnect(args);
                        break;

                    case SocketAsyncOperation.Accept:
                       // Console.WriteLine(args);
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

            private void BackBeginReceive(SocketAsyncEventArgs args)
            {
                if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
                {
                    this.ReceiveData = new byte[args.BytesTransferred];

                    Array.Copy(args.Buffer, args.Offset, this.ReceiveData, 0, this.ReceiveData.Length);
                   // Console.WriteLine(this.LocalEndPoint + " " + BitConverter.ToString(this.ReceiveData).Replace("-", " "));

                    if (this.ReceiveAsync(args) == false)
                    {
                        BackBeginReceive(args);
                    }
                }
                else
                {
                   // Console.WriteLine(this.LocalEndPoint + " 断线" + args.UserToken);

                    if (args.Buffer != null)
                    {
                        args.SetBuffer(null, 0, 0);
                    }
                }
            }

            /// <summary>
            /// 该连接最后使用时间。
            /// </summary>
            public DateTime LastTime = DateTime.Now;

            /// <summary>
            /// 该连接当前是否可用。
            /// </summary>
            public bool IsFree = true;

            /// <summary>
            /// 最后接收到的数据。
            /// </summary>
            public byte[] ReceiveData;
        }
    }

    /// <summary>
    /// 后台所连接的通道服务成员集合。
    /// </summary>
    public class ChannelDriveConnectMemberCollection : IEnumerable<ChannelDriveConnectMember>
    {
        /// <summary>
        /// 存储对象数据的 List
        /// </summary>
        private System.Collections.Generic.List<ChannelDriveConnectMember> Items = new List<ChannelDriveConnectMember>();

        internal ChannelDriveConnectMemberCollection()
        {
        }

        #region IList<ChannelDriveConnectMember> 成员

        /// <summary>
        /// 将后台所连接的通道服务成员插入集合的指定索引处。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        internal void Insert(int index, ChannelDriveConnectMember item)
        {
            Items.Insert(index, item);
        }

        /// <summary>
        /// 移除集合的指定索引处的后台所连接的通道服务成员。
        /// </summary>
        /// <param name="index"></param>
        internal void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        #endregion

        #region ICollection<ChannelDriveConnectMember> 成员

        /// <summary>
        /// 将后台所连接的通道服务成员添加到集合的结尾处。
        /// </summary>
        /// <param name="item"></param>
        internal void Add(ChannelDriveConnectMember item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// 从集合中移除所有后台所连接的通道服务成员。
        /// </summary>
        internal void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// 获取一个值，该值指示后台所连接的通道服务成员集合是否为只读。
        /// </summary>
        internal bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// 从集合中移除特定后台所连接的通道服务成员的第一个匹配项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal bool Remove(ChannelDriveConnectMember item)
        {
            return Items.Remove(item);
        }

        #endregion

        #region IReadOnlyCollection<ChannelDriveConnectMember> 成员

        /// <summary>
        /// 搜索指定的后台所连接的通道服务成员，并返回整个集合中第一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(ChannelDriveConnectMember item)
        {
            return Items.IndexOf(item);
        }

        /// <summary>
        /// 获取指定索引处的后台所连接的通道服务成员。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ChannelDriveConnectMember this[int index]
        {
            get
            {
                return Items[index];
            }
            internal set
            {
                Items[index] = value;
            }
        }

        /// <summary>
        /// 确定某后台所连接的通道服务成员是否在集合中。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ChannelDriveConnectMember item)
        {
            return Items.Contains(item);
        }

        /// <summary>
        /// 将整个后台所连接的通道服务成员集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ChannelDriveConnectMember[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取集合中实际包含的后台所连接的通道服务成员数。
        /// </summary>
        public int Count
        {
            get { return Items.Count; }
        }

        #endregion

        #region IEnumerable<ChannelDriveConnectMember> 成员

        /// <summary>
        /// 返回循环访问后台所连接的通道服务成员集合的枚举数。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ChannelDriveConnectMember> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        /// <summary>
        /// 返回一个循环访问后台所连接的通道服务成员集合的枚举数。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion
    }
}
