using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace MainForm
{
    public class PortControl
    {
        public static int BIT_8 = 3;	  /* IOCTL : 8 data bits */
        public static int BIT_7 = 2;	  /* IOCTL : 7 data bits */
        public static int BIT_6 = 1;	  /* IOCTL : 6 data bits */
        public static int BIT_5 = 0;	  /* IOCTL : 5 data bits */

        public static int STOP_1 = 0;	  /* IOCTL : 1 stop bit      */
        public static int STOP_2 = 4;	  /* IOCTL : 2/1.5 stop bits */

        public static int P_NONE = 0;	  /* IOCTL : none parity	*/
        public static int P_EVEN = 8;	  /* IOCTL : even parity	*/
        public static int P_ODD = 16;	  /* IOCTL : odd parity	    */
        public static int P_MARK = 24;	  /* IOCTL : mark parity	*/
        public static int P_SPACE = 32;  /* IOCTL : space parity   */

        public static byte DCMD_IOCTL = 16;   /* IOCTL command	       */

        private static Dictionary<int, int> baudMap = new Dictionary<int, int>();
        private static Dictionary<int, int> comPort = new Dictionary<int, int>();
        private static Dictionary<int, int> dataPort = new Dictionary<int, int>();

        public static bool setPortBaudandMode(string IP, int CommandPort, int BaudRate, int Mode)
        {
            baudMap[1200] = 2;
            baudMap[2400] = 3;
            baudMap[4800] = 4;
            baudMap[7200] = 5;
            baudMap[9600] = 6;
            baudMap[19200] = 7;
            baudMap[38400] = 8;
            baudMap[57600] = 9;
            baudMap[115200] = 10;
            baudMap[230400] = 11;
            baudMap[460800] = 12;
            baudMap[921600] = 13;

            BaudRate = baudMap[BaudRate];

            //SocketData = connectSocket(IP, Port, 3000);
            //if (SocketData == null || !SocketData.Connected)
            //    return false;

            Socket socket = connectSocket(IP, CommandPort, 3000);
            if (socket == null || !socket.Connected)
                return false;

            byte[] sendBytes = new byte[16];
            sendBytes[0] = DCMD_IOCTL;
            sendBytes[1] = 2;
            sendBytes[2] = (byte)BaudRate;
            sendBytes[3] = (byte)Mode;
            if (socket.Send(sendBytes, 4, 0) != 4)
            {
                closeSocket(socket);
                return false;
            }
            Thread.Sleep(30);

            closeSocket(socket);
            return true;
        }

        private static Socket connectSocket(string address, int port, int timeout = 1000)
        {
            Socket s = null;
            try
            {
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(address), port);
                //创建Socket
                Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //设置Socket参数
                tempSocket.ReceiveBufferSize = 8192;       //8k
                tempSocket.ReceiveTimeout = timeout;       //ms
                tempSocket.SendBufferSize = 8192;	       //8k	
                tempSocket.SendTimeout = timeout;          //ms

                //连接Socket
                tempSocket.Connect(ipe);
                if (tempSocket.Connected)
                {
                    s = tempSocket;
                }
            }
            catch (Exception) { }
            return s;
        }

        private static void closeSocket(Socket s)
        {
            try
            {
                if (s != null)
                {
                    s.Shutdown(SocketShutdown.Both);
                    s.Disconnect(true);
                    s.Close();
                }
            }
            catch (Exception) { }
        }
    }
}
